<script lang="ts" setup>
import { useI18n } from "vue-i18n";
import type { Settings } from "@/api/commerce/settings";
import { saveSettings } from "@/api/commerce/settings";
import { ref, watch } from "vue";
import { useShortcut } from "@/hooks/use-shortcuts";
import { useCommerceStore } from "@/store/commerce";
import CurrencyAmount from "../components/currency-amount.vue";
import OrderRedeemEditor from "./earn-points-config/order-redeem-editor.vue";

const { t } = useI18n();
const store = useCommerceStore();
const settings = ref<Settings>();

async function save() {
  await saveSettings(settings.value!);
  store.loadSettings();
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

useShortcut("save", save);
</script>

<template>
  <ElForm v-if="settings" label-position="top">
    <div
      class="rounded-normal bg-fff dark:bg-[#252526] mt-16 mb-24 py-24 px-56px"
    >
      <el-form-item :label="t('commerce.pointExchangeAmount')">
        <div class="flex items-center gap-8 dark:text-[#cfd3dc]">
          <ElInputNumber v-model.number="settings.redeemPoint.exchangeRate" />
          <div class="flex items-center gap-4">
            <span class="font-bold text-l text-blue">{{
              settings.redeemPoint.exchangeRate
            }}</span>
            <span>{{ t("commerce.points") }}</span>
          </div>
          <span>=</span>
          <CurrencyAmount :amount="1" :currency="settings.currencyCode" />
        </div>
      </el-form-item>
      <OrderRedeemEditor :redeem-point="settings.redeemPoint" />
    </div>
  </ElForm>
  <KBottomBar
    :permission="{
      feature: 'loyalty',
      action: 'edit',
    }"
    hidden-cancel
    @save="save"
  />
</template>
