<template>
  <el-form-item
    :label="field.displayName ?? field.name"
    :prop="field.prop"
    :required="field.required"
    :class="cssClass"
  >
    <ColorPicker v-slot="{ color }" v-model="model[field.name]" show-alpha>
      <div
        class="flex items-center space-x-8 p-4 max-w-230px w-full justify-end"
      >
        <div
          :style="{ backgroundColor: color }"
          class="w-24 h-24 rounded-4px shadow-s-10"
        />
      </div>
    </ColorPicker>
  </el-form-item>
</template>

<script lang="ts" setup>
import ColorPicker from "@/components/basic/color-picker.vue";
import type { Field } from "@/components/field-control/types";
defineProps<{
  model: Record<string, any>;
  field: Field;
  cssClass?: any;
}>();
</script>

<style lang="scss" scoped>
:deep(.el-form-item__content .el-color-picker) {
  display: flex;
  height: 40px;
  align-items: center;
}
</style>
