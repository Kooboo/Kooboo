<script lang="ts" setup>
import { useRouter } from "vue-router";
import Breadcrumb from "@/components/basic/breadcrumb.vue";
import { useI18n } from "vue-i18n";
import { ref } from "vue";
import { createShipping } from "@/api/commerce/shipping";
import { useRouteSiteId } from "@/hooks/use-site-id";
import type { ShippingCreate } from "@/api/commerce/shipping";
import EditForm from "./edit-form.vue";

const { t } = useI18n();
const router = useRouter();

const model = ref<ShippingCreate>({
  name: "",
  description: "",
  baseCost: 10,
  additionalCosts: [],
  estimatedDaysOfArrival: 3,
  countries: [],
  isDefault: false,
});

function goBack() {
  router.goBackOrTo(
    useRouteSiteId({
      name: "shippings",
    })
  );
}

async function save() {
  await createShipping(model.value);
  goBack();
}
</script>

<template>
  <Breadcrumb
    class="p-24"
    :crumb-path="[
      {
        name: t('common.shippings'),
        route: { name: 'shippings' },
      },
      { name: t('common.create') },
    ]"
  />
  <EditForm :model="model" />
  <KBottomBar
    :permission="{
      feature: 'shipping',
      action: 'edit',
    }"
    @cancel="goBack"
    @save="save"
  />
</template>
