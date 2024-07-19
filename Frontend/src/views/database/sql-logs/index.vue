<template>
  <div class="p-24">
    <Breadcrumb :name="t('common.sqlLogs')" />
    <div class="flex items-center py-24 space-x-16">
      <el-select
        v-model="query.week"
        class="w-240px"
        :placeholder="t('common.week')"
        :title="t('common.selectAWeek')"
        @change="getLogs"
      >
        <el-option
          v-for="item in weeks"
          :key="item"
          :value="item"
          :label="weekToDates(item)"
          data-cy="week-opt"
        />
      </el-select>
      <el-select
        v-model="query.type"
        class="w-200px"
        clearable
        :placeholder="t('common.type')"
        @change="getLogs"
      >
        <el-option
          v-for="item in types"
          :key="item.value"
          :value="item.value"
          :label="item.label"
          data-cy="type-opt"
        />
      </el-select>
      <SearchInput
        v-model="query.keyword"
        class="w-250px"
        :placeholder="t('common.enterYourKeyword')"
        clearable
        data-cy="keyword"
      />
    </div>
    <KTable :data="list" :pagination="pagination" @change="getLogs">
      <el-table-column :label="t('common.type')" prop="type" width="100" />
      <el-table-column :label="t('common.lastModified')" width="180">
        <template #default="{ row }">{{ useTime(row.dateTime) }}</template>
      </el-table-column>
      <el-table-column :label="t('common.content')" prop="sql" />
      <el-table-column :label="t('common.parameters')" width="160">
        <template #default="{ row }">{{
          // params不存在或等于"null" 则返回""
          !row.params || row.params === "null" ? "" : row.params
        }}</template>
      </el-table-column>
    </KTable>
  </div>
</template>

<script lang="ts" setup>
import KTable from "@/components/k-table";
import { onMounted, reactive, ref, watch } from "vue";
import type { SqlLog } from "@/api/database/sql-log";
import { getList, getWeeks } from "@/api/database/sql-log";
import type { Pagination } from "@/components/k-table/types";
import { useTime } from "@/hooks/use-date";
import Breadcrumb from "@/components/basic/breadcrumb.vue";
import SearchInput from "@/components/basic/search-input.vue";

import { useI18n } from "vue-i18n";
import { weekToDates } from "@/utils/date";
import { searchDebounce } from "@/utils/url";
const { t } = useI18n();
const pagination = reactive<Pagination>({
  currentPage: 1,
  pageCount: 0,
  pageSize: 1,
});
const weeks = ref<string[]>([]);
const types = [
  {
    label: "SQLite",
    value: "sqlite",
  },
  {
    label: "MySQL",
    value: "mysql",
  },
  {
    label: "SQLServer",
    value: "sqlserver",
  },
];
const query = reactive<{
  type: string;
  week: string;
  keyword: string;
  pageIndex: number;
  [x: string]: string | number;
}>({
  type: "",
  week: "",
  keyword: "",
  pageIndex: 1,
});
const list = ref<SqlLog[]>([]);
onMounted(async () => {
  await fetchWeeks();
  await getLogs();
});

async function fetchWeeks() {
  weeks.value = await getWeeks();
  query.week = weeks.value[0];
}

async function getLogs(pageNr?: number) {
  query.pageIndex = pageNr || 1;
  const data = await getList({
    ...query,
    keyword: query.keyword.trim(),
  });
  list.value = data.list;
  pagination.currentPage = data.pageNr;
  pagination.pageCount = data.totalPages;
  pagination.pageSize = data.pageSize;
}

watch(
  () => query.keyword,
  () => {
    if (query.keyword) {
      search();
    } else {
      getLogs();
    }
  }
);

const search = searchDebounce(getLogs, 1000);
</script>
