<script lang="ts" setup>
import { useRouter } from "vue-router";
import Breadcrumb from "@/components/basic/breadcrumb.vue";
import { useI18n } from "vue-i18n";
import { computed, ref } from "vue";
import { editTax, getTaxEdit } from "@/api/commerce/tax";
import { useRouteSiteId } from "@/hooks/use-site-id";
import type { TaxEdit } from "@/api/commerce/tax";
import { getQueryString } from "@/utils/url";
import EditForm from "./edit-form.vue";
import { useCommerceStore } from "@/store/commerce";
import { systemDisplay } from "@/utils/commerce";

const { t } = useI18n();
const router = useRouter();
const id = getQueryString("id");
const model = ref<TaxEdit>();
const commerceStore = useCommerceStore();

getTaxEdit(id!).then((rsp) => {
  model.value = rsp;
});

function goBack() {
  router.goBackOrTo(
    useRouteSiteId({
      name: "discounts",
    })
  );
}

const countryName = computed(() => {
  if (!model.value) return "";
  const country = commerceStore?.countries.find(
    (f) => f.name == model.value!.country
  );
  if (!country) return model.value.country;
  return systemDisplay(country.nameTranslations, country.name);
});

async function save() {
  await editTax(model.value);
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
  <EditForm v-if="model" :model="model" />
  <KBottomBar
    :permission="{
      feature: 'discounts',
      action: 'edit',
    }"
    @cancel="goBack"
    @save="save"
  />
</template>
