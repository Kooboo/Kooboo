import { cloneDeep, omit, pickBy } from "lodash-es";
import {
  createTBody,
  createTable,
  createTd,
  createTr,
  setAttributes,
  setContainerStyles,
  setStyles,
} from "./utils";
import { render, setBindings } from ".";

import type { Meta } from "../types";
import { isColumn } from "../utils/widget";

const commonClassicStyles =
  "-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;margin: 0;padding: 0;border-spacing: 0;";

function wrapMso(el: HTMLElement, containerWidth: number) {
  return `<!--[if mso]>
  <table role="presentation" aria-hidden="true" border="0" cellspacing="0" cellpadding="0" align="center" width="${containerWidth}">
  <tr><td align="center" valign="top" width="${containerWidth}">
  <![endif]-->
  ${el.outerHTML}
  <!--[if mso]>
  </td>
  </tr>
  </table>
  <![endif]-->`;
}

function getContainerWidth(rootMeta: Meta): number {
  return rootMeta.props.contentWidth.value;
}

async function buildColumns(
  row: Meta,
  rootMeta: Meta,
  globalWidth: number
): Promise<string> {
  const table = createTable();
  setAttributes(table, {
    align: "center",
    width: `${globalWidth.toFixed()}`,
  });

  const tbody = createTBody(globalWidth);
  const tr = createTr(globalWidth);
  for (let i = 0; i < row.children.length; i++) {
    const column = row.children[i];
    if (!isColumn(column)) {
      continue;
    }
    const clonedMeta = cloneDeep(column);
    clonedMeta.props = omit(clonedMeta.props, ["widthPercent"]);
    const columnWidth = (column.props.widthPercent / 100) * globalWidth;
    const col = createTd();
    setAttributes(col, {
      align: "left",
      valign: "top",
      width: `${columnWidth.toFixed()}`,
    });

    const columnMeta = cloneDeep(clonedMeta);
    columnMeta.props = pickBy(
      clonedMeta.props,
      (value, key) =>
        key.startsWith("veContainerBackground") ||
        key.startsWith("veContainerBorder")
    );
    setContainerStyles(col, columnMeta);
    col.style.removeProperty("display");

    const wrapper = document.createElement("div");
    const wrapperMeta = cloneDeep(clonedMeta);
    wrapperMeta.props = omit(clonedMeta.props, Object.keys(columnMeta.props));
    setContainerStyles(wrapper, wrapperMeta); // set styles for column container

    wrapper.innerHTML = await render(clonedMeta, true, rootMeta);
    col.appendChild(wrapper);
    tr.appendChild(col);
  }
  tbody.appendChild(tr);
  table.appendChild(tbody);
  return table.outerHTML;
}

async function createContentTable(
  row: Meta,
  rootMeta: Meta
): Promise<HTMLElement> {
  const globalWidth = getContainerWidth(rootMeta);
  const table = createTable();
  setAttributes(table, {
    align: "center",
    width: `${globalWidth}`,
  });
  setStyles(table, {
    margin: "0 auto",
    padding: "0",
    "table-layout": "fixed",
    width: `${globalWidth}px`,
    "max-width": `${globalWidth}px`,
  });

  const tbody = createTBody(globalWidth);

  const tr = createTr(globalWidth);

  const td = createTd();
  td.classList.add("row");
  setAttributes(td, {
    "data-type": "row",
    align: "center",
    valign: "top",
  });
  setStyles(td, {
    "background-color": "transparent",
    width: `${globalWidth}px`, // fix qq & gmail mobile app style issue
  });
  setContainerStyles(td, row); // set styles for row container

  td.innerHTML = await buildColumns(row, rootMeta, globalWidth);

  tr.appendChild(td);
  tbody.appendChild(tr);
  table.appendChild(tbody);
  return table;
}

async function createContainerTable(row: Meta, rootMeta: Meta) {
  const globalWidth = getContainerWidth(rootMeta);
  const table = createTable();
  setAttributes(table, {
    align: "center",
    width: `${globalWidth}`,
  });
  setStyles(table, {
    margin: "0 auto",
    padding: "0",
    "table-layout": "fixed",
  });
  table.setAttribute("align", "center");
  table.setAttribute("width", `${globalWidth}`);

  const tbody = document.createElement("tbody");
  tbody.setAttribute("style", commonClassicStyles);

  const tr = document.createElement("tr");
  tr.setAttribute("style", commonClassicStyles);

  const td = createTd();
  setAttributes(td, {
    align: "center",
    valign: "top",
    width: `${globalWidth}`,
    bgcolor: "transparent",
  });
  setStyles(td, {
    "background-color": "transparent",
  });

  td.innerHTML = wrapMso(await createContentTable(row, rootMeta), globalWidth);

  tr.appendChild(td);
  tbody.appendChild(tr);
  table.appendChild(tbody);
  return table;
}

async function renderClassic(row: Meta, rootMeta: Meta): Promise<string> {
  const container = document.createElement("div");
  setStyles(container, {
    "-ms-text-size-adjust": "100%",
    "-webkit-text-size-adjust": "100%",
    margin: "0",
    padding: "0",
    width: "100%",
  });

  const table = await createContainerTable(row, rootMeta);
  container.appendChild(table);

  return `<center
  style="${commonClassicStyles}width: 100%;text-align: left">
  ${container.outerHTML}
  </center>`;
}

export default async function (
  row: Meta,
  classic: boolean,
  rootMeta: Meta
): Promise<string> {
  if (classic) {
    return await renderClassic(row, rootMeta);
  }

  const el = document.createElement("div");
  el.classList.add("ve-row-container");
  el.style.setProperty("display", "flex");

  for (const column of row.children) {
    if (!isColumn(column)) {
      continue;
    }
    const wrapperEl = document.createElement("div");
    wrapperEl.innerHTML = await render(column, classic, rootMeta);
    for (let index = 0; index < wrapperEl.children.length; index++) {
      const child = wrapperEl.children[index];
      el.appendChild(child);
    }
  }
  setContainerStyles(el, row);
  setBindings(el, row);
  return el.outerHTML;
}
