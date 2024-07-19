<script lang="ts" setup>
import { computed } from "@vue/reactivity";
import { isLocal } from "@/utils/network";

const props = defineProps<{ url?: string; scale?: number; siteId?: string }>();

const realTime = computed(() => {
  var host = location.hostname;
  return props.url && isLocal(host?.toLowerCase());
});

const coverUrl = computed(() => {
  return `__cover?siteId=${props.siteId}`;
});
</script>

<template>
  <div class="w-full h-full overflow-hidden">
    <iframe
      v-if="realTime"
      class="origin-top-left"
      style="width: 1280px; height: 800px; transform: scale(0.28)"
      :style="{ transform: `scale(${scale || 1})` }"
      sandbox="allow-same-origin allow-scripts"
      :src="url"
    />
    <img v-else :src="coverUrl" />
  </div>
</template>
