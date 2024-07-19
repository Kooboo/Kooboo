<script setup lang="ts">
import Header from "@/layout/components/header";
import { useAppStore } from "@/store/app";
import { useMailModuleEditorStore } from "@/store/mail-module-editor";
import { getQueryString } from "@/utils/url";
import SideBar from "./side-bar.vue";
import TabContainer from "./tab-container.vue";
import KmailButton from "@/components/kmail-button/kmail-button.vue";
import SiteButton from "@/components/site-button/site-button.vue";
import { onBeforeRouteLeave } from "vue-router";
import { showConfirm } from "@/components/basic/confirm";
import { useI18n } from "vue-i18n";

const appStore = useAppStore();
const { t } = useI18n();
const mailModuleEditorStore = useMailModuleEditorStore();
mailModuleEditorStore.initialize(getQueryString("id")!);

onBeforeRouteLeave(async (to, from, next) => {
  if (to.name === "login") {
    next();
  } else {
    if (mailModuleEditorStore.tabs.find((item) => item.changed)) {
      await showConfirm(t("common.unsavedChangesLeaveTips"))
        .then(() => {
          next();
        })
        .catch(() => {
          next(false);
        });
    } else {
      next();
    }
  }
});
</script>

<template>
  <el-config-provider :locale="appStore.locale">
    <div class="h-full flex flex-col">
      <Header class="pl-40px">
        <template #left>
          <SiteButton />
          <KmailButton />
        </template>
        <template #right />
      </Header>
      <div class="flex-1 overflow-hidden relative">
        <div class="h-full flex min-h-600px">
          <SideBar />
          <TabContainer />
        </div>
      </div>
    </div>
  </el-config-provider>
</template>
