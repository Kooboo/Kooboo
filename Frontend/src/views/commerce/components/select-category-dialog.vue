<script lang="ts" setup>
import type { CategoryListItem } from "@/api/commerce/category";
import { useCommerceStore } from "@/store/commerce";
import { ref, onMounted, computed } from "vue";
import { useI18n } from "vue-i18n";
import DynamicColumns from "@/components/dynamic-columns/index.vue";
import { useCategoryFields } from "../useFields";
type CategoryList = CategoryListItem & { selected: boolean };
const { getColumns } = useCategoryFields();
const { t } = useI18n();
const show = ref(true);
const commerceStore = useCommerceStore();
const keyword = ref("");

const props = defineProps<{
  excludes?: string[];
  modelValue: boolean;
}>();

const emit = defineEmits<{
  (e: "update:modelValue", value: boolean): void;
  (e: "selected", value: CategoryList[]): void;
}>();

const list = ref<CategoryList[]>([]);
const columns = getColumns([
  {
    name: "image",
    attrs: {
      width: 80,
    },
  },
  {
    name: "title",
  },
  {
    name: "type",
    displayName: t("common.type"),
  },
  {
    name: "active",
    attrs: {
      align: "center",
      width: 100,
    },
  },
]);

const filteredList = computed(() => {
  let _keyword = keyword.value?.toLowerCase()?.trim();
  let result = list.value.filter(
    (f) => f.type == "Manual" && f.title?.toLowerCase()?.indexOf(_keyword) > -1
  );

  if (props.excludes) {
    result = result.filter((f) => !props.excludes!.includes(f.id));
  }

  return result;
});

const manualList = computed(() => {
  return filteredList.value.filter((f) => f.type == "Manual");
});

async function load() {
  await commerceStore.loadCategories();
  list.value = commerceStore.categories.map((m) => ({ ...m, selected: false }));
}

const indeterminateSelected = computed(() => {
  return (
    manualList.value.some((s) => s.selected) &&
    manualList.value.some((s) => !s.selected)
  );
});

const allSelected = computed(() => {
  return manualList.value.length && manualList.value.every((s) => s.selected);
});

function onChangeSelectAll(val: any) {
  manualList.value.forEach((f) => (f.selected = val));
}

onMounted(() => {
  load();
});

function onSave() {
  emit(
    "selected",
    manualList.value.filter((f) => f.selected)
  );
  show.value = false;
}
</script>

<template>
  <el-dialog
    :model-value="show"
    width="800px"
    :title="t('commerce.selectCategory')"
    :close-on-click-modal="false"
    @closed="emit('update:modelValue', false)"
  >
    <div class="flex items-center pb-12 space-x-16">
      <div class="flex-1" />
      <SearchInput
        v-model="keyword"
        class="w-248px"
        @keydown.enter.prevent="load"
      />
    </div>
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
            <ElCheckbox
              v-model="row.selected"
              :disabled="!manualList.find((f) => f.id == row.id)"
              size="large"
            />
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
