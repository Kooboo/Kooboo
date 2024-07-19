import { Completer } from "@/utils/lang";
import { ref } from "vue";
import ProductDialog from "./product-dialog.vue";

const show = ref(false);
let completer: Completer<void>;
const productId = ref<string>();

const editProduct = async (id: string) => {
  productId.value = id;
  show.value = true;
  completer = new Completer();
  return await completer.promise;
};

const close = (success: boolean) => {
  if (success) completer.resolve();
  else completer.reject();
  show.value = false;
};

export { show, editProduct, productId, close, ProductDialog };
