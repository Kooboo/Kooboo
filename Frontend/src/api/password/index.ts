import request from "@/utils/request";
import { i18n } from "@/modules/i18n";

const $t = i18n.global.t;
export const smsCode = (tel: string) => {
  return request.post("/Password/SmsCode", { tel }, undefined, {
    successMessage: $t("common.verificationCodeHasSent"),
  });
};

export const EmailCode = (email: string) => {
  return request.post("/Password/EmailCode", { email }, undefined, {
    successMessage: $t("common.emailHasSent"),
  });
};

export const ResetByEmail = (
  email: string,
  code: string,
  newPassword: string
) => {
  return request.post<{ error: string }>("/Password/ResetByEmail", {
    email,
    code,
    newPassword,
  });
};

export const ResetByTel = (tel: string, code: string, newPassword: string) => {
  return request.post<{ error: string }>("/Password/ResetByTel", {
    tel,
    code,
    newPassword,
  });
};
