<script lang="ts" setup>
import KEditor from "@/components/k-editor/index.vue";
import SelectMediaFile from "@/components/select-media-file/index.vue";
import SelectFile from "@/components/select-file/index.vue";
import type { CustomField } from "@/api/commerce/settings";
import ContentField from "./content-field.vue";
import { onMounted, ref } from "vue";
import KeyValueEditor from "@/components/basic/key-value-editor.vue";

interface PropsType {
  field: CustomField;
  modelValue: any;
}

const props = defineProps<PropsType>();
const show = ref(false);
const emit = defineEmits<{
  (e: "update:model-value", value: any): void;
}>();

if (props.field.multiple && !Array.isArray(props.modelValue)) {
  var list = [];
  if (props.modelValue !== undefined) {
    list.push(props.modelValue);
  }
  emit("update:model-value", list);
}

onMounted(() => {
  show.value = true;
});
</script>

<template>
  <template v-if="show">
    <el-input
      v-if="field.type === 'TextBox'"
      style="width: 100%"
      :model-value="modelValue"
      @update:model-value="$emit('update:model-value', $event)"
    />
    <el-input
      v-else-if="field.type === 'TextArea'"
      :model-value="modelValue"
      class="break-normal"
      type="textarea"
      :autosize="{ minRows: 8, maxRows: 15 }"
      style="width: 100%"
      @update:model-value="$emit('update:model-value', $event)"
    />
    <KEditor
      v-else-if="field.type === 'RichEditor'"
      :model-value="modelValue"
      :min_height="385"
      @update:model-value="$emit('update:model-value', $event)"
    />
    <el-select
      v-else-if="field.type === 'Selection'"
      :model-value="modelValue"
      clearable
      @update:model-value="$emit('update:model-value', $event)"
    >
      <el-option
        v-for="opt in field.selectionOptions"
        :key="opt.value"
        :value="opt.value"
        :label="opt.key"
      />
    </el-select>
    <el-checkbox-group
      v-else-if="field.type === 'CheckBox'"
      :model-value="modelValue"
      @update:model-value="$emit('update:model-value', $event)"
    >
      <el-checkbox
        v-for="opt in field.selectionOptions"
        :key="opt.value"
        size="large"
        :label="opt.value"
        >{{ opt.key }}</el-checkbox
      >
    </el-checkbox-group>
    <el-radio-group
      v-else-if="field.type === 'RadioBox'"
      :model-value="modelValue"
      @update:model-value="$emit('update:model-value', $event)"
    >
      <el-radio
        v-for="opt in field.selectionOptions"
        :key="opt.value"
        size="large"
        :label="opt.value"
        >{{ opt.key }}</el-radio
      >
    </el-radio-group>
    <el-switch
      v-else-if="field.type === 'Switch'"
      :model-value="modelValue"
      @update:model-value="$emit('update:model-value', $event)"
    />
    <el-date-picker
      v-else-if="field.type === 'DateTime'"
      :model-value="modelValue"
      type="datetime"
      popper-class="filed-control-el-date-picker"
      @update:model-value="$emit('update:model-value', $event)"
    />
    <el-color-picker
      v-else-if="field.type === 'ColorPicker'"
      :model-value="modelValue"
      popper-class="filed-control-el-color-picker"
      show-alpha
      @update:model-value="$emit('update:model-value', $event)"
    />
    <el-input-number
      v-else-if="field.type === 'Number'"
      :model-value="modelValue"
      controls-position="right"
      @update:model-value="$emit('update:model-value', $event)"
    />
    <SelectMediaFile
      v-else-if="field.type === 'MediaFile'"
      :model-value="modelValue"
      :multiple="field.multiple"
      :validations="field.validations"
      @update:model-value="$emit('update:model-value', $event)"
    />
    <SelectFile
      v-else-if="field.type === 'File'"
      :model-value="modelValue"
      :multiple="field.multiple"
      @update:model-value="$emit('update:model-value', $event)"
    />
    <ContentField
      v-else-if="field.type === 'Content'"
      :model-value="modelValue || []"
      :content-folder="field.contentFolder!"
      :multiple="field.multiple"
      :allow-repetition="field.allowRepetition"
      @update:model-value="$emit('update:model-value', $event)"
    />
    <KeyValueEditor
      v-else-if="field.type === 'KeyValues'"
      :model-value="modelValue || []"
      @update:model-value="$emit('update:model-value', $event)"
    />
  </template>
</template>
<style>
.filed-control-el-date-picker .el-picker-panel__icon-btn {
  margin-top: 0;
}
</style>
