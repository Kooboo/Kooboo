<script lang="ts" setup>
import EditableTag from "./editable-tag.vue";
import { computed, ref } from "vue";
import { errorMessage } from "./message";
import { useI18n } from "vue-i18n";
import { pullAll } from "lodash-es";

const { t } = useI18n();
const props = defineProps<{
  modelValue: string[];
  readonly?: boolean;
  addLabel?: string;
  options?: string[];
  optionDeletable?: boolean;
  size?: string;
  labelFormatter?: (key: string) => string;
}>();

const list = computed(() => {
  return (
    props.modelValue?.map((m) => ({
      key: m,
      value: m,
    })) ?? []
  );
});

const availableOptions = computed(() => {
  return pullAll((props.options ?? []).slice(), props.modelValue ?? []);
});

const emit = defineEmits<{
  (e: "update:model-value", value: string[]): void;
  (e: "remove-item", index: number): void;
  (e: "add-item", value: string): void;
  (e: "change-item", index: number, value: string): void;
  (e: "delete-option", value: string): void;
}>();

const editingItem = ref<number>();

function onAdd(newItem: string) {
  if (
    props.modelValue
      .map((it) => it.toLowerCase())
      .includes(newItem.toLowerCase())
  ) {
    errorMessage(t("common.valueExist"));
    return;
  }
  emit("update:model-value", [...props.modelValue, newItem]);
  emit("add-item", newItem);
  editingItem.value = undefined;
}

function onEditingChange(editing: boolean, index: number) {
  editingItem.value = editing ? index : undefined;
}

function onDelete(index: number) {
  emit("remove-item", index);
  const newModelValue = [...props.modelValue];
  newModelValue.splice(index, 1);
  emit("update:model-value", newModelValue);
}

function onUpdateValue(index: number, value: string) {
  var oldValue = props.modelValue[index];
  if (oldValue == value) return;
  if (
    props.modelValue.map((it) => it.toLowerCase()).includes(value.toLowerCase())
  ) {
    errorMessage(t("common.valueExist"));
    return;
  }
  emit("change-item", index, value);
  const newModelValue = [...props.modelValue];
  newModelValue.splice(index, 1, value);
  emit("update:model-value", newModelValue);
}
</script>

<template>
  <div class="flex flex-wrap gap-4">
    <EditableTag
      v-for="(item, index) in list"
      :key="item.key"
      :model-value="item.value"
      :editing="index == editingItem"
      :readonly="readonly"
      :options="availableOptions"
      :option-deletable="optionDeletable"
      :label-formatter="labelFormatter"
      :size="size"
      @update:model-value="onUpdateValue(index, $event)"
      @update:editing="onEditingChange($event, index)"
      @delete="onDelete(index)"
      @delete-option="$emit('delete-option', $event)"
    >
      <template #option="{ option }">
        <slot name="option" :option="option" />
      </template>
    </EditableTag>
    <template v-if="!readonly">
      <EditableTag
        v-if="editingItem == -1"
        :model-value="''"
        editing
        :readonly="readonly"
        :options="availableOptions"
        :option-deletable="optionDeletable"
        :label-formatter="labelFormatter"
        :size="size"
        @update:model-value="onAdd"
        @update:editing="onEditingChange($event, -1)"
        @delete-option="$emit('delete-option', $event)"
      >
        <template #option="{ option }">
          <slot name="option" :option="option" />
        </template>
      </EditableTag>
      <ElTag
        v-if="editingItem == undefined"
        type="success"
        class="cursor-pointer"
        :size="size"
        @click="editingItem = -1"
      >
        <el-icon class="iconfont icon-a-addto" />
        {{ addLabel }}
      </ElTag>
    </template>
    <slot />
  </div>
</template>
