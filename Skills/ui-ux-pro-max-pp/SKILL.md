---
name: ui-ux-pro-max-pp
description: >-
  Use when Kooboo AI has entered site-building SOP S2/S3 and needs a Page Plan,
  Design System, or page View prompts. Also use for single-page, component,
  and local UI optimization in an existing site.
---

# UI/UX Pro Max++ Router

Run `scripts/search.js` through `run_skill_script`, placing all parameters inside `arguments`. A ToB App follows the independent `kooboo-coding` App Building flow and its bundled design-system reference; it does not use this Site Building planner. A new full site must load `kooboo-coding` and complete S0/S1 first; the order is `page-plan` → milestones → `design-system` → `page-item` / `view-batch`.

Every `run_skill_script` call supplies all three outer fields: `skillName: "ui-ux-pro-max-pp"`, `scriptName: "scripts/search.js"`, and `arguments`. Put `mode`, `brief`, and mode-specific inputs inside `arguments`; keep the outer fields on each call, including parallel calls. Example: `{"skillName":"ui-ux-pro-max-pp","scriptName":"scripts/search.js","arguments":{"mode":"help"}}`.

## Current Modes

| mode                     | Input                                                                                         |
| ------------------------ | --------------------------------------------------------------------------------------------- |
| `page-plan`              | `brief`                                                                                       |
| `design-system`          | `brief`                                                                                       |
| `page-item`              | `brief`、`pageName`                                                                           |
| `view-batch`             | `brief`、`blockRefs`                                                                          |
| `prompt-recommendations` | `query` or `brief`, plus `sectionType`, `pageName`, and optional `limit` / `excludePromptIds` |
| `search`                 | `query`, optional `domain` / `limit`                                                          |
| `stack`                  | `query`、`stack`                                                                              |
| `help`                   | No additional fields                                                                          |

Pass only the fields allowed by one mode at a time. Structured modes return an object directly; argument errors use this common response:

```json
{ "status": "error", "code": "invalid_arguments", "message": "..." }
```

## Minimal Site Brief Contract

Required: `verticalText`, string `verticalKeywords`, `siteType`, 0–2 `tone` values, `themeMode`, and `contentLocale`. `contentLocale` must be `zh` or `en`. `pages` is an optional override for the current batch; by default, the script selects the first pages by page role, the smallest complete experience loop, and a shared budget, then writes them back to `resolvedBrief.pages`.

The script normalizes ecommerce `commerceModel` from explicit transaction intent: use `catalog-contact` when the request contains only catalog intent such as selling, mall, store, or products; use `online-checkout` only when the current request or batch explicitly includes a cart, checkout, online ordering, or payment. Later calls reuse `resolvedBrief.commerceModel`; do not guess it again on the model side.

`siteType`：`ecommerce-catalog`、`ecommerce-luxury`、`ecommerce-florist`、`local-service`、`portfolio-creative`、`saas-product`、`content-publication`、`landing-leadgen`、`general-multipage`。

Pass `projectName` when the user specifies a brand. Leave `confirmedCategory` empty by default; only after the user explicitly confirms or corrects the industry, copy the exact returned `classification.category` or `topCandidates[].productType`. `siteType` describes site structure and is not a Product Type. Reuse `resolvedBrief` on later calls without reclassifying. Confidence is diagnostic; ask the user only when uncertainty changes business content or capability scope.

Seed ownership is strict:

- Put natural-language design decisions in `creativeDirection` and `palettePreference`.
- `designVariantSeed` is only an opaque UUID or script-returned `dvs-*` identity, never design prose, an industry term, a project name, or a placeholder.
- During the first Page Plan call in AI Chat, pass only `mode + brief` and omit `brief.designVariantSeed`; AI Chat injects the authoritative session context. Outside AI Chat, pass a real top-level `sessionId` or an explicit UUID as top-level `designVariantSeed`.
- Later calls reuse only the `resolvedBrief` returned by the script.

## On-Demand References

- S2 Page Plan and first-batch scope: [Site Planning](references/site-planning.md)
- S3/S4 Design System, Page Item, and View Batch: [Site Implementation](references/site-implementation.md)
- Single-page replacement, search, and component recommendations: [Local UI](references/local-ui.md)

## Completion Boundaries

Page Plan confirms only the pages and blocks in the current batch; Design System provides only site-wide visual variables; Page Item provides assembly order and Page composition groups across Views; View Batch is the sole source of authoritative View structure/content/capability prompts. The model composes the page using UUPM component prompt suggestions and optional [page composition guidance](references/page-composition.md), preserving those contracts. A structural Preflight pass does not mean runtime behavior, visual quality, or business results have been verified.

For `view-batch`, prefer compact `blockRefs` strings such as `["site-header", "site-footer"]`, or pass a list of plan objects carrying `blockRef`. For `brief.pages`, prefer `resolvedBrief.pages` strings, or pass page reference objects carrying `name`. Each list uses one representation and unique non-empty IDs. Objects select items by ID; their other metadata does not override the plan.
