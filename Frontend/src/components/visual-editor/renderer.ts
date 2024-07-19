import type { Meta, NumberUnit } from "./types";
import { isEmpty, kebabCase } from "lodash-es";

import { encodeStyleQuote } from "./render/utils";
import { isRow } from "./utils/widget";
import { render } from "./render";

async function renderClassicSection(section: Meta, rootMeta: Meta) {
  const rows: Meta[] = section.children;
  const rowBuilder: string[] = [];
  for (const row of rows) {
    if (!isRow(row)) {
      continue;
    }

    const rowHtml = await render(row, true, rootMeta);
    rowBuilder.push(rowHtml);
  }

  return rowBuilder.join("");
}

async function renderModernSection(section: Meta, rootMeta: Meta) {
  const rows: Meta[] = section.children;
  const rowBuilder: string[] = [];
  for (const row of rows) {
    if (!isRow(row)) {
      continue;
    }
    const rowHtml = await render(row, false, rootMeta);
    rowBuilder.push(rowHtml);
  }

  return rowBuilder.join("");
}

function normalizeStyle(html: string): string {
  const el = document.createElement("div");
  el.innerHTML = html;

  // https://www.w3.org/TR/css-syntax-3/#consume-url-token
  // 浏览器解析时，会自动将url()中的双引号转义为&quot;
  // 根据W3规范，url() 值支持单引号或双引号，不支持转义后的&quot;
  // 后端计算关系时遵循W3规范，不会解析&quot;的情况，因此将style中的双引号替换为单引号
  const styleEls = el.querySelectorAll("[style]");
  for (let index = 0; index < styleEls.length; index++) {
    const element = styleEls[index] as HTMLElement;
    encodeStyleQuote(element);
  }
  return el.innerHTML;
}

export async function renderPage(rootMeta: Meta, classic: boolean) {
  const result: Record<string, string> = {};
  for (const section of rootMeta.children) {
    if (classic) {
      result[section.name] = await renderClassicSection(section, rootMeta).then(
        normalizeStyle
      );
    } else {
      result[section.name] = await renderModernSection(section, rootMeta).then(
        normalizeStyle
      );
    }
  }
  return result;
}

const styleValueFormatters: Record<string, (input: any) => string> = {
  NumberUnit({ unit, value }: NumberUnit) {
    return `${value}${unit}`;
  },
};

export function renderElementStyles(
  el: HTMLElement,
  meta: Meta,
  keys?: string[]
): void {
  if (!meta.propDefines) {
    return;
  }
  if (!keys || isEmpty(keys)) {
    keys = meta.propDefines.map((it) => it.name);
  }

  for (const prop of meta.propDefines) {
    const key = prop.name;
    if (!keys.includes(key)) {
      continue;
    }
    if (Object.hasOwnProperty.call(meta.props, key)) {
      let value = meta.props[key];
      const formatter = styleValueFormatters[prop.controlType];
      if (typeof formatter === "function") {
        value = formatter(value);
      }
      el.style.setProperty(kebabCase(key), value);
    }
  }
}

export function renderElementAttributes(
  el: HTMLElement,
  meta: Meta,
  keys?: string[]
) {
  if (!meta.propDefines) {
    return;
  }
  if (!keys || isEmpty(keys)) {
    keys = meta.propDefines.map((it) => it.name);
  }

  for (const prop of meta.propDefines) {
    const key = prop.name;
    if (!keys.includes(key)) {
      continue;
    }
    if (Object.hasOwnProperty.call(meta.props, key)) {
      const value = meta.props[key];
      el.setAttribute(kebabCase(key), value);
    }
  }
}
