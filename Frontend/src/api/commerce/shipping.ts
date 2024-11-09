import type { Condition, ConditionSchema } from "./common";
import request from "@/utils/request";
import { useUrlSiteId } from "@/hooks/use-site-id";
import { i18n } from "@/modules/i18n";
import type { KeyValue } from "@/global/types";
const $t = i18n.global.t;

export interface Shipping {
  name: string;
  description: string;
  baseCost: number;
  additionalCosts: AdditionalCost[];
  countries: SupportCountry[];
  isDefault: boolean;
  trackingNumberMatching?: string;
  code: string;
}

export interface SupportCountry {
  name: string;
  display: string;
  estimatedDaysOfArrival: number;
}

export interface AdditionalCost {
  cost: number;
  description: string;
  condition: Condition;
}

export type ShippingCreate = Shipping;

export interface ShippingEdit extends Shipping {
  id: string;
}

export interface ShippingListItem extends Shipping {
  id: string;
}

export const getSchemas = () =>
  request.get<ConditionSchema[]>(useUrlSiteId("shipping/Schemas"), undefined, {
    hiddenLoading: true,
  });

export const createShipping = (body: unknown) =>
  request.post(useUrlSiteId("shipping/create"), body, undefined, {
    successMessage: $t("common.saveSuccess"),
  });

export const editShipping = (body: unknown) =>
  request.post(useUrlSiteId("shipping/Edit"), body, undefined, {
    successMessage: $t("common.saveSuccess"),
  });

export const getShippingEdit = (id: string) =>
  request.get<ShippingEdit>(useUrlSiteId("shipping/getEdit"), {
    id,
  });

export const getShippings = () =>
  request.get<ShippingListItem[]>(useUrlSiteId("shipping/list"));

export const getShipping = (id: string) =>
  request.get<ShippingListItem>(useUrlSiteId("shipping/get"), { id });

export const deleteShippings = (ids: string[]) =>
  request.post(useUrlSiteId("shipping/deletes"), { ids }, undefined, {
    successMessage: $t("common.deleteSuccess"),
  });

export const getCountries = () =>
  request.get<KeyValue[]>(useUrlSiteId("shipping/Countries"));

export const setDefault = (id: string) =>
  request.post<KeyValue[]>(useUrlSiteId("shipping/SetDefault"), {
    id,
  });
