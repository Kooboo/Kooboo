import { KOOBOO_ID } from "@/common/constants";

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

interface cssRule {
  url: string;
  koobooId: string;
  selector: string;
  text: string;
}

export function getElementCssRules(el: HTMLElement) {
  for (const style of getStyles()) {
    if (!style || !(style instanceof CSSStyleSheet) || !(style.ownerNode instanceof HTMLElement)) continue;
    let koobooId = style.ownerNode.getAttribute(KOOBOO_ID);
    let href = style.ownerNode.getAttribute("href");
    if (!koobooId) continue;
    if (href && href.startsWith("http")) continue;
    for (const rule of getRules(style)) {
      if (!rule || !(rule instanceof CSSStyleRule)) continue;
      if (el.matches(rule.selectorText)) {
        if (!rule.style.color) continue;
        console.log(rule, koobooId, href);
      }
    }
  }
}
