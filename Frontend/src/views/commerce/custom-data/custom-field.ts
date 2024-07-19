import { i18n } from "@/modules/i18n";
import type { FormItemRule } from "element-plus";

const $t = i18n.global.t;

interface TypeDefine {
  displayName: string;
  selection?: boolean;
  multiple?: boolean;
  multilingual?: boolean;
}

export const fieldTypes: Record<string, TypeDefine> = {
  ["TextBox"]: {
    displayName: $t("common.textBox"),
    multilingual: true,
  },
  ["TextArea"]: {
    displayName: $t("common.textArea"),
    multilingual: true,
  },
  ["RichEditor"]: {
    displayName: $t("common.richEditor"),
    multilingual: true,
  },
  ["Selection"]: {
    displayName: $t("common.selection"),
    selection: true,
  },
  ["CheckBox"]: {
    displayName: $t("common.checkbox"),
    selection: true,
  },
  ["RadioBox"]: {
    displayName: $t("common.radiobox"),
    selection: true,
  },
  ["Number"]: {
    displayName: $t("common.number"),
  },
  ["Switch"]: {
    displayName: $t("common.switch"),
  },
  ["Content"]: {
    displayName: $t("common.content"),
    multiple: true,
  },
  ["KeyValues"]: {
    displayName: $t("common.keyValue"),
    multilingual: true,
  },
  ["MediaFile"]: {
    displayName: $t("common.mediaFile"),
    multiple: true,
  },
  ["File"]: {
    displayName: $t("common.file"),
    multiple: true,
  },
  ["DateTime"]: {
    displayName: $t("common.dateTime"),
  },
  ["ColorPicker"]: {
    displayName: $t("common.colorPicker"),
  },
};

export function getFieldRules(
  validations: any,
  type: keyof typeof fieldTypes
): FormItemRule[] {
  const t = $t;
  const rules: FormItemRule[] = [];
  if (!validations) {
    return rules;
  }

  validations.forEach(function (rule: any) {
    switch (rule.type) {
      case "required":
        rules.push({
          required: true,
          message: rule.msg || t("common.fieldRequiredTips"),
          trigger: [
            "checkbox",
            "radiobox",
            "number",
            "selection",
            "content",
          ].includes((type ?? "").toLowerCase())
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
