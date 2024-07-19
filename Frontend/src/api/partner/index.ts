import request from "@/utils/request";
import { i18n } from "@/modules/i18n";
import type { AddUser, partnerDNS, partnerServer, partnerUser } from "./type";
const $t = i18n.global.t;

export const getPartnerServer = () =>
  request.get<partnerServer[]>("/Partner/Servers");

export const getDNS = () => request.get<partnerDNS[]>("/Partner/DNS");

export const getUsers = () => request.get<partnerUser[]>("/Partner/Users");

export const impersonateApi = (username: string) =>
  request.get<partnerUser[]>("/Partner/Impersonate", { username });

export const addNewAccount = (body: AddUser) =>
  request.post("Partner/AddNewUser", body, undefined, {
    successMessage: $t("common.addSuccess"),
  });

export const addExistingAccount = (body: AddUser) =>
  request.post("Partner/AddExisting", body, undefined, {
    successMessage: $t("common.addSuccess"),
  });

export const changePassword = (body: unknown) => {
  return request.post("/Partner/ResetPassword", body, undefined, {
    successMessage: $t("common.passwordChanged"),
  });
};
