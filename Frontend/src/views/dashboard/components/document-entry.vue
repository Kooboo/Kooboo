<script lang="ts" setup>
import type { Doc } from "@/api/dashboard/types";
import { useI18n } from "vue-i18n";

const { t } = useI18n();

defineProps<{ list: Doc[] }>();
</script>

<template>
  <el-card shadow="always">
    <div class="mb-16 text-18px">{{ t("common.document") }}</div>
    <div class="grid grid-cols-2 gap-12">
      <a
        v-for="item in list"
        :key="item.name"
        :href="item.url"
        target="_blank"
        class="cursor-pointer px-12 py-8 border-1 border-[#e1e2e8] dark:border-666 rounded-5px flex items-center leading-6 space-x-4 dark:hover:bg-[#444] hover:bg-[#eff6ff]"
      >
        <img :src="item.ico" class="w-24 h-24" />

        <span class="ellipsis" :title="item.name">
          {{ item.name }}
        </span>
      </a>
    </div>
  </el-card>
</template>
