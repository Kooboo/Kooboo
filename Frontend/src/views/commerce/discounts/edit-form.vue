<script lang="ts" setup>
import type { ConditionSchema } from "@/api/commerce/common";
import type { Discount } from "@/api/commerce/discount";
import { ref, watch } from "vue";
import { useI18n } from "vue-i18n";
import { getOrderLineSchemas, getOrderSchemas } from "@/api/commerce/discount";
import Condition from "../components/condition.vue";
import DiscountCodeInput from "./discount-code-input.vue";
import { useCommerceStore } from "@/store/commerce";

const props = defineProps<{ model: Discount }>();
const schemas = ref<ConditionSchema[]>([]);
const conditionLabel = ref("");
const commerceStore = useCommerceStore();
const { t } = useI18n();

async function loadSchemas(type: string) {
  if (type == "ProductAmountOff") {
    conditionLabel.value = t("commerce.orderProduct");
    schemas.value = await getOrderLineSchemas();
  } else {
    conditionLabel.value = t("commerce.order");
    schemas.value = await getOrderSchemas();
  }
}

loadSchemas(props.model.type);

watch(
  () => props.model.type,
  () => {
    schemas.value = [];
    props.model.condition.isAny = false;
    props.model.condition.items = [];
    loadSchemas(props.model.type);
  }
);
</script>

<template>
  <div class="px-24 pt-0 pb-84px space-y-12">
    <div class="bg-fff dark:bg-[#252526] px-24 py-16 rounded-normal">
      <ElForm label-position="top">
        <ElFormItem :label="t('common.title')">
          <ElInput v-model="model.title" />
        </ElFormItem>

        <div class="grid grid-cols-2 gap-x-8">
          <ElFormItem :label="t('common.activeTime')">
            <div class="flex items-center space-x-8">
              <ElDatePicker v-model="model.startDate" type="datetime" />
              <span class="text-l font-bold">~</span>
              <ElDatePicker v-model="model.endDate" type="datetime" />
            </div>
          </ElFormItem>
          <ElFormItem :label="t('common.priority')">
            <ElInputNumber v-model.number="model.priority" />
          </ElFormItem>
        </div>
        <div class="grid grid-cols-2 gap-x-8">
          <ElFormItem :label="t('commerce.method')">
            <div class="space-y-4">
              <div>
                <ElRadioGroup v-model="model.method">
                  <ElRadio label="AutomaticDiscount">{{
                    t("commerce.automaticDiscount")
                  }}</ElRadio>
                  <ElRadio label="DiscountCode">{{
                    t("commerce.discountCode")
                  }}</ElRadio>
                </ElRadioGroup>
              </div>
              <DiscountCodeInput
                v-if="model.method == 'DiscountCode'"
                v-model="model.code"
                auto
              />
            </div>
          </ElFormItem>
          <ElFormItem :label="t('commerce.isExclusion')">
            <ElSwitch v-model="model.isExclusion" />
          </ElFormItem>
        </div>
      </ElForm>
    </div>

    <div class="bg-fff dark:bg-[#252526] px-24 py-16 rounded-normal">
      <ElForm label-position="top">
        <ElFormItem :label="t('common.type')">
          <div class="space-y-4 w-full">
            <div class="space-y-4">
              <div>
                <ElRadioGroup v-model="model.type">
                  <ElRadio label="ProductAmountOff">{{
                    t("commerce.productAmountOff")
                  }}</ElRadio>
                  <ElRadio label="OrderAmountOff">{{
                    t("commerce.orderAmountOff")
                  }}</ElRadio>
                  <ElRadio label="FreeShipping">{{
                    t("commerce.freeShipping")
                  }}</ElRadio>
                </ElRadioGroup>
              </div>
            </div>
          </div>
        </ElFormItem>

        <ElFormItem
          v-if="model.type != 'FreeShipping'"
          :label="t('commerce.accordingBy')"
        >
          <div>
            <div>
              <ElRadioGroup v-model="model.isPercent">
                <ElRadio :label="false">{{ t("common.amount") }}</ElRadio>
                <ElRadio :label="true">{{ t("common.percent") }}</ElRadio>
              </ElRadioGroup>
            </div>

            <ElInput v-model.number="model.value" class="w-280px">
              <template #suffix>
                <div
                  class="w-16 h-16 flex items-center justify-center transform origin-center scale-75"
                >
                  {{
                    model.isPercent ? "%" : commerceStore.settings.currencyCode
                  }}
                </div>
              </template>
            </ElInput>
          </div>
        </ElFormItem>
      </ElForm>
    </div>

    <div class="bg-fff dark:bg-[#252526] px-24 py-16 rounded-normal">
      <Condition
        v-if="schemas.length"
        :condition="model.condition"
        :schemas="schemas"
        :title="conditionLabel"
      />
    </div>
  </div>
</template>
