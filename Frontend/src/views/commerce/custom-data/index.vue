<script lang="ts" setup>
import CustomField from "./custom-field.vue";
import type { CustomField as CustomFieldType } from "@/api/commerce/settings";
import { useMultilingualStore } from "@/store/multilingual";
import { fieldTypes, getFieldRules } from "./custom-field";
import { ref } from "vue";

const props = defineProps<{
  data: Record<string, any>;
  customFields: CustomFieldType[];
}>();

const multilingualStore = useMultilingualStore();
const form = ref();

function getModel(name: string) {
  name =
    Object.keys(props.data).find(
      (f) => f.toLowerCase() == name.toLowerCase()
    ) || name;
  if (!props.data[name]) props.data[name] = {};
  return props.data[name];
}

function getModelValue(name: string, lang: string) {
  const model = getModel(name);
  return model[lang];
}

function setModelValue(name: string, lang: string, value: any) {
  const model = getModel(name);
  model[lang] = value;
}

function getDefaultLang(field: CustomFieldType) {
  const multilingual = fieldTypes[field.type]?.multilingual;
  if (multilingual) return multilingualStore.default;
  const model = getModel(field.name);
  const keys = Object.keys(model);
  if (!keys.length || keys.includes(multilingualStore.default)) {
    return multilingualStore.default;
  }
  return keys[0];
}

defineExpose({
  form,
});
</script>

<template>
  <div
    v-if="customFields?.length"
    class="bg-fff dark:bg-[#252526] px-24 py-16 rounded-normal"
  >
    <ElForm ref="form" label-position="top" :model="data">
      <template v-for="item of customFields" :key="item.name">
        <template v-if="item.editable">
          <template
            v-if="fieldTypes[item.type]?.multilingual && item.multilingual"
          >
            <MultilingualFormItem
              :label="item.displayName || item.name"
              :multilingual-model="getModel(item.name)"
              :rules="getFieldRules(item.validations, item.type)"
              :multilingual-prop="item.name"
            >
              <template #default="{ value, onchange }">
                <CustomField
                  :model-value="value"
                  :field="item"
                  @update:model-value="onchange($event)"
                />
              </template>
            </MultilingualFormItem>
          </template>

          <template v-else>
            <el-form-item
              :label="item.displayName || item.name"
              :rules="getFieldRules(item.validations, item.type)"
              :prop="`${item.name}.${getDefaultLang(item)}`"
            >
              <CustomField
                :model-value="getModelValue(item.name, getDefaultLang(item))"
                :field="item"
                @update:model-value="
                  setModelValue(item.name, getDefaultLang(item), $event)
                "
              />
            </el-form-item>
          </template>
        </template>
      </template>
    </ElForm>
  </div>
</template>
