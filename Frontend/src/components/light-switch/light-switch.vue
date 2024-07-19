<script setup lang="ts">
import { useI18n } from "vue-i18n";
import { dark, toggleDark } from "@/composables/dark";
import { Sunny, Moon } from "@element-plus/icons-vue";

const { t } = useI18n();
</script>

<template>
  <div
    class="border-r-1 border-[#e1e2e8] dark:border-opacity-50 h-full flex items-center justify-center"
  >
    <el-icon
      class="cursor-pointer dark:text-fff/50 hover:text-blue hover:dark:text-blue text-16px mx-16"
      :title="dark ? t('common.toggleLight') : t('common.toggleDark')"
      data-cy="togger-theme"
      @click="toggleDark()"
    >
      <Moon v-if="dark" />
      <Sunny v-else />
    </el-icon>
  </div>
</template>
