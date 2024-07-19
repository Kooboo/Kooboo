<template>
  <div class="p-24">
    <Breadcrumb :name="t('common.codeLogs')" />
  </div>
  <el-tabs
    v-model="currentTab"
    class="el-tabs--hide-content"
    @tab-change="handleClickTab"
  >
    <el-tab-pane
      v-for="tab in tabs"
      :key="tab.value"
      :label="tab.label"
      :name="tab.value"
    />
  </el-tabs>
  <div class="p-24">
    <div class="flex items-center space-x-16 mb-24">
      <el-date-picker
        v-model="dateValue"
        type="datetimerange"
        :unlink-panels="true"
        format="YYYY-MM-DD HH:mm"
        value-format="YYYY-MM-DD HH:mm"
        :range-separator="t('common.to')"
        :start-placeholder="t('common.startTime')"
        :end-placeholder="t('common.endTime')"
        class="h-40px max-w-460px"
        :editable="false"
        @change="changeTime"
      />
      <el-input
        v-model="model.traceId"
        class="w-250px roundedInput"
        :placeholder="t('common.traceId')"
        clearable
        data-cy="trace-id"
        @input="search"
      />

      <SearchInput
        v-model="model.keyword"
        class="w-250px"
        :placeholder="t('common.enterYourKeyword')"
        clearable
        data-cy="keyword"
      />
    </div>
    <KTable
      :data="list"
      sort="creationDate"
      :pagination="{
        pageSize: pagination.pageSize,
        totalCount: pagination.totalCount,
        currentPage: pagination.pageNr,
      }"
      @change="handlePageChange"
    >
      <el-table-column
        :label="t('codelog.level')"
        prop="level"
        width="120"
        data-cy="code-level"
      />
      <el-table-column
        :label="t('common.dateTime')"
        width="180"
        prop="lastModified"
        data-cy="last-modified"
      >
        <template #default="{ row }">{{
          useTime(row.creationDate ? row.creationDate + "Z" : "")
        }}</template>
      </el-table-column>
      <el-table-column
        :label="t('common.traceId')"
        prop="traceId"
        width="240"
        data-cy="trace-id"
      />
      <el-table-column
        :label="t('common.category')"
        prop="category"
        data-cy="category"
      />
      <el-table-column
        :label="t('common.message')"
        prop="message"
        data-cy="message"
      />
    </KTable>
  </div>
</template>

<script lang="ts" setup>
import type { CodeLog } from "@/api/development/code-log";
import { query } from "@/api/development/code-log";
import Breadcrumb from "@/components/basic/breadcrumb.vue";
import KTable from "@/components/k-table";
import { useTime } from "@/hooks/use-date";
import { searchDebounce } from "@/utils/url";
import type { DateModelType } from "element-plus";
import { onMounted, reactive, ref, watch } from "vue";

import { useI18n } from "vue-i18n";
const { t } = useI18n();
const tabs = [
  {
    label: t("common.all"),
    value: "All",
  },
  {
    label: t("common.debug"),
    value: "Debug",
  },
  {
    label: t("common.information"),
    value: "Information",
  },
  {
    label: t("common.warning"),
    value: "Warning",
  },
  {
    label: t("common.error"),
    value: "Error",
  },
  {
    label: t("common.critical"),
    value: "Critical",
  },
];

const currentTab = ref<string>("All");
const model = ref<{
  startDate: string;
  endDate: string;
  category: string;
  level: any;
  keyword: string;
  traceId: string;
  pageIndex: number;
  pageSize: number;
  [x: string]: number | string;
}>({
  startDate: "",
  endDate: "",
  category: "",
  level: undefined as any,
  keyword: "",
  traceId: "",
  pageIndex: 1,
  pageSize: 30,
});

const dateValue = ref<[DateModelType, DateModelType]>([
  model.value.startDate,
  model.value.endDate,
]);

const changeTime = () => {
  if (
    dateValue.value &&
    dateValue.value.some((item: DateModelType) => item !== null)
  ) {
    model.value.startDate = useTime(dateValue.value[0].toString());
    model.value.endDate = useTime(dateValue.value[1].toString());
  } else {
    model.value.startDate = "";
    model.value.endDate = "";
  }
  search();
};

const pagination = reactive({
  pageNr: model.value.pageIndex,
  pageSize: model.value.pageSize,
  totalCount: 0,
});
const list = ref<CodeLog[]>([]);
onMounted(() => {
  getList();
});
async function getList(pageNr = 1) {
  model.value.pageIndex = pageNr;
  const body = { ...model.value };
  body.keyword = body.keyword?.trim();
  body.traceId = body.traceId?.trim();
  const response = await query(body);
  list.value = response.list;
  pagination.totalCount = response.total;
  pagination.pageNr = pageNr;
}
function refreshList() {
  model.value.pageIndex = 1;
  getList();
}

function handlePageChange(pageNr: number) {
  getList(pageNr);
}
function handleClickTab() {
  model.value.level = currentTab.value === "All" ? undefined : currentTab.value;
  refreshList();
}
const search = searchDebounce(refreshList, 1000);

watch(
  () => model.value.keyword,
  () => {
    if (model.value.keyword) {
      search();
    } else {
      refreshList();
    }
  }
);
</script>

<style scoped>
:deep(.el-range-editor.el-input__inner) {
  --tw-border-opacity: 0.3;
  border-color: rgba(34, 150, 243, var(--tw-border-opacity));
  border-radius: 8px;
}
:deep(.el-range-editor .el-range-input) {
  font-size: 14px;
}
:deep(svg.icon) {
  color: #000000;
}
:deep(.roundedInput .el-input__wrapper) {
  @apply rounded-full px-16;
}
</style>
