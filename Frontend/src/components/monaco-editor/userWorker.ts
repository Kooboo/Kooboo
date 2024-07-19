import * as monacoInstance from "monaco-editor";
export type {
  languages,
  Range,
  editor,
  Uri,
  IRange,
  ISelection,
} from "monaco-editor";
import editorWorker from "monaco-editor/esm/vs/editor/editor.worker?worker";
import jsonWorker from "monaco-editor/esm/vs/language/json/json.worker?worker";
import cssWorker from "monaco-editor/esm/vs/language/css/css.worker?worker";
import htmlWorker from "monaco-editor/esm/vs/language/html/html.worker?worker";
import tsWorker from "monaco-editor/esm/vs/language/typescript/ts.worker?worker";
import { useMonacoEx, useDirective, useContentRegin } from "monaco-editor-ex";

export const monaco = monacoInstance;
export type Monaco = typeof monaco;

self.MonacoEnvironment = {
  getWorker(_: any, label: string) {
    if (label === "json") {
      return new jsonWorker();
    }
    if (label === "css" || label === "scss" || label === "less") {
      return new cssWorker();
    }
    if (label === "html" || label === "handlebars" || label === "razor") {
      return new htmlWorker();
    }
    if (label === "typescript" || label === "javascript") {
      return new tsWorker();
    }
    return new editorWorker();
  },
};
const zoomKey = "monaco-editor-zoom-level";

const zoomLevel = localStorage.getItem(zoomKey);

if (!zoomLevel) {
  localStorage.setItem(
    zoomKey,
    monaco.editor.EditorZoom.getZoomLevel().toString()
  );
} else {
  monaco.editor.EditorZoom.setZoomLevel(parseInt(zoomLevel));
}

monaco.editor.EditorZoom.onDidChangeZoomLevel((e) => {
  localStorage.setItem(zoomKey, e.toString());
});

monaco.editor.addKeybindingRules([
  {
    keybinding: monaco.KeyMod.CtrlCmd | monaco.KeyCode.Equal,
    command: "editor.action.fontZoomIn",
  },
  {
    keybinding: monaco.KeyMod.CtrlCmd | monaco.KeyCode.Minus,
    command: "editor.action.fontZoomOut",
  },
  {
    keybinding: monaco.KeyMod.CtrlCmd | monaco.KeyCode.Digit0,
    command: "editor.action.fontZoomReset",
  },
]);

monaco.languages.typescript.typescriptDefaults.setDiagnosticsOptions({
  noSemanticValidation: true,
});

monaco.languages.typescript.typescriptDefaults.setEagerModelSync(true);
useMonacoEx(monaco);
useDirective([
  {
    language: "javascript",
    matcher: /^(\:\w+)$/i,
  },
  {
    language: "javascript",
    matcher: /^(v-bind\:\w+)$/i,
  },
  {
    language: "javascript",
    matcher: "v-if",
  },
  {
    language: "javascript",
    matcher: "v-elseif",
  },
  {
    language: "javascript",
    matcher: "v-text",
  },
  {
    language: "javascript",
    matcher: "v-html",
  },
  {
    language: "javascript",
    matcher: "v-for",
    appendContent: appendContent,
  },
  {
    language: "javascript",
    matcher: "k-content",
  },
  {
    language: "javascript",
    matcher: "k-if",
  },
  {
    language: "javascript",
    matcher: "k-elseif",
  },
  {
    language: "javascript",
    matcher: "k-text",
  },
  {
    language: "javascript",
    matcher: "k-for",
    appendContent: appendContent,
  },
]);

useContentRegin([
  {
    language: "javascript",
    matcher: (value) => {
      const result = [];
      let currentPosition = 0;
      while (value) {
        let start = value.indexOf("{{{", currentPosition);
        if (start == -1) {
          start = value.indexOf("{{", currentPosition);
          if (start == -1) break;
          else {
            currentPosition = start + 2;
            const end = value.indexOf("}}", currentPosition);
            if (end > start) {
              result.push({
                start: currentPosition,
                end: end,
              });
            }
          }
        } else {
          currentPosition = start + 3;
          const end = value.indexOf("}}}", currentPosition);
          if (end > start) {
            result.push({
              start: currentPosition,
              end: end,
            });
          } else {
            break;
          }
        }
      }
      return result;
    },
  },
]);

function appendContent(value: string): string {
  let result = "";
  let target = "";
  if (!value) return result;
  let splitIndex = value.indexOf(" in ");
  if (splitIndex < 1) splitIndex = value.indexOf(" of ");
  if (splitIndex > 0) target = value.substring(splitIndex + 4).trim();
  else {
    splitIndex = value.indexOf(" ");
    if (splitIndex > 0) target = value.substring(splitIndex).trim();
  }
  if (splitIndex < 1) return result;
  if (!target) return result;
  value = value.substring(0, splitIndex).trim();
  while (value.startsWith("(")) {
    value = value.substring(1);
  }
  while (value.endsWith(")")) {
    value = value.substring(0, value.length - 1);
  }

  const params =
    value.startsWith("{") && value.endsWith("}") ? [value] : value.split(",");
  if (params[0]) {
    result += `let ${params[0]} = Object.values(${target})[0];`;
  }

  if (params[1]) {
    result += `let ${params[1]}:keyof ${target};`;
  }

  if (params[2]) {
    result += `let ${params[2]}:keyof ${target};`;
  }

  return result;
}
