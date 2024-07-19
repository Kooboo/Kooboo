import { useUrlSiteId } from "@/hooks/use-site-id";
import request, { api } from "@/utils/request";
import { i18n } from "@/modules/i18n";
import { downloadFromResponse } from "@/utils/dom";

const $t = i18n.global.t;

export type Label = {
  id: string;
  keyHash?: string;
  lastModified?: string;
  name?: string;
  relations?: Record<string, number>;
  storeNameHash?: number;
  values: Record<string, string>;
  relationDetails?: {
    name: string;
    id: string;
    type: string;
  }[];
};

export type ScanList = {
  isDom: boolean;
  isTextNode: boolean;
  attributeName: string;
  key: string;
  displayName: string;
  type: string;
  koobooId: string;
  constType: number;
  objectId: string;
  attribute: string;
  tag: string;
  suggestKey: string;
};

export const getList = () => request.get<Label[]>(useUrlSiteId("Label/list"));

export const deletes = (data: { ids: string[] }) =>
  request.post(useUrlSiteId("Label/Deletes"), data, undefined, {
    successMessage: $t("common.deleteSuccess"),
  });

export const update = (data: { id: string; values: Record<string, string> }) =>
  request.post(useUrlSiteId("Label/Update"), data, undefined, {
    successMessage: $t("common.saveSuccess"),
  });

export const get = (name: string) =>
  request.get<Label>(useUrlSiteId("Label/get"), { name });

export const Scan = () => request.get<ScanList[]>(useUrlSiteId("Label/scan"));
export const ConfirmAdd = (data: ScanList[]) =>
  request.post(useUrlSiteId("Label/confirmAdd"), data, undefined, {
    successMessage: $t("common.saveSuccess"),
  });

export const ScanLabel = () =>
  request.get<ScanList[]>(useUrlSiteId("Label/ScanI18n"));
export const ConfirmI18NAdd = (data: ScanList[]) =>
  request.post(useUrlSiteId("Label/ConfirmI18NAdd"), data, undefined, {
    successMessage: $t("common.saveSuccess"),
  });

export const create = (data: { key: string; values: Record<string, string> }) =>
  request.post(useUrlSiteId("Label/create"), data, undefined, {
    successMessage: $t("common.createSuccess"),
  });

export const exportLabel = () => {
  api
    .get<Blob>(useUrlSiteId("/Label/export"), {
      responseType: "blob",
      timeout: 1000 * 60 * 30,
    })
    .then((response) => {
      downloadFromResponse(response);
    });
};

export const importLabel = (params: unknown) =>
  request.post<string>(useUrlSiteId("/Label/Import"), params, undefined, {
    timeout: 1000 * 60 * 30,
    hiddenLoading: true,
    hiddenError: true,
    successMessage: $t("common.importSuccess"),
    errorMessage: $t("common.importFailed"),
    keepShowErrorMessage: true,
  });
