<script lang="ts" setup>
import { ref } from "vue";
import { useI18n } from "vue-i18n";
import type { ProductVariant } from "@/api/commerce/product";
import VariantForm from "./variant-form.vue";

const { t } = useI18n();
const show = ref(true);

const props = defineProps<{
  model: ProductVariant;
  variants: ProductVariant[];
}>();

if (!props.model.tags) props.model.tags = [];
const copyValue = ref<ProductVariant>(JSON.parse(JSON.stringify(props.model)));

const emit = defineEmits<{
  (e: "update:modelValue", value: boolean): void;
  (e: "update:model", value: ProductVariant): void;
}>();

function onSave() {
  emit("update:model", copyValue.value);
  show.value = false;
}
</script>

<template>
  <div>
    <teleport to="body">
      <ElDialog
        :model-value="show"
        width="600px"
        :title="t('common.edit')"
        :close-on-click-modal="false"
        @closed="emit('update:modelValue', false)"
      >
        <VariantForm :model="copyValue" :variants="variants" />
        <template #footer>
          <DialogFooterBar @confirm="onSave" @cancel="show = false" />
        </template>
      </ElDialog>
    </teleport>
  </div>
</template>
