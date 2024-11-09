<script lang="ts" setup>
import KBottomBar from "@/components/k-bottom-bar/index.vue";
import Breadcrumb from "@/components/basic/breadcrumb.vue";
import { useI18n } from "vue-i18n";
import { useShortcut } from "@/hooks/use-shortcuts";
import { ref, watch } from "vue";
import type { Currency } from "@/api/commerce/settings";
import {
  getCurrencies,
  getPayments,
  saveSettings,
} from "@/api/commerce/settings";
import type { Settings } from "@/api/commerce/settings";
import { useCommerceStore } from "@/store/commerce";
import type { KeyValue } from "@/global/types";
import { refreshMonacoCache } from "@/components/monaco-editor/monaco";
import CustomDataEditor from "./custom-data-editor.vue";
import { productLabels, categoryLabels } from "../useLabels";
import CurrencyAmount from "../components/currency-amount.vue";

const { t } = useI18n();
const currencies = ref<Currency[]>([]);
const payments = ref<KeyValue[]>([]);
const store = useCommerceStore();
getCurrencies().then((rsp) => {
  currencies.value = rsp;
});

getPayments().then((rsp) => {
  payments.value = rsp;
});
const settings = ref<Settings>();

const onSave = async () => {
  await saveSettings(settings.value!);
  store.loadSettings();
  refreshMonacoCache();
};

function onCurrencyCodeChange(value: string) {
  if (!currencies.value) return;
  var currency = currencies.value.find((f) => f.code == value);
  if (currency) {
    settings.value!.currencySymbol = currency.symbolNative;
  }
}

watch(
  () => store.settings,
  () => {
    settings.value = JSON.parse(JSON.stringify(store.settings));
  },
  {
    immediate: true,
  }
);

useShortcut("save", onSave);
</script>

<template>
  <div class="p-24 pb-150px">
    <Breadcrumb :name="t('commerce.commerceSettings')" />
    <ElForm v-if="settings" label-position="top">
      <div
        class="rounded-normal bg-fff dark:bg-[#252526] mt-16 mb-24 py-24 px-56px"
      >
        <div class="max-w-504px">
          <el-form-item :label="t('common.currency')">
            <el-select
              v-model="settings.currencyCode"
              class="w-full"
              @change="onCurrencyCodeChange"
            >
              <el-option
                v-for="item of currencies"
                :key="item.code"
                :value="item.code"
                :label="`${item.namePlural} - ${item.code} - ${item.symbolNative}`"
              />
            </el-select>
          </el-form-item>
          <!-- <el-form-item :label="t('commerce.shippingCost')">
            <ElInput v-model.number="settings.shippingCost" />
          </el-form-item> -->
          <el-form-item :label="t('common.weightUnit')">
            <ElSelect v-model="settings.weightUnit" class="w-full">
              <ElOption label="KG" value="kg" />
              <ElOption label="G" value="g" />
            </ElSelect>
          </el-form-item>
          <el-form-item>
            <template #label>
              <span>{{ t("commerce.availablePaymentMethods") }}</span>
              <Tooltip :tip="t('commerce.paymentsTip')" custom-class="ml-4" />
            </template>
            <ElSelect v-model="settings.payments" multiple class="w-full">
              <ElOption
                v-for="item of payments"
                :key="item.key"
                :label="item.value"
                :value="item.key"
              />
            </ElSelect>
          </el-form-item>
        </div>
      </div>

      <div
        class="rounded-normal bg-fff dark:bg-[#252526] mt-16 mb-24 py-24 px-56px"
      >
        <div class="max-w-504px">
          <el-form-item>
            <template #label>
              <span>{{ t("commerce.productFields") }}</span>
              <Tooltip
                :tip="t('common.productFieldsTip')"
                custom-class="ml-4"
              />
            </template>
            <CustomDataEditor
              :data="settings.productCustomFields"
              :labels="productLabels"
            />
          </el-form-item>

          <el-form-item>
            <template #label>
              <span>{{ t("commerce.categoryFields") }}</span>
              <Tooltip
                :tip="t('common.categoryFieldsTip')"
                custom-class="ml-4"
              />
            </template>
            <CustomDataEditor
              :data="settings.categoryCustomFields"
              :labels="categoryLabels"
            />
          </el-form-item>
        </div>
      </div>
    </ElForm>

    <KBottomBar hidden-cancel @save="onSave" />
  </div>
</template>
