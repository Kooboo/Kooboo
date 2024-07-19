<script setup lang="ts">
import Header from "./components/header";
import LeftMenu from "./components/left-menu/index.vue";
import SiteList from "./components/header/site-list.vue";
import { useSiteStore } from "@/store/site";
import { useAppStore } from "@/store/app";
import DevMode from "./components/header/dev-mode.vue";
import ConflictDialog from "@/components/conflict-dialog/code-conflict.vue";
import { useDiffStore } from "@/store/diff";
import KmailButton from "@/components/kmail-button/kmail-button.vue";
import { ref, watch } from "vue";
import { ElMessage } from "element-plus";
import { useI18n } from "vue-i18n";
import { useRoute } from "vue-router";
import { router } from "@/modules/router";

const { t } = useI18n();
const route = useRoute();
const appStore = useAppStore();
const siteStore = useSiteStore();
const diffStore = useDiffStore();
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
    router.addRoute({
      name: "test",
      path: "/sites/test",
      component: () => import("@/views/modules/module-index.vue"),
      meta: {
        menu: {
          display: "aaa",
          icon: "icon-media1",
        },
      },
    });
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
          <SiteList v-if="!appStore.limitSite" @change="change($event)" />
          <DevMode />
          <KmailButton v-if="!appStore.limitSite" />
        </template>
        <template #right />
      </Header>
      <div v-if="appStore.header" class="flex-1 overflow-hidden relative">
        <LeftMenu class="absolute inset-0 right-auto" />
        <div class="absolute inset-0 left-202px">
          <el-scrollbar id="main-scrollbar" class="w-full">
            <router-view :key="$route.name!" />
          </el-scrollbar>
        </div>
      </div>
    </div>
  </el-config-provider>
</template>
