import type { Property } from "@/global/control-type";

export interface Component {
  displayName: string;
  engineName: string;
  requireEngine: boolean;
  tagName: string;
  attribute: string;
}

export interface Source {
  body: string;
  metaBindings: [];
  urlParamsBindings: [];
}

export interface TagObject {
  id: string;
  name: string;
  settings: Record<string, string>;
  propDefines?: PropDefine[];
}

export interface PropDefine extends Property {
  defaultValue: any;
  required: boolean;
}
