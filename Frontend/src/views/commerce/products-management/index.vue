<script lang="ts" setup>
import { useRoute, useRouter } from "vue-router";
import Breadcrumb from "@/components/basic/breadcrumb.vue";
import { useI18n } from "vue-i18n";
import { useRouteSiteId } from "@/hooks/use-site-id";
import { getPagingProducts, deleteProducts } from "@/api/commerce/product";
import type {
  ProductListItem,
  ProductPagingResult,
} from "@/api/commerce/product";
import { onMounted, ref, computed } from "vue";
import { showDeleteConfirm } from "@/components/basic/confirm";
import { useCommerceStore } from "@/store/commerce";
import { emptyGuid } from "@/utils/guid";
import type { PagingParams } from "@/api/commerce/common";
import PropertyItem from "../components/property-item.vue";
import type { Settings } from "@/api/commerce/settings";
import { useProductLabels } from "../useLabels";
import DynamicColumns, {
  type SummaryColumn,
} from "@/components/dynamic-columns/index.vue";
import { ignoreCaseEqual, getValueIgnoreCase, camelCase } from "@/utils/string";
import { useProductFields } from "../useFields";
import { getQueryString } from "@/utils/url";
import { tryParseInt } from "@/utils/lang";

const { fields } = useProductFields();
const { t } = useI18n();
const route = useRoute();
const router = useRouter();
const routeName = route.meta.title as string;
const commerceStore = useCommerceStore();
const setting = ref<Settings>(commerceStore.settings);
const result = ref<ProductPagingResult>();
commerceStore.loadTypes();
commerceStore.loadSettings().then(() => {
  setting.value = commerceStore.settings;
});
const { getFieldDisplayName } = useProductLabels();

const inventoryPlaceholder = computed(() => {
  const field = fields.value?.find((it) =>
    ignoreCaseEqual(it.name, "inventory")
  );
  if (!field) {
    return t("common.inventory");
  }
  return getFieldDisplayName(field);
});

const commonAttrs = {
  "min-width": 180,
};
const customAttrs = {
  featuredImage: {
    width: 80,
    align: "center",
  },
  variantsCount: {
    align: "center",
  },
  inventory: {
    align: "center",
  },
  tags: {
    align: "center",
  },
  active: {
    align: "center",
  },
  categories: {
    "min-width": 180,
  },
};

const productColumns = computed<SummaryColumn[]>(() => {
  let summaryColumns = fields.value.filter((it) => it.isSummaryField);
  if (!summaryColumns.length) {
    summaryColumns = fields.value.slice(0, 2);
  }
  return summaryColumns.map((it) => {
    const column: SummaryColumn = {
      name: camelCase(it.name),
      displayName: getFieldDisplayName(it),
      controlType: it.type,
      multipleValue: it.multiple,
      attrs:
        summaryColumns.length <= 1
          ? {
              align: "left",
            }
          : Object.assign(
              {},
              commonAttrs,
              getValueIgnoreCase(customAttrs, it.name) || {}
            ),
    };
    return column;
  });
});
const productList = computed<ProductListItem[]>(() => {
  return (result.value?.list ?? []).map((it) => {
    const customData = getValueIgnoreCase(it, "customData") ?? {};
    for (const column of productColumns.value) {
      if (!getValueIgnoreCase(it, column.name)) {
        const customField = getValueIgnoreCase(customData, column.name);
        if (!customField) {
          continue;
        }
        it[column.name] = getValueIgnoreCase(customField, "en");
      }
    }
    return it;
  });
});

const queryParams = ref<
  PagingParams & {
    keyword?: string;
    inStock?: boolean;
    active?: string;
  }
>({
  pageIndex: tryParseInt(getQueryString("pageIndex")) || 1,
  pageSize: 30,
  keyword: "",
  inStock: undefined,
  active: undefined,
});

async function load(pageIndex?: number) {
  if (pageIndex) queryParams.value.pageIndex = pageIndex;
  result.value = await getPagingProducts(queryParams.value);
  if (pageIndex) {
    router.push({
      ...route,
      query: {
        ...route.query,
        pageIndex: pageIndex,
      },
    });
  }
}

async function onDelete(rows: ProductListItem[]) {
  await showDeleteConfirm(rows.length);
  await deleteProducts(rows.map((m) => m.id));
  load(queryParams.value.pageIndex);
}

function onCreateProduct(command: string) {
  router.push(
    useRouteSiteId({
      name: "product management create",
      query: {
        typeId: command,
      },
    })
  );
}

onMounted(async () => {
  load();
});
</script>

<template>
  <div class="p-24">
    <Breadcrumb :name="routeName" />
    <div v-if="result" class="flex items-center space-x-16 mt-8 text-m">
      <PropertyItem :name="t('commerce.outOfStock')">{{
        result.stats.outOfStock
      }}</PropertyItem>
      <div class="flex-1" />
      <PropertyItem :name="t('commerce.totalProducts')">{{
        result.stats.product
      }}</PropertyItem>
      <PropertyItem :name="t('commerce.totalInventory')">{{
        result.stats.inventory
      }}</PropertyItem>
    </div>

    <div class="flex items-center py-12">
      <el-dropdown trigger="click" @command="onCreateProduct">
        <el-button
          v-hasPermission="{
            feature: 'productManagement',
            action: 'edit',
          }"
          round
        >
          <span>{{ t("commerce.createProduct") }}</span>
          <el-icon class="iconfont icon-pull-down text-12px ml-8 !mr-0" />
        </el-button>
        <template #dropdown>
          <el-dropdown-menu>
            <el-dropdown-item :command="emptyGuid">
              <div class="flex gap-8">
                <span>{{ t("common.create") }}</span>
                <ElTag round type="info">{{
                  t("commerce.blankProduct")
                }}</ElTag>
              </div>
            </el-dropdown-item>
            <el-dropdown-item
              v-for="item of commerceStore.types"
              :key="item.id"
              :command="item.id"
            >
              <div class="flex gap-8">
                <span>{{ t("common.create") }}</span>
                <ElTag round type="info">{{ item.name }}</ElTag>
              </div>
            </el-dropdown-item>
          </el-dropdown-menu>
        </template>
      </el-dropdown>
      <div class="flex-1" />

      <div class="flex items-center py-12 space-x-16">
        <el-select
          v-model="queryParams.active"
          :placeholder="t('common.status')"
          class="w-180px"
          clearable
          @clear="queryParams.active = undefined"
        >
          <el-option :label="t('commerce.active')" :value="true" />
          <el-option :label="t('commerce.inactive')" :value="false" />
        </el-select>

        <el-select
          v-model="queryParams.inStock"
          :placeholder="inventoryPlaceholder"
          class="w-180px"
          clearable
          @clear="queryParams.inStock = undefined"
        >
          <el-option :label="t('commerce.available')" :value="true" />
          <el-option :label="t('commerce.outOfStock')" :value="false" />
        </el-select>
        <ElInput
          v-model="queryParams.keyword"
          class="w-200px"
          :placeholder="t('common.keyword')"
          clearable
        />
        <ElButton round type="primary" @click="load(1)">{{
          t("common.search")
        }}</ElButton>
      </div>
    </div>
    <KTable
      v-if="result"
      :data="productList"
      :pagination="{
        currentPage: result.pageIndex,
        totalCount: result.count,
        pageSize: result.pageSize,
      }"
      show-check
      @change="load"
      @delete="onDelete"
    >
      <DynamicColumns :columns="productColumns">
        <template #title="{ row }">
          <div class="flex flex-col">
            <span class="text-black dark:text-[#cfd3dc]">
              {{ row.title }}
            </span>
            <div>
              <ElTag v-if="row.isDigital" round size="small">{{
                t("common.digitalProduct")
              }}</ElTag>
            </div>
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
        <template #categories="{ row }">
          <div class="inline-flex flex-wrap gap-4">
            <ElTag
              v-for="(item, index) of row.categories"
              :key="index"
              round
              size="small"
            >
              {{ item }}
            </ElTag>
          </div>
        </template>
      </DynamicColumns>

      <el-table-column align="right" width="60" fixed="right">
        <template #default="{ row }">
          <router-link
            :to="
              useRouteSiteId({
                name: 'product management edit',
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
        </template>
      </el-table-column>
    </KTable>
  </div>
</template>
