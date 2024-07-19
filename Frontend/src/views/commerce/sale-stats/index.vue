<script lang="ts" setup>
import { ref } from "vue";
import ProductStats from "./product-stats.vue";
import OrderStats from "./order-stats.vue";
import DailyStats from "./daily-stats.vue";
import Breadcrumb from "@/components/basic/breadcrumb.vue";

import { useI18n } from "vue-i18n";
const { t } = useI18n();
const endDate = new Date();
const startDate = new Date();
startDate.setMonth(startDate.getMonth() - 1);
const dateRange = ref<[Date, Date]>([startDate, endDate]);

const tabs = [
  {
    displayName: t("commerce.productStats"),
    value: "ProductStats",
    component: ProductStats,
  },
  {
    displayName: t("commerce.orderStats"),
    value: "OrderStats",
    component: OrderStats,
  },
  {
    displayName: t("commerce.dailyStats"),
    value: "DailyStats",
    component: DailyStats,
  },
];

const activeTab = ref(tabs[0].value);
</script>

<template>
  <div>
    <div class="p-24">
      <Breadcrumb :name="t('common.saleStats')" />
      <div class="absolute top-16 right-24">
        <span class="text-black dark:text-fff/86 mr-12">
          {{ t("common.dateRange") }}
        </span>
        <ElDatePicker v-model="dateRange" type="daterange" />
      </div>
    </div>
    <el-tabs v-model="activeTab">
      <el-tab-pane
        v-for="tab in tabs"
        :key="tab.value"
        :label="tab.displayName"
        :name="tab.value"
      >
        <component
          :is="tab.component"
          v-if="activeTab === tab.value"
          :data-range="dateRange"
        />
      </el-tab-pane>
    </el-tabs>
  </div>
</template>
