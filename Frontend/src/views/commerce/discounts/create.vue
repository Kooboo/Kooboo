<script lang="ts" setup>
import { useRouter } from "vue-router";
import Breadcrumb from "@/components/basic/breadcrumb.vue";
import { useI18n } from "vue-i18n";
import { ref } from "vue";
import { createDiscount } from "@/api/commerce/discount";
import { useRouteSiteId } from "@/hooks/use-site-id";
import type { DiscountCreate } from "@/api/commerce/discount";
import EditForm from "./edit-form.vue";

const { t } = useI18n();
const router = useRouter();

const model = ref<DiscountCreate>({
  title: "",
  code: "",
  condition: {
    isAny: false,
    items: [],
  },
  endDate: "",
  startDate: "",
  isPercent: false,
  method: "AutomaticDiscount",
  type: "ProductAmountOff",
  value: 10,
  priority: 0,
  isExclusion: false,
});

function goBack() {
  router.goBackOrTo(
    useRouteSiteId({
      name: "discounts",
    })
  );
}

async function save() {
  await createDiscount(model.value);
  goBack();
}
</script>

<template>
  <Breadcrumb
    class="p-24"
    :crumb-path="[
      {
        name: t('common.discounts'),
        route: { name: 'discounts' },
      },
      { name: t('common.create') },
    ]"
  />
  <EditForm :model="model" />
  <KBottomBar
    :permission="{
      feature: 'discounts',
      action: 'edit',
    }"
    @cancel="goBack"
    @save="save"
  />
</template>
