<script lang="ts" setup>
import type { Schema, Type } from "./user-options";
import OptionItem from "./option-item.vue";
import { getDefaultValue } from "./user-options";

defineProps<{ schemas: Schema[]; data: any; deletable?: boolean }>();

defineEmits<{
  (e: "update:model-value", value: any): void;
  (e: "delete"): void;
}>();

function getOption(obj: any, name: string, type: Type) {
  if (obj[name]) return obj[name];
  obj[name] = getDefaultValue(type);
  return obj[name];
}

function updateOrCreateOption(obj: any, name: string, value: any) {
  obj[name] = value;
}
</script>

<template>
  <el-form-item
    v-for="item of schemas"
    :key="item.name"
    :label="item.display || item.name"
    class="w-504px"
  >
    <OptionItem
      :key="item.name"
      :schema="item"
      :value="getOption(data, item.name, item.type)"
      @update:model-value="updateOrCreateOption(data, item.name, $event)"
    />
  </el-form-item>
</template>
