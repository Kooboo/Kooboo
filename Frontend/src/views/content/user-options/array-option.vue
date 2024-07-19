<script lang="ts" setup>
import OptionItem from "./option-item.vue";
import type { Type } from "./user-options";
import { getDefaultValue, type Schema } from "./user-options";
import { useI18n } from "vue-i18n";

const props = defineProps<{ schema: Schema; value: any[] }>();
const { t } = useI18n();

function addItem(array: any[], type: Type) {
  array.push(getDefaultValue(type));
}

function updateItemValue(index: number, value: any) {
  props.value.splice(index, 1, value);
}
</script>

<template>
  <div class="space-y-8">
    <OptionItem
      v-for="(item, index) of value"
      :key="index"
      :value="item"
      :schema="{
        type: schema.arrayType!,
        children: schema.children,
        display: schema.display,
        name: schema.name,
      }"
      :deletable="true"
      @update:model-value="updateItemValue(index, $event)"
      @delete="value.splice(index, 1)"
    />
    <IconButton
      circle
      class="text-blue"
      icon="icon-a-addto "
      :tip="t('common.add')"
      @click="addItem(value, schema.arrayType!)"
    />
  </div>
</template>
