import { useUrlSiteId } from "@/hooks/use-site-id";
import request from "@/utils/request";
import { i18n } from "@/modules/i18n";
import type { PagingResult, PagingParams, CustomerInfo } from "./common";
import type { Option } from "./product";
const $t = i18n.global.t;

export const getCarts = (params: PagingParams) =>
  request.post<CartPagingResult>(useUrlSiteId("Cart/list"), params);

export const calculate = (params: CartCalculateParams) =>
  request.post<CartCalculateResult>(useUrlSiteId("Cart/Calculate"), params);

export const createCart = (params: CartCreate) =>
  request.post(useUrlSiteId("Cart/Create"), params);

export const editCart = (params: CartEdit) =>
  request.post(useUrlSiteId("Cart/Edit"), params);

export const getCartEdit = (id: string) =>
  request.get<CartEdit>(useUrlSiteId("Cart/GetEdit"), { id });

export const deleteCarts = (ids: string[]) =>
  request.post(useUrlSiteId("Cart/deletes"), { ids }, undefined, {
    successMessage: $t("common.deleteSuccess"),
  });

export interface CartListItemLine {
  variantId: string;
  title: string;
  options: Option[];
  quantity: number;
  image: string;
}

export interface CartListItem {
  id: string;
  customer: CustomerInfo;
  contact: string;
  country: string;
  updateAt: string;
  lines: CartListItemLine[];
}

export interface CartCreate extends CartCalculateParams {
  note: string;
  digitalShippingId?: string;
}

export interface CartEdit extends CartCreate {
  id: string;
  extensionButton?: any;
  digitalShippingId?: string;
}

export interface CartLine {
  quantity: number;
  variantId: string;
  groupName?: string;
  isMain?: boolean;
}

export interface DiscountAllocation {
  amount: number;
  code: string;
  title: string;
}

export interface ShippingAllocation {
  cost: number;
  isAdditional: boolean;
  title: string;
}

export interface CartCalculateParams {
  customerId: string;
  shippingId?: string;
  discountCodes: string[];
  lines: CartLine[];
  redeemPoints: boolean;
}

export interface CartCalculateResult {
  lines: {
    variantId: string;
    title: string;
    options: Option[];
    sku: string;
    image: string;
    price: string;
    originalPrice: number;
    quantity: number;
    totalQuantity: number;
    amount: number;
    originalAmount: number;
    groupName: string;
    isMain: boolean;
    note: string;
    discountAllocations: DiscountAllocation[];
    isDigital: boolean;
  }[];
  discountAllocations: DiscountAllocation[];
  shippingAllocations: ShippingAllocation[];
  insuranceAmount: number;
  shippingAmount: number;
  subtotalAmount: number;
  totalAmount: number;
  originalAmount: number;
  originalSubtotalAmount: number;
  totalQuantity: number;
  redeemPoints: number;
  canRedeemPoints: number;
  pointsDeductionAmount: number;
  canPointsDeductionAmount: number;
  earnPoints: number;
}

export type CartPagingResult = PagingResult<CartListItem>;
