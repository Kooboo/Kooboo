import { useUrlSiteId } from "@/hooks/use-site-id";
import request from "@/utils/request";
import { i18n } from "@/modules/i18n";
const $t = i18n.global.t;

export type Condition = {
  left: string;
  operator: string;
  right: string;
};

export type Authentication = {
  id: string;
  matcher: string;
  conditions: Condition[];
  action: string;
  failedAction: string;
  customCodeName?: string;
  customCode: string;
  url: string;
  httpCode: number;
  lastModified?: string;
  name: string;
};

export type AuthTypeName = {
  name: string;
  display: string;
};
export type AuthType = {
  matcher: AuthTypeName[];
  action: AuthTypeName[];
  failedAction: AuthTypeName[];
};

export const getList = () =>
  request.get<Authentication[]>(useUrlSiteId("Authentication/list"));

export const deletes = (data: { ids: string[] }) =>
  request.post(useUrlSiteId("Authentication/Deletes"), data, undefined, {
    successMessage: $t("common.deleteSuccess"),
  });

export const save = (data: Authentication) =>
  request.post(useUrlSiteId("Authentication/post"), data, undefined, {
    successMessage: $t("common.saveSuccess"),
  });

export const getTypes = () =>
  request.get<AuthType>(useUrlSiteId("Authentication/GetTypes"));

export const isUniqueName = (name: string) =>
  request.get(
    useUrlSiteId("Authentication/isUniqueName"),
    { name },
    { hiddenLoading: true, hiddenError: true }
  );
