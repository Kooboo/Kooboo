<script lang="ts" setup>
import { useCommerceStore } from "@/store/commerce";

defineProps<{ amount?: number; currency?: string; original?: number }>();

const commerceStore = useCommerceStore();
</script>

<template>
  <div class="inline-flex items-center gap-4">
    <span>{{ currency || commerceStore.settings.currencySymbol }}</span>
    <span>{{ amount ? amount?.toFixed(2) : 0 }}</span>
    <span
      v-if="original && original != amount"
      class="text-s text-999 line-through"
      >{{ original ? original?.toFixed(2) : 0 }}</span
    >
  </div>
</template>
