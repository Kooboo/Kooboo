import type { Property, PropertyJsonString } from "@/global/control-type";

import { i18n } from "@/modules/i18n";
import request from "@/utils/request";
import { useUrlSiteId } from "@/hooks/use-site-id";

const $t = i18n.global.t;

export type ContentTypeItem = {
  id: string;
  name: string;
  propertyCount: number;
};

export type ContentType = {
  id: string;
  name: string;
  properties: Property[];
};
export const getContentType = (params: { id: string }) =>
  request.get<ContentType>(useUrlSiteId("ContentType/Get"), params);

export const getList = () =>
  request.get<ContentTypeItem[]>(useUrlSiteId("ContentType/list"));

export const deletes = (data: { ids: string[] }) =>
  request.post(useUrlSiteId("ContentType/Deletes"), data, undefined, {
    successMessage: $t("common.deleteSuccess"),
  });

export const save = (data: {
  id: string;
  name: string;
  properties: PropertyJsonString[];
}) =>
  request.post(useUrlSiteId("ContentType/Post"), data, undefined, {
    successMessage: $t("common.saveSuccess"),
  });

export const isUniqueName = (name: string) =>
  request.get(
    useUrlSiteId("ContentType/isUniqueName"),
    { name },
    { hiddenLoading: true, hiddenError: true }
  );
