import { useUrlSiteId } from "@/hooks/use-site-id";
import request from "@/utils/request";
import { i18n } from "@/modules/i18n";
const $t = i18n.global.t;

export const saveUserOptions = (model: unknown) =>
  request.post(useUrlSiteId("userOptions/AddOrUpdate"), model);

export const updateOptions = (model: unknown) =>
  request.post(useUrlSiteId("userOptions/UpdateOptions"), model);

export interface UserOptions {
  id: string;
  name: string;
  display: string;
  data: string;
  schema: string;
}

export const getUserOptionsList = () =>
  request.get<UserOptions[]>(useUrlSiteId("userOptions/All"));

export const getUserOptions = (id: string) =>
  request.get<UserOptions>(useUrlSiteId("userOptions/Get"), { id });

export const deletes = (data: { ids: string[] }) =>
  request.post(useUrlSiteId("userOptions/Deletes"), data, undefined, {
    successMessage: $t("common.deleteSuccess"),
  });

export const isUniqueName = (name: string) =>
  request.get(
    useUrlSiteId("UserOptions/isUniqueName"),
    { name: name },
    { hiddenLoading: true, hiddenError: true }
  );
