import { useUrlSiteId } from "@/hooks/use-site-id";
import request from "@/utils/request";
import type { PagingParams, PagingResult } from "./common";
import { i18n } from "@/modules/i18n";

const $t = i18n.global.t;
export const getCustomers = (params: PagingParams) =>
  request.post<CustomerPagingResult>(useUrlSiteId("Customer/list"), params);

export const getCustomerEdit = (id: string) =>
  request.get<CustomerEdit>(useUrlSiteId("Customer/GetEdit"), { id });

export const getCustomer = (id: string) =>
  request.get<CustomerListItem>(useUrlSiteId("Customer/Get"), { id });

export const createCustomer = (body: unknown) =>
  request.post(useUrlSiteId("Customer/create"), body, undefined, {
    successMessage: $t("common.saveSuccess"),
  });

export const editCustomer = (body: unknown) =>
  request.post(useUrlSiteId("Customer/Edit"), body, undefined, {
    successMessage: $t("common.saveSuccess"),
  });

export const deleteCustomers = (ids: string[]) =>
  request.post(useUrlSiteId("Customer/deletes"), { ids }, undefined, {
    successMessage: $t("common.deleteSuccess"),
  });

export interface CustomerListItem extends CustomerCreate {
  id: string;
  membershipId: string;
  membership: string;
  membershipStartedAt?: string;
  membershipEndAt?: string;
  rewardPoints: number;
}

export interface CustomerCreate {
  email: string;
  phone: string;
  firstName: string;
  lastName: string;
  tags: string[];
  addresses: Address[];
}

export interface Address {
  isDefault: boolean;
  address1: string;
  address2: string;
  city: string;
  country: string;
  firstName: string;
  lastName: string;
  phone: string;
  province: string;
  zip: string;
}

export interface CustomerEdit extends CustomerCreate {
  id: string;
}

export type CustomerPagingResult = PagingResult<CustomerListItem>;
