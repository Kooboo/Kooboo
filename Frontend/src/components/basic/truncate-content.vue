<script lang="ts" setup>
import { onMounted, onUnmounted, ref } from "vue";

defineProps<{
  tip: string;
}>();
const container = ref();
const enableTip = ref(false);

function computeTruncate() {
  setTimeout(() => {
    if (!container.value) return;
    enableTip.value = container.value.scrollWidth > container.value.offsetWidth;
  }, 10);
}

const observer = new MutationObserver(computeTruncate);
onMounted(() => {
  observer.observe(container.value, { childList: true, subtree: true });
  computeTruncate();
});

onUnmounted(() => {
  observer.disconnect();
});
</script>

<template>
  <el-tooltip
    :disabled="!enableTip"
    :content="tip"
    placement="top"
    popper-class="max-w-504px"
  >
    <div ref="container" class="truncate" v-bind="$attrs">
      <slot />
    </div>
  </el-tooltip>
</template>
