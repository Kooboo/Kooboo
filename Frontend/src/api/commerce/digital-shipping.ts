import request from "@/utils/request";
import { useUrlSiteId } from "@/hooks/use-site-id";
import { i18n } from "@/modules/i18n";
import type { KeyValue } from "@/global/types";
import type { SmtpSetting } from "./settings";
const $t = i18n.global.t;

export interface DigitalShipping {
  name: string;
  description: string;
  isDefault?: boolean;
  mailServerType: string;
  koobooEmailAddress: string;
  customMailServer: SmtpSetting;
  mailTemplate: ShippingMail;
}

export interface ShippingMail {
  subject: string;
  body: string;
}

export type ShippingCreate = DigitalShipping;

export interface ShippingEdit extends DigitalShipping {
  id: string;
}

export interface ShippingListItem extends DigitalShipping {
  id: string;
}
export const createShipping = (body: unknown) =>
  request.post(useUrlSiteId("digitalshipping/create"), body, undefined, {
    successMessage: $t("common.saveSuccess"),
  });

export const editShipping = (body: unknown) =>
  request.post(useUrlSiteId("digitalshipping/Edit"), body, undefined, {
    successMessage: $t("common.saveSuccess"),
  });

export const getShippingEdit = (id: string) =>
  request.get<ShippingEdit>(useUrlSiteId("digitalshipping/getEdit"), {
    id,
  });

export const getShippings = () =>
  request.get<ShippingListItem[]>(useUrlSiteId("digitalshipping/list"));

export const getShipping = (id: string) =>
  request.get<ShippingListItem>(useUrlSiteId("digitalshipping/get"), { id });

export const deleteShippings = (ids: string[]) =>
  request.post(useUrlSiteId("digitalshipping/deletes"), { ids }, undefined, {
    successMessage: $t("common.deleteSuccess"),
  });
export const setDefault = (id: string) =>
  request.post<KeyValue[]>(useUrlSiteId("digitalshipping/SetDefault"), {
    id,
  });

export const preview = (mail: ShippingMail) =>
  request.post<ShippingMail>(useUrlSiteId("digitalshipping/preview"), mail);
