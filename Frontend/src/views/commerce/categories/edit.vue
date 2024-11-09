<script lang="ts" setup>
import { useRouter } from "vue-router";
import Breadcrumb from "@/components/basic/breadcrumb.vue";
import { useI18n } from "vue-i18n";
import { ref } from "vue";
import type { CategoryEdit } from "@/api/commerce/category";
import { getCategoryEdit, editCategory } from "@/api/commerce/category";
import { useRouteSiteId } from "@/hooks/use-site-id";
import { getQueryString } from "@/utils/url";
import BasicInfo from "./basic-info.vue";
import { getConditionSchemas } from "@/api/commerce/category";
import Condition from "../components/condition.vue";
import type { ConditionSchema } from "@/api/commerce/common";
import CustomData from "../custom-data/index.vue";
import { displayFormError } from "@/utils/common";
import { useCategoryFields } from "../useFields";

const { t } = useI18n();
const router = useRouter();
const model = ref<CategoryEdit>();
const id = getQueryString("id")!;
const schemas = ref<ConditionSchema[]>([]);
const basicInfo = ref();
const customData = ref();
const { fields, customFields } = useCategoryFields();

getConditionSchemas().then((rsp) => {
  schemas.value = rsp;
});

getCategoryEdit(id).then(async (rsp) => {
  model.value = rsp;
  if (!model.value.tags) model.value.tags = [];
  if (model.value.type == "Automated") {
    schemas.value = await getConditionSchemas();
  }
});

function goBack() {
  router.goBackOrTo(
    useRouteSiteId({
      name: "product categories",
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
  let data: any = model.value;
  await editCategory(data, true);
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
      { name: t('common.edit') },
    ]"
  />
  <div v-if="model" class="px-24 pt-0 pb-84px space-y-12">
    <BasicInfo ref="basicInfo" :model="model" :fields="fields" />
    <CustomData
      ref="customData"
      :data="model.customData"
      :custom-fields="customFields"
    />
    <div
      v-if="model && model.type == 'Automated'"
      class="bg-fff dark:bg-[#252526] px-24 py-16 rounded-normal"
    >
      <Condition
        :schemas="schemas"
        :condition="model.condition"
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
