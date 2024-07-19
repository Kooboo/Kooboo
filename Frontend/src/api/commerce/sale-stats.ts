import { useUrlSiteId } from "@/hooks/use-site-id";
import request from "@/utils/request";
import { i18n } from "@/modules/i18n";
import type { PaginationResponse } from "@/global/types";
const $t = i18n.global.t;

export const getProductSaleStats = (query: any) =>
  request.post<PaginationResponse<any>>(
    useUrlSiteId("SaleStats/ProductSaleStats"),
    query
  );

export const generateProductSaleStats = (query: any) => {
  return request.post<{ file: string; name: string }>(
    useUrlSiteId("/SaleStats/GenerateProductSaleStats"),
    query,
    undefined,
    {
      timeout: 1000 * 60 * 60,
    }
  );
};

export const getDailySaleStats = (query: any) =>
  request.post<PaginationResponse<any>>(
    useUrlSiteId("SaleStats/DailySaleStats"),
    query
  );

export const generateDailySaleStats = (query: any) => {
  return request.post<{ file: string; name: string }>(
    useUrlSiteId("/SaleStats/GenerateDailySaleStats"),
    query,
    undefined,
    {
      timeout: 1000 * 60 * 60,
    }
  );
};

export const getOrderSaleStats = (query: any) =>
  request.post<PaginationResponse<any>>(
    useUrlSiteId("SaleStats/OrderSaleStats"),
    query
  );

export const generateOrderSaleStats = (query: any) => {
  return request.post<{ file: string; name: string }>(
    useUrlSiteId("/SaleStats/GenerateOrderSaleStats"),
    query,
    undefined,
    {
      timeout: 1000 * 60 * 60,
    }
  );
};
