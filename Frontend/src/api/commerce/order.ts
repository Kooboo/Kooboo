import { useUrlSiteId } from "@/hooks/use-site-id";
import request from "@/utils/request";
import { i18n } from "@/modules/i18n";
import type { CustomerInfo, PagingParams, PagingResult } from "./common";
import type { DigitalItem, Option } from "./product";
import type { Address } from "./customer";
import { timeZoneOffset } from "@/utils/date";

const $t = i18n.global.t;
export const createOrder = (body: unknown) =>
  request.post(useUrlSiteId("Order/Create"), body);

export const getOrders = (params: PagingParams) =>
  request.post<OrderPagingResult>(useUrlSiteId("Order/list"), params);

export const cancelOrder = (body: unknown) =>
  request.post(useUrlSiteId("Order/Cancel"), body);

export const payOrder = (body: unknown) =>
  request.post(useUrlSiteId("Order/Pay"), body);

export const deliveryOrder = (body: unknown) =>
  request.post(useUrlSiteId("Order/Delivery"), body);

export const getOrderDetail = (id: string) =>
  request.get<OrderDetail>(useUrlSiteId("Order/GetDetail"), { id });

export const deleteOrders = (ids: string[]) =>
  request.post(useUrlSiteId("Order/deletes"), { ids }, undefined, {
    successMessage: $t("common.deleteSuccess"),
  });

export const generateOrderExcel = (query: any) => {
  return request.post<{ file: string; name: string }>(
    useUrlSiteId(`/Order/generateOrderExcel?timezone=${timeZoneOffset()}`),
    query,
    undefined,
    {
      timeout: 1000 * 60 * 60,
    }
  );
};
export interface OrderLine {
  id: string;
  variantId: string;
  title: string;
  options: Option[];
  quantity: number;
  totalQuantity: number;
  image: string;
  price: number;
  totalAmount: number;
  groupName?: string;
  isMain?: boolean;
  delivered: boolean;
  isDigital: boolean;
  digitalItems: DigitalItem[];
  shippingCarrier?: string;
  trackingNumber?: string;
  errorMessage?: string;
}

export interface OrderListItem {
  id: string;
  customer: CustomerInfo;
  totalAmount: number;
  paid: boolean;
  paymentMethod: string;
  delivered: boolean;
  partialDelivered: boolean;
  trackingNumber: string;
  shippingCarrier: string;
  canceled: boolean;
  cancelReason: string;
  createAt: string;
  updateAt: string;
  lines: OrderLine[];
}

export interface OrderDetail {
  id: string;
  customer: CustomerInfo;
  totalAmount: number;
  originalAmount: number;
  insuranceAmount: number;
  subtotalAmount: number;
  originalSubtotalAmount: number;
  shippingAmount: number;
  scheduledDeliveryTime?: string;
  paid: boolean;
  paymentMethod: string;
  delivered: boolean;
  partialDelivered: boolean;
  trackingNumber: string;
  shippingCarrier: string;
  canceled: boolean;
  cancelReason: string;
  currency: string;
  createdAt: string;
  updatedAt: string;
  lines: OrderLine[];
  shippingAddress: Address;
  note: string;
  ip: string;
  country: string;
  source: string;
  clientInfo: any;
  discountAllocations: any;
  shippingAllocations: any;
  extensionButton: any;
  pointsDeductionAmount: number;
  redeemPoints: number;
  earnPoints: number;
}

export type OrderPagingResult = PagingResult<OrderListItem> & { stats: any };
