<template>
  <div class="edit-data">
    <div class="flex justify-end mb-8">
      <MultilingualSelector />
    </div>
    <el-form
      v-if="fields.length"
      ref="form"
      label-position="top"
      :model="model"
      :rules="rules"
      @submit.prevent
    >
      <FieldControl
        v-for="(item, index) in fields"
        v-show="isShowField(item)"
        :key="index"
        :field="item"
        :model="model"
      />
      <ContentCategories v-model="categories" />
      <ContentEmbeddeds
        :id="id"
        v-model="embeddeds"
        :paths="[...(paths ?? [])]"
      />
    </el-form>

    <GuidInfo v-else-if="!loading">
      <p>{{ t("common.noFieldYet") }}</p>
    </GuidInfo>
  </div>
</template>

<script lang="ts" setup>
import { computed, nextTick, onMounted, onUnmounted, ref, watch } from "vue";
import type {
  ContentCategory,
  ContentEmbedded,
  ContentFieldItem,
} from "@/api/content/textContent";
import { getEditContent, langupdate } from "@/api/content/textContent";
import { useMultilingualStore } from "@/store/multilingual";
import type { ElForm, FormRules } from "element-plus";
import { groupBy } from "lodash-es";
import FieldControl from "@/components/field-control/index.vue";
import type { Field } from "@/components/field-control/types";
import { useControlTypes, getFieldRules } from "@/hooks/use-control-types";
import GuidInfo from "@/components/guid-info/index.vue";
import MultilingualSelector from "@/components/multilingual-selector/index.vue";
import ContentCategories from "./content-categories.vue";
import ContentEmbeddeds from "./content-embeddeds.vue";
import { getQueryString } from "@/utils/url";

import { useI18n } from "vue-i18n";
import { useSaveTip } from "@/hooks/use-save-tip";
import { onBeforeRouteLeave } from "vue-router";
import { useSiteStore } from "@/store/site";

interface PropsType {
  folderId: string;
  contentType?: string;
  id?: string;
  paths?: string[];
  associated?: boolean;
}
const props = defineProps<PropsType>();
const { t } = useI18n();
const form = ref<InstanceType<typeof ElForm>>();
const fields = ref<Field[]>([]);
const categories = ref<ContentCategory[]>([]);
const embeddeds = ref<ContentEmbedded[]>([]);
const model = ref<Record<string, any>>({});
const rules = ref<FormRules>({});
const multilingualStore = useMultilingualStore();
const loading = ref(false);
const saveTip = useSaveTip();
const siteStore = useSiteStore();

onMounted(async () => {
  await getEdit();
  if (!props.associated) {
    saveTip.init([model.value, categories.value, embeddeds.value]);
  }
});

const multilingualSite = computed(
  () =>
    multilingualStore.visible &&
    Object.keys(multilingualStore.cultures).length > 1
);

// 组件销毁时重置firstActiveMenu的值，防止影响到activeName外面的行为
onUnmounted(() => {
  siteStore.firstActiveMenu = "";
});

async function getEdit() {
  loading.value = true;
  const response = await getEditContent({
    id: props.id,
    folderId: props.folderId,
    typeId: props.contentType,
  });
  loading.value = false;
  setFields(response.properties);
  categories.value = response.categories || [];
  embeddeds.value = response.embedded || [];
}
onBeforeRouteLeave(async (to, from, next) => {
  if (props.associated) return next();
  if (to.name === "login") {
    next();
  } else {
    siteStore.firstActiveMenu = to.meta.activeMenu ?? to.name;
    await saveTip
      .check([model.value, categories.value, embeddeds.value])
      .then(() => {
        next();
      })
      .catch(() => {
        siteStore.firstActiveMenu = "contents";
        next(false);
      });
  }
});

const { getControlType } = useControlTypes();
function setFields(properties: ContentFieldItem[]) {
  if (!properties || properties.length === 0) {
    return;
  }
  const defaultCulture = multilingualStore.default;
  const allFields: Field[] = [];
  const fieldModel: Record<string, any> = {};
  const fieldRules: FormRules = {};
  properties.forEach((item) => {
    const langKeys = Object.keys(item.values).sort((a, b) => {
      if (a === defaultCulture) {
        return -1;
      }
      if (b === defaultCulture) {
        return 1;
      }
      return 0;
    });
    const valueArr = langKeys.map((key) => {
      return {
        lang: key,
        value: item.values[key],
      };
    });
    valueArr.forEach((v) => {
      const field: Field = {
        lang: v.lang,
        name: item.name,
        displayName:
          item.isMultilingual && multilingualSite.value
            ? item.displayName + " (" + v.lang + ")"
            : item.displayName,
        prop: item.name + "_" + v.lang,
        toolTip: item.toolTip,
        selectionOptions: JSON.parse(item.selectionOptions || "[]") || [],
        validations: JSON.parse(item.validations ?? "[]") || [],
        multipleValue: item.multipleValue,
        isMultilingual: item.isMultilingual,
        controlType: item.controlType,
        settings: JSON.parse(item.settings || "{}") || {},
      };
      // fields
      allFields.push(field);
      // model
      let value = v.value;
      if (field.multipleValue) {
        value = JSON.parse(value || "[]");
      } else {
        let control = getControlType(item.controlType);
        if (control) {
          if (control.value === "Switch") {
            if (value && typeof value === "string") {
              value = JSON.parse(value.toLowerCase());
            } else {
              value = !!value;
            }
          } else if (control.value === "CheckBox") {
            value = value ? JSON.parse(value) : [];
          } else if (control.value === "KeyValues") {
            value = value ? JSON.parse(value) : [];
          } else if (control.value === "Number") {
            value = value === null ? undefined : value;
          }
        }
      }

      fieldModel[field.prop] = value;

      fieldRules[field.prop] = getFieldRules(field);
    });
  });

  fields.value = allFields;
  model.value = fieldModel;
  rules.value = fieldRules;
}

async function getSaveData(values: Record<string, Record<string, any>> = {}) {
  try {
    await form.value?.validate();
  } catch (error) {
    await afterSaveError(error);
    throw error;
  }

  const groups = groupBy(fields.value, (field) => field.lang);
  for (const key in groups) {
    values[key] = {};
    groups[key].forEach((field) => {
      let value = model.value[field.prop];
      if (field.multipleValue || Array.isArray(value)) {
        value = JSON.stringify(value);
      }
      values[key][field.name] = value === undefined ? null : value;
    });
  }
  const selectedCategories: Record<string, string[]> = {};
  categories.value?.forEach((item) => {
    selectedCategories[item.categoryFolder.id] = item.contents.map((x) => x.id);
  });
  const embeddedData: Record<string, string[]> = {};
  embeddeds.value?.forEach((item) => {
    embeddedData[item.embeddedFolder.id] = item.contents.map((x) => x.id);
  });
  const isCopy = getQueryString("copy");
  return {
    id: isCopy ? undefined : props.id,
    folderId: props.folderId,
    typeId: props.contentType,
    values,
    categories: selectedCategories,
    embedded: embeddedData,
  };
}

async function save() {
  const values: Record<string, Record<string, any>> = {};
  const saveData = await getSaveData(values);
  const result = await langupdate(saveData);
  saveTip.init([model.value, categories.value, embeddeds.value]);
  return { id: result, values };
}

async function afterSaveError(error: unknown) {
  if (multilingualSite.value) {
    for (const key in multilingualStore.cultures) {
      if (!multilingualStore.selected.includes(key)) {
        multilingualStore.selectedChanged(key);
      }
    }
  }
  if (error && typeof error === "object") {
    const firstErrorField = Object.keys(error)[0];
    await nextTick();
    form.value?.scrollToField(firstErrorField);
  }
}

function isShowField(field: Field) {
  if (props.associated && field.name == "Online") return;
  return multilingualStore.selected.includes(field.lang);
}

defineExpose({
  save,
  getSaveData,
});

watch(
  () => [model.value, categories.value, embeddeds.value],
  () => {
    saveTip.changed([model.value, categories.value, embeddeds.value]);
  },
  {
    deep: true,
  }
);
</script>

<style scoped lang="scss">
:deep(.el-input),
:deep(.el-textarea),
:deep(.el-select) {
  width: 504px;
}
</style>
