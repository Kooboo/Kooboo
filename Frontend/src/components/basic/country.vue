<script lang="ts" setup>
import { computed } from "vue";

const props = defineProps<{ nameOrCode?: string }>();

const flag = computed(() => {
  if (!props.nameOrCode || props.nameOrCode.length != 2) return "";
  const codePoints = props.nameOrCode
    .toUpperCase()
    .split("")
    .map((char) => 127397 + char.charCodeAt(0));
  return String.fromCodePoint(...codePoints);
});
</script>

<template>
  <div class="inline-flex justify-center space-x-4">
    <span>{{ flag }}</span>
    <span> {{ nameOrCode }}</span>
  </div>
</template>
