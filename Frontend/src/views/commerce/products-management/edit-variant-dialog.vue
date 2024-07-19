<script lang="ts" setup>
import { ref } from "vue";
import { useI18n } from "vue-i18n";
import type { ProductVariant } from "@/api/commerce/product";
import { getVariantOptions } from "./product-variant";
import EditableTags from "@/components/basic/editable-tags.vue";
import { useCommerceStore } from "@/store/commerce";
import { useTag } from "../useTag";

const { t } = useI18n();
const show = ref(true);
const commerceStore = useCommerceStore();

const props = defineProps<{
  model: ProductVariant;
  variants: ProductVariant[];
}>();

if (!props.model.tags) props.model.tags = [];
const copyValue = ref<ProductVariant>(JSON.parse(JSON.stringify(props.model)));
const { tags, removeTag } = useTag("Variant");

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
        <div class="mb-24 flex justify-center">
          <ImageCover v-model="copyValue.image" editable size="large" />
        </div>

        <ElForm label-position="top">
          <ElFormItem
            v-if="copyValue.selectedOptions.length"
            :label="t('commerce.variantOptions')"
          >
            <div class="space-y-8 w-full">
              <div
                v-for="(item, index) of copyValue.selectedOptions"
                :key="index"
                class="flex items-center space-x-4"
              >
                <ElInput v-model="item.name" disabled />
                <div class="text-999">:</div>
                <DropdownInput
                  v-model="item.value"
                  :options="getVariantOptions(variants, item.name)"
                  class="w-full"
                  :placeholder="t('common.value')"
                />
              </div>
            </div>
          </ElFormItem>
          <div class="grid grid-cols-3 gap-x-8">
            <ElFormItem
              :label="`${t('common.price')} (${
                commerceStore.settings.currencySymbol
              })`"
            >
              <ElInput v-model.number="copyValue.price" />
            </ElFormItem>
            <ElFormItem
              :label="`${t('common.weight')} (${
                commerceStore.settings.weightUnit
              })`"
            >
              <ElInput v-model.number="copyValue.weight" />
            </ElFormItem>
            <ElFormItem :label="t('common.inventory')">
              <ElInput v-model.number="copyValue.newInventory" />
            </ElFormItem>
            <ElFormItem label="SKU">
              <ElInput v-model="copyValue.sku" />
            </ElFormItem>
            <ElFormItem :label="t('common.barcode')">
              <ElInput v-model="copyValue.barcode" />
            </ElFormItem>
          </div>
          <ElFormItem :label="t('common.tag')">
            <EditableTags
              v-model="copyValue.tags"
              :options="tags"
              option-deletable
              @delete-option="removeTag"
            />
          </ElFormItem>
          <ElFormItem :label="t('commerce.active')">
            <ElSwitch v-model="copyValue.active" />
          </ElFormItem>
        </ElForm>
        <template #footer>
          <DialogFooterBar @confirm="onSave" @cancel="show = false" />
        </template>
      </ElDialog>
    </teleport>
  </div>
</template>
