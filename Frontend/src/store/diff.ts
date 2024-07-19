import type { Conflict } from "@/global/types";
import { Completer } from "@/utils/lang";
import { defineStore } from "pinia";
import { ref } from "vue";

type actionType = "cancel" | "merge" | "force";

export const useDiffStore = defineStore("diffStore", () => {
  const model = ref();
  const conflict = ref<Conflict>();
  const show = ref(false);
  const action = ref<actionType>("cancel");
  let completer: Completer<any>;

  const showDiaLog = (currentModel: unknown, diff: Conflict) => {
    model.value = currentModel;
    conflict.value = diff;
    show.value = true;
    action.value = "cancel";
    completer = new Completer();
    return completer.promise;
  };

  const closeDialog = () => {
    action.value === "cancel"
      ? completer.reject()
      : completer.resolve(undefined);
    show.value = false;
  };

  return {
    action,
    model,
    conflict,
    showDiaLog,
    show,
    closeDialog,
  };
});
