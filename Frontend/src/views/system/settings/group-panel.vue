<script lang="ts" setup>
import { site } from "./settings";

const emit = defineEmits<{
  (e: "update:modelValue", value: boolean): void;
}>();
defineProps<{ modelValue: boolean; label: string; tooltipLabel?: string }>();
</script>

<template>
  <template v-if="site">
    <el-form-item>
      <div class="flex items-center flex-1 max-w-504px">
        <span class="font-bold dark:text-fff/86">{{ label }}</span>
        <Tooltip v-if="tooltipLabel" :tip="tooltipLabel" custom-class="ml-4" />

        <div class="flex-1" />
        <el-switch
          :model-value="modelValue"
          :data-cy="label.replace(/\s+/g, '-')"
          @update:model-value="emit('update:modelValue', !!$event)"
        />
      </div>
    </el-form-item>
    <div
      v-if="modelValue"
      class="!-ml-56px !-mr-56px !px-56px bg-[#fafafa] dark:bg-[#333]"
    >
      <div class="max-w-504px py-16 mb-18px">
        <slot />
      </div>
    </div>
  </template>
</template>
