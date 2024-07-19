import { ElMessage, type MessageParamsWithType } from "element-plus";
import { i18n } from "@/modules/i18n";
import { router } from "@/modules/router";
import { watch } from "vue";

const $t = i18n.global.t;
let messageHandles: any[] = [];

export const uploadMessage = () =>
  ElMessage.success({
    message: $t("common.uploadSuccess"),
    grouping: true,
  });
export const notAccessMessage = () =>
  ElMessage.error({
    message: $t("common.noAccessPermission"),
    grouping: true,
  });

export const showSaveSuccess = () =>
  ElMessage.success($t("common.saveSuccess"));

export const errorMessage = (
  message: string,
  keepShow?: boolean,
  options?: Partial<MessageParamsWithType>
) => {
  // Skip SQLite.Interop.dll Unable load, this only happen in arm mac
  if (message.includes("Unable to load shared library 'SQLite.Interop.dll'"))
    return;

  const result = ElMessage.error({
    message,
    duration: keepShow ? 0 : undefined,
    showClose: keepShow,
    ...(options ?? {}),
  });

  if (keepShow) messageHandles.push(result);
  return result;
};

export const warningMessage = (message: string, keepShow?: boolean) => {
  // Skip SQLite.Interop.dll Unable load, this only happen in arm mac
  if (message.includes("Unable to load shared library 'SQLite.Interop.dll'"))
    return;

  const result = ElMessage.warning({
    message,
    duration: keepShow ? 0 : undefined,
    showClose: keepShow,
  });

  if (keepShow) messageHandles.push(result);
  return result;
};

watch(
  () => router.currentRoute.value,
  () => {
    for (const i of messageHandles) {
      i?.close();
    }
    messageHandles = [];
  }
);
