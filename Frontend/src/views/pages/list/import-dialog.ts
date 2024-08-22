import type { KeyValue } from "@/global/types";
import { ref } from "vue";
import type { Rules } from "async-validator";
import { urlRule, requiredRule, isUniqueNameRule } from "@/utils/validate";
import { i18n } from "@/modules/i18n";
import { pageUrlIsUniqueName } from "@/api/pages";
const $t = i18n.global.t;

export const fromList = ref<KeyValue[]>([
  {
    key: "url",
    value: "URL",
  },
  {
    key: "file",
    value: $t("common.file"),
  },
]);

export const accept = [
  "application/pdf",
  "application/msword",
  "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
  "application/vnd.ms-word.document.macroEnabled.12",
  "application/x-rar-compressed",
  "application/x-zip-compressed",
  "application/zip",
  "text/html",
];

export const rules = {
  name: [requiredRule($t("common.pageUrlRequiredTips"))],
  pageUrl: [
    requiredRule($t("common.urlRequiredTips")),
    isUniqueNameRule(pageUrlIsUniqueName, $t("common.urlOccupied")),
  ],
  file: requiredRule($t("common.selectFileTips")),
} as Rules;

export function createModel() {
  return {
    pageUrl: "",
    name: "",
    file: null as any,
    headless: false,
  };
}
