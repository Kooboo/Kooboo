import type { Action } from "../types";
import EditAction from "../icon-action.vue";
import { doc, selectionChangeEvent } from "@/views/inline-design/page";
import { i18n } from "@/modules/i18n";
import { triggerRef } from "vue";

const t = i18n.global.t;

export default {
  name: "underline",
  invoke: async () => {
    doc.value?.execCommand("underline");
    triggerRef(selectionChangeEvent);
  },
  component: EditAction,
  order: 12,
  icon: "icon-underline",
  display: t("common.underline"),
  active: () => {
    return doc.value.queryCommandValue("underline") === "true";
  },
} as Action;
