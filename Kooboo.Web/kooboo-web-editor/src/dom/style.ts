import { KOOBOO_ID } from "@/common/constants";
import { colorProps, ColorProp } from "./colorProps";
import { sortStylePriority } from "./stylePriority";
import { colorEnum } from "./colorEnum";
import { KoobooComment } from "@/kooboo/KoobooComment";
import { getViewComment } from "@/components/floatMenu/utils";

const pseudoes = ["visited", "hover", "active", "focus"];
// var cantMatchedPseudoes = [":+\\s*visited$", ":+\\s*hover$", ":+\\s*active$", ":+\\s*focus$"];

function* getStyles() {
  for (let i = 0; i < document.styleSheets.length; i++) {
    yield document.styleSheets.item(i);
  }
}

function* getRules(style: CSSStyleSheet) {
  for (let i = 0; i < style.rules.length; i++) {
    yield style.rules.item(i);
  }
}

export function* getCssRules() {
  let styleSequence = 0;
  for (const style of getStyles()) {
    if (!style || !(style instanceof CSSStyleSheet) || !(style.ownerNode instanceof HTMLElement)) continue;
    let comments = KoobooComment.getComments(style.ownerNode);
    let comment = getViewComment(comments);
    if (!comment) continue;
    let koobooId = style.ownerNode.getAttribute(KOOBOO_ID);
    let href = style.ownerNode.getAttribute("href");
    if ((!koobooId && !href) || (href && href.startsWith("http"))) continue;
    for (const rule of getRules(style)) {
      if (!rule || !(rule instanceof CSSStyleRule)) continue;
      yield {
        styleSequence: ++styleSequence,
        koobooId,
        url: href,
        rule,
        nameorid: comment.nameorid,
        objecttype: comment.objecttype
      };
    }
  }
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
  nameorid: string | undefined;
  objecttype: string | undefined;
  url: string | null;
  koobooId: string | null;
  pseudo: string | null;
  cssStyleRule: CSSStyleRule | null;
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
  for(const item of [...groups]){
    let color = item.cssColors[0].prop.getColor(item.cssColors[0].value);
    if(isColor(color)){
      continue;
    }

    let index = groups.findIndex(i=>i==item);
    if(index < 0){
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

function addInlineMatchedColors(el: HTMLElement, matchedColors: CssColor[]) {
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
      nameorid: undefined,
      objecttype: undefined,
      url: "",
      value: i.value,
      pseudo: "",
      cssStyleRule: null
    });
  }
}

function addStyleMatchedColors(el: HTMLElement, matchedColors: CssColor[]) {
  for (const i of getCssRules()) {
    let matched = matchSelector(el, i.rule.selectorText);
    if (!matched) continue;
    let colors = getCssColors(i.rule.style);
    if (!colors) continue;
    for (const color of colors) {
      matchedColors.push({
        prop: color.prop,
        styleSequence: i.styleSequence,
        important: color.important,
        inline: false,
        koobooId: i.koobooId,
        rawSelector: i.rule.selectorText,
        targetSelector: matched.selector,
        url: i.url,
        value: color.value,
        pseudo: matched.pseudo,
        cssStyleRule: i.rule,
        nameorid: i.nameorid,
        objecttype: i.objecttype
      });
    }
  }
}

function getCssColors(style: CSSStyleDeclaration) {
  let colors = [];
  for (const i of colorProps) {
    let value = style.getPropertyValue(i.prop);
    if (!value || style.cssText.indexOf(i.prop) == -1) continue;
    // let color = i.getColor(value);
    // if (!isColor(color)) continue;
    colors.push({
      prop: i,
      value,
      important: !!style.getPropertyPriority(i.prop)
    });
  }
  return colors;
}

function isColor(color: string) {
  color = color.trim().toLowerCase();
  if (color.startsWith("#")) return true;
  if (color.startsWith("rgb")) return true;
  for (const key in colorEnum) {
    if (key == color) return true;
  }
}

function splitPseudo(selector: string) {
  for (const i of pseudoes) {
    let pseudo = selector.match(`:+${i}\s*$`);
    if (!pseudo) continue;
    return {
      pseudo: pseudo[0],
      selector: selector.replace(pseudo[0], "")
    };
  }
}

function matchSelector(el: HTMLElement, selector: string) {
  var selectors = selector.split(",");
  for (const i of selectors) {
    let splited = splitPseudo(i);
    if (splited && el.matches(splited.selector)) {
      return {
        selector: i,
        pseudo: splited.pseudo
      };
    } else if (el.matches(i)) {
      return {
        selector: i,
        pseudo: ""
      };
    }
  }
}

function addDefaultColor(groups: CssColorGroup[], prop: string, value: string) {
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
          nameorid: undefined,
          objecttype: undefined
        }
      ]
    });
  }
}
