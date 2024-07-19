import { range, sortBy } from "lodash-es";

import { KOOBOO_ID } from "@/views/inline-design/constants";
import type { KeyValue } from "./types";
import { isTag } from "@/utils/dom";

export const backgroundSizes: KeyValue[] = [
  {
    key: "auto",
    value: "auto",
  },
  {
    key: "cover",
    value: "cover",
  },
  {
    key: "contain",
    value: "contain",
  },
];

export const lengthUnits: KeyValue[] = [
  {
    key: "px",
    value: "px",
  },
  {
    key: "%",
    value: "%",
  },
  {
    key: "em",
    value: "em",
  },
];

export const backgroundRepeats: KeyValue[] = [
  {
    key: "repeat-x",
    value: "repeat-x",
  },
  {
    key: "repeat-y",
    value: "repeat-y",
  },
  {
    key: "repeat",
    value: "repeat",
  },
  {
    key: "space",
    value: "space",
  },
  {
    key: "round",
    value: "round",
  },
  {
    key: "no-repeat",
    value: "no-repeat",
  },
];

export const backgroundPositions: KeyValue[] = [
  {
    key: "center",
    value: "center",
  },
  {
    key: "top",
    value: "top",
  },
  {
    key: "left",
    value: "left",
  },
  {
    key: "right",
    value: "right",
  },
  {
    key: "bottom",
    value: "bottom",
  },
];

export const fontFamilies = [
  "PingFang SC-Regular",
  "PingFang SC",
  "Arial",
  "Arial-Regular",
  "Monaco",
  "Consolas",
  "Lucida Console",
  "Liberation Mono",
  "DejaVu Sans Mono",
  "Bitstream Vera Sans Mono",
  "Courier New",
  "monospace"!,
].sort();

export const tinymceFontSizes = [...range(12, 36, 2), 48, 56].map(
  (it) => `${it}px`
);

export const tinymceFonts = sortBy(
  [
    "Simsun=simsun,serif",
    "FangSong=FangSong,serif",
    "NSimSun=NSimSun,serif",
    "SimHei=SimHei,serif",
    "KaiTi=KaiTi,serif",
    "Microsoft YaHei=Microsoft YaHei,Helvetica Neue,PingFang SC,sans-serif",
    "LiSu=LiSu,serif",
    "YouYuan=YouYuan,serif",
    "STXihei=STXihei,serif",
    "STKaiti=STKaiti,serif",
    "STSong=STSong,serif",
    "Andale Mono=andale mono,times",
    "Arial=arial,helvetica,sans-serif",
    "Arial Black=arial black,avant garde",
    "Book Antiqua=book antiqua,palatino",
    "Comic Sans MS=comic sans ms,sans-serif",
    "Courier New=courier new,courier",
    "Georgia=georgia,palatino",
    "Helvetica=helvetica",
    "Impact=impact,chicago",
    "Symbol=symbol",
    "Tahoma=tahoma,arial,helvetica,sans-serif",
    "Terminal=terminal,monaco",
    "Times New Roman=times new roman,times",
    "Trebuchet MS=trebuchet ms,geneva",
    "Verdana=verdana,geneva",
    "Webdings=webdings",
    "Wingdings=wingdings,zapf dingbats",
  ],
  [(key) => key.toLowerCase()]
);

export const excludeValues = ["initial", "inherit", "none"];

export function isCssRule(rule: CSSRule) {
  return "selectorText" in rule;
}

export function isMediaRule(rule: CSSRule) {
  return "media" in rule;
}

export interface CssStyle {
  type: "embedded" | "file";
  url?: string;
  rules: CSSRuleList;
  owner: HTMLElement;
  koobooId?: string;
}

export function getStyles(doc: Document) {
  const result: CssStyle[] = [];

  for (const styleSheet of Array.from(doc.styleSheets)) {
    try {
      const el = styleSheet.ownerNode as HTMLElement;
      const url = el.getAttribute("href")!;
      if (url?.toLocaleLowerCase().startsWith("http")) continue;
      const koobooId = el.getAttribute(KOOBOO_ID)!;
      if (!koobooId) continue;

      result.push({
        type: isTag(styleSheet.ownerNode, "link") ? "file" : "embedded",
        url,
        owner: el,
        rules: styleSheet.cssRules,
        koobooId: koobooId,
      });
    } catch (error) {
      // external style
    }
  }

  return result;
}

export function getStyleRules(el: HTMLElement) {
  const result: CSSStyleRule[] = [];
  const styles = getStyles(el.ownerDocument);

  for (const style of styles) {
    for (const i of Array.from(style.rules)) {
      if (isCssRule(i)) {
        const rule = i as CSSStyleRule;
        try {
          if (el.matches(rule.selectorText)) {
            result.push(rule);
          }
        } catch (error) {
          console.log(`matches selectorText error ${rule.selectorText!}`);
        }
      } else if (isMediaRule(i)) {
        const mediaRule = i as CSSMediaRule;

        for (const cssRule of Array.from(mediaRule.cssRules ?? [])) {
          if (!isCssRule(cssRule)) continue;
          const rule = cssRule as CSSStyleRule;
          try {
            if (el.matches(rule.selectorText)) {
              result.push(rule);
            }
          } catch (error) {
            console.log(`matches selectorText error ${rule.selectorText!}`);
          }
        }
      }
    }
  }

  return result;
}

export function getPriority(el: HTMLElement, property: string) {
  const rules = getStyleRules(el);
  for (const rule of rules) {
    const priority = rule.style.getPropertyPriority(property);
    if (priority) return priority;
  }
}
