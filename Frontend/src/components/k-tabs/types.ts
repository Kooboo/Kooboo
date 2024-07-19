import type { ComponentPublicInstance } from "vue";

export interface IKTabItem {
  label: string;
  name: string;
  // todo 组件的类型
  content: ComponentPublicInstance<any> | JSX.Element;
}
