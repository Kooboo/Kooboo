import { defineStore } from "pinia";
import { ref, watch } from "vue";
import type { CategoryListItem } from "@/api/commerce/category";
import { getCategories } from "@/api/commerce/category";
import { getProductTypes, type ProductType } from "@/api/commerce/type";
import { getSettings } from "@/api/commerce/settings";
import type { Settings } from "@/api/commerce/settings";
import { useSiteStore } from "./site";

export const useCommerceStore = defineStore("commerceStore", () => {
  const categories = ref<CategoryListItem[]>([]);
  const types = ref<ProductType[]>([]);
  const siteStore = useSiteStore();
  const settings = ref<Settings>({
    currencyCode: "",
    currencySymbol: "",
    payments: [],
    shippingCost: 0,
    weightUnit: "",
    productCustomFields: [],
    categoryCustomFields: [],
    koobooEmailAddress: "",
    emailNotifications: [],
    enableEmailNotification: false,
    webhooks: [],
    mailServerType: "kooboo",
  });

  const loadSettings = async () => {
    settings.value = await getSettings();
  };

  loadSettings();

  const loadCategories = async () => {
    categories.value = await getCategories();
  };

  const loadTypes = async () => {
    types.value = await getProductTypes();
  };

  watch(
    () => siteStore.site?.id,
    () => {
      loadSettings();
    }
  );

  return {
    loadCategories,
    categories,
    loadTypes,
    types,
    loadSettings,
    settings,
  };
});
