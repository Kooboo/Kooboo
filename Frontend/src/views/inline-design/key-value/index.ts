import { Completer } from "@/utils/lang";
import { ref } from "vue";

const show = ref(false);
let completer: Completer<void>;
const keyValueId = ref<string>();

const editKeyValue = async (id: string) => {
  keyValueId.value = id;
  show.value = true;
  completer = new Completer();
  return await completer.promise;
};

const close = (success: boolean) => {
  if (success) completer.resolve();
  else completer.reject();
  show.value = false;
};

export { show, editKeyValue, keyValueId, close };
