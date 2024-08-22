import { useUrlSiteId } from "@/hooks/use-site-id";
import request from "@/utils/request";
import type {
  ByLevelBody,
  Domain,
  TransferReponse,
  TransferSingle,
} from "./types";
import { i18n } from "@/modules/i18n";
const $t = i18n.global.t;

export const checkDomainBindingAvailable = (body: Domain) =>
  request.post("/Site/CheckDomainBindingAvailable", body, undefined, {
    hiddenError: true,
    hiddenLoading: true,
  });

export const byLevel = (body: ByLevelBody) =>
  request.post<{ siteId: string; taskId: string }>(
    "/Transfer/ByLevel",
    body,
    undefined,
    {
      timeout: 60000 * 5,
    }
  );

export const byPage = (body: unknown) =>
  request.post<{ siteId: string; taskId: string }>(
    "/Transfer/ByPage",
    body,
    undefined,
    {
      timeout: 60000 * 5,
    }
  );

export const getTaskStatus = (siteId: string) =>
  request.get<{ done: boolean }>("/Transfer/GetTaskStatus", { siteId });

export const single = (body: TransferSingle) =>
  request.post<TransferReponse>(
    useUrlSiteId("Transfer/Single"),
    body,
    undefined,
    {
      successMessage: $t("common.importSuccess"),
    }
  );

export const getStatus = (body: unknown) =>
  request.post<{ url: string; done: boolean }[]>(
    "Transfer/GetStatus",
    body,
    undefined,
    { hiddenLoading: true, hiddenError: true }
  );

export const getSubUrl = (url: string, pages: number) =>
  request.get<string[]>("/Transfer/GetSubUrl", { url, pages });
