<template>
  <div class="px-16 py-8">
    <EditableTags
      v-model="innerValue"
      :options="tinymceFontSizes"
      @update:model-value="onChanged"
    />
  </div>
</template>

<script lang="ts" setup>
import { tinymceFontSizes } from "@/global/style";
import { ref, watch } from "vue";
import EditableTags from "@/components/basic/editable-tags.vue";
import { isEqual } from "lodash-es";

interface PropsType {
  modelValue?: string;
}

const props = defineProps<PropsType>();

interface EmitType {
  (e: "update:modelValue", data: string): void;
}

const emit = defineEmits<EmitType>();

const innerValue = ref<string[]>([]);

const init = () => {
  innerValue.value = props.modelValue
    ? props.modelValue.split(" ")
    : tinymceFontSizes.slice();
};
init();
watch(() => props.modelValue, init);
const onChanged = (values: string[]) => {
  const sortedValues = values.sort();
  if (isEqual(sortedValues, tinymceFontSizes)) {
    emit("update:modelValue", "");
  } else {
    emit("update:modelValue", sortedValues.join(" "));
  }
};
</script>
