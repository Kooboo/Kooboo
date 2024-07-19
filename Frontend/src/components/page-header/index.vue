<script lang="ts" setup>
import { useRouter } from "vue-router";
import { useRouteSiteId } from "@/hooks/use-site-id";

import { useI18n } from "vue-i18n";
const router = useRouter();
defineProps<{ name: string }>();
const { t } = useI18n();
</script>

<template>
  <div>
    <p class="text-2l mb-12px font-bold">{{ name }}</p>
    <div class="flex items-center text-l">
      <span
        class="cursor-pointer hover:text-blue"
        @click="router.push({ name: 'home' })"
        >{{ t("common.sites") }}</span
      >
      <el-icon class="iconfont icon-a-nextstep2 ml-16px mr-16px text-12px" />
      <span
        class="cursor-pointer hover:text-blue"
        @click="router.push(useRouteSiteId({ name: 'dashboard' }))"
        >{{ t("common.dashboard") }}</span
      >
      <el-icon class="iconfont icon-a-nextstep2 ml-16px mr-16px text-12px" />
      <span class="text-666">{{ name }}</span>
    </div>
  </div>
</template>
