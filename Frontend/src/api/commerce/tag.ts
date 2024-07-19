import { useUrlSiteId } from "@/hooks/use-site-id";
import request from "@/utils/request";
import { i18n } from "@/modules/i18n";
const $t = i18n.global.t;

export const getTags = (type: string) =>
  request.get<string[]>(useUrlSiteId("CommerceTag/list"), { type });

export const deleteTag = (type: string, name: string) =>
  request.post(useUrlSiteId("CommerceTag/Delete"), { type, name });
