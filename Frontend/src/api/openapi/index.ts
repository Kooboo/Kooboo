import { useUrlSiteId } from "@/hooks/use-site-id";
import request, { api } from "@/utils/request";
import type { OpenApi, Template, Authorize } from "./types";
import { i18n } from "@/modules/i18n";
const $t = i18n.global.t;

export const get = (id: string) =>
  request.get<OpenApi>(useUrlSiteId("OpenApi/get"), { id });

export const updateDoc = (id: string) =>
  request.post(useUrlSiteId("OpenApi/UpdateDoc"), { id }, undefined, {
    successMessage: $t("common.updateSuccess"),
  });

export const getList = () =>
  request.get<OpenApi[]>(useUrlSiteId("OpenApi/list"));

export const post = (openApi: OpenApi) =>
  request.post<string>(useUrlSiteId("OpenApi/post"), openApi, undefined, {
    successMessage: $t("common.saveSuccess"),
  });

export const deletes = (ids: string[]) =>
  request.post(useUrlSiteId("OpenApi/Deletes"), { ids }, undefined, {
    successMessage: $t("common.deleteSuccess"),
  });

export const isUniqueName = (name: string) =>
  request.get(
    useUrlSiteId("OpenApi/IsUniqueAuthorizeName"),
    { name },
    { hiddenLoading: true, hiddenError: true }
  );

export const getTemplates = () =>
  api.get<Template[]>("https://openapi_template.kooboo.net/list");

export const getAuthorize = (id: string) =>
  request.get<Authorize>(useUrlSiteId("OpenApi/GetAuthorize"), { id });

export const getAuthorizes = (id: string) =>
  request.get<Authorize[]>(useUrlSiteId("OpenApi/GetAuthorizes"), { id });

export const postAuthorizes = (authorize: Authorize) =>
  request.post<string>(
    useUrlSiteId("OpenApi/PostAuthorize"),
    authorize,
    undefined,
    {
      successMessage: $t("common.saveSuccess"),
    }
  );

export const deleteAuthorizes = (ids: string[]) =>
  request.post(useUrlSiteId("OpenApi/DeleteAuthorizes"), ids, undefined, {
    successMessage: $t("common.deleteSuccess"),
  });
