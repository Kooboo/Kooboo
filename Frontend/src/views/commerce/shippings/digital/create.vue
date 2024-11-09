<script lang="ts" setup>
import { useRouter } from "vue-router";
import Breadcrumb from "@/components/basic/breadcrumb.vue";
import { useI18n } from "vue-i18n";
import { ref } from "vue";
import {
  createShipping,
  getShippingEdit,
} from "@/api/commerce/digital-shipping";
import { useRouteSiteId } from "@/hooks/use-site-id";
import type { ShippingEdit } from "@/api/commerce/digital-shipping";
import EditForm from "./edit-form.vue";
import { emptyGuid } from "@/utils/guid";

const { t } = useI18n();
const router = useRouter();
const model = ref<ShippingEdit>();

getShippingEdit(emptyGuid).then((rsp) => {
  model.value = rsp;
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
        route: {
          name: 'shippings',
          query: {
            name: 'digital',
          },
        },
      },
      { name: t('common.create') },
    ]"
  />
  <EditForm v-if="model" :model="model" />
  <KBottomBar
    :permission="{
      feature: 'shipping',
      action: 'edit',
    }"
    @cancel="goBack"
    @save="save"
  />
</template>
