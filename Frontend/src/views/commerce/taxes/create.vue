<script lang="ts" setup>
import { useRouter } from "vue-router";
import Breadcrumb from "@/components/basic/breadcrumb.vue";
import { useI18n } from "vue-i18n";
import { computed, ref } from "vue";
import { createTax } from "@/api/commerce/tax";
import { useRouteSiteId } from "@/hooks/use-site-id";
import type { TaxCreate } from "@/api/commerce/tax";
import EditForm from "./edit-form.vue";
import { getQueryString } from "@/utils/url";
import { useCommerceStore } from "@/store/commerce";
import { systemDisplay } from "@/utils/commerce";

const commerceStore = useCommerceStore();
const { t } = useI18n();
const router = useRouter();

const model = ref<TaxCreate>({
  country: getQueryString("country")!,
  baseTax: 2,
  regionOverrides: [],
  productOverrides: [],
});

function goBack() {
  router.goBackOrTo(
    useRouteSiteId({
      name: "taxes",
    })
  );
}

const countryName = computed(() => {
  const country = commerceStore?.countries.find(
    (f) => f.name == model.value.country
  );
  if (!country) return model.value.country;
  return systemDisplay(country.nameTranslations, country.name);
});

async function save() {
  await createTax(model.value);
  goBack();
}
</script>

<template>
  <Breadcrumb
    class="p-24"
    :crumb-path="[
      {
        name: t('common.taxes'),
        route: { name: 'taxes' },
      },
      { name: countryName },
    ]"
  />
  <EditForm :model="model" />
  <KBottomBar
    :permission="{
      feature: 'taxes',
      action: 'edit',
    }"
    @cancel="goBack"
    @save="save"
  />
</template>
