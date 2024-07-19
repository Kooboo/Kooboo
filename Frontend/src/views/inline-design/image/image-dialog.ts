import { ref } from "vue";
import { Completer } from "@/utils/lang";
import type { MediaFileItem } from "@/components/k-media-dialog";

const show = ref(false);
let completer: Completer<MediaFileItem[]>;
const multipleSelect = ref(false);

const chooseImage = async (multiple = false) => {
  completer = new Completer<MediaFileItem[]>();
  show.value = true;
  multipleSelect.value = multiple;
  return await completer.promise;
};

const close = (items: MediaFileItem[]) => {
  if (items) {
    completer.resolve(items);
  } else {
    completer.reject("close image dialog");
  }
};

export { show, multipleSelect, chooseImage, close };
