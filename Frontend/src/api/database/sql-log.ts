import type { Pagination } from "@/global/types";
import { useUrlSiteId } from "@/hooks/use-site-id";
import request from "@/utils/request";

export type SqlLog = {
  dateTime: string;
  params: string;
  siteId: string;
  sql: string;
  type: string;
};
export type SqlLogResponse = Pagination & {
  list: SqlLog[];
};
export const getList = (params: {
  type?: string;
  week: string;
  keyword?: string;
  pageIndex: number;
}) => request.get<SqlLogResponse>(useUrlSiteId("SqlLog/List"), params);

export const getWeeks = () =>
  request.get<string[]>(useUrlSiteId("SqlLog/weeks"));
