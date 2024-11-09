<script lang="ts" setup>
import { useRoute, useRouter } from "vue-router";
import Breadcrumb from "@/components/basic/breadcrumb.vue";
import { useI18n } from "vue-i18n";
import { ref } from "vue";
import ExpressPanel from "./express-panel.vue";
import DigitalPanel from "./digital-panel.vue";
import { getQueryString } from "@/utils/url";

const { t } = useI18n();
const route = useRoute();
const routeName = route.meta.title as string;
const router = useRouter();

const tabs = [
  {
    displayName: t("common.expressShippings"),
    value: "express",
    component: ExpressPanel,
  },
  {
    displayName: t("common.digitalShippings"),
    value: "digital",
    component: DigitalPanel,
  },
];

const activeTab = ref(getQueryString("name") || tabs[0].value);

const pushActiveName = () => {
  router.push({
    name: route.name?.toString(),
    query: {
      ...route.query,
      name: activeTab.value,
    },
  });
};
</script>

<template>
  <div class="p-24">
    <Breadcrumb :name="routeName" />
  </div>
  <el-tabs v-model="activeTab" @tab-change="pushActiveName">
    <el-tab-pane
      v-for="tab in tabs"
      :key="tab.value"
      :label="tab.displayName"
      :name="tab.value"
    >
      <component :is="tab.component" v-if="activeTab === tab.value" />
    </el-tab-pane>
  </el-tabs>
</template>
