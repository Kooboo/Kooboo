<script lang="ts" setup>
import type { Action } from "./types";
import { ref, watch } from "vue";
import { selectionChangeEvent } from "../page";

const props = defineProps<{ action: Action }>();
const active = ref(false);

watch(
  () => selectionChangeEvent.value,
  () => {
    if (!props.action.active) return;
    active.value = props.action.active();
  },
  {
    immediate: true,
    deep: true,
  }
);
</script>

<template>
  <div
    class="rounded-normal p-4 hover:bg-blue/10 flex items-center cursor-pointer"
    :class="active ? 'bg-blue/10' : ''"
    @click="action.invoke()"
  >
    <el-icon class="iconfont w-24 h-24" :class="action.icon" />
  </div>
</template>
