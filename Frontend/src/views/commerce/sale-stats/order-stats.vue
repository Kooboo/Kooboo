<script lang="ts" setup>
import KTable from "@/components/k-table";
import { ref, watch } from "vue";
import {
  getOrderSaleStats,
  generateOrderSaleStats,
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
  PaginationResponse<any> & { amount?: number; order?: number; count?: number }
>();
const queryParams = ref<PagingParams>({
  pageIndex: 1,
  pageSize: 30,
});
const { getColumns } = useProductFields();

const columns = getColumns([
  {
    name: "paidAt",
    displayName: t("common.date"),
    controlType: "DateTime",
    attrs: {
      width: 180,
    },
  },
  {
    name: "orderId",
    displayName: t("commerce.orderNumber"),
    attrs: {
      width: 150,
    },
  },
  {
    name: "productName",
    displayName: t("commerce.product"),
    attrs: {
      width: 240,
    },
  },
  {
    name: "variantsCount",
    prop: "options",
    attrs: {
      width: 120,
    },
  },
  {
    name: "sku",
    displayName: "SKU",
    attrs: {
      width: 120,
    },
  },
  {
    name: "originalPrice",
    displayName: t("common.originalPrice"),
    attrs: {
      width: 120,
    },
  },
  {
    name: "avgPrice",
    displayName: t("common.price"),
    attrs: {
      width: 120,
    },
  },
  {
    name: "quantity",
    displayName: t("common.quantity"),
    attrs: {
      width: 100,
    },
  },
  {
    name: "amount",
    displayName: t("common.amount"),
    attrs: {
      width: 120,
    },
  },
  {
    name: "paymentMethod",
    displayName: t("common.paymentMethod"),
    attrs: {
      width: 140,
    },
  },
  {
    name: "address",
    displayName: t("common.address"),
    attrs: {
      width: 280,
    },
  },
  {
    name: "phone",
    displayName: t("common.phone"),
    attrs: {
      width: 120,
    },
  },
  {
    name: "customer",
    displayName: t("common.customer"),
    attrs: {
      width: 120,
    },
  },
]);
const load = async (index?: number) => {
  if (index) {
    queryParams.value.pageIndex = index;
  }
  data.value = await getOrderSaleStats({
    index,
    startDate: props.dataRange[0],
    endDate: props.dataRange[1],
    pageIndex: queryParams.value.pageIndex,
    pageSize: queryParams.value.pageSize,
  });
};

async function exportExcel() {
  const rsp = await generateOrderSaleStats({
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
      <template #originalPrice="{ row }">
        <CurrencyAmount :amount="row.originalPrice" />
      </template>
      <template #amount="{ row }">
        <CurrencyAmount :amount="row.amount" />
      </template>
    </DynamicColumns>
  </KTable>
</template>
