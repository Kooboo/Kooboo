import type {
  CleanLogRunningTask,
  Compare,
  Log,
  LogOptions,
  TaskStatus,
  Version,
} from "./types";
import request, { api } from "@/utils/request";

import type { PaginationResponse } from "@/global/types";
import { downloadFromResponse } from "@/utils/dom";
import { i18n } from "@/modules/i18n";
import { useUrlSiteId } from "@/hooks/use-site-id";

const $t = i18n.global.t;

export const getLogs = (params: Record<string, unknown>) =>
  request.get<PaginationResponse<Log>>(useUrlSiteId("SiteLog/list"), params);

export const getOptions = () =>
  request.get<LogOptions>(useUrlSiteId("SiteLog/Options"));

export const saveLogVideo = (
  id: string,
  videoData: string,
  isEmbedded = false
) =>
  request.post(useUrlSiteId("SiteLog/SaveLogVideo"), {
    id,
    videoData,
    isEmbedded,
  });

export const getLogVideo = (versionId: number) =>
  request.get(useUrlSiteId("SiteLog/GetLogVideo"), { versionId });

export const getVersions = (params: {
  keyHash: string;
  storeNameHash: string;
  tableNameHash?: string;
}) => request.get<Version[]>(useUrlSiteId("SiteLog/Versions"), params);

export const blame = (body: number[]) =>
  request.post(useUrlSiteId("SiteLog/Blame"), body, undefined, {
    successMessage: $t("common.undoChangeSuccess"),
  });

export const restore = (id: number) =>
  request.post(useUrlSiteId("SiteLog/Restore"), { id }, undefined, {
    successMessage: $t("common.restoreSuccess"),
  });

export const checkout = (body: unknown) =>
  request.post(useUrlSiteId("SiteLog/CheckOut"), body, undefined, {
    successMessage: $t("common.checkoutSuccess"),
  });

export const revert = (id: number) =>
  request.post(useUrlSiteId("SiteLog/Revert"), { id });

export const getCompare = (id1: string, id2: string) =>
  request.get<Compare>(useUrlSiteId("SiteLog/Compare"), { id1, id2 });

export const exportBatch = (id: number) =>
  request
    .get<string>(useUrlSiteId("SiteLog/ExportBatch"), { id })
    .then((rsp) => {
      api
        .get<Blob>(useUrlSiteId("/SiteLog/DownloadBatch"), {
          params: { id: rsp },
          responseType: "blob",
        })
        .then((response) => {
          downloadFromResponse(response);
        });
    });

export const exportItems = (ids: number[]) =>
  request
    .post<string>(useUrlSiteId("SiteLog/ExportItems"), { ids })
    .then((rsp) => {
      api
        .get<Blob>(useUrlSiteId("/SiteLog/DownloadBatch"), {
          params: { id: rsp },
          responseType: "blob",
        })
        .then((response) => {
          downloadFromResponse(response);
        });
    });

export const cleanLogApi = () =>
  request.get<string>(useUrlSiteId("SiteLog/cleanLog"));
export const cleanLogStatusApi = (taskId: string) =>
  request.get<TaskStatus>(useUrlSiteId("SiteLog/CleanLogStatus"), {
    TaskId: taskId,
  });
export const GetCleanLogRunningTask = () =>
  request.get<CleanLogRunningTask>(
    useUrlSiteId("SiteLog/GetCleanLogRunningTask")
  );
