import type { LighthouseItem, Site, SiteViewModel } from "./site";
import request, { api } from "@/utils/request";

import type { KeyValue } from "@/global/types";
import { i18n } from "@/modules/i18n";
import { useUrlSiteId } from "@/hooks/use-site-id";

const $t = i18n.global.t;

export const getSiteList = () => request.get<SiteItem[]>("/Site/list");

export type SiteItem = {
  [key: string]: unknown;
  siteId: string;
  homePageLink: string;
  online: boolean;
  siteDisplayName: string;
  imageCount: number;
  pageCount: number;
  visitors: number;
  inProgress?: boolean;
};
export type SitePagedlistResponse = {
  list: SiteItem[];
  totalCount: number;
};
export const getSitePagedlist = (
  params: {
    pageNr: number;
    pageSize: number;
    keyword?: string;
    folder?: string;
  },
  hiddenLoading?: boolean
) =>
  request.get<SitePagedlistResponse>("/Site/pagedlist", params, {
    hiddenLoading: hiddenLoading,
  });

export const switchStatus = (param: { id: string }) =>
  request.post("/Site/SwitchStatus", { siteId: param.id });
export const deleteSite = (param: { id: string }) =>
  request.post("/Site/Delete", { siteId: param.id }, undefined, {
    successMessage: $t("common.deleteSuccess"),
  });

export const batchSitesDelete = (ids: string[]) =>
  request.post("/Site/BatchDelete", { ids }, undefined, {
    successMessage: $t("common.deleteSuccess"),
  });
export const exportSite = (params: { siteId: string; copyMode: string }) => {
  return request.get<string>("/Site/ExportGenerate", params, {
    timeout: 1000 * 60 * 60,
  });
};
export const exportStore = (params: {
  siteId: string;
  stores: string;
  copyMode: string;
}) =>
  request.get<string>("/Site/ExportStoreGenerate", params, {
    timeout: 1000 * 60 * 60,
  });

export const getKeyValueList = (): Promise<KeyValue[]> =>
  request.post<KeyValue[]>("/Site/KeyValueList");

export type StoreName = {
  [key: string]: unknown;
  name: string;
  displayName: string;
};
export const exportStoreNames = () =>
  request.get<StoreName[]>("/Site/ExportStoreNames");

export const newSite = (params: unknown) =>
  request.post<Site>("/Site/Create", params, undefined, {
    successMessage: $t("common.createSuccess"),
  });
// eslint-disable-next-line @typescript-eslint/no-explicit-any
export const importSite = (
  params: unknown,
  onUploading: (e: { loaded: number; total: number }) => void
) =>
  request.post<string>("/Site/ImportSite", params, undefined, {
    timeout: 1000 * 60 * 60,
    hiddenLoading: true,
    hiddenError: true,
    onUploadProgress: onUploading,
    successMessage: $t("common.importSuccess"),
    errorMessage: $t("common.importFailed"),
    keepShowErrorMessage: true,
  });

export const importChunkPackage = (
  params: unknown,
  onUploading: (e: { loaded: number; total: number }) => void
) =>
  request.post<string>("/Site/MultiChunkUpload", params, undefined, {
    timeout: 1000 * 60 * 60,
    hiddenLoading: true,
    hiddenError: true,
    onUploadProgress: onUploading,
    errorMessage: $t("common.importFailed"),
    keepShowErrorMessage: true,
  });

export const importSiteByUrl = (params: unknown) =>
  request.post("/Site/importurl", params, undefined, {
    timeout: 1000 * 60 * 30,
    hiddenLoading: true,
    hiddenError: true,
    successMessage: $t("common.importSuccess"),
    errorMessage: $t("common.importFailed"),
    keepShowErrorMessage: true,
  });

export const newFolder = (params: { name: string }) =>
  request.post("/Site/NewFolder", params, undefined, {
    successMessage: $t("common.createSuccess"),
  });
export const getFolderList = () =>
  request.get<{ key: string; value: number }[]>("/Site/FolderList");
export const removeFolder = (data: { name: string }) =>
  request.delete("/Site/RemoveFolder", data, undefined, {
    successMessage: $t("common.deleteSuccess"),
  });
export const renameFolder = (params: { name: string; newName: string }) =>
  request.put("/Site/RenameFolder", params, undefined, {
    successMessage: $t("common.saveSuccess"),
  });
export const setSitesFolder = (params: {
  siteIds: string[];
  folderName: string;
}) => request.put("/Site/SetSitesFolder", params);
export const isUniqueName = (name: string) =>
  request.get(
    "/Site/isUniqueName",
    { name },
    {
      hiddenError: true,
      hiddenLoading: true,
    }
  );

export const getSite = () =>
  request.get<SiteViewModel>(useUrlSiteId("Site/Get"), undefined, {
    hiddenError: true,
    errorMessage: $t("common.siteNotFound"),
  });

export const getSiteById = (siteId: string) =>
  request.get<SiteViewModel>("Site/Get", { siteId });

export const getTypes = () =>
  request.get<Record<string, string>>(useUrlSiteId("Site/Types"));

export const getCultures = () =>
  request.get<Record<string, string>>(useUrlSiteId("Site/Cultures"));

export const getLighthouseItems = () =>
  request.get<LighthouseItem[]>(useUrlSiteId("Site/GetLighthouseItems"));

export const uploadPackage = (body: unknown) =>
  request.post(useUrlSiteId("Upload/Package"), body, undefined, {
    timeout: 1000 * 60 * 30,
    headers: { "Content-Type": "multipart/form-data" },
    hiddenError: true,
    successMessage: $t("common.uploadSuccess"),
    errorMessage: $t("common.importFailed"),
    keepShowErrorMessage: true,
  });

export const saveSite = (body: unknown, closeLoadingAndMessage?: boolean) =>
  request.post(
    useUrlSiteId("Site/post"),
    body,
    undefined,
    closeLoadingAndMessage
      ? {
          hiddenLoading: true,
        }
      : {
          successMessage: $t("common.saveSuccess"),
        }
  );

export const updateAdvancedMenus = (body?: string[]) =>
  request.post(useUrlSiteId("Site/UpdateAdvancedMenus"), body, undefined, {
    hiddenLoading: true,
  });

export const saveSiteById = (siteId: string, body: unknown) =>
  request.post(
    "Site/post",
    body,
    { siteId },
    {
      successMessage: $t("common.saveSuccess"),
    }
  );
