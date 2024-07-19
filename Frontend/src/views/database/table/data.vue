<template>
  <div class="p-24">
    <Breadcrumb
      :crumb-path="[
        {
          name:
            dbType === 'Database'
              ? t('common.database')
              : t('common.dbTypeTable', {
                  dbType,
                }),
          route: {
            name:
              dbType === 'Database' ? 'table' : `${dbType.toLowerCase()}.table`,
          },
        },
        { name: table },
      ]"
    />
    <div class="flex items-center py-24 space-x-16">
      <el-button
        v-hasPermission="{
          feature: 'database',
          action: 'edit',
        }"
        round
        @click="gotoEditData()"
      >
        <el-icon class="iconfont icon-a-addto" />
        {{ t("common.newData") }}
      </el-button>
      <el-button
        v-hasPermission="{
          feature: 'database',
          action: 'view',
        }"
        round
        @click="onExportData"
      >
        <el-icon class="iconfont icon-share" />
        {{ t("common.exportData") }}
      </el-button>
      <!-- TODO: support indexed DB -->
      <el-button
        v-if="dbType !== 'Database'"
        v-hasPermission="{
          feature: 'database',
          action: 'edit',
        }"
        round
        @click="showImportDialog = true"
      >
        <el-icon class="iconfont icon-a-Cloudupload" />
        {{ t("common.importData") }}
      </el-button>
      <div class="flex-1" />
      <el-tooltip
        class="box-item"
        effect="dark"
        :content="t('common.setting')"
        placement="top"
      >
        <router-link
          :to="
            appendQueryToRoute({
              name: 'table-columns',
              query: { table, from: 'table-data' },
            })
          "
        >
          <el-button circle>
            <el-icon class="iconfont icon-a-setup" /> </el-button
        ></router-link>
      </el-tooltip>
    </div>
    <KTable
      v-if="columnLoaded"
      :data="tableRecords"
      show-check
      :pagination="pagination"
      :permission="{
        feature: 'database',
        action: 'edit',
      }"
      :sort="order.name"
      :order="order.desc ? 'descending' : 'ascending'"
      @delete="onDelete"
      @change="changePage"
      @sort-change="sortChange"
    >
      <el-table-column
        v-for="{ name } in tableColumns.filter(
          (c) => c.name !== '_id' && c.name !== '_version'
        )"
        :key="name"
        sortable="custom"
        :prop="name"
        :label="name"
        :min-width="180"
      >
        <template #default="{ row }">
          <span class="ellipsis">{{ row[name] }}</span>
        </template>
      </el-table-column>
      <el-table-column fixed="right" width="70" align="right">
        <template #default="{ row }">
          <router-link
            :to="
              appendQueryToRoute({
                name: 'table-edit-data',
                query: { table, id: row._id, pageNr: pagination.currentPage },
              })
            "
          >
            <IconButton
              class="inline-flex"
              icon="icon-a-writein"
              :tip="t('common.edit')"
            />
          </router-link>
        </template>
      </el-table-column>
    </KTable>
    <ImportDataDialog
      v-model="showImportDialog"
      :db-type="dbType"
      :table="table"
      :database-columns="tableColumns"
      @done="getData"
    />
  </div>
</template>

<script lang="ts" setup>
import KTable from "@/components/k-table";
import { reactive, ref, watch, computed } from "vue";
import { getDatabaseData, deleteTableData, exportData } from "@/api/database";
import type { DatabaseListColumn } from "@/api/database";
import type { Pagination } from "@/components/k-table/types";
import type { KeyValue } from "@/global/types";
import { useTable } from "./use-table";
import { getQueryString } from "@/utils/url";
import Breadcrumb from "@/components/basic/breadcrumb.vue";
import { showDeleteConfirm } from "@/components/basic/confirm";
import ImportDataDialog from "./components/import-data-dialog.vue";

import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";
import dayjs from "dayjs";
import { useTime } from "@/hooks/use-date";
const { t } = useI18n();
const router = useRouter();
const table = getQueryString("table") as string;
const list = ref<KeyValue[][]>([]);
const showImportDialog = ref(false);
type DataRow = Record<string, string> & { _id: string };

const tableRecords = computed<DataRow[]>(() => {
  return list.value.map((row) => {
    const dataRow: DataRow = { _id: "" };
    tableColumns.value.forEach((column) => {
      row.forEach(({ key, value }) => {
        if (key !== column.name) {
          return;
        }

        if (
          column.dataType &&
          column.dataType.toLowerCase().includes("datetime") &&
          dayjs(value).isValid()
        ) {
          // format iso date
          dataRow[column.name] = useTime(value);
        } else {
          dataRow[column.name] = value;
        }
      });
    });

    return dataRow;
  });
});
const tableColumns = ref<DatabaseListColumn[]>([]);
const pagination = reactive<Pagination>({
  currentPage: 1,
  pageCount: 0,
  pageSize: 1,
});

let order = {
  name: "",
  desc: true,
};

var lastOrderState = localStorage.getItem(getOrderStateKey());
if (lastOrderState) {
  order = JSON.parse(lastOrderState);
}

const { dbType, appendQueryToRoute, getListRouteName } = useTable();
const listRouteName = getListRouteName();
const route = useRoute();
route.meta.activeMenu = listRouteName;
const columnLoaded = ref(false);
const getData = async () => {
  const datas = await getDatabaseData(dbType, {
    table,
    pageNr: pagination.currentPage!,
    order: order.name,
    desc: order.desc,
  });
  tableColumns.value = datas.columns ?? [];
  list.value = datas.list;
  pagination.currentPage = datas.pageNr;
  pagination.pageCount = datas.totalPages;
  pagination.pageSize = datas.pageSize;
  columnLoaded.value = true;
};

function onExportData() {
  exportData(dbType, {
    table,
    order: order.name,
    desc: order.desc,
    pageNr: 1,
    pageSize: 99999999,
  });
}

watch(
  () => route.query?.pageNr,
  async () => {
    pagination.currentPage = !route.query?.pageNr
      ? 1
      : parseInt(route.query?.pageNr as string);
    getData();
    if (route.query.dbType) {
      route.meta.activeMenu =
        (route.query.dbType as string).toLocaleLowerCase() + ".table";
    }
  },
  {
    immediate: true,
  }
);
async function changePage(value: number) {
  router.push(
    appendQueryToRoute({
      name: "table-data",
      query: {
        ...router.currentRoute.value.query,
        pageNr: value,
      },
    })
  );
}

async function onDelete(rows: DataRow[]) {
  try {
    await showDeleteConfirm(rows.length);
    await deleteTableData(dbType, {
      tableName: table,
      values: rows.map((item) => item._id),
    });
    getData();
  } catch {
    void 0;
  }
}

function gotoEditData() {
  router.push(
    appendQueryToRoute({ name: "table-edit-data", query: { table } })
  );
}

function getOrderStateKey() {
  return `${getQueryString("SiteId")}_${getQueryString(
    "dbType"
  )}_${getQueryString("table")}`;
}

function sortChange(obj: any) {
  order = {
    name: obj.prop,
    desc: obj.order != "ascending",
  };
  localStorage.setItem(getOrderStateKey(), JSON.stringify(order));
  getData();
}
</script>
