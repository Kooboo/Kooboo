<script setup lang="ts">
import { computed, withDefaults } from "vue";

interface Props {
  username: string;
  size: number;
}
const props = withDefaults(defineProps<Props>(), {
  username: "",
  size: 0,
});

const firstChar = computed(() => Array.from(props.username).shift());
</script>

<template>
  <div
    class="bg-blue rounded-full flex justify-center items-center text-fff leading-1 uppercase overflow-hidden"
    :style="{
      width: `${size}px`,
      height: `${size}px`,
      fontSize: `${size * 0.625}px`,
    }"
  >
    {{ firstChar }}
  </div>
</template>
