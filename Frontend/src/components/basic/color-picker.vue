<script lang="ts" setup>
defineProps<{ modelValue?: string; hidePicker?: boolean }>();
</script>

<template>
  <div class="relative __color-picker-container">
    <slot :color="modelValue">
      <div
        :style="{ backgroundColor: modelValue }"
        class="w-24 h-24 rounded-4px shadow-s-10"
      />
    </slot>
    <el-color-picker
      v-if="!hidePicker"
      v-bind="$attrs"
      :model-value="modelValue"
      class="w-full h-full"
    />
  </div>
</template>

<style lang="scss">
.__color-picker-container {
  .el-color-picker {
    @apply !w-full !h-full opacity-0 absolute inset-0;
    .el-color-picker__trigger {
      @apply !w-full !h-full;
      .el-color-picker__color-inner {
        display: none;
      }
    }
  }
}
</style>
