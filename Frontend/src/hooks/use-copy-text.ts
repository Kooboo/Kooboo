import { ElMessage } from "element-plus";
import { i18n } from "@/modules/i18n";
const $t = i18n.global.t;

export function copyText(text: string) {
  if (navigator.clipboard && window.isSecureContext) {
    navigator.clipboard.writeText(text);
    ElMessage.success($t("common.copySuccess"));
  } else {
    const textArea = document.createElement("textarea");
    textArea.value = text;
    document.body.appendChild(textArea);
    textArea.focus();
    textArea.select();
    ElMessage.success($t("common.copySuccess"));
    return new Promise((res, rej) => {
      document.execCommand("copy") ? res(text) : rej();
      textArea.remove();
    });
  }
}
