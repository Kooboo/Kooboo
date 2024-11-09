<script lang="ts" setup>
import { useRoute, useRouter } from "vue-router";
import Breadcrumb from "@/components/basic/breadcrumb.vue";
import { useI18n } from "vue-i18n";
import { useRouteSiteId } from "@/hooks/use-site-id";
import { deleteCategories, moveCategory } from "@/api/commerce/category";
import type { CategoryListItem, MoveCategory } from "@/api/commerce/category";
import { onMounted, ref, computed } from "vue";
import { showDeleteConfirm } from "@/components/basic/confirm";
import { useCommerceStore } from "@/store/commerce";
import ProductDialog from "./product-dialog.vue";
import type { Settings } from "@/api/commerce/settings";
import DynamicColumns, {
  type SummaryColumn,
} from "@/components/dynamic-columns/index.vue";
import { useCategoryLabels } from "../useLabels";
import { useMultilingualStore } from "@/store/multilingual";
import { getValueIgnoreCase, camelCase } from "@/utils/string";
import type { SortableEvent } from "sortablejs";
import type { TableRowItem } from "@/views/content/contents/types";

const { t } = useI18n();
const route = useRoute();
const router = useRouter();
const routeName = route.meta.title as string;
const commerceStore = useCommerceStore();
const selectedCategory = ref<CategoryListItem>();
const showProductDialog = ref(false);
const setting = ref<Settings>(commerceStore.settings);
commerceStore.loadSettings().then(() => {
  setting.value = commerceStore.settings;
});
const { getFieldDisplayName } = useCategoryLabels();
const multilingualStore = useMultilingualStore();

const customAttrs = {
  image: {
    width: 80,
  },
};
const commonAttrs = {
  "min-width": 180,
};

const draggable = computed(() => {
  return categoryList.value?.length > 1;
});

const showSort = computed(() => draggable.value);

async function onDelete(rows: CategoryListItem[]) {
  await showDeleteConfirm(rows.length);
  await deleteCategories(rows.map((m) => m.id));
  commerceStore.loadCategories();
}
const summaryColumns = computed<SummaryColumn[]>(() => {
  const fields = setting.value!.categoryCustomFields ?? [];
  let columns = fields.filter((it) => it.isSummaryField);
  if (!columns.length) {
    columns = fields.slice(0, 2);
  }
  return columns.map((it) => {
    const column: SummaryColumn = {
      name: camelCase(it.name),
      displayName: getFieldDisplayName(it),
      controlType: it.type,
      multipleValue: it.multiple,
      attrs: {
        ...commonAttrs,
        ...getValueIgnoreCase(customAttrs, it.name),
      },
    };
    return column;
  });
});

const categoryList = computed<CategoryListItem[]>(() => {
  return (commerceStore.categories ?? []).map((it) => {
    const customData = getValueIgnoreCase(it, "customData") ?? {};
    for (const column of summaryColumns.value) {
      if (!getValueIgnoreCase(it, column.name)) {
        const customField = getValueIgnoreCase(customData, column.name);
        if (!customField) {
          continue;
        }
        it[column.name] = getValueIgnoreCase(
          customField,
          multilingualStore.default
        );
      }
    }
    return it;
  });
});

function onProducts(item: CategoryListItem) {
  selectedCategory.value = item;
  showProductDialog.value = true;
}

async function onSort(sortedData: any[], e: SortableEvent) {
  const { newIndex, oldIndex } = e;
  if (
    newIndex === undefined ||
    oldIndex === undefined ||
    oldIndex === newIndex
  ) {
    return;
  }

  const target: TableRowItem = sortedData[newIndex];

  // Order by Descending, so prev is newIndex+1, next is newIndex-1
  const prev: TableRowItem | undefined = sortedData[newIndex + 1];
  const next: TableRowItem | undefined = sortedData[newIndex - 1];
  const req: MoveCategory = {
    source: target.id,
    prevId: prev?.id,
    nextId: next?.id,
  };
  await moveCategory(req);
  commerceStore.loadCategories();
}

async function onSortChanged() {
  commerceStore.loadCategories();
}

onMounted(async () => {
  commerceStore.loadCategories();
});
</script>

<template>
  <div class="p-24">
    <Breadcrumb :name="routeName" />
    <div class="flex items-center py-24 space-x-16">
      <el-button
        v-hasPermission="{ feature: 'productCategories', action: 'edit' }"
        round
        @click="
          router.push(
            useRouteSiteId({
              name: 'product category create',
            })
          )
        "
      >
        <div class="flex items-center">
          <el-icon class="mr-16 iconfont icon-a-addto" />
          {{ t("common.create") }}
        </div>
      </el-button>
    </div>
    <KTable
      :data="categoryList"
      show-check
      :draggable="draggable"
      row-key="id"
      @delete="onDelete"
      @sorted="onSort"
      @sort-change="onSortChanged"
      @change="() => commerceStore.loadCategories()"
    >
      <DynamicColumns :columns="summaryColumns">
        <template #title="{ row }">
          <div class="flex items-center">
            <span class="text-black dark:text-[#cfd3dc]">
              {{ row.title }}
            </span>
          </div>
        </template>
        <template #tags="{ row }">
          <div class="inline-flex flex-wrap gap-4">
            <ElTag
              v-for="(item, index) of row.tags"
              :key="index"
              round
              size="small"
              type="warning"
            >
              {{ item }}
            </ElTag>
          </div>
        </template>
        <template #productCount="{ row }">
          <ElTag
            round
            class="cursor-pointer"
            type="success"
            @click="onProducts(row)"
          >
            {{ row.productCount }}
          </ElTag>
        </template>
        <template #condition="{ row }">
          <div v-if="row.type == 'Automated'" class="text-s">
            <div v-for="(item, index) in row.condition.items" :key="index">
              <span v-if="index > 0">{{
                row.condition.isAny ? t("common.or") : t("common.and")
              }}</span>
              {{ item.option }} {{ item.method }} {{ item.value }}
            </div>
          </div>
          <span v-else class="text-s">{{ t("common.manual") }}</span>
        </template>
      </DynamicColumns>
      <el-table-column align="right" width="100" fixed="right">
        <template #default="{ row }">
          <router-link
            :to="
              useRouteSiteId({
                name: 'product category edit',
                query: {
                  id: row.id,
                },
              })
            "
            class="mx-8"
          >
            <el-tooltip placement="top" :content="t('common.edit')">
              <el-icon class="iconfont icon-a-writein hover:text-blue text-l" />
            </el-tooltip>
          </router-link>
          <IconButton
            v-if="showSort"
            icon="icon-move js-sortable cursor-move"
            :tip="t('common.move')"
            data-cy="move"
          />
        </template>
      </el-table-column>
    </KTable>
    <ProductDialog
      v-if="showProductDialog"
      v-model="showProductDialog"
      :category-id="selectedCategory!.id"
      :editable="selectedCategory!.type == 'Manual'"
      @reload="commerceStore.loadCategories()"
    />
  </div>
</template>
