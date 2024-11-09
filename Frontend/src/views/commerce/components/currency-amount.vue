<script lang="ts" setup>
import { useCommerceStore } from "@/store/commerce";

defineProps<{ amount?: number; currency?: string; original?: number }>();

const commerceStore = useCommerceStore();
</script>

<template>
  <div class="inline-flex items-center gap-4 flex-wrap">
    <div
      class="space-x-4 break-normal"
      :class="amount! < 0 ? 'text-orange' : ''"
    >
      <span>{{ amount ? amount.toFixed(2) : 0 }}</span>
      <span>{{ currency || commerceStore.settings.currencyCode }}</span>
    </div>
    <div
      v-if="original && original != amount"
      class="text-s text-999 line-through break-normal"
    >
      {{ original ? original?.toFixed(2) : 0 }}
    </div>
  </div>
</template>
