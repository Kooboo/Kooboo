import type { Meta } from "./types";
import type { Property } from "@/global/control-type";

export interface Meta {
  children: Meta[];
  htmlStr: string;
  id?: string;
  readonly name: string;
  props: Record<string, any>;
  propDefines: VeWidgetPropDefine[];
  readonly attribute?: string;
  readonly type: string;
}

export type VeRenderContext = {
  baseUrl: string;
  classic: boolean;
  rootMeta?: Meta;
};

export type VeColumn = {
  id: string;
  width: number;
  widgets: Meta[];
};

export type MessageWidget = {
  type:
    | "ve-sync"
    | "ve-init"
    | "ve-select"
    | "ve-layout"
    | "ve-holder"
    | "ve-switch-tab";
  group?: string;
  item?: any;
};

export type InUpdateDomContextType = {
  meta: Meta;
  group?: string;
  context?: VeWidgetSelectContext;
};

export type InSelectWidgetContextType = {
  meta?: Meta;
  group?: string;
  context?: VeWidgetSelectContext;
};

export type InChangeContextType = any[];

export type MessageIframe<T> = {
  type:
    | "in-sync"
    | "in-select"
    | "in-edit"
    | "in-change"
    | "in-update-dom"
    | "in-page-style"
    | "in-reset-holder"
    | "in-reset-drag"
    | "in-dragging-item";
  context: T;
  group?: string;
};

export interface VeWidgetType {
  id: string;
  name: string;
  settings: Record<string, string>;

  tooltip: string;

  readonly icon?: string;
  readonly svg?: string;
  readonly meta: Meta;
  render: (meta: Meta, rootMeta?: Meta) => Promise<string>;
  renderClassic: (meta: Meta, rootMeta?: Meta) => Promise<string>;
  injection?: (meta: Meta) => Promise<string>;
  init?: (props: Ref<Record<string, any>>) => void;
  getAdditionalSettings?: () => Record<string, Record<string, any>>;
}

export interface VeWidgetPropDefine extends Property {
  defaultValue: any;
  controlType: string;
  required: boolean;
  formatter?: (value: any) => any;
}

export interface VeSource {
  body: string;
  metaBindings: [];
  urlParamsBindings: [];
}

export interface VeAddonSource {
  id: string;
  tag: string;
  source: VeSource;
}

export type PaddingOptions = {
  moreOptions: boolean;
  all?: number;
  top?: number;
  right?: number;
  bottom?: number;
  left?: number;
};

export type HolderType = "hover" | "active";

export type VeWidgetSelectContext = {
  id?: string;
  rowId?: string;
  columnId?: string;
  group?: string;
};

export type TypeUnit<TValue> = {
  unit: string;
  value: TValue;
};

export type NumberUnit = TypeUnit<number | null>;

export type DeviceType = "full" | "pad" | "phone";

export type VeDraggableContext = {
  element: VeWidgetType;
  index: number;
};

export type VeDraggableElement = HTMLElement & {
  __draggable_context: VeDraggableContext;
};
