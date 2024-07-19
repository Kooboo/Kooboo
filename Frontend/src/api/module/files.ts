import { useUrlSiteId } from "@/hooks/use-site-id";
import request from "@/utils/request";
import type { ModuleFileInfo } from "./types";
import { i18n } from "@/modules/i18n";
const $t = i18n.global.t;

export const getAllFiles = (moduleid: string) =>
  request.get<ModuleFileInfo[]>(useUrlSiteId("ModuleFiles/AllFiles"), {
    moduleid,
  });

export const updateFile = (
  type: string,
  moduleid: string,
  fileName: string,
  content: string
) =>
  request.post<ModuleFileInfo[]>(useUrlSiteId("ModuleFiles/UpdateText"), {
    moduleid,
    folder: "/",
    fileName,
    content,
    objectType: type,
  });

export const updateBase64 = (
  type: string,
  moduleid: string,
  fileName: string,
  content: string
) =>
  request.post<ModuleFileInfo[]>(
    useUrlSiteId("ModuleFiles/UpdateBinary"),
    {
      moduleid,
      folder: "/",
      fileName,
      base64: content,
      objectType: type,
    },
    undefined,

    { hiddenError: true, successMessage: $t("common.uploadSuccess") }
  );

export const removeFile = (type: string, moduleid: string, fileName: string) =>
  request.post(
    useUrlSiteId("ModuleFiles/Remove"),
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

export const getText = (type: string, moduleid: string, fileName: string) =>
  request.post<string>(useUrlSiteId("ModuleFiles/GetText"), {
    moduleid,
    fileName,
    folder: "/",
    objectType: type,
  });

export const getTextByName = (
  type: string,
  moduleName: string,
  fileName: string
) =>
  request.post<string>(
    useUrlSiteId("ModuleFiles/GetTextByName"),
    {
      moduleName,
      fileName,
      folder: "/",
      objectType: type,
    },
    undefined,
    {
      hiddenError: true,
      hiddenLoading: true,
    }
  );
