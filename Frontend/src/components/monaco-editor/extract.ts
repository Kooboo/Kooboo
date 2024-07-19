import { applyCustomHtml } from "./html-customization";
import { addShortcuts } from "./shortcut";
import { monaco } from "./userWorker";
import Axios from "axios";
import { combineUrl } from "@/utils/url";
import { addExtraLib } from "./extraLib";
import { registerClassCompletionItemProvider } from "./classCompletion";
import { registerToggleComment } from "./toggleComment";

async function getCustomHtml() {
  const { data } = await Axios.get(
    combineUrl(import.meta.env.VITE_API!, "KScript/GetKViewSuggestions")
  );
  return data;
}

async function getDefine() {
  const { data } = await Axios.get(
    combineUrl(import.meta.env.VITE_API!, "KScript/GetDefine")
  );
  return data;
}

async function getClassDefine() {
  const { data } = await Axios.get(
    combineUrl(import.meta.env.VITE_API!, "KScript/GetClassSuggestions")
  );
  return data;
}

interface Options {
  kview?: boolean;
  kscript?: boolean;
  classCompletion?: boolean;
}

(window as any)._monaco = {
  instance: monaco,
  async init(
    el: HTMLElement,
    code: string,
    language: string,
    options?: Options
  ) {
    options = options! ?? {};
    if (options.kview === undefined) options.kview = true;
    if (options.kscript === undefined) options.kscript = true;
    if (options.classCompletion === undefined) options.classCompletion = true;

    if (options.kview) {
      const customHtml = await getCustomHtml();
      applyCustomHtml(customHtml);
    }

    if (options.classCompletion) {
      const classDefine = await getClassDefine();
      registerClassCompletionItemProvider(classDefine, true);
    }

    if (options.kscript) {
      const define = await getDefine();
      addExtraLib(define, "default.d.ts");
    }

    const model = monaco.editor.createModel(code, language);
    const editor = monaco.editor.create(el, {
      value: code,
      model: model,
    });
    addShortcuts(editor);
    registerToggleComment(editor);
    return model;
  },
};
