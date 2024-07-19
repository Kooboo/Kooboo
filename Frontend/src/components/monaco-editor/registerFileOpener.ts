import type { ResolvedRoute } from "@/api/route/types";
import type { Uri } from "monaco-editor";
import type { editor } from "./userWorker";
import { resolveRoute } from "@/api/route";

let doOpenEditor: any = undefined;

function doOpenEditorOverride(
  codeEditor: any,
  onOpen?: (req: ResolvedRoute) => void,
  onNotFound?: (url: string) => void
) {
  if (!doOpenEditor) {
    const doOpenEditorBackup = codeEditor._codeEditorService.doOpenEditor.bind(
      codeEditor._codeEditorService
    );
    doOpenEditor = (
      e: editor.IStandaloneCodeEditor,
      a: {
        resource: Uri;
      }
    ) => {
      doOpenEditorBackup(e, a);
      if (typeof onOpen !== "function" && typeof onNotFound !== "function") {
        return;
      }
      const { path, query } = a.resource;
      const link = `${path}${query ? "?" + query : ""}`;
      resolveRoute(path)
        .then((req) => {
          if (req) {
            typeof onOpen === "function" && onOpen(req);
          } else {
            typeof onNotFound === "function" && onNotFound(link);
          }
        })
        .catch(() => {
          typeof onNotFound === "function" && onNotFound(link);
        });
    };
  }

  return doOpenEditor;
}

export function registerFileOpener(
  editor: editor.IStandaloneCodeEditor,
  onOpen?: (req: ResolvedRoute) => void,
  onNotFound?: (url: string) => void
) {
  const codeEditor: any = editor;
  codeEditor._codeEditorService.doOpenEditor = doOpenEditorOverride(
    codeEditor,
    onOpen,
    onNotFound
  ).bind(codeEditor._codeEditorService);
}
