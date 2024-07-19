import type { ResourceType, ScriptModule } from "./types";
import request, { api } from "@/utils/request";

import { downloadFromResponse } from "@/utils/dom";
import { i18n } from "@/modules/i18n";
import { useUrlSiteId } from "@/hooks/use-site-id";

const $t = i18n.global.t;

export const getList = () =>
  request.get<ScriptModule[]>(useUrlSiteId("ScriptModule/list", true));

export const createModule = (name: string) =>
  request.post<ScriptModule>(
    useUrlSiteId("ScriptModule/create"),
    { name },
    undefined,
    {
      successMessage: $t("common.createSuccess"),
    }
  );

export const deleteModules = (ids: string[]) =>
  request.post(useUrlSiteId("ScriptModule/Deletes"), { ids }, undefined, {
    successMessage: $t("common.deleteSuccess"),
  });

export const updateStatus = (id: string, status: boolean) =>
  request.post(useUrlSiteId("ScriptModule/UpdateStatus"), {
    moduleId: id,
    status,
  });

export const getType = () =>
  request.get<ResourceType[]>(useUrlSiteId("ScriptModule/ResourceType"));

export const exportModule = (name: string) => {
  api
    .get<Blob>(useUrlSiteId("/ScriptModule/export"), {
      params: { name },
      responseType: "blob",
      timeout: 1000 * 60 * 30,
    })
    .then((response) => {
      downloadFromResponse(response);
    });
};

export const importModule = (params: unknown) =>
  request.post<string>(
    useUrlSiteId("/ScriptModule/Import"),
    params,
    undefined,
    {
      timeout: 1000 * 60 * 30,
      hiddenLoading: true,
      hiddenError: true,
      successMessage: $t("common.importSuccess"),
      errorMessage: $t("common.importFailed"),
      keepShowErrorMessage: true,
    }
  );

export const shareModule = (id: string) =>
  request.post(
    useUrlSiteId("ScriptModule/share"),
    {
      id,
    },
    undefined,
    {
      successMessage: $t("common.shareSuccess"),
    }
  );
