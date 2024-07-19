import type { KConfig } from "@/api/content/kconfig";
import { getKConfig } from "@/api/content/kconfig";
import { Completer } from "@/utils/lang";
import { ref } from "vue";
import ConfigDialog from "./config-dialog.vue";

const show = ref(false);
let completer: Completer<KConfig>;
const config = ref<KConfig>();

const editConfig = async (id: string) => {
  config.value = await getKConfig({ id, name: id });
  show.value = true;
  completer = new Completer();
  return await completer.promise;
};

const close = (success: boolean) => {
  if (success) completer.resolve(config.value!);
  else completer.reject();
};

export { show, editConfig, config, close, ConfigDialog };
