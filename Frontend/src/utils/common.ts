import { useMultilingualStore } from "@/store/multilingual";
import { inject, type InjectionKey } from "vue";
import Cookies from "universal-cookie";

export function bytesToSize(filesize: number): string {
  const gigabytes = 1024 * 1024 * 1024;
  let returnValue = filesize / gigabytes;
  if (returnValue > 1) {
    return Math.ceil(returnValue * 100) / 100 + " GB";
  }
  const megabyte = 1024 * 1024;
  returnValue = filesize / megabyte;
  if (returnValue > 1) {
    return Math.ceil(returnValue * 100) / 100 + " MB";
  }
  const kilobyte = 1024;
  returnValue = filesize / kilobyte;
  return Math.ceil(returnValue * 100) / 100 + " KB";
}

export function injectStrict<T>(
  key: InjectionKey<T>,
  defaultValue?: T | (() => T)
): T {
  let injected: T | undefined;

  if (typeof defaultValue === "function")
    injected = inject(key, defaultValue, true);
  else injected = inject(key, defaultValue);

  if (typeof injected === "undefined")
    throw new Error(`Could not resolve injection ${key.description}`);

  return injected;
}

export function displayFormError() {
  const multilingualStore = useMultilingualStore();
  multilingualStore.expendAll();
  setTimeout(() => {
    document.querySelector(".el-form-item + .is-error")?.scrollIntoView({
      behavior: "auto",
      block: "center",
      inline: "center",
    });
  }, 300);
}

export function vscodeLogin(accessToken?: string) {
  const cookies = new Cookies();

  const url = new URL(
    `vscode://Kooboo.vscode-kooboo-devtool/login?token=${
      accessToken ?? cookies.get("jwt_token")
    }`
  );
  location.href = url.href;
  throw new Error("vscode login");
}
