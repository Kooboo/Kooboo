<script lang="ts" setup>
import { ref, watch } from "vue";
import ExternalPanel from "./external-panel.vue";
import EmbeddedPanel from "./embedded-panel.vue";
import GroupPanel from "./group-panel.vue";
import Breadcrumb from "@/components/basic/breadcrumb.vue";
import { useRouter, useRoute } from "vue-router";
import { getQueryString } from "@/utils/url";
import { useI18n } from "vue-i18n";
import SettingsDialog from "./settings-dialog.vue";
import { useSiteStore } from "@/store/site";

const siteStore = useSiteStore();
const router = useRouter();
const route = useRoute();
const { t } = useI18n();
const showSettingsDialog = ref(false);

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
  {
    displayName: t("common.group"),
    value: "group",
    component: GroupPanel,
  },
];

const activeTab = ref(getQueryString("name") || tabs[0].value);
const showSetting = () => {
  if (siteStore.hasAccess("site", "edit")) {
    showSettingsDialog.value = true;
  }
};

watch(
  () => activeTab.value,
  (tab) => {
    router.push({
      name: route.name?.toString(),
      query: {
        ...route.query,
        name: tab,
      },
    });
  }
);
</script>

<template>
  <div>
    <Breadcrumb :name="t('common.scripts')" class="p-24" />
    <div class="relative">
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
      <div
        class="absolute top-0 right-24 h-52px flex justify-center items-center"
      >
        <IconButton
          :permission="{
            feature: 'site',
            action: 'edit',
          }"
          class="dark:bg-999"
          circle
          icon="icon-a-setup"
          :tip="t('common.settings')"
          @click="showSetting"
        />
      </div>
    </div>
    <SettingsDialog v-if="showSettingsDialog" v-model="showSettingsDialog" />
  </div>
</template>
