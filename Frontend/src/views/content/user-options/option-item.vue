<script lang="ts" setup>
import type { Schema } from "./user-options";
import ObjectOption from "./object-option.vue";
import ArrayOption from "./array-option.vue";
import { useI18n } from "vue-i18n";

const { t } = useI18n();

defineProps<{ schema: Schema; value: any; deletable?: boolean }>();
defineEmits<{
  (e: "update:model-value", value: any): void;
  (e: "delete"): void;
}>();
</script>

<template>
  <div v-if="schema.type == 'string'" class="flex gap-4 w-full">
    <el-input
      :model-value="value"
      @update:model-value="$emit('update:model-value', $event)"
    />
    <div v-if="deletable">
      <IconButton
        circle
        class="hover:text-orange text-orange"
        icon="icon-delete "
        :tip="t('common.delete')"
        @click="$emit('delete')"
      />
    </div>
  </div>

  <div v-else-if="schema.type == 'boolean'" class="flex gap-4 w-full">
    <el-switch
      :model-value="value"
      @update:model-value="$emit('update:model-value', $event)"
    />
    <div v-if="deletable">
      <IconButton
        circle
        class="hover:text-orange text-orange"
        icon="icon-delete "
        :tip="t('common.delete')"
        @click="$emit('delete')"
      />
    </div>
  </div>

  <div v-else-if="schema.type == 'number'" class="flex gap-4 w-full">
    <el-input-number
      :model-value="value"
      @update:model-value="$emit('update:model-value', $event)"
    />
    <div v-if="deletable">
      <IconButton
        circle
        class="hover:text-orange text-orange"
        icon="icon-delete "
        :tip="t('common.delete')"
        @click="$emit('delete')"
      />
    </div>
  </div>

  <div
    v-else-if="schema.type == 'object'"
    class="px-24 py-12 border-l-2 border-blue/70 relative transition-all duration-300 hover:shadow-m-10 rounded-tr-normal rounded-br-normal"
  >
    <ObjectOption
      :schemas="schema.children"
      :data="value"
      :deletable="deletable"
    />

    <ElIcon
      v-if="deletable"
      class="iconfont icon-delete cursor-pointer text-orange absolute top-16 right-16"
      @click="$emit('delete')"
    />
  </div>
  <div
    v-else-if="schema.type == 'array'"
    class="flex items-center gap-4 w-full"
  >
    <ArrayOption :schema="schema" :value="value" />
  </div>
</template>
