import type { Meta, VeWidgetPropDefine } from "../../types";
import { createNumber, createPropDefine } from "../../utils/prop";

import { getDefaultSocials, type SocialItem } from "./social";
import WidgetBase from "./widget-base";
import { i18n } from "@/modules/i18n";
import { cloneDeep, isEmpty } from "lodash-es";
import { createTable, createTd, setSides } from "../../render/utils";
const placeholder = "[ShareOn]";
const t = i18n.global.t;

function getPropItems(props: Record<string, any>): SocialItem[] {
  const items = props["items"];
  if (isEmpty(items)) {
    return [];
  }

  if (typeof items === "string") {
    return JSON.parse(items);
  }

  return items;
}

function renderItem(it: SocialItem, width: number): HTMLElement {
  const wrapper = document.createElement(it.url ? "a" : "span");
  wrapper.style.setProperty("background-image", "none");
  wrapper.style.setProperty("background-color", "transparent");
  if (it.url) {
    const prefix = it.prefix ?? "";
    let href = prefix + it.url;
    if (it.type === "share" && it.url !== placeholder) {
      href = prefix + window.encodeURIComponent(it.url);
    }
    wrapper.setAttribute("href", href);
    wrapper.setAttribute("target", "_blank");
    wrapper.setAttribute("title", it.title);
  }
  const img = document.createElement("img");
  img.setAttribute("width", `${width}`);
  img.setAttribute("src", it.icon);
  img.setAttribute("alt", it.alternateText || it.name);
  wrapper.appendChild(img);
  return wrapper;
}

export default class VeSocialWidget extends WidgetBase {
  buildPropDefines(): VeWidgetPropDefine[] {
    return [
      createPropDefine("items", {
        displayName: t("ve.socialIcons"),
        controlType: "social-control",
        defaultValue: JSON.stringify(getDefaultSocials()),
      }),
      createPropDefine("display", {
        isSystemField: true,
        defaultValue: "flex",
      }),
      createPropDefine("listStyleType", {
        isSystemField: true,
        defaultValue: "none",
      }),
      createPropDefine("flexDirection", {
        displayName: t("ve.flexDirection"),
        controlType: "Selection",
        defaultValue: "row",
        selectionOptions: [
          {
            key: t("ve.horizontal"),
            value: "row",
          },
          {
            key: t("ve.vertical"),
            value: "column",
          },
        ],
      }),
      createNumber("iconSize", t("ve.iconSize"), {
        defaultValue: 32,
        settings: {
          min: 10,
        },
      }),
      createPropDefine("listItemMargin", {
        controlType: "side-control",
        displayName: t("ve.listItemMargin"),
        defaultValue: JSON.stringify({ moreOptions: false, all: 5 }),
      }),
    ];
  }

  constructor() {
    super("social", t("ve.social"));
    this.icon = "icon-link2";
  }

  async render(meta: Meta): Promise<string> {
    const props: Record<string, any> = cloneDeep(meta.props ?? {});
    const items: SocialItem[] = getPropItems(props);
    const el = document.createElement("ul");
    this.setStyles(el, props);
    el.style.setProperty("padding-left", "0");

    const margin = props["listItemMargin"];
    const padding = props["listItemPadding"];
    const size = props["iconSize"] || 32;
    items.forEach((it) => {
      const child = document.createElement("li");
      const wrapper = renderItem(it, size);
      setSides("padding", wrapper, padding);
      setSides("margin", wrapper, margin);
      child.appendChild(wrapper);
      el.appendChild(child);
    });
    return el.outerHTML;
  }

  async renderClassic(meta: Meta): Promise<string> {
    const props: Record<string, any> = cloneDeep(meta.props ?? {});
    const items: SocialItem[] = getPropItems(props);
    const el = createTable();
    const margin = props["listItemMargin"];
    const padding = props["listItemPadding"];
    const isHorizontal = props["flexDirection"] === "row";

    const size = props["iconSize"] || 32;
    if (isHorizontal) {
      this.setStyles(el, props);
      const row = document.createElement("tr");
      items.forEach((it) => {
        const cell = createTd();
        const wrapper = renderItem(it, size);
        setSides("padding", wrapper, padding);
        setSides("margin", wrapper, margin);
        cell.appendChild(wrapper);
        row.appendChild(cell);
      });
      el.appendChild(row);
    } else {
      items.forEach((it) => {
        const row = document.createElement("tr");
        const cell = createTd();
        const div = document.createElement("div");
        setSides("padding", div, padding);
        setSides("margin", div, margin);
        const wrapper = renderItem(it, size);
        div.appendChild(wrapper);
        cell.appendChild(div);
        row.appendChild(cell);
        el.appendChild(row);
      });
    }

    el.style.display = "inline-table";
    el.style.removeProperty("flex-direction");
    return el.outerHTML;
  }

  async injection(meta: Meta): Promise<string> {
    const script = `<script>document.querySelectorAll('[data-designer-id="${meta.id}"] a[href]').forEach((it) => {
  const link = it.getAttribute("href");
  it.setAttribute("href", link.replace("${placeholder}", encodeURIComponent(window.location.href)));
});</script>`;
    return script;
  }
}
