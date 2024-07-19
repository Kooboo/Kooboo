<template>
  <div class="px-16 py-8">
    <EditableTags
      v-model="innerValue"
      :options="defaultValues"
      @update:model-value="onChanged"
    />
  </div>
</template>

<script lang="ts" setup>
import { tinymceFonts } from "@/global/style";
import { ref, watch } from "vue";
import EditableTags from "@/components/basic/editable-tags.vue";
import { isEqual, sortBy } from "lodash-es";

interface PropsType {
  modelValue?: string;
}

const props = defineProps<PropsType>();
const fontMaps: Record<string, string> = {};
for (const it of tinymceFonts) {
  const ix = it.indexOf("=");
  const name = it.substring(0, ix);
  fontMaps[name] = it;
}

const defaultValues = ref<string[]>(
  sortBy(Object.keys(fontMaps), [(key) => key.toLowerCase()])
);

interface EmitType {
  (e: "update:modelValue", data: string): void;
}

const emit = defineEmits<EmitType>();

const innerValue = ref<string[]>([]);

const init = () => {
  innerValue.value = props.modelValue
    ? props.modelValue.split(";").map((it) => it.split("=")[0])
    : Object.keys(fontMaps);
};
init();
watch(() => props.modelValue, init);
const onChanged = (values: string[]) => {
  const sortedValues = sortBy(values, [(key) => key.toLowerCase()]);
  if (isEqual(sortedValues, defaultValues.value)) {
    emit("update:modelValue", "");
  } else {
    const fullValue = sortedValues
      .map((it) => fontMaps[it] || `${it}=${it}`)
      .join(";");
    emit("update:modelValue", fullValue);
  }
};
</script>
