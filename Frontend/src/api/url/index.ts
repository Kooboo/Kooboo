import type { KeyValue, PaginationResponse } from "@/global/types";

import type { UrlItem } from "./types";
import { i18n } from "@/modules/i18n";
import request from "@/utils/request";
import { useUrlSiteId } from "@/hooks/use-site-id";

export type { UrlItem } from "./types";

const $t = i18n.global.t;

export type GetInternalParams = GetExternalParams & {
  hasObject?: boolean;
};

export type GetExternalParams = {
  pageNr?: number;
  keyword?: string;
  type?: string;
  pageSize: number;
};

export const getInternal = (params: GetInternalParams) =>
  request.get<PaginationResponse<UrlItem>>(
    useUrlSiteId("Url/Internal"),
    params
  );

export const getNotFound = (params: any) =>
  request.get<PaginationResponse<any>>(useUrlSiteId("Url/NotFound"), params);

export const getRoutes = () =>
  request.get<(KeyValue & { parameters: any })[]>(useUrlSiteId("Url/Routes"));

export const getInternalOptions = () =>
  request.get<Record<string, KeyValue[]>>(
    useUrlSiteId("Url/InternalOptions"),
    {}
  );

export const getExternal = (params: GetExternalParams) =>
  request.get<PaginationResponse<UrlItem>>(
    useUrlSiteId("Url/External"),
    params
  );

export const getExternalOptions = () =>
  request.get<Record<string, KeyValue[]>>(
    useUrlSiteId("Url/ExternalOptions"),
    {}
  );
export const updateUrl = (body: unknown) =>
  request.post(useUrlSiteId("Url/UpdateUrl"), body, undefined, {
    successMessage: $t("common.saveSuccess"),
  });

export const makeAlias = (body: unknown) =>
  request.post(useUrlSiteId("Url/MakeAlias"), body, undefined, {
    successMessage: $t("common.saveSuccess"),
  });

export const deletes = (type: string, ids: string[]) =>
  request.post(
    useUrlSiteId("Url/Deletes"),
    {
      type,
      ids: JSON.stringify(ids),
    },
    undefined,
    {
      successMessage: $t("common.deleteSuccess"),
    }
  );

export const getLinks = () =>
  request.get<{ pages: { url: string }[] }>(useUrlSiteId("Link/all"));

export const internalizeResource = (id: string) =>
  request.post(useUrlSiteId("Url/internalizeResource"), null, { id });
