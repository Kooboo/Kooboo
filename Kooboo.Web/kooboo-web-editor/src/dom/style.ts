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
  url: string | null;
  koobooId: string;
  rule: CSSStyleRule;
}

export function getElementCssRules(el: HTMLElement) {
  let rules: cssRule[] = [];
  for (const style of getStyles()) {
    if (!style || !(style instanceof CSSStyleSheet) || !(style.ownerNode instanceof HTMLElement)) continue;
    let koobooId = style.ownerNode.getAttribute(KOOBOO_ID);
    let href = style.ownerNode.getAttribute("href");
    if (!koobooId) continue;
    if (href && href.startsWith("http")) continue;
    for (const rule of getRules(style)) {
      if (!rule || !(rule instanceof CSSStyleRule)) continue;
      if (el.matches(rule.selectorText)) {
        rules.push({
          koobooId,
          url: href,
          rule
        });
      }
    }
  }
  return rules;
}
