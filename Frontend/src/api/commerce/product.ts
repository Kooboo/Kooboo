import type { KeyValue } from "@/global/types";
import type { PagingResult } from "./common";
import { i18n } from "@/modules/i18n";
import request from "@/utils/request";
import { useUrlSiteId } from "@/hooks/use-site-id";

const $t = i18n.global.t;

export const getProducts = (body: unknown) =>
  request.post<PagingResult<ProductListItem>>(
    useUrlSiteId("productManagement/list"),
    body
  );

export type ProductPagingResult = PagingResult<ProductListItem> & {
  stats: any;
};
export const getPagingProducts = (params: any) =>
  request.post<ProductPagingResult>(
    useUrlSiteId("productManagement/pagingList"),
    params
  );

export const getProductEdit = (id: string) =>
  request.get<ProductEdit>(useUrlSiteId("productManagement/getEdit"), {
    id,
  });
export const getVariants = (id: string) =>
  request.get<ProductVariant[]>(useUrlSiteId("productManagement/Variants"), {
    id,
  });

export const getProductVariantItems = (ids: string[]) =>
  request.post<ProductVariantItem[]>(
    useUrlSiteId("productManagement/ProductVariantItems"),
    {
      ids,
    }
  );

export const createProduct = (body: unknown, hideMessage?: boolean) =>
  request.post<string>(
    useUrlSiteId("productManagement/create"),
    body,
    undefined,
    {
      successMessage: hideMessage ? undefined : $t("common.saveSuccess"),
    }
  );

export const editProduct = (body: unknown, hideMessage?: boolean) =>
  request.post(useUrlSiteId("productManagement/edit"), body, undefined, {
    successMessage: hideMessage ? undefined : $t("common.saveSuccess"),
  });

export const deleteProducts = (ids: string[]) =>
  request.post(useUrlSiteId("productManagement/deletes"), { ids }, undefined, {
    successMessage: $t("common.deleteSuccess"),
  });

export const uploadDigitalFile = (body: unknown) =>
  request.post(
    useUrlSiteId("productManagement/UploadDigitalFile"),
    body,
    undefined,
    {
      headers: { "Content-Type": "multipart/form-data" },
      successMessage: $t("common.uploadSuccess"),
    }
  );

export const deleteDigitalFile = (variantId: string, fileName: string) =>
  request.delete(
    useUrlSiteId("productManagement/DeleteDigitalFile"),
    { variantId, fileName },
    {
      successMessage: $t("common.deleteSuccess"),
    }
  );

export interface ProductListItem {
  id: string;
  title: string;
  active: boolean;
  featuredImage: string;
  inventory: number;
  variantsCount: number;
  [key: string]: any;
}

export interface ProductEdit extends ProductBasicInfo {
  id: string;
  attributes: KeyValue[];
  variants: ProductVariant[];
}

export interface ProductVariant {
  id: string;
  price: number;
  sku: string;
  image: string;
  barcode: string;
  active: boolean;
  originalInventory: number;
  newInventory: number;
  weight: number;
  selectedOptions: Option[];
  tags: string[];
  digitalItems: DigitalItem[];
  autoDelivery: boolean;
  order: number;
}

export interface DigitalItem {
  id: string;
  type: string;
  name: string;
  value: string;
  contentType?: string;
  size?: number;
}

export interface ProductVariantItem {
  variantId: string;
  price: number;
  sku: string;
  image: string;
  title: string;
  selectedOptions: Option[];
}

export interface Option {
  name: string;
  value: string;
}

export interface VariantOption extends VariantOptionItem {
  items: VariantOptionItem[];
}

export interface VariantOptionItem {
  name: string;
  type: string;
  multilingual: Record<string, any>;
  image?: string;
}

export interface OptionGroup {
  name: string;
  options: string[];
}

export interface ProductBasicInfo {
  categories: string[];
  title: string;
  seoName: string;
  active: boolean;
  isDigital: boolean;
  maxDownloadCount?: number;
  featuredImage: string;
  description: string;
  images: string[];
  tags: string[];
  customData: Record<string, any>;
  variantOptions: VariantOption[];
}

export interface ProductCreate extends ProductBasicInfo {
  attributes: KeyValue[];
  variants: ProductVariant[];
}
