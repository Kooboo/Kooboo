<script lang="ts" setup>
import { ref } from "vue";
import { useI18n } from "vue-i18n";
import DigitalItemsEditor from "../components/digital-items-editor.vue";
import type { OrderLine } from "@/api/commerce/order";

const { t } = useI18n();
const show = ref(true);

const emit = defineEmits<{
  (e: "update:modelValue", value: boolean): void;
}>();

defineProps<{ line: OrderLine }>();
</script>

<template>
  <div>
    <teleport to="body">
      <ElDialog
        :model-value="show"
        width="600px"
        :title="t('common.digitalItems')"
        :close-on-click-modal="false"
        @closed="emit('update:modelValue', false)"
      >
        <DigitalItemsEditor :id="line.id" v-model:items="line.digitalItems" />
      </ElDialog>
    </teleport>
  </div>
</template>
