import "ts-replace-all";

import type { MessageIframe, Meta, VeRenderContext } from "./types";
import { createApp, toRaw } from "vue/dist/vue.esm-bundler.js";

import VeBorder from "./components/ve-inject/ve-border.vue";
import VePlaceholder from "./components/ve-inject/index.vue";
import { install as installI18n } from "@/modules/i18n";
import { on } from "@/utils/dom";
import { useInjectGlobalStore } from "./components/ve-inject/inject-global-store";

const {
  init,
  updateSelect,
  updateHover,
  resetHolder,
  setDragToMeta,
  setDragFromMeta,
  updateRootMeta,
} = useInjectGlobalStore();

const app = createApp({
  components: {
    VePlaceholder,
    VeBorder,
  },
});
installI18n(app);
let mounted = false;
(window as any).loadDesign = function (
  rootMeta: Meta,
  renderContext: VeRenderContext,
  pageStyles: string,
  i18n: Record<string, string>
) {
  const data = toRaw(rootMeta);
  init(data, {
    baseUrl: renderContext.baseUrl,
    classic: renderContext.classic ?? false,
    i18n,
  });

  if (!mounted) {
    const scripts = document.querySelectorAll("body script");
    scripts.forEach((script) => {
      script.setAttribute("defer", "true");
      document.head.appendChild(script);
    });
    const activeBorder = document.createElement("ve-border");
    document.body.appendChild(activeBorder);

    app.mount(document.getElementById("app") || document.body);
    mounted = true;
  }
  setPageStyle(pageStyles);
};

on(window, "click", () => {
  resetHolder();
  window.parent.postMessage({
    type: "ve-layout",
  });
});

on(window, "scroll", () => {
  updateSelect();
  updateHover();
});

on(window, "message", (e) => {
  const ctx = e as MessageEvent<MessageIframe<any>>;
  const type = ctx?.data?.type;
  if (type === "in-page-style") {
    const { cssText, meta } = ctx.data.context;
    setPageStyle(cssText);
    if (meta) {
      updateRootMeta(meta);
    }
  } else if (type === "in-reset-holder") {
    resetHolder();
  } else if (type === "in-reset-drag") {
    setDragToMeta();
  } else if (type === "in-dragging-item") {
    setDragToMeta();
    setDragFromMeta(ctx.data.context);
  }
});

function setPageStyle(cssText?: string) {
  let style = document.head.querySelector("#designer-page-style");
  if (cssText) {
    if (!style) {
      style = document.createElement("style");
      style.setAttribute("id", "designer-page-style");
      style.innerHTML = cssText;
      document.head.appendChild(style);
      return;
    }
    style.innerHTML = cssText;
    return;
  }

  if (style) {
    document.head.removeChild(style);
  }
}
