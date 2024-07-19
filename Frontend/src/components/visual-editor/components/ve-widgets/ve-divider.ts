import type { Meta, VeWidgetPropDefine } from "../../types";
import { createBorderProps, createPropDefine } from "../../utils/prop";

import WidgetBase from "./widget-base";
import { i18n } from "@/modules/i18n";
import { lengthUnits } from "@/global/style";

const t = i18n.global.t;

export default class VeDividerWidget extends WidgetBase {
  buildPropDefines(): VeWidgetPropDefine[] {
    return [
      createPropDefine("width", {
        displayName: t("common.width"),
        controlType: "NumberUnit",
        defaultValue: {
          value: 90,
          unit: "%",
        },
        selectionOptions: lengthUnits,
      }),
      createPropDefine("veContainerFontSize", {
        isSystemField: true,
        defaultValue: "1px",
      }),
      ...createBorderProps(
        {
          borderWidth: "borderTopWidth",
          borderStyle: "borderTopStyle",
          borderColor: "borderTopColor",
        },
        {
          borderWidth: {
            value: 1,
            unit: "px",
          },
          borderStyle: "solid",
          borderColor: "#CCCCCC",
        },
        ["borderRadius"]
      ),
    ];
  }

  constructor() {
    super("divider", t("ve.divider"));
  }

  async render(meta: Meta): Promise<string> {
    const props = meta.props ?? {};
    const el = document.createElement("div");

    const { value, unit }: any = props["width"] ?? {};
    if (!value || !unit) {
      return el.outerHTML;
    }

    this.setStyles(el, props);
    el.style.display = "inline-block";
    el.style.fontSize = "1px";
    el.style.lineHeight = "1px";
    return el.outerHTML;
  }
}
