<template>
  <KMediaDialog
    v-if="props.modelValue"
    v-model="props.modelValue"
    folder-type="File"
    v-bind="$attrs"
  />
</template>
<script setup lang="ts">
import KMediaDialog from "@/components/k-media-dialog";

const props = defineProps<{
  modelValue: boolean;
}>();
</script>
