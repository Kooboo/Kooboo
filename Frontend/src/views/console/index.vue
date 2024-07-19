<script setup lang="ts">
import Header from "@/layout/components/header";
import KmailButton from "@/components/kmail-button/kmail-button.vue";
import SiteButton from "@/components/site-button/site-button.vue";

import LeftMenu from "./components/left-menu/index.vue";
import { useAppStore } from "@/store/app";
// import SiteList from "@/components/layout/header/site-list.vue";

const appStore = useAppStore();
</script>

<template>
  <el-config-provider :locale="appStore.locale">
    <div class="h-full flex flex-col bg-[#f3f5f5] dark:bg-[#1e1e1e]">
      <Header class="pl-40px">
        <template #left>
          <SiteButton />
          <KmailButton />
        </template>
        <template #right />
      </Header>
      <div class="flex-1 overflow-hidden relative">
        <LeftMenu />
        <div class="absolute inset-0 left-202px">
          <el-scrollbar id="main-scrollbar" class="w-full">
            <router-view :key="$route.name!" />
          </el-scrollbar>
        </div>
      </div>
    </div>
  </el-config-provider>
</template>
