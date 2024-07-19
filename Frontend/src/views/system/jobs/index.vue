<script lang="ts" setup>
import { ref } from "vue";
import JobPanel from "./job-panel.vue";
import LogPanel from "./log-panel.vue";
import Breadcrumb from "@/components/basic/breadcrumb.vue";

import { useI18n } from "vue-i18n";
const { t } = useI18n();
const tabs = [
  {
    displayName: t("common.taskList"),
    value: "taskList",
    component: JobPanel,
  },
  {
    displayName: t("common.completed"),
    value: "Completed",
    component: LogPanel,
  },
  {
    displayName: t("common.failed"),
    value: "Failed",
    component: LogPanel,
  },
];

const activeTab = ref(tabs[0].value);
</script>

<template>
  <div>
    <Breadcrumb :name="t('common.jobs')" class="p-24" />
    <el-tabs v-model="activeTab">
      <el-tab-pane
        v-for="tab in tabs"
        :key="tab.value"
        :label="tab.displayName"
        :name="tab.value"
      >
        <component
          :is="tab.component"
          v-if="activeTab === tab.value"
          :success="tab.value === 'Completed'"
        />
      </el-tab-pane>
    </el-tabs>
  </div>
</template>
