<template>
  <el-form
    ref="form"
    label-position="top"
    :model="field"
    @submit.prevent
    @keydown.enter="emit('save')"
  >
    <el-form-item
      v-if="unselectedOptions.length"
      :label="t('common.selectValidationRules')"
    >
      <div class="w-full flex items-center">
        <el-select
          v-model="selectedValidation"
          class="w-3/5"
          value-key="type"
          data-cy="validation-rules-dropdown"
        >
          <el-option
            v-for="item in unselectedOptions"
            :key="item.type"
            :label="item.name"
            :value="item"
            data-cy="validation-rules-option"
          />
        </el-select>
        <el-button
          circle
          class="ml-12"
          data-cy="add-validation"
          @click="onAdd()"
        >
          <el-icon class="iconfont icon-a-addto text-blue" />
        </el-button>
      </div>
    </el-form-item>
    <div
      v-for="(item, index) in field.validations"
      :key="item.type"
      :data-cy="item.type"
    >
      <div class="flex justify-between items-center py-5px">
        <span class="font-bold">{{ item.name || getName(item.type) }}</span>
        <el-button circle data-cy="remove-validation" @click="onRemove(index)">
          <el-icon class="iconfont icon-delete text-orange" />
        </el-button>
      </div>
      <div class="bg-gray/60 p-16 rounded-4px dark:bg-[#333]">
        <el-form-item
          v-if="['stringLength', 'range'].includes(item.type)"
          :label="t('common.rangeValue')"
        >
          <div class="flex items-center justify-between">
            <el-form-item
              :prop="'validations.' + index + '.min'"
              :rules="requiredRule(t('common.fieldRequiredTips'))"
            >
              <el-input-number
                v-model="item.min"
                :placeholder="t('common.minimalValue')"
                class="w-full"
                data-cy="min"
              />
            </el-form-item>
            <span class="mx-8">-</span>
            <el-form-item
              :prop="'validations.' + index + '.max'"
              :rules="requiredRule(t('common.fieldRequiredTips'))"
            >
              <el-input-number
                v-model="item.max"
                :placeholder="t('common.maximumValue')"
                class="w-full"
                data-cy="max"
              />
            </el-form-item>
          </div>
        </el-form-item>
        <el-form-item
          v-else-if="['min', 'minLength', 'minChecked'].includes(item.type)"
          :label="t('common.minimalValue')"
          :prop="'validations.' + index + '.value'"
          :rules="requiredRule(t('common.fieldRequiredTips'))"
        >
          <el-input-number
            v-model="item.value"
            :placeholder="t('common.minimalValue')"
            class="w-full"
            data-cy="min"
          />
        </el-form-item>
        <el-form-item
          v-else-if="['max', 'maxLength', 'maxChecked'].includes(item.type)"
          :label="t('common.maximumValue')"
          :prop="'validations.' + index + '.value'"
          :rules="requiredRule(t('common.fieldRequiredTips'))"
        >
          <el-input-number
            v-model="item.value"
            :placeholder="t('common.maximumValue')"
            class="w-full"
            data-cy="max"
          />
        </el-form-item>
        <el-form-item
          v-else-if="item.type === 'regex'"
          :label="t('common.pattern')"
          :prop="'validations.' + index + '.pattern'"
          :rules="requiredRule(t('common.fieldRequiredTips'))"
        >
          <el-input
            v-model="item.pattern"
            :placeholder="t('common.fieldPattern')"
            data-cy="regex-input"
          />
        </el-form-item>
        <el-form-item
          v-else-if="['fileSize'].includes(item.type)"
          :label="t('common.maximumValue')"
          :prop="'validations.' + index + '.value'"
          :rules="requiredRule(t('common.fieldRequiredTips'))"
        >
          <div class="w-full flex gap-12">
            <el-input-number
              v-model="item.value"
              class="flex-1"
              :placeholder="t('common.maximumValue')"
              data-cy="max"
            />
            <el-select v-model="item.pattern" class="flex-1">
              <el-option label="KB" value="kb" />
              <el-option label="MB" value="mb" />
            </el-select>
          </div>
        </el-form-item>
        <el-form-item
          v-else-if="['fileTypes'].includes(item.type)"
          :label="t('common.fileTypes')"
          :prop="'validations.' + index + '.pattern'"
          :rules="requiredRule(t('common.fieldRequiredTips'))"
        >
          <el-select
            :model-value="parseFileTypes(item.pattern)"
            multiple
            filterable
            allow-create
            default-first-option
            class="w-full"
            data-cy="file-types-dropdown"
            @change="onFileTypesChange($event, item)"
          >
            <el-option label=".jpg" value=".jpg" />
            <el-option label=".jpeg" value=".jpeg" />
            <el-option label=".png" value=".png" />
            <el-option label=".gif" value=".gif" />
            <el-option label=".svg" value=".svg" />
            <el-option label=".webp" value=".webp" />
            <el-option label=".ico" value=".ico" />
          </el-select>
        </el-form-item>
        <el-form-item :label="t('common.validationFailedMessage')">
          <template #label>
            <label class="el-form-item__label">
              {{ t("common.validationFailedMessage") }}
              <Tooltip
                v-if="messageTooltips[item.type]"
                :tip="messageTooltips[item.type]"
                :enable-html="true"
                placement="top"
              />
            </label>
          </template>
          <el-input
            v-model="item.msg"
            :placeholder="t('common.errorMessage')"
            data-cy="error-message"
          />
        </el-form-item>
      </div>
    </div>
  </el-form>
</template>

<script lang="ts" setup>
import type {
  FieldValidation,
  ValidationRuleType,
} from "@/global/control-type";
import type { ElForm } from "element-plus";
import { computed, ref, watch } from "vue";
import { requiredRule } from "@/utils/validate";

import { useI18n } from "vue-i18n";
interface PropsType {
  field: { controlType: string; validations: FieldValidation[] };
}
const props = defineProps<PropsType>();
const { t } = useI18n();

const messageTooltips: Partial<Record<ValidationRuleType, string>> = {
  fileTypes: t("common.exampleCode", {
    code: t("common.fileTypesNotAllowed", {
      types: "{types}",
    }),
  }),
  fileSize: t("common.exampleCode", {
    code: t("common.fileSizeExceedLimitTips", {
      size: "{size}",
    }),
  }),
};

const form = ref<InstanceType<typeof ElForm>>();

const validationOptions: FieldValidation[] = [
  {
    name: t("common.required"),
    type: "required",
  },
  {
    name: t("common.stringLength"),
    type: "stringLength",
    min: 0,
    max: 0,
  },
  {
    name: t("common.range"),
    type: "range",
    min: 0,
    max: 0,
  },
  {
    name: t("common.regex"),
    type: "regex",
    pattern: "",
  },
  {
    name: t("common.min"),
    type: "min",
    value: 0,
  },
  {
    name: t("common.max"),
    type: "max",
    value: 0,
  },
  {
    name: t("common.minStringLength"),
    type: "minLength",
    value: 0,
  },
  {
    name: t("common.maxStringLength"),
    type: "maxLength",
    value: 0,
  },
  {
    name: t("common.minChecked"),
    type: "minChecked",
    value: 0,
  },
  {
    name: t("common.maxChecked"),
    type: "maxChecked",
    value: 0,
  },
  {
    name: t("common.fileSize"),
    type: "fileSize",
    value: 1,
    pattern: "mb",
  },
  {
    name: t("common.fileTypes"),
    type: "fileTypes",
  },
];
const selectedValidation = ref({} as FieldValidation);
const availableOptions = computed(() => {
  let options: ValidationRuleType[] = [];
  switch (props.field.controlType.toLowerCase()) {
    case "textbox":
    case "textarea":
      options = ["required", "stringLength", "minLength", "maxLength", "regex"];
      break;
    case "content":
    case "checkbox":
      options = ["required", "minChecked", "maxChecked"];
      break;
    case "number":
      options = ["required", "range", "regex", "min", "max"];
      break;
    case "mediafile":
      options = ["required", "fileSize", "fileTypes"];
      break;
    default:
      options = ["required"];
      break;
  }
  return options;
});
const selectedOptions = computed(() => {
  return props.field.validations.map((item) => item.type);
});
const unselectedOptions = computed(() => {
  const result = validationOptions.filter(
    (item) =>
      availableOptions.value.includes(item.type) &&
      !selectedOptions.value.includes(item.type)
  );
  return result;
});
watch(
  () => unselectedOptions.value,
  (val) => {
    selectedValidation.value = val[0];
  },
  {
    immediate: true,
  }
);
watch(
  () => availableOptions.value,
  (val) => {
    const removed = props.field.validations.filter(
      (item) => !val.includes(item.type)
    );

    for (const i of removed) {
      const index = props.field.validations.findIndex((f) => f == i);
      if (index > -1) props.field.validations.splice(index, 1);
    }
  }
);

function getName(type: string) {
  return validationOptions.find((item) => item.type === type)?.name;
}

function onAdd() {
  props.field.validations.push(selectedValidation.value);
}
function onRemove(index: number) {
  props.field.validations.splice(index, 1);
}

const emit = defineEmits<{
  (e: "save"): void;
}>();

function parseFileTypes(value: string | undefined) {
  try {
    if (!value) {
      return [];
    }
    if (Array.isArray(value)) {
      return value;
    }
    return value.split(",");
  } catch (e) {
    console.log(["parseFileTypes", e, value]);
    return [];
  }
}

function onFileTypesChange(value: string[], item: FieldValidation) {
  item.pattern = value.join(",");
}

defineExpose({
  form,
});
</script>
