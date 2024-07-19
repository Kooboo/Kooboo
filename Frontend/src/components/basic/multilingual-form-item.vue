<script lang="ts" setup>
import { useMultilingualStore } from "@/store/multilingual";

const props = defineProps<{
  defaultModel?: any;
  multilingualModel: Record<string, any>;
  label: string;
  rules?: any;
  defaultProp?: string;
  multilingualProp?: string;
}>();

const emit = defineEmits<{
  (e: "update:default-model", value: any): void;
}>();
const multilingualStore = useMultilingualStore();
emit("update:default-model", getDefaultValue());

function onChangeDefaultValue(value: any) {
  emit("update:default-model", value);
  props.multilingualModel[multilingualStore.default] = value;
}

function onChangeLangValue(lang: string, value: any) {
  var isDefault = lang == multilingualStore.default;
  if (isDefault) {
    emit("update:default-model", value);
  }
  props.multilingualModel[lang] = value;
}

function getLangValue(lang: string) {
  var model = props.multilingualModel;
  var isDefault = lang == multilingualStore.default;
  return model[lang] ?? (isDefault ? props.defaultModel : undefined);
}

function getDefaultValue() {
  if (props.defaultModel != undefined) return props.defaultModel;
  var model = props.multilingualModel;
  return model[multilingualStore.default];
}
</script>

<template>
  <template v-if="multilingualStore.visible">
    <ElFormItem
      v-for="(item, key) of multilingualStore.cultures"
      v-show="multilingualStore.selected.includes(key)"
      :key="key"
      :label="multilingualStore.appendLangText(label, key)"
      :prop="`${multilingualProp}.${key}`"
      :rules="rules"
    >
      <slot
        :value="getLangValue(key)"
        :onchange="(value: any) => onChangeLangValue(key, value)"
      />
    </ElFormItem>
  </template>
  <ElFormItem v-else :label="label" :prop="defaultProp" :rules="rules">
    <slot :value="getDefaultValue()" :onchange="onChangeDefaultValue" />
  </ElFormItem>
</template>
