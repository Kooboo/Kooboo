<script lang="ts" setup>
import type { Position } from "@/views/inline-design/types";
import { ref, watch } from "vue";

const props = defineProps<{ handler: HTMLElement; init: Position }>();
const state = ref<"start" | "end">("end");
const container = ref<HTMLDivElement>();
const offset = ref<Position>({ x: 0, y: 0 });
const position = ref<Position>(props.init);
var img = new Image();
img.src =
  "data:image/gif;base64,R0lGODlhAQABAIAAAAAAAP///yH5BAEAAAAALAAAAAABAAEAAAIBRAA7";

const dragStart = (e: DragEvent) => {
  state.value = "start";
  const transfer = e.dataTransfer as any;
  transfer.setDragImage(img, 0, 0);
  transfer.effectAllowed = "none";
  const containerRect = container.value!.getBoundingClientRect();
  offset.value = { x: e.x - containerRect.x, y: e.y - containerRect.y };
};

const dragOver = (e: DragEvent) => {
  e.preventDefault();
  position.value.x = e.x - offset.value.x;
  position.value.y = e.y - offset.value.y;
};

watch(
  () => props.handler,
  () => {
    if (!props.handler) return;
    props.handler.draggable = true;
    props.handler.ondragstart = dragStart;
    props.handler.ondragover = dragOver;
    props.handler.ondragend = () => (state.value = "end");
  },
  {
    immediate: true,
  }
);

watch(
  () => props.init,
  () => (position.value = props.init)
);
</script>
<template>
  <div
    class="fixed"
    :class="state !== 'end' ? 'inset-0' : ''"
    @dragover="dragOver"
  />
  <div
    ref="container"
    class="fixed"
    :style="{ top: position.y + 'px', left: position.x + 'px' }"
  >
    <slot />
  </div>
</template>
