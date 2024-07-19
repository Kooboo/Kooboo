import type { DomValueWrapper } from "@/global/types";
import { Completer } from "@/utils/lang";
import { ref } from "vue";
import StyleDialog from "./style-dialog.vue";

const show = ref(false);
let completer: Completer<DomValueWrapper[]>;
const element = ref<HTMLElement>();

const editStyle = async (el: HTMLElement) => {
  completer = new Completer<DomValueWrapper[]>();
  element.value = el;
  show.value = true;
  return await completer.promise;
};

const close = (success: boolean, wrappers: DomValueWrapper[]) => {
  if (success) completer.resolve(wrappers);
  else completer.reject();
  show.value = false;
};

export { show, editStyle, element, close, StyleDialog };
