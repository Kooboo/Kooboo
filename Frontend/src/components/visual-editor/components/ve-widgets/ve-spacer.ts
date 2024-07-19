import type { Meta, VeWidgetPropDefine } from "../../types";

import WidgetBase from "./widget-base";
import { createLengthUnit } from "../../utils/prop";
import { i18n } from "@/modules/i18n";

const t = i18n.global.t;
const defaultValue = `
<div style="width:100%; height: 40px;"></div>
`;
export default class VeSpacerWidget extends WidgetBase {
  buildPropDefines(): VeWidgetPropDefine[] {
    return [
      createLengthUnit(
        "height",
        t("common.height"),
        {
          defaultValue: {
            value: 40,
            unit: "px",
          },
          settings: {
            value: {
              min: 1,
            },
          },
        },
        ["%", "auto"]
      ),
    ];
  }

  constructor() {
    super("spacer", t("ve.spacer"));
  }

  async render(meta: Meta): Promise<string> {
    const props = meta.props ?? {};
    const height: any = props["height"];
    if (!height) {
      return defaultValue;
    }

    const el = document.createElement("div");
    el.innerHTML = "&nbsp;";
    this.setStyles(el, props);
    el.style.fontSize = "1px";
    el.style.lineHeight = "1px";
    return el.outerHTML;
  }
}
