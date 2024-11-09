import { useUrlSiteId } from "@/hooks/use-site-id";
import request from "@/utils/request";
import { i18n } from "@/modules/i18n";
const $t = i18n.global.t;

export interface Term {
  name: string;
  type: "Selection" | "Custom";
  valueType: string;
  options: string[];
}

export interface ProductTypeCreate {
  name: string;
  attributes: Term[];
  options: Term[];
}

export interface ProductType {
  id?: string;
  name: string;
  attributes: Term[];
  options: Term[];
  isDigital: boolean;
  maxDownloadCount?: number;
}

export const getProductTypes = () =>
  request.get<ProductType[]>(useUrlSiteId("productType/list"));

export const getProductType = (id: string) =>
  request.get<ProductType>(useUrlSiteId("productType/GetEdit"), { id });

export const createProductType = (body: unknown) =>
  request.post(useUrlSiteId("productType/create"), body, undefined, {
    successMessage: $t("common.saveSuccess"),
  });

export const editProductType = (body: unknown) =>
  request.post(useUrlSiteId("productType/edit"), body, undefined, {
    successMessage: $t("common.saveSuccess"),
  });

export const deleteProductTypes = (ids: string[]) =>
  request.post(useUrlSiteId("productType/deletes"), { ids }, undefined, {
    successMessage: $t("common.deleteSuccess"),
  });
