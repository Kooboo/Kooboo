<script lang="ts" setup>
import { onMounted, onUnmounted, ref, watch, inject } from "vue";
import type { Language } from "./types";
import { useClassCompletion, useKscript } from "./monaco";
import type { editor, IRange, ISelection, Uri } from "./userWorker";
import { monaco } from "./userWorker";
import { addExtraLib } from "./extraLib";
import config, {
  onFileOpenInjectionFlag,
  onFileNotFoundInjectionFlag,
} from "./config";
import { newGuid } from "@/utils/guid";
import { Completer, isJson } from "@/utils/lang";
import { dark } from "@/composables/dark";
import kMailDefine from "./kmail-define.d.ts?raw";
import { addShortcuts } from "./shortcut";
import { registerToggleComment } from "./toggleComment";
import {
  useModuleResolve,
  useModuleSuggest,
  useUnocss,
  useJavascriptInHtmlSuggestFilter,
} from "monaco-editor-ex";
import { useCodeStore } from "@/store/code";
import { useSiteStore } from "@/store/site";
import registerDatabaseHint from "./registerDatabaseHint";
import registerJsonSchema from "./registerJsonSchema";
import { registerFileOpener } from "./registerFileOpener";
import { getByName } from "@/api/code";
import type { ResolvedRoute } from "@/api/route/types";
import { useModuleStore } from "@/store/module";
import { getTextByName } from "@/api/module/files";
import { useDevModeStore } from "@/store/dev-mode";

const container = ref();
const modelId = newGuid();
let model: editor.IModel | undefined = undefined;
let codeEditor: editor.IStandaloneCodeEditor | undefined = undefined;
const readyCompleter = new Completer();
const siteStore = useSiteStore();
const codeStore = useCodeStore();
const moduleStore = useModuleStore();
const devModeStore = useDevModeStore();
const openFile = inject<(req: ResolvedRoute) => void>(onFileOpenInjectionFlag);
const onFileNotFound = inject<(url: string) => void>(
  onFileNotFoundInjectionFlag
);

const props = defineProps<{
  modelValue: string | null | undefined;
  language: Language;
  kScript?: boolean;
  kmail?: boolean;
  options?: Record<string, unknown>;
  uri?: any;
  module?: boolean;
}>();

const emit = defineEmits<{
  (e: "update:modelValue", value: string): void;
  (e: "monacoLoadComplete", value: editor.IStandaloneCodeEditor): void;
}>();

const focus = async () => {
  await readyCompleter.promise;
  codeEditor?.focus();
};

const goToLine = async (line?: number) => {
  await readyCompleter.promise;
  const count = line || codeEditor?.getModel()?.getLineCount() || 0;
  const column = codeEditor?.getModel()?.getLineLength(count) || 0;
  codeEditor?.revealLineInCenter(count);
  codeEditor?.setPosition({ column: column + 1, lineNumber: count });
  codeEditor?.focus();
};

const goto = async (range: IRange) => {
  await readyCompleter.promise;
  //选中指定range的文本
  codeEditor?.setSelection(range);
  //把选中的位置放到中间显示
  codeEditor?.revealRangeInCenter(range);
};

const search = async (searchText: string, line?: number) => {
  await readyCompleter.promise;
  let lineRange: IRange | undefined = undefined;
  const ranges = codeEditor
    ?.getModel()
    ?.findMatches(searchText, false, false, false, null, false)
    ?.map((i) => {
      // 存储这行第一个搜索结果
      if (
        Number.isInteger(line) &&
        !lineRange &&
        i.range.startLineNumber === line
      ) {
        lineRange = i.range;
      }

      // 返回匹配的结果
      return {
        selectionStartLineNumber: i.range.startLineNumber,
        selectionStartColumn: i.range.startColumn,
        positionLineNumber: i.range.endLineNumber,
        positionColumn: i.range.endColumn,
      } as ISelection;
    });

  if (lineRange) {
    codeEditor?.setSelection(lineRange);
    codeEditor?.revealRangeInCenter(lineRange);
  } else {
    // 没有指定行 全部选中
    ranges && codeEditor?.setSelections(ranges);
    // 定位第一个结果
    ranges &&
      codeEditor?.revealLineInCenter(ranges[0].selectionStartLineNumber);
  }

  codeEditor?.focus();
};

const format = async () => {
  await codeEditor?.getAction("editor.action.formatDocument")?.run();
};

const replace = async (text: string) => {
  var selection = codeEditor?.getSelection();
  if (!selection) return;
  var range = new monaco.Range(
    selection.startLineNumber,
    selection.startColumn,
    selection.endLineNumber,
    selection.endColumn
  );
  codeEditor?.executeEdits("", [{ range, text }]);
};

defineExpose({ focus, goToLine, goto, search, format, replace });

watch(
  () => props.modelValue,
  () => {
    if (model && model.getValue() !== props.modelValue) {
      model.setValue(props.modelValue ?? "");
    }
  }
);

useModuleSuggest(async () => {
  if (devModeStore.activeTab?.params?.moduleId) {
    const currentName = (devModeStore.activeTab?.params as any)?.file?.name;
    return moduleStore.files
      .filter((f) => f.objectType == "code" && f.name != currentName)
      .map((m) => `./${m.name}`);
  } else {
    const codes = codeStore.codes
      .filter((f) => f.codeType == "CodeBlock")
      .map((m) => `./${m.name}`);
    const modules = moduleStore.list.map((m) => `module:${m.name}`);
    return [...codes, ...modules];
  }
});

const includes = [
  "JSON",
  "decodeURI",
  "decodeURIComponent",
  "encodeURI",
  "encodeURIComponent",
  "parseFloat",
  "parseInt",
];

useJavascriptInHtmlSuggestFilter((uri, regin, suggestions) => {
  if (regin?.type == "attribute" && regin.start == regin.end)
    return { suggestions: [], snippet: false };
  if (regin?.type != "content" && regin?.type != "attribute")
    return { suggestions, snippet: true };
  const result = suggestions.filter((f) => {
    if (f.kind == "keyword") return false;
    if (f.name == "globalThis") return false;
    if (f.name == "undefined") return false;
    if (f.name == "Vue") return false;
    if (includes.includes(f.name)) return true;
    var declare = f.kindModifiers?.indexOf("declare") > -1;
    if (f.kindModifiers?.indexOf("deprecated") > -1) return false;
    if (
      declare &&
      f.sortText === "15" &&
      (f.kind == "var" ||
        f.kind == "module" ||
        f.kind == "class" ||
        f.kind == "function")
    ) {
      return false;
    }

    return true;
  });
  return { suggestions: result, snippet: false };
});

onMounted(async () => {
  if (siteStore.site) {
    useModuleResolve(async (uri) => {
      let path = monaco.Uri.parse(uri).path;
      path = path.substring(1, path.length - 3);
      if (!path) throw new Error("empty path");
      if (path.startsWith("node_modules")) {
        path = path.split("module:")[1];
        const moduleName = path.split("/")[0];
        let fileName = path.split("/")[1];
        return await getTextByName("code", moduleName, fileName);
      } else {
        const code = await getByName(path);
        return code.body;
      }
    });

    if (!codeStore.codesInitialized) {
      codeStore.loadCodes();
    }

    if (!moduleStore.modulesInitialized) {
      moduleStore.load();
    }
  }

  let defines = "";
  if (props.kScript) defines = await useKscript();
  if (props.language == "html") await useClassCompletion();

  let uri = props.uri;

  if (typeof uri == "string") {
    uri = monaco.Uri.parse(uri);
  }

  if (!uri) {
    uri = monaco.Uri.from({
      scheme: "memory",
      path: modelId,
    });
  }

  model = monaco.editor.getModel(uri)!;
  if (model) {
    model.setValue(props.modelValue ?? "");
  } else {
    model = monaco.editor.createModel(
      props.modelValue ?? "",
      props.language,
      uri
    );
  }

  function jsonSyntaxSwitch(language: string, content: string) {
    if (language == "javascript") {
      if (isJson(content) && content.length > 10) {
        setTimeout(() => {
          monaco.editor.setModelLanguage(model!, "json");
        }, 1000);
      }
    }
    if (props.language == "javascript" && language == "json") {
      if (!isJson(content) || !content?.trim()) {
        setTimeout(() => {
          monaco.editor.setModelLanguage(model!, "javascript");
        }, 1000);
      }
    }
  }

  model.onDidChangeContent(() => {
    const content = model!.getValue();
    const language = model!.getLanguageId();
    jsonSyntaxSwitch(language, content);
    emit("update:modelValue", content);
  });

  codeEditor = monaco.editor.create(container.value, {
    ...config,
    ...props.options,
    model,
  });

  jsonSyntaxSwitch(props.language, props.modelValue ?? "");
  registerToggleComment(codeEditor);
  registerDatabaseHint(props.kScript!);
  registerJsonSchema();

  addShortcuts(codeEditor);

  if (props.kScript) {
    addExtraLib(defines, "default.d.ts");
  }

  if (props.kmail) {
    addExtraLib(kMailDefine, "kmail.d.ts");
  }

  registerFileOpener(codeEditor, openFile, onFileNotFound);

  emit("monacoLoadComplete", codeEditor);

  readyCompleter.resolve(null);
});

onUnmounted(() => {
  if (!model) return;

  if (model.uri.scheme == "memory") {
    model.dispose();
  }

  if (model.uri.toString() == "file:///00000000-0000-0000-0000-000000000000") {
    model.dispose();
  }
});

let dispose: () => void;

watch(
  () => siteStore.site,
  () => {
    if (!siteStore.site?.unocssSettings?.enable) {
      if (dispose) dispose();
      return;
    }
    dispose = useUnocss(JSON.parse(siteStore.site.unocssSettings.config));
  },
  {
    immediate: true,
    deep: true,
  }
);

watch(dark, (dark) => monaco.editor.setTheme(dark ? "vs-dark" : "vs"), {
  immediate: true,
});
</script>

<template>
  <div
    ref="container"
    class="w-full h-full min-h-300px min-w-400px monaco-editor bg-fff"
  />
</template>

<style lang="scss">
@import "./style.scss";
</style>
