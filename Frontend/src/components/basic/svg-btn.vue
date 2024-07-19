<script lang="ts" setup>
const props = defineProps<{ src: string; disabled?: boolean }>();
const emit = defineEmits<{ (e: "click"): void }>();

function onClick() {
  if (props.disabled) return;
  emit("click");
}
</script>

<template>
  <div
    class="hover:bg-blue/10 cursor-pointer w-24 h-24 p-4 rounded-4px"
    :class="{
      'opacity-30': disabled,
      'hover:bg-blue/0': disabled,
      'cursor-not-allowed': disabled,
    }"
    v-bind="$attrs"
    @click="onClick"
  >
    <img :src="src" />
  </div>
</template>
