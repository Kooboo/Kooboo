<script setup lang="ts">
import LeftMenu from "./left-menu.vue";
import Header from "@/layout/components/header";
import { useAppStore } from "@/store/app";
import KmailButton from "@/components/kmail-button/kmail-button.vue";
import SiteButton from "@/components/site-button/site-button.vue";

const appStore = useAppStore();
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
        <LeftMenu />
        <div class="absolute inset-0 left-202px bg-[#f3f5f5] dark:bg-[#121212]">
          <el-scrollbar id="main-scrollbar" class="w-full">
            <router-view :key="$route.name!" />
          </el-scrollbar>
        </div>
      </div>
    </div>
  </el-config-provider>
</template>
