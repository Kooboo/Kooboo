<script lang="ts" setup>
import { ref, watch, computed } from "vue";
import type { Action } from "./types";
import { selectionChangeEvent } from "../page";

const props = defineProps<{ action: Action }>();

const selections = [
  {
    key: 1,
    value: "10px",
  },
  {
    key: 2,
    value: "13px",
  },
  {
    key: 3,
    value: "16px",
  },
  {
    key: 4,
    value: "18px",
  },
  {
    key: 5,
    value: "24px",
  },
  {
    key: 6,
    value: "32px",
  },
  {
    key: 7,
    value: "48px",
  },
];

const selected = ref(3);

const display = computed(() => {
  const item = selections.find((f) => f.key == selected.value);
  if (item) return item.value;
  else return "16px";
});

watch(
  () => selectionChangeEvent.value,
  () => {
    if (!props.action.active) return;
    selected.value = parseInt(props.action.active());
  },
  { immediate: true, deep: true }
);
</script>

<template>
  <el-dropdown trigger="click" @command="action.invoke($event)">
    <div
      class="text-black rounded-normal p-4 hover:bg-blue/10 flex items-center cursor-pointer"
    >
      <div class="h-24 flex items-center space-x-4 px-4">
        <span>{{ display }}</span>
        <el-icon class="iconfont icon-pull-down text-s leading-none" />
      </div>
    </div>
    <template #dropdown>
      <el-dropdown-menu>
        <el-dropdown-item
          v-for="item of selections"
          :key="item.key"
          :command="item.key"
        >
          <div>{{ item.value }}</div>
        </el-dropdown-item>
      </el-dropdown-menu>
    </template>
  </el-dropdown>
</template>
