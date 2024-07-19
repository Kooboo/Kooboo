<script lang="ts" setup>
import KTable from "@/components/k-table";
import { ref, watch } from "vue";
import { getErrorList } from "@/api/visitor-log";
import type { Error } from "@/api/visitor-log/types";
import { useTime } from "@/hooks/use-date";

import { useI18n } from "vue-i18n";
const props = defineProps<{ week: string }>();
const { t } = useI18n();
const data = ref<{
  dataList: Error[];
  pageNr: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}>();

const load = async (index?: number) => {
  data.value = await getErrorList(props.week, index);
};

watch(
  () => props.week,
  (val) => {
    val && load(1);
  },
  { immediate: true }
);
</script>

<template>
  <KTable
    :data="data?.dataList!"
    :pagination="{
      currentPage: data?.pageNr,
      pageCount: data?.totalPages,
      pageSize: data?.pageSize,
    }"
    @change="load"
  >
    <el-table-column label="IP" width="140">
      <template #default="{ row }">
        <span data-cy="ip">{{ row.clientIP }}</span>
      </template>
    </el-table-column>
    <el-table-column label="URL">
      <template #default="{ row }">
        <a class="text-blue ellipsis" :href="row.previewUrl" target="_blank">{{
          row.url
        }}</a>
      </template>
    </el-table-column>
    <el-table-column :label="t('common.startTime')" width="180">
      <template #default="{ row }">
        <span data-cy="start-time"
          >{{ useTime(row.startTime) }}
        </span></template
      >
    </el-table-column>
    <el-table-column :label="t('common.message')" prop="message" />
    <el-table-column :label="t('common.status')" prop="statusCode" />
  </KTable>
</template>
