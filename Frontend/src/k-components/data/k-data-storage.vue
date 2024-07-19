<script lang="ts" setup>
import { getCurrentInstance, isRef } from "vue";
import type { DataStorage, StorageType } from ".";

interface Props {
  name: string;
  from: "header";
  to: StorageType;
}

const props = defineProps<Props>();
const storages = getCurrentInstance()?.parent?.exposed?.storages;

if (storages && isRef(storages)) {
  (storages.value as any).push({
    name: props.name,
    from: props.from,
    to: props.to,
  } as DataStorage);
}
</script>

<template>
  <div class="hidden">
    <slot />
  </div>
</template>
