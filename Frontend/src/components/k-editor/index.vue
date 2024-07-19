<template>
  <Editor v-bind="$attrs" :key="key" />
</template>

<script lang="ts" setup>
import { dark } from "@/composables/dark";
import Editor from "./editor.vue";
import { ref, watch, useAttrs } from "vue";
const attrs = useAttrs();
const key = ref(Date.now().toString());
watch(
  [() => dark.value, () => attrs.min_height, () => attrs.max_height],
  () => {
    key.value = Date.now().toString();
  }
);
</script>
