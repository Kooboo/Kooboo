<script setup lang="ts">
import Header from "./components/header";
import KmailButton from "../components/kmail-button/kmail-button.vue";
import SiteButton from "../components/site-button/site-button.vue";

import { useAppStore } from "@/store/app";

const appStore = useAppStore();
</script>

<template>
  <el-config-provider :locale="appStore.locale">
    <div class="h-full flex flex-col dark:bg-[#333]">
      <div class="w-full z-100 shadow-s-10">
        <Header class="max-w-1120px mx-auto shadow-none">
          <template #left>
            <SiteButton />
            <KmailButton />
          </template>
        </Header>
      </div>
      <div class="overflow-hidden flex-1 bg-[#f3f5f5] dark:bg-[#1e1e1e]">
        <el-scrollbar id="main-scrollbar">
          <div>
            <router-view />
          </div>
        </el-scrollbar>
      </div>
    </div>
  </el-config-provider>
</template>
