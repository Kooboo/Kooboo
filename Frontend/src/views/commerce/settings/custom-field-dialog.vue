<script setup lang="ts">
import { computed, ref, watch } from "vue";
import type { ElForm, FormRules } from "element-plus";
import {
  letterAndDigitStartRule,
  rangeRule,
  requiredRule,
} from "@/utils/validate";

import { useI18n } from "vue-i18n";
import DialogFooterBar from "@/components/dialog-footer-bar/index.vue";
import type { CustomField } from "@/api/commerce/settings";
import { fieldTypes } from "../custom-data/custom-field";
import type { ContentFolder } from "@/api/content/content-folder";
import { getList as getContentFolders } from "@/api/content/content-folder";
import EditFieldValidation from "@/components/fields-editor/edit-field-validation.vue";
import { ignoreCaseEqual, ignoreCaseContains } from "@/utils/string";
import { get } from "lodash-es";

interface PropsType {
  modelValue: boolean;
  field: CustomField;
  fields: CustomField[];
}

const props = defineProps<PropsType>();
const { t } = useI18n();
const show = ref(true);
const model = ref<CustomField>(JSON.parse(JSON.stringify(props.field)));
const fieldsWithOutCurrent = computed(() =>
  props.fields.filter((f) => !ignoreCaseEqual(f.name, props.field.name))
);
const contentFolders = ref<ContentFolder[]>([]);
const fieldValidation = ref();
const form = ref();

const emit = defineEmits<{
  (e: "update:modelValue", value: boolean): void;
  (e: "success", value: CustomField): void;
}>();

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

const rules: FormRules = {
  name: [
    requiredRule(t("common.fieldRequiredTips")),
    rangeRule(1, 50),
    letterAndDigitStartRule(),
    {
      validator(_rule: unknown, value: string) {
        const lowerCaseValue = value?.toLowerCase();
        return !fieldsWithOutCurrent.value.find(
          (f) => f.name.toLowerCase() == lowerCaseValue
        );
      },
      message: () => t("common.valueHasBeenTakenTips"),
    },
  ],
  contentFolder: model.value.isSystemField
    ? []
    : [requiredRule(t("common.fieldRequiredTips"))],
};

const disableMultiple = computed(() => {
  return get(model.value, "options.disableMultiple", false);
});

async function onEditorSave() {
  try {
    await fieldValidation.value?.form?.validate();
  } catch (error) {
    currentTab.value = "Validation";
    throw Error("Validation not valid");
  }

  try {
    await form.value.validate();
  } catch (error) {
    currentTab.value = "Basic";
    throw Error("Basic not valid");
  }

  emit("success", model.value);
  show.value = false;
}

watch(
  () => model.value.type,
  async (type) => {
    if (ignoreCaseContains(["Content", "RichEditor"], type)) {
      model.value.isSummaryField = false;
    }
    if (!contentFolders.value?.length) {
      contentFolders.value = await getContentFolders();
    }
  },
  {
    immediate: true,
  }
);
</script>

<template>
  <el-dialog
    :model-value="show"
    width="700px"
    :close-on-click-modal="false"
    :title="t('common.fieldEditor')"
    custom-class="el-dialog--zero-padding"
    @closed="emit('update:modelValue', false)"
  >
    <el-scrollbar max-height="60vh">
      <el-tabs v-model="currentTab">
        <el-tab-pane :label="tabs[0].displayName" :name="tabs[0].value">
          <el-form
            ref="form"
            :model="model"
            :rules="rules"
            label-position="top"
          >
            <el-form-item prop="name" :label="t('common.name')">
              <div class="w-full flex items-center">
                <el-input
                  v-model="model.name"
                  :disabled="!!model.name && model.isSystemField"
                  class="w-3/5"
                  data-cy="field-name-input"
                />
                <el-checkbox
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

            <el-form-item prop="type" :label="t('common.controlType')">
              <div class="w-full flex items-center">
                <el-select
                  v-model="model.type"
                  :disabled="model.isSystemField"
                  class="w-3/5"
                  data-cy="control-type-dropdown"
                >
                  <el-option
                    v-for="(value, key) in fieldTypes"
                    :key="key"
                    :value="key"
                    :label="value.displayName"
                  />
                </el-select>
                <el-checkbox
                  v-if="fieldTypes[model.type]?.multiple"
                  v-model="model.multiple"
                  :disabled="disableMultiple"
                  class="ml-12"
                  data-cy="enable-multiple"
                >
                  {{ t("common.enableMultiple") }}
                </el-checkbox>
              </div>
            </el-form-item>
            <el-form-item
              v-if="fieldTypes[model.type]?.selection"
              :label="t('common.options')"
            >
              <SelectionEditor :options="model.selectionOptions" />
            </el-form-item>

            <el-form-item
              v-if="model.type == 'Content'"
              :label="t('common.contentFolder')"
              prop="contentFolder"
            >
              <el-select v-model="model.contentFolder" class="w-3/5">
                <el-option
                  v-for="item in contentFolders"
                  :key="item.name"
                  :value="item.name"
                  :label="item.name || item.displayName"
                />
              </el-select>
            </el-form-item>
          </el-form>
        </el-tab-pane>
        <el-tab-pane
          v-if="!model.isSystemField"
          :label="tabs[1].displayName"
          :name="tabs[1].value"
        >
          <el-form-item>
            <el-checkbox v-model="model.editable">
              {{ t("common.userEditable") }}
            </el-checkbox>
          </el-form-item>

          <el-form-item v-if="fieldTypes[model.type].multilingual">
            <el-checkbox v-model="model.multilingual">
              {{ t("common.enableMultilingual") }}
            </el-checkbox>
          </el-form-item>

          <el-form-item v-if="model.multiple && model.type == 'Content'">
            <el-checkbox v-model="model.allowRepetition">
              {{ t("common.allowRepetition") }}
            </el-checkbox>
          </el-form-item>
        </el-tab-pane>
        <el-tab-pane
          v-if="!model.isSystemField"
          :label="tabs[2].displayName"
          :name="tabs[2].value"
        >
          <EditFieldValidation
            ref="fieldValidation"
            :field="{ controlType: model.type, validations: model.validations }"
            @save="onEditorSave"
          />
        </el-tab-pane>
      </el-tabs>
    </el-scrollbar>

    <template #footer>
      <DialogFooterBar @confirm="onEditorSave" @cancel="show = false" />
    </template>
  </el-dialog>
</template>

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
