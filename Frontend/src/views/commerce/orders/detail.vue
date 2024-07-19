<script lang="ts" setup>
import { useRouter } from "vue-router";
import Breadcrumb from "@/components/basic/breadcrumb.vue";
import { useI18n } from "vue-i18n";
import { ref } from "vue";
import { useRouteSiteId } from "@/hooks/use-site-id";
import type { OrderDetail } from "@/api/commerce/order";
import { getOrderDetail } from "@/api/commerce/order";
import { getQueryString } from "@/utils/url";
import { useTime } from "@/hooks/use-date";
import { buildOptionsDisplay } from "../products-management/product-variant";
import PropertyItem from "../components/property-item.vue";
import CancelDialog from "./cancel-dialog.vue";
import DeliveryDialog from "./delivery-dialog.vue";
import PaymentDialog from "./payment-dialog.vue";
import DynamicColumns from "@/components/dynamic-columns/index.vue";
import { useProductFields } from "../useFields";
import CurrencyAmount from "../components/currency-amount.vue";

const { getColumns } = useProductFields();
const id = getQueryString("id");
const { t } = useI18n();
const router = useRouter();
const showCancelDialog = ref(false);
const showDeliveryDialog = ref(false);
const showPaymentDialog = ref(false);
const columns = getColumns([
  {
    name: "featuredImage",
    prop: "image",
    attrs: {
      width: 80,
      align: "center",
    },
  },
  {
    name: "title",
  },
  {
    name: "quantity",
    displayName: t("common.quantity"),
    attrs: {
      align: "center",
    },
  },
  {
    name: "price",
    displayName: t("common.price"),
  },
  {
    name: "totalAmount",
    displayName: t("common.totalAmount"),
  },
  {
    name: "discounts",
    displayName: t("common.discounts"),
    attrs: {
      align: "center",
    },
  },
]);
const model = ref<OrderDetail>();

async function load() {
  model.value = await getOrderDetail(id!);
}

function getClientInfo(row: any) {
  return `${row.clientInfo.os || ""} ${row.clientInfo?.platform || ""} ${
    row.clientInfo?.application?.name || ""
  } ${row.clientInfo?.application?.version || ""} ${row?.userAgent || ""}`;
}

load();

function goBack() {
  router.goBackOrTo(
    useRouteSiteId({
      name: "orders",
    })
  );
}
</script>

<template>
  <Breadcrumb
    class="p-24"
    :crumb-path="[
      {
        name: t('common.orders'),
        route: { name: 'orders' },
      },
      { name: t('common.orderDetail') },
    ]"
  />
  <div v-if="model" class="px-24 pt-0 pb-84px space-y-12">
    <div class="bg-fff dark:bg-[#252526] px-24 py-16 rounded-normal">
      <el-descriptions :title="t('common.basicInfo')">
        <el-descriptions-item :label="t('commerce.orderNumber')"
          >{{ model.id }}
          <ElTag v-if="model.canceled" type="danger" size="small"
            >{{ t("commerce.canceled") }}
          </ElTag>
        </el-descriptions-item>
        <el-descriptions-item :label="t('common.createTime')">{{
          useTime(model.createdAt)
        }}</el-descriptions-item>
        <el-descriptions-item :label="t('common.lastModified')">{{
          useTime(model.updatedAt)
        }}</el-descriptions-item>
        <el-descriptions-item :label="t('common.contact')">
          {{ model.customer.firstName }}
          {{ model.customer.lastName }}</el-descriptions-item
        >
        <el-descriptions-item :label="t('common.email')">{{
          model.customer.email
        }}</el-descriptions-item>
        <el-descriptions-item :label="t('common.phone')">{{
          model.customer.phone
        }}</el-descriptions-item>
        <el-descriptions-item label="IP">{{ model.ip }}</el-descriptions-item>
        <el-descriptions-item :label="t('common.country')"
          ><Country :name-or-code="model.country"
        /></el-descriptions-item>
        <el-descriptions-item :label="t('common.source')">{{
          model.source
        }}</el-descriptions-item>
        <el-descriptions-item :label="t('common.clientInfo')">
          <div
            v-if="model.clientInfo"
            class="text-s inline-flex space-x-4 items-center"
          >
            <ElTag
              v-if="model.clientInfo?.application?.isWebBrowser"
              round
              size="small"
              >{{ t("common.webBrowser") }}</ElTag
            >
            <div class="ellipsis" :title="getClientInfo(model)">
              {{ getClientInfo(model) }}
            </div>
          </div>
        </el-descriptions-item>

        <el-descriptions-item :label="t('common.paymentMethod')">
          <ElTag v-if="model.paid" round type="success">{{
            model.paymentMethod
          }}</ElTag>
          <ElTag v-else round type="info">{{ t("common.notPaid") }}</ElTag>
        </el-descriptions-item>
        <el-descriptions-item :label="t('commerce.deliveryMethod')">
          <span v-if="model.delivered">
            {{ model.shippingCarrier }} {{ model.trackingNumber }}
          </span>
          <ElTag v-else round type="info">{{ t("commerce.unshipped") }}</ElTag>
        </el-descriptions-item>
      </el-descriptions>
      <ElTable :data="model.lines" class="el-table--gray">
        <DynamicColumns :columns="columns">
          <template #title="{ row }">
            <div>
              <div>{{ row.title }}</div>
              <div class="text-s">{{ buildOptionsDisplay(row.options) }}</div>
            </div>
          </template>
          <template #price="{ row }">
            <CurrencyAmount
              :currency="model.currency"
              :original="row.originalPrice"
              :amount="row.price"
            />
          </template>
          <template #totalAmount="{ row }">
            <CurrencyAmount
              :currency="model.currency"
              :amount="row.totalAmount"
            />
          </template>
          <template #discounts="{ row }">
            <div class="inline-flex flex-wrap gap-4">
              <ElTag
                v-for="(item, index) of row.discountAllocations"
                :key="index"
                >{{ item.title }}</ElTag
              >
            </div>
          </template>
        </DynamicColumns>
      </ElTable>

      <div class="text-s mt-12">
        <PropertyItem
          v-if="model.discountAllocations.length"
          :name="t('common.discounts')"
        >
          <div class="flex gap-4">
            <ElTag
              v-for="(item, index) of model.discountAllocations"
              :key="index"
              >{{ item.title }}</ElTag
            >
          </div>
        </PropertyItem>
        <PropertyItem :name="t('common.totalAmount')">
          <CurrencyAmount
            :currency="model.currency"
            :amount="model.totalAmount"
            :original="model.originalAmount"
          />
        </PropertyItem>
      </div>
    </div>

    <div class="bg-fff dark:bg-[#252526] px-24 py-16 rounded-normal">
      <el-descriptions :title="t('commerce.shippingAddress')">
        <el-descriptions-item :label="t('common.country')">{{
          model.shippingAddress?.country
        }}</el-descriptions-item>
        <el-descriptions-item :label="t('common.contact')">
          {{ model.shippingAddress?.firstName }}
          {{ model.shippingAddress?.lastName }}
        </el-descriptions-item>
        <el-descriptions-item :label="t('common.phone')">{{
          model.shippingAddress?.phone
        }}</el-descriptions-item>
        <el-descriptions-item :label="t('common.province')">{{
          model.shippingAddress?.province
        }}</el-descriptions-item>
        <el-descriptions-item :label="t('common.city')">{{
          model.shippingAddress?.city
        }}</el-descriptions-item>
        <el-descriptions-item :label="t('common.postalCode')">{{
          model.shippingAddress?.zip
        }}</el-descriptions-item>
        <el-descriptions-item :label="t('common.address')" :span="3">{{
          model.shippingAddress?.address1
        }}</el-descriptions-item>
      </el-descriptions>
    </div>
    <div class="bg-fff dark:bg-[#252526] px-24 py-16 rounded-normal">
      <el-descriptions :title="t('commerce.note')">
        <div />
      </el-descriptions>
      <div class="text-m text-999">{{ model.note }}</div>
    </div>
    <CancelDialog
      v-if="showCancelDialog"
      :id="model.id"
      v-model="showCancelDialog"
      @reload="load"
    />

    <DeliveryDialog
      v-if="showDeliveryDialog"
      :id="model.id"
      v-model="showDeliveryDialog"
      @reload="load"
    />

    <PaymentDialog
      v-if="showPaymentDialog"
      :id="model.id"
      v-model="showPaymentDialog"
      @reload="load"
    />
  </div>

  <KBottomBar
    v-if="model"
    :permission="{
      feature: 'orders',
      action: 'edit',
    }"
    back
    hidden-confirm
    @cancel="goBack"
  >
    <template #extra-buttons>
      <el-button
        v-if="!model?.paid && !model?.canceled"
        round
        type="primary"
        @click="showPaymentDialog = true"
      >
        {{ t("common.pay") }}
      </el-button>
      <el-button
        v-if="model?.paid && !model?.canceled && !model?.delivered"
        round
        type="primary"
        @click="showDeliveryDialog = true"
      >
        {{ t("commerce.delivery") }}
      </el-button>
      <el-button
        v-if="!model?.canceled"
        round
        type="danger"
        @click="showCancelDialog = true"
      >
        {{ t("common.cancel") }}
      </el-button>
    </template>
  </KBottomBar>
</template>
