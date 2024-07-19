import type { Meta, VeWidgetPropDefine } from "../../types";
import {
  createBorderProps,
  createFontProps,
  createLetterSpacing,
  createLinkElement,
  createLinkProps,
  createLinkStyleProps,
  createPropDefine,
} from "../../utils/prop";

import WidgetBase from "./widget-base";
import { getLinkStyles } from "../../render/utils";
import { i18n } from "@/modules/i18n";

const t = i18n.global.t;

export default class VeButtonWidget extends WidgetBase {
  buildPropDefines(): VeWidgetPropDefine[] {
    return [
      createPropDefine("buttonText", {
        displayName: t("ve.buttonText"),
        defaultValue: "Button Text",
      }),
      ...createLinkProps(),
      createLetterSpacing(),
      ...createFontProps({
        fontWeight: "normal",
        fontSize: 14,
      }),
      ...createLinkStyleProps({
        linkColor: "#FFFFFF",
        linkHoverColor: "#FFFFFF",
        linkBackgroundColor: "#3AAEE0",
        linkHoverBackgroundColor: "#3AAEE0",
        linkUnderline: false,
        linkHoverUnderline: false,
      }),
      ...createBorderProps(undefined, {
        borderRadius: {
          value: 4,
          unit: "px",
        },
      }),
      createPropDefine("padding", {
        controlType: "side-control",
        displayName: t("ve.padding"),
        defaultValue: JSON.stringify({
          moreOptions: true,
          top: 10,
          bottom: 10,
          left: 20,
          right: 20,
        }),
      }),
      createPropDefine("wordWrap", {
        isSystemField: true,
        defaultValue: "break-word",
      }),
      createPropDefine("display", {
        isSystemField: true,
        defaultValue: "inline-block",
      }),
    ];
  }

  constructor() {
    super("button", t("ve.button"));
    this.tagName = "a";
  }

  async render(meta: Meta): Promise<string> {
    const props = meta.props ?? {};
    const text: any = props["buttonText"] || " ";
    const el = createLinkElement(props, "#");
    el.innerText = text;

    this.setStyles(el, props);
    return el.outerHTML;
  }

  async renderClassic(meta: Meta): Promise<string> {
    const props = meta.props ?? {};
    const text: any = props["buttonText"] || " ";
    const el = createLinkElement(props, "#");
    el.innerText = text;
    this.setStyles(el, props);
    const { link } = getLinkStyles(props);
    for (const key in link) {
      if (Object.prototype.hasOwnProperty.call(link, key)) {
        el.style.setProperty(key, link[key]);
      }
    }
    return el.outerHTML;
  }
}
