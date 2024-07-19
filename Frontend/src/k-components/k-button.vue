<script lang="ts" setup>
import { inject } from "vue";
import type { PageState } from "./k-page";
import { PAGE_STATE_KEY } from "./k-page";
import { GetModuleUrl, showConfirm } from "./utils";

interface Props {
  get?: string;
  post?: string;
  to?: string;
  confirm?: string;
}

const props = defineProps<Props>();
const pageState = inject<PageState>(PAGE_STATE_KEY);

async function click() {
  if (props.confirm) await showConfirm(props.confirm);

  if (props.get) {
    const action = pageState?.getExposedAction(props.get, "get");
    if (action) await action();
  }

  if (props.post) {
    const action = pageState?.getExposedAction(props.post, "post");
    if (action) await action();
  }

  if (props.to) {
    location.href = GetModuleUrl(props.to);
  }
}
</script>

<template>
  <el-button @click="click"><slot /></el-button>
</template>
