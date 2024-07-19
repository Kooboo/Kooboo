<script lang="ts" setup>
import type { CartCalculateResult, CartLine } from "@/api/commerce/cart";
import { ref, watch } from "vue";
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

const props = defineProps<{
  customerId?: string;
  discountCodes: string[];
  lines: CartLine[];
  readonly?: boolean;
}>();

const emit = defineEmits<{
  (e: "update:lines", value: CartLine[]): void;
}>();

const { t } = useI18n();
const showSelectVariantDialog = ref(false);
const calculateResult = ref<CartCalculateResult>();
const codes = ref<string[]>();
const { getColumns } = useProductFields();
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
    attrs: {
      align: "center",
    },
  },
  {
    name: "inventory",
    attrs: {
      width: 100,
      align: "center",
    },
  },
  {
    name: "quantity",
    displayName: t("common.quantity"),
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

function onDelete(id: string) {
  const lines = props.lines.filter((f) => f.variantId != id);
  emit("update:lines", lines);
}

function onChangeQuantity() {
  const lines = calculateResult.value!.lines.map((m) => ({
    quantity: m.quantity,
    variantId: m.variantId,
  }));
  emit("update:lines", lines);
}

function addVariant(row: ProductVariant) {
  const lines = calculateResult.value!.lines.map((m) => ({
    quantity: m.quantity,
    variantId: m.variantId,
  }));

  let variant = lines.find((f: any) => f.variantId == row.id);

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

watch(
  [() => props.customerId, () => props.discountCodes, () => props.lines],
  async () => {
    calculateResult.value = await calculate({
      customerId: props.customerId!,
      discountCodes: props.discountCodes,
      lines: props.lines,
    });
  },
  {
    deep: true,
    immediate: true,
  }
);
</script>

<template>
  <div class="space-y-12">
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
    <ElTable :data="calculateResult?.lines ?? []" class="el-table--gray">
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
        <template #quantity="{ row }">
          <div v-if="readonly">{{ row.quantity }}</div>
          <ElInput
            v-else
            v-model.number="row.quantity"
            class="w-80px text-center"
            @change="onChangeQuantity()"
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

      <el-table-column v-if="!readonly" align="right" width="80">
        <template #default="{ row }">
          <div class="flex space-x-12 justify-end">
            <el-tooltip placement="top" :content="t('common.delete')">
              <el-icon
                class="iconfont icon-delete hover:text-orange text-l"
                @click="onDelete(row.variantId)"
              />
            </el-tooltip>
          </div>
        </template>
      </el-table-column>
    </ElTable>
    <EditableTags
      :model-value="discountCodes"
      :add-label="t('commerce.discountCodes')"
      :readonly="readonly"
      :options="codes"
      @add-item="discountCodes.push($event)"
      @remove-item="discountCodes.splice($event, 1)"
      @change-item="(index, value) => discountCodes.splice(index, 1, value)"
    />

    <div class="text-s space-y-4">
      <PropertyItem
        v-if="calculateResult?.discountAllocations.length"
        :name="t('common.discounts')"
      >
        <div class="flex gap-4">
          <ElTag
            v-for="(item, index) of calculateResult.discountAllocations"
            :key="index"
            >{{ item.title }}</ElTag
          >
        </div>
      </PropertyItem>
      <PropertyItem :name="t('commerce.subtotalAmount')"
        ><CurrencyAmount :amount="calculateResult?.subtotalAmount"
      /></PropertyItem>
      <PropertyItem :name="t('commerce.shippingAmount')">
        <CurrencyAmount :amount="calculateResult?.shippingAmount"
      /></PropertyItem>
      <PropertyItem class="text-l" :name="t('commerce.totalAmount')"
        ><CurrencyAmount :amount="calculateResult?.totalAmount" />
      </PropertyItem>
    </div>
  </div>
  <SelectVariantDialog
    v-if="showSelectVariantDialog"
    v-model="showSelectVariantDialog"
    @selected="addVariant"
  />
</template>
