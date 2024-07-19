import type {
  CrumbPathItem,
  MediaFileItem,
  MediaFolderItem,
  MediaItem,
  MediaPagedList,
} from "./media";

import type { DownloadRequest } from "./download-request-type";
import { i18n } from "@/modules/i18n";
import request from "@/utils/request";
import { useUrlSiteId } from "@/hooks/use-site-id";

const $t = i18n.global.t;

export type { MediaFolderItem, MediaFileItem, CrumbPathItem, MediaItem };

export type FileList = MediaPagedList;
export const getList = (params: {
  pageNr: number;
  pageSize: number;
  path: string;
  keyword?: string;
  provider: string;
  startAfter?: string;
}) => request.get<FileList>(useUrlSiteId("File/list"), params);

export const getUploadActionUrl = (provider = "default") =>
  useUrlSiteId(`${import.meta.env.VITE_API}/Upload/File?provider=${provider}`);

export const getUploadCsvActionUrl = () =>
  useUrlSiteId(`${import.meta.env.VITE_API}/Upload/ReadCsv`);

export const uploadFiles = (data: unknown) =>
  request.post(useUrlSiteId("Upload/File"), data, undefined, {
    successMessage: $t("common.saveSuccess"),
  });

export const downloadFiles = (req: DownloadRequest, provider: string) =>
  request.post(
    useUrlSiteId(`Download/Files?provider=${provider}`),
    req,
    undefined,
    {}
  );

export const createFileFolder = (data: {
  path: string;
  name: string;
  provider: string;
}) =>
  request.post(useUrlSiteId(`File/CreateFolder`), data, undefined, {
    successMessage: $t("common.saveSuccess"),
  });

export const deleteFolders = (fullPaths: string[], provider: string) =>
  request.post(
    useUrlSiteId(`File/DeleteFolders?provider=${provider}`),
    fullPaths,
    undefined,
    {
      successMessage: $t("common.deleteSuccess"),
    }
  );

export const deleteFiles = (fileIds: string[], provider: string) =>
  request.post(
    useUrlSiteId(`File/DeleteFiles?provider=${provider}`),
    fileIds,
    undefined,
    {
      successMessage: $t("common.deleteSuccess"),
    }
  );

export const isUniqueKey = (provider: string, name: string, oldName?: string) =>
  request.get(
    useUrlSiteId(`Upload/IsUniqueKey?provider=${provider}&type=file`),
    { name, oldName },
    { hiddenLoading: true, hiddenError: true }
  );
