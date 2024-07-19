import type { Action } from "../types";
import EditAction from "../icon-action.vue";
import { doc } from "@/views/inline-design/page";
import { editLink } from "@/views/inline-design/link/link-dialog";
import { i18n } from "@/modules/i18n";

const t = i18n.global.t;

export default {
  name: "link",
  invoke: async () => {
    const attributes = await editLink(document.createElement("a"));
    doc.value.execCommand("createLink", false, attributes[0].value);
  },
  component: EditAction,
  order: 31,
  display: t("common.insertLink"),
  icon: "icon-link1",
  active: () => {
    return doc.value.queryCommandValue("createLink");
  },
} as Action;
