import type { MailModule, SearchResult } from "./types";
import type { ModuleFileInfo, ResourceType } from "../module/types";
import request, { api } from "@/utils/request";

import { downloadFromResponse } from "@/utils/dom";
import { i18n } from "@/modules/i18n";

const $t = i18n.global.t;

export const getMailModuleList = () =>
  request.get<MailModule[]>("/MailModule/List");

export const createMailModule = (name: string) =>
  request.post<MailModule>("/MailModule/Create", { name }, undefined, {
    successMessage: $t("common.createSuccess"),
  });

export const deleteMailModules = (ids: string[]) =>
  request.post<MailModule>("/MailModule/Deletes", { ids }, undefined, {
    successMessage: $t("common.deleteSuccess"),
  });

export const updateMailModuleStatus = (id: string, status: boolean) =>
  request.post("/MailModule/UpdateStatus", {
    moduleId: id,
    status,
  });

export const exportMailModule = (name: string) => {
  api
    .get<Blob>("/MailModule/export", {
      params: { name },
      responseType: "blob",
      timeout: 1000 * 60 * 30,
    })
    .then((response) => {
      downloadFromResponse(response);
    });
};

export const importMailModule = (params: unknown) =>
  request.post<string>("/MailModule/Import", params, undefined, {
    timeout: 1000 * 60 * 30,
    hiddenLoading: true,
    hiddenError: true,
    successMessage: $t("common.importSuccess"),
  });

export const shareMailModule = (id: string) =>
  request.post(
    "MailModule/share",
    {
      id,
    },
    undefined,
    {
      successMessage: $t("common.shareSuccess"),
    }
  );

export const searchMailModule = (keyword: string) =>
  request.post<SearchResult[]>("/MailModule/Search", {
    keyword,
  });

export const installMailModule = (packageId: string, name: string) =>
  request.post<SearchResult[]>(
    "/MailModule/InstallModule",
    {
      packageId,
      name,
    },
    undefined,
    {
      hiddenLoading: true,
      hiddenError: true,
    }
  );

export const getMailModuleFileType = () =>
  request.get<ResourceType[]>("MailModule/ResourceType");

export const getMailModuleSetting = (id: string) =>
  request.get<string>("MailModuleSetting/GetSetting", {
    id,
  });

export const updateMailModuleSetting = (id: string, json: string) =>
  request.post(
    "MailModuleSetting/UpdateSetting",
    {
      id,
      json,
    },
    undefined,
    {
      successMessage: $t("common.saveSuccess"),
    }
  );

export const getMailModuleText = (
  type: string,
  moduleid: string,
  fileName: string
) =>
  request.post<string>("MailModuleFiles/GetText", {
    moduleid,
    fileName,
    folder: "/",
    objectType: type,
  });

export const getMailModuleAllFiles = (moduleid: string) =>
  request.get<ModuleFileInfo[]>("MailModuleFiles/AllFiles", {
    moduleid,
  });

export const updateMailModuleBase64 = (
  type: string,
  moduleid: string,
  fileName: string,
  content: string
) =>
  request.post<ModuleFileInfo[]>(
    "MailModuleFiles/UpdateBinary",
    {
      moduleid,
      folder: "/",
      fileName,
      base64: content,
      objectType: type,
    },
    undefined
  );

export const updateMailModuleFile = (
  type: string,
  moduleid: string,
  fileName: string,
  content: string,
  successMessage: string
) =>
  request.post<ModuleFileInfo[]>(
    "MailModuleFiles/UpdateText",
    {
      moduleid,
      folder: "/",
      fileName,
      content,
      objectType: type,
    },
    undefined,
    {
      successMessage: successMessage,
    }
  );

export const removeMailModuleFile = (
  type: string,
  moduleid: string,
  fileName: string
) =>
  request.post(
    "MailModuleFiles/Remove",
    {
      moduleid,
      fileName,
      folder: "/",
      objectType: type,
    },
    undefined,
    {
      successMessage: $t("common.deleteSuccess"),
    }
  );
