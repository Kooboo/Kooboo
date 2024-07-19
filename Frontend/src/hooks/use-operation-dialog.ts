import { onMounted, ref, watch } from "vue";
import { UPDATE_MODEL_EVENT } from "@/constants/constants";

// defineProps 目前不支持从另一个文件继承接口，后续官方会支持
// https://v3.cn.vuejs.org/api/sfc-script-setup.html#%E4%BB%85%E9%99%90-typescript-%E7%9A%84%E5%8A%9F%E8%83%BD
export interface IDialogProps {
  modelValue: boolean;
  onDestroy?: () => void;
}

export interface IDialogEmits {
  (e: typeof UPDATE_MODEL_EVENT, value: boolean): void;
}

// todo emits's type need provide
export default function useOperationDialog<T extends IDialogProps>(
  props: T,
  emits: IDialogEmits
) {
  const visible = ref<boolean>(false);

  watch(
    () => props.modelValue,
    (val) => {
      visible.value = val;
    }
  );

  onMounted(() => {
    if (props.modelValue) {
      visible.value = true;
    }
  });

  function handleClose() {
    visible.value = false;
    emits(UPDATE_MODEL_EVENT, false);
  }

  return {
    visible,
    handleClose,
  };
}
