import { CssColor } from "./style";

export const getStylePriority = function(styleSelector: CssColor) {
  const TOKEN_ID = /(#[\w-]+)/g;
  const TOKEN_CLASS = /(\.[\w-]+)/g;
  const TOKEN_ATTR = /(\[[^[\]]+])/g;
  const TOKEN_PSEUDO_CLASS = /(:[\w-]+)/g;
  const TOKEN_PSEUDO_ELEM = /(::[\w-]+)/g;
  const TOKEN_ELEM = /([\w-]+)/g;
  const PSEUDO_ELEMS = ["first-letter", "last-letter", "first-line", "last-line", "first-child", "last-child", "before", "after"];
  const SYMBOL_B = "\x02";
  const SYMBOL_C = "\x03";
  const SYMBOL_D = "\x04";
  // priority: [ important, style, R1, R2, R3, line ]
  let important = styleSelector.important ? 1 : 0;
  let inline = styleSelector.inline ? 1 : 0;
  let styleSequence = styleSelector.styleSequence || 0;
  let child = styleSelector.prop ? (styleSelector.prop.isChild ? 1 : 0) : 0;
  let priority = [important, inline, 0, 0, 0, child, styleSequence];
  let selector = styleSelector.targetSelector;
  selector = selector.replace(/(::?)([\w-]+)/g, function(total, left, elem) {
    if (PSEUDO_ELEMS.indexOf(elem) >= 0) {
      if (left === ":") {
        return "::" + elem;
      } else {
        return total;
      }
    } else {
      return total;
    }
  });
  // replace with symbols
  selector = selector
    .replace(TOKEN_ATTR, SYMBOL_C)
    .replace(TOKEN_PSEUDO_ELEM, SYMBOL_D)
    .replace(TOKEN_PSEUDO_CLASS, SYMBOL_C)
    .replace(TOKEN_ID, SYMBOL_B)
    .replace(TOKEN_CLASS, SYMBOL_C)
    .replace(TOKEN_ELEM, SYMBOL_D);
  // count
  selector = selector.replace(/[\2\3\4]/g, function(symbol) {
    var idx = symbol.charCodeAt(0);
    priority[idx]++;
    return "<" + idx + ">";
  });
  return priority;
};
//返回0为a等于b；1为ada于b，-1为a小于b
export const compareStylePriority = function(a: Array<number>, b: Array<number>) {
  for (var i = 0; i < a.length; i++) {
    if (a[i] !== b[i]) {
      return a[i] - b[i];
    }
  }
  return 0;
};

export const sortStylePriority = function(styleSelectorList: CssColor[]) {
  styleSelectorList.sort((a, b) => {
    let priorityA = getStylePriority(a);
    let priorityB = getStylePriority(b);

    return compareStylePriority(priorityA, priorityB);
  });
  return styleSelectorList;
};
