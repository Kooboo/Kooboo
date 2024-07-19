<script lang="ts" setup>
import type { Log } from "@/api/site/job";
import { getLogs } from "@/api/site/job";
import KTable from "@/components/k-table";
import { useTime } from "@/hooks/use-date";
import { ref } from "vue";

import { useI18n } from "vue-i18n";
const props = defineProps<{ success: boolean }>();
const { t } = useI18n();
const data = ref<Log[]>([]);
getLogs(props.success).then((rsp) => (data.value = rsp));
</script>

<template>
  <div>
    <KTable :data="data">
      <el-table-column :label="t('common.jobName')">
        <template #default="{ row }">
          <span data-cy="name">{{ row.jobName }}</span>
        </template>
      </el-table-column>
      <el-table-column :label="t('common.description')">
        <template #default="{ row }">
          <span data-cy="description">{{ row.description }}</span>
        </template>
      </el-table-column>
      <el-table-column :label="t('common.executionTime')">
        <template #default="{ row }"
          ><span data-cy="execution-time">
            {{ useTime(row.executionTime + "Z") }}
          </span></template
        >
      </el-table-column>
      <el-table-column :label="t('common.message')">
        <template #default="{ row }">
          <span data-cy="message">{{ row.message }}</span>
        </template>
      </el-table-column>
    </KTable>
  </div>
</template>
