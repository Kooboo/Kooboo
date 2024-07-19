<script lang="ts" setup>
import ColorPicker from "@/components/basic/color-picker.vue";
import { ref } from "vue";

const props = defineProps<{ modelValue: string; displayColor?: string }>();
const emit = defineEmits<{ (e: "update:modelValue", value: string): void }>();
const picker = ref();
// eslint-disable-next-line vue/no-setup-props-destructure
let oldValue = props.modelValue || "rgba(0,0,0,0)";

function change(value: string) {
  if (value.endsWith("0)") && oldValue?.endsWith("0)")) {
    emit("update:modelValue", value.substring(0, value.length - 2) + "1)");
  }
  oldValue = value;
}
</script>

<template>
  <el-input
    :model-value="modelValue"
    :placeholder="displayColor"
    @update:model-value="$emit('update:modelValue', $event)"
  >
    <template #append>
      <div class="w-46px flex items-center justify-center pr-4">
        <ColorPicker
          ref="picker"
          show-alpha
          :model-value="modelValue || displayColor"
          @update:model-value="$emit('update:modelValue', $event)"
          @active-change="change"
        />
      </div>
    </template>
  </el-input>
</template>

<style lang="scss" scoped>
:deep(.el-color-picker) {
  .el-color-picker__trigger {
    @apply border-none;
  }
}
</style>
