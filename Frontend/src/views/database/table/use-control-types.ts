import { i18n } from "@/modules/i18n";
const $t = i18n.global.t;

export function useControlTypes() {
  const controlTypes = [
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
      displayName: $t("common.WYSIWYGEditor"),
      value: "Tinymce",
      dataType: "String",
    },
    {
      displayName: $t("common.selection"),
      value: "Selection",
      dataType: "Undefined",
    },
    {
      displayName: $t("common.checkbox"),
      value: "CheckBox",
      dataType: "Undefined",
    },
    {
      displayName: $t("common.radiobox"),
      value: "RadioBox",
      dataType: "Undefined",
    },
    {
      displayName: $t("common.boolean"),
      value: "Boolean",
      dataType: "Bool",
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
  ];
  function getControlType(value: string) {
    const controlType = controlTypes.find(
      (item) => item.value?.toLowerCase() === value?.toLowerCase()
    );
    return controlType;
  }

  function getAvailableControlTypes(dataType: string) {
    const result = controlTypes.filter(
      (item) => item.dataType.toLowerCase() === dataType.toLowerCase()
    );
    return result;
  }

  return {
    controlTypes,
    getControlType,
    getAvailableControlTypes,
  };
}
