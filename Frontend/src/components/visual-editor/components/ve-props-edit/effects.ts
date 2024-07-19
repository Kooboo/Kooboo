import type { Meta, VeWidgetPropDefine } from "../../types";
import { getFieldRules, useControlTypes } from "@/hooks/use-control-types";

import type { Field } from "@/components/field-control/types";
import FieldControl from "@/components/field-control/index.vue";
import type { FormRules } from "element-plus";
import { createField } from "../../utils/prop";
import { getContainerPropDefines } from "../../utils/prop-define";
import { getValueIgnoreCase } from "@/utils/string";
import { isCustomWidget } from "../../utils/widget";
import { ref } from "vue";
import { useBuiltinWidgets } from "../ve-widgets";
import { useCustomWidgetEffects } from "../../custom-widgets";

const { widgets } = useBuiltinWidgets();
const { customWidgets } = useCustomWidgetEffects();

const { getControlType } = useControlTypes();

const propComponents = import.meta.globEager("./components/*.vue");

function getPropDefines(meta: Meta) {
  if (isCustomWidget(meta)) {
    return (
      customWidgets.value.find(
        (it) => it.meta?.type === meta.type && it.meta?.name === meta.name
      )?.meta?.propDefines ?? meta.propDefines
    );
  }

  const widget = widgets.value.find((it) => it.meta?.type === meta.type);
  const propDefines = widget?.meta?.propDefines ?? meta.propDefines;
  if (typeof widget?.getAdditionalSettings === "function") {
    const settings = widget.getAdditionalSettings();
    propDefines.forEach((prop) => {
      const additionSetting = getValueIgnoreCase(settings, prop.name);
      if (additionSetting) {
        prop.settings = {
          ...prop.settings,
          ...additionSetting,
        };
      }
    });
  }
  return propDefines;
}

function getDefaultValueByControl(def: VeWidgetPropDefine): any {
  if (def.dataType === "Array" || def.multipleValue) {
    return [];
  }

  return def.settings?.defaultValue ?? null;
}

function assignPropDefaultValue(def: VeWidgetPropDefine, meta: Meta) {
  if (!meta.props) {
    meta.props = {};
  }
  let value = meta.props[def.name];
  if (value === undefined) {
    value = def.defaultValue ?? getDefaultValueByControl(def);
  }

  meta.props[def.name] = value;
}
function getFields(propDefines: VeWidgetPropDefine[]) {
  const fieldList: Field[] = [];
  const fieldRules: FormRules = {};
  if (!propDefines) {
    return {
      fieldList,
      fieldRules,
    };
  }

  propDefines.forEach((it) => {
    if (it.isSystemField || it.name.startsWith("veContainer")) {
      return;
    }
    const field: Field = {
      lang: "en-US", // TODO
      name: it.name,
      displayName: it.displayName,
      prop: it.name,
      toolTip: it.tooltip ?? "",
      selectionOptions: it.selectionOptions,
      validations: it.validations,
      isMultilingual: false,
      // isMultilingual: it.multipleLanguage,
      multipleValue: it.multipleValue,
      controlType: it.controlType,
      settings: it.settings,
      required: it.required,
    };
    fieldRules[field.name] = getFieldRules(field);
    fieldList.push(field);
  });
  return {
    fieldList,
    fieldRules,
  };
}

export function getFieldControl(field: Field) {
  const control = getControlType(field.controlType);
  if (control) {
    return FieldControl;
  }

  const key = `./components/${field.controlType}.vue`;
  return propComponents[key]?.default || "div";
}

export function usePropsEditor(meta: Meta, classic: boolean) {
  const model = ref<Record<string, any>>({});
  const fields = ref<Field[]>([]);
  const rules = ref<FormRules>({});

  const containerFields = ref<Field[]>([]);

  const propDefines = getPropDefines(meta);
  const { fieldList, fieldRules } = getFields(propDefines);
  fields.value = fieldList;
  rules.value = fieldRules;
  const validContainerProps = getContainerPropDefines(meta).filter((it) => {
    if (!classic) {
      return true;
    }

    return it.name !== "veContainerMargin";
  });
  containerFields.value = validContainerProps.map(createField);
  validContainerProps.forEach((item) => {
    assignPropDefaultValue(item, meta);
  });
  propDefines.forEach((item) => {
    assignPropDefaultValue(item, meta);
  });
  model.value = meta.props;
  return {
    rules,
    fields,
    containerFields,
    model,
  };
}
