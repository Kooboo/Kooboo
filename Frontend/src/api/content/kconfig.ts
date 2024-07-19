import { useUrlSiteId } from "@/hooks/use-site-id";
import request from "@/utils/request";
import { i18n } from "@/modules/i18n";
const $t = i18n.global.t;

export type KConfig = {
  id: string;
  binding: Record<string, string>;
  keyHash: string;
  lastModified: string;
  name: string;
  relations: Record<string, number>;
  storeNameHash: number;
  tagHtml: string;
  tagName: string;
  controlType?: string;
};

export const getList = () =>
  request.get<KConfig[]>(useUrlSiteId("KConfig/list"));

export const getKConfig = (params: { id?: string; name?: string }) =>
  request.get<KConfig>(useUrlSiteId("KConfig/Get"), params);

export const deletes = (data: { ids: string[] }) =>
  request.post(useUrlSiteId("KConfig/Deletes"), data, undefined, {
    successMessage: $t("common.deleteSuccess"),
  });

export const update = (data: { id: string; binding: Record<string, string> }) =>
  request.post(useUrlSiteId("KConfig/Update"), data, undefined, {
    successMessage: $t("common.saveSuccess"),
  });
