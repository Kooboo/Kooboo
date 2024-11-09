<script lang="ts" setup>
import { ref, watch } from "vue";

const props = defineProps<{
  modelValue: string;
  editing: boolean;
  readonly?: boolean;
  options?: string[];
  optionDeletable?: boolean;
  size?: string;
  labelFormatter?: (key: string) => string;
}>();

const copyValue = ref(props.modelValue);
const popover = ref();

const emit = defineEmits<{
  (e: "update:model-value", value: string): void;
  (e: "update:editing", value: boolean): void;
  (e: "delete"): void;
  (e: "delete-option", value: string): void;
}>();

function onSave() {
  var value = copyValue.value.trim();
  if (value) {
    emit("update:model-value", value);
  } else {
    emit("delete");
  }
  emit("update:editing", false);
}

function onCancel() {
  copyValue.value = props.modelValue;
  emit("update:editing", false);
  if (!props.modelValue.trim()) {
    emit("delete");
  }
}

watch(
  () => props.editing,
  (value) => {
    if (!value) copyValue.value = props.modelValue;
  }
);

function onSelected(value: string) {
  copyValue.value = value;
  popover.value?.hide();
  onSave();
}

function formatLabel(key: string) {
  return typeof props.labelFormatter === "function"
    ? props.labelFormatter(key)
    : key;
}
</script>

<template>
  <div v-if="editing && !readonly" class="flex items-center space-x-8">
    <el-popover
      ref="popover"
      :disabled="!options?.length"
      placement="top-start"
      trigger="click"
      width="auto"
      content="this is content, this is content, this is content"
    >
      <div class="flex gap-4 flex-wrap max-w-500px">
        <ElTag
          v-for="item of options"
          :key="item"
          class="cursor-pointer"
          :closable="optionDeletable"
          @click="onSelected(item)"
          @close="$emit('delete-option', item)"
        >
          {{ formatLabel(item) }}
        </ElTag>
      </div>
      <template #reference>
        <ElInput
          v-model="copyValue"
          size="small"
          @keydown.enter.stop="onSave"
        />
      </template>
    </el-popover>

    <el-icon
      class="iconfont icon-yes3 text-green cursor-pointer"
      @click.stop="onSave"
    />
    <el-icon
      class="iconfont icon-close text-orange cursor-pointer"
      @click.stop="onCancel"
    />
  </div>
  <ElTag v-else type="warning" :size="size">
    <div class="space-x-8 flex items-center">
      <div
        class="cursor-pointer flex items-center space-x-8"
        @click="emit('update:editing', true)"
      >
        <slot name="option" :option="formatLabel(modelValue)" />
        <div>{{ formatLabel(modelValue) }}</div>
      </div>
      <el-icon
        v-if="!readonly"
        class="iconfont icon-delete"
        @click="emit('delete')"
      />
    </div>
  </ElTag>
</template>
