import type { Meta, VeWidgetPropDefine } from "../../types";
import {
  createLinkElement,
  createLinkProps,
  createPropDefine,
  createSizeProps,
  getLinkUrl,
} from "../../utils/prop";
import {
  createTable,
  createTd,
  setAttributes,
  setStyles,
} from "../../render/utils";
import { isArray, isEmpty } from "lodash-es";

import WidgetBase from "./widget-base";
import { findWidget } from "../../utils/widget";
import { i18n } from "@/modules/i18n";

const t = i18n.global.t;
const image = `
<div
  style="width: 100%; height: ${166 / 16}rem; padding: ${
  20 / 16
}rem 0; display: flex; flex-direction: column; justify-content: space-between; align-items: center; background-color: #F3F3F3; border: 2px dashed #DDDDDD;"
>
  <svg
    width="${166 / 16}rem"
    height="${166 / 16}rem"
    fill="#979894"
    xmlns="http://www.w3.org/2000/svg"
    viewBox="0 0 512 512"
  >
    <path d="M152 120c-26.51 0-48 21.49-48 48s21.49 48 48 48s48-21.49 48-48S178.5 120 152 120zM447.1 32h-384C28.65 32-.0091 60.65-.0091 96v320c0 35.35 28.65 64 63.1 64h384c35.35 0 64-28.65 64-64V96C511.1 60.65 483.3 32 447.1 32zM463.1 409.3l-136.8-185.9C323.8 218.8 318.1 216 312 216c-6.113 0-11.82 2.768-15.21 7.379l-106.6 144.1l-37.09-46.1c-3.441-4.279-8.934-6.809-14.77-6.809c-5.842 0-11.33 2.529-14.78 6.809l-75.52 93.81c0-.0293 0 .0293 0 0L47.99 96c0-8.822 7.178-16 16-16h384c8.822 0 16 7.178 16 16V409.3z"/>
  </svg>
</div>
`;

function getInboxWidth(column: Meta, rootMeta: Meta) {
  const columnWidth =
    (column.props.widthPercent / 100) * rootMeta.props.contentWidth.value;
  // const sides = parseSides(column.props["veContainerPadding"]);

  // return columnWidth - (sides.left ?? 0) - (sides.right ?? 0);
  return columnWidth;
}

export default class VeImageWidget extends WidgetBase {
  buildPropDefines(): VeWidgetPropDefine[] {
    return [
      createPropDefine("src", {
        displayName: t("ve.src"),
        controlType: "MediaFile",
        defaultValue: "",
      }),
      ...createLinkProps(),
      createPropDefine("alt", {
        displayName: t("common.altText"),
        defaultValue: "image",
      }),
      createSizeProps({}, {}, { width: ["%"], height: ["%"] })[0], // only allow width, remove height
      createPropDefine("mobileFullWidth", {
        displayName: t("ve.mobileFullWidth"),
        defaultValue: false,
        controlType: "Switch",
      }),
    ];
  }

  constructor() {
    super("image", t("common.image"));
    this.tagName = "img";
  }

  async renderClassic(meta: Meta, rootMeta?: Meta): Promise<string> {
    const props = meta.props ?? {};
    let srcList: any = props["src"];
    if (isEmpty(srcList)) {
      return image;
    }

    if (!isArray(srcList)) {
      srcList = [srcList];
    }

    const el = document.createElement("div");
    const align = props["veContainerJustifyContent"];
    for (const src of srcList) {
      // container table
      const table = createTable();
      setAttributes(table, {
        align,
      });
      setStyles(table, {
        width: "100%",
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
      setAttributes(td, {
        align,
        style:
          "-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;margin: 0;padding: 0",
      });
      const img = document.createElement("img");
      const imageWidth = meta.props?.width?.value;
      const imgAttrs = {
        border: "0",
        alt: props["alt"],
        width: imageWidth + "",
        style:
          "-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;margin: 0;padding: 0;-ms-interpolation-mode: bicubic;outline: none;text-decoration: none;clear: both;display: block;border: 0;float: none;width: auto;height: auto;max-width: 100%;",
      };
      setAttributes(img, imgAttrs);
      if (imageWidth) {
        img.style.width = `${imageWidth}${meta.props?.width?.unit}`;
      }
      if (props["mobileFullWidth"]) {
        img.classList.add("full-width");
      }

      img.setAttribute("src", src);

      const href = getLinkUrl(props);
      if (href) {
        const wrapper = createLinkElement(props);
        setStyles(wrapper, {
          overflow: "hidden",
          display: "block",
          "background-color": "transparent",
        });
        wrapper.appendChild(img);

        td.appendChild(wrapper);
      } else {
        td.appendChild(img);
      }

      tr.appendChild(td);
      tbody.appendChild(tr);
      table.appendChild(tbody);
      el.appendChild(table);
    }
    setStyles(el, {
      overflow: "hidden",
    });
    return el.outerHTML;
  }

  async render(meta: Meta): Promise<string> {
    const props = meta.props ?? {};
    let srcList: any = props["src"];
    if (isEmpty(srcList)) {
      return image;
    }

    if (!isArray(srcList)) {
      srcList = [srcList];
    }

    const el = document.createElement("div");
    for (const src of srcList) {
      const img = document.createElement("img");
      const imgAttrs = {
        src,
        alt: props["alt"],
      };
      setAttributes(img, imgAttrs);
      setStyles(img, {
        "max-width": "100%",
      });
      this.setStyles(img, props);
      if (props["mobileFullWidth"]) {
        img.classList.add("full-width");
      }

      const href = getLinkUrl(props);
      if (href) {
        const wrapper = createLinkElement(props);
        setStyles(wrapper, {
          overflow: "hidden",
          display: "block",
          "background-color": "transparent",
        });
        wrapper.appendChild(img);
        el.appendChild(wrapper);
      } else {
        el.appendChild(img);
      }
    }
    return el.outerHTML;
  }
}
