import { useUrlSiteId } from "@/hooks/use-site-id";
import request from "@/utils/request";
import { i18n } from "@/modules/i18n";
const $t = i18n.global.t;

export interface User {
  role: string;
  userId: string;
  userName: string;
  email: string;
}

export const getCurrentUsers = () =>
  request.get<User[]>(useUrlSiteId("SiteUser/CurrentUsers"));

export const getAvailableUsers = () =>
  request.get<User[]>(useUrlSiteId("SiteUser/AvailableUsers"));

export const getRoles = () =>
  request.get<string[]>(useUrlSiteId("SiteUser/Roles"));

export const addUser = (body: unknown) =>
  request.post(useUrlSiteId("SiteUser/AddUser"), body, undefined, {
    successMessage: $t("common.addSuccess"),
  });

export const deletes = (ids: string[]) =>
  request.post(useUrlSiteId("SiteUser/Deletes"), { ids }, undefined, {
    successMessage: $t("common.deleteSuccess"),
  });
