import type { Install } from "./types";
import messages from "@intlify/vite-plugin-vue-i18n/messages";
import { createI18n } from "vue-i18n";
import zhElPackage from "element-plus/lib/locale/lang/zh-cn";
import { usePersistent } from "../store/persistent";
import { computed } from "vue";
import { getQueryString } from "@/utils/url";
import Cookies from "universal-cookie";
const cookies = new Cookies();

messages.zh.elementPackage = zhElPackage;

const displayMap: Record<string, string> = {
  en: "English",
  zh: "简体中文",
};

export const languages = computed(() => {
  const result = [];
  for (const key in messages) {
    result.push({
      lang: key,
      name: displayMap[key],
      messages: messages[key],
      elementPackage: messages[key].elementPackage,
    });
  }
  return result;
});

// allowed for legacy usage
export const i18n = createI18n({
  locale: getQueryString("inline_design_lang") || getDefaultLanguage(),
  messages,
  fallbackLocale: "en",
  formatFallbackMessages: true,
  silentFallbackWarn: true,
  silentTranslationWarn: true,
});

export const install: Install = (app) => app.use(i18n);

export function getDefaultLanguage() {
  const result = cookies.get("_cms_lang") || "en";
  return result;
}

export function changeLanguage(newLang: string) {
  const { language } = usePersistent();
  if (language.value == newLang) return;
  language.value = newLang;
  window.location.reload();
}
