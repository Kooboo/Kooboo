<script lang="ts" setup>
import type { KeyValue } from "@/global/types";
import { toList, toObject } from "@/utils/lang";
import { ref, watch } from "vue";

import { useI18n } from "vue-i18n";
const props = defineProps<{ modelValue: Record<string, string> }>();
const emit = defineEmits<{
  (e: "update:modelValue", value: Record<string, unknown>): void;
}>();
const { t } = useI18n();

const list = ref<KeyValue[]>(toList(props.modelValue));

const onRemove = (index: number) => {
  list.value.splice(index, 1);
};

watch(
  () => list.value,
  () => {
    emit("update:modelValue", toObject(list.value));
  },
  { deep: true }
);
</script>

<template>
  <div class="px-24 py-16 space-y-4">
    <div
      v-for="(item, index) in list"
      :key="index"
      class="flex items-center space-x-4"
    >
      <el-input
        v-model="item.key"
        :placeholder="t('common.key')"
        data-cy="key"
      />
      <el-input
        v-model="item.value"
        :placeholder="t('common.value')"
        data-cy="value"
      />
      <div>
        <el-button circle data-cy="remove" @click="onRemove(index)">
          <el-icon class="text-orange iconfont icon-delete" />
        </el-button>
      </div>
    </div>
    <el-button circle data-cy="add" @click="list.push({ key: '', value: '' })">
      <el-icon class="text-blue iconfont icon-a-addto" />
    </el-button>
  </div>
</template>
