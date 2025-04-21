<script lang="ts" setup>
import { timeZoneLabel } from "@/utils/date";
import { useUtcTime } from "@/hooks/use-date";
import { useI18n } from "vue-i18n";
import { computed, useAttrs } from "vue";

const { t } = useI18n();
const attrs = useAttrs();

const utcTime = computed(() => {
  if (attrs.modelValue) return useUtcTime(attrs.modelValue as any);
  if (attrs["model-value"]) return useUtcTime(attrs["model-value"] as any);
  return "";
});
</script>

<template>
  <div class="relative flex items-center gap-8">
    <el-date-picker v-bind="$attrs" />
    <el-tooltip placement="top" :disabled="!utcTime">
      <template #content>
        <span>{{ t("common.utcTime") }}: </span>
        <span>{{ utcTime }}</span>
      </template>
      <span
        v-if="utcTime"
        class="text-s text-666/50 absolute right-32 cursor-default hover:text-666"
        >{{ timeZoneLabel }}</span
      >
    </el-tooltip>
  </div>
</template>
