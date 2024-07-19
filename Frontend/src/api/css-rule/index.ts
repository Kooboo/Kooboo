import { useUrlSiteId } from "@/hooks/use-site-id";
import request from "@/utils/request";
import type { InlineItem, InlineStyle } from "./types";
import { i18n } from "@/modules/i18n";
const $t = i18n.global.t;

export const getEdit = (id: string) =>
  request.get<InlineStyle>(useUrlSiteId("CSSRule/getInline"), { id });

export const getList = () =>
  request.get<InlineItem[]>(useUrlSiteId("CSSRule/InlineList"));

export const updateInline = (body: { id: string; ruleText: string }) =>
  request.post<string>(useUrlSiteId("CSSRule/UpdateInline"), body);

export const deletes = (body: Record<string, unknown>) =>
  request.post(useUrlSiteId("CSSRule/Deletes"), body, undefined, {
    successMessage: $t("common.deleteSuccess"),
  });
