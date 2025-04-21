<script setup lang="ts">
import { computed, ref, watch } from "vue";
import type { FormRules } from "element-plus";
import {
  letterAndDigitStartRule,
  rangeRule,
  requiredRule,
} from "@/utils/validate";

import { useI18n } from "vue-i18n";
import DialogFooterBar from "@/components/dialog-footer-bar/index.vue";
import type { ExtensionField } from "@/api/commerce/settings";
import type { ContentFolder } from "@/api/content/content-folder";
import { getList as getContentFolders } from "@/api/content/content-folder";
import { ignoreCaseEqual, ignoreCaseContains } from "@/utils/string";
import { dataTypes } from "./settings";

interface PropsType {
  modelValue: boolean;
  field: ExtensionField;
  fields: ExtensionField[];
}

const props = defineProps<PropsType>();
const { t } = useI18n();
const show = ref(true);
const model = ref<ExtensionField>(JSON.parse(JSON.stringify(props.field)));
const fieldsWithOutCurrent = computed(() =>
  props.fields.filter((f) => !ignoreCaseEqual(f.name, props.field.name))
);
const contentFolders = ref<ContentFolder[]>([]);
const form = ref();

const emit = defineEmits<{
  (e: "update:modelValue", value: boolean): void;
  (e: "success", value: ExtensionField): void;
}>();

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
};

async function onEditorSave() {
  try {
    await form.value.validate();
  } catch (error) {
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
    @closed="emit('update:modelValue', false)"
  >
    <el-scrollbar max-height="60vh">
      <el-form ref="form" :model="model" :rules="rules" label-position="top">
        <el-form-item prop="name" :label="t('common.name')">
          <div class="w-full flex items-center">
            <el-input
              v-model="model.name"
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
        <el-form-item
          v-if="model.isSummaryField"
          :label="t('common.columnWidth')"
        >
          <el-input v-model.number="model.width" class="w-3/5" />
        </el-form-item>
        <el-form-item :label="t('common.displayName')">
          <el-input
            v-model="model.displayName"
            class="w-3/5"
            data-cy="display-name-input"
          />
        </el-form-item>

        <el-form-item prop="type" :label="t('common.dataType')">
          <div class="w-full flex items-center">
            <el-select v-model="model.type" class="w-3/5">
              <el-option
                v-for="(value, key) in dataTypes"
                :key="key"
                :value="key"
                :label="value"
              />
            </el-select>
          </div>
        </el-form-item>
        <el-form-item>
          <el-checkbox v-model="model.editable">
            {{ t("common.editable") }}
          </el-checkbox>
          <el-checkbox v-model="model.filterable">
            {{ t("common.filterable") }}
          </el-checkbox>
          <el-checkbox v-model="model.exportable">
            {{ t("common.exportable") }}
          </el-checkbox>
        </el-form-item>
      </el-form>
    </el-scrollbar>

    <template #footer>
      <DialogFooterBar @confirm="onEditorSave" @cancel="show = false" />
    </template>
  </el-dialog>
</template>

<style lang="scss" scoped>
.field-default-value {
  :deep(.el-form-item__content > div) {
    width: 100%;
  }
}
</style>
