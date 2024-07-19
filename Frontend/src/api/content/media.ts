import type { DownloadRequest } from "./download-request-type";
import type { RouteLocationRaw } from "vue-router";
import { i18n } from "@/modules/i18n";
import request from "@/utils/request";
import { useUrlSiteId } from "@/hooks/use-site-id";

const $t = i18n.global.t;

export type MediaFolderItem = {
  [key: string]: unknown;
  fullPath: string;
  name: string;
  lastModified: Date;
  count: number;
};
export type MediaFileItem = {
  [key: string]: unknown;
  id: string;
  key: string;
  name: string;
  alt?: string;
  isImage: boolean;
  thumbnail: string;
  url: string;
  lastModified: Date;
  size: number;
  references?: Record<string, number>;
  relations?: Record<string, number>;
  previewUrl: string;
  downloadUrl?: string;
  loading?: boolean;
  mimeType?: string;
  base64?: string;
};
export type MediaPagedList = {
  errorMessage?: string;
  folders?: MediaFolderItem[];
  files: {
    infinite: boolean;
    list: MediaFileItem[];
    totalCount: number;
    pageSize: number;
    totalPages: number;
    pageNr: number;
  };
  crumbPath: CrumbPathItem[];
  providers?: string[];
};

export type CrumbPathItem = {
  name: string;
  fullPath?: string;
  route?: RouteLocationRaw;
};

export const getMediaPagedList = (
  params: {
    pageNr: number;
    pageSize: number;
    path: string;
    provider?: string;
    keyword?: string;
    startAfter?: string;
  },
  hiddenLoading?: boolean
) =>
  request.get<MediaPagedList>(useUrlSiteId("Media/PagedList"), params, {
    hiddenLoading,
  });

export const getMediaPagedListBy = (
  params: {
    pageNr: number;
    pageSize: number;
    by: string;
    provider?: string;
    startAfter?: string;
  },
  hiddenLoading?: boolean
) =>
  request.get<MediaPagedList>(useUrlSiteId("Media/PagedListBy"), params, {
    hiddenLoading,
  });

export const getUploadActionUrl = (provider?: string) =>
  useUrlSiteId(
    `${import.meta.env.VITE_API}/Upload/Image?provider=${provider || "default"}`
  );

export const createMediaFolder = (data: {
  path: string;
  name: string;
  provider: string;
}) =>
  request.post(useUrlSiteId("Media/CreateFolder"), data, undefined, {
    successMessage: $t("common.createSuccess"),
  });

export const deleteFolders = (fullPaths: string[], provider: string) =>
  request.post(
    useUrlSiteId(`Media/DeleteFolders?provider=${provider || "default"}`),
    fullPaths,
    undefined,
    {
      successMessage: $t("common.deleteSuccess"),
    }
  );

export const deleteImages = (keys: string[], provider: string) =>
  request.post(
    useUrlSiteId(`Media/DeleteImages?provider=${provider || "default"}`),
    keys,
    undefined,
    {
      successMessage: $t("common.deleteSuccess"),
    }
  );

export const downloadMediaFile = (req: DownloadRequest, provider: string) =>
  request.post(
    useUrlSiteId(`Download/Images?provider=${provider}`),
    req,
    undefined,
    {}
  );

export type MediaItem = Pick<
  MediaFileItem,
  "key" | "url" | "alt" | "name" | "base64" | "svg"
> & {
  siteUrl: string;
};

export const getMedia = (params: { id: string; provider: string }) =>
  request.get<MediaItem>(useUrlSiteId("Media/Get"), params);

export const imageUpdate = (data: {
  id: string;
  alt: string;
  provider: string;
  base64?: string;
  name: string;
}) =>
  request.post(useUrlSiteId("Media/ImageUpdate"), data, undefined, {
    successMessage: $t("common.saveSuccess"),
  });

export const isUniqueKey = (provider: string, name: string, oldName?: string) =>
  request.get(
    useUrlSiteId(`Upload/IsUniqueKey?provider=${provider}&type=media`),
    { name, oldName },
    { hiddenLoading: true, hiddenError: true }
  );
