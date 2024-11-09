<script lang="ts" setup>
import KTable from "@/components/k-table";
import { ref, watch } from "vue";
import {
  getDailySaleStats,
  generateDailySaleStats,
} from "@/api/commerce/sale-stats";
import type { PaginationResponse } from "@/global/types";
import PropertyItem from "../components/property-item.vue";
import { useI18n } from "vue-i18n";
import CurrencyAmount from "../components/currency-amount.vue";
import { openInHiddenFrame } from "@/utils/url";
import { useUrlSiteId } from "@/hooks/use-site-id";
import { useDate } from "@/hooks/use-date";
import type { PagingParams } from "@/api/commerce/common";
const props = defineProps<{ dataRange: [Date, Date] }>();
const { t } = useI18n();
const data = ref<
  PaginationResponse<any> & {
    amount?: number;
    customer?: number;
    order?: number;
    count?: number;
  }
>();
const queryParams = ref<PagingParams>({
  pageIndex: 1,
  pageSize: 30,
});

const load = async (index?: number) => {
  if (index) {
    queryParams.value.pageIndex = index;
  }
  data.value = await getDailySaleStats({
    index,
    startDate: props.dataRange[0],
    endDate: props.dataRange[1],
    pageIndex: queryParams.value.pageIndex,
    pageSize: queryParams.value.pageSize,
  });
};

async function exportExcel() {
  const rsp = await generateDailySaleStats({
    startDate: props.dataRange[0],
    endDate: props.dataRange[1],
  });
  openInHiddenFrame(
    useUrlSiteId(
      `${import.meta.env.VITE_API}/SaleStats/ExportSaleStats?exportfile=${
        rsp.file
      }&name=${rsp.name}`
    )
  );
}

watch(
  () => props.dataRange,
  (val) => {
    val && load(1);
  },
  { immediate: true }
);
</script>

<template>
  <div v-if="data" class="flex items-center pb-16">
    <div class="flex flex-1 items-center space-x-16 text-m">
      <PropertyItem :name="t('common.orders')">{{ data.order }}</PropertyItem>
      <PropertyItem :name="t('common.customer')">{{
        data.customer
      }}</PropertyItem>
      <PropertyItem :name="t('commerce.totalAmount')">
        <CurrencyAmount :amount="data.amount" />
      </PropertyItem>
    </div>
    <el-button round @click="exportExcel">
      <el-icon class="iconfont icon-share" />
      {{ t("common.exportExcel") }}
    </el-button>
  </div>
  <KTable
    v-if="data"
    :data="data?.list"
    :pagination="{
      currentPage: queryParams.pageIndex,
      totalCount: data.count,
      pageSize: queryParams.pageSize,
    }"
    @change="load"
  >
    <el-table-column :label="t('common.date')">
      <template #default="{ row }">
        <span>{{ useDate(row.date) }}</span>
      </template>
    </el-table-column>

    <el-table-column :label="t('common.customer')">
      <template #default="{ row }">
        <span>{{ row.customer }} </span></template
      >
    </el-table-column>

    <el-table-column :label="t('commerce.orderCount')" align="center">
      <template #default="{ row }">
        <span>{{ row.order }} </span></template
      >
    </el-table-column>

    <el-table-column :label="t('common.atv')">
      <template #default="{ row }">
        <CurrencyAmount :amount="row.atv"
      /></template>
    </el-table-column>

    <el-table-column :label="t('common.amount')">
      <template #default="{ row }">
        <CurrencyAmount :amount="row.amount" />
      </template>
    </el-table-column>
  </KTable>
</template>
