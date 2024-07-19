import type { ClassDefine, ClassDefineItem } from "@/api/site/kscript";
import { languages } from "monaco-editor";
import type { editor, Position } from "monaco-editor";
import { monaco } from "./userWorker";

let classDefines: ClassDefine[] = [];
let initialized = false;

class ClassCompletionItemProvider implements languages.CompletionItemProvider {
  triggerCharacters = ["'", '"', " ", ""];
  provideCompletionItems(
    model: editor.ITextModel,
    position: Position
  ): languages.ProviderResult<languages.CompletionList> {
    const result: languages.ProviderResult<languages.CompletionList> = {
      suggestions: [],
    };

    const content = model.getValue();
    const offset = model.getOffsetAt(position);
    const beforeContent = content.substring(0, offset);
    const classStarts = beforeContent.match(
      /class[\s,\n,\r]*=[\s,\n,\r]*\"|\'/
    );
    if (!classStarts) return result;
    const classStart = classStarts.pop();
    const classStartIndex =
      beforeContent.lastIndexOf(classStart!) + classStart!.length;
    const quotes = classStart!.match(/\"|\'/);
    const quote = quotes![0].toString();
    model.getValue().substring(model.getOffsetAt(position));
    const afterContent = content.substring(offset);
    const classEndIndex = afterContent.indexOf(quote) + beforeContent.length;
    if (classEndIndex == -1) return result;
    const classContent = content.substring(classStartIndex, classEndIndex);
    if (classContent.search(/\<|\>|\'\"/) > -1) return result;
    const existClasses = classContent.split(" ").map((m) => m.trim());
    const filterKeyWord = model.getWordAtPosition(position);
    for (const classDefine of classDefines) {
      for (const classDefineItem of classDefine.classes) {
        for (const i of expandRepeatSuggestion(classDefineItem)) {
          // const filterKeyWord = existClasses[existClasses.length - 1];
          const keyOffset = existClasses.indexOf(i.name);

          if (keyOffset > -1 && keyOffset < existClasses.length - 1) {
            continue;
          }

          let range = new monaco.Range(
            position.lineNumber,
            position.column,
            position.lineNumber,
            position.column
          );

          if (content.charAt(offset - 1) != " ") {
            if (filterKeyWord) {
              range = new monaco.Range(
                position.lineNumber,
                filterKeyWord.startColumn,
                position.lineNumber,
                filterKeyWord.endColumn
              );
            }
          }

          result.suggestions.push({
            kind: languages.CompletionItemKind.Property,
            label: i.name,
            insertText: i.name,
            detail: classDefine.name,
            documentation: {
              value: i.description,
            },
            range: range,
            command: {
              title: "Suggest",
              id: "editor.action.triggerSuggest",
            },
          });
        }
      }
      result.incomplete = false;
    }

    return result;
  }
}

export async function registerClassCompletionItemProvider(
  define: ClassDefine[]
) {
  classDefines = define;
  if (initialized) return;
  initialized = true;

  monaco.languages.registerCompletionItemProvider(
    "html",
    new ClassCompletionItemProvider()
  );
}

function expandRepeatSuggestion(
  item: ClassDefineItem
): Omit<ClassDefineItem, "repeat">[] {
  const result = [];

  if (item.repeat) {
    for (const repeatItem of item.repeat) {
      const define = {
        name: item.name,
        description: item.description,
      };

      if (Array.isArray(repeatItem)) {
        for (let i = 0; i < repeatItem.length; i++) {
          define.name = define.name.replaceAll(
            `#$repeat-${i}$#`,
            repeatItem[i]
          );
          define.description = define.description.replaceAll(
            `#$repeat-${i}$#`,
            repeatItem[i]
          );
        }
      } else {
        define.name = define.name.replaceAll("#$repeat$#", repeatItem);
        define.description = define.description.replaceAll(
          "#$repeat$#",
          repeatItem
        );
      }

      result.push(define);
    }
  } else {
    result.push(item);
  }

  return result;
}
