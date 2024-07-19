import type { FieldValidation } from "@/global/control-type";
import type { KeyValue } from "@/global/types";

export type Field = {
  lang: string;
  name: string;
  displayName: string;
  prop: string;
  toolTip: string;
  selectionOptions: KeyValue[];
  isMultilingual: boolean;
  multipleValue: boolean;
  controlType: string;
  settings: Record<string, any>;
  validations?: FieldValidation[];
  required?: boolean;
};
