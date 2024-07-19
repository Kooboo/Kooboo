<template>
  <div class="flex items-center justify-center w-full">
    <div class="overflow-hidden flex-1">
      {{ title }}
    </div>
    <IconButton
      icon="icon-Refresh"
      class="text-orange"
      style="position: initial"
      :tip="t('common.confirmResetDefaultValues')"
      data-cy="reset"
      @click.stop
      @confirm="emit('reset')"
    />
  </div>
</template>

<script lang="ts" setup>
import { useI18n } from "vue-i18n";
const { t } = useI18n();
defineProps<{
  title: string;
}>();

const emit = defineEmits<{
  (e: "reset"): void;
}>();
</script>
