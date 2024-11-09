<script lang="ts" setup>
import type { CartCalculateResult, CartLine } from "@/api/commerce/cart";
import { computed, ref, watch } from "vue";
import { useI18n } from "vue-i18n";
import { buildOptionsDisplay } from "../products-management/product-variant";
import { calculate } from "@/api/commerce/cart";
import EditableTags from "@/components/basic/editable-tags.vue";
import PropertyItem from "../components/property-item.vue";
import type { ProductVariant } from "@/api/commerce/product";
import SelectVariantDialog from "../components/select-variant-dialog.vue";
import { getDiscountCodes } from "@/api/commerce/discount";
import CurrencyAmount from "../components/currency-amount.vue";
import { useProductFields } from "../useFields";
import DynamicColumns from "@/components/dynamic-columns/index.vue";
import { ElButton, ElCheckbox, ElTableColumn, ElTooltip } from "element-plus";
import { combineUrl, openInNewTab } from "@/utils/url";
import { useSiteStore } from "@/store/site";
import { useCommerceStore } from "@/store/commerce";

const props = defineProps<{
  customerId?: string;
  shippingId?: string;
  discountCodes: string[];
  lines: CartLine[];
  extensionButton?: any;
  readonly?: boolean;
  redeemPoints: boolean;
}>();

const emit = defineEmits<{
  (e: "update:lines", value: CartLine[]): void;
  (e: "update:has-physics-products", value: boolean): void;
  (e: "update:has-digital-products", value: boolean): void;
  (e: "update:redeem-points", value: boolean): void;
}>();

const { t } = useI18n();
const showSelectVariantDialog = ref(false);
const calculateResult = ref<CartCalculateResult>();
const codes = ref<string[]>();
const { getColumns } = useProductFields();
const siteStore = useSiteStore();
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
    name: "inventory",
    attrs: {
      width: 100,
      align: "center",
    },
  },
  {
    name: "price",
    displayName: t("common.price"),
    attrs: {
      align: "center",
    },
  },
  {
    name: "quantity",
    displayName: t("common.quantity"),
    attrs: {
      width: 100,
      align: "center",
    },
  },
  {
    name: "totalQuantity",
    displayName: t("common.totalQuantity"),
    attrs: {
      width: 120,
      align: "center",
    },
  },
  {
    name: "amount",
    displayName: t("common.amount"),
    attrs: {
      align: "center",
    },
  },
  {
    name: "discounts",
    displayName: t("common.discounts"),
    attrs: {
      align: "center",
    },
  },
]);

if (!props.readonly) {
  getDiscountCodes().then((rsp) => (codes.value = rsp));
}

function onDelete(id: string, groupName: string) {
  const line = props.lines.find(
    (f) => f.variantId == id && f.groupName == groupName
  );

  if (!line) return;

  if (line.groupName && line.isMain) {
    emit(
      "update:lines",
      props.lines.filter((f) => f.groupName != groupName)
    );
  } else {
    emit(
      "update:lines",
      props.lines.filter((f) => f != line)
    );
  }
}

function onChangeQuantity(row: any, newQuantity: string) {
  const lines = [...props.lines];
  let variant = lines.find(
    (f) => f.variantId == row.variantId && f.groupName == row.groupName
  );
  if (variant) {
    variant.quantity = parseInt(newQuantity);
  }
  emit("update:lines", lines);
}

function addVariant(row: ProductVariant) {
  const lines = [...props.lines];
  let variant = lines.find((f) => f.variantId == row.id && !f.groupName);

  if (variant) {
    variant.quantity += 1;
  } else {
    variant = {
      quantity: 1,
      variantId: row.id,
    };
    lines.push(variant);
  }

  emit("update:lines", lines);
}

const data = computed(() => {
  const result: any[] = [];
  if (!calculateResult.value?.lines) return result;
  for (const line of calculateResult.value.lines) {
    if (!line.groupName) {
      result.push({ ...line, rowKey: line.variantId + line.groupName });
    } else if (line.isMain) {
      const children = calculateResult.value.lines
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
      const mainLine = calculateResult.value.lines.find(
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

watch(
  [
    () => props.customerId,
    () => props.shippingId,
    () => props.discountCodes,
    () => props.lines,
    () => props.redeemPoints,
  ],
  async () => {
    calculateResult.value = await calculate({
      customerId: props.customerId!,
      shippingId: props.shippingId!,
      discountCodes: props.discountCodes,
      lines: props.lines,
      redeemPoints: props.redeemPoints,
    });
    emit(
      "update:has-digital-products",
      calculateResult.value.lines.some((s) => s.isDigital)
    );

    emit(
      "update:has-physics-products",
      calculateResult.value.lines.some((s) => !s.isDigital)
    );
  },
  {
    deep: true,
    immediate: true,
  }
);
</script>

<template>
  <div class="space-y-12">
    <div class="flex items-center">
      <el-button
        v-if="!readonly"
        v-hasPermission="{ feature: 'carts', action: 'edit' }"
        round
        @click="showSelectVariantDialog = true"
      >
        <div class="flex items-center">
          <el-icon class="mr-16 iconfont icon-a-addto" />
          {{ t("common.addProduct") }}
        </div>
      </el-button>
      <div class="flex-1" />
      <el-button
        v-if="extensionButton"
        round
        type="primary"
        @click="extensionButtonClick(extensionButton)"
      >
        <div class="flex items-center">
          {{ extensionButton.text }}
        </div>
      </el-button>
    </div>

    <ElTable
      :data="data"
      class="el-table--gray"
      row-key="rowKey"
      default-expand-all
      :row-class-name="tableRowClassName"
    >
      <ElTableColumn v-if="data.some((s) => s.children?.length)" width="25">
        <template #default="{ row }">
          <div v-if="row.isChild" class="absolute inset-0 w-25px pl-8">
            <div class="bg-blue h-full mx-auto w-2px" />
          </div>
        </template>
      </ElTableColumn>
      <DynamicColumns :columns="columns">
        <template #title="{ row }">
          <div>
            <div>{{ row.title }}</div>
            <div class="text-s">{{ buildOptionsDisplay(row.options) }}</div>
          </div>
        </template>
        <template #price="{ row }">
          <CurrencyAmount :original="row.originalPrice" :amount="row.price" />
        </template>
        <template #amount="{ row }">
          <CurrencyAmount :amount="row.amount" :original="row.originalAmount" />
        </template>
        <template #quantity="{ row }">
          <div v-if="readonly">{{ row.quantity }}</div>
          <ElInput
            v-else
            v-model.number="row.quantity"
            class="w-50px text-center"
            @change="onChangeQuantity(row, $event)"
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

      <el-table-column v-if="!readonly" align="right" width="30">
        <template #default="{ row }">
          <div class="flex space-x-12 justify-end">
            <el-tooltip placement="top" :content="t('common.delete')">
              <el-icon
                class="iconfont icon-delete hover:text-orange text-l"
                @click="onDelete(row.variantId, row.groupName)"
              />
            </el-tooltip>
          </div>
        </template>
      </el-table-column>
    </ElTable>
    <div class="flex relative">
      <EditableTags
        :model-value="discountCodes"
        :add-label="t('commerce.discountCodes')"
        :readonly="readonly"
        :options="codes"
        @add-item="discountCodes.push($event)"
        @remove-item="discountCodes.splice($event, 1)"
        @change-item="(index, value) => discountCodes.splice(index, 1, value)"
      />
      <div class="flex-1" />
      <div class="absolute right-0 space-y-4">
        <div
          v-if="
            customerId &&
            !readonly &&
            calculateResult?.canPointsDeductionAmount! > 0
          "
          class="flex items-center gap-4 text-s justify-end"
        >
          <ElCheckbox
            :label="
              t('common.redeemPoints', {
                points: calculateResult?.canRedeemPoints,
                amount: calculateResult?.canPointsDeductionAmount?.toFixed(2),
                currency: commerceStore.settings.currencyCode,
              })
            "
            :model-value="redeemPoints"
            @update:model-value="emit('update:redeem-points', $event as any)"
          />
        </div>
        <div
          v-if="calculateResult && calculateResult?.earnPoints > 0"
          class="flex items-center gap-4 text-s justify-end"
        >
          <span>{{ t("common.orderWillEarn") }}</span>
          <span>{{ calculateResult?.earnPoints }}</span>
          <span>{{ t("commerce.points") }}</span>
        </div>
      </div>
    </div>

    <div class="text-s space-y-4">
      <PropertyItem
        v-if="calculateResult?.discountAllocations.length"
        :name="t('common.discount')"
      >
        <div class="flex gap-4">
          <ElTag
            v-for="(item, index) of calculateResult.discountAllocations"
            :key="index"
            >{{ item.title }}</ElTag
          >
        </div>
      </PropertyItem>
      <PropertyItem :name="t('commerce.subtotalAmount')">
        <CurrencyAmount
          :amount="calculateResult?.subtotalAmount"
          :original="calculateResult?.originalSubtotalAmount"
        />
      </PropertyItem>
      <PropertyItem
        v-if="calculateResult?.insuranceAmount"
        :name="t('commerce.insuranceAmount')"
      >
        <CurrencyAmount :amount="calculateResult?.insuranceAmount" />
      </PropertyItem>
      <PropertyItem
        v-if="calculateResult?.pointsDeductionAmount"
        :name="t('commerce.points')"
      >
        <CurrencyAmount :amount="-calculateResult?.pointsDeductionAmount" />
        ({{ calculateResult.redeemPoints }} {{ t("commerce.points") }})
      </PropertyItem>
      <PropertyItem
        v-if="
          calculateResult &&
          (calculateResult.lines.some((s) => !s.isDigital) ||
            calculateResult.shippingAmount > 0)
        "
        :name="t('commerce.shippingAmount')"
      >
        <div class="flex items-center gap-8">
          <CurrencyAmount :amount="calculateResult?.shippingAmount" />
          <div
            v-if="(calculateResult?.shippingAllocations?.length ?? 0) > 1"
            class="flex items-center gap-4"
          >
            (
            <div
              v-for="(item, index) of calculateResult!.shippingAllocations"
              :key="index"
              class="flex items-center gap-4"
            >
              <ElTooltip placement="top" :content="item.title">
                <CurrencyAmount :amount="item.cost" />
              </ElTooltip>
              <span
                v-if="index != calculateResult!.shippingAllocations.length - 1"
                >+</span
              >
            </div>
            )
          </div>
        </div>
      </PropertyItem>
      <PropertyItem class="text-l" :name="t('common.total')">
        <CurrencyAmount
          :amount="calculateResult?.totalAmount"
          :original="calculateResult?.originalAmount"
        />
      </PropertyItem>
    </div>
  </div>
  <SelectVariantDialog
    v-if="showSelectVariantDialog"
    v-model="showSelectVariantDialog"
    @selected="addVariant"
  />
</template>
