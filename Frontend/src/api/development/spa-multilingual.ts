import { useUrlSiteId } from "@/hooks/use-site-id";
import request from "@/utils/request";
import { i18n } from "@/modules/i18n";
const $t = i18n.global.t;

export enum Langs {
  "af" = "af",
  "ar" = "ar",
  "bg" = "bg",
  "ca" = "ca",
  "cs" = "cs",
  "da" = "da",
  "de" = "de",
  "el" = "el",
  "en" = "en",
  "eo" = "eo",
  "es" = "es",
  "et" = "et",
  "eu" = "eu",
  "fa" = "fa",
  "fi" = "fi",
  "fr" = "fr",
  "he" = "he",
  "hr" = "hr",
  "hu" = "hu",
  "hy-am" = "hy-am",
  "id" = "id",
  "it" = "it",
  "ja" = "ja",
  "kk" = "kk",
  "km" = "km",
  "ko" = "ko",
  "ku" = "ku",
  "ky" = "ky",
  "lt" = "lt",
  "lv" = "lv",
  "mn" = "mn",
  "nb-no" = "nb-no",
  "nl" = "nl",
  "pl" = "pl",
  "pt-br" = "pt-br",
  "pt" = "pt",
  "ro" = "ro",
  "ru" = "ru",
  "sk" = "sk",
  "sl" = "sl",
  "sr" = "sr",
  "sv" = "sv",
  "ta" = "ta",
  "th" = "th",
  "tk" = "tk",
  "tr" = "tr",
  "ug-cn" = "ug-cn",
  "uk" = "uk",
  "uz-uz" = "uz-uz",
  "vi" = "vi",
  "zh-cn" = "zh-cn",
  "zh-tw" = "zh-tw",
}

// 多语言
export interface Multilingual {
  value: {
    [key in Langs]?: string;
  };
  defaultLang: Langs;
  online: boolean;
  version: number;
  constType: number;
  creationDate: string;
  lastModified: string;
  lastModifyTick: number;
  id: string;
  name: string;
}

export const getList = () =>
  request.get<Multilingual[]>(useUrlSiteId("/SpaMultilingual/List"));

export const post = (data: Multilingual) =>
  request.post(useUrlSiteId("/SpaMultilingual/post"), data, undefined, {
    successMessage: $t("common.saveSuccess"),
  });

export const setLang = (langs: Langs[]) =>
  request.post(useUrlSiteId("/SpaMultilingual/SetLang"), langs, undefined, {
    successMessage: $t("common.saveSuccess"),
  });

export const deletes = (ids: string[]) =>
  request.post(useUrlSiteId("/SpaMultilingual/Deletes"), ids, undefined, {
    successMessage: $t("common.deleteSuccess"),
  });

export const upload = (data: unknown) =>
  request.post(useUrlSiteId("/SpaMultilingual/Import"), data, undefined, {
    successMessage: $t("common.importSuccess"),
  });
