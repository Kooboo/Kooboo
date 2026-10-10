# Kooboo AI chat — function (tool) definitions

The 149 callable functions exposed to the Kooboo AI chat, exported from the compiled
`Kooboo.Sites` assembly as OpenAI-style tool definitions. One JSON file per function.

```
Tools/
  create_page.json
  update_text_object.json
  ...
```

Each file follows the tool-call schema the model receives:

```json
{
  "type": "function",
  "function": {
    "name": "create_page",
    "description": "Create or replace the named site object. ...",
    "parameters": {
      "type": "object",
      "properties": {
        "name": { "type": "string", "minLength": 1, "pattern": "^[a-zA-Z0-9_\\-\\.]{1,64}$" },
        "content": { "type": ["string", "null"] },
        "url": { "type": "string", "minLength": 1 }
      }
    }
  }
}
```

The `parameters` schema is generated from the function's C# input type, so it carries
validation rules (`required`, `minLength`, `pattern`, `enum`, `description`) rather than
just property names.

## Source

- Exporter: `Kooboo/scripts/ai/package-ai.sh` (Windows: `package-ai.bat`), which builds a
  throwaway `packages/.kooboo-ai-exporter` project against `Kooboo.Sites`, reflects over
  every non-abstract `BuiltInAIFunction` subclass in
  `Kooboo.Sites/AI/Functions/BuiltIn/**`, and writes one JSON per function into
  `packages/kooboo-ai/tools/`.
- Source revision: Kooboo `9ab1cc7b8` (branch `wg/ai-chatbox`, 2026-10-09),
  export run 2026-10-10.
- Result of that run: **149 tools created, 0 export errors**. The exporter only writes
  `_export-errors.json` when a function fails to export; no such file is present here.
- `packages/` is git-ignored in the Kooboo repository, so this directory is a copy of a
  build artifact — re-run the script to regenerate it.

## English only

These definitions have **no Chinese variant**. The `Description` property is a hard-coded
English string in each C# class (for example `Kooboo.Sites/AI/Functions/BuiltIn/Core/Page/CreatePage.cs`),
and no localized resource exists in the Kooboo source. Chinese content ships in the skills
instead — see [`Skills/zh/`](../Skills/zh).

## Refresh

```bash
cd /Users/achen/Priv/Yardi/Kooboo
bash scripts/ai/package-ai.sh
cp packages/kooboo-ai/tools/*.json /Users/achen/Priv/Yardi/kb-github-docs/Tools/
```

Related content: [`Skills/`](../Skills) holds the skill packages that reference many of
these functions by name.
