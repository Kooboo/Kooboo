<script lang="ts" setup>
import { withDefaults, computed } from "vue";

interface Props {
  align?: "center" | "end" | "start";
}

const props = withDefaults(defineProps<Props>(), {
  align: "center",
});

const alignItems = computed(() => {
  switch (props.align) {
    case "end":
      return "flex-end";
    case "start":
      return "flex-start";
    default:
      return "center";
  }
});
</script>

<template>
  <div class="flex space-x-8" :style="{ alignItems: alignItems }">
    <slot />
  </div>
</template>
