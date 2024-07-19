/* eslint-disable @typescript-eslint/no-explicit-any */
/// <reference types="vite/client" />

interface ImportMetaEnv {
  readonly VITE_API_Proxy: string;
  readonly VITE_API: string;
  readonly VITE_BASE_PATH: string;
}

declare module "*.vue" {
  import type { DefineComponent } from "vue";
  // eslint-disable-next-line @typescript-eslint/no-explicit-any, @typescript-eslint/ban-types
  const component: DefineComponent<{}, {}, any>;
  export default component;
}
declare module "*.yml" {
  const yamlObj: Record<string, any>;
  export default yamlObj;
}
declare module "css-color-extractor" {
  const extractor: { fromString(value: string): string[] };
  export default extractor;
}

interface Window {
  tinymce: any;
  editor: any;
}

declare let tinymce: any;
interface Document {
  editorIframe: Window & typeof globalThis;
}

interface Array<T> {
  at(n: number): T;
}

declare module "vue/dist/vue.esm-bundler.js";
