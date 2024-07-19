import { Completer } from "@/utils/lang";
import { ref } from "vue";
import ContentDialog from "./content-dialog.vue";

const show = ref(false);
let completer: Completer<void>;
const contentId = ref<string>();

const editContent = async (id: string) => {
  contentId.value = id;
  show.value = true;
  completer = new Completer();
  return await completer.promise;
};

const close = (success: boolean) => {
  if (success) completer.resolve();
  else completer.reject();
  show.value = false;
};

export { show, editContent, contentId, close, ContentDialog };
