<script lang="ts" setup>
import { useI18n } from "vue-i18n";
import type { News } from "@/api/dashboard/types";

const { t } = useI18n();
defineProps<{ list: News[] }>();
</script>

<template>
  <el-card shadow="always">
    <div class="mb-16 text-18px flex items-center justify-between">
      <span>{{ t("common.productNews") }}</span
      ><a
        class="text-blue text-14px cursor-pointer w-80px"
        href="https://www.kooboo.com/"
        target="_blank"
        >{{ t("common.viewMore") }}</a
      >
    </div>
    <ul class="space-y-20px text-999">
      <li v-for="item in list" :key="item.title" class="flex space-x-8">
        <img v-if="item.image" :src="item.image" class="w-85px shadow-s-10" />
        <div class="flex-1 overflow-hidden">
          <div class="mb-12 text-18px flex items-center space-x-24">
            <a
              class="dark:text-gray hover:text-blue text-16px ellipsis text-666 flex-1"
              :href="item.url"
              :title="item.title"
              :class="item.image ? 'w-150px' : 'w-250px'"
              target="_blank"
              >{{ item.title }}</a
            >
            <span class="text-12px cursor-pointer w-80px ellipsis">{{
              item.date
            }}</span>
          </div>
          <p class="text-12px leading-4">
            {{ item.summary }}
          </p>
        </div>
      </li>
    </ul>
  </el-card>
</template>
