import { useUrlSiteId } from "@/hooks/use-site-id";
import request from "@/utils/request";
import { i18n } from "@/modules/i18n";
const $t = i18n.global.t;

export interface Role {
  displayName: string;
  id: string;
  name: string;
  selected: boolean;
  subItems: Role[];
}

export const getList = () => request.get<Role[]>(useUrlSiteId("Role/list"));

export const isUniqueName = (name: string) =>
  request.get(
    useUrlSiteId("Role/isUniqueName"),
    { name },
    { hiddenError: true, hiddenLoading: true }
  );

export const getEdit = () =>
  request.get<{ subItems: never[] }>(useUrlSiteId("Role/GetEdit"));

export const post = (body: unknown) =>
  request.post(useUrlSiteId("Role/post"), body);

export const deletes = (ids: string[]) =>
  request.post(useUrlSiteId("Role/Deletes"), { ids }, undefined, {
    successMessage: $t("common.deleteSuccess"),
  });
