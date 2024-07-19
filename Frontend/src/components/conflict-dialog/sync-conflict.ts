import type { PullFeedBack, PushFeedBack } from "@/api/publish/types";
import { Completer } from "@/utils/lang";
import { ref } from "vue";

const show = ref(false);
let completer: Completer<void>;
const pushFeedBack = ref<PushFeedBack | PullFeedBack>();
const syncSettingId = ref<string>();

const showConflict = async (
  data: PushFeedBack | PullFeedBack,
  push: boolean,
  settingId: string
) => {
  pushFeedBack.value = data;
  syncSettingId.value = settingId;
  show.value = true;
  completer = new Completer();
  return await completer.promise;
};

const close = (success: boolean) => {
  if (success) completer.resolve();
  else completer.reject();
  show.value = false;
};

export { show, showConflict, syncSettingId, pushFeedBack, close };
