import { Completer } from "@/utils/lang";
import { ElMessage } from "element-plus";
import { ref } from "vue";
import RepeaterDialog from "./repeater-dialog.vue";
import { i18n } from "@/modules/i18n";
const $t = i18n.global.t;

const show = ref(false);
let completer: Completer<void>;
const element = ref<HTMLElement>();

const editRepeater = async (el: HTMLElement) => {
  element.value = el;
  show.value = true;
  completer = new Completer();
  return await completer.promise;
};

const close = (success: boolean) => {
  if (success) {
    ElMessage.success($t("common.saveSuccess"));
    completer.resolve();
  } else completer.reject();
  show.value = false;
};

export { show, editRepeater, element, close, RepeaterDialog };
