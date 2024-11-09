<script lang="ts" setup>
import { useRouter } from "vue-router";
import Breadcrumb from "@/components/basic/breadcrumb.vue";
import { useI18n } from "vue-i18n";
import { ref } from "vue";
import { editDiscount, getDiscountEdit } from "@/api/commerce/discount";
import { useRouteSiteId } from "@/hooks/use-site-id";
import type { DiscountEdit } from "@/api/commerce/discount";
import { getQueryString } from "@/utils/url";
import EditForm from "./edit-form.vue";

const { t } = useI18n();
const router = useRouter();
const id = getQueryString("id");
const model = ref<DiscountEdit>();

getDiscountEdit(id!).then((rsp) => {
  model.value = rsp;
});

function goBack() {
  router.goBackOrTo(
    useRouteSiteId({
      name: "discounts",
    })
  );
}

async function save() {
  await editDiscount(model.value);
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
