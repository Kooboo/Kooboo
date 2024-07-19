import type { Action } from "../types";
import FontSizeAction from "../font-size-action.vue";
import { doc, selectionChangeEvent } from "@/views/inline-design/page";
import { i18n } from "@/modules/i18n";
import { triggerRef } from "vue";

const t = i18n.global.t;

export default {
  name: "fontSize",
  invoke: (size: string) => {
    doc.value?.execCommand("styleWithCSS", false, "true");
    doc.value?.execCommand("fontSize", false, size);
    triggerRef(selectionChangeEvent);
  },
  component: FontSizeAction,
  order: 13,
  icon: "icon-underline",
  display: t("common.fontSize"),
  active: () => {
    return doc.value.queryCommandValue("fontSize");
  },
} as Action;
