import { KOOBOO_ID } from "@/common/constants";
import { colorProps, ColorProp } from "./colorProps";
import { sortStylePriority } from "./stylePriority";
import { colorEnum } from "./colorEnum";
import { KoobooComment } from "@/kooboo/KoobooComment";
import { getEditableComment } from "@/components/floatMenu/utils";

const pseudoes = ["visited", "hover", "active", "focus"];

type rule = { cssRule: CSSStyleRule; mediaRuleList?: string; url?: string };

export function getStyles() {
  let styles: CSSStyleSheet[] = [];
  for (const style of document.styleSheets as any) {
    if (!style || !(style instanceof CSSStyleSheet)) continue;
    if (style.media && style.media.mediaText && !matchMedia(style.media.mediaText).matches) continue;
    styles.push(style);
  }
  return styles;
}

export function getRules(style: CSSStyleSheet, href?: string | null) {
  let rules: rule[] = [];
  try {
    for (const rule of style.rules as any) {
      if (rule instanceof CSSStyleRule) rules.push({ cssRule: rule, url: href! });
      else if (rule instanceof CSSMediaRule && matchMedia(rule.media.mediaText).matches) {
        for (const cssRule of rule.cssRules as any) {
          rules.push({
            cssRule: cssRule as CSSStyleRule,
            mediaRuleList: rule.media.mediaText,
            url: href!
          });
        }
      } else if (rule instanceof CSSImportRule) {
        rules.push(...getRules(rule.styleSheet, rule.styleSheet.href));
      }
    }
    // eslint-disable-next-line no-empty
  } catch (error) {}

  return rules;
}

export function getCssRules() {
  let cssRules: CssRule[] = [];
  let styleSequence = 0;
  for (const style of getStyles()) {
    if (!(style.ownerNode instanceof HTMLElement)) continue;
    let comments = KoobooComment.getComments(style.ownerNode);
    let comment = getEditableComment(comments);
    if (!comment) continue;
    let koobooId = style.ownerNode.getAttribute(KOOBOO_ID);
    let href = style.ownerNode.getAttribute("href");
    if ((!koobooId && !href) || (href && href.startsWith("http") && !href.startsWith(location.origin))) continue;
    for (const { cssRule, mediaRuleList, url } of getRules(style, href)) {
      if (!cssRule || !(cssRule instanceof CSSStyleRule)) continue;
      cssRules.push({
        styleSequence: ++styleSequence,
        koobooId,
        url: url,
        cssRule,
        mediaRuleList
      });
    }
  }
  return cssRules;
}

export interface CssRule {
  styleSequence: number;
  koobooId: string | null;
  url: string | undefined;
  cssRule: CSSStyleRule;
  mediaRuleList: string | undefined;
}

export interface CssColor {
  prop: ColorProp;
  value: string;
  newValue?: string;
  newImportant?: string;
  styleSequence: number;
  important: boolean;
  inline: boolean;
  rawSelector: string;
  targetSelector: string;
  url: string | null;
  koobooId: string | null;
  pseudo: string | null;
  cssStyleRule: CSSStyleRule | null;
  mediaRuleList: string | null;
}

export interface CssColorGroup {
  prop: string;
  pseudo: string | null;
  cssColors: CssColor[];
}

export function getMatchedColorGroups(el: HTMLElement) {
  let groups: CssColorGroup[] = [];
  let colors = getMatchedColors(el);
  for (const i of colors) {
    let item = groups.find(f => f.prop == i.prop.parent && f.pseudo == i.pseudo);
    if (item) {
      item.cssColors.push(i);
    } else {
      groups.push({
        prop: i.prop.parent,
        cssColors: [i],
        pseudo: i.pseudo
      });
    }
  }
  for (const i of groups) {
    i.cssColors = sortStylePriority(i.cssColors).reverse();
  }

  // 如果优先级最高的cssColor没有明确指定颜色则移除
  for (const item of [...groups]) {
    let color = item.cssColors[0].prop.getColor(item.cssColors[0].value);
    if (isOneColor(color)) {
      continue;
    }

    let index = groups.findIndex(i => i == item);
    if (index < 0) {
      continue;
    }
    groups = groups.filter((gi, gindex) => gindex != index);
  }

  let style = getComputedStyle(el);
  addDefaultColor(groups, "color", style.color!);
  addDefaultColor(groups, "background-color", style.backgroundColor!);
  return groups;
}

export function getMatchedColors(el: HTMLElement) {
  let matchedColors: CssColor[] = [];
  addInlineMatchedColors(el, matchedColors);
  addStyleMatchedColors(el, matchedColors);
  return matchedColors;
}

export function addInlineMatchedColors(el: HTMLElement, matchedColors: CssColor[]) {
  let inlineColors = getCssColors(el.style);
  for (const i of inlineColors) {
    matchedColors.push({
      prop: i.prop,
      styleSequence: Number.MAX_VALUE / 2,
      important: i.important,
      inline: true,
      koobooId: "",
      rawSelector: "",
      targetSelector: "",
      url: "",
      value: i.value,
      pseudo: "",
      cssStyleRule: null,
      mediaRuleList: null
    });
  }
}

function addStyleMatchedColors(el: HTMLElement, matchedColors: CssColor[]) {
  for (const i of getCssRules()) {
    let matcheds = matchSelector(el, i.cssRule.selectorText);
    for (const matched of matcheds) {
      let colors = getCssColors(i.cssRule.style);
      for (const color of colors) {
        matchedColors.push({
          prop: color.prop,
          styleSequence: i.styleSequence,
          important: color.important,
          inline: false,
          koobooId: i.koobooId,
          rawSelector: i.cssRule.selectorText,
          targetSelector: matched.selector,
          url: i.url!,
          value: color.value,
          pseudo: matched.pseudo,
          cssStyleRule: i.cssRule,
          mediaRuleList: i.mediaRuleList!
        });
      }
    }
  }
}

export function getCssColors(style: CSSStyleDeclaration) {
  let colors = [];
  for (const i of colorProps) {
    let value = style.getPropertyValue(i.prop);
    if (!value) continue;
    colors.push({
      prop: i,
      value,
      important: !!style.getPropertyPriority(i.prop)
    });
  }
  return colors;
}

// 是否是单个颜色（有的时候color是 #fff #fff 多个颜色组成）
export function isOneColor(color: string): boolean {
  color = color.trim().toLowerCase();
  if (/^(#|rgb)((?!(#|rgb)).)*$/g.test(color)) return true;
  if (colorEnum.hasOwnProperty(color)) return true;
  return false;
}

export function splitPseudo(selector: string) {
  for (const i of pseudoes) {
    // eslint-disable-next-line no-useless-escape
    let pseudo = selector.match(`:+${i}\s*$`);
    if (!pseudo) continue;
    return {
      pseudo: pseudo[0],
      selector: selector.replace(pseudo[0], "")
    };
  }
}

export function matchSelector(el: HTMLElement, selector: string) {
  let matcheds = [];
  var selectors = selector.split(",");
  for (const i of selectors) {
    let splited = splitPseudo(i);
    try {
      if (splited && el.matches(splited.selector)) {
        matcheds.push({
          selector: i,
          pseudo: splited.pseudo
        });
      } else if (el.matches(i)) {
        matcheds.push({
          selector: i,
          pseudo: ""
        });
      }
      // eslint-disable-next-line no-empty
    } catch (error) {}
  }
  return matcheds;
}

export function addDefaultColor(groups: CssColorGroup[], prop: string, value: string) {
  let colorProp = colorProps.find(f => f.prop == prop)!;
  if (!groups.some(e => e.prop == colorProp.parent && !e.pseudo)) {
    groups.push({
      pseudo: "",
      prop: colorProp.parent,
      cssColors: [
        {
          inline: true,
          prop: colorProp,
          cssStyleRule: null,
          important: false,
          koobooId: "",
          pseudo: "",
          rawSelector: "",
          styleSequence: 0,
          targetSelector: "",
          url: "",
          value: value,
          mediaRuleList: null
        }
      ]
    });
  }
}
