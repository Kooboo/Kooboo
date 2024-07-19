<script lang="ts" setup>
import { useI18n } from "vue-i18n";
import { useTime } from "@/hooks/use-date";
import type { SiteInfo } from "@/api/dashboard/types";

const { t } = useI18n();

defineProps<{ info: SiteInfo }>();
</script>

<template>
  <el-card shadow="always">
    <div class="flex space-x-4">
      <div
        class="bg-blue h-60px text-30px w-60px rounded-full flex justify-center items-center text-fff uppercase overflow-hidden mr-16"
      >
        {{ info?.siteName.substring(0, 1) }}
      </div>
      <ul class="flex-1 space-y-12 overflow-hidden">
        <li class="flex space-x-4">
          <span class="w-60px text-999">{{ t("common.name") }}</span>
          <span
            class="flex-1 ellipsis overflow-hidden"
            :title="info?.siteName"
            >{{ info?.siteName }}</span
          >
        </li>
        <li class="flex space-x-4">
          <span class="w-60px text-999">{{ t("common.type") }}</span>
          <span class="flex-1">{{ info?.type }}</span>
        </li>
        <li class="flex space-x-4">
          <span class="w-60px text-999">{{ t("common.created") }}</span>
          <span
            class="flex-1 ellipsis"
            >{{ useTime(info?.creationDate!) }}</span
          >
        </li>
      </ul>
    </div>
    <ul class="space-y-18px mt-36px">
      <li class="flex space-x-4">
        <span class="w-76px text-999">{{ t("common.domain") }}</span>
        <div class="flex-1 overflow-hidden">
          <div
            v-for="item in info?.domains"
            :key="item"
            class="overflow-hidden ellipsis"
            :title="item"
          >
            {{ item }}
          </div>
        </div>
      </li>
      <li class="flex space-x-4">
        <span class="w-76px text-999">{{ t("common.user") }}</span>
        <div class="flex-1 space-y-4 overflow-hidden">
          <div
            v-for="item in info?.users"
            :key="item.userName"
            class="flex space-x-32 h-16 leading-16px"
          >
            <span class="ellipsis max-w-1/2" :title="item.userName">
              {{ item.userName }}</span
            >
            <span class="ellipsis max-w-1/2"> {{ item.role }}</span>
          </div>
        </div>
      </li>
      <li class="flex space-x-4">
        <span class="w-76px text-999">{{ t("common.url") }}</span>
        <a
          class="flex-1 text-blue overflow-hidden ellipsis"
          :href="info?.previewUrl"
          :title="info?.previewUrl"
          target="_blank"
          >{{ info?.previewUrl }}</a
        >
      </li>
    </ul>
  </el-card>
</template>
