<script lang="ts" setup>
import { computed, inject } from "vue";
import type { PageState } from "./k-page";
import { PAGE_STATE_KEY } from "./k-page";

interface Props {
  data?: string;
}

const props = defineProps<Props>();
const pageState = inject<PageState>(PAGE_STATE_KEY);

const content = computed(() => {
  const state = pageState?.getState(props.data).value;
  return state ?? props.data;
});
</script>

<template>
  <div v-html="content" />
</template>
