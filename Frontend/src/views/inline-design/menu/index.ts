import { Completer } from "@/utils/lang";
import { ref } from "vue";
import MenuDialog from "./menu-dialog.vue";

const show = ref(false);
let completer: Completer<void>;
const menuId = ref<string>();

const editMenu = async (id: string) => {
  completer = new Completer();
  menuId.value = id;
  show.value = true;
  return await completer.promise;
};

const close = (success: boolean) => {
  if (success) completer.resolve();
  else completer.reject();
  show.value = false;
};

export { show, editMenu, menuId, close, MenuDialog };
