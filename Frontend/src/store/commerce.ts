import { defineStore } from "pinia";
import { ref, watch } from "vue";
import type { CategoryListItem } from "@/api/commerce/category";
import { getCategories } from "@/api/commerce/category";
import { getProductTypes, type ProductType } from "@/api/commerce/type";
import { getSettings } from "@/api/commerce/settings";
import type { Settings } from "@/api/commerce/settings";
import { useSiteStore } from "./site";
import type { MembershipListItem } from "@/api/commerce/loyalty";
import { getMemberships } from "@/api/commerce/loyalty";
import type { Country } from "@/api/commerce/address";
import { getCountries } from "@/api/commerce/address";

export const useCommerceStore = defineStore("commerceStore", () => {
  const categories = ref<CategoryListItem[]>([]);
  const types = ref<ProductType[]>([]);
  const memberships = ref<MembershipListItem[]>();
  const countries = ref<Country[]>([]);

  getCountries().then((rsp) => (countries.value = rsp));

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
    enableWebhook: false,
    earnPoint: {
      activeDuration: 1,
      activeDurationUnit: "Year",
      orderEarnRules: [],
      loginEarnRules: [],
    },
    redeemPoint: {
      exchangeRate: 10,
      orderRedeemRules: [],
    },
    productDigitalItemRequired: false,
    hideAttributes: false,
    hideVariants: false,
    orderExtensionFields: [],
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

  const loadMemberships = async (force?: boolean) => {
    if (!memberships.value || force) {
      memberships.value = await getMemberships();
    }
  };

  watch(
    () => siteStore.site?.id,
    () => {
      loadSettings();
    }
  );

  return {
    loadCategories,
    loadMemberships,
    categories,
    memberships,
    loadTypes,
    types,
    loadSettings,
    settings,
    countries,
  };
});
