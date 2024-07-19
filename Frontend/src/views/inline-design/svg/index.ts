import { Completer } from "@/utils/lang";
import { ref } from "vue";
import SvgDialog from "./svg-dialog.vue";

const show = ref(false);
let completer: Completer<string>;
const element = ref<HTMLElement>();

const editSvg = async (el: HTMLElement) => {
  completer = new Completer<string>();
  element.value = el;
  show.value = true;
  return await completer.promise;
};

const close = (success: boolean, value: string) => {
  if (success) completer.resolve(value);
  else completer.reject();
  show.value = false;
};

export { show, editSvg, element, close, SvgDialog };
