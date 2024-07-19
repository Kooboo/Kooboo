<template>
  <div class="mt-16">
    <div class="font-bold py-5px">{{ t("common.preview") }}</div>
    <div
      class="bg-gray/60 rounded-4px p-10px pointer-events-none dark:bg-[#333]"
      data-cy="preview"
    >
      <el-form label-position="top">
        <FieldPreview :field="field" :model="model" />
      </el-form>
    </div>
  </div>
</template>
<script setup lang="ts">
import type { Property } from "@/global/control-type";
import FieldPreview from "./field-preview.vue";
import { useControlTypes } from "@/hooks/use-control-types";
import { computed } from "vue";

import { useI18n } from "vue-i18n";
import { cloneDeep } from "lodash-es";
interface PropsType {
  field: Property;
}
const props = defineProps<PropsType>();
const { t } = useI18n();

const { getControlType } = useControlTypes();
const controlModel = computed(() => getControlType(props.field.controlType));
const model = computed(() => {
  let defaultValue: unknown = "";
  if (controlModel.value) {
    if (controlModel.value.value === "Switch") {
      defaultValue = false;
    } else if (controlModel.value.value === "CheckBox") {
      defaultValue = [];
    }
  }

  return {
    [props.field.name]: defaultValue,
  };
});
</script>

<style lang="scss" scoped>
:deep(.el-form-item) {
  margin-bottom: 0;
}
:deep(.el-input),
:deep(.el-textarea),
:deep(.el-select),
:deep(.el-input-number) {
  width: 100%;
}
</style>
