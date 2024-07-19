import request from "@/utils/request";
import type { DomainOrder } from "./types";

//MarketOrders
export const createDomainOrder = (model: any) =>
  request.post("MarketOrder/NewDomain", model);

export const getDomainOrder = () =>
  request.get<DomainOrder[]>("MarketOrder/RecentList");

export const getWeChatPay = (id: string, title: string, total: number) =>
  request.post<{ qrcode: string; requestId: string }>("MarketOrder/WeChatPay", {
    id,
    title,
    total,
  });
