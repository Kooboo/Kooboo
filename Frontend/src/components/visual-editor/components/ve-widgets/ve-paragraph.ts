import type { Meta, VeWidgetPropDefine } from "../../types";

import WidgetBase from "./widget-base";
import { createPropDefine } from "../../utils/prop";
import { extendEmailEditorButtons } from "../../utils/editor";
import { i18n } from "@/modules/i18n";
import { isClassic } from "../../utils";
import { setStyles } from "../../render/utils";

const t = i18n.global.t;
const empty = `
<div>
  I'm a new paragraph block.
</div>
`;

export default class VeParagraphWidget extends WidgetBase {
  async render(meta: Meta): Promise<string> {
    const props: Record<string, any> = meta.props ?? {};
    const content = props["content"];
    if (!content) {
      return empty;
    }
    const el = document.createElement("div");
    setStyles(el, {
      width: "100%",
      fontFamily: "Arial, helvetica, sans-serif",
      fontSize: "14px",
      ...props,
    });
    el.innerHTML = content;
    return el.outerHTML;
  }

  buildPropDefines(): VeWidgetPropDefine[] {
    return [
      createPropDefine("content", {
        displayName: t("common.content"),
        controlType: "RichEditor",
        defaultValue: "I'm a new paragraph block.",
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
    ];
  }

  getAdditionalSettings(): Record<string, Record<string, any>> {
    if (!isClassic()) {
      return {};
    }

    return {
      content: extendEmailEditorButtons(),
    };
  }

  constructor() {
    super("paragraph", t("ve.paragraph"));
  }
}
