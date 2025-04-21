import type { KeyValue } from "@/global/types";
import { useUrlSiteId } from "@/hooks/use-site-id";
import request from "@/utils/request";

export type CodeLog = {
  level: string;
  dateTime: string;
  category: string;
  traceId: string;
  message: string;
};
export type CodeLogResponse = {
  total: number;
  pageIndex: number;
  pageSize: number;
  pageCount: number;
  list: CodeLog[];
};
export const query = (data: unknown) =>
  request.post<CodeLogResponse>(useUrlSiteId("CodeLog/query"), data);

export const getWeeks = () => {
  return request.get<KeyValue[]>(useUrlSiteId("CodeLog/WeekNames"));
};
