import type { CustomField } from "@/api/commerce/settings";
import { getValueIgnoreCase } from "@/utils/string";
import { i18n } from "@/modules/i18n";

const t = i18n.global.t;

export const productLabels: Record<string, string> = {
  featuredImage: t("common.image"),
  title: t("commerce.productName"),
  variantsCount: t("commerce.variants"),
  inventory: t("common.inventory"),
  tags: t("common.tags"),
  active: t("commerce.active"),
  seoName: t("common.seoName"),
  description: t("common.description"),
  categories: t("common.productCategories"),
  images: t("common.images"),
  id: "ID",
};

export const categoryLabels: Record<string, string> = {
  image: t("common.image"),
  title: t("common.categoryName"),
  condition: t("common.condition"),
  productCount: t("common.productCount"),
  tags: t("common.tags"),
  active: t("commerce.active"),
  seoName: t("common.seoName"),
  description: t("common.description"),
  parentId: t("common.parent"),
};

export function useProductLabels() {
  return useLabels(productLabels);
}

export function useCategoryLabels() {
  return useLabels(categoryLabels);
}

export function useLabels(labels: Record<string, string>) {
  function getFieldDisplayName(
    field: Pick<CustomField, "name" | "displayName">
  ) {
    return getDisplayName(field.name, field.displayName);
  }

  function getDisplayName(name: string, displayName?: string) {
    return displayName || getValueIgnoreCase(labels, name) || name;
  }

  return {
    labels,
    getFieldDisplayName,
    getDisplayName,
  };
}
