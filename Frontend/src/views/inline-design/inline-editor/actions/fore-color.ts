import type { Action } from "../types";
import ColorAction from "../color-action.vue";
import { i18n } from "@/modules/i18n";
import { doc, selectionChangeEvent } from "@/views/inline-design/page";
import TextColor from "@/components/icons/text-color.vue";
import { triggerRef } from "vue";

const t = i18n.global.t;

export default {
  name: "foreColor",
  invoke: (color: string) => {
    doc.value?.execCommand("styleWithCSS", false, "true");
    doc.value?.execCommand("foreColor", false, color);
    triggerRef(selectionChangeEvent);
  },
  component: ColorAction,
  order: 20,
  icon: "icon-a-writein3",
  display: t("common.textColor"),
  active: () => {
    if (!doc.value) return false;
    return doc.value.queryCommandValue("foreColor");
  },
  params: {
    icon: TextColor,
  },
} as Action;
