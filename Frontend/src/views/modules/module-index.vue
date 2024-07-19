<script lang="ts" setup>
import { ref } from "vue";
import { useRoute } from "vue-router";
const route = useRoute();
const loading = ref(true);
</script>

<template>
  <div
    v-loading="loading"
    class="absolute inset-0 p-12 rounded-normal overflow-hidden"
  >
    <iframe
      class="w-full h-full rounded-normal"
      :src="route.query.url?.toString()"
      @load="loading = false"
      @error="loading = false"
    />
  </div>
</template>
