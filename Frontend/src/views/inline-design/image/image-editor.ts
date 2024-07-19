import { ref } from "vue";
import { Completer } from "@/utils/lang";
import type { DomValueWrapper } from "@/global/types";
import ImageEditor from "./image-editor.vue";

const show = ref(false);
const image = ref<HTMLImageElement | HTMLPictureElement>();
let completer: Completer<DomValueWrapper[]>;

const showImageDialog = async (img: HTMLImageElement | HTMLPictureElement) => {
  completer = new Completer<DomValueWrapper[]>();
  image.value = img;
  show.value = true;
  return await completer.promise;
};

const close = (success: boolean, values: DomValueWrapper[]) => {
  if (success) {
    const result = values.filter((f) => f.value != f.origin);
    completer.resolve(result);
  } else {
    completer.reject();
  }
  show.value = false;
};

export { show, image, close, showImageDialog, ImageEditor };
