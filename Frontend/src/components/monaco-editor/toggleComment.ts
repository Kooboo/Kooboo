import type { editor, Range } from "./userWorker";
import { monaco } from "./userWorker";

export function registerToggleComment(editor: editor.IStandaloneCodeEditor) {
  if (editor.getModel()?.getLanguageId() !== "html") return;

  editor.addAction({
    id: "editor.action.toggleComment",

    label: "Toggle Line Comment",

    keybindings: [monaco.KeyMod.CtrlCmd | monaco.KeyCode.Slash],

    precondition: "editorLangId == html",

    run: function (ed) {
      const selections = Array.from(ed.getSelections() ?? []);
      if (!selections.length) {
        return;
      }
      const model = ed.getModel()!;
      const text = model.getValue();
      const [jsSelections, otherSelections] = partition(selections, (s) =>
        isJs(text, model.getOffsetAt(s.getStartPosition()))
      );

      // console.log("jsSelections", jsSelections);

      if (otherSelections.length) {
        editor.setSelections(otherSelections);
        editor.getAction("editor.action.commentLine")?.run();
        editor.setSelections(selections);
      }

      const edits: editor.IIdentifiedSingleEditOperation[] = [];
      jsSelections.forEach((s, i) => {
        const linesContent = lines(model, s);
        let uncomment = true;
        let minIndent = Number.MAX_SAFE_INTEGER;

        for (const line of linesContent) {
          const indent = line.search(/\S/);
          if (indent < 0) continue;
          if (indent < minIndent) minIndent = indent;
          if (uncomment && !line.match(/^\s*?\/\//)) uncomment = false;
        }

        if (minIndent === Number.MAX_SAFE_INTEGER) minIndent = 0;
        const indent = " ".repeat(minIndent);

        let lineno = s.getStartPosition().lineNumber;
        for (const line of linesContent) {
          edits.push({
            range: new monaco.Range(lineno, 1, lineno, line.length + 1),
            text: uncomment
              ? line.replace(/\/\/\s?/, "")
              : `${indent}// ${line.replace(indent, "")}`,
          });
          lineno++;
        }
      });
      editor.executeEdits("my-unique-id", edits);
    },
  });
}

const isJs = (text: string, offset: number) => {
  const i = text.indexOf("</script", offset);
  if (i < 0) return false;
  const j = text.indexOf("<script", offset);
  if (j < 0) return true;
  return i < j;
};

const lines = (model: editor.ITextModel, range: Range) =>
  Array.from(
    (function* (): Generator<string> {
      const { startLineNumber, endLineNumber } = range;
      for (let i = startLineNumber; i < endLineNumber + 1; i++)
        yield model.getLineContent(i);
    })()
  );

interface PartitionFn {
  <T, U extends T = T>(it: Iterable<T>, pred: (x: T) => x is U): [U[], T[]];
  <T>(it: Iterable<T>, pred: (x: T) => boolean): [T[], T[]];
}

const partition: PartitionFn = (
  it: Iterable<any>,
  pred: (x: any) => boolean
) => {
  return Array.from(it).reduce(
    ([yea, nay], x) => ((pred(x) ? yea : nay).push(x), [yea, nay]),
    [[], []]
  );
};
