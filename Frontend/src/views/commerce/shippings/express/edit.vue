<script lang="ts" setup>
import { useRouter } from "vue-router";
import Breadcrumb from "@/components/basic/breadcrumb.vue";
import { useI18n } from "vue-i18n";
import { ref } from "vue";
import { editShipping, getShippingEdit } from "@/api/commerce/shipping";
import { useRouteSiteId } from "@/hooks/use-site-id";
import type { ShippingEdit } from "@/api/commerce/shipping";
import { getQueryString } from "@/utils/url";
import EditForm from "./edit-form.vue";

const { t } = useI18n();
const router = useRouter();
const id = getQueryString("id");
const model = ref<ShippingEdit>();

getShippingEdit(id!).then((rsp) => {
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
  await editShipping(model.value);
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
      { name: t('common.edit') },
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
