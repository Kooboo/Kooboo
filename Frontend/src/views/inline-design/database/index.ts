import type { DatabaseType } from "@/api/database";
import { Completer } from "@/utils/lang";
import { ref } from "vue";
import DatabaseDialog from "./database-dialog.vue";

const show = ref(false);
let completer: Completer<void>;
const dataId = ref<string>();
const dataTable = ref<string>();
const dbType = ref<DatabaseType>();

const editDatabase = async (type: DatabaseType, table: string, id: string) => {
  dbType.value = type;
  dataTable.value = table;
  dataId.value = id;
  show.value = true;
  completer = new Completer();
  return await completer.promise;
};

const close = (success: boolean) => {
  if (success) completer.resolve();
  else completer.reject();
  show.value = false;
};

export { show, editDatabase, dbType, dataTable, dataId, close, DatabaseDialog };
