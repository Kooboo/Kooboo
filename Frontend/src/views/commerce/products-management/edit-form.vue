<script lang="ts" setup>
import { useI18n } from "vue-i18n";
import { onMounted, ref } from "vue";
import type { ProductEdit } from "@/api/commerce/product";
import { editProduct, getProductEdit } from "@/api/commerce/product";
import { useCommerceStore } from "@/store/commerce";
import VariantEditor from "./variant-editor.vue";
import BasicInfo from "./basic-info.vue";
import KeyValueEditor from "@/components/basic/key-value-editor.vue";
import OptionGroupEditor from "./option-group-editor.vue";
import { useVariants } from "./product-variant";
import type { KeyValue } from "@/global/types";
import CustomData from "../custom-data/index.vue";
import { displayFormError } from "@/utils/common";
import { useProductFields } from "../useFields";

type Attribute = KeyValue & { options: string[] };
const attributes = ref<Attribute[]>([]);
const { t } = useI18n();

const props = defineProps<{
  id: string;
}>();

const commerceStore = useCommerceStore();
const basicInfo = ref();
commerceStore.loadCategories();
let variants = useVariants();
const customData = ref();
const model = ref<ProductEdit>();
const { fields, customFields } = useProductFields();

onMounted(async () => {
  model.value = await getProductEdit(props.id);
  if (!model.value.tags) model.value.tags = [];

  attributes.value = model.value.attributes.map((m) => ({
    key: m.key,
    value: m.value,
    options: [],
  }));

  for (const i of model.value.variants) {
    variants.addVariant(i);
  }
});

async function onSave() {
  try {
    await basicInfo.value.form.validate();
    await customData.value?.form?.validate();
  } catch (error) {
    displayFormError();
    throw error;
  }
  const postData = { ...model.value };
  postData.attributes = attributes.value.map((m) => ({
    key: m.key,
    value: m.value,
  }));
  postData.variants = variants.list.value;
  await editProduct(postData);
}

defineExpose({ onSave });
</script>

<template>
  <div v-if="model" class="px-24 pt-0 pb-84px space-y-12">
    <BasicInfo ref="basicInfo" :model="model" :fields="fields" />

    <CustomData
      ref="customData"
      :data="model.customData"
      :custom-fields="customFields"
    />

    <div class="bg-fff dark:bg-[#252526] px-24 py-16 rounded-normal">
      <ElForm label-position="top">
        <ElFormItem :label="t('common.attributes')">
          <KeyValueEditor
            v-model="attributes"
            :key-input-attributes="{ placeholder: t('common.name') }"
            class="max-w-600px space-y-8 w-full"
          />
        </ElFormItem>
        <ElFormItem :label="t('commerce.variantOptions')">
          <OptionGroupEditor
            :options="variants.options.value"
            :variants="variants.list.value"
            :variant-options="model.variantOptions"
            class="max-w-600px"
            @update-option-item="variants.updateOptionItem"
            @update-option-name="variants.updateOptionName"
            @add-option-item="
              (name, option) =>
                variants.addOptionItem(name, option, model?.featuredImage ?? '')
            "
            @delete-option-item="variants.deleteOptionItem"
            @delete-option="variants.deleteOption"
            @add-option="variants.addOption"
          />
        </ElFormItem>
      </ElForm>
    </div>

    <div class="bg-fff dark:bg-[#252526] px-24 py-16 rounded-normal">
      <VariantEditor
        :variant-options="model.variantOptions"
        :variants="variants.list.value"
        :options="variants.options.value"
        :default-image="model?.featuredImage ?? ''"
      />
    </div>
  </div>
</template>
