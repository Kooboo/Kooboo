<script lang="ts" setup>
import { useRouter } from "vue-router";
import Breadcrumb from "@/components/basic/breadcrumb.vue";
import { useI18n } from "vue-i18n";
import { ref } from "vue";
import { useRouteSiteId } from "@/hooks/use-site-id";
import type { CartEdit } from "@/api/commerce/cart";
import { getCartEdit } from "@/api/commerce/cart";
import { createOrder } from "@/api/commerce/order";
import { getQueryString } from "@/utils/url";
import ProductsEditor from "./products-editor.vue";
import ContactCard from "../components/contact-card.vue";
import AddressSelector from "../components/address-selector.vue";
import type { Address } from "@/api/commerce/customer";

const id = getQueryString("id");
const { t } = useI18n();
const router = useRouter();
const address = ref<Address>();
const note = ref("");
const cart = ref<CartEdit>();

getCartEdit(id!).then(async (rsp) => {
  cart.value = rsp;
});

function goBack() {
  router.goBackOrTo(
    useRouteSiteId({
      name: "carts",
    })
  );
}

async function save() {
  await createOrder({
    cartId: cart.value!.id,
    note: note.value,
    address: address.value,
  });
  goBack();
}

function onAddressSelected(value: Address) {
  address.value = value;
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
      { name: t('commerce.checkout') },
    ]"
  />
  <div class="px-24 pt-0 pb-84px space-y-12">
    <div
      v-if="cart"
      class="bg-fff dark:bg-[#252526] px-24 py-16 rounded-normal"
    >
      <ElForm label-position="top">
        <ElFormItem :label="t('common.products')">
          <div class="w-full">
            <ProductsEditor
              v-model:lines="cart.lines"
              readonly
              :customer-id="cart.customerId"
              :discount-codes="cart.discountCodes"
            />
          </div>
        </ElFormItem>
        <ElFormItem :label="t('common.contact')">
          <ContactCard v-model="cart.customerId" readonly />
        </ElFormItem>
        <ElFormItem :label="t('commerce.cartNote')">
          <div
            class="bg-card dark:bg-444 dark:text-gray rounded-normal px-16 py-8 w-full"
          >
            {{ cart.note ? cart.note : "-" }}
          </div>
        </ElFormItem>
      </ElForm>
    </div>
    <div class="bg-fff dark:bg-[#252526] px-24 py-16 rounded-normal">
      <ElForm v-if="cart" label-position="top">
        <ElFormItem :label="t('commerce.shippingAddress')">
          <AddressSelector
            :customer-id="cart.customerId"
            class="w-full"
            @selected="onAddressSelected"
          />
        </ElFormItem>

        <ElFormItem :label="t('commerce.orderNote')">
          <ElInput v-model="note" :rows="2" type="textarea" />
        </ElFormItem>
      </ElForm>
    </div>
  </div>
  <KBottomBar
    :permission="{
      feature: 'carts',
      action: 'edit',
    }"
    :confirm-label="t('commerce.checkout')"
    @cancel="goBack"
    @save="save"
  />
</template>

<style scoped>
:deep(.text-center .el-input__inner) {
  text-align: center;
}
</style>
