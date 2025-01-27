import { i18n } from "@/modules/i18n";

const $t = i18n.global.t;

export const methods = [
  { key: " ", value: $t("common.none") },
  { key: "email", value: $t("common.emailMessage") },
  { key: "tel", value: $t("common.phoneMessage") },
  { key: "otp", value: $t("common.authenticatorApp") },
];
