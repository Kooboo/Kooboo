<template>
  <FieldControl :field="mapField" :model="model" :rules="rules" />
</template>

<script lang="ts" setup>
import { computed } from "vue";
import type { Field } from "@/components/field-control/types";
import FieldControl from "@/components/field-control/index.vue";
import type { Property } from "@/global/control-type";
import { useControlTypes } from "@/hooks/use-control-types";
import type { FormItemRule } from "element-plus";

interface PropsType {
  field: Property;
  model: Record<string, any>;
  overrideField?: Partial<Field>;
  rules?: FormItemRule[];
}

const { getControlType } = useControlTypes();
const controlModel = computed(() => getControlType(props.field.controlType));
const props = defineProps<PropsType>();

const mapField = computed<Field>(() => {
  return {
    lang: "",
    name: props.field.name,
    displayName: props.field.displayName || props.field.name,
    multipleValue: props.field.multipleValue,
    isMultilingual: false,
    prop: props.field.name,
    toolTip: props.field.tooltip || "",
    settings: props.field.settings ?? {},
    selectionOptions:
      controlModel.value?.value === "Selection"
        ? []
        : props.field.selectionOptions,
    controlType: props.field.controlType,
    ...props.overrideField,
  };
});
</script>
