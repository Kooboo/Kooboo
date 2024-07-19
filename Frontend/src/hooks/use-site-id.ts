import type { RouteLocationRaw, RouteQueryAndHash } from "vue-router";
import { getQueryString, updateQueryString } from "@/utils/url";

export function useUrlSiteId(url: string, determineDevMode?: boolean): string {
  const siteId = getQueryString("siteid");

  if (siteId) {
    const args: Record<string, any> = {
      SiteId: siteId,
    };

    if (determineDevMode) {
      args["devMode"] = window.location.pathname
        .toLowerCase()
        .includes("/dev-mode");
    }

    url = updateQueryString(url, args);
  }

  return url;
}

export function useRouteSiteId(route: RouteLocationRaw): RouteLocationRaw {
  const siteId = getQueryString("siteid");

  if (siteId) {
    if (!(route as RouteQueryAndHash).query) {
      (route as RouteQueryAndHash).query = {};
    }

    const query = (route as RouteQueryAndHash).query;
    if (query) query.SiteId = siteId;
  }

  return route;
}

export function useRouteEmailId(route: RouteLocationRaw): RouteLocationRaw {
  const address = getQueryString("address");

  if (address) {
    if (!(route as RouteQueryAndHash).query) {
      (route as RouteQueryAndHash).query = {};
    }

    const query = (route as RouteQueryAndHash).query;
    if (query) query.address = address;
  }

  return route;
}
