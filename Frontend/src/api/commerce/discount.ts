import type { Condition, ConditionSchema } from "./common";
import request from "@/utils/request";
import { useUrlSiteId } from "@/hooks/use-site-id";
import { i18n } from "@/modules/i18n";
const $t = i18n.global.t;

export interface Discount {
  title: string;
  startDate: string;
  endDate: string;
  code: string;
  type: string;
  method: string;
  value: number;
  isPercent: boolean;
  condition: Condition;
  priority: number;
  isExclusion: boolean;
}

export type DiscountCreate = Discount;

export interface DiscountEdit extends Discount {
  id: string;
}

export interface DiscountListItem extends Discount {
  id: string;
}

export const getOrderLineSchemas = () =>
  request.get<ConditionSchema[]>(
    useUrlSiteId("Discount/OrderLineSchemas"),
    undefined,
    {
      hiddenLoading: true,
    }
  );

export const getOrderSchemas = () =>
  request.get<ConditionSchema[]>(
    useUrlSiteId("Discount/OrderSchemas"),
    undefined,
    {
      hiddenLoading: true,
    }
  );

export const createDiscount = (body: unknown) =>
  request.post(useUrlSiteId("Discount/create"), body, undefined, {
    successMessage: $t("common.saveSuccess"),
  });

export const editDiscount = (body: unknown) =>
  request.post(useUrlSiteId("Discount/Edit"), body, undefined, {
    successMessage: $t("common.saveSuccess"),
  });

export const getDiscountEdit = (id: string) =>
  request.get<DiscountEdit>(useUrlSiteId("Discount/getEdit"), {
    id,
  });

export const getDiscounts = () =>
  request.get<DiscountListItem[]>(useUrlSiteId("Discount/list"));

export const getDiscountCodes = () =>
  request.get<string[]>(useUrlSiteId("Discount/Codes"));

export const deleteDiscounts = (ids: string[]) =>
  request.post(useUrlSiteId("Discount/deletes"), { ids }, undefined, {
    successMessage: $t("common.deleteSuccess"),
  });
