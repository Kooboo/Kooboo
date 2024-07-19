import type { ElMessageBoxOptions } from "element-plus";
import { ElLoading, ElMessageBox } from "element-plus";

import type { Slots } from "vue";

export function getSlotContent(slots: Slots) {
  if (!slots.default) return;
  const child = slots.default()[0]?.children;

  if (typeof child === "string") {
    return child;
  }
}

export async function showLoading(
  action: () => Promise<void>,
  message?: string | boolean
) {
  if (message === undefined || message === false) {
    await action();
    return;
  }

  const instance = ElLoading.service({
    lock: true,
    text: message as any,
  });

  try {
    await action();
  } finally {
    instance.close();
  }
}

export async function showConfirm(
  message?: string,
  options?: ElMessageBoxOptions
) {
  if (message) await ElMessageBox.confirm(message, options);
}

export function GetModuleUrl(url: string) {
  if (location.pathname.startsWith("/_")) {
    const moduleName = location.pathname.split("/")[1];
    if (!url.startsWith(`/${moduleName}/`)) url = `/${moduleName}${url}`;
  }

  return url;
}
