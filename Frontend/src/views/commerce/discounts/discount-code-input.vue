<script lang="ts" setup>
import { newGuid } from "@/utils/guid";
import { useI18n } from "vue-i18n";

const props = defineProps<{
  modelValue: string;
  auto?: boolean;
}>();

const emit = defineEmits<{
  (e: "update:model-value", value: string): void;
}>();

const { t } = useI18n();

function generateCode() {
  emit("update:model-value", newGuid().substring(0, 6).toUpperCase());
}

if (props.auto && !props.modelValue) generateCode();
</script>

<template>
  <div class="flex space-x-4">
    <ElInput
      v-model="modelValue"
      class="w-280px"
      @update:model-value="emit('update:model-value', $event)"
    />
    <ElButton v-if="!auto" type="primary" @click="generateCode">{{
      t("common.generate")
    }}</ElButton>
  </div>
</template>
