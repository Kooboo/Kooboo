<script lang="ts" setup>
import { useI18n } from "vue-i18n";

const props = defineProps<{
  modelValue: string[];
  placeholder?: string;
}>();

const emit = defineEmits<{
  (e: "update:model-value", value: string[]): void;
}>();

const { t } = useI18n();

function onDelete(index: number) {
  const list = [...props.modelValue];
  list.splice(index, 1);
  emit("update:model-value", list);
}

function onAdd() {
  emit("update:model-value", [...props.modelValue, ""]);
}

function onUpdateItem(value: string, index: number) {
  const list = [...props.modelValue];
  list[index] = value;
  emit("update:model-value", list);
}
</script>

<template>
  <div class="space-y-4!">
    <div
      v-for="(item, index) of modelValue"
      :key="index"
      class="flex items-center space-x-4 mb-16"
    >
      <el-input
        class="flex-1"
        :model-value="item"
        :placeholder="placeholder"
        @input="onUpdateItem($event, index)"
      />
      <div>
        <IconButton
          circle
          class="hover:text-orange text-orange"
          icon="icon-delete "
          :tip="t('common.delete')"
          @click="onDelete(index)"
        />
      </div>
    </div>
    <IconButton
      circle
      class="text-blue"
      icon="icon-a-addto "
      :tip="t('common.add')"
      @click="onAdd"
    />
  </div>
</template>
