import type { Field } from "@/components/field-control/types";
import type { FormItemRule } from "element-plus";
import { i18n } from "@/modules/i18n";
import { ignoreCaseEqual } from "@/utils/string";

const $t = i18n.global.t;

export type ControlName =
  | "TextBox"
  | "TextArea"
  | "RichEditor"
  | "Selection"
  | "CheckBox"
  | "RadioBox"
  | "Switch"
  | "MediaFile"
  | "AdvancedMediaFile"
  | "File"
  | "DateTime"
  | "Number"
  | "ColorPicker"
  | "KeyValues";

export type ControlDataType =
  | "String"
  | "Array"
  | "Bool"
  | "Undefined"
  | "DateTime"
  | "Number";

export type Control = {
  value: ControlName;
  displayName: string;
  dataType: ControlDataType;
};

export function useControlTypes() {
  const controlTypes: Control[] = [
    {
      displayName: $t("common.textBox"),
      value: "TextBox",
      dataType: "String",
    },
    {
      displayName: $t("common.textArea"),
      value: "TextArea",
      dataType: "String",
    },
    {
      displayName: $t("common.richEditor"),
      value: "RichEditor",
      dataType: "String",
    },
    {
      displayName: $t("common.selection"),
      value: "Selection",
      dataType: "Array",
    },
    {
      displayName: $t("common.checkbox"),
      value: "CheckBox",
      dataType: "Array",
    },
    {
      displayName: $t("common.radiobox"),
      value: "RadioBox",
      dataType: "Array",
    },
    {
      displayName: $t("common.switch"),
      value: "Switch",
      dataType: "Bool",
    },
    {
      displayName: $t("common.mediaFile"),
      value: "MediaFile",
      dataType: "Undefined",
    },
    {
      displayName: $t("common.mediaFileAdvanced"),
      value: "AdvancedMediaFile",
      dataType: "Undefined",
    },
    {
      displayName: $t("common.file"),
      value: "File",
      dataType: "Undefined",
    },
    {
      displayName: $t("common.dateTime"),
      value: "DateTime",
      dataType: "DateTime",
    },
    {
      displayName: $t("common.number"),
      value: "Number",
      dataType: "Number",
    },
    {
      displayName: $t("common.colorPicker"),
      value: "ColorPicker",
      dataType: "String",
    },
    {
      displayName: $t("common.keyValue"),
      value: "KeyValues",
      dataType: "Undefined",
    },
  ];
  function getControlType(value: string): Control | undefined {
    let lowerValue = value.toLowerCase();
    switch (lowerValue) {
      case "tinymce":
        lowerValue = "richeditor";
        break;
      case "boolean":
        lowerValue = "switch";
        break;
    }
    const controlType = controlTypes.find(
      (item) => item.value.toLowerCase() === lowerValue
    );
    return controlType;
  }

  function getAvailableControlTypes(dataType: string): Control[] {
    const result = controlTypes.filter(
      (item) =>
        ignoreCaseEqual(item.dataType, dataType) &&
        !ignoreCaseEqual(item.value, "AdvancedMediaFile")
    );
    return result;
  }

  const defaultAdvancedMediaFileOptions: string[] = [
    "alt",
    "id",
    "url",
    "downloadUrl",
    "previewUrl",
    "size",
    "lastModified",
    "folder",
    "mimeType",
  ]
    .sort()
    .map((it) => `{{${it}}}`);

  return {
    controlTypes,
    defaultAdvancedMediaFileOptions,
    getControlType,
    getAvailableControlTypes,
  };
}

export function getFieldRules(
  field: Pick<Field, "validations" | "controlType">
): FormItemRule[] {
  const t = $t;
  const rules: FormItemRule[] = [];
  const { validations, controlType } = field;
  if (!validations) {
    return rules;
  }

  validations.forEach(function (rule) {
    switch (rule.type) {
      case "required":
        rules.push({
          required: true,
          message: rule.msg || t("common.fieldRequiredTips"),
          trigger: ["checkbox", "radiobox", "number", "selection"].includes(
            (controlType ?? "").toLowerCase()
          )
            ? "change"
            : "blur",
        });
        break;
      case "regex":
        if (rule.pattern) {
          rules.push({
            pattern: new RegExp(rule.pattern),
            message: rule.msg || t("common.inputError"),
            trigger: "blur",
          });
        }
        break;
      case "range":
        rules.push({
          type: "number",
          min: Number(rule.min),
          max: Number(rule.max),
          message: rule.msg || t("common.inputError"),
          trigger: "change",
        });
        break;
      case "stringLength":
        rules.push({
          min: Number(rule.min),
          max: Number(rule.max),
          message: rule.msg || t("common.inputError"),
          trigger: "blur",
        });
        break;
      case "min":
        rules.push({
          type: "number",
          min: Number(rule.value),
          message: rule.msg || t("common.inputError"),
          trigger: "change",
        });
        break;
      case "minLength":
        rules.push({
          min: Number(rule.value),
          message: rule.msg || t("common.inputError"),
          trigger: "blur",
        });
        break;
      case "minChecked":
        rules.push({
          type: "array",
          min: Number(rule.value),
          message: rule.msg || t("common.inputError"),
          trigger: "change",
        });
        break;
      case "max":
        rules.push({
          type: "number",
          max: Number(rule.value),
          message: rule.msg || t("common.inputError"),
          trigger: "change",
        });
        break;
      case "maxLength":
        rules.push({
          max: Number(rule.value),
          message: rule.msg || t("common.inputError"),
          trigger: "blur",
        });
        break;
      case "maxChecked":
        rules.push({
          type: "array",
          max: Number(rule.value),
          message: rule.msg || t("common.inputError"),
          trigger: "change",
        });
        break;
    }
  });
  return rules;
}
