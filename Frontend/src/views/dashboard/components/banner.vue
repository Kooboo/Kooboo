<script lang="ts" setup>
import type { Banner } from "@/api/dashboard/types";

defineProps<{ info: Banner }>();
</script>

<template>
  <el-card shadow="always">
    <div class="mb-12 text-000 dark:text-gray text-18px">
      {{ info?.title }}
    </div>
    <p class="my-12 text-12px dark:text-999">{{ info?.summary }}</p>
    <a
      class="text-blue underline mt-16 block"
      :href="info?.url"
      target="_blank"
      >{{ info?.linkText }}</a
    >
  </el-card>
</template>
