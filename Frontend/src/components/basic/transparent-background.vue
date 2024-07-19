<script lang="ts" setup>
import { nextTick, onBeforeMount, onMounted, ref } from "vue";

const props = defineProps<{
  color?: string;
  size?: number;
}>();

function paint() {
  if (!canvas.value) return;
  const ctx = canvas.value!.getContext("2d")!;
  let width = 0;
  let height = 0;
  let count = 0;
  const size = props.size ?? 12;

  while (width < canvas.value.width || height < canvas.value.height) {
    if (!(count % 2)) {
      ctx.fillStyle = props.color ?? "#D7D7D7";
      ctx.fillRect(width, height, size, size);
    }

    width += size;

    if (width > canvas.value.width) {
      width = 0;
      count = (height / size) % 2 ? 1 : 0;
      height += size;
      if (height > canvas.value.height) break;
    }

    count++;
  }
}

const canvas = ref<HTMLCanvasElement>();
const showCanvas = ref(false);
const observer = new ResizeObserver(async () => {
  if (!container.value) return;
  showCanvas.value = false;
  rect.value = container.value!.getBoundingClientRect();
  showCanvas.value = true;
  await nextTick();
  paint();
});
const container = ref<HTMLCanvasElement>();
const rect = ref<DOMRect>();

onMounted(() => {
  observer.observe(container.value!);
});

onBeforeMount(() => {
  observer.disconnect();
});
</script>
<template>
  <div ref="container" class="w-full h-full">
    <canvas
      v-if="showCanvas"
      ref="canvas"
      :width="rect?.width"
      :height="rect?.height"
      class="dark:bg-666"
    />
  </div>
</template>
