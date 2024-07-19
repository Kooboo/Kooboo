<template>
  <div class="p-24 overflow-auto h-full">
    <pre class="w-full break-words whitespace-pre-wrap">{{ data }}</pre>
  </div>
</template>

<script lang="ts" setup>
import { viewSource } from "@/api/mail";
import { ref } from "vue";
import { useRoute } from "vue-router";

const route = useRoute();
const data = ref();

async function load() {
  data.value = await viewSource(route.query.messageId as string);
}
load();
</script>
