<script lang="ts" setup>
import { ref } from "vue";
import InternalPanel from "./internal-panel.vue";
import ExternalPanel from "./external-panel.vue";
import NotFound from "./not-found-panel.vue";
import Breadcrumb from "@/components/basic/breadcrumb.vue";

import { useI18n } from "vue-i18n";
const { t } = useI18n();
const tabs = [
  {
    displayName: t("common.internal"),
    value: "internal",
    component: InternalPanel,
  },
  {
    displayName: t("common.external"),
    value: "external",
    component: ExternalPanel,
  },
  {
    displayName: t("common.notFound"),
    value: "notFound",
    component: NotFound,
  },
];

const activeTab = ref(tabs[0].value);
</script>

<template>
  <div>
    <Breadcrumb :name="t('common.urls')" class="p-24" />
    <el-tabs v-model="activeTab">
      <el-tab-pane
        v-for="tab in tabs"
        :key="tab.value"
        :label="tab.displayName"
        :name="tab.value"
      >
        <component :is="tab.component" v-if="activeTab === tab.value" />
      </el-tab-pane>
    </el-tabs>
  </div>
</template>
