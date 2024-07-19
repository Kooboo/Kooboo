import type { Meta, PaddingOptions, VeWidgetPropDefine } from "../types";
import { isEmpty, kebabCase, pick, pickBy } from "lodash-es";

import { generateCssText } from "@/utils/dom";
import { getContainerPropDefines } from "../utils/prop-define";
import { ignoreCaseContains } from "@/utils/string";

function getValidProps(
  props: Record<string, any>,
  keys?: string[]
): Record<string, any> {
  if (!keys || isEmpty(keys)) {
    keys = Object.keys(props);
  }
  return pickBy(pick(props, keys), (value) => value);
}

const sizeProps = ["width", "height"];

function getStyleValue(props: Record<string, any>, key: string) {
  const value = props[key];
  if (
    typeof value === "number" &&
    (sizeProps.includes(key) ||
      key.startsWith("padding-") ||
      key.startsWith("margin-"))
  ) {
    return `${value}px`;
  }
  if (key === "background-image" && value && !value.startsWith("url(")) {
    return `url('${value}')`;
  }

  return value;
}

export function setStyles(
  el: HTMLElement,
  props: Record<string, any>,
  keys?: string[]
) {
  const attrs = getValidProps(props, keys);
  for (const key in attrs) {
    const value = getStyleValue(attrs, key);
    el.style.setProperty(kebabCase(key), value + "");
  }
}

export function setAttributes(
  el: HTMLElement,
  props: Record<string, any>,
  keys?: string[]
) {
  const attrs = getValidProps(props, keys);
  for (const key in attrs) {
    el.setAttribute(key, attrs[key]);
  }
}

export function getContentWidth(meta: Meta, containerWidth: number) {
  const margin = parseSides(meta.props.veContainerMargin);
  const padding = parseSides(meta.props.veContainerPadding);
  const contentWidth =
    containerWidth -
    margin.left -
    margin.right -
    padding.left -
    padding.right -
    (meta.props.veContainerBorderWidth?.value ?? 0) * 2;
  return contentWidth;
}

export function parseSides(value: string) {
  if (typeof value !== "string" || !value) {
    return { top: 0, right: 0, bottom: 0, left: 0 };
  }

  const json: PaddingOptions = JSON.parse(value);
  if (!json.moreOptions) {
    return {
      top: json.all ?? 0,
      right: json.all ?? 0,
      bottom: json.all ?? 0,
      left: json.all ?? 0,
    };
  }
  return {
    top: json.top ?? 0,
    right: json.right ?? 0,
    bottom: json.bottom ?? 0,
    left: json.left ?? 0,
  };
}

export function getSides(
  prefix: string,
  value: string
): Record<string, string> {
  if (typeof value !== "string" || !value) {
    return getSideStyles(prefix, { moreOptions: false, all: 0 });
  }
  const json: PaddingOptions = JSON.parse(value);

  return getSideStyles(prefix, json);
}

export function setSides(prefix: string, el: HTMLElement, value: string) {
  const styles = getSides(prefix, value);
  setStyles(el, styles, Object.keys(styles));
}

export function setContainerStyles(el: HTMLElement, meta: Meta) {
  const containerProps = getContainerPropDefines(meta);
  const containerStyles = toStyles(
    meta.props,
    containerProps,
    containerPropMaps
  );
  setStyles(el, containerStyles);
  if (ignoreCaseContains(["divider", "spacer"], meta.name)) {
    el.style.fontSize = "1px";
  }
  el.style.setProperty("display", "flex");
  return el;
}

export function getContainerStyles(meta: Meta): any {
  const el = document.createElement("div");
  setContainerStyles(el, meta);
  return el.getAttribute("style");
}

export function getLinkStyles(metaProps: Record<string, any>): {
  link: Record<string, string>;
  hover: Record<string, string>;
} {
  const styles = {
    link: {},
    hover: {},
  };
  const props = pick(metaProps, [
    "linkColor",
    "linkBackgroundColor",
    "linkHoverColor",
    "linkHoverBackgroundColor",
    "linkUnderline",
    "linkHoverUnderline",
  ]);
  if (isEmpty(props)) {
    return styles;
  }

  const linkStyles: Record<string, string> = {
    color: props["linkColor"],
    "background-color": props["linkBackgroundColor"],
    "text-decoration": props["linkUnderline"] ? "underline" : "none",
  };
  const linkHoverStyles: Record<string, string> = {
    color: props["linkHoverColor"],
    "background-color": props["linkHoverBackgroundColor"],
    "text-decoration": props["linkHoverUnderline"] ? "underline" : "none",
  };

  return {
    link: linkStyles,
    hover: linkHoverStyles,
  };
}

export function generateLinkStyles(
  metaProps: Record<string, any>,
  prefix?: string
): string[] {
  const { link, hover } = getLinkStyles(metaProps);
  const styles: string[] = [];
  if (isEmpty(link) && isEmpty(hover)) {
    return styles;
  }

  const selector = prefix ? prefix + " " : "";

  const linkCssText = generateCssText(link);
  if (linkCssText) {
    styles.push(`${selector}a {${linkCssText}}`);
  }

  const linkHoverCssText = generateCssText(hover);
  if (linkHoverCssText) {
    styles.push(`${selector}a:hover {${linkHoverCssText}}`);
  }

  return styles;
}

export function setLinkStyles(el: HTMLElement, meta: Meta, prefix: string) {
  const linkStyles = generateLinkStyles(meta.props, prefix);
  if (isEmpty(linkStyles)) {
    return;
  }
  const style = document.createElement("style");
  style.innerText = linkStyles.join(" ");
  el.appendChild(style);
}

function getPropValue(
  key: string,
  props: Record<string, any>,
  def?: VeWidgetPropDefine
) {
  const value = props[key] === undefined ? def?.defaultValue : props[key];

  if (value) {
    switch (def?.controlType) {
      case "NumberUnit":
        return `${value.value}${value.unit}`;
      case "VeTwoSelection":
        return value.join(" ");
      default:
        break;
    }
  }

  if (
    ["width", "height", "fontSize", "borderWidth", "letterSpacing"].includes(
      key
    ) &&
    Number.isInteger(value)
  ) {
    return `${value}px`;
  }

  if (key === "backgroundImage" && value) {
    return `url('${value}')`;
  }

  if (key === "fontFamily" && value) {
    return value + ""; // Array to string
  }

  return value;
}
export const containerPropMaps: Record<string, string> = {
  veContainerBackgroundColor: "background-color",
  veContainerBackgroundImage: "background-image",
  veContainerBackgroundRepeat: "background-repeat",
  veContainerBackgroundPosition: "background-position",
  veContainerJustifyContent: "justify-content",
  veContainerBorderColor: "border-color",
  veContainerBorderRadius: "border-radius",
  veContainerBorderStyle: "border-style",
  veContainerBorderWidth: "border-width",
  veContainerFontSize: "font-size",
};

function getSideStyles(
  prefix: string,
  json: PaddingOptions
): Record<string, string> {
  if (json.moreOptions) {
    return {
      [`${prefix}-top`]: json.top ? json.top + "px" : "0",
      [`${prefix}-right`]: json.right ? json.right + "px" : "0",
      [`${prefix}-bottom`]: json.bottom ? json.bottom + "px" : "0",
      [`${prefix}-left`]: json.left ? json.left + "px" : "0",
    };
  }
  const all = json.all ? json.all + "px" : "0";
  return {
    [`${prefix}-top`]: all,
    [`${prefix}-right`]: all,
    [`${prefix}-bottom`]: all,
    [`${prefix}-left`]: all,
  };
}

function handleSpecialProps(
  props: Record<string, any>,
  styles: Record<string, string>,
  def?: VeWidgetPropDefine
): boolean {
  if (!def) return false;

  const value = props[def.name];
  if (def.controlType === "side-control") {
    const json: PaddingOptions = value ? JSON.parse(value) : { all: 0 };
    const prefix = def.name.toLowerCase().includes("margin")
      ? "margin"
      : "padding";
    const sideStyles = getSideStyles(prefix, json);
    for (const key in sideStyles) {
      if (Object.prototype.hasOwnProperty.call(sideStyles, key)) {
        styles[key] = sideStyles[key];
      }
    }

    return true;
  }

  return false;
}

// Convert meta props to css styles
export function toStyles(
  props: Record<string, any>,
  defines: VeWidgetPropDefine[],
  propMaps?: Record<string, string>
) {
  const style = document.body.style;
  const result: Record<string, string> = {};
  for (const key in props) {
    if (!Object.prototype.hasOwnProperty.call(props, key)) {
      continue;
    }
    const def = defines.find((it) => it.name === key);
    if (!def) {
      continue;
    }
    const handled = handleSpecialProps(props, result, def);
    if (handled) {
      continue;
    }

    let styleKey = key;
    const value = getPropValue(key, props, def);
    if (propMaps && propMaps[key]) {
      styleKey = propMaps[key];
    }
    const kebabKey = kebabCase(styleKey);
    if (style.getPropertyValue(kebabKey) !== undefined) {
      result[kebabKey] = value;
    }
  }

  return result;
}

export const classicStyles: Record<string, string> = {
  "-ms-text-size-adjust": "100%",
  "-webkit-text-size-adjust": "100%",
  margin: "0 auto",
  padding: "0",
  "border-spacing": "0",
};

export function getTableAlign(alignItems?: string) {
  if (!alignItems) {
    return undefined;
  }

  if (alignItems.includes("start")) {
    return "left";
  }
  if (alignItems.includes("end")) {
    return "right";
  }

  return "center";
}

export function createTd(): HTMLTableCellElement {
  const td = document.createElement("td");
  td.setAttribute(
    "style",
    "-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;margin: 0;padding: 0;mso-table-lspace: 0pt;mso-table-rspace: 0pt"
  );
  td.style.color = "inherit"; // QQ邮箱（.readmail_content_html td）会把颜色给覆盖掉
  td.style.fontFamily = "inherit";
  td.style.fontWeight = "inherit";
  return td;
}

export function createTr(globalWidth: number | string): HTMLTableRowElement {
  const tr = document.createElement("tr");
  const width =
    typeof globalWidth === "number"
      ? `${globalWidth.toFixed()}px`
      : globalWidth;
  setStyles(tr, {
    "-ms-text-size-adjust": "100%",
    "-webkit-text-size-adjust": "100%",
    margin: "0",
    padding: "0",
    width,
  });
  return tr;
}

export function createTBody(
  globalWidth: number | string
): HTMLTableSectionElement {
  const tbody = document.createElement("tbody");
  const width =
    typeof globalWidth === "number"
      ? `${globalWidth.toFixed()}px`
      : globalWidth;
  setStyles(tbody, {
    "-ms-text-size-adjust": "100%",
    "-webkit-text-size-adjust": "100%",
    margin: "0",
    padding: "0",
    width: width,
  });
  return tbody;
}

export function createTable(): HTMLTableElement {
  const table = document.createElement("table");
  setAttributes(table, {
    cellspacing: "0",
    cellpadding: "0",
    border: "0",
  });
  setStyles(table, {
    "-ms-text-size-adjust": "100%",
    "-webkit-text-size-adjust": "100%",
    "mso-table-lspace": "0pt",
    "mso-table-rspace": "0pt",
    "border-collapse": "collapse",
    "border-spacing": "0",
    color: "inherit",
    "font-family": "inherit",
    "font-weight": "inherit",
  });
  return table;
}

export function encodeStyleQuote(el: HTMLElement): void {
  // 双引号会被转义成&quote;后端统计不到引用关系，所以这里需要替换成单引号
  const style = el.getAttribute("style")?.replaceAll(`"`, `'`);
  if (style) {
    el.setAttribute("style", style);
  }
}
