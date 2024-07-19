<script lang="ts" setup>
import type { ProductListItem, ProductVariant } from "@/api/commerce/product";
import { getProducts, getVariants } from "@/api/commerce/product";
import { ref, onMounted } from "vue";
import { useI18n } from "vue-i18n";
import { buildOptionsDisplay } from "../products-management/product-variant";
import { useProductFields } from "../useFields";
import DynamicColumns from "@/components/dynamic-columns/index.vue";

const { t } = useI18n();
const show = ref(true);
const showVariantsDialog = ref(false);
const { getColumns } = useProductFields();

defineProps<{
  excludes?: string[];
  modelValue: boolean;
}>();

const emit = defineEmits<{
  (e: "update:modelValue", value: boolean): void;
  (e: "selected", value: ProductVariant): void;
}>();
const list = ref<ProductListItem[]>([]);
const variants = ref<ProductVariant[]>([]);
const columns = getColumns([
  {
    name: "featuredImage",
  },
  {
    name: "title",
  },
  {
    name: "variantsCount",
  },
  {
    name: "inventory",
  },
  {
    name: "active",
  },
]);
const variantColumns = getColumns([
  {
    name: "featuredImage",
    prop: "image",
  },
  {
    name: "selectedOptions",
    displayName: t("common.options"),
  },
  {
    name: "inventory",
  },
  {
    name: "active",
  },
]);

async function load() {
  var products = await getProducts();
  list.value = products.map((m) => ({ ...m, selected: false }));
}

onMounted(() => {
  load();
});

async function onRowClick(row: ProductListItem) {
  variants.value = await getVariants(row.id);
  if (variants.value.length == 1) {
    emit("selected", variants.value[0]);
    show.value = false;
  } else {
    showVariantsDialog.value = true;
  }
}

function onVariantRowClick(row: ProductVariant) {
  emit("selected", row);
  showVariantsDialog.value = false;
  show.value = false;
}
</script>

<template>
  <el-dialog
    :model-value="show"
    width="800px"
    :title="t('common.selectProduct')"
    :close-on-click-modal="false"
    @closed="emit('update:modelValue', false)"
  >
    <el-scrollbar max-height="400px">
      <ElTable :data="list" class="el-table--gray" @row-click="onRowClick">
        <DynamicColumns :columns="columns" />
      </ElTable>
    </el-scrollbar>
  </el-dialog>
  <ElDialog
    v-model="showVariantsDialog"
    width="600px"
    :title="t('commerce.selectVariant')"
  >
    <el-scrollbar max-height="400px">
      <ElTable
        :data="variants"
        class="el-table--gray"
        @row-click="onVariantRowClick"
      >
        <DynamicColumns :columns="variantColumns">
          <template #selectedOptions="{ row }">
            <div class="flex items-center">
              <span class="text-black dark:text-[#cfd3dc]">
                {{ buildOptionsDisplay(row.selectedOptions, true) }}
              </span>
            </div>
          </template>
        </DynamicColumns>
      </ElTable>
    </el-scrollbar>
  </ElDialog>
</template>
