<script lang="ts" setup>
import { Close } from "@element-plus/icons-vue";
import { computed } from "@vue/reactivity";
import { getRect } from "../utils/dom";
import { doc, currentElement, offset } from "../page";
import { editing, endEdit } from ".";

defineProps<{
  color?: string;
}>();

const position = computed(() => getRect(currentElement.value));

const handleWheel = (e: WheelEvent) => {
  const documentElement = doc.value.documentElement as Element;
  if (!documentElement) return;
  documentElement.scrollTop += e.deltaY;
  documentElement.scrollLeft += e.deltaX;
};
</script>

<template>
  <div v-if="position && editing">
    <div
      class="absolute top-0 left-0 right-0"
      :style="{
        height: position.y + offset.y + 'px',
        left: position.x + offset.x + 'px',
        opacity: 0.5,
        backgroundColor: color || 'black',
      }"
      @wheel="handleWheel"
    />
    <div
      class="absolute bottom-0 right-0"
      :style="{
        left: position.x + offset.x + position.width + 'px',
        top: position.y + offset.y + 'px',
        opacity: 0.5,
        backgroundColor: color || 'black',
      }"
      @wheel="handleWheel"
    />
    <div
      class="absolute bottom-0 left-0"
      :style="{
        width: position.x + offset.x + position.width + 'px',
        top: position.y + offset.y + position.height + 'px',
        opacity: 0.5,
        backgroundColor: color || 'black',
      }"
      @wheel="handleWheel"
    />
    <div
      class="absolute top-0 left-0"
      :style="{
        width: position.x + offset.x + 'px',
        height: position.y + offset.y + position.height + 'px',
        opacity: 0.5,
        backgroundColor: color || 'black',
      }"
      @wheel="handleWheel"
    />
    <el-button
      circle
      bg
      type="info"
      class="absolute top-24 right-24"
      @click="endEdit(true)"
    >
      <el-icon color="white">
        <Close />
      </el-icon>
    </el-button>
  </div>
</template>
