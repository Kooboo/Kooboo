<template>
  <div class="database-columns p-24 pb-150px">
    <div class="text-2l mb-12px font-bold dark:text-[#fffa]">
      {{ t("common.dataTable") }}: {{ table }}
    </div>
    <div class="flex items-center py-24 space-x-16">
      <el-button
        v-hasPermission="{
          feature: 'database',
          action: 'edit',
        }"
        round
        data-cy="new-column"
        @click="onShowEditColumnModal()"
      >
        <el-icon class="iconfont icon-a-addto" />
        {{ t("common.newColumn") }}
      </el-button>
    </div>
    <KTable :data="columns">
      <el-table-column :label="t('common.columnName')">
        <template #default="{ row }">
          <span data-cy="column-name">{{ row.name }}</span>
        </template>
      </el-table-column>
      <el-table-column :label="t('common.controlType')" width="200">
        <template #default="{ row }">
          <span data-cy="control-type">
            {{ getControlType(row.controlType)?.displayName }}
          </span>
        </template>
      </el-table-column>
      <template v-if="dbType === 'Database'">
        <el-table-column
          :label="t('common.primaryKey')"
          width="150"
          align="center"
        >
          <template #default="{ row }">
            <span
              :class="row.isPrimaryKey ? 'text-green' : 'text-999'"
              data-cy="primary-key"
              >{{ row.isPrimaryKey ? t("common.yes") : t("common.no") }}</span
            >
          </template>
        </el-table-column>
        <el-table-column :label="t('common.unique')" width="150" align="center">
          <template #default="{ row }">
            <span
              :class="row.isUnique ? 'text-green' : 'text-999'"
              data-cy="unique"
              >{{ row.isUnique ? t("common.yes") : t("common.no") }}</span
            >
          </template>
        </el-table-column>
      </template>
      <el-table-column :label="t('common.index')" width="150" align="center">
        <template #default="{ row }">
          <span
            :class="row.isIndex ? 'text-green' : 'text-999'"
            data-cy="index"
            >{{ row.isIndex ? t("common.yes") : t("common.no") }}</span
          >
        </template>
      </el-table-column>
      <el-table-column width="90" align="center">
        <template #default="{ row }">
          <IconButton
            icon="icon-a-writein"
            :tip="t('common.edit')"
            data-cy="edit"
            @click="onShowEditColumnModal(row)"
          />
          <IconButton
            :permission="{
              feature: 'database',
              action: 'edit',
            }"
            class="text-orange hover:text-orange"
            icon="icon-delete"
            :tip="t('common.delete')"
            data-cy="remove"
            @click="removeColumn(row)"
          />
        </template>
      </el-table-column>
    </KTable>
    <KBottomBar
      :permission="{
        feature: 'database',
        action: 'edit',
      }"
      @cancel="onCancel"
      @save="onSave"
    />
  </div>
  <EditColumnDialog
    v-if="showEditDialog"
    v-model="showEditDialog"
    :columns="columns"
    :column="currentColumn"
    :db-type="dbType"
  />
</template>

<script lang="ts" setup>
import { onMounted, onUnmounted, ref, watch } from "vue";
import { useControlTypes } from "./use-control-types";
import KTable from "@/components/k-table";
import type { DatabaseColumn } from "@/api/database";
import { getDatabaseColumns, updateColumn } from "@/api/database";
import EditColumnDialog from "./components/edit-column-dialog.vue";
import { cloneDeep } from "lodash-es";
import { useTable } from "./use-table";
import KBottomBar from "@/components/k-bottom-bar/index.vue";
import { getQueryString } from "@/utils/url";

import { useI18n } from "vue-i18n";
import { onBeforeRouteLeave, useRoute, useRouter } from "vue-router";
import { useRouteSiteId } from "@/hooks/use-site-id";
import { useSiteStore } from "@/store/site";
import { useSaveTip } from "@/hooks/use-save-tip";
const { t } = useI18n();
const router = useRouter();
const siteStore = useSiteStore();
const table = getQueryString("table") as string;
const from = getQueryString("from");
const columns = ref<DatabaseColumn[]>([]);
const currentColumn = ref<DatabaseColumn | undefined>(undefined);
const { getControlType } = useControlTypes();
const { dbType, appendQueryToRoute, getListRouteName } = useTable();
const listRouteName = getListRouteName();
const route = useRoute();
const saveTip = useSaveTip();
route.meta.activeMenu = listRouteName;
onMounted(async () => {
  let result = await getDatabaseColumns(dbType, { table });
  result = result.filter((f) => f.name != "_version");
  columns.value = result;
  saveTip.init(columns.value);
});

// 组件销毁时重置firstActiveMenu的值，防止影响到activeName外面的行为
onUnmounted(() => {
  siteStore.firstActiveMenu = "";
});

onBeforeRouteLeave(async (to, from, next) => {
  if (to.name === "login") {
    next();
  } else {
    siteStore.firstActiveMenu = to.meta.activeMenu ?? to.name;
    await saveTip
      .check(columns.value)
      .then(() => {
        next();
      })
      .catch(() => {
        // 当前路由的query的dbType不存在时，firstActiveMenu默认是table
        siteStore.firstActiveMenu = from.query.dbType
          ? (from.query.dbType as string).toLocaleLowerCase() + ".table"
          : "table";
        next(false);
      });
  }
});

async function removeColumn(row: DatabaseColumn) {
  if (siteStore.hasAccess("database", "edit")) {
    columns.value = columns.value.filter((item) => item !== row);
  }
}
const showEditDialog = ref(false);
function onShowEditColumnModal(column?: DatabaseColumn) {
  showEditDialog.value = true;
  currentColumn.value = column ? cloneDeep(column) : undefined;

  if (currentColumn.value) {
    if (currentColumn.value.setting) {
      currentColumn.value.setting =
        typeof currentColumn.value.setting === "string"
          ? JSON.parse(currentColumn.value.setting)
          : currentColumn.value.setting;
    } else {
      currentColumn.value.setting = { options: [] };
    }
  }
}
async function onSave() {
  const newColumns = columns.value.map((item) => {
    return {
      ...item,
      setting:
        typeof item.setting === "string"
          ? item.setting
          : JSON.stringify(item.setting),
    };
  });
  await updateColumn(dbType, { tableName: table, columns: newColumns });
  saveTip.init(columns.value);
  goBack();
}
function goBack() {
  if (from) {
    router.push(appendQueryToRoute({ name: from, query: { table } }));
  } else {
    router.push(useRouteSiteId({ name: listRouteName }));
  }
}
async function onCancel() {
  goBack();
}
watch(
  () => columns.value,
  () => {
    saveTip.changed(columns.value);
  },
  {
    deep: true,
  }
);
</script>
