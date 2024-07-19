/* eslint-disable @typescript-eslint/no-explicit-any */
import { getColumns, getTables } from "@/api/site/kscript";
import { useModuleStore } from "@/store/module";
import { monaco } from "./userWorker";

let initialized = false;
let _enable = false;
let tables: Record<string, string[]> = {};
let columns: Record<string, string[]> = {};

const databases = [
  "indexeddb",
  "mysql",
  "sqlserver",
  "sqlite",
  "mongo",
  "sql",
  "localdatabase",
  "localsqlite",
];

const methods = [
  ".find('",
  ".findall('",
  ".query('",
  ".orderby('",
  ".orderbydescending('",
  ".where('",
];

const comparers = [
  ">",
  ">=",
  "<",
  "<=",
  "=",
  "==",
  "!=",
  "contains",
  "startwith",
];

const excludeMethods = [".skip(", ".take(", ".count("];

function getStr(model: any, position: any) {
  let startLine = position.lineNumber;
  while (startLine > 0) {
    const firstChar = model.getValueInRange({
      startLineNumber: startLine,
      startColumn: model.getLineFirstNonWhitespaceColumn(startLine),
      endLineNumber: startLine,
      endColumn: model.getLineFirstNonWhitespaceColumn(startLine) + 1,
    });

    const preLineLastChat = model.getValueInRange({
      startLineNumber: startLine - 1,
      startColumn: model.getLineLastNonWhitespaceColumn(
        startLine === 1 ? 1 : startLine - 1
      ),
      endLineNumber: startLine - 1,
      endColumn:
        model.getLineLastNonWhitespaceColumn(
          startLine === 1 ? 1 : startLine - 1
        ) + 1,
    });

    if (firstChar === "." || preLineLastChat === ".") {
      startLine--;
    } else {
      break;
    }
  }

  return model.getValueInRange({
    startLineNumber: startLine,
    startColumn: 1,
    endLineNumber: position.lineNumber,
    endColumn: position.column,
  });
}

function getResult(arr: any) {
  return arr.map(function (item: any) {
    return {
      label: item,
      kind: monaco.languages.CompletionItemKind.Property,
      documentation: item,
      insertText: item,
      preselect: true,
    };
  });
}

export default function registerDatabaseHint(enable: boolean) {
  _enable = enable;
  tables = {};
  columns = {};
  if (initialized) return;
  initialized = true;

  monaco.languages.registerCompletionItemProvider("typescript", {
    triggerCharacters: ['"', "'", "`", " ", "(", "."],
    async provideCompletionItems(model: any, position) {
      return {
        suggestions: (await getSuggestions(model, position)) as any,
      };
    },
  });

  monaco.languages.registerCompletionItemProvider("html", {
    triggerCharacters: ['"', "'", "`", " ", "(", "."],
    async provideCompletionItems(model: any, position) {
      // this api is hidden from the typedocs
      const lang: string = model.getLanguageIdAtPosition(
        position.lineNumber,
        position.column
      );

      const result = {
        suggestions: [],
      };

      if (lang == "javascript") {
        result.suggestions = await getSuggestions(model, position);
      }

      return result;
    },
  });
}

async function getSuggestions(model: any, position: any) {
  if (!_enable) return [];
  let str = getStr(model, position).replace(/\s+/g, "").toLowerCase();

  const database = databases.find((f) =>
    mathKeywords(str, ["k.db." + f, "k.module." + f])
  );

  if (!database) return [];

  if (!tables[database]) {
    const moduleStore = useModuleStore();
    tables[database] = await getTables(database, moduleStore.editingModule);
  }

  if (mathEnd(str, [`k.db.${database}.`, `k.module.${database}.`])) {
    return getResult(tables[database]);
  }

  str = str.replace(/\"+/g, "'").replace(/\`+/g, "'");

  const table = tables[database].find(function (f) {
    return mathKeywords(str, [
      `k.db.${database}.${f}`,
      `k.module.${database}.${f}`,
      `k.db.${database}.gettable('${f}')`,
      `k.module.${database}.gettable('${f}')`,
    ]);
  });

  if (!table) return [];
  const tableKey = `${database}_${table}`;

  if (!columns[tableKey]) {
    const moduleStore = useModuleStore();
    columns[tableKey] = await getColumns(
      database,
      table,
      moduleStore.editingModule
    );
  }

  if (mathKeywords(str, methods) && !mathKeywords(str, excludeMethods)) {
    const endsWithCol = mathEnd(
      str,
      columns[tableKey].map((m) => m.toLowerCase())
    );

    return getResult(endsWithCol ? comparers : columns[tableKey]);
  }

  return [];
}

function mathKeywords(str: string, keywords: string[]) {
  for (const keyword of keywords) {
    if (str.indexOf(keyword) > -1) return true;
  }
}

function mathEnd(str: string, keywords: string[]) {
  for (const keyword of keywords) {
    if (str.endsWith(keyword)) return true;
  }
}
