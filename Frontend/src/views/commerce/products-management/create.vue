<script lang="ts" setup>
import { useRouter } from "vue-router";
import Breadcrumb from "@/components/basic/breadcrumb.vue";
import { useI18n } from "vue-i18n";
import { onMounted, ref, watch } from "vue";
import type { ProductCreate } from "@/api/commerce/product";
import { createProduct } from "@/api/commerce/product";
import { getProductType } from "@/api/commerce/type";
import { useRouteSiteId } from "@/hooks/use-site-id";
import { useCommerceStore } from "@/store/commerce";
import VariantEditor from "./variant-editor.vue";
import BasicInfo from "./basic-info.vue";
import KeyValueEditor from "@/components/basic/key-value-editor.vue";
import OptionGroupEditor from "./option-group-editor.vue";
import { getQueryString } from "@/utils/url";
import type { ProductType } from "@/api/commerce/type";
import { emptyGuid } from "@/utils/guid";
import type { KeyValue } from "@/global/types";
import { createNewVariant, useVariants } from "./product-variant";
import CustomData from "../custom-data/index.vue";
import { displayFormError } from "@/utils/common";
import { useProductFields } from "../useFields";

const { t } = useI18n();
const router = useRouter();
const typeId = getQueryString("typeId");
const commerceStore = useCommerceStore();
commerceStore.loadCategories();
const productType = ref<ProductType>();
type Attribute = KeyValue & { options: string[] };
const attributes = ref<Attribute[]>([]);
const basicInfo = ref();
const customData = ref();
let variants = useVariants();
variants.addVariant(createNewVariant([], ""));

const { fields, customFields } = useProductFields();

const model = ref<ProductCreate>({
  categories: [],
  attributes: [],
  title: "",
  seoName: "",
  description: "",
  featuredImage: "",
  images: [],
  active: true,
  variants: [],
  tags: [],
  customData: {},
  variantOptions: [],
  isDigital: false,
});

onMounted(async () => {
  if (typeId && typeId != emptyGuid) {
    productType.value = await getProductType(typeId);
  }

  if (productType.value) {
    for (const i of productType.value.attributes) {
      attributes.value.push({
        key: i.name,
        value: "",
        options: i.type == "Selection" ? i.options : [],
      });
    }

    for (const i of productType.value.options) {
      variants.options.value.push(i.name);
    }

    for (const option of productType.value.options) {
      for (const i of option.options) {
        variants.addOptionItem(option.name, i, "");
      }
      setTimeout(() => {
        const variantOption = model.value.variantOptions.find(
          (f) => f.name == option.name
        );
        if (variantOption) variantOption.type = option.valueType;
      });
    }
    model.value.isDigital = productType.value.isDigital;
    model.value.maxDownloadCount = productType.value.maxDownloadCount;
  }
});

function goBack() {
  router.goBackOrTo(
    useRouteSiteId({
      name: "product management",
    })
  );
}

async function save() {
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
  variants.list.value.forEach((i, index) => (i.order = index));
  postData.variants = variants.list.value;
  await createProduct(postData);
  goBack();
}

watch(
  () => model.value?.featuredImage,
  (value) => {
    if (!value) return;
    for (const i of variants.list.value) {
      if (!i.image) i.image = value;
    }
  }
);
</script>

<template>
  <Breadcrumb
    class="p-24"
    :crumb-path="[
      {
        name: t('common.productManagement'),
        route: { name: 'product management' },
      },
      { name: t('common.create') },
    ]"
  />
  <div class="px-24 pt-0 pb-84px space-y-12">
    <BasicInfo ref="basicInfo" :model="model" :fields="fields">
      <ElFormItem :label="t('common.attributes')">
        <KeyValueEditor
          v-model="attributes"
          :key-input-attributes="{ placeholder: t('common.attributeSamples') }"
          :value-input-attributes="{ placeholder: t('common.value') }"
          lass="max-w-600px space-y-8 w-full"
        />
      </ElFormItem>
    </BasicInfo>
    <CustomData
      ref="customData"
      :data="model.customData"
      :custom-fields="customFields"
    />
    <div class="bg-fff dark:bg-[#252526] px-24 py-16 rounded-normal">
      <ElForm label-position="top">
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
                variants.addOptionItem(name, option, model.featuredImage)
            "
            @delete-option-item="variants.deleteOptionItem"
            @delete-option="variants.deleteOption"
            @add-option="variants.addOption"
          />
        </ElFormItem>
        <VariantEditor
          :variants="variants.list.value"
          :options="variants.options.value"
          :default-image="model.featuredImage"
          :variant-options="model.variantOptions"
          :is-digital="model.isDigital"
        />
      </ElForm>
    </div>
  </div>
  <KBottomBar
    :permission="{
      feature: 'productManagement',
      action: 'edit',
    }"
    @cancel="goBack"
    @save="save"
  />
</template>

<style scoped>
:deep(.el-tabs__header) {
  padding: 0;
  background: transparent;
}

:deep(.el-tabs__content) {
  padding: 24px 0 12px 0;
}
</style>
