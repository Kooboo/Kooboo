<script lang="ts" setup>
import { offset } from "@/views/inline-design/page";

defineProps<{
  position: DOMRect;
  color?: string;
  width?: number;
}>();
</script>

<template>
  <div
    class="absolute pointer-events-none"
    :style="{
      top: position.y + offset.y + 'px',
      left: position.x + offset.x + 'px',
      width: position.width + 'px',
      height: position.height + 'px',
      outlineStyle: 'solid',
      outlineColor: color || 'red',
      outlineWidth: (width || 1) + 'px',
    }"
  />
</template>
