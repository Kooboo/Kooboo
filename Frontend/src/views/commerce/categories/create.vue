<script lang="ts" setup>
import { useRouter } from "vue-router";
import Breadcrumb from "@/components/basic/breadcrumb.vue";
import { useI18n } from "vue-i18n";
import { ref } from "vue";
import type { CategoryCreate } from "@/api/commerce/category";
import { createCategory } from "@/api/commerce/category";
import { useRouteSiteId } from "@/hooks/use-site-id";
import BasicInfo from "./basic-info.vue";
import Condition from "../components/condition.vue";
import type { ConditionSchema } from "@/api/commerce/common";
import { getConditionSchemas } from "@/api/commerce/category";
import CustomData from "../custom-data/index.vue";
import { displayFormError } from "@/utils/common";
import { useCategoryFields } from "../useFields";

const { fields, customFields } = useCategoryFields();
const { t } = useI18n();
const router = useRouter();
const schemas = ref<ConditionSchema[]>([]);
const basicInfo = ref();
const customData = ref();

getConditionSchemas().then((rsp) => {
  schemas.value = rsp;
});

const types = [
  {
    name: "Manual",
    display: t("common.manual"),
    description: t("commerce.manualProductTip"),
  },
  {
    name: "Automated",
    display: t("common.automated"),
    description: t("commerce.automatedProductTip"),
  },
];

const model = ref<CategoryCreate>({
  title: "",
  seoName: "",
  description: "",
  image: "",
  active: true,
  type: types[0].name,
  parentId: "",
  tags: [],
  customData: {},
  condition: {
    isAny: true,
    items: [],
  },
});

function goBack() {
  router.goBackOrTo(
    useRouteSiteId({
      name: "product collections",
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
  await createCategory(model.value);
  goBack();
}
</script>

<template>
  <Breadcrumb
    class="p-24"
    :crumb-path="[
      {
        name: t('common.productCategories'),
        route: { name: 'product categories' },
      },
      { name: t('common.create') },
    ]"
  />
  <div class="px-24 pt-0 pb-84px space-y-12">
    <BasicInfo ref="basicInfo" :model="model" :fields="fields" />
    <CustomData
      ref="customData"
      :data="model.customData"
      :custom-fields="customFields"
    />
    <div class="bg-fff dark:bg-[#252526] px-24 py-16 rounded-normal">
      <ElForm label-position="top">
        <ElFormItem :label="t('common.type')">
          <ElRadioGroup v-model="model.type" class="block">
            <div
              v-for="item of types"
              :key="item.name"
              class="flex items-center"
            >
              <ElRadio :label="item.name">{{ item.display }}</ElRadio>
              <div class="w-full text-s text-999">{{ item.description }}</div>
            </div>
          </ElRadioGroup>
        </ElFormItem>
      </ElForm>
    </div>
    <div
      v-if="model.type == 'Automated' && schemas"
      class="bg-fff dark:bg-[#252526] px-24 py-16 rounded-normal"
    >
      <Condition
        :condition="model.condition"
        :schemas="schemas"
        :title="t('commerce.product')"
      />
    </div>
  </div>
  <KBottomBar
    :permission="{
      feature: 'productCategories',
      action: 'edit',
    }"
    @cancel="goBack"
    @save="save"
  />
</template>
