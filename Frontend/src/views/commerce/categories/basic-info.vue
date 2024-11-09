<script lang="ts" setup>
import type { CategoryBasicInfo } from "@/api/commerce/category";
import { useCommerceStore } from "@/store/commerce";
import { useI18n } from "vue-i18n";
import EditableTags from "@/components/basic/editable-tags.vue";
import SeoNameFormItem from "../components/seo-name-form-item.vue";
import { useTag } from "../useTag";
import type { FormRules } from "element-plus";
import { rangeRule, requiredRule } from "@/utils/validate";
import { ref } from "vue";
import type { CustomField } from "@/api/commerce/settings";
import { useCategoryLabels } from "../useLabels";
import { ignoreCaseEqual } from "@/utils/string";
const { getFieldDisplayName } = useCategoryLabels();

const { t } = useI18n();
const form = ref();
const props = defineProps<{
  model: CategoryBasicInfo;
  fields: CustomField[];
}>();

if (!props.model.customData["title"]) {
  props.model.customData["title"] = {};
}

if (!props.model.customData["description"]) {
  props.model.customData["description"] = {};
}

const rules: FormRules = {
  title: [requiredRule(t("common.fieldRequiredTips")), rangeRule(1, 50)],
  seoName: [requiredRule(t("common.fieldRequiredTips")), rangeRule(1, 50)],
};

const commerceStore = useCommerceStore();
commerceStore.loadCategories();
const { tags, removeTag } = useTag("Category");
function getDisplayName(name: string) {
  const field = props.fields?.find((it) => ignoreCaseEqual(it.name, name)) ?? {
    name,
    displayName: "",
  };
  return getFieldDisplayName(field);
}
defineExpose({
  form,
});
</script>

<template>
  <div class="bg-fff dark:bg-[#252526] px-24 py-16 rounded-normal">
    <div class="flex justify-end">
      <MultilingualSelector />
    </div>
    <ElForm ref="form" label-position="top" :rules="rules" :model="model">
      <MultilingualFormItem
        v-model:default-model="model.title"
        :label="getDisplayName('title')"
        :multilingual-model="model.customData.title"
        :rules="rules.title"
        default-prop="title"
        multilingual-prop="customData.title"
      >
        <template #default="{ value, onchange }">
          <ElInput
            :model-value="value"
            @update:model-value="onchange($event)"
          />
        </template>
      </MultilingualFormItem>
      <SeoNameFormItem
        v-model:seo-name="model.seoName"
        prop="seoName"
        path="category"
        :title="model.title"
        :label="getDisplayName('seoName')"
      />
      <ElFormItem :label="getDisplayName('image')">
        <ImageCover
          v-model="model.image"
          editable
          size="large"
          folder="/commerce/category"
          :prefix="new Date().getTime().toString()"
        />
      </ElFormItem>
      <MultilingualFormItem
        v-model:default-model="model.description"
        :label="getDisplayName('description')"
        :multilingual-model="model.customData.description"
      >
        <template #default="{ value, onchange }">
          <KEditor
            :model-value="value"
            @update:model-value="onchange($event)"
          />
        </template>
      </MultilingualFormItem>
      <ElFormItem :label="getDisplayName('tags')">
        <EditableTags
          v-model="model.tags"
          option-deletable
          :options="tags"
          @delete-option="removeTag"
        />
      </ElFormItem>
      <ElFormItem :label="getDisplayName('parentId')">
        <ElSelect
          v-model="model.parentId"
          class="w-max-400px"
          :clearable="true"
        >
          <ElOption
            v-for="item in commerceStore.categories"
            :key="item.id"
            :label="item.title"
            :value="item.id"
          />
        </ElSelect>
      </ElFormItem>
      <ElFormItem :label="getDisplayName('active')">
        <ElSwitch v-model="model.active" />
      </ElFormItem>
    </ElForm>
  </div>
</template>
