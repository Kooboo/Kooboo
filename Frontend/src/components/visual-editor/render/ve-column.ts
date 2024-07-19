import {
  createTable,
  createTd,
  setAttributes,
  setContainerStyles,
  setStyles,
} from "./utils";
import { render, setBindings } from ".";

import type { Meta } from "../types";

async function renderClassicColumn(
  meta: Meta,
  rootMeta: Meta
): Promise<string> {
  const wrapper = createTable();
  setAttributes(wrapper, {
    bgcolor: "transparent",
    width: "100%",
  });
  setStyles(wrapper, {
    margin: "0 auto",
    padding: "0",
    "table-layout": "fixed",
  });

  const tbody = document.createElement("tbody");
  tbody.setAttribute(
    "style",
    "-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;margin: 0;padding: 0"
  );
  const tr = document.createElement("tr");
  tr.setAttribute(
    "style",
    "-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;margin: 0;padding: 0"
  );
  const td = createTd();
  td.style.width = "100%";

  const widgetBuilder: string[] = [];
  for (const widget of meta.children) {
    widgetBuilder.push(await render(widget, true, rootMeta));
  }
  td.innerHTML = widgetBuilder.join("");
  tr.appendChild(td);
  tbody.appendChild(tr);
  wrapper.appendChild(tbody);
  return wrapper.outerHTML;
}

export default async function (
  meta: Meta,
  classic: boolean,
  rootMeta: Meta
): Promise<string> {
  if (classic) {
    return await renderClassicColumn(meta, rootMeta);
  }
  const el = document.createElement("div");
  setContainerStyles(el, meta);
  el.style.setProperty("width", `${meta.props["widthPercent"]}%`);
  el.style.setProperty("flex-direction", "column");
  setBindings(el, meta);

  for (const widget of meta.children) {
    const columnEl = document.createElement("div");
    columnEl.innerHTML = await render(widget, classic, rootMeta);
    for (let index = 0; index < columnEl.children.length; index++) {
      const child = columnEl.children[index];
      el.appendChild(child);
    }
  }
  return el.outerHTML;
}
