<script setup lang="ts">
import Header from "./components/header";
import { useAppStore } from "@/store/app";
import SearchInput from "@/components/basic/search-input.vue";
import { useRoute } from "vue-router";
import { useTemplateStore } from "@/store/template";
import { useI18n } from "vue-i18n";
import { watch } from "vue";
import { searchDebounce } from "@/utils/url";
import KmailButton from "../components/kmail-button/kmail-button.vue";
import SiteButton from "../components/site-button/site-button.vue";
const { t } = useI18n();
const route = useRoute();
const templateStore = useTemplateStore();
const appStore = useAppStore();
templateStore.clearTemplate();

const searchEvent = () => {
  templateStore.changePage(1);
};
const search = searchDebounce(searchEvent, 1000);
watch(
  () => templateStore.keyword,
  () => {
    search();
  }
);
</script>

<template>
  <el-config-provider :locale="appStore.locale">
    <div class="h-full flex flex-col dark:bg-[#333]">
      <div class="w-full shadow-s-10 z-100">
        <Header class="max-w-1120px mx-auto shadow-none">
          <template #left>
            <SiteButton />
            <KmailButton />
          </template>
          <template v-if="route.name === 'templates'" #center>
            <SearchInput
              v-model="templateStore.keyword"
              class="w-250px h-32"
              :placeholder="t('common.templateName')"
              data-cy="search"
            />
          </template>
          <template #right />
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
