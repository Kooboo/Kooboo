import type { HtmlBlockItem } from "@/api/content/html-block";
import { Completer } from "@/utils/lang";
import { ElMessage } from "element-plus";
import { ref } from "vue";
import HtmlblockDialog from "./htmlblock-dialog.vue";
import { i18n } from "@/modules/i18n";
const $t = i18n.global.t;

const show = ref(false);
let completer: Completer<HtmlBlockItem>;
const htmlblockId = ref<string>();

const editHtmlblock = async (id: string) => {
  htmlblockId.value = id;
  show.value = true;
  completer = new Completer();
  return await completer.promise;
};

const close = (success: boolean, model: HtmlBlockItem) => {
  if (success) {
    ElMessage.success($t("common.saveSuccess"));
    completer.resolve(model);
  } else completer.reject();
  show.value = false;
};

export { show, editHtmlblock, htmlblockId, close, HtmlblockDialog };
