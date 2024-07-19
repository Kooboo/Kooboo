<template>
  <div class="px-24 py-16">
    <el-form
      ref="form"
      label-position="top"
      :model="model"
      :rules="rules"
      @submit.prevent
    >
      <template v-for="item in fields" :key="item.name">
        <component
          :is="getFieldControl(item)"
          :class="widgetClass(item)"
          :field="item"
          :model="model"
        />
      </template>
    </el-form>
  </div>
</template>

<script lang="ts" setup>
import type { Field } from "@/components/field-control/types";
import { getFieldControl } from "./effects";
import { ref, nextTick } from "vue";
import { kebabCase } from "lodash-es";
import type { FormRules } from "element-plus";

defineProps<{
  model: Record<string, any>;
  fields: Field[];
  rules?: FormRules;
}>();
const form = ref();
function widgetClass(item: Field) {
  return kebabCase(`ve-${item.controlType}`);
}
async function validate() {
  return new Promise((resolve) => {
    nextTick(async () => {
      await form.value?.validate();
      resolve(true);
    });
  });
}

defineExpose({
  validate,
});
</script>
