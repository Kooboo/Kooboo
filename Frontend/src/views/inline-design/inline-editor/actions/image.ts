import type { Action } from "../types";
import EditAction from "../icon-action.vue";
import { doc } from "@/views/inline-design/page";
import { chooseImage } from "@/views/inline-design/image/image-dialog";
import { i18n } from "@/modules/i18n";

const t = i18n.global.t;

export default {
  name: "image",
  invoke: async () => {
    const items = await chooseImage();
    doc.value.execCommand("insertImage", false, items[0].url);

    //TODO improve
    setTimeout(() => {
      doc.value?.dispatchEvent(new Event("scroll"));
    }, 100);
  },
  component: EditAction,
  order: 30,
  icon: "icon-photo",
  display: t("common.insertImage"),
  divider: true,
  active: () => {
    return doc.value.queryCommandValue("insertImage");
  },
} as Action;
