import { ElLoading } from "element-plus";

let loadingInstance: { close: () => void } | undefined;

export function showLoading() {
  if (!loadingInstance) {
    loadingInstance = ElLoading.service({ background: "rgba(0, 0, 0, 0.5)" });
  }
}

export function hideLoading() {
  loadingInstance?.close();
  loadingInstance = undefined;
}
