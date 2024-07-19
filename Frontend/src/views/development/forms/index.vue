<script lang="ts" setup>
import { ref } from "vue";
import ExternalPanel from "./external-panel.vue";
import EmbeddedPanel from "./embedded-panel.vue";
import Breadcrumb from "@/components/basic/breadcrumb.vue";
import { useFormStore } from "@/store/form";
import { useRoute, useRouter } from "vue-router";

import { useI18n } from "vue-i18n";
import { getQueryString } from "@/utils/url";

const router = useRouter();
const route = useRoute();
const { t } = useI18n();
const tabs = [
  {
    displayName: t("common.external"),
    value: "external",
    component: ExternalPanel,
  },
  {
    displayName: t("common.embedded"),
    value: "embedded",
    component: EmbeddedPanel,
  },
];

const activeTab = ref(getQueryString("name") || tabs[0].value);
const formStore = useFormStore();
formStore.loadAll();
const pushActiveName = () => {
  router.push({
    name: route.name?.toString(),
    query: { ...route.query, name: activeTab.value },
  });
};
</script>

<template>
  <div>
    <Breadcrumb :name="t('common.form')" class="p-24" />
    <el-tabs v-model="activeTab" @tab-change="pushActiveName()">
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
