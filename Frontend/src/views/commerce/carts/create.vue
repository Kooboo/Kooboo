<script lang="ts" setup>
import { useRouter } from "vue-router";
import Breadcrumb from "@/components/basic/breadcrumb.vue";
import { useI18n } from "vue-i18n";
import { ref } from "vue";
import { useRouteSiteId } from "@/hooks/use-site-id";
import type { CartCreate } from "@/api/commerce/cart";
import { createCart } from "@/api/commerce/cart";
import ContactCard from "../components/contact-card.vue";
import ProductsEditor from "./products-editor.vue";

const { t } = useI18n();
const router = useRouter();

const model = ref<CartCreate>({
  customerId: "",
  discountCodes: [],
  lines: [],
  note: "",
});

function goBack() {
  router.goBackOrTo(
    useRouteSiteId({
      name: "carts",
    })
  );
}

async function save() {
  await createCart(model.value);
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
      { name: t('common.create') },
    ]"
  />
  <div class="px-24 pt-0 pb-84px space-y-12">
    <div class="bg-fff dark:bg-[#252526] px-24 py-16 rounded-normal">
      <ProductsEditor
        v-model:lines="model.lines"
        :customer-id="model.customerId"
        :discount-codes="model.discountCodes"
      />
    </div>
    <div class="bg-fff dark:bg-[#252526] px-24 py-16 rounded-normal">
      <ElForm label-position="top">
        <ElFormItem :label="t('common.contact')">
          <ContactCard v-model="model.customerId" />
        </ElFormItem>
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
