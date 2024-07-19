<script lang="ts" setup>
import MultilingualSelector from "@/components/multilingual-selector/index.vue";
import { options } from "./types";

import { useI18n } from "vue-i18n";
defineProps<{ modelValue: options }>();

defineEmits<{
  (e: "update:modelValue", value: options): void;
}>();
const { t } = useI18n();

const tabs: { key: options; value: string }[] = [
  { key: options.design, value: t("common.design") },
  { key: options.settings, value: t("common.setting") },
];
</script>

<template>
  <div class="flex items-center px-24 py-12 leading-none">
    <el-radio-group
      class="el-radio-group--rounded"
      :model-value="modelValue"
      @update:model-value="$emit('update:modelValue', $event as options)"
    >
      <el-radio-button
        v-for="item of tabs"
        :key="item.key"
        :label="item.key"
        :data-cy="item.key === 0 ? 'design' : 'setting'"
        >{{ item.value }}</el-radio-button
      >
    </el-radio-group>
    <p class="flex-1 text-m" />
    <MultilingualSelector />
  </div>
</template>
