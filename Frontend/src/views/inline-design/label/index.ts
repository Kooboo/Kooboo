import type { Label } from "@/api/content/label";
import { get } from "@/api/content/label";
import { Completer } from "@/utils/lang";
import { ref } from "vue";

const show = ref(false);
let completer: Completer<Label>;
const label = ref<Label>();

const editLabel = async (id: string) => {
  label.value = await get(id);
  show.value = true;
  completer = new Completer();
  return await completer.promise;
};

const close = (success: boolean) => {
  if (success) completer.resolve(label.value!);
  else completer.reject();
};

export { show, editLabel, label, close };
