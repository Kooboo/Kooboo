import type { Meta, VeWidgetPropDefine } from "../../types";
import { cloneDeep, isEmpty, kebabCase, omit } from "lodash-es";
import {
  createColorPicker,
  createFontProps,
  createLinkStyleProps,
  createPropDefine,
  createTextProps,
} from "../../utils/prop";
import {
  createTable,
  createTd,
  getSides,
  setSides,
  setStyles,
} from "../../render/utils";

import type { KeyValue } from "@/global/types";
import WidgetBase from "./widget-base";
import { i18n } from "@/modules/i18n";
import { ignoreCaseContains } from "@/utils/string";
import { isClassic } from "../../utils";
import { newGuid } from "@/utils/guid";

const t = i18n.global.t;

function getPropItems(props: Record<string, any>): KeyValue[] {
  const items = props["items"];
  if (isEmpty(items)) {
    return [];
  }

  if (typeof items === "string") {
    return JSON.parse(items);
  }

  return items;
}
export default class VeListWidget extends WidgetBase {
  buildPropDefines(): VeWidgetPropDefine[] {
    const result: VeWidgetPropDefine[] = [
      createPropDefine("items", {
        displayName: t("ve.listItems"),
        controlType: "items-control",
        defaultValue: JSON.stringify([
          {
            key: newGuid(),
            value: "This is a list item",
          },
        ]),
      }),
      createPropDefine("listStyleType", {
        displayName: t("ve.listStyleType"),
        controlType: "Selection",
        defaultValue: "disc",
        selectionOptions: [
          { key: t("ve.disc"), value: "disc" },
          {
            key: t("ve.circle"),
            value: "circle",
          },
          {
            key: t("ve.square"),
            value: "square",
          },
          // { key: t("common.none"), value: "none" },
          { key: t("common.number"), value: "number" },
        ],
        settings: {
          clearable: false,
        },
      }),
      ...createTextProps(),
      ...createFontProps({
        fontSize: 16,
        fontWeight: "normal",
      }),
      createPropDefine("lineHeight", {
        displayName: t("common.lineHeight"),
        controlType: "Selection",
        selectionOptions: [
          {
            key: "120%",
            value: "120%",
          },
          {
            key: "150%",
            value: "150%",
          },
          {
            key: "180%",
            value: "180%",
          },
          {
            key: "200%",
            value: "200%",
          },
        ],
      }),
      createPropDefine("flexDirection", {
        displayName: t("ve.flexDirection"),
        controlType: "Selection",
        defaultValue: "column",
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
        settings: {
          clearable: false,
        },
      }),
      createPropDefine("margin", {
        controlType: "side-control",
        displayName: t("ve.margin"),
        defaultValue: JSON.stringify({
          moreOptions: true,
          top: 16,
          right: 0,
          bottom: 16,
          left: 0,
        }),
      }),
      createPropDefine("padding", {
        controlType: "side-control",
        displayName: t("ve.padding"),
        defaultValue: JSON.stringify({
          moreOptions: true,
          top: 0,
          right: 0,
          bottom: 0,
          left: 40,
        }),
      }),
      createPropDefine("listItemMargin", {
        controlType: "side-control",
        displayName: t("ve.listItemMargin"),
      }),
      createPropDefine("listItemPadding", {
        controlType: "side-control",
        displayName: t("ve.listItemPadding"),
      }),
      createColorPicker(
        "listItemBackgroundColor",
        t("ve.listItemBackgroundColor")
      ),
      createPropDefine("display", {
        defaultValue: "flex",
        isSystemField: true,
      }),
    ];
    if (!isClassic()) {
      result.push(...createLinkStyleProps());
    }
    return result;
  }

  constructor() {
    super("list", t("common.list"));
    this.tagName = "ul";
  }

  async render(meta: Meta): Promise<string> {
    const props: Record<string, any> = cloneDeep(meta.props ?? {});
    const items: KeyValue[] = getPropItems(props);
    const listStyleType = props["listStyleType"];
    const tagName = listStyleType === "number" ? "ol" : "ul";
    if (tagName === "ol") {
      props["listStyleType"] = "";
    }
    const el = document.createElement(tagName);
    this.setStyles(el, omit(props, ["margin", "padding"]));
    setSides("margin", el, props["margin"]);
    setSides("padding", el, props["padding"]);

    const margin = props["listItemMargin"];
    const padding = props["listItemPadding"];
    const listBgColor = props["listItemBackgroundColor"];

    items.forEach((it) => {
      const child = document.createElement("li");
      setSides("padding", child, padding);
      setSides("margin", child, margin);
      setStyles(child, {
        "list-style": listStyleType,
        "list-style-position": "unset",
      });
      if (listBgColor) {
        child.style.setProperty("background-color", listBgColor);
      }
      child.innerHTML = it.value;
      el.appendChild(child);
    });
    return el.outerHTML;
  }

  async renderClassic(meta: Meta): Promise<string> {
    const props: Record<string, any> = cloneDeep(meta.props ?? {});
    const items: KeyValue[] = getPropItems(props);
    const el = createTable();
    const listItemStyles: Record<string, any> = {
      "list-style-position": "unset", // override sina mail default style
    };
    for (const key in props) {
      if (!key.startsWith("listItem")) {
        continue;
      }
      const prop = kebabCase(key.replace("listItem", ""));
      if (ignoreCaseContains(["margin", "padding"], prop)) {
        const sideStyles = getSides(prop, props[key]);
        for (const side in sideStyles) {
          const sideValue = sideStyles[side] || "0"; // fix yahoo li padding issue
          listItemStyles[side] = sideValue;
        }
      } else {
        listItemStyles[prop] = props[key];
      }
    }

    const isHorizontal = props["flexDirection"] === "row";

    el.style.setProperty("width", "100%");

    const tr = document.createElement("tr");
    const td = createTd();
    const align = props["veContainerJustifyContent"];
    if (align) {
      td.setAttribute("align", align);
    }

    const listStyleType = props["listStyleType"];
    const tagName = listStyleType === "number" ? "ol" : "ul";

    const listEl = document.createElement(tagName);
    if (tagName === "ol") {
      props["listStyleType"] = "";
    } else {
      listEl.style.setProperty("list-style", props["listStyleType"]);
    }

    this.setStyles(listEl, omit(props, ["margin", "padding"]));
    listEl.style.listStylePosition = "unset"; // override sina mail default style
    setSides("margin", listEl, props["margin"]);
    setSides("padding", listEl, props["padding"]);

    items.forEach((it) => {
      const li = document.createElement("li");
      if (props["listStyleType"]) {
        li.style.setProperty("list-style-type", props["listStyleType"]);
      }
      if (isHorizontal) {
        li.style.setProperty("float", "left");
      } else {
        li.style.setProperty("width", "100%");
      }
      setStyles(li, listItemStyles);
      li.innerHTML = it.value;
      listEl.appendChild(li);
    });
    listEl.style.setProperty("display", "inline-block"); // 让list可以整体居中
    listEl.style.removeProperty("flex-direction");
    if (isHorizontal) {
      listEl.style.setProperty("overflow", "hidden");
    }
    td.appendChild(listEl);
    tr.appendChild(td);
    el.appendChild(tr);

    return el.outerHTML;
  }

  fixedStyles = {
    "padding-inline-start": "40px",
    "margin-block-start": "16px",
    "margin-block-end": "16px",
    "box-sizing": "content-box",
  };
}
