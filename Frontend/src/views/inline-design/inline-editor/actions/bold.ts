import type { Action } from "../types";
import EditAction from "../icon-action.vue";
import { doc, selectionChangeEvent } from "@/views/inline-design/page";
import { i18n } from "@/modules/i18n";
import { triggerRef } from "vue";

const t = i18n.global.t;

const action: Action = {
  name: "bold",
  invoke: async () => {
    doc.value.execCommand("styleWithCSS", false, "true");
    doc.value.execCommand("bold");
    triggerRef(selectionChangeEvent);
  },
  component: EditAction,
  order: 10,
  icon: "icon-bold",
  display: t("common.bold"),
  divider: true,
  active: () => {
    return doc.value.queryCommandValue("bold") == "true";
  },
};

export default action;
