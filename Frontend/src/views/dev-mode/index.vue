<script lang="ts" setup>
import { useDevModeStore } from "@/store/dev-mode";
import SideBar from "./side-bar.vue";
import TabContainer from "./tab-container.vue";
import { onBeforeRouteLeave, onBeforeRouteUpdate } from "vue-router";
import { showConfirm } from "@/components/basic/confirm";
import { useI18n } from "vue-i18n";
import { getQueryString } from "@/utils/url";
import { provide, onBeforeMount } from "vue";
import { useFileOpener } from "./file-opener";
import { errorMessage } from "@/components/basic/message";
import {
  onFileNotFoundInjectionFlag,
  onFileOpenInjectionFlag,
} from "@/components/monaco-editor/config";

const fileOpener = useFileOpener();
const onFileNotFound = (url: string) => {
  errorMessage(t("common.resourceNotFound", { url }));
};

const { t } = useI18n();
const devModeStore = useDevModeStore();

onBeforeMount(async () => {
  await devModeStore.loadActivePanel();
});

onBeforeRouteLeave(async (to) => {
  if (devModeStore.tabs.length && to.name != "login") {
    await showConfirm(t("common.leaveDevModeTip"));
    devModeStore.activeTab = undefined;
    devModeStore.tabs = [];
  }
});

onBeforeRouteUpdate(async (to, from) => {
  var toSiteId = getQueryString("siteId", to.query as Record<string, string>);
  var fromSiteId = getQueryString(
    "siteId",
    from.query as Record<string, string>
  );
  if (devModeStore.tabs.length && toSiteId != fromSiteId) {
    await showConfirm(t("common.leaveDevModeTip"));
    devModeStore.activeTab = undefined;
    devModeStore.tabs = [];
  }
});
const updateTabModel = async (model: any) => {
  const tab = await devModeStore.tabs.find((f) => f.id === model.id)
    ?.panelInstance?.promise;
  if (!tab) return;
  tab.updateModel(model);
};

//导出这个方法的类型
export type UpdateTabModel = typeof updateTabModel;
provide("updateTabModel", updateTabModel);
provide(onFileOpenInjectionFlag, fileOpener.open);
provide(onFileNotFoundInjectionFlag, onFileNotFound);
</script>

<template>
  <div v-if="devModeStore.activities.length" class="h-full flex min-h-600px">
    <SideBar />
    <TabContainer />
  </div>
</template>
