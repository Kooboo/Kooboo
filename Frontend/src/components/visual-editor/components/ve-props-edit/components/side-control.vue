<template>
  <el-form-item :prop="field.prop" :required="field.required" class="w-full">
    <template #label>
      <div class="flex w-full justify-between items-center">
        <div>{{ field.displayName ?? field.name }}</div>
        <div class="flex justify-between items-center">
          <div class="mr-12">{{ t("ve.moreOptions") }}</div>
          <el-switch v-model="vm.moreOptions" @change="onOptionChanged" />
        </div>
      </div>
    </template>
  </el-form-item>
  <template v-if="vm.moreOptions">
    <div class="w-full flex justify-between items-center">
      <el-form-item :label="t('ve.top')">
        <el-input-number v-model="vm.top" />
      </el-form-item>
      <el-form-item :label="t('ve.right')">
        <el-input-number v-model="vm.right" />
      </el-form-item>
    </div>
    <div class="w-full flex justify-between items-center">
      <el-form-item :label="t('ve.bottom')">
        <el-input-number v-model="vm.bottom" />
      </el-form-item>
      <el-form-item :label="t('ve.left')">
        <el-input-number v-model="vm.left" />
      </el-form-item>
    </div>
  </template>
  <el-form-item v-else :label="t('common.all')">
    <el-input-number v-model="vm.all" />
  </el-form-item>
</template>

<script lang="ts" setup>
import type { Field } from "@/components/field-control/types";
import { watch, ref } from "vue";
import { useI18n } from "vue-i18n";
import type { PaddingOptions } from "../../../types";

const { t } = useI18n();
const props = defineProps<{
  model: Record<string, any>;
  field: Field;
  cssClass?: any;
}>();

const vm = ref<PaddingOptions>({
  moreOptions: false,
  all: 0,
});

function init() {
  let json: string = props.model[props.field.name];
  if (typeof json === "string") {
    vm.value = JSON.parse(json);
  }
}
init();

watch(
  () => vm.value,
  (data) => {
    props.model[props.field.name] = JSON.stringify(data);
  },
  {
    deep: true,
  }
);
function onOptionChanged() {
  if (vm.value.moreOptions) {
    const all = vm.value.all;
    vm.value.top = all;
    vm.value.right = all;
    vm.value.bottom = all;
    vm.value.left = all;
  } else {
    vm.value.all = vm.value.top;
  }
}
</script>
