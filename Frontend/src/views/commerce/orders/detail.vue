<script lang="ts" setup>
import { useRouter } from "vue-router";
import Breadcrumb from "@/components/basic/breadcrumb.vue";
import { useI18n } from "vue-i18n";
import { computed, ref } from "vue";
import { useRouteSiteId } from "@/hooks/use-site-id";
import type { OrderDetail } from "@/api/commerce/order";
import {
  getOrderDetail,
  updateKeyValue,
  updateNote,
} from "@/api/commerce/order";
import { combineUrl, getQueryString, openInNewTab } from "@/utils/url";
import { useTime } from "@/hooks/use-date";
import { buildOptionsDisplay } from "../products-management/product-variant";
import PropertyItem from "../components/property-item.vue";
import CancelDialog from "./cancel-dialog.vue";
import DeliveryDialog from "./delivery-dialog.vue";
import PaymentDialog from "./payment-dialog.vue";
import DynamicColumns from "@/components/dynamic-columns/index.vue";
import { useProductFields } from "../useFields";
import CurrencyAmount from "../components/currency-amount.vue";
import { useSiteStore } from "@/store/site";
import DeliveryStatus from "./delivery-status.vue";
import OrderStatus from "./order-status.vue";
import { getDetails } from "@/api/commerce/address";
import { systemDisplay } from "@/utils/commerce";
import type { KeyValue } from "@/global/types";
import { useCommerceStore } from "@/store/commerce";

const { getColumns } = useProductFields();
const id = getQueryString("id");
const { t } = useI18n();
const router = useRouter();
const showCancelDialog = ref(false);
const showDeliveryDialog = ref(false);
const showPaymentDialog = ref(false);
const showNoteDialog = ref(false);
const showKeyValueDialog = ref(false);
const siteStore = useSiteStore();
const note = ref("");
const commerceStore = useCommerceStore();

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
    name: "price",
    displayName: t("common.price"),
  },
  {
    name: "totalQuantity",
    displayName: t("common.quantity"),
    attrs: {
      align: "center",
    },
  },
  {
    name: "totalAmount",
    displayName: t("common.totalAmount"),
  },
  {
    name: "taxAmount",
    displayName: t("common.tax"),
  },
  {
    name: "discounts",
    displayName: t("common.discount"),
    attrs: {
      align: "center",
    },
  },
]);
const model = ref<OrderDetail>();
const addressDetail = ref();

async function load() {
  model.value = await getOrderDetail(id!);
  if (model.value.shippingAddress) {
    getDetails([model.value.shippingAddress]).then(
      (rsp) => (addressDetail.value = rsp[0])
    );
  }
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

const data = computed(() => {
  const result: any[] = [];
  if (!model.value?.lines) return result;
  for (const line of model.value.lines) {
    if (!line.groupName) {
      result.push({ ...line, rowKey: line.variantId + line.groupName });
    } else if (line.isMain) {
      const children = model.value.lines
        .filter((f) => f.groupName == line.groupName && !f.isMain)
        .map((m) => ({
          ...m,
          rowKey: m.variantId + m.groupName,
          isChild: true,
        }));

      result.push({
        ...line,
        rowKey: line.variantId + line.groupName,
        children,
      });
    } else {
      const mainLine = model.value.lines.find(
        (f) => f.groupName == line.groupName && f.isMain
      );
      if (!mainLine) {
        result.push({ ...line, rowKey: line.variantId + line.groupName });
      }
    }
  }

  return result;
});

function tableRowClassName({ row }: any) {
  if (!row.isChild) return;
  return "text-999 text-s !bg-gray/20";
}

function extensionButtonClick(data: any) {
  if (!data.url) return;
  let url = combineUrl(siteStore.site.baseUrl, data.url);
  openInNewTab(url);
}

async function editNode() {
  await updateNote({
    note: note.value,
    id: id,
  });
  load();
  showNoteDialog.value = false;
}

const keyvalue = ref<KeyValue>();

async function editKeyValue() {
  await updateKeyValue({
    id,
    key: keyvalue.value?.key,
    value: keyvalue.value?.value,
  });
  load();
  showKeyValueDialog.value = false;
}

function getExtensionFieldValue(name: string) {
  return model.value?.extensionFields?.find((f) => f.key == name)?.value;
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
    <div class="bg-fff dark:bg-[#252526] px-24 py-16 rounded-normal relative">
      <el-descriptions :title="t('common.basicInfo')">
        <el-descriptions-item :label="t('commerce.orderNumber')"
          >{{ model.id }}
          <OrderStatus :order="model" />
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
        <el-descriptions-item :label="t('common.country')">
          <Country :name-or-code="model.country" />
        </el-descriptions-item>
        <el-descriptions-item v-if="model.source" :label="t('common.source')">{{
          model.source
        }}</el-descriptions-item>
        <el-descriptions-item
          v-if="model.scheduledDeliveryTime"
          :label="t('common.scheduledDeliveryTime')"
          >{{ useTime(model.scheduledDeliveryTime) }}</el-descriptions-item
        >
        <el-descriptions-item :label="t('common.clientInfo')">
          <div
            v-if="model.clientInfo"
            class="text-s inline-flex space-x-4 items-center"
          >
            <ElTag
              v-if="model.clientInfo?.application?.isWebBrowser"
              round
              size="small"
              >{{ t("common.webBrowser") }}
            </ElTag>
            <div class="ellipsis" :title="getClientInfo(model)">
              {{ getClientInfo(model) }}
            </div>
          </div>
        </el-descriptions-item>

        <el-descriptions-item
          v-if="model.paymentMethod"
          :label="t('common.paymentMethod')"
        >
          <ElTag round type="success">{{ model.paymentMethod }}</ElTag>
        </el-descriptions-item>
        <el-descriptions-item :label="t('commerce.shippingStatus')">
          <ElTag v-if="model.partialDelivered" round type="warning">{{
            t("commerce.partialShipped")
          }}</ElTag>
          <ElTag v-else-if="model.delivered" round type="success">{{
            t("commerce.shipped")
          }}</ElTag>

          <ElTag v-else round type="info">{{ t("commerce.unshipped") }}</ElTag>
        </el-descriptions-item>
        <template v-if="commerceStore.settings.orderExtensionFields">
          <el-descriptions-item
            v-for="(i, index) of commerceStore.settings.orderExtensionFields"
            :key="index"
            :label="i.displayName || i.name"
          >
            <div class="max-w-150px inline-flex items-center gap-x-4">
              <TruncateContent
                v-if="getExtensionFieldValue(i.name)"
                :tip="getExtensionFieldValue(i.name)"
                >{{ getExtensionFieldValue(i.name) }}</TruncateContent
              >
              <el-icon
                v-if="i.editable"
                class="cursor-pointer iconfont icon-a-writein text-blue"
                @click="
                  keyvalue = {
                    key: i.name,
                    value: getExtensionFieldValue(i.name) || '',
                  };
                  showKeyValueDialog = true;
                "
              />
            </div>
          </el-descriptions-item>
        </template>
      </el-descriptions>
      <ElTable
        :data="data"
        row-key="rowKey"
        class="el-table--gray"
        default-expand-all
        :row-class-name="tableRowClassName"
      >
        <ElTableColumn v-if="data.some((s) => s.children?.length)" width="25" />
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
              :original="row.originalAmount"
            />
          </template>
          <template #taxAmount="{ row }">
            <CurrencyAmount
              :currency="model.currency"
              :amount="row.taxAmount"
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
        <ElTableColumn :label="t('commerce.deliveryStatus')">
          <template #default="{ row }">
            <DeliveryStatus :line="row" />
          </template>
        </ElTableColumn>
        <ElTableColumn align="right">
          <template #default="{ row }">
            <ElButton
              v-if="row.extensionButton"
              type="primary"
              size="small"
              @click="extensionButtonClick(row.extensionButton)"
            >
              {{ row.extensionButton.text }}
            </ElButton>
          </template>
        </ElTableColumn>
      </ElTable>

      <div class="text-s mt-12">
        <div class="relative flex">
          <PropertyItem
            v-if="model.discountAllocations.length"
            :name="t('common.discount')"
          >
            <div class="flex gap-4">
              <ElTag
                v-for="(item, index) of model.discountAllocations"
                :key="index"
                >{{ item.title }}</ElTag
              >
            </div>
          </PropertyItem>
          <div class="absolute right-0 space-y-4">
            <div
              v-if="model.earnPoints > 0"
              class="flex items-center gap-4 text-s justify-end"
            >
              <span>{{ t("common.earn") }}</span>
              <span>{{ model.earnPoints }}</span>
              <span>{{ t("commerce.points") }}</span>
            </div>
          </div>
        </div>

        <PropertyItem :name="t('commerce.subtotalAmount')">
          <CurrencyAmount
            :amount="model?.subtotalAmount"
            :currency="model.currency"
            :original="model.originalSubtotalAmount"
          />
        </PropertyItem>
        <PropertyItem :name="t('common.tax')">
          <CurrencyAmount
            :amount="model?.taxAmount"
            :currency="model.currency"
          />
        </PropertyItem>
        <PropertyItem
          v-if="model?.insuranceAmount"
          :name="t('commerce.insuranceAmount')"
        >
          <CurrencyAmount
            :amount="model?.insuranceAmount"
            :currency="model.currency"
          />
        </PropertyItem>
        <PropertyItem
          v-if="model?.pointsDeductionAmount"
          :name="t('commerce.points')"
        >
          <CurrencyAmount :amount="-model?.pointsDeductionAmount" />
          ({{ model.redeemPoints }} {{ t("commerce.points") }})
        </PropertyItem>
        <PropertyItem :name="t('commerce.shippingAmount')">
          <div class="flex items-center gap-8">
            <CurrencyAmount
              :amount="model?.shippingAmount"
              :currency="model.currency"
            />
            <div
              v-if="(model?.shippingAllocations?.length ?? 0) > 1"
              class="flex items-center gap-4"
            >
              (
              <div
                v-for="(item, index) of model!.shippingAllocations"
                :key="index"
                class="flex items-center gap-4"
              >
                <ElTooltip placement="top" :content="item.title">
                  <CurrencyAmount
                    :amount="item.cost"
                    :currency="model.currency"
                  />
                </ElTooltip>
                <span v-if="index != model!.shippingAllocations.length - 1"
                  >+</span
                >
              </div>
              )
            </div>
          </div>
        </PropertyItem>
        <PropertyItem :name="t('common.total')">
          <CurrencyAmount
            :currency="model.currency"
            :amount="model.totalAmount"
            :original="model.originalAmount"
          />
        </PropertyItem>
      </div>
    </div>

    <div
      v-if="model.shippingAddress"
      class="bg-fff dark:bg-[#252526] px-24 py-16 rounded-normal"
    >
      <el-descriptions :title="t('commerce.shippingAddress')">
        <el-descriptions-item :label="t('common.country')">{{
          systemDisplay(
            addressDetail?.countryDetail?.nameTranslations,
            model.shippingAddress?.country
          )
        }}</el-descriptions-item>
        <el-descriptions-item :label="t('common.contact')">
          {{ model.shippingAddress?.firstName }}
          {{ model.shippingAddress?.lastName }}
        </el-descriptions-item>
        <el-descriptions-item :label="t('common.phone')">{{
          model.shippingAddress?.phone
        }}</el-descriptions-item>
        <el-descriptions-item :label="t('common.province')">{{
          systemDisplay(
            addressDetail?.provinceDetail?.nameTranslations,
            model.shippingAddress?.province
          )
        }}</el-descriptions-item>
        <el-descriptions-item :label="t('common.city')">{{
          systemDisplay(
            addressDetail?.cityDetail?.nameTranslations,
            model.shippingAddress?.city
          )
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
        <template #extra>
          <el-icon
            class="cursor-pointer iconfont icon-a-writein text-blue text-l"
            @click="
              note = model.note;
              showNoteDialog = true;
            "
          />
        </template>
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
      :lines="model.lines"
      @reload="load"
    />

    <PaymentDialog
      v-if="showPaymentDialog"
      :id="model.id"
      v-model="showPaymentDialog"
      @reload="load"
    />
    <el-dialog v-model="showNoteDialog" :title="t('commerce.note')">
      <ElForm label-position="top">
        <ElFormItem>
          <ElInput v-model="note" type="textarea" :rows="5" />
        </ElFormItem>
      </ElForm>
      <template #footer>
        <DialogFooterBar @confirm="editNode" @cancel="showNoteDialog = false" />
      </template>
    </el-dialog>

    <el-dialog
      v-if="keyvalue"
      v-model="showKeyValueDialog"
      :title="keyvalue.key"
    >
      <ElForm label-position="top">
        <ElFormItem>
          <ElInput v-model="keyvalue.value" type="textarea" :rows="5" />
        </ElFormItem>
      </ElForm>
      <template #footer>
        <DialogFooterBar
          @confirm="editKeyValue"
          @cancel="showKeyValueDialog = false"
        />
      </template>
    </el-dialog>
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
      <ElButton
        v-if="model.extensionButton"
        type="primary"
        round
        @click="extensionButtonClick(model.extensionButton)"
      >
        {{ model.extensionButton.text }}
      </ElButton>
      <el-button
        v-if="!model?.paid && !model?.canceled"
        round
        type="primary"
        @click="showPaymentDialog = true"
      >
        {{ t("common.pay") }}
      </el-button>
      <el-button
        v-if="
          model?.paid &&
          !model?.canceled &&
          (!model?.delivered || model.partialDelivered)
        "
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
