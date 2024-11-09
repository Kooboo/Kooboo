<script lang="ts" setup>
import { useRoute } from "vue-router";
import Breadcrumb from "@/components/basic/breadcrumb.vue";
import { useI18n } from "vue-i18n";
import { onMounted, ref } from "vue";
import type { PagingParams } from "@/api/commerce/common";
import type { OrderPagingResult } from "@/api/commerce/order";
import {
  getOrders,
  deleteOrders,
  generateOrderExcel,
} from "@/api/commerce/order";
import { useTime } from "@/hooks/use-date";
import { buildOptionsDisplay } from "../products-management/product-variant";
import { useRouteSiteId, useUrlSiteId } from "@/hooks/use-site-id";
import PropertyItem from "../components/property-item.vue";
import CurrencyAmount from "../components/currency-amount.vue";
import { showDeleteConfirm } from "@/components/basic/confirm";
import { openInHiddenFrame } from "@/utils/url";
import TruncateContent from "@/components/basic/truncate-content.vue";

const { t } = useI18n();
const route = useRoute();
const routeName = route.meta.title as string;
const pagingResult = ref<OrderPagingResult>();

const queryParams = ref<
  PagingParams & {
    keyword?: string;
    paid?: boolean;
    deliveryStatus?: string;
    status?: string;
    createdAtStart?: string;
    createdAtEnd?: string;
  }
>({
  pageIndex: 1,
  pageSize: 30,
  keyword: "",
  paid: true,
  status: undefined,
  createdAtStart: undefined,
  createdAtEnd: undefined,
});

async function load(pageIndex = 1) {
  queryParams.value.pageIndex = pageIndex;
  pagingResult.value = await getOrders(queryParams.value);
}

async function onDelete(rows: any[]) {
  await showDeleteConfirm(rows.length);
  await deleteOrders(rows.map((m) => m.id));
  load();
}

async function exportExcel() {
  const rsp = await generateOrderExcel(queryParams);
  openInHiddenFrame(
    useUrlSiteId(
      `${import.meta.env.VITE_API}/Order/ExportExcel?exportfile=${
        rsp.file
      }&name=${rsp.name}`
    )
  );
}

function getClientInfo(row: any) {
  return `${row.clientInfo.os || ""} ${row.clientInfo?.platform || ""} ${
    row.clientInfo?.application?.name || ""
  } ${row.clientInfo?.application?.version || ""} ${row?.userAgent || ""}`;
}

onMounted(async () => {
  load();
});

function getDiscountAllocations(row: any) {
  const list = [];

  if (row.discountAllocations) {
    list.push(...row.discountAllocations);
  }

  for (const i of row.lines) {
    if (i.discountAllocations) {
      list.push(...i.discountAllocations);
    }
  }

  return list;
}
</script>

<template>
  <div class="p-24">
    <Breadcrumb :name="routeName" />
    <div v-if="pagingResult" class="flex items-center space-x-16 mt-8 text-m">
      <PropertyItem :name="t('commerce.delivered')">{{
        pagingResult.stats.delivered
      }}</PropertyItem>
      <PropertyItem :name="t('commerce.waitPay')">{{
        pagingResult.stats.waitPay
      }}</PropertyItem>
      <PropertyItem :name="t('commerce.waitDeliver')">{{
        pagingResult.stats.waitDeliver
      }}</PropertyItem>
      <div class="flex-1" />
      <PropertyItem :name="t('commerce.orderCount')">{{
        pagingResult.stats.order
      }}</PropertyItem>
      <PropertyItem :name="t('commerce.orderTotalAmount')">
        <CurrencyAmount :amount="pagingResult.stats.totalAmount" />
      </PropertyItem>
    </div>
    <div class="flex items-center py-16 space-x-16">
      <el-select
        v-model="queryParams.paid"
        :placeholder="t('common.paymentStatus')"
        class="w-180px"
        clearable
        @clear="queryParams.paid = undefined"
      >
        <el-option :label="t('common.paid')" :value="true" />
        <el-option :label="t('common.notPaid')" :value="false" />
      </el-select>
      <el-select
        v-model="queryParams.deliveryStatus"
        :placeholder="t('commerce.deliveryStatus')"
        class="w-180px"
        clearable
        @clear="queryParams.deliveryStatus = ''"
      >
        <el-option :label="t('commerce.shipped')" value="shipped" />
        <el-option :label="t('commerce.partialShipped')" value="partial" />
        <el-option :label="t('commerce.unshipped')" value="unshipped" />
      </el-select>
      <el-select
        v-model="queryParams.status"
        :placeholder="t('commerce.orderStatus')"
        class="w-180px"
        clearable
        @clear="queryParams.status = undefined"
      >
        <el-option :label="t('common.active')" value="Normal" />
        <el-option :label="t('commerce.canceled')" value="Canceled" />
      </el-select>
      <ElDatePicker
        v-model="queryParams.createdAtStart"
        :placeholder="t('common.startDate')"
      />
      <ElDatePicker
        v-model="queryParams.createdAtEnd"
        :placeholder="t('common.endDate')"
      />
      <ElInput
        v-model="queryParams.keyword"
        class="w-200px"
        :placeholder="t('common.keyword')"
        clearable
      />
      <ElButton round type="primary" @click="load(1)">{{
        t("common.search")
      }}</ElButton>
      <el-button round @click="exportExcel">
        <el-icon class="iconfont icon-share" />
        {{ t("common.exportExcel") }}
      </el-button>
    </div>
    <KTable
      v-if="pagingResult"
      :data="pagingResult.list"
      :pagination="{
        currentPage: pagingResult.pageIndex,
        totalCount: pagingResult.count,
        pageSize: pagingResult.pageSize,
      }"
      show-check
      @delete="onDelete"
      @change="load"
    >
      <el-table-column :label="t('commerce.orderNumber')" width="150">
        <template #default="{ row }">
          <div>{{ row.id }}</div>
        </template>
      </el-table-column>
      <el-table-column :label="t('common.totalAmount')" width="180">
        <template #default="{ row }">
          <CurrencyAmount
            :amount="row.totalAmount"
            :original="row.originalAmount"
            :currency="row.currency"
          />
        </template>
      </el-table-column>
      <el-table-column :label="t('common.createTime')" width="180">
        <template #default="{ row }">{{ useTime(row.createdAt) }}</template>
      </el-table-column>
      <el-table-column :label="t('common.products')" width="100" align="center">
        <template #default="{ row }">
          <el-popover width="auto" trigger="hover" placement="left">
            <template #reference>
              <ElTag round type="success">{{ row.lines.length }}</ElTag>
            </template>
            <div class="space-y-4">
              <div
                v-for="item of row.lines"
                :key="item.variantId"
                class="flex space-x-4"
              >
                <ImageCover
                  v-model="item.image"
                  :description="`x ${item.totalQuantity}`"
                />
                <div>
                  <div>{{ item.title }}</div>
                  <div class="text-s">
                    {{ buildOptionsDisplay(item.options, true) }}
                  </div>
                </div>
              </div>
            </div>
          </el-popover>
        </template>
      </el-table-column>
      <el-table-column :label="t('common.discount')" width="110" align="center">
        <template #default="{ row }">
          <el-popover
            v-if="getDiscountAllocations(row).length"
            width="auto"
            trigger="hover"
            :disabled="!getDiscountAllocations(row).length"
          >
            <template #reference>
              <ElTag round type="success" class="cursor-default">{{
                getDiscountAllocations(row).length
              }}</ElTag>
            </template>
            <div class="space-y-4">
              <table>
                <thead>
                  <th>{{ t("common.title") }}</th>
                  <th class="px-8">{{ t("commerce.discountCode") }}</th>
                  <th>{{ t("common.amount") }}</th>
                </thead>
                <tbody>
                  <tr
                    v-for="(item, index) of getDiscountAllocations(row)"
                    :key="index"
                  >
                    <td>{{ item.title }}</td>
                    <td class="px-8">{{ item.code }}</td>
                    <td>-{{ item.amount?.toFixed(2) }}</td>
                  </tr>
                </tbody>
              </table>
            </div>
          </el-popover>
        </template>
      </el-table-column>

      <el-table-column :label="t('common.customer')" width="300">
        <template #default="{ row }">
          <div>
            <div class="truncate">
              <TruncateContent
                :tip="`${row.customer?.email} ${row.customer?.phone}`"
                >{{ row.customer?.email }}
                {{ row.customer?.phone }}</TruncateContent
              >
            </div>
            <!-- <div class="text-s text-666 truncate">
              <TruncateContent
                :tip="`${row.customer?.firstName} ${row.customer?.lastName}`"
              >
                {{ row.customer?.firstName }} {{ row.customer?.lastName }}
              </TruncateContent>
            </div> -->
          </div>
        </template>
      </el-table-column>
      <el-table-column :label="t('common.paymentMethod')" width="140">
        <template #default="{ row }">
          <ElTag v-if="row.paid" round type="success">{{
            row.paymentMethod
          }}</ElTag>
          <ElTag v-else round type="info">{{ t("common.notPaid") }}</ElTag>
        </template>
      </el-table-column>

      <el-table-column :label="t('commerce.shippingStatus')" width="180">
        <template #default="{ row }">
          <div>
            <ElTag v-if="row.partialDelivered" round type="warning">{{
              t("commerce.partialShipped")
            }}</ElTag>
            <ElTag v-else-if="row.delivered" round type="success">{{
              t("commerce.shipped")
            }}</ElTag>

            <ElTag v-else round type="info">{{
              t("commerce.unshipped")
            }}</ElTag>
          </div>
        </template>
      </el-table-column>

      <el-table-column :label="t('common.country')" width="180" align="center">
        <template #default="{ row }"
          ><Country :name-or-code="row.country"
        /></template>
      </el-table-column>

      <el-table-column :label="t('common.source')" width="180" align="center">
        <template #default="{ row }">{{ row.source }}</template>
      </el-table-column>

      <el-table-column align="right" width="60" fixed="right">
        <template #default="{ row }">
          <router-link
            :to="
              useRouteSiteId({
                name: 'order detail',
                query: {
                  id: row.id,
                },
              })
            "
            class="mx-8"
          >
            <el-tooltip placement="top" :content="t('common.detail')">
              <el-icon class="iconfont icon-eyes hover:text-blue text-l" />
            </el-tooltip>
          </router-link>
        </template>
      </el-table-column>
    </KTable>
  </div>
</template>
