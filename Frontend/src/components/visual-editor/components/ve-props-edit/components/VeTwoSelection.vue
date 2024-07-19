<template>
  <el-form-item
    :label="field.displayName ?? field.name"
    :prop="field.prop"
    :required="field.required"
    :class="cssClass"
  >
    <el-row class="w-full" :gutter="10">
      <el-col :span="12">
        {{ t("ve.horizontal") }}
      </el-col>
      <el-col :span="12">
        {{ t("ve.vertical") }}
      </el-col>
    </el-row>
    <el-row class="w-full" :gutter="10">
      <el-col :span="12">
        <el-select
          v-model="model[field.prop][0]"
          :placeholder="field.toolTip"
          clearable
          v-bind="field.settings"
        >
          <el-option
            v-for="opt in field.selectionOptions"
            :key="opt.value"
            :value="opt.value"
            :label="opt.key"
          />
        </el-select>
      </el-col>
      <el-col :span="12">
        <el-select
          v-model="model[field.prop][1]"
          :placeholder="field.toolTip"
          clearable
          v-bind="field.settings"
        >
          <el-option
            v-for="opt in field.selectionOptions"
            :key="opt.value"
            :value="opt.value"
            :label="opt.key"
          />
        </el-select>
      </el-col>
    </el-row>
  </el-form-item>
</template>

<script lang="ts" setup>
import type { Field } from "@/components/field-control/types";
import { useI18n } from "vue-i18n";
const props = defineProps<{
  model: Record<string, any>;
  field: Field;
  cssClass?: any;
}>();

const { t } = useI18n();
(function init() {
  const initValue = props.model[props.field.prop];
  if (!initValue || !Array.isArray(initValue)) {
    props.model[props.field.prop] = ["no-repeat", "no-repeat"];
  }
})();
</script>

<style lang="scss" scoped>
:deep(.el-form-item__content .el-color-picker) {
  display: flex;
  height: 40px;
  align-items: center;
}
</style>
