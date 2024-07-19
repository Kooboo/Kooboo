import type {
  IChangePasswordParam,
  ILoginParam,
  IRegisterParam,
  IUser,
} from "./types";

import { i18n } from "@/modules/i18n";
import request from "@/utils/request";

const $t = i18n.global.t;

export const register = (param: IRegisterParam) => {
  return request.post("/user/register", param);
};

export const login = (param: ILoginParam) => {
  param.withToken = true;
  return request.post("/user/login", param);
};

export const forgotPassword = (param: { email: string }) => {
  return request.post("/user/ForgotPassword", param);
};

export const updateProfile = (param: any) => {
  return request.post("/user/updateProfile", param);
};

export const changeLanguage = (language: string) => {
  return request.post("/user/ChangeLanguage", { language });
};

export const getUser = (): Promise<IUser> => {
  return request.get("/user/getUser").then((res: any) => {
    for (const key in res) {
      res[key] = res[key] || "";
    }
    return res;
  });
};

export const changePassword = (data: IChangePasswordParam) => {
  return request.post("/user/changePassword", data, undefined, {
    successMessage: $t("common.passwordChanged"),
  });
};

export const updateName = (firstName: string, lastName: string) => {
  return request.post("/user/updateName", { firstName, lastName });
};

export const sendTelCode = (tel: string) => {
  return request.post("/user/sendTelCode", { tel });
};

export const updateTel = (tel: string, code: number) => {
  return request.post("/user/updateTel", { tel, code }, undefined, {
    successMessage: $t("common.phoneNumberUpdated"),
  });
};

export const updateEmail = (email: string, code: number) => {
  return request.post("/user/UpdateEmail", { email, code }, undefined, {
    successMessage: $t("common.emailUpdated"),
  });
};

export const isUniqueEmailName = (name: string) =>
  request.get(
    "user/IsUniqueEmail",
    { email: name },
    { hiddenLoading: true, hiddenError: true }
  );

export const getAvailableCurrency = () => {
  return request.get("/user/AvailableCurrency");
};

export const changeCurrencyApi = (newCurrency: string) => {
  return request.post("/user/changeCurrency", { newCurrency }, undefined, {
    successMessage: $t("user.changeSuccess"),
  });
};
