import { i18n } from "@/modules/i18n";

const $t = i18n.global.t;

export const methods = [
  {
    key: " ",
    value: $t("common.no2FA"),
    description: $t("common.none2FATip"),
  },
  {
    key: "email",
    value: $t("common.emailCode"),
    description: $t("common.emailCodeTip"),
  },
  {
    key: "tel",
    value: $t("common.smsCode"),
    description: $t("common.smsCodeTip"),
  },
  {
    key: "otp",
    value: $t("common.useAuthenticatorApp"),
    description: $t("common.useAuthenticatorAppTip"),
  },
];
