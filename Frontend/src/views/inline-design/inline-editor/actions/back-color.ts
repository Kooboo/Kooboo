import type { Action } from "../types";
import ColorAction from "../color-action.vue";
import { i18n } from "@/modules/i18n";
import { doc } from "@/views/inline-design/page";
import BackgroundColor from "@/components/icons/background-color.vue";

const t = i18n.global.t;

export default {
  name: "backColor",
  invoke: (color: string) => {
    doc.value?.execCommand("styleWithCSS", false, "true");
    doc.value?.execCommand("backColor", false, color);
  },
  component: ColorAction,
  order: 21,
  display: t("common.backColor"),
  active: () => {
    if (!doc.value) return false;
    return doc.value.queryCommandValue("backColor");
  },
  params: {
    icon: BackgroundColor,
  },
} as Action;
