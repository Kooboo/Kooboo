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
import OrderStatus from "./order-status.vue";
import { useCommerceStore } from "@/store/commerce";

const { t } = useI18n();
const route = useRoute();
const routeName = route.meta.title as string;
const pagingResult = ref<OrderPagingResult>();
const commerceStore = useCommerceStore();

const queryParams = ref<
  PagingParams & {
    keyword?: string;
    deliveryStatus?: string;
    status?: string;
    createdAtStart?: string;
    createdAtEnd?: string;
  }
>({
  pageIndex: 1,
  pageSize: 30,
  keyword: "",
  status: "",
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
  const rsp = await generateOrderExcel(queryParams.value);
  openInHiddenFrame(
    useUrlSiteId(
      `${import.meta.env.VITE_API}/Order/ExportExcel?exportfile=${
        rsp.file
      }&name=${rsp.name}`
    )
  );
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
      <PropertyItem :name="t('commerce.paidOrders')">{{
        pagingResult.stats.order
      }}</PropertyItem>
      <PropertyItem :name="t('commerce.paidAmounts')">
        <CurrencyAmount :amount="pagingResult.stats.totalAmount" />
      </PropertyItem>
    </div>
    <div class="flex items-center flex-wrap gap-8 py-16">
      <el-select
        v-model="queryParams.status"
        :placeholder="t('commerce.orderStatus')"
        class="w-150px"
        clearable
        @clear="queryParams.status = undefined"
        @change="load(1)"
      >
        <el-option :label="t('common.active')" value="Normal" />
        <el-option :label="t('common.paid')" value="Paid" />
        <el-option :label="t('common.notPaid')" value="Unpaid" />
        <el-option :label="t('commerce.canceled')" value="Canceled" />
      </el-select>
      <el-select
        v-model="queryParams.deliveryStatus"
        :placeholder="t('commerce.deliveryStatus')"
        class="w-170px"
        clearable
        @clear="queryParams.deliveryStatus = ''"
        @change="load(1)"
      >
        <el-option :label="t('commerce.shipped')" value="shipped" />
        <el-option :label="t('commerce.partialShipped')" value="partial" />
        <el-option :label="t('commerce.unshipped')" value="unshipped" />
      </el-select>
      <ElDatePicker
        v-model="queryParams.createdAtStart"
        class="!w-170px"
        :placeholder="t('common.startDate')"
        @change="load(1)"
      />
      <ElDatePicker
        v-model="queryParams.createdAtEnd"
        class="!w-170px"
        :placeholder="t('common.endDate')"
        @change="load(1)"
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
      <el-button round class="!m-0" @click="exportExcel">
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
      <el-table-column
        v-for="item of commerceStore.settings.orderExtensionFields.filter(
          (f) => f.isSummaryField
        )"
        :key="item.name"
        :label="item.displayName || item.name"
        :width="item.width || 120"
      >
        <template #default="{ row }">
          <div>
            {{row.extensionFields?.find((f: any) => f.key == item.name)?.value}}
          </div>
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
      <el-table-column :label="t('common.status')" width="140">
        <template #default="{ row }">
          <OrderStatus :order="row" />
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
        <template #default="{ row }">
          <Country :name-or-code="row.country" />
        </template>
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
