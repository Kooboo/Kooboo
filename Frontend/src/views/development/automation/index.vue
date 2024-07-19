<script setup lang="ts">
import { ref } from "vue";
import KTable from "@/components/k-table";

import { useI18n } from "vue-i18n";

import ExecutionResultDialogVue from "./execution-result-dialog.vue";
import { getAutomationTask } from "@/api/development/automation";
import type { Automation } from "@/api/development/automation";

const { t } = useI18n();
const showResultDialog = ref(false);
const dataList = ref<Automation[]>([]);
const currentId = ref();
const load = async () => {
  dataList.value = await getAutomationTask();
  dataList.value.forEach((item) => {
    item.id = item.id.toString();
  });
};

const execute = async (row: any) => {
  showResultDialog.value = true;
  currentId.value = row.id;
  load();
};

load();
</script>

<template>
  <div class="p-24">
    <KTable :data="dataList" show-check :is-radio="true" hide-delete>
      <template #leftBar="{ selected }">
        <div class="h-60px flex items-center px-44px">
          <div>
            <el-button
              round
              class="dark:bg-666"
              :disabled="!selected.length"
              @click="execute(selected[0])"
            >
              <div class="flex items-center">
                {{ t("common.execute") }}
              </div>
            </el-button>
          </div>
        </div>
      </template>

      <el-table-column :label="t('common.name')">
        <template #default="{ row }">
          {{ row.name }}
        </template>
      </el-table-column>
      <el-table-column :label="t('common.description')">
        <template #default="{ row }">
          {{ row.description }}
        </template>
      </el-table-column>
      <el-table-column
        :label="t('common.siteModification')"
        width="200"
        align="center"
      >
        <template #default="{ row }">
          <el-icon
            v-if="row.siteModification"
            class="iconfont icon-yes2 text-green"
          />
        </template>
      </el-table-column>
    </KTable>
  </div>
  <ExecutionResultDialogVue
    v-if="showResultDialog"
    :id="currentId"
    v-model="showResultDialog"
  />
</template>
