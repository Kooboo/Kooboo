import type { Condition, ConditionSchema } from "./common";
import request from "@/utils/request";
import { useUrlSiteId } from "@/hooks/use-site-id";
import { i18n } from "@/modules/i18n";
const $t = i18n.global.t;

export type OverridesType = "Added" | "Instead" | "Compounded";

export interface RegionOverrides {
  region: string;
  type: OverridesType;
  tax: number;
}

export interface ProductOverrides {
  condition: Condition;
  type: OverridesType;
  tax: number;
}

export interface Tax {
  country: string;
  baseTax: number;
  regionOverrides: RegionOverrides[];
  productOverrides: ProductOverrides[];
}

export type TaxCreate = Tax;

export interface TaxEdit extends Tax {
  id: string;
}

export interface TaxItem extends Tax {
  id: string;
}

export const getTaxSchemas = () =>
  request.get<ConditionSchema[]>(useUrlSiteId("Tax/Schemas"), undefined, {
    hiddenLoading: true,
  });

export const createTax = (body: unknown) =>
  request.post(useUrlSiteId("Tax/create"), body, undefined, {
    successMessage: $t("common.saveSuccess"),
  });

export const editTax = (body: unknown) =>
  request.post(useUrlSiteId("Tax/Edit"), body, undefined, {
    successMessage: $t("common.saveSuccess"),
  });

export const getTaxEdit = (id: string) =>
  request.get<TaxEdit>(useUrlSiteId("Tax/getEdit"), {
    id,
  });

export const getTaxes = () => request.get<TaxItem[]>(useUrlSiteId("Tax/list"));

export const deleteTaxes = (ids: string[]) =>
  request.post(useUrlSiteId("Tax/deletes"), { ids }, undefined, {
    successMessage: $t("common.deleteSuccess"),
  });
