<template>
  <el-form-item
    :label="field.displayName ?? field.name"
    :prop="field.prop"
    :required="field.required"
    class="flex"
    :class="cssClass"
  >
    <el-input-number
      v-if="!hideValue"
      v-model="innerValue.value"
      style="flex: 2"
      v-bind="field.settings?.value"
    />
    <el-select
      v-model="innerValue.unit"
      :class="{ 'ml-8': !hideValue }"
      style="flex: 1"
      v-bind="field.settings?.unit"
      @change="onUnitChanged"
    >
      <el-option
        v-for="item in field.selectionOptions"
        :key="item.value"
        :label="item.key"
        :value="item.value"
      />
    </el-select>
  </el-form-item>
</template>

<script lang="ts" setup>
import type { Field } from "@/components/field-control/types";
import { computed, nextTick } from "vue";
import type { NumberUnit } from "../../../types";
import { cloneDeep, get } from "lodash-es";

const props = defineProps<{
  model: Record<string, any>;
  field: Field;
  cssClass?: any;
}>();

const innerValue = computed<NumberUnit>({
  get() {
    const defaultUnit = props.field.selectionOptions[0]?.value ?? "";
    let originValue = props.model[props.field.name] ?? {
      value: 0,
      unit: defaultUnit,
    };
    if (typeof originValue === "number") {
      originValue = {
        value: originValue,
        unit: defaultUnit,
      };
    }
    return originValue;
  },
  set(data) {
    const cloneData = cloneDeep(data);
    if (hideValue.value) {
      cloneData.value = null;
    }
    props.model[props.field.name] = data;
  },
});

const hideValue = computed(() => {
  const noValueUnits: string[] = get(
    props.field,
    `settings["noValueUnits"]`,
    []
  );
  return noValueUnits.includes(innerValue.value.unit);
});

function onUnitChanged() {
  nextTick(() => {
    if (hideValue.value) {
      innerValue.value.value = null;
    }
  });
}
</script>
