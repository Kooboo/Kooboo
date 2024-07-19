import type { ResolvedRoute } from "./types";
import request from "@/utils/request";
import { useUrlSiteId } from "@/hooks/use-site-id";

export const resolveRoute = (url: string) => {
  return request.post<ResolvedRoute>(useUrlSiteId("Route/Resolve"), { url });
};

export const routesByType = (type: string) => {
  return request.get<ResolvedRoute[]>(useUrlSiteId("Route/ListByType"), {
    type,
  });
};

export const isUniqueRoute = (name: string, oldName?: string) =>
  request.get(
    useUrlSiteId("Route/IsUniqueName"),
    { name, oldName },
    { hiddenLoading: true, hiddenError: true }
  );
