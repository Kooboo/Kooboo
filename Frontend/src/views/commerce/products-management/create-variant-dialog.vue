<script lang="ts" setup>
import { ref } from "vue";
import { useI18n } from "vue-i18n";
import type { ProductVariant } from "@/api/commerce/product";
import { newGuid } from "@/utils/guid";
import VariantForm from "./variant-form.vue";

const { t } = useI18n();
const show = ref(true);

const props = defineProps<{
  variants: ProductVariant[];
  defaultImage: string;
}>();

const model = ref<ProductVariant>({
  id: newGuid(),
  active: true,
  barcode: "",
  newInventory: 0,
  originalInventory: 0,
  price: 0,
  sku: "",
  image: "",
  weight: 0,
  selectedOptions: props.variants[0].selectedOptions.map((m) => ({
    name: m.name,
    value: "",
  })),
  tags: [],
  digitalItems: [],
  autoDelivery: true,
});

const emit = defineEmits<{
  (e: "update:modelValue", value: boolean): void;
}>();

function onSave() {
  if (!model.value.image) {
    model.value.image = props.defaultImage;
  }
  props.variants.push(model.value);
  show.value = false;
}
</script>

<template>
  <div>
    <teleport to="body">
      <ElDialog
        :model-value="show"
        width="600px"
        :title="t('common.create')"
        :close-on-click-modal="false"
        @closed="emit('update:modelValue', false)"
      >
        <VariantForm :model="model" :variants="variants" />
        <template #footer>
          <DialogFooterBar @confirm="onSave" @cancel="show = false" />
        </template>
      </ElDialog>
    </teleport>
  </div>
</template>
