import type { Meta, VeWidgetPropDefine } from "../../types";
import {
  createFontProps,
  createPropDefine,
  createTextProps,
} from "../../utils/prop";

import WidgetBase from "./widget-base";
import { cloneDeep, omit } from "lodash-es";
import { i18n } from "@/modules/i18n";
import { watch, type Ref } from "vue";

const t = i18n.global.t;

const defaultSizes: Record<string, number> = {
  h1: 2,
  h2: 1.5,
  h3: 1.17,
  h4: 1,
  h5: 0.83,
  h6: 0.67,
  span: 1,
};

export default class VeTitleWidget extends WidgetBase {
  buildPropDefines(): VeWidgetPropDefine[] {
    return [
      createPropDefine("content", {
        displayName: t("common.content"),
        defaultValue: "I'm a new title block",
        controlType: "TextArea",
        settings: {
          autosize: true,
        },
      }),
      createPropDefine("titleType", {
        displayName: t("ve.titleType"),
        controlType: "Selection",
        defaultValue: "h1",
        selectionOptions: [
          { key: "H1", value: "h1" },
          { key: "H2", value: "h2" },
          { key: "H3", value: "h3" },
          { key: "H4", value: "h4" },
          { key: "H5", value: "h5" },
          { key: "H6", value: "h6" },
          { key: t("common.normal"), value: "span" },
        ],
        settings: {
          clearable: false,
        },
      }),
      ...createTextProps(),
      ...createFontProps({
        fontSize: 2,
        fontSizeUnit: "em",
        fontWeight: "bold",
      }),
    ];
  }

  constructor() {
    super("title", t("common.title"));
    this.tagName = "h1";
  }

  async render(meta: Meta): Promise<string> {
    const props = cloneDeep(meta.props ?? {});
    const content: any = props["content"];
    const titleType: string = props["titleType"] || "span";

    const fontSize = props["fontSize"];
    if (!fontSize?.value) {
      props["fontSize"] = {
        value: defaultSizes[titleType],
        unit: "em",
      };
    }
    const el = document.createElement(titleType);
    el.innerText = content || " ";
    this.setStyles(el, omit(props, ["content", "titleType"]));

    return el.outerHTML;
  }

  init(props: Ref<Record<string, any>>): void {
    super.init(props);
    watch(
      () => props.value["titleType"],
      (titleType) => {
        if (defaultSizes[titleType]) {
          props.value["fontSize"] = {
            value: defaultSizes[titleType],
            unit: "em",
          };
          props.value["fontWeight"] = titleType === "span" ? "normal" : "bold";
        }
      }
    );
  }

  fixedStyles = {
    "margin-top": "0.67em",
    "margin-bottom": "0.67em",
    "margin-left": "0",
    "margin-right": "0",
    "font-weight": "bold",
  };
}
