<script lang="ts" setup>
import { useRouter } from "vue-router";
import Breadcrumb from "@/components/basic/breadcrumb.vue";
import { useI18n } from "vue-i18n";
import { ref } from "vue";
import { useRouteSiteId } from "@/hooks/use-site-id";
import type { CartEdit } from "@/api/commerce/cart";
import { editCart, getCartEdit } from "@/api/commerce/cart";
import { getQueryString } from "@/utils/url";
import ProductsEditor from "./products-editor.vue";
import ContactCard from "../components/contact-card.vue";
import ShippingCard from "../components/shipping-card.vue";
import DigitalShippingCard from "../components/digital-shipping-card.vue";

const id = getQueryString("id");
const { t } = useI18n();
const router = useRouter();
const model = ref<CartEdit>();
const hasPhysicsProducts = ref(false);
const hasDigitalProducts = ref(false);

getCartEdit(id!).then(async (rsp) => {
  model.value = rsp;
  if (!model.value.discountCodes) model.value.discountCodes = [];
});

function goBack() {
  router.goBackOrTo(
    useRouteSiteId({
      name: "carts",
    })
  );
}

async function save() {
  await editCart(model.value!);
  goBack();
}
</script>

<template>
  <Breadcrumb
    class="p-24"
    :crumb-path="[
      {
        name: t('common.carts'),
        route: { name: 'carts' },
      },
      { name: t('common.edit') },
    ]"
  />
  <div class="px-24 pt-0 pb-84px space-y-12">
    <div class="bg-fff dark:bg-[#252526] px-24 py-16 rounded-normal">
      <ProductsEditor
        v-if="model"
        v-model:lines="model.lines"
        v-model:redeem-points="model.redeemPoints"
        :customer-id="model.customerId"
        :shipping-id="model.shippingId"
        :discount-codes="model.discountCodes"
        :extension-button="model.extensionButton"
        @update:has-digital-products="hasDigitalProducts = $event"
        @update:has-physics-products="hasPhysicsProducts = $event"
      />
    </div>
    <div class="bg-fff dark:bg-[#252526] px-24 py-16 rounded-normal">
      <ElForm v-if="model" label-position="top">
        <div class="grid grid-cols-3 gap-8">
          <ElFormItem :label="t('common.contact')">
            <ContactCard v-model="model.customerId" readonly />
          </ElFormItem>
          <ElFormItem
            v-if="hasPhysicsProducts"
            :label="t('common.expressShipping')"
          >
            <ShippingCard v-model="model.shippingId" />
          </ElFormItem>
          <ElFormItem
            v-if="hasDigitalProducts"
            :label="t('common.digitalShipping')"
          >
            <DigitalShippingCard v-model="model.digitalShippingId" />
          </ElFormItem>
        </div>

        <ElFormItem :label="t('commerce.note')">
          <ElInput v-model="model.note" :rows="2" type="textarea" />
        </ElFormItem>
      </ElForm>
    </div>
  </div>
  <KBottomBar
    :permission="{
      feature: 'carts',
      action: 'edit',
    }"
    @cancel="goBack"
    @save="save"
  />
</template>

<style scoped>
:deep(.text-center .el-input__inner) {
  text-align: center;
}
</style>
