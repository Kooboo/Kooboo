import type { Meta, VeWidgetPropDefine } from "../../types";
import { createPropDefine, createSizeProps } from "../../utils/prop";

import WidgetBase from "./widget-base";
import { i18n } from "@/modules/i18n";
import { setAttributes } from "../../render/utils";

const t = i18n.global.t;

const video = `
<div 
  style="display: flex; flex-direction: column; justify-content: center; align-items: center;"
>
<div style="width: 560px; height: 315px; background-color: #F3F3F3; border: 2px dashed #DDDDDD; display: flex;justify-content: center; align-items: center;">
  <svg 
    width="200px" 
    height="166px" 
    fill="#979894"
    xmlns="http://www.w3.org/2000/svg" 
    viewBox="0 0 512 512"
  >
  <path d="M549.655 124.083c-6.281-23.65-24.787-42.276-48.284-48.597C458.781 64 288 64 288 64S117.22 64 74.629 75.486c-23.497 6.322-42.003 24.947-48.284 48.597-11.412 42.867-11.412 132.305-11.412 132.305s0 89.438 11.412 132.305c6.281 23.65 24.787 41.5 48.284 47.821C117.22 448 288 448 288 448s170.78 0 213.371-11.486c23.497-6.321 42.003-24.171 48.284-47.821 11.412-42.867 11.412-132.305 11.412-132.305s0-89.438-11.412-132.305zm-317.51 213.508V175.185l142.739 81.205-142.739 81.201z"/>
  </svg>
  </div>
</div>
`;

function getUrl(props: Record<string, any>): string {
  const url = props["url"];
  if (!url) {
    return url ?? "";
  }

  let v: string | null = null;
  const uri = new URL(url);
  const hostname = uri.hostname.toLowerCase();
  if (hostname.includes("youtube.com")) {
    if (uri.pathname.startsWith("/watch")) {
      v = "/" + uri.searchParams.get("v");
    } else {
      const paths = uri.pathname.split("/");
      v = "/" + paths[paths.length - 1] + uri.search;
    }
  } else if (hostname === "youtu.be") {
    v = uri.pathname.trim() + uri.search;
  }

  if (v) {
    const host = props["privacyMode"]
      ? "www.youtube-nocookie.com"
      : "www.youtube.com";
    const iframeUrl = `https://${host}/embed${v}`;
    if (props["showControls"]) {
      return iframeUrl;
    }
    const result = new URL(iframeUrl);
    result.searchParams.set("controls", "0");
    return result.href;
  }

  return url;
}
export default class VeYouTubeWidget extends WidgetBase {
  buildPropDefines(): VeWidgetPropDefine[] {
    return [
      createPropDefine("url", {
        displayName: t("common.url"),
      }),
      ...createSizeProps(
        undefined,
        {
          width: {
            value: 560,
            unit: "px",
          },
          height: {
            value: 315,
            unit: "px",
          },
        },
        {
          width: ["auto", "%"],
          height: ["auto", "%"],
        }
      ),
      createPropDefine("showControls", {
        displayName: t("ve.showControls"),
        controlType: "Switch",
        defaultValue: true,
      }),
      createPropDefine("privacyMode", {
        displayName: t("ve.privacyMode"),
        controlType: "Switch",
      }),
    ];
  }

  constructor() {
    super("youtube", t("ve.youtube"));
    this.tagName = "iframe";
    this.icon = "icon-ve-video";
  }

  async render(meta: Meta): Promise<string> {
    const props = meta.props ?? {};
    const url: string = getUrl(props);
    if (!url) {
      return video;
    }

    const iframe = document.createElement("iframe");
    const attrs: Record<string, any> = {
      src: url,
      title: "YouTube video player",
      frameborder: "0",
      allow:
        "accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture; web-share",
      allowfullscreen: "true",
    };
    setAttributes(iframe, attrs);
    this.setStyles(iframe, props);

    return iframe.outerHTML;
  }

  async renderClassic(meta: Meta): Promise<string> {
    return "YouTube is not supported in classic mode.";
  }
}
