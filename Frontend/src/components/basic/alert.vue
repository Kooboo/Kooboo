<script lang="ts" setup>
defineProps<{ title?: string; content: string }>();
</script>

<template>
  <div class="bg-gray dark:bg-fff/10 dark:text-999 px-32 py-16 text-s">
    <p class="font-bold">{{ title }}</p>
    <pre class="whitespace-pre-wrap">{{ content }}</pre>
  </div>
</template>
