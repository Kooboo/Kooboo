<script lang="ts" setup>
import { useI18n } from "vue-i18n";
import { getVariantOptions } from "./product-variant";
import type { ProductVariant } from "@/api/commerce/product";
import { useTag } from "../useTag";

const { t } = useI18n();
defineProps<{ model: ProductVariant; variants: ProductVariant[] }>();
const { tags, removeTag } = useTag("Variant");
</script>

<template>
  <div>
    <div class="mb-24 flex justify-center">
      <ImageCover
        v-model="model.image"
        editable
        size="large"
        folder="/commerce/product"
        :prefix="new Date().getTime().toString()"
      />
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
          <ElInputNumber v-model="model.price" :controls="false" />
        </ElFormItem>
        <ElFormItem :label="t('common.weight')">
          <ElInputNumber v-model="model.weight" :controls="false" />
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
  </div>
</template>
