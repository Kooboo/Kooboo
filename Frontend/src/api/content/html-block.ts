import { useUrlSiteId } from "@/hooks/use-site-id";
import request from "@/utils/request";
import { i18n } from "@/modules/i18n";
const $t = i18n.global.t;

export type HtmlBlock = {
  id: string;
  keyHash: string;
  lastModified: string;
  name: string;
  relations: Record<string, number>;
  storeNameHash: number;
  values: Record<string, string>;
};

export type HtmlBlockItem = {
  id: string;
  name: string;
  values: Record<string, string>;
};

export const getList = () =>
  request.get<HtmlBlock[]>(useUrlSiteId("HtmlBlock/list"));

export const getHtmlBlock = (params: { id?: string; name?: string }) =>
  request.get<HtmlBlockItem>(useUrlSiteId("HtmlBlock/Get"), params);

export const deletes = (data: { ids: string[] }) =>
  request.post(useUrlSiteId("HtmlBlock/Deletes"), data, undefined, {
    successMessage: $t("common.deleteSuccess"),
  });

export const update = (data: {
  id: string;
  name: string;
  values: Record<string, string>;
}) =>
  request.post(useUrlSiteId("HtmlBlock/post"), data, undefined, {
    successMessage: $t("common.saveSuccess"),
  });

export const isUniqueName = (name: string) =>
  request.get(
    useUrlSiteId("HtmlBlock/isUniqueName"),
    { name },
    { hiddenLoading: true, hiddenError: true }
  );
