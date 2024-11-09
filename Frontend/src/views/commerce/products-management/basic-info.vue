<script lang="ts" setup>
import type { ProductBasicInfo } from "@/api/commerce/product";
import { useCommerceStore } from "@/store/commerce";
import { useI18n } from "vue-i18n";
import SeoNameFormItem from "../components/seo-name-form-item.vue";
import SelectCategoryDialog from "../components/select-category-dialog.vue";
import { computed, ref } from "vue";
import type { CategoryListItem } from "@/api/commerce/category";
import EditableTags from "@/components/basic/editable-tags.vue";
import ProductImages from "./product-images.vue";
import { useTag } from "../useTag";
import type { FormRules } from "element-plus";
import { rangeRule, requiredRule } from "@/utils/validate";
import { useProductLabels } from "../useLabels";
import type { CustomField } from "@/api/commerce/settings";
import { ignoreCaseEqual } from "@/utils/string";
const { getFieldDisplayName } = useProductLabels();
const { t } = useI18n();
const props = defineProps<{
  model: ProductBasicInfo;
  fields: CustomField[];
}>();
const commerceStore = useCommerceStore();
const showSelectCategoryDialog = ref(false);
commerceStore.loadCategories();
const { tags, removeTag } = useTag("Product");
const form = ref();

if (!props.model.customData["title"]) {
  props.model.customData["title"] = {};
}

if (!props.model.customData["description"]) {
  props.model.customData["description"] = {};
}

const rules: FormRules = {
  title: [requiredRule(t("common.fieldRequiredTips")), rangeRule(1, 200)],
  seoName: [requiredRule(t("common.fieldRequiredTips")), rangeRule(1, 200)],
};

const selectedCategories = computed(() => {
  const result = [];

  for (const i of commerceStore.categories) {
    if (!props.model.categories.includes(i.id)) continue;
    result.push(i);
  }
  return result;
});

function onDeleteCategory(id: string) {
  const index = props.model.categories.indexOf(id);
  props.model.categories.splice(index, 1);
}

function onSelectedCategory(categories: CategoryListItem[]) {
  props.model.categories.push(...categories.map((m) => m.id));
}

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
        path="product"
        :title="model.title"
        :label="getDisplayName('seoName')"
      />
      <ElFormItem :label="getDisplayName('images')">
        <ProductImages
          v-model="model.images"
          v-model:main="model.featuredImage"
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

      <ElFormItem :label="getDisplayName('categories')">
        <div class="flex gap-4 flex-wrap">
          <ElTag v-for="item of selectedCategories" :key="item.id">
            <div class="space-x-8 flex items-center">
              <div class="cursor-pointer">
                {{ item.title }}
              </div>
              <el-icon
                v-if="item.type == 'Manual'"
                class="iconfont icon-delete"
                @click="onDeleteCategory(item.id)"
              />
            </div>
          </ElTag>

          <ElTag type="success" @click="showSelectCategoryDialog = true">
            <el-icon class="iconfont icon-a-addto" />
          </ElTag>
        </div>
      </ElFormItem>
      <ElFormItem :label="getDisplayName('tags')">
        <EditableTags
          v-model="model.tags"
          option-deletable
          :options="tags"
          @delete-option="removeTag"
        />
      </ElFormItem>

      <ElFormItem :label="getDisplayName('active')">
        <ElSwitch v-model="model.active" />
      </ElFormItem>
      <slot />
    </ElForm>
  </div>
  <SelectCategoryDialog
    v-if="showSelectCategoryDialog"
    v-model="showSelectCategoryDialog"
    :excludes="model.categories"
    @selected="onSelectedCategory"
  />
</template>
