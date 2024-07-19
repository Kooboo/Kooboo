export interface EventItem {
  count: number;
  name: string;
}

export interface EventTypeItem {
  name: string;
  category: string;
}

export interface Setting {
  controlType: string;
  defaultValue: string;
  name: string;
  selectionValues: Record<string, string>;
  toolTip: string;
}

export interface Rule {
  if?: Condition[];
  then?: Rule[];
  else?: Rule[];
  do?: CodeItem[];
  id: string;
}

export interface CodeItem {
  codeId?: string;
  code: string;
  setting: Record<string, string>;
}

export interface Condition {
  left: string;
  operator: string;
  right: string;
}

export interface Option {
  controlType: string;
  defaultValue: string;
  name: string;
  operator: string[];
}
