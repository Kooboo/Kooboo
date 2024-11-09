<script lang="ts" setup>
import { durationUnits } from "@/utils/date";

defineProps<{ modelValue: number; unit: string; readonly?: boolean }>();
defineEmits<{
  (e: "update:unit", value: string): void;
  (e: "update:model-value", value: number): void;
}>();
</script>

<template>
  <span v-if="readonly">
    {{ modelValue
    }}{{ durationUnits.find((f) => f.key == unit)?.value ?? unit }}
  </span>
  <el-input
    v-else
    :model-value="modelValue"
    class="max-w-250px"
    @update:model-value="
      $emit('update:model-value', $event == '' ? $event : parseInt($event))
    "
  >
    <template #append>
      <el-select
        class="w-120px !m-0"
        :model-value="unit"
        @update:model-value="$emit('update:unit', $event)"
      >
        <el-option
          v-for="item of durationUnits"
          :key="item.key"
          :label="item.value"
          :value="item.key"
        />
      </el-select>
    </template>
  </el-input>
</template>
