import { useUrlSiteId } from "@/hooks/use-site-id";
import request from "@/utils/request";
import { i18n } from "@/modules/i18n";
const $t = i18n.global.t;

export type WeekName = string;

export type IndexStat = {
  diskSize: number;
  docCount: number;
  enableFullTextSearch: boolean;
  wordCount: number;
};

export type Lastest = {
  ip: string;
  keywords: string;
  badge: string;
  time: string;
};

export type SearchStat = {
  [key: string]: number;
};

export const getWeekNames = () =>
  request.get<WeekName[]>(useUrlSiteId("Search/WeekNames"));

export const getIndexStat = () =>
  request.get<IndexStat>(useUrlSiteId("Search/IndexStat"));

export const getLastest = () =>
  request.get<Lastest[]>(useUrlSiteId("Search/Lastest"));

export const SearchStat = (params: { weekname: string }) =>
  request.get<SearchStat>(useUrlSiteId("Search/SearchStat"), params);

export const Enable = (params: { rebuild: boolean }) =>
  request.post<IndexStat>(useUrlSiteId("Search/Enable"), params, undefined, {
    successMessage: $t("common.enableSuccess"),
    errorMessage: $t("common.enableFailed"),
  });

export const Disable = () =>
  request.post<null>(useUrlSiteId("Search/Disable"), undefined, undefined, {
    successMessage: $t("common.disableSuccess"),
    errorMessage: $t("common.disableFailed"),
  });

export const Rebuild = () =>
  request.post<IndexStat>(
    useUrlSiteId("Search/Rebuild"),
    undefined,
    undefined,
    {
      successMessage: $t("common.rebuildSuccess"),
      errorMessage: $t("common.rebuildFailed"),
    }
  );

export const Clean = () =>
  request.get<IndexStat>(useUrlSiteId("Search/Clean"), undefined, {
    successMessage: $t("common.cleanSuccess"),
    errorMessage: $t("common.cleanFailed"),
  });
