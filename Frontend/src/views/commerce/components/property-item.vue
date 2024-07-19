<script lang="ts" setup>
defineProps<{
  name: string;
  prefix?: string;
  append?: string;
}>();
</script>

<template>
  <div class="flex items-center space-x-8">
    <div class="space-x-2px dark:text-[#cfd3dc]">
      <span>{{ name }}</span>
      <span>:</span>
    </div>
    <div class="text-666 space-x-2px flex items-center">
      <div v-if="prefix">{{ prefix }}</div>
      <div><slot /></div>
      <div v-if="append">{{ append }}</div>
    </div>
  </div>
</template>
