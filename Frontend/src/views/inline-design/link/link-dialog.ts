import type { DomValueWrapper } from "@/global/types";
import { Completer } from "@/utils/lang";
import { ref } from "vue";
import LinkDialog from "./link-dialog.vue";

const show = ref(false);
let completer: Completer<DomValueWrapper[]>;
const anchor = ref<HTMLAnchorElement>();

const editLink = async (el: HTMLAnchorElement) => {
  completer = new Completer<DomValueWrapper[]>();
  anchor.value = el;
  show.value = true;
  return await completer.promise;
};

const close = (success: boolean, values: DomValueWrapper[]) => {
  if (success) completer.resolve(values);
  else completer.reject();
  show.value = false;
};

export { show, editLink, anchor, close, LinkDialog };
