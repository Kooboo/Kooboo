<script lang="ts" setup>
import type { ProductListItem } from "@/api/commerce/product";
import { getProducts } from "@/api/commerce/product";
import { ref, onMounted, computed } from "vue";
import { useI18n } from "vue-i18n";
import { useProductFields } from "../useFields";
import DynamicColumns from "@/components/dynamic-columns/index.vue";
type ProductList = ProductListItem & { selected: boolean };

const { getColumns } = useProductFields();
const columns = getColumns([
  {
    name: "featuredImage",
    attrs: {
      width: 80,
      align: "center",
    },
  },
  {
    name: "title",
  },
  {
    name: "variantsCount",
    attrs: {
      align: "center",
    },
  },
  {
    name: "inventory",
    attrs: {
      align: "center",
    },
  },
  {
    name: "active",
    attrs: {
      width: 100,
      align: "center",
    },
  },
]);
const { t } = useI18n();
const show = ref(true);

const props = defineProps<{
  excludes?: string[];
  modelValue: boolean;
}>();

const emit = defineEmits<{
  (e: "update:modelValue", value: boolean): void;
  (e: "selected", value: ProductList[]): void;
}>();

const list = ref<ProductList[]>([]);

const filteredList = computed(() => {
  let result = list.value;

  if (props.excludes) {
    result = result.filter((f) => !props.excludes!.includes(f.id));
  }

  return result;
});

async function load() {
  var products = await getProducts();
  list.value = products.map((m) => ({ ...m, selected: false }));
}

const indeterminateSelected = computed(() => {
  return (
    filteredList.value.some((s) => s.selected) &&
    filteredList.value.some((s) => !s.selected)
  );
});

const allSelected = computed(() => {
  return (
    filteredList.value.length && filteredList.value.every((s) => s.selected)
  );
});

function onChangeSelectAll(val: any) {
  filteredList.value.forEach((f) => (f.selected = val));
}

onMounted(() => {
  load();
});

function onSave() {
  emit(
    "selected",
    filteredList.value.filter((f) => f.selected)
  );
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
      <ElTable :data="filteredList" class="el-table--gray">
        <el-table-column width="60" align="center">
          <template #header>
            <ElCheckbox
              size="large"
              class="!block !h-20px"
              :indeterminate="indeterminateSelected"
              :model-value="allSelected"
              @change="onChangeSelectAll"
            />
          </template>
          <template #default="{ row }">
            <ElCheckbox v-model="row.selected" size="large" />
          </template>
        </el-table-column>
        <DynamicColumns :columns="columns" />
      </ElTable>
    </el-scrollbar>
    <template #footer>
      <DialogFooterBar
        :disabled="!filteredList.some((s) => s.selected)"
        @confirm="onSave"
        @cancel="show = false"
      />
    </template>
  </el-dialog>
</template>
