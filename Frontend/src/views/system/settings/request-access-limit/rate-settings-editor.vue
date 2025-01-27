<script lang="ts" setup>
import { useI18n } from "vue-i18n";
import RateSettingsItem from "./rate-settings-item.vue";
import type { ReteSettings } from "@/api/site/site";

const props = defineProps<{
  modelValue: { key: string; value: ReteSettings }[];
  placeholder: string;
}>();
const emit = defineEmits<{
  (
    e: "update:model-value",
    value: { key: string; value: ReteSettings }[]
  ): void;
}>();

const { t } = useI18n();

function add() {
  emit("update:model-value", [
    ...props.modelValue,
    {
      key: "",
      value: { permitLimit: 10, withinSeconds: 3 },
      editing: true,
    } as any,
  ]);
}

function remove(item: any) {
  emit(
    "update:model-value",
    props.modelValue.filter((f) => f != item)
  );
}
</script>

<template>
  <div class="space-y-8 w-full">
    <ElCard
      v-for="(item, index) of modelValue"
      :key="index"
      class="relative mb-8 w-500px"
      :shadow="false"
    >
      <RateSettingsItem
        :placeholder="placeholder"
        :item="item"
        @remove="remove(item)"
      />
    </ElCard>
    <IconButton
      circle
      class="text-blue"
      icon="icon-a-addto"
      :tip="t('common.add')"
      @click="add"
    />
  </div>
</template>
