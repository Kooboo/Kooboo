<template>
  <el-form-item
    v-if="controlModel"
    :label="getTitleLabel(field, controlModel.value)"
    :prop="field.prop"
    :rules="rules"
    :required="field.required"
    :class="cssClass"
  >
    <el-input
      v-if="controlModel.value === 'TextBox'"
      v-model="model[field.prop]"
      :placeholder="field.toolTip"
      style="width: 100%"
      v-bind="field.settings"
    />
    <el-input
      v-else-if="controlModel.value === 'TextArea'"
      v-model="model[field.prop]"
      class="break-normal"
      type="textarea"
      :autosize="{ minRows: 8, maxRows: 15 }"
      style="width: 100%"
      :placeholder="field.toolTip"
      v-bind="field.settings"
    />
    <KEditor
      v-else-if="controlModel.value === 'RichEditor'"
      v-model="model[field.prop]"
      :min_height="385"
      v-bind="fieldSettings"
    />
    <el-select
      v-else-if="controlModel.value === 'Selection'"
      v-model="model[field.prop]"
      :placeholder="field.toolTip"
      clearable
      filterable
      v-bind="field.settings"
    >
      <el-option
        v-for="opt in field.selectionOptions"
        :key="opt.value"
        :value="opt.value"
        :label="opt.key"
      >
        <div v-html="opt.key" />
      </el-option>
    </el-select>
    <el-checkbox-group
      v-else-if="controlModel.value === 'CheckBox'"
      v-model="model[field.prop]"
      v-bind="field.settings"
    >
      <el-checkbox
        v-for="opt in field.selectionOptions"
        :key="opt.value"
        size="large"
        :label="opt.value"
        >{{ opt.key }}</el-checkbox
      >
    </el-checkbox-group>
    <el-radio-group
      v-else-if="controlModel.value === 'RadioBox'"
      v-model="model[field.prop]"
      v-bind="field.settings"
    >
      <el-radio
        v-for="opt in field.selectionOptions"
        :key="opt.value"
        size="large"
        :label="opt.value"
        >{{ opt.key }}</el-radio
      >
    </el-radio-group>
    <el-switch
      v-else-if="controlModel.value === 'Switch'"
      v-model="model[field.prop]"
      v-bind="field.settings"
    />
    <el-date-picker
      v-else-if="controlModel.value === 'DateTime'"
      v-model="model[field.prop]"
      type="datetime"
      popper-class="filed-control-el-date-picker"
      :placeholder="field.toolTip"
      v-bind="field.settings"
    />
    <el-color-picker
      v-else-if="controlModel.value === 'ColorPicker'"
      v-model="model[field.prop]"
      popper-class="filed-control-el-color-picker"
      show-alpha
      v-bind="field.settings"
    />
    <el-input-number
      v-else-if="controlModel.value === 'Number'"
      v-model="model[field.prop]"
      controls-position="right"
      :placeholder="field.toolTip"
      v-bind="field.settings"
    />
    <SelectMediaFile
      v-else-if="controlModel.value === 'MediaFile'"
      v-model="model[field.prop]"
      :multiple="field.multipleValue"
      :placeholder="field.toolTip"
      :validations="field.validations"
      v-bind="field.settings"
    />
    <SelectAdvancedMediaFile
      v-else-if="controlModel.value === 'AdvancedMediaFile'"
      v-model="model[field.prop]"
      :multiple="field.multipleValue"
      :placeholder="field.toolTip"
      v-bind="field.settings"
    />
    <SelectFile
      v-else-if="controlModel.value === 'File'"
      v-model="model[field.prop]"
      :multiple="field.multipleValue"
      :placeholder="field.toolTip"
      v-bind="field.settings"
    />
    <KeyValueEditor
      v-else-if="controlModel.value === 'KeyValues'"
      v-model="model[field.prop]"
      :multiple="field.multipleValue"
      :placeholder="field.toolTip"
      v-bind="field.settings"
      class="w-1/2 min-w-300px"
    />
    <div
      v-if="tooltipBelow"
      class="w-full py-8 text-s text-666"
      data-cy="tooltip-below"
    >
      {{ field.toolTip }}
    </div>
  </el-form-item>
</template>
<script lang="ts" setup>
import type { Field } from "./types";
import KEditor from "@/components/k-editor/index.vue";
import { computed } from "vue";
import type { ControlName } from "@/hooks/use-control-types";
import { useControlTypes } from "@/hooks/use-control-types";
import SelectMediaFile from "@/components/select-media-file/index.vue";
import SelectAdvancedMediaFile from "@/components/select-media-file/advanced.vue";
import SelectFile from "@/components/select-file/index.vue";
import { useMultilingualStore } from "@/store/multilingual";
import { useI18n } from "vue-i18n";
import { useSiteStore } from "@/store/site";
import type { FormItemRule } from "element-plus";

interface PropsType {
  field: Field;
  model: Record<string, any>;
  cssClass?: any;
  rules?: FormItemRule[];
}

const { site } = useSiteStore();

const fieldSettings = computed(() => {
  const settings = props.field.settings;
  if (props.field.controlType !== "RichEditor") {
    return settings;
  }
  if (site.enableTinymceToolbarSettings) {
    if (!settings["toolbar"] && site.tinymceSettings.toolbar) {
      settings["toolbar"] = site.tinymceSettings.toolbar;
    }
    if (!settings["font_formats"] && site.tinymceSettings.font_formats) {
      settings["font_formats"] = site.tinymceSettings.font_formats;
    }
    if (
      !settings["font_size_formats"] &&
      site.tinymceSettings.font_size_formats
    ) {
      settings["font_size_formats"] = site.tinymceSettings.font_size_formats;
    }
  }
  return settings;
});

const { t } = useI18n();
const multilingualStore = useMultilingualStore();
const props = defineProps<PropsType>();
const { getControlType } = useControlTypes();
const controlModel = computed(() => {
  if (typeof props.field.controlType !== "string") {
    return {
      value: props.field.controlType,
    };
  }
  return getControlType(props.field.controlType);
});
const tooltipBelow = computed(() => {
  if (props.field.toolTip && controlModel.value) {
    const controls: ControlName[] = [
      "CheckBox",
      "RadioBox",
      "Switch",
      "RichEditor",
      "ColorPicker",
    ];
    return (
      typeof controlModel.value.value === "string" &&
      controls.includes(controlModel.value.value)
    );
  }
  return false;
});

const getTitleLabel = (field: Field, controlModel: string) => {
  if (
    ["TextBox", "TextArea", "RichEditor"].includes(controlModel) &&
    field.isMultilingual
  ) {
    if (field.lang === multilingualStore.default) {
      return multilingualStore.selected.length > 1
        ? field.displayName +
            " - " +
            field.lang +
            " (" +
            t("common.default") +
            ")"
        : field.displayName;
    } else return field.displayName + " - " + field.lang;
  } else return field.displayName;
};
</script>

<style>
.filed-control-el-date-picker .el-picker-panel__icon-btn {
  margin-top: 0;
}
</style>
