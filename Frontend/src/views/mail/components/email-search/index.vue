<template>
  <div class="flex items-center">
    <SearchInput
      v-model="emailStore.messageKeyword"
      class="w-300px h-42px"
      :placeholder="t('common.searchEmails')"
      data-cy="search"
      @keydown.enter="searchEmail('common')"
    />
    <!-- 先隐藏高级搜索
    <span
      class="iconfont icon-gaojisousuo1 hover:text-blue text-24px cursor-pointer ml-8"
      :title="t('common.advancedSearchForEmail')"
      @click="advancedSearchDialog = true"
    />  -->
  </div>
  <AdvancedSearchDialog
    v-if="advancedSearchDialog"
    v-model="advancedSearchDialog"
    @search-email="searchEmail('advanced')"
  />
</template>
<script setup lang="ts">
import { ref } from "vue";
import { useEmailStore } from "@/store/email";
import { useRouter, useRoute } from "vue-router";
import AdvancedSearchDialog from "@/views/mail/components/advanced-search-dialog/index.vue";

const router = useRouter();
const route = useRoute();

import { useI18n } from "vue-i18n";

const advancedSearchDialog = ref(false);
const emailStore = useEmailStore();

const { t } = useI18n();

const searchEmail = async (flag: string) => {
  emailStore.messageKeyword = emailStore.messageKeyword?.trim();
  if (flag === "common") {
    if (!emailStore.messageKeyword.trim()) return;
  }
  if (route.name === "searchEmail") {
    await emailStore.refreshMessageList(flag);
    emailStore.selectedMessage = [];
  }

  let query =
    flag === "common"
      ? {
          flag: flag,
          keyword: emailStore.messageKeyword,
        }
      : {
          flag: "advanced",
          ...emailStore.searchModel,
        };
  router.push({
    name: "searchEmail",
    query,
  });
};
</script>
