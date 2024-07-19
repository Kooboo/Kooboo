import type { Condition, ConditionSchema } from "./common";

import type { ProductListItem } from "./product";
import { i18n } from "@/modules/i18n";
import request from "@/utils/request";
import { useUrlSiteId } from "@/hooks/use-site-id";

const $t = i18n.global.t;

export const getCategories = () =>
  request.get<CategoryListItem[]>(useUrlSiteId("ProductCategory/list"));

export const getConditionSchemas = () =>
  request.get<ConditionSchema[]>(
    useUrlSiteId("ProductCategory/ConditionSchemas")
  );

export const getCategoryEdit = (id: string) =>
  request.get<CategoryEdit>(useUrlSiteId("ProductCategory/getEdit"), {
    id,
  });

export const createCategory = (body: unknown, hideMessage?: boolean) =>
  request.post<string>(
    useUrlSiteId("ProductCategory/create"),
    body,
    undefined,
    {
      successMessage: hideMessage ? undefined : $t("common.saveSuccess"),
    }
  );

export const editCategory = (body: unknown, hideMessage?: boolean) =>
  request.post(useUrlSiteId("ProductCategory/edit"), body, undefined, {
    successMessage: hideMessage ? undefined : $t("common.saveSuccess"),
  });

export const deleteCategories = (ids: string[]) =>
  request.post(useUrlSiteId("ProductCategory/deletes"), { ids }, undefined, {
    successMessage: $t("common.deleteSuccess"),
  });

export const getProducts = (categoryId: string) =>
  request.get<ProductListItem[]>(useUrlSiteId("ProductCategory/Products"), {
    categoryId,
  });

export const editProducts = (categoryId: string, products: string[]) =>
  request.post(
    useUrlSiteId("ProductCategory/EditProducts"),
    { categoryId, products },
    undefined,
    {
      successMessage: $t("common.updateSuccess"),
    }
  );

export const moveCategory = (changes: MoveCategory) =>
  request.put(useUrlSiteId("ProductCategory/Move"), { changes });
export interface CategoryListItem {
  id: string;
  title: string;
  active: boolean;
  image: string;
  type: string;
  productCount: number;
  condition: Condition;
  [key: string]: any;
}

export interface CategoryBasicInfo {
  title: string;
  active: boolean;
  image: string;
  description: string;
  seoName: string;
  parentId: string;
  tags: string[];
  customData: Record<string, any>;
}

export interface CategoryCreate extends CategoryBasicInfo {
  type: string;
  condition: Condition;
}

export interface CategoryEdit extends CategoryBasicInfo {
  id: string;
  type: string;
  condition: Condition;
  products: string[];
}

export interface MoveCategory {
  source: string;
  prevId?: string;
  nextId?: string;
}
