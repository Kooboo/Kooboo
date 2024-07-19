<script setup lang="ts">
import Header from "./components/header";
import SiteList from "./components/header/site-list.vue";
import { useSiteStore } from "@/store/site";
import { useAppStore } from "@/store/app";
import DevMode from "./components/header/dev-mode.vue";
import ConflictDialog from "@/components/conflict-dialog/code-conflict.vue";
import KmailButton from "@/components/kmail-button/kmail-button.vue";
import { useDiffStore } from "@/store/diff";
import { watch, ref } from "vue";
import { useRoute } from "vue-router";
import { ElMessage } from "element-plus";
import { useI18n } from "vue-i18n";

const { t } = useI18n();
const appStore = useAppStore();
const siteStore = useSiteStore();
const diffStore = useDiffStore();
const route = useRoute();
const isChangeSite = ref(false);
const change = (e: boolean) => {
  isChangeSite.value = e;
};

watch(
  () => route.query.SiteId,
  (n) => {
    if (n && isChangeSite.value) {
      ElMessage({
        message: t("common.siteChangingSuccess"),
        type: "success",
      });
      isChangeSite.value = false;
    }
  }
);
</script>

<template>
  <el-config-provider :locale="appStore.locale">
    <ConflictDialog v-if="diffStore.show" />
    <div
      v-if="siteStore.site"
      class="h-full flex flex-col bg-[#f3f5f5] dark:bg-[#1e1e1e]"
    >
      <Header class="pl-40px">
        <template #left>
          <SiteList @change="change($event)" />
          <DevMode />
          <KmailButton />
        </template>
        <template #right />
      </Header>
      <div class="flex-1 overflow-hidden">
        <router-view v-if="appStore.header" />
      </div>
    </div>
  </el-config-provider>
</template>
