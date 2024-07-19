<template>
  <el-dialog
    :model-value="show"
    width="600px"
    :close-on-click-modal="false"
    :title="t('common.columnSettings')"
    @closed="emits('update:modelValue', false)"
  >
    <el-form
      ref="columnForm"
      :model="columnModel"
      :rules="columnModelRules"
      label-position="top"
      @submit.prevent
      @keydown.enter="onEditorSave"
    >
      <el-form-item v-if="isNew" prop="name" :label="t('common.name')">
        <el-input v-model="columnModel.name" data-cy="column-name" />
      </el-form-item>
      <el-form-item v-else :label="t('common.name')">
        <el-input v-model="columnModel.name" disabled data-cy="control-type" />
      </el-form-item>

      <el-form-item :label="t('common.controlType')">
        <el-select
          v-model="columnModel.controlType"
          class="w-full"
          :disabled="availableControlTypes.length <= 1"
        >
          <el-option
            v-for="item in availableControlTypes"
            :key="item.value"
            :value="item.value"
            :label="item.displayName"
            data-cy="control-type-opt"
          />
        </el-select>
      </el-form-item>
      <template v-if="dbType === 'Database'">
        <el-form-item
          v-if="showStringLengthForm"
          prop="length"
          :label="t('common.length')"
        >
          <el-input-number
            v-model="columnModel.length"
            :min="1"
            :precision="0"
            :placeholder="t('common.length')"
            data-cy="length"
            @keydown.enter="onBlur($event)"
          />
        </el-form-item>
      </template>

      <el-form-item
        v-if="showOptionForm"
        prop="options"
        :label="t('common.options')"
      >
        <div
          v-for="(item, index) in (columnModel.setting as ColumnSetting).options"
          :key="index"
          class="flex items-center space-x-4 mb-16"
        >
          <el-form-item
            :prop="'setting.options.' + index + '.key'"
            :rules="requiredRule(t('common.fieldRequiredTips'))"
          >
            <el-input
              v-model="item.key"
              :placeholder="t('common.displayName')"
            />
          </el-form-item>

          <el-form-item
            :prop="'setting.options.' + index + '.value'"
            :rules="requiredRule(t('common.fieldRequiredTips'))"
          >
            <el-input v-model="item.value" :placeholder="t('common.value')" />
          </el-form-item>
          <el-button circle @click="removeOption(item)">
            <el-icon class="iconfont icon-delete text-orange" />
          </el-button>
        </div>
        <div class="w-full">
          <el-button circle @click="addOption()">
            <el-icon class="iconfont icon-a-addto text-blue" />
          </el-button>
        </div>
      </el-form-item>
      <template v-if="isNumber && dbType === 'Database'">
        <el-form-item>
          <el-checkbox
            v-model="columnModel.isIncremental"
            size="large"
            :disabled="disableIncremental"
            >{{ t("common.incremental") }}</el-checkbox
          >
        </el-form-item>

        <el-form-item
          v-if="columnModel.isIncremental"
          prop="seed"
          :label="t('common.seed')"
        >
          <el-input-number v-model="columnModel.seed" :min="0" :precision="0" />
        </el-form-item>
        <el-form-item
          v-if="columnModel.isIncremental"
          prop="scale"
          :label="t('common.scale')"
        >
          <el-input-number
            v-model="columnModel.scale"
            :min="1"
            :precision="0"
            :placeholder="t('common.scale')"
          />
        </el-form-item>
      </template>

      <el-form-item>
        <template v-if="dbType === 'Database'">
          <el-checkbox
            v-model="columnModel.isPrimaryKey"
            size="large"
            data-cy="primary-key"
          >
            {{ t("common.primaryKey") }}
          </el-checkbox>
          <el-checkbox
            v-model="columnModel.isUnique"
            size="large"
            data-cy="unique"
          >
            {{ t("common.unique") }}
          </el-checkbox>
        </template>
        <el-checkbox v-model="columnModel.isIndex" size="large" data-cy="index">
          {{ t("common.index") }}
        </el-checkbox>
      </el-form-item>
    </el-form>

    <template #footer>
      <DialogFooterBar
        :permission="{
          feature: 'database',
          action: 'edit',
        }"
        @confirm="onEditorSave"
        @cancel="show = false"
      />
    </template>
  </el-dialog>
</template>
<script setup lang="ts">
import { computed, onMounted, ref, toRefs, watch } from "vue";
import type { ElForm, FormRules } from "element-plus";
import type { DatabaseColumn, DatabaseType } from "@/api/database";
import type { ColumnSetting } from "@/api/database";
import { useControlTypes } from "../use-control-types";
import type { KeyValue } from "@/global/types";
import { cloneDeep } from "lodash-es";
import {
  editColumIsUniqueNameRule,
  rangeRule,
  requiredRule,
  letterAndDigitStartRule,
} from "@/utils/validate";
import DialogFooterBar from "@/components/dialog-footer-bar/index.vue";

import { useI18n } from "vue-i18n";
interface PropsType {
  modelValue: boolean;
  columns: DatabaseColumn[];
  column?: DatabaseColumn;
  dbType: DatabaseType;
}
const show = ref(true);

const emits = defineEmits<{
  (e: "update:modelValue", value: boolean): void;
}>();

const props = defineProps<PropsType>();
const { columns } = toRefs(props);
let initValue: string | undefined = undefined;

const { t } = useI18n();
const columnForm = ref<InstanceType<typeof ElForm>>();
const defaultColumnModel: () => DatabaseColumn = () => ({
  name: "",
  dataType: "String",
  isIncremental: false,
  seed: 0,
  scale: 1,
  isIndex: false,
  isPrimaryKey: false,
  isUnique: false,
  controlType: "TextBox",
  isSystem: false,
  length: 1024,
  setting: {
    options: [],
  },
});

const columnModel = ref<DatabaseColumn>(defaultColumnModel());
const isNew = computed(() => !props.column);
const isNumber = computed(
  () => columnModel.value.controlType?.toLowerCase() === "number"
);
const disableIncremental = ref(false);
const onBlur = (e: any) => {
  e.target.blur();
  onEditorSave();
};
function replacer(key: string, value: any) {
  if (key === "dataType") {
    return undefined;
  }
  return value;
}

onMounted(() => {
  if (show.value) {
    columnForm.value?.resetFields();
    if (props.column) {
      columnModel.value = props.column;
      disableIncremental.value =
        isNumber.value && !columnModel.value.isIncremental;
      initValue = JSON.stringify(props.column, replacer);
    } else {
      columnModel.value = defaultColumnModel();
      disableIncremental.value = false;
    }
  }
});

const columnModelRules: FormRules = {
  name: [
    requiredRule(t("common.fieldRequiredTips")),
    letterAndDigitStartRule(),
    {
      validator(_rule, value: string, callback) {
        if (value.trim().toLowerCase() === "_id") {
          callback(t("common.columnIsReservedWord", { columnName: value }));
          return;
        }
        callback();
      },
    },
    rangeRule(1, 50),
    editColumIsUniqueNameRule(isNew, columns),
  ],
  length: requiredRule(t("common.fieldRequiredTips")),
  seed: requiredRule(t("common.fieldRequiredTips")),
  scale: requiredRule(t("common.fieldRequiredTips")),
  options: [
    {
      validator(_rule, _value, callback) {
        const setting = columnModel.value.setting as ColumnSetting;
        if (setting.options.length === 0) {
          callback(new Error(t("common.fieldRequiredTips")));
        } else {
          callback();
        }
      },
      trigger: "blur",
    },
  ],
};
const { controlTypes, getControlType, getAvailableControlTypes } =
  useControlTypes();
const availableControlTypes = computed(() => {
  if (props.column) {
    return getAvailableControlTypes(props.column.dataType);
  }
  return controlTypes;
});
watch(
  () => columnModel.value.isPrimaryKey,
  (value) => {
    if (value) {
      columnModel.value.isUnique = true;
    }
  }
);
watch(
  () => columnModel.value.isUnique,
  (value) => {
    if (value) {
      columnModel.value.isIndex = true;
    } else if (
      columnModel.value.isIncremental ||
      columnModel.value.isPrimaryKey
    ) {
      columnModel.value.isUnique = true;
    }
  }
);
watch(
  () => columnModel.value.isPrimaryKey,
  (value) => {
    if (value) {
      columnModel.value.isUnique = true;
    }
  }
);
watch(
  () => columnModel.value.isIndex,
  (value) => {
    if (!value && columnModel.value.isUnique) {
      columnModel.value.isIndex = true;
    }
  }
);
watch(
  () => columnModel.value.isIncremental,
  (value) => {
    columnModel.value.isUnique = value;
    columnModel.value.isIndex = value;
  }
);
watch(
  () => columnModel.value.controlType,
  () => {
    columnModel.value.dataType = controlModel.value?.dataType || "String";
  }
);

const controlModel = computed(() => {
  if (columnModel.value.controlType) {
    return getControlType(columnModel.value?.controlType);
  }
  return undefined;
});

const showStringLengthForm = computed(() => {
  if (columnModel.value.controlType) {
    return ["textbox", "textarea", "tinymce"].includes(
      columnModel.value.controlType.toLowerCase()
    );
  }
  return false;
});
const showOptionForm = computed(() => {
  if (columnModel.value.controlType) {
    return ["selection", "checkbox", "radiobox"].includes(
      columnModel.value.controlType.toLowerCase()
    );
  }
  return false;
});

function removeOption(item: KeyValue) {
  const setting = columnModel.value.setting as ColumnSetting;
  setting.options.splice(setting.options.indexOf(item), 1);
}

function addOption() {
  const setting = columnModel.value.setting as ColumnSetting;
  setting.options.push({
    key: "",
    value: "",
  });
  if (showOptionForm.value) {
    columnForm.value?.clearValidate("options");
  }
}

async function onEditorSave() {
  await columnForm.value?.validate(async (valid) => {
    if (valid) {
      const column = cloneDeep(columnModel.value);
      if (!showOptionForm.value) {
        const setting = (column.setting || {}) as ColumnSetting;
        setting.options = [];
      }
      if (!isNumber.value) {
        column.isIncremental = false;
      }
      if (column.isPrimaryKey) {
        columns.value.forEach((item) => (item.isPrimaryKey = false));
      }
      if (isNew.value) {
        columns.value.push(column);
      } else {
        if (initValue !== JSON.stringify(columnModel.value, replacer)) {
          const columnIndex = columns.value.findIndex(
            (x) => x.name === column.name
          );
          columns.value[columnIndex] = column;
        }
      }
      show.value = false;
    }
  });
}
</script>
