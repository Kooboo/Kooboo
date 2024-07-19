<script lang="ts" setup>
import KTable from "@/components/k-table";
import { computed, ref, watch, nextTick } from "vue";
import type { Label } from "@/api/content/label";
import { exportLabel } from "@/api/content/label";
import { getList, deletes } from "@/api/content/label";
import { useTime } from "@/hooks/use-date";
import EditDialog from "./edit-dialog.vue";
import ScanResultDialog from "./scan-result-dialog.vue";
import { useMultilingualStore } from "@/store/multilingual";
import { cloneDeep } from "lodash-es";
import Breadcrumb from "@/components/basic/breadcrumb.vue";
import IconButton from "@/components/basic/icon-button.vue";
import SearchInput from "@/components/basic/search-input.vue";
import { showDeleteConfirm } from "@/components/basic/confirm";
import { Expand, Fold } from "@element-plus/icons-vue";

import { useI18n } from "vue-i18n";
import { searchDebounce } from "@/utils/url";
import ImportDialog from "./import-dialog.vue";
import type { KeyValue } from "@/global/types";

const { t } = useI18n();
const list = ref<Label[]>([]);
const keywordsTemp = ref("");
const keywords = ref("");
const visibleEditDialog = ref(false);
const scanResultDialog = ref(false);
const showImportDialog = ref(false);
const currentEdit = ref<Label>({} as Label);

const type = ref();
const selectedData = ref<any[]>([]);
const tableRef = ref();
const tableView = ref("default");
// 用于存储当前已打开的文件夹
const expandLabelList = ref<any[]>([]);

// 存储所有树状的数据，展开全部的时候存储起来
const expandAllLabelList = ref<any[]>([]);
const initExpandAllLabelList = () => {
  expandAllLabelList.value = [];
};

const expandLabel = () => {
  nextTick(() => {
    expandLabelList.value.forEach((row) => {
      tableRef.value!.table?.toggleRowExpansion?.(row, true);
    });
  });
};
const types: KeyValue[] = [
  {
    key: "default",
    value: t("common.defaultList"),
  },
  {
    key: "category",
    value: t("common.categoryList"),
  },
];

const usedByTypes: Record<string, string> = {
  Page: t("common.page"),
  View: t("common.view"),
  Layout: t("common.layout"),
};

const sortByName = (a: any, b: any) => {
  const nameA = a.name.toUpperCase();
  const nameB = b.name.toUpperCase();
  if (nameA < nameB) {
    return -1;
  }
  if (nameA > nameB) {
    return 1;
  }
  return 0;
};

async function load() {
  const records = await getList();
  list.value = records.sort((a, b) =>
    a.lastModified! > b.lastModified! ? -1 : 1
  );
}

const addPointerStyle = () => {
  nextTick(() => {
    document.querySelectorAll(".el-table__expand-icon").forEach((item) => {
      item?.parentElement?.parentElement?.parentElement?.classList?.add(
        "cursor-pointer"
      );
    });
  });
};

// 默认列表
const searchList = computed(() => {
  let filterList = list.value;
  if (keywords.value) {
    const lowerKeywords = keywords.value.trim().toLowerCase();
    filterList = filterList.filter((label: any) => {
      if (label.name!.toLowerCase().includes(lowerKeywords)) {
        return true;
      }
      for (let key in label.values) {
        if (label.values[key]?.toLowerCase().includes(lowerKeywords)) {
          return true;
        }
      }
      return false;
    });
  }
  return filterList;
});

// 分类列表
const categoryLabelList = computed(() => {
  // 初始化所有选中数据的数据
  initExpandAllLabelList();
  let labelList = [] as any[];
  let folderList = [] as any[];
  let list = [] as any[];

  let finalLabelList = [] as any[];
  cloneDeep(searchList.value).forEach((item: any) => {
    let o = item;
    o["category_label_id"] = item.name + "-" + item.id;
    if (item.relationDetails.length) {
      item.relationDetails.forEach((subItem: any) => {
        const cur = folderList.find((i) => i.id === subItem.id);
        if (cur) {
          cur.children.push({
            ...o,
            category_label_id: subItem.name + o.id,
          });
        } else {
          const obj = {
            ...subItem,
            children: [
              {
                ...o,
                category_label_id: subItem.name + o.id,
              },
            ],
          };
          folderList.push(obj);
          expandAllLabelList.value.push(obj);
        }
      });
    } else {
      list.push(o);
    }
  });

  // 对展开行数据进行排序
  folderList.sort(sortByName);
  // 对展开行的子数据进行排序
  folderList.forEach((f) => {
    f.children.sort(sortByName);
  });

  labelList = folderList.concat(list);
  labelList.forEach((item) => {
    let o = item;
    o["category_label_id"] = item.name + "-" + item.id;
    if (item.type) {
      const cur = finalLabelList.find((i) => i.type === item.type);
      if (cur) {
        cur.children.push(o);
      } else {
        let obj = {
          ...item,
          name: usedByTypes[item.type],
          id: item.type,
          category_label_id: item.type,
          children: [o],
        };
        finalLabelList.push(obj);
        expandAllLabelList.value.push(obj);
      }
    } else {
      finalLabelList.push(item);
    }
  });
  // 一级展开行排序
  finalLabelList.sort((a, b) => {
    const nameA = a.type;
    const nameB = b.type;
    if (nameA < nameB) {
      return -1;
    }
    if (nameA > nameB) {
      return 1;
    }
    return 0;
  });
  return finalLabelList;
});

const expandAll = (root = categoryLabelList.value) => {
  expandLabelList.value = expandAllLabelList.value;
  root.forEach((row) => {
    tableRef.value!.table?.toggleRowExpansion?.(row, true);
    row.children && expandAll(row.children);
  });
};

const foldAll = (root = categoryLabelList.value) => {
  root.forEach((row) => {
    tableRef.value!.table?.toggleRowExpansion?.(row, false);
    row.children && foldAll(row.children);
    expandLabelList.value = [];
  });
};

const handelRowClick = (row: any) => {
  if (row?.children) {
    tableRef.value!.table?.toggleRowExpansion?.(row);
    if (expandLabelList.value.find((f) => f.id === row.id)) {
      expandLabelList.value = expandLabelList.value.filter(
        (f) => f.id !== row.id
      );
    } else {
      expandLabelList.value.push(row);
    }
  }
};

async function onDelete(rows: Label[]) {
  await showDeleteConfirm(rows.length);
  const ids = rows.map((item) => item.id);
  await deletes({ ids });
  load();
}

const multilingualStore = useMultilingualStore();
function getDefaultValue(row: Label) {
  return row.values[multilingualStore.default] || "";
}

async function edit(row: Label) {
  currentEdit.value = cloneDeep(row);
  visibleEditDialog.value = true;
}

async function newLabel() {
  currentEdit.value = {
    id: "",
    values: Object.fromEntries(
      multilingualStore.selected.map((item) => [item, ""])
    ),
  };
  visibleEditDialog.value = true;
}

function searchEvent() {
  keywords.value = keywordsTemp.value;
  if (keywords.value && tableView.value === "category") {
    nextTick(() => {
      expandAll();
    });
  }
}

function onDialogClose(reload: boolean) {
  if (reload) load();
}
const search = searchDebounce(searchEvent, 1000);
const scan = () => {
  type.value = "scan";
  scanResultDialog.value = true;
};

const scanI18n = async () => {
  type.value = "scanI18n";
  scanResultDialog.value = true;
};

const onExportLabel = async () => {
  await exportLabel();
};

const checkBoxChange = (row: any) => {
  let label = JSON.parse(JSON.stringify(row));
  delete label.category_label_id;
  if (selectedData.value.find((item) => item.id === label.id)) {
    selectedData.value = selectedData.value.filter((f) => f.id !== label.id);
  } else {
    if (label.relationDetails.length && tableView.value === "category") {
      for (let i = 0; i < label.relationDetails.length; i++) {
        selectedData.value.push(label);
      }
    } else {
      selectedData.value.push(label);
    }
  }
};

const isChecked = computed(() => (row: any) => {
  let label = JSON.parse(JSON.stringify(row));
  delete label.category_label_id;
  return selectedData.value.some((f) => f.id === label.id);
});

watch(
  () => keywordsTemp.value,
  () => {
    search();
  }
);

watch(
  () => [tableView.value, categoryLabelList.value],
  () => {
    if (tableView.value === "category" && expandLabelList.value.length) {
      expandLabel();
    }
    if (tableView.value === "category") addPointerStyle();
  }
);

load();
</script>

<template>
  <div class="p-24">
    <Breadcrumb :name="t('common.labels')" />
    <div class="flex items-center py-24 justify-between">
      <div class="flex items-center">
        <el-button round data-cy="scan" @click="newLabel">
          {{ t("common.new") }}
        </el-button>
        <el-button round data-cy="scan" @click="scan()">
          {{ t("common.scanlabel") }}
        </el-button>
        <el-button round data-cy="scan" @click="scanI18n()">
          {{ t("common.ScanI18N") }}
        </el-button>
        <el-button round data-cy="scan" @click="onExportLabel()">
          <el-icon class="iconfont icon-share" />
          {{ t("common.export") }}
        </el-button>
        <el-button round data-cy="scan" @click="showImportDialog = true">
          <el-icon class="mr-16 iconfont icon-a-Pullin" />
          {{ t("common.import") }}
        </el-button>
      </div>

      <SearchInput
        v-model="keywordsTemp"
        class="w-238px"
        :placeholder="t('common.searchLabels')"
        clearable
        data-cy="search"
      />
    </div>
    <KTable
      ref="tableRef"
      v-model:selectedData="selectedData"
      :data="tableView === 'default' ? searchList : categoryLabelList"
      show-check
      :permission="{
        feature: 'label',
        action: 'delete',
      }"
      :row-key="tableView === 'default' ? 'id' : 'category_label_id'"
      custom-check
      :is-multi-tree-table="true"
      @delete="onDelete"
      @row-click="handelRowClick"
    >
      <template #leftBar>
        <div class="flex item-center space-x-16">
          <el-radio-group v-model="tableView" class="el-radio-group--rounded">
            <el-radio-button
              v-for="item of types"
              :key="item.key"
              :label="item.key"
              :data-cy="item.key"
              >{{ item.value }}</el-radio-button
            >
          </el-radio-group>
          <template v-if="tableView === 'category'">
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
      </template>
      <el-table-column
        :label="t('common.name')"
        prop="name"
        class-name="expanded-column"
      >
        <template #default="{ row }">
          <span v-if="row.children">
            {{ row.name }}
          </span>
          <span v-else class="flex items-center space-x-12">
            <el-checkbox
              size="large"
              :model-value="isChecked(row)"
              data-cy="checkbox-label"
              @change="checkBoxChange(row)"
            />
            <span class="ellipsis" :title="row.name" data-cy="name">
              {{ row.name }}
            </span>
          </span>
        </template>
      </el-table-column>
      <el-table-column :label="t('common.value')">
        <template #default="{ row }">
          <div
            v-if="!row.children"
            class="ellipsis"
            :title="getDefaultValue(row)"
            data-cy="value"
          >
            {{ getDefaultValue(row) }}
          </div>
        </template>
      </el-table-column>
      <el-table-column
        v-if="tableView === 'default'"
        :label="t('common.usedBy')"
        width="200"
      >
        <template #default="{ row }">
          <RelationsTag :id="row.id" :relations="row.relations" type="label" />
        </template>
      </el-table-column>
      <el-table-column :label="t('common.lastModified')" width="180">
        <template #default="{ row }"
          ><span v-if="!row.children">{{
            useTime(row.lastModified)
          }}</span></template
        >
      </el-table-column>
      <el-table-column width="100">
        <template #default="{ row }">
          <div v-if="!row.children" class="flex items-center justify-end">
            <IconButton
              class="inline-flex"
              icon="icon-a-writein"
              :tip="t('common.edit')"
              data-cy="edit"
              @click="edit(row)"
            />
            <IconButton
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
          </div>
        </template>
      </el-table-column>
    </KTable>
    <EditDialog
      v-if="visibleEditDialog"
      v-model="visibleEditDialog"
      :current="currentEdit"
      @success="onDialogClose"
    />
    <ScanResultDialog
      v-if="scanResultDialog"
      v-model="scanResultDialog"
      :type="type"
      @load="load"
    />
    <ImportDialog
      v-if="showImportDialog"
      v-model="showImportDialog"
      @reload="load"
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

/* 隐藏默认列表出现的横向滚动条(分类列表切到默认列表时由于多出一列会出现横向滚动条) */
:deep(.el-scrollbar__bar.is-horizontal) {
  @apply hidden;
}
</style>
