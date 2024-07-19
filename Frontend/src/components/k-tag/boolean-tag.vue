<script lang="ts" setup>
import { useI18n } from "vue-i18n";
defineProps<{ value: boolean }>();
const { t } = useI18n();
</script>

<template>
  <el-tag
    class="rounded-full"
    :type="value ? 'success' : 'info'"
    size="small"
    data-cy="bool"
  >
    {{ value ? t("common.yes") : t("common.no") }}
  </el-tag>
</template>
