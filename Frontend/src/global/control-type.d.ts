export type Property = {
  name: string;
  displayName: string;
  controlType: string;
  dataType: string;
  isSummaryField: boolean;
  multipleLanguage: boolean;
  editable: boolean;
  tooltip: string | null;
  maxLength: number;
  validations: FieldValidation[];
  settings: Record<string, any>;
  isSystemField: boolean;
  multipleValue: boolean;
  displayInSearchResult: boolean;
  selectionOptions: KeyValue[];
  order: number;
};

export type PropertyOptions = {
  hideMultipleLanguage?: boolean;
  hideDisplayInSearchResult?: boolean;
  hideValidation?: boolean;
  hideSummaryField?: boolean;
};

export type FieldValidation = {
  name?: string;
  type: ValidationRuleType;
  min?: number;
  max?: number;
  pattern?: string;
  value?: number;
  msg?: string;
};

export type ValidationRuleType =
  | "required"
  | "regex"
  | "range"
  | "stringLength"
  | "min"
  | "minLength"
  | "minChecked"
  | "max"
  | "maxLength"
  | "maxChecked"
  | "fileSize"
  | "fileTypes";

export type JsonStringField = "selectionOptions" | "validations" | "settings";

export type PropertyJsonString = Omit<Property, JsonStringField> & {
  selectionOptions: string;
  validations: string;
};
