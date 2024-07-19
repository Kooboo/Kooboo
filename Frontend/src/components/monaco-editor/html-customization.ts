import type { languages } from "monaco-editor";
import type { Suggestions, Tag, Attribute } from "@/api/site/kscript";
import { monaco } from "./userWorker";

export async function applyCustomHtml(suggestions: Suggestions[]) {
  const options: languages.html.Options = {
    data: {
      useDefaultDataProvider: true,
      dataProviders: {},
    },
  };

  for (const item of suggestions) {
    (options.data?.dataProviders as any)[item.name] = getProvider(item);
  }

  monaco.languages.html.htmlDefaults.setOptions(options);
}

function getProvider(suggestion: Suggestions) {
  return {
    version: 1.1,
    tags: getTags(suggestion.tags),
    globalAttributes: getAttributes(suggestion.globalAttributes),
  };
}

function getTags(tags: Tag[]) {
  const list: languages.html.ITagData[] = [];

  if (tags) {
    for (const i of tags) {
      list.push({
        name: i.name,
        description: { kind: "markdown", value: i.description },
        attributes: getAttributes(i.attributes),
      });
    }
  }

  return list;
}

function getAttributes(attributes: Attribute[]) {
  const list: languages.html.IAttributeData[] = [];

  if (attributes) {
    for (const i of attributes) {
      list.push({
        name: i.name,
        description: { kind: "markdown", value: i.description },
        valueSet: i.values ? i.values[0]?.name : undefined,
        values: i.values
          ? i.values.map((m) => ({
              name: m.name,
              description: { kind: "markdown", value: m.description },
            }))
          : [],
      });
    }
  }

  return list;
}
