import type { Action } from "../types";
import EditAction from "../icon-action.vue";
import { doc, selectionChangeEvent } from "@/views/inline-design/page";
import { i18n } from "@/modules/i18n";
import { triggerRef } from "vue";

const t = i18n.global.t;

export default {
  name: "italic",
  invoke: async () => {
    doc.value?.execCommand("italic");
    triggerRef(selectionChangeEvent);
  },
  component: EditAction,
  order: 11,
  icon: "icon-italic",
  display: t("common.italic"),
  active: () => {
    return doc.value.queryCommandValue("italic") === "true";
  },
} as Action;
