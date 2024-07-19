import type { Meta, VeRenderContext } from "../types";
import {
  classicStyles,
  createTBody,
  createTable,
  createTd,
  createTr,
  getTableAlign,
  setContainerStyles,
  setLinkStyles,
  setStyles,
} from "./utils";

import { getRootStyleObjects } from "../page-styles";
import { getSource } from "./api";
import { isCustomWidget } from "../utils/widget";
import { useBuiltinWidgets } from "../components/ve-widgets";

const renders = import.meta.globEager("./ve-*.ts");
const { widgets } = useBuiltinWidgets();

const noWrapTypes = ["section", "row", "column", "spacer"];

function _wrapClassicBlock(meta: Meta, html: string) {
  const table = createTable();
  table.style.width = "100%";
  const tbody = createTBody("100%");
  const tr = createTr("100%");
  const td = createTd();
  setContainerStyles(td, meta);

  td.style.removeProperty("justify-content");
  td.style.removeProperty("display");
  const justifyContent = meta.props?.veContainerJustifyContent;
  if (justifyContent) {
    td.style.setProperty("text-align", justifyContent);
    td.setAttribute("align", justifyContent);
  }
  td.innerHTML = html;

  tr.appendChild(td);
  tbody.appendChild(tr);
  table.appendChild(tbody);

  setLinkStyles(table, meta, `.ve-global [data-designer-id="${meta.id}"]`);
  setBindings(table, meta);
  return table.outerHTML;
}

function wrapBlockOptions(meta: Meta, html: string, classic: boolean) {
  const el = document.createElement("div");
  el.innerHTML = html;

  if (noWrapTypes.includes(meta.type)) {
    const child = el.firstElementChild as HTMLElement;
    setBindings(child, meta);
    return child.outerHTML;
  }
  if (classic) {
    return _wrapClassicBlock(meta, html);
  }

  setContainerStyles(el, meta);

  setLinkStyles(el, meta, `.ve-global [data-designer-id="${meta.id}"]`);
  setBindings(el, meta);

  return el.outerHTML;
}

export function setBindings(el: HTMLElement, meta: Meta) {
  const bindings = [
    {
      id: meta.id!,
      source: "designer",
      type: meta.type,
    },
  ];
  el.setAttribute("k-binding", JSON.stringify(bindings));
  el.setAttribute("data-designer-id", meta.id!);
  el.setAttribute("data-designer-type", meta.type);
}

export async function render(
  meta: Meta,
  classic: boolean,
  rootMeta?: Meta
): Promise<string> {
  if (isCustomWidget(meta)) {
    const tagName = meta.type;
    const tempTag = document.createElement("div");
    meta.propDefines.forEach(({ name, defaultValue }) => {
      const value = meta.props[name] ?? defaultValue;
      tempTag.setAttribute(name, value);
    });
    tempTag.setAttribute("id", meta.name);

    const html = tempTag.outerHTML;
    let attrs = html.substring(4, html.length - 4);
    if (meta.attribute) {
      attrs = attrs + " " + meta.attribute;
    }
    return wrapBlockOptions(
      meta,
      `<div><${tagName}${attrs}${tagName}></div>`,
      classic
    );
  }

  const widget = widgets.value.find((it) => it.id === meta.type);
  const renderFunction = classic ? widget?.renderClassic : widget?.render;
  if (typeof renderFunction === "function") {
    return renderFunction.call(widget, meta, rootMeta).then((html) => {
      if (typeof widget?.injection === "function") {
        return widget
          .injection(meta)
          .then((inj) => wrapBlockOptions(meta, [html, inj].join(""), classic));
      }
      return wrapBlockOptions(meta, html, classic);
    });
  }

  const key = `./ve-${meta.type.toLowerCase()}.ts`;
  const invoker = renders[key]?.default;
  if (typeof invoker === "function") {
    return await invoker(meta, classic, rootMeta).then((html: string) =>
      wrapBlockOptions(meta, html, classic)
    );
  }

  return wrapBlockOptions(
    meta,
    `<div>Unknown widget: ${meta.type}</div>`,
    classic
  );
}

export async function preview(
  meta: Meta,
  ctx: VeRenderContext
): Promise<string> {
  const widget = widgets.value.find((it) => it.id === meta.type);
  const renderFunction = ctx?.classic ? widget?.renderClassic : widget?.render;
  if (typeof renderFunction === "function") {
    return renderFunction
      .call(widget, meta, ctx.rootMeta)
      .then((html) => wrapBlockOptions(meta, html, ctx.classic));
  }

  const key = `./ve-${meta.type.toLowerCase()}.ts`;
  const invoker = renders[key]?.default;
  if (typeof invoker === "function") {
    return await invoker(meta, ctx.classic, ctx.rootMeta, ctx).then(
      (html: string) => wrapBlockOptions(meta, html, ctx.classic)
    );
  }

  const source = await getSource(meta.type, meta.name);
  const body = source.body || `Empty widget: ${meta.type} (${meta.name})`;
  return wrapBlockOptions(meta, `<div>${body}</div>`, ctx.classic);
}

function wrapClassicBody(bodyHtml: string, rootProps?: Record<string, any>) {
  const table = createTable();
  table.classList.add("ve-global");
  const { body, row } = getRootStyleObjects(rootProps ?? {}, true);
  setStyles(table, {
    ...classicStyles,
    ...body,
  });

  const tr = document.createElement("tr");
  const td = createTd();
  setStyles(td, row);
  const align = getTableAlign(rootProps?.alignItems);
  if (align) {
    td.setAttribute("align", align);
    table.style.setProperty("justify-content", rootProps?.alignItems);
  }
  td.innerHTML = bodyHtml;
  tr.appendChild(td);
  table.appendChild(tr);
  setStyles(table, {
    width: "100%",
    display: "table",
    "min-width": row["width"],
  });
  table.style.removeProperty("flex-direction");
  table.style.removeProperty("align-items");
  table.style.removeProperty("justify-content");

  const bgColor = body["background-color"];
  return `<table
  style="border-spacing: 0px;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;margin: 0 auto;padding: 0;mso-table-lspace: 0pt;mso-table-rspace: 0pt;border-spacing: 0;border-collapse: collapse;table-layout: fixed;min-width: 100px;mso-line-height-rule: exactly;background-color: ${bgColor}"
  bgcolor="${bgColor}" width="100%" align="center" cellpadding="0" cellspacing="0">
  <tbody style="-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;margin: 0;padding: 0">
    <tr style="-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;margin: 0;padding: 0">
      <td
        style="-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;margin: 0;padding: 0;mso-table-lspace: 0pt;mso-table-rspace: 0pt"
        align="center">
        ${table.outerHTML}
      </td>
    </tr>
  </tbody>
</table>`;
}

export function renderBody(
  body: string,
  classic: boolean,
  rootProps?: Record<string, any>
) {
  if (classic) {
    return wrapClassicBody(body, rootProps);
  }

  return `<div class="ve-container ve-global" data-designer-type="root">${body}</div>`;
}
