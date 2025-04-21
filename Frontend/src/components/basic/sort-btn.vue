<script lang="ts" setup>
import { CaretTop, CaretBottom } from "@element-plus/icons-vue";
const props = defineProps<{ desc?: boolean }>();
defineEmits<{ (e: "change", value?: boolean): void }>();

function change(value: boolean) {
  if (value === undefined) {
    return value;
  } else {
    return props.desc === value ? undefined : value;
  }
}
</script>

<template>
  <div class="flex flex-col">
    <div class="overflow-hidden">
      <el-icon
        :class="{ 'text-blue': desc === false }"
        class="relative bottom-[-4px]"
        @click.stop="$emit('change', change(false))"
        ><CaretTop
      /></el-icon>
    </div>

    <div class="overflow-hidden">
      <el-icon
        :class="{ 'text-blue': desc === true }"
        class="relative top-[-4px]"
        @click.stop="$emit('change', change(true))"
        ><CaretBottom
      /></el-icon>
    </div>
  </div>
</template>
