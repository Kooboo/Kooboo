<script setup lang="ts">
import { useI18n } from "vue-i18n";
import { useRouter } from "vue-router";

const router = useRouter();
const { t } = useI18n();
</script>

<template>
  <a
    class="h-full !mx-0 px-16px flex items-center justify-center cursor-pointer hover:text-blue border-l-1 border-[#e1e2e8] dark:border-opacity-50"
    data-cy="site"
    @click="
      router.push({
        name: 'home',
      })
    "
  >
    {{ t("common.site") }}
  </a>
</template>
