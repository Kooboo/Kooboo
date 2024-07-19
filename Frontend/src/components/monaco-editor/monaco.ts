import { applyCustomHtml } from "./html-customization";
import type { ClassDefine } from "@/api/site/kscript";
import { getClassSuggestions } from "@/api/site/kscript";
import { getDefine } from "@/api/site/kscript";
import Cookies from "universal-cookie";
import { getQueryString } from "@/utils/url";
import { getKViewSuggestions } from "@/api/site/kscript";
import { registerClassCompletionItemProvider } from "./classCompletion";

const cookies = new Cookies();
let htmlDefineCache: string;
let classDefineCache: string;
let currentKscript: Promise<string>;
let promise: Promise<ClassDefine[]>;
let settingLastModify = new Date().getTime();

export function refreshMonacoCache() {
  settingLastModify = new Date().getTime();
}

export function getCustomHtml() {
  return getKViewSuggestions();
}

export async function useKscript() {
  const siteId = getQueryString("siteId");
  const version = cookies.get("_site_version_");
  if (currentKscript && siteId + version + settingLastModify == htmlDefineCache)
    return currentKscript;
  currentKscript = (async () => {
    const customHtml = await getCustomHtml();
    await applyCustomHtml(customHtml);
    const defines = await getDefine();
    return defines;
  })();
  htmlDefineCache = siteId + version + settingLastModify;
  return currentKscript;
}

export async function useClassCompletion() {
  const siteId = getQueryString("siteId");
  const version = cookies.get("_site_version_");
  if (siteId + version + settingLastModify != classDefineCache) {
    promise = getClassSuggestions();
    classDefineCache = siteId + version + settingLastModify;
  }

  registerClassCompletionItemProvider(await promise);
}
