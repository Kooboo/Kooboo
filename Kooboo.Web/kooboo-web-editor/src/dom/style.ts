import { KOOBOO_ID } from "@/common/constants";
import { colorProps, ColorProp } from "./colorProps";
import { sortStylePriority } from "./stylePriority";

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
    let koobooId = style.ownerNode.getAttribute(KOOBOO_ID);
    let href = style.ownerNode.getAttribute("href");
    if (!koobooId) continue;
    if (href && href.startsWith("http")) continue;
    for (const rule of getRules(style)) {
      if (!rule || !(rule instanceof CSSStyleRule)) continue;
      yield {
        styleSequence: ++styleSequence,
        koobooId,
        url: href,
        rule
      };
    }
  }
}

export interface CssColor {
  prop: ColorProp;
  value: string;
  styleSequence: number;
  important: boolean;
  inline: boolean;
  rawSelector: string;
  targetSelector: string;
  url: string | null;
  koobooId: string;
  pseudo: string | null;
  cssStyleRule: CSSStyleRule | null;
}

interface CssColorGroup {
  prop: string;
  pseudo: string | null;
  cssColors: CssColor[];
}

export function getMatchedColorGroups(el: HTMLElement) {
  let group: CssColorGroup[] = [];
  let colors = getMatchedColors(el);
  for (const i of colors) {
    let item = group.find(f => f.prop == i.prop.parent && f.pseudo == i.pseudo);
    if (item) {
      item.cssColors.push(i);
    } else {
      group.push({
        prop: i.prop.parent,
        cssColors: [i],
        pseudo: i.pseudo
      });
    }
  }
  for (const i of group) {
    i.cssColors = sortStylePriority(i.cssColors).reverse();
  }
  return group;
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
        cssStyleRule: i.rule
      });
    }
  }
}

function getCssColors(style: CSSStyleDeclaration) {
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
