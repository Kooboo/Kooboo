<script lang="ts" setup>
import { ref } from "vue";
import { getRect } from "../utils/dom";
import { createActions } from "./actions";
import Draggable from "@/components/basic/draggable.vue";
import { computed } from "@vue/reactivity";
import { currentElement, offset } from "../page";

const rect = getRect(currentElement.value);
const barElement = ref<HTMLElement>();
const handleElement = ref<HTMLElement>();
const actions = createActions();

const position = computed(() => {
  let barHeight = 40;

  if (barElement.value) {
    barHeight = getRect(barElement.value)!.height;
  }

  let y = rect!.y + offset.value.y - barHeight;
  let x = rect!.x + offset.value.x;
  y = y < 0 ? 0 : y;
  x = x < 0 ? 0 : x;
  return { x, y };
});
</script>
<template>
  <Draggable :handler="handleElement!" :init="position">
    <div
      ref="barElement"
      class="shadow-s-10 bg-fff py-4 p-x-8 rounded-normal flex items-center fixed text-m"
    >
      <div ref="handleElement" class="rounded-normal py-4 px-8 cursor-move">
        <el-icon class="iconfont icon-move" />
      </div>
      <template v-for="item of actions" :key="item.name">
        <el-divider v-if="item.divider" direction="vertical" />
        <component :is="item.component" :action="item" :title="item.display" />
      </template>
    </div>
  </Draggable>
</template>
