import type { PaginationResponse } from "@/global/types";
import { useUrlSiteId } from "@/hooks/use-site-id";
import request from "@/utils/request";
import type { Error, TopPage, VisitorLog, TopImage } from "./types";

interface ErrorList {
  dataList: Error[];
  pageNr: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}
export const getWeeks = () => {
  return request.get<string[]>(useUrlSiteId("VisitorLog/WeekNames"));
};

export const getTopPages = (weekname: string) => {
  return request.get<TopPage[]>(useUrlSiteId("VisitorLog/TopPages"), {
    weekname,
  });
};

export const getTopBots = (weekname: string) => {
  return request.get<TopPage[]>(useUrlSiteId("VisitorLog/TopBots"), {
    weekname,
  });
};

export const getTopReferer = (weekname: string) => {
  return request.get<TopPage[]>(useUrlSiteId("VisitorLog/TopReferer"), {
    weekname,
  });
};

export const getTopImages = (weekname: string) => {
  return request.get<TopImage[]>(useUrlSiteId("VisitorLog/TopImages"), {
    weekname,
  });
};

export const getMonthly = () => {
  return request.get<TopPage[]>(useUrlSiteId("VisitorLog/Monthly"));
};

export const getErrorList = (weekname: string, pageNr?: number) => {
  return request.get<ErrorList>(useUrlSiteId("VisitorLog/errors"), {
    weekname,
    pageNr,
  });
};

export const getList = (weekname: string, pageNr?: number) => {
  return request.get<PaginationResponse<VisitorLog>>(
    useUrlSiteId("VisitorLog/list"),
    {
      weekname,
      pageNr,
    }
  );
};

export const getBotList = (weekname: string, pageNr?: number) => {
  return request.get<PaginationResponse<VisitorLog>>(
    useUrlSiteId("VisitorLog/BotList"),
    {
      weekname,
      pageNr,
    }
  );
};
