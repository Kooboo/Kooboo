<script lang="ts" setup>
import { ref } from "vue";
import { useI18n } from "vue-i18n";
import type { ProductVariant } from "@/api/commerce/product";
import { newGuid } from "@/utils/guid";
import { getVariantOptions } from "./product-variant";
import EditableTags from "@/components/basic/editable-tags.vue";
import { useTag } from "../useTag";

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
});

const { tags, removeTag } = useTag("Variant");

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
        <div class="mb-24 flex justify-center">
          <ImageCover v-model="model.image" editable size="large" />
        </div>

        <ElForm label-position="top">
          <ElFormItem
            v-if="model.selectedOptions.length"
            :label="t('commerce.variantOptions')"
          >
            <div class="space-y-8 w-full">
              <div
                v-for="(item, index) of model.selectedOptions"
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
            <ElFormItem :label="t('common.price')">
              <ElInput v-model.number="model.price" />
            </ElFormItem>
            <ElFormItem :label="t('common.weight')">
              <ElInput v-model.number="model.weight" />
            </ElFormItem>
            <ElFormItem :label="t('common.inventory')">
              <ElInput v-model.number="model.newInventory" />
            </ElFormItem>
            <ElFormItem label="SKU">
              <ElInput v-model="model.sku" />
            </ElFormItem>
            <ElFormItem :label="t('common.barcode')">
              <ElInput v-model="model.barcode" />
            </ElFormItem>
          </div>
          <ElFormItem :label="t('common.tag')">
            <EditableTags
              v-model="model.tags"
              :options="tags"
              @delete-option="removeTag"
            />
          </ElFormItem>
          <ElFormItem :label="t('common.active')">
            <ElSwitch v-model="model.active" />
          </ElFormItem>
        </ElForm>
        <template #footer>
          <DialogFooterBar @confirm="onSave" @cancel="show = false" />
        </template>
      </ElDialog>
    </teleport>
  </div>
</template>
