<template>
  <div class="flex justify-center mt-12">
    <div>
      <img src="@/assets/images/no_matching_site.svg" class="dark:opacity-60" />
      <p class="text-center mt-16 dark:text-666">
        {{ t("common.noMatchingWebsite") }}
      </p>
    </div>
  </div>
</template>

<script lang="ts" setup>
import { useI18n } from "vue-i18n";

const { t } = useI18n();
</script>
