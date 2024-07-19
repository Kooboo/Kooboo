import { createApp } from "vue";
import HtmlEditor from "./monaco.vue";
import { sourceCode } from "./store";
import { i18n } from "@/modules/i18n";
const $t = i18n.global.t;

tinymce.PluginManager.add("monaco", function (n: any) {
  function o() {
    n.focus(),
      n.selection.collapse(!0),
      n.settings.monaco.saveCursorPosition &&
        n.selection.setContent(
          '<span style="display: none;" class="CmCaReT">&#x0;</span>'
        );
    const o = {
        title: $t("common.HtmlSourceCode"),
        size: "large", //width,heigth not work
        body: {
          type: "panel",
          items: [
            {
              type: "htmlpanel",
              html: `<div class="__mce_monaco" style="height: 100%;"></div>`,
            },
          ],
        },
        resizable: !0,
        maximizable: !0,
        fullScreen: n.settings.monaco.fullscreen,
        buttons: [
          {
            type: "cancel",
            text: "Close",
            onclick: "close",
          },
          {
            type: "submit",
            text: "Save",
            primary: true,
          },
        ],
        onSubmit: function (e: any) {
          n.setContent(sourceCode.value);
          e.close();
        },
      },
      e = n.windowManager.open(o);
    sourceCode.value = n.getContent(); // 换行
    createApp(HtmlEditor).mount(".__mce_monaco");
    const el = document.querySelector(".tox-form__group") as HTMLElement;
    el && (el.style.height = "calc(100% - 14px)");
    n.settings.monaco.fullscreen && e.fullscreen(!0);
  }

  n.ui.registry.addButton("code", {
    title: "Source code",
    icon: "sourcecode",
    onAction: o,
  }),
    n.ui.registry.addMenuItem("code", {
      icon: "sourcecode",
      text: "Source code",
      context: "tools",
      onAction: o,
    });
});
