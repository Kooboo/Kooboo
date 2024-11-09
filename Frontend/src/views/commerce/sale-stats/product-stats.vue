<script lang="ts" setup>
import KTable from "@/components/k-table";
import { ref, watch } from "vue";
import {
  getProductSaleStats,
  generateProductSaleStats,
} from "@/api/commerce/sale-stats";
import type { PaginationResponse } from "@/global/types";
import PropertyItem from "../components/property-item.vue";

import { useI18n } from "vue-i18n";
import CurrencyAmount from "../components/currency-amount.vue";
import { openInHiddenFrame } from "@/utils/url";
import { useUrlSiteId } from "@/hooks/use-site-id";
import DynamicColumns from "@/components/dynamic-columns/index.vue";
import { useProductFields } from "../useFields";
import type { PagingParams } from "@/api/commerce/common";
const props = defineProps<{ dataRange: [Date, Date] }>();
const { t } = useI18n();
const data = ref<
  PaginationResponse<any> & {
    amount?: number;
    product?: number;
    count?: number;
  }
>();
const queryParams = ref<PagingParams>({
  pageIndex: 1,
  pageSize: 30,
});

const { getColumns } = useProductFields();

const columns = getColumns([
  {
    name: "productName",
    displayName: t("commerce.product"),
  },
  {
    name: "variantsCount",
    prop: "options",
  },
  {
    name: "sku",
    displayName: "SKU",
  },
  {
    name: "order",
    displayName: t("commerce.orderCount"),
    attrs: {
      align: "center",
    },
  },
  {
    name: "avgPrice",
    displayName: t("common.price"),
  },
  {
    name: "quantity",
    displayName: t("common.quantity"),
    attrs: {
      align: "center",
    },
  },
  {
    name: "amount",
    displayName: t("common.amount"),
  },
]);

const load = async (index?: number) => {
  if (index) {
    queryParams.value.pageIndex = index;
  }
  data.value = await getProductSaleStats({
    index,
    startDate: props.dataRange[0],
    endDate: props.dataRange[1],
    pageIndex: queryParams.value.pageIndex,
    pageSize: queryParams.value.pageSize,
  });
};

async function exportExcel() {
  const rsp = await generateProductSaleStats({
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
      <PropertyItem :name="t('common.products')">{{
        data.product
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
    <DynamicColumns :columns="columns">
      <template #avgPrice="{ row }">
        <CurrencyAmount :amount="row.avgPrice" />
      </template>
      <template #amount="{ row }">
        <CurrencyAmount :amount="row.amount" />
      </template>
    </DynamicColumns>
  </KTable>
</template>
