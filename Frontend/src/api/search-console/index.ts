import request from "@/utils/request";
import type { Site, Sitemap } from "./types";

export const getAuthUrl = () =>
  request.get<string>("SearchConsole/GetAuthUrl", {
    callbackUrl: location.href,
  });

export const getSiteList = () =>
  request.get<Site[]>("SearchConsole/GetSiteList", undefined, {
    hiddenError: true,
  });

export const deleteSite = (siteUrl: string) =>
  request.post("SearchConsole/DeleteSite", { siteUrl });

export const addSite = (siteUrl: string) =>
  request.post("SearchConsole/AddSite", { siteUrl });

export const validSite = (siteUrl: string, onGoogle: boolean) =>
  request.post("SearchConsole/ValidSite", { siteUrl, onGoogle });

export const getSitemapList = (siteUrl: string) =>
  request.get<Sitemap[]>("SearchConsole/GetSitemapList", { siteUrl });

export const deleteSitemap = (siteUrl: string, feedPath: string) =>
  request.post("SearchConsole/DeleteSitemap", { siteUrl, feedPath });

export const submitSitemap = (siteUrl: string, feedPath: string) =>
  request.post("SearchConsole/SubmitSitemap", { siteUrl, feedPath });

export const getFeeds = (domain: string) =>
  request.post<string[]>("sitemap/Feeds", { domain });

export const getSearchAnalytics = (body: any) =>
  request.post<any>("SearchConsole/GetSearchAnalytics", body);
