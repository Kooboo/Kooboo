<script lang="ts" setup>
import KTable from "@/components/k-table";
import { ref, watch } from "vue";
import { getTopPages } from "@/api/visitor-log";
import { bytesToSize } from "@/utils/common";
import type { TopPage } from "@/api/visitor-log/types";

import { useI18n } from "vue-i18n";
const props = defineProps<{ week: string }>();
const { t } = useI18n();
const data = ref<TopPage[]>([]);

const load = async () => {
  data.value = await getTopPages(props.week);
};

watch(
  () => props.week,
  (val) => {
    val && load();
  },
  { immediate: true }
);
</script>

<template>
  <KTable :data="data">
    <el-table-column :label="t('common.name')">
      <template #default="{ row }"> {{ row.name }} </template>
    </el-table-column>
    <el-table-column :label="t('common.size')">
      <template #default="{ row }"> {{ bytesToSize(row.size) }} </template>
    </el-table-column>
    <el-table-column :label="t('visitorLog.views')" prop="count" />
  </KTable>
</template>
