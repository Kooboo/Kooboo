<script lang="ts" setup>
import { dmarcReport } from "@/api/mail";

import { ref } from "vue";
import KTable from "@/components/k-table";
import { useI18n } from "vue-i18n";
import { useDate } from "@/hooks/use-date";
import { CircleCheckFilled, CircleCloseFilled } from "@element-plus/icons-vue";

const { t } = useI18n();

const list = ref<any[]>([]);
dmarcReport().then((rsp) => {
  list.value = rsp;
});
</script>

<template>
  <div class="p-24">
    <KTable :data="list">
      <el-table-column :label="t('common.reportDomain')" prop="reportDomain" />
      <el-table-column :label="t('common.ourDomain')" prop="ourDomain" />
      <el-table-column :label="t('common.date')" prop="date" align="center">
        <template #default="{ row }">
          <span>{{ useDate(row.start) }}</span>
        </template>
      </el-table-column>
      <el-table-column
        :label="t('common.mailCount')"
        prop="count"
        align="center"
      />
      <el-table-column :label="t('common.status')" align="center">
        <template #default="{ row }">
          <div class="flex items-center space-x-4">
            <el-icon :class="row.pass ? 'text-green' : 'text-orange'"
              ><CircleCheckFilled
            /></el-icon>
            <span>{{ row.pass ? t("common.pass") : t("common.reject") }}</span>
          </div>
        </template>
      </el-table-column>
      <el-table-column
        :label="t('common.fromIp')"
        prop="sourceIP"
        align="center"
      />
      <el-table-column
        :label="t('common.mailFrom')"
        prop="msgFrom"
        align="center"
      />
    </KTable>
  </div>
</template>
