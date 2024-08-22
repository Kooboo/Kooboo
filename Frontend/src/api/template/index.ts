import { useUrlSiteId } from "@/hooks/use-site-id";
import request from "@/utils/request";
import type {
  PagedTemplate,
  ShareValidator,
  Template,
  SearchOptions,
  Facet,
} from "./types";
import { i18n } from "@/modules/i18n";
const $t = i18n.global.t;

export const share = (params: {
  siteId: string;
  typeName: string;
  siteName: string;
  coverImage: string;
  screenshot: string;
  zhSiteName: string;
  zhCoverImage: string;
  zhScreenshot: string;
  shareMethod: string;
  templateId: string;
  updateItem: string;
}) =>
  request.post("/Template/Share", params, undefined, {
    successMessage: $t("common.shareSuccess"),
  });

export const getList = (params: Record<string, unknown>) =>
  request.get<PagedTemplate>("/Template/List", params);

export const getTemplateDetail = (Id: string) =>
  request.get<Template>("/Template/Detail", { Id });
export const search = (params: SearchOptions) =>
  request.post<PagedTemplate>("/Template/FullTextSearch", params);

export const use = (body: unknown) =>
  request.post<string>("/Template/Use", body, undefined, {
    successMessage: $t("common.createSuccess"),
  });

export const getType = () => request.get(useUrlSiteId("/Template/type"));
export const shareValidate = (siteId: string) =>
  request.get<ShareValidator>("/Template/shareValidate", { siteId });
export const getPersonalList = () => request.get("/Template/Personal");
