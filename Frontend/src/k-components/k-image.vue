<script lang="ts" setup>
import { computed, inject } from "vue";
import type { PageState } from "./k-page";
import { PAGE_STATE_KEY } from "./k-page";

interface Props {
  data?: string;
}

const props = defineProps<Props>();
const pageState = inject<PageState>(PAGE_STATE_KEY);

const src = computed(() => {
  const state = pageState?.getState(props.data).value;
  return (state ?? props.data) as string;
});

// const data = computed(() => pageState?.getState(props.data).value);
</script>

<template>
  <el-image :src="src" />
</template>
