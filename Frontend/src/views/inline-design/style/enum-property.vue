<script lang="ts" setup>
import type { KeyValue } from "@/global/types.js";
import { computed } from "vue";

const props = defineProps<{ options: KeyValue[] }>();

const list = computed(() => {
  return [{ key: "", value: "" }, ...props.options];
});
</script>

<template>
  <div>
    <el-select
      v-bind="$attrs"
      filterable
      allow-create
      default-first-option
      class="w-full"
    >
      <el-option
        v-for="item of list"
        :key="item.key"
        :label="item.value"
        :value="item.key"
      />
    </el-select>
  </div>
</template>
