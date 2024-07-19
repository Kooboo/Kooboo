<script lang="ts" setup>
import { computed, ref, watch, nextTick } from "vue";
import Breadcrumb from "@/components/basic/breadcrumb.vue";
import { useCodeStore } from "@/store/code";
import KTable from "@/components/k-table";
import type { Code } from "@/api/code/types";
import RelationsTag from "@/components/relations/relations-tag.vue";
import { useTime } from "@/hooks/use-date";
import { searchDebounce } from "@/utils/url";
import SearchInput from "@/components/basic/search-input.vue";
import { useRouteSiteId } from "@/hooks/use-site-id";
import { newGuid } from "@/utils/guid";
import { Expand, Fold } from "@element-plus/icons-vue";
import { getQueryString } from "@/utils/url";
import { useRouter, useRoute } from "vue-router";

import { useI18n } from "vue-i18n";
import { showDeleteConfirm } from "@/components/basic/confirm";
import { usePreviewUrl } from "@/hooks/use-preview-url";
import SearchOnlineDialog from "@/components/search-dialog/package-search.vue";
import type SearchOnlineDialogType from "@/components/search-dialog/package-search.vue";

const { t } = useI18n();
interface TreeCode extends Code {
  children?: TreeCode[];
  isFolder: boolean;
}

const codeStore = useCodeStore();
const activeTab = ref(getQueryString("name") || "all");
const searchKey = ref("");
const router = useRouter();
const route = useRoute();
const tableRef = ref();
const selectedData = ref<any[]>([]);
const key = ref();
const { onPreview } = usePreviewUrl();

const showSearch = computed(() =>
  ["all", "codeblock"].includes(activeTab.value?.toLowerCase())
);
const searchDialog = ref<InstanceType<typeof SearchOnlineDialogType>>();
function handleSearchOnline() {
  (searchDialog.value as any)?.show();
}

// 切换Tab清空选中
watch(activeTab, () => {
  selectedData.value = [];
});

const searchEvent = () => {
  key.value = searchKey.value?.toLocaleLowerCase()?.trim();
  if (data.value.find((f) => f.isFolder)) {
    nextTick(() => {
      expandAll();
    });
  }
};
const search = searchDebounce(searchEvent, 1000);
watch(
  () => searchKey.value,
  () => {
    search();
  }
);
const data = computed(() => {
  const result: TreeCode[] = [];

  for (const i of codeStore.codes) {
    const code = { ...i, isFolder: false };

    if (activeTab.value !== "all") {
      if (i.codeType?.toLowerCase() !== activeTab.value?.toLowerCase())
        continue;
    }

    if (key.value && code.name?.toLocaleLowerCase().indexOf(key.value) === -1) {
      if (!code.url) continue;
      if (code.url.indexOf(key.value) === -1) continue;
    }
    if (!code.url) {
      result.push(code);
    } else {
      const paths = code.url
        .split("/")
        .filter((f) => !!f)
        .map((m) => m.toLocaleLowerCase());

      let currentLayer = result;
      while (paths.length >= 0) {
        if (paths.length <= 1) {
          currentLayer.push(code);
          break;
        }
        const path = paths.shift()!;
        let parent = currentLayer.find((f) => f.isFolder && f.name === path);
        if (!parent) {
          parent = {
            isFolder: true,
            id: newGuid(),
            name: path,
            children: [],
            codeType: "",
            lastModified: "",
            previewUrl: "",
            url: "",
            isEmbedded: false,
            scriptType: "",
            references: {},
          };
          currentLayer.unshift(parent!);
        }
        currentLayer = parent!.children!;
      }
    }
  }
  // 为能展开的行添加 cursor-pointer
  nextTick(() => {
    document.querySelectorAll(".el-table__expand-icon").forEach((item) => {
      item?.parentElement?.parentElement?.parentElement?.classList?.add(
        "cursor-pointer"
      );
    });
  });

  return shortTreeCode(result);
});

function shortTreeCode(treeCodes: TreeCode[]) {
  const result = treeCodes.sort((left, right) => {
    if (left.isFolder && right.isFolder)
      return left.name[0].charCodeAt(0) - right.name[0].charCodeAt(0);

    if (!left.isFolder && !right.isFolder)
      return (
        Date.parse(useTime(right.lastModified)) -
        Date.parse(useTime(left.lastModified))
      );

    return 0;
  });

  for (const i of treeCodes) {
    if (i.children) shortTreeCode(i.children);
  }

  return result;
}

const codeTypesWithAll = computed(() => ({
  all: t("common.all"),
  ...codeStore.types,
}));

const onCreate = (type: string) => {
  router.push(
    useRouteSiteId({
      name: "code-edit",
      query: {
        type: type,
      },
    })
  );
};

const onDeletes = async (items: Code[]) => {
  await showDeleteConfirm(items.length);
  await codeStore.deleteCodes(items.map((m) => m.id));
};

const onReload = async () => {
  await codeStore.loadCodes();
};

onReload();
const expandAll = (root = data.value) => {
  root.forEach((row) => {
    tableRef.value!.table?.toggleRowExpansion?.(row, true);
    row.children && expandAll(row.children);
  });
};

const foldAll = (root = data.value) => {
  root.forEach((row) => {
    tableRef.value!.table?.toggleRowExpansion?.(row, false);
    row.children && foldAll(row.children);
  });
};

const handelRowClick = (row: any) => {
  if (row?.isFolder === true) {
    tableRef.value!.table?.toggleRowExpansion?.(row);
  }
};

const isNested = computed(() => data.value.some((m) => m.isFolder));
const pushActiveName = () => {
  router.push({
    name: route.name?.toString(),
    query: {
      ...route.query,
      name: activeTab.value,
    },
  });
};
</script>

<template>
  <div>
    <div class="flex items-center p-24 relative">
      <Breadcrumb :name="t('common.code')" />
      <SearchInput
        v-model="searchKey"
        :placeholder="t('common.enterKeywords')"
        class="absolute right-24 w-238px"
        data-cy="search"
      />
    </div>

    <el-tabs v-model="activeTab" @tab-change="pushActiveName">
      <el-tab-pane
        v-for="(value, typeKey) in codeTypesWithAll"
        :key="typeKey"
        :label="value"
        :name="typeKey"
      >
        <div class="flex justify-between pb-12 space-x-16">
          <div class="flex items-center space-x-16">
            <el-dropdown
              v-if="activeTab === 'all'"
              trigger="click"
              @command="onCreate"
            >
              <el-button
                v-hasPermission="{
                  feature: 'code',
                  action: 'edit',
                }"
                round
                class="shadow-s-10 border-none"
                data-cy="create"
              >
                <div class="flex items-center">
                  <span>{{ t("common.create") }}</span>
                  <el-icon
                    class="iconfont icon-pull-down text-12px ml-8 !mr-0"
                  />
                </div>
              </el-button>
              <template #dropdown>
                <el-dropdown-menu>
                  <el-dropdown-item
                    v-for="(v, k) of codeStore.types"
                    :key="k"
                    :command="k"
                    data-cy="code-type-opt"
                  >
                    <span>{{ v }}</span>
                  </el-dropdown-item>
                </el-dropdown-menu>
              </template>
            </el-dropdown>

            <el-button
              v-else
              v-hasPermission="{
                feature: 'code',
                action: 'edit',
              }"
              round
              :data-cy="'create-' + `${activeTab}`"
              @click="onCreate(activeTab)"
            >
              <div class="flex items-center">
                <el-icon class="iconfont icon-a-addto" />
                {{ t("code.create", { type: codeStore.types[activeTab] }) }}
              </div>
            </el-button>

            <el-button
              v-if="showSearch"
              v-hasPermission="{
                feature: 'code',
                action: 'edit',
              }"
              round
              data-cy="search"
              @click="handleSearchOnline"
            >
              <el-icon class="iconfont icon-search" />
              {{ t("common.search") }}
            </el-button>
          </div>
          <div>
            <template v-if="isNested">
              <el-button
                circle
                :icon="Fold"
                :title="t('common.foldAll')"
                data-cy="fold-all"
                @click="foldAll()"
              />
              <el-button
                circle
                :icon="Expand"
                :title="t('common.expandAll')"
                data-cy="expand-all"
                @click="expandAll()"
              />
            </template>
          </div>
        </div>
        <KTable
          v-if="activeTab === typeKey"
          :ref="(table: any) => (tableRef = table)"
          v-model:selectedData="selectedData"
          :data="data"
          show-check
          custom-check
          row-key="id"
          sort="name"
          order="ascending"
          :permission="{
            feature: 'code',
            action: 'delete',
          }"
          :is-multi-tree-table="true"
          @row-click="handelRowClick"
          @delete="onDeletes"
        >
          <el-table-column
            :label="t('common.name')"
            class-name="expanded-column"
          >
            <template #default="{ row }">
              <span
                v-if="!row.isFolder"
                class="flex-1 overflow-hidden flex items-center ml-12"
              >
                <el-checkbox
                  size="large"
                  :model-value="selectedData.some((f) => f === row)"
                  data-cy="checkbox-label"
                  @change="
                    () => {
                      if (selectedData.includes(row)) {
                        selectedData = selectedData.filter((f) => f !== row);
                      } else {
                        selectedData.push(row);
                      }
                    }
                  "
                />
                <div class="ml-14px text-blue flex-1 ellipsis">
                  <router-link
                    :to="
                      useRouteSiteId({
                        name: 'code-edit',
                        query: {
                          type: row.codeType,
                          id: row.id,
                        },
                      })
                    "
                    data-cy="name"
                  >
                    <span :title="row.name" class="cursor-pointer">
                      {{ row.name }}
                    </span>
                  </router-link>
                </div>
              </span>
              <span v-else>{{ row.name }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="t('common.codeType')" width="120px">
            <template #default="{ row }">
              <el-tag
                v-if="!row.isFolder"
                size="small"
                class="rounded-full"
                data-cy="code-type"
              >
                {{ codeStore.types[row.codeType] || row.codeType }}
              </el-tag>
            </template>
          </el-table-column>

          <el-table-column :label="t('common.scriptType')" width="120px">
            <template #default="{ row }">
              <el-tag
                v-if="
                  !row.isFolder &&
                  !(row.codeType == 'PageScript' && row.isEmbedded) &&
                  row.codeType != 'CodeBlock'
                "
                size="small"
                class="rounded-full"
                data-cy="script-type"
              >
                {{ row.scriptType }}
              </el-tag>
            </template>
          </el-table-column>
          <el-table-column :label="t('common.usedBy')" width="120px">
            <template #default="{ row }">
              <RelationsTag
                v-if="!row.isFolder"
                :id="row.id"
                :relations="row.references"
                type="code"
              />
            </template>
          </el-table-column>
          <el-table-column :label="t('common.preview')">
            <template #default="{ row }">
              <span
                v-if="!row.isFolder"
                class="text-blue cursor-pointer ellipsis"
                data-cy="preview"
                @click="onPreview(row.previewUrl)"
              >
                {{ row.url }}
              </span>
            </template>
          </el-table-column>
          <el-table-column
            :label="t('common.lastModified')"
            width="180"
            prop="lastModified"
          >
            <template #default="{ row }">
              <template v-if="!row.isFolder">
                {{ useTime(row.lastModified) }}
              </template>
            </template>
          </el-table-column>
          <el-table-column width="80px" align="right">
            <template #default="{ row }">
              <IconButton
                v-if="!row.isFolder"
                class="inline-flex"
                icon="icon-time"
                :permission="{
                  feature: 'site',
                  action: 'log',
                }"
                :tip="t('common.version')"
                data-cy="versions"
                @click="$router.goLogVersions(row.keyHash, row.storeNameHash)"
              />
            </template>
          </el-table-column>
        </KTable>
      </el-tab-pane>
    </el-tabs>
    <SearchOnlineDialog
      v-if="showSearch"
      ref="searchDialog"
      module="code"
      @reload="onReload"
    />
  </div>
</template>

<style scoped>
/* 展开column的单元格改弹性布局 */
:deep(.expanded-column .cell) {
  @apply flex items-center;
}

:deep(.el-table__placeholder) {
  @apply hidden;
}
</style>
