<template>
  <el-dialog
    :model-value="show"
    width="700px"
    :close-on-click-modal="false"
    :title="t('common.fieldEditor')"
    custom-class="el-dialog--zero-padding"
    @closed="emits('update:modelValue', false)"
  >
    <el-scrollbar max-height="60vh">
      <el-tabs
        v-model="currentTab"
        :class="{ 'hide-tab': model.isSystemField }"
      >
        <el-tab-pane :label="tabs[0].displayName" :name="tabs[0].value">
          <el-form
            ref="form"
            :model="model"
            :rules="rules"
            label-position="top"
            @keydown.enter="onEditorSave"
          >
            <el-form-item prop="name" :label="t('common.name')">
              <div class="w-full flex items-center">
                <el-input
                  v-model="model.name"
                  :disabled="!isNew"
                  class="w-3/5"
                  data-cy="field-name-input"
                />
                <el-checkbox
                  v-if="!options?.hideSummaryField && !model.isSystemField"
                  v-model="model.isSummaryField"
                  class="ml-12"
                  data-cy="summary-field"
                >
                  {{ t("common.summaryField") }}
                </el-checkbox>
              </div>
            </el-form-item>
            <el-form-item :label="t('common.displayName')">
              <el-input
                v-model="model.displayName"
                class="w-3/5"
                data-cy="display-name-input"
              />
            </el-form-item>
            <el-form-item v-if="model.isSystemField">
              <el-checkbox v-model="model.editable" data-cy="user-editable">
                {{ t("common.userEditable") }}
              </el-checkbox>
            </el-form-item>
            <el-form-item
              v-if="!model.isSystemField"
              :label="t('common.controlType')"
            >
              <el-select
                v-if="availableControlTypes.length > 1"
                v-model="model.controlType"
                class="w-3/5"
                data-cy="control-type-dropdown"
                @change="onControlTypeChange"
              >
                <el-option
                  v-for="item in availableControlTypes"
                  :key="item.value"
                  :value="item.value"
                  :label="item.displayName"
                  data-cy="control-type-opt"
                />
              </el-select>
              <p v-else>
                {{ controlModel?.displayName || model.controlType }}
              </p>
              <el-checkbox
                v-if="showOptionForm"
                v-model="model.settings['isDynamicOptions']"
                class="ml-12"
                data-cy="dynamic-options"
              >
                {{ t("common.dynamicOptions") }}
              </el-checkbox>
            </el-form-item>
            <el-form-item v-if="showOptionForm">
              <template #label>
                {{ t("common.options") }}
                <Tooltip
                  v-if="showDynamicOptions"
                  :enable-html="true"
                  :tip="
                    t('common.dynamicOptionsTips', { example: optionsExample })
                  "
                  custom-class="ml-4"
                />
              </template>
              <div
                v-if="showDynamicOptions"
                class="w-full border border-solid border-[#cccccc] focus-within:border-[#2296f3]"
                @keydown.enter.stop
              >
                <MonacoEditor
                  v-model="model.settings['code']"
                  language="typescript"
                  k-script
                />
              </div>
              <SelectionEditor v-else :options="model.selectionOptions" />
            </el-form-item>

            <el-form-item v-if="showMultipleCheckbox">
              <el-checkbox
                v-model="model.multipleValue"
                :disabled="!isNew || hasDefaultValue"
                data-cy="enable-multiple"
                @change="onMultipleValueChanged"
              >
                {{ t("common.enableMultiple") }}
              </el-checkbox>
            </el-form-item>
          </el-form>
          <el-form
            v-if="showDefaultValue"
            ref="defaultValueForm"
            label-position="top"
            :model="model.settings"
            @keydown.enter="onEditorSave"
          >
            <FieldPreview
              data-cy="field-default-value"
              :field="model"
              :model="model.settings"
              :rules="defaultValueRules"
              :override-field="defaultValueField"
              :css-class="defaultValueCss"
            />
          </el-form>
          <EditFieldPreview v-if="!hidePreviewField" :field="model" />
        </el-tab-pane>
        <el-tab-pane
          v-if="!model.isSystemField"
          :label="tabs[1].displayName"
          :name="tabs[1].value"
        >
          <el-form
            ref="advancedForm"
            label-position="top"
            :model="model.settings"
            @keydown.enter="onEditorSave"
            @submit.prevent
          >
            <el-form-item
              v-if="controlModel?.value === 'AdvancedMediaFile'"
              :label="t('common.meta')"
            >
              <template #label>
                {{ t("common.meta") }}
                <Tooltip
                  :enable-html="true"
                  :tip="
                    t('common.mediaFileAdvancedTips', {
                      parameter: '{{name}}',
                    })
                  "
                  custom-class="ml-4"
                />
              </template>
              <KeyValueEditor
                v-model="model.settings.meta"
                prop="meta"
                :value-input-attributes="{
                  placeholder: t('common.defaultValue'),
                }"
                :key-rules="advancedMediaFileMetaKeyRules"
                :default-options="defaultAdvancedMediaFileOptions"
                @delete="() => advancedForm?.validate()"
              />
            </el-form-item>
            <el-form-item :label="t('common.tooltip')">
              <el-input
                v-model="model.tooltip"
                class="w-3/5"
                :placeholder="t('common.fieldTooltip')"
                data-cy="tooltip"
              />
            </el-form-item>
            <el-form-item>
              <el-checkbox v-model="model.editable" data-cy="user-editable">
                {{ t("common.userEditable") }}
              </el-checkbox>
            </el-form-item>
            <el-form-item v-if="showMultipleLanguage">
              <el-checkbox
                v-model="model.multipleLanguage"
                data-cy="enable-multilingual"
              >
                {{ t("common.enableMultilingual") }}
              </el-checkbox>
            </el-form-item>
            <el-form-item v-if="!options?.hideDisplayInSearchResult">
              <el-checkbox
                v-model="model.displayInSearchResult"
                data-cy="enable-search"
                >{{ t("common.displayInSearchResult") }}
              </el-checkbox>
            </el-form-item>

            <el-form-item v-if="controlModel?.value === 'RichEditor'">
              <TinymceConfig v-model="model.settings" @reset="onSettingReset" />
            </el-form-item>
          </el-form>
          <EditFieldPreview :field="model" />
        </el-tab-pane>
        <el-tab-pane
          v-if="showValidationTab"
          :label="tabs[2].displayName"
          :name="tabs[2].value"
        >
          <EditFieldValidation
            ref="fieldValidation"
            :field="model"
            @save="onEditorSave"
          />
        </el-tab-pane>
      </el-tabs>
    </el-scrollbar>

    <template #footer>
      <DialogFooterBar
        :permission="{
          feature: 'contentType',
          action: 'edit',
        }"
        @confirm="onEditorSave"
        @cancel="show = false"
      />
    </template>
  </el-dialog>
</template>
<script setup lang="ts">
import { computed, onMounted, ref, toRefs, watch, type Ref } from "vue";
import type { ElForm, FormItemRule, FormRules } from "element-plus";
import { useControlTypes, getFieldRules } from "@/hooks/use-control-types";
import KeyValueEditor from "@/components/basic/key-value-editor.vue";

import type { Field } from "@/components/field-control/types";
import { cloneDeep, isEmpty } from "lodash-es";
import type { Property, PropertyOptions } from "@/global/control-type";
import EditFieldValidation from "./edit-field-validation.vue";
import type EditFieldValidationType from "./edit-field-validation.vue";
import EditFieldPreview from "./edit-field-preview.vue";
import TinymceConfig from "@/components/tinymce-editor-config/index.vue";
import FieldPreview from "./field-preview.vue";
import MonacoEditor from "@/components/monaco-editor/index.vue";
import {
  provideDefaultFileNotFound,
  provideDefaultFileOpen,
} from "@/components/monaco-editor/config";
import {
  editColumIsUniqueNameRule,
  letterAndDigitStartRule,
  rangeRule,
  requiredRule,
} from "@/utils/validate";

import { useI18n } from "vue-i18n";
import DialogFooterBar from "@/components/dialog-footer-bar/index.vue";
import SelectionEditor from "./selection-editor.vue";
import { useSiteStore } from "@/store/site";
import type { OptionItemType } from "@/global/types";
import { ignoreCaseEqual, ignoreCaseContains } from "@/utils/string";
provideDefaultFileOpen();
provideDefaultFileNotFound();
const { site } = useSiteStore();

interface PropsType {
  modelValue: boolean;
  fields: Property[];
  field?: Property;
  options?: PropertyOptions;
  showDefaultValue?: boolean;
  hidePreviewField?: boolean;
  enableDynamicOptions?: boolean;
}

const props = defineProps<PropsType>();
const { fields } = toRefs(props);
const { t } = useI18n();
const show = ref(true);
const emits = defineEmits<{
  (e: "update:modelValue", value: boolean): void;
}>();

const optionsExample = `const products = k.commerce.product.list();
return products.map(it => {
    return {
        key: it.title,
        value: it.id,
    }
});`
  .replaceAll("\n", "<br/>")
  .replaceAll(" ", "&nbsp;");

const tabs = [
  {
    displayName: t("common.basic"),
    value: "Basic",
  },
  {
    displayName: t("common.advanced"),
    value: "Advanced",
  },
  {
    displayName: t("common.validation"),
    value: "Validation",
  },
] as const;

type TabType = typeof tabs[number]["value"];
const currentTab = ref<TabType>("Basic");

const form = ref<InstanceType<typeof ElForm>>();
const defaultValueForm = ref<InstanceType<typeof ElForm>>();
const fieldValidation = ref<InstanceType<typeof EditFieldValidationType>>();
const advancedForm = ref<InstanceType<typeof ElForm>>();

const defaultModel: () => Property = () => ({
  name: "",
  displayName: "",
  controlType: "TextBox",
  dataType: "String",
  isSummaryField: false,
  multipleLanguage: true,
  editable: true,
  tooltip: "",
  maxLength: 0,
  validations: [],
  settings: {},
  isSystemField: false,
  multipleValue: false,
  displayInSearchResult: true,
  selectionOptions: [],
  order: 0,
});

const model = ref<Property>(defaultModel());
const isNew = computed(() => !props.field);
let initField: string | undefined = undefined;

const defaultValueField: Ref<Partial<Field>> = computed(() => {
  return {
    name: "defaultValue",
    prop: "defaultValue",
    displayName: t("common.defaultValue"),
    selectionOptions: model.value.selectionOptions ?? [],
  };
});

const defaultValueCss = computed(() => {
  const fullWidth = [
    "MediaFile",
    "AdvancedMediaFile",
    "File",
    "CheckBox",
    "RadioBox",
    "RichEditor",
    "TextArea",
  ].includes(controlModel.value?.value ?? "TextBox");
  return {
    "field-default-value": true,
    "w-full": fullWidth,
    "w-3/5": !fullWidth,
  };
});
const hasDefaultValue = computed(() => {
  return !isEmpty(model.value.settings["defaultValue"]);
});
const defaultValueRules = computed(() => getFieldRules(model.value));
const showDynamicOptions = computed(
  () => props.enableDynamicOptions && model.value.settings["isDynamicOptions"]
);

onMounted(() => {
  if (show.value) {
    form.value?.resetFields();
    validationForm.value?.resetFields();
    defaultValueForm.value?.resetFields();
    advancedForm.value?.resetFields();
    currentTab.value = "Basic";
    if (props.field) {
      model.value = props.field;
      initField = JSON.stringify(props.field);
    } else {
      model.value = defaultModel();
    }

    // fallback to site settings
    const { tinymceSettings, enableTinymceToolbarSettings } = site ?? {};
    if (enableTinymceToolbarSettings) {
      if (!model.value.settings["toolbar"] && tinymceSettings.toolbar) {
        model.value.settings["toolbar"] = tinymceSettings.toolbar;
      }
      if (
        !model.value.settings["font_formats"] &&
        tinymceSettings.font_formats
      ) {
        model.value.settings["font_formats"] = tinymceSettings.font_formats;
      }
      if (
        !model.value.settings["font_size_formats"] &&
        tinymceSettings.font_size_formats
      ) {
        model.value.settings["font_size_formats"] =
          tinymceSettings.font_size_formats;
      }
    }

    onControlTypeChange();
  }
});

const rules: FormRules = {
  name: [
    !isNew.value ? "" : (requiredRule(t("common.fieldRequiredTips")) as any),
    rangeRule(1, 50),
    letterAndDigitStartRule(),
    editColumIsUniqueNameRule(isNew, fields),
  ],
  options: [
    {
      validator(_rule, _value, callback) {
        if (model.value.selectionOptions.length === 0) {
          callback(new Error(t("common.fieldRequiredTips")));
        } else {
          callback();
        }
      },
      trigger: "blur",
    },
  ],
};
const {
  controlTypes,
  getControlType,
  getAvailableControlTypes,
  defaultAdvancedMediaFileOptions,
} = useControlTypes();
const availableControlTypes = computed(() => {
  if (props.field) {
    if (props.field.controlType === "AdvancedMediaFile") {
      return [];
    }
    return getAvailableControlTypes(props.field.dataType);
  }
  return controlTypes;
});

const advancedMediaFileMetaKeyRules = computed<FormItemRule[]>(() => {
  return [
    requiredRule(t("common.nameRequiredTips")),
    {
      validator: (_rule: unknown, value: string) => {
        return !ignoreCaseContains(["src"], value);
      },
      message: () => t("common.columnIsReservedWord", { columnName: "src" }),
    },
    {
      validator: (_rule: unknown, value: string) => {
        if (!value) {
          return true;
        }
        const { pattern } = letterAndDigitStartRule();
        return pattern.test(value);
      },
      message: () => t("common.letterAndDigitsAllowedToStarting"),
    },
    {
      validator(_rule: unknown, value: string) {
        return (
          (model.value.settings.meta ?? []).filter(
            (it: OptionItemType) => it.key && ignoreCaseEqual(it.key, value)
          ).length <= 1
        );
      },
      message: () => t("common.keyHasBeenTakenTips"),
    },
  ];
});

watch(
  () => model.value.controlType,
  () => {
    model.value.dataType = controlModel.value?.dataType || "String";
  }
);

const controlModel = computed(() => {
  if (model.value.controlType) {
    return getControlType(model.value?.controlType);
  }
  return undefined;
});

const showOptionForm = computed(() =>
  isInControls(["selection", "checkbox", "radiobox"])
);
const showMultipleCheckbox = computed(() =>
  isInControls(["mediafile", "file", "advancedmediafile"])
);
const showMultipleLanguage = computed(() => {
  if (props.options?.hideMultipleLanguage) {
    return false;
  }
  let controlValues = [
    "TextBox",
    "TextArea",
    "RichEditor",
    "MediaFile",
    "KeyValues",
  ];
  if (controlModel.value && controlValues.includes(controlModel.value.value)) {
    return true;
  } else {
    return false;
  }
});
const showValidationTab = computed(
  () =>
    !props.options?.hideValidation &&
    !model.value.isSystemField &&
    !isInControls(["boolean", "switch"])
);

const validationForm = computed(() => (fieldValidation.value as any)?.form);

function isInControls(lowerControlNames: string[]) {
  if (model.value.controlType) {
    return lowerControlNames.includes(model.value.controlType.toLowerCase());
  }
  return false;
}

async function onEditorSave() {
  try {
    await form.value?.validate();
    await defaultValueForm.value?.validate();
    console.log("basic success");
  } catch (error) {
    currentTab.value = "Basic";
    return;
  }
  try {
    await advancedForm.value?.validate();
    console.log("advanced success");
  } catch {
    currentTab.value = "Advanced";
    return;
  }
  try {
    await validationForm.value?.validate();
    console.log("validation success");
  } catch {
    currentTab.value = "Validation";
    return;
  }

  const field = cloneDeep(model.value);
  if (!showMultipleLanguage.value) {
    field.multipleLanguage = false;
  }
  if (!field.displayName) {
    field.displayName = field.name;
  }
  field.validations?.forEach((item) => {
    delete item.name;
  });

  if (isNew.value) {
    const systemFieldIndex = fields.value.findIndex((x) => x.isSystemField);
    const normalField = fields.value
      .filter((it) => !it.isSystemField)
      .map((it) => it.order);
    field.order = normalField.length ? Math.max(...normalField) + 1 : 0;
    fields.value.splice(systemFieldIndex, 0, field);
  } else {
    if (initField !== JSON.stringify(model.value)) {
      const fieldIndex = fields.value.findIndex((x) => x.name === field.name);
      fields.value[fieldIndex] = field;
    }
  }

  if (site?.enableTinymceToolbarSettings) {
    if (field.settings["toolbar"] === site?.tinymceSettings.toolbar) {
      delete field.settings["toolbar"];
    }
    if (field.settings["font_formats"] === site?.tinymceSettings.font_formats) {
      delete field.settings["font_formats"];
    }
    if (
      field.settings["font_size_formats"] ===
      site?.tinymceSettings.font_size_formats
    ) {
      delete field.settings["font_size_formats"];
    }
  }

  show.value = false;
}

function onMultipleValueChanged() {
  model.value.settings["defaultValue"] = undefined;
}

function onControlTypeChange() {
  // AdvancedMediaFile special case
  if (model.value.controlType === "AdvancedMediaFile") {
    model.value.settings = model.value.settings || {};
    model.value.settings["meta"] = model.value.settings["meta"] || [
      { key: "alt", value: "{{alt}}" },
    ];
  }
}

function onSettingReset(key: string) {
  const tinymceSettings = site.tinymceSettings;
  model.value.settings[key] = tinymceSettings[key] || undefined;
}
</script>

<style lang="scss" scoped>
.hide-tab {
  :deep(.el-tabs__header) {
    display: none;
  }
}

.field-default-value {
  :deep(.el-form-item__content > div) {
    width: 100%;
  }
}
</style>
