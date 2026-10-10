# Kooboo AI skills

> **This directory is AI-facing.** It holds the skill packages that the Kooboo AI assistant
> loads at runtime to decide how to build and modify Kooboo sites and apps. It is *not*
> end-user documentation — readers looking for Kooboo documentation should start at
> [`Docs/official-docs/`](../Docs/official-docs).
>
> Chinese mirror: [zh/](zh) · Machine-readable map of the whole repository: [llms.txt](../../llms.txt)

11 skills, 747 files. A skill is a folder containing `SKILL.md` (routing,
priorities and hard rules) and `references/` (detailed material loaded only when a task needs it).

## Skills at a glance

| Skill | Applies to | Files |
| --- | --- | --- |
| [browser-control](browser-control/SKILL.md) | Browser control for the expanded right-side site preview, including observation, interaction, navigation, upload, and verification. | 1 |
| [debug](debug/SKILL.md) | Diagnose and fix problems in the current Kooboo site through an evidence-first workflow using Preview diagnostics, Network, Browser Control, source inspection, and Preflight. / Diagnose and fix blank pages, errors, netwo… | 4 |
| [history](history/SKILL.md) | Inspect Kooboo CMS history, list and filter change records, view object versions, compare revisions, undo selected changes, restore a website to a historical point, or check out a historical state as another website. Use… | 1 |
| [kooboo-ai-task-automation](kooboo-ai-task-automation/SKILL.md) | Use when the user wants to create or refine a Kooboo AI Task Job, fixed routine, scheduled task, or chat-created automation from natural language, including write-capable jobs such as scheduled posting, content sync, cle… | 3 |
| [kooboo-app-i18n](kooboo-app-i18n/SKILL.md) | Add or maintain multilingual Vue AI Apps using Kooboo Labels and vue-i18n, including language switching, translated menus and titles, and component-library locales. Use for requested App internationalization or changes t… | 11 |
| [kooboo-cms-guide](kooboo-cms-guide/SKILL.md) | Use when guiding users to Kooboo CMS admin URLs, show_action_button, get_site_info (siteId, baseUrl), or JWT/payment/SMS/OAuth setup links. Always use documented deep links with correct path and query params. | 85 |
| [kooboo-coding](kooboo-coding/SKILL.md) | Kooboo Site / App / Scoped Coding Router. Routes discussion, full-site builds, ToB App Blueprint and App Plan, client-side rendering, server-side rendering, App maintenance, external page import, and scoped coding tasks.… | 614 |
| [kooboo-jsonld](kooboo-jsonld/SKILL.md) | Use when adding, inspecting, or debugging JSON-LD structured data in Kooboo pages, views, or layouts: k-json-ld, schema.org, Product, Article, Organization, BreadcrumbList, rich results, field mappings, or missing applic… | 1 |
| [lighthouse-optimizer](lighthouse-optimizer/SKILL.md) | Analyze Lighthouse or PageSpeed results for a Kooboo site, map failed audits and DOM/resource evidence to Pages, Views, Layouts, Scripts, Styles, Images, APIs, or CodeBlocks, produce prioritized optimization recommendati… | 3 |
| [sandbox](sandbox/SKILL.md) | Use a hidden Kooboo Sandbox site to copy selected site objects, make isolated changes, execute KData or KScript, render templates, and verify page output without changing the current site. | 1 |
| [ui-ux-pro-max-pp](ui-ux-pro-max-pp/SKILL.md) | Use when Kooboo AI has entered site-building SOP S2/S3 and needs a Page Plan, Design System, or page View prompts. Also use for single-page, component, and local UI optimization in an existing site. | 23 |

## Skill details

### `browser-control`

Browser control for the expanded right-side site preview, including observation, interaction, navigation, upload, and verification.

- English: [SKILL.md](browser-control/SKILL.md) · [references/](browser-control/references) · 1 files
- Chinese: [zh/browser-control/SKILL.md](zh/browser-control/SKILL.md)

### `debug`

Diagnose and fix problems in the current Kooboo site through an evidence-first workflow using Preview diagnostics, Network, Browser Control, source inspection, and Preflight. / Diagnose and fix blank pages, errors, network failures, interaction anomalies, and rendering problems in the current Kooboo site.

- English: [SKILL.md](debug/SKILL.md) · [references/](debug/references) · 4 files
- Chinese: [zh/debug/SKILL.md](zh/debug/SKILL.md)

### `history`

Inspect Kooboo CMS history, list and filter change records, view object versions, compare revisions, undo selected changes, restore a website to a historical point, or check out a historical state as another website. Use for audit, recovery, rollback, revision comparison, and historical checkout tasks involving k.history or native History functions.

- English: [SKILL.md](history/SKILL.md) · [references/](history/references) · 1 files
- Chinese: [zh/history/SKILL.md](zh/history/SKILL.md)

### `kooboo-ai-task-automation`

Use when the user wants to create or refine a Kooboo AI Task Job, fixed routine, scheduled task, or chat-created automation from natural language, including write-capable jobs such as scheduled posting, content sync, cleanup, SEO updates, reports, and inspections.

- English: [SKILL.md](kooboo-ai-task-automation/SKILL.md) · [references/](kooboo-ai-task-automation/references) · 3 files
- Chinese: [zh/kooboo-ai-task-automation/SKILL.md](zh/kooboo-ai-task-automation/SKILL.md)

### `kooboo-app-i18n`

Add or maintain multilingual Vue AI Apps using Kooboo Labels and vue-i18n, including language switching, translated menus and titles, and component-library locales. Use for requested App internationalization or changes to an App already using this integration; single-language Apps need no installation.

- English: [SKILL.md](kooboo-app-i18n/SKILL.md) · [references/](kooboo-app-i18n/references) · 11 files
- Chinese: [zh/kooboo-app-i18n/SKILL.md](zh/kooboo-app-i18n/SKILL.md)

### `kooboo-cms-guide`

Use when guiding users to Kooboo CMS admin URLs, show_action_button, get_site_info (siteId, baseUrl), or JWT/payment/SMS/OAuth setup links. Always use documented deep links with correct path and query params.

- English: [SKILL.md](kooboo-cms-guide/SKILL.md) · [references/](kooboo-cms-guide/references) · 85 files
- Chinese: [zh/kooboo-cms-guide/SKILL.md](zh/kooboo-cms-guide/SKILL.md)

### `kooboo-coding`

Kooboo Site / App / Scoped Coding Router. Routes discussion, full-site builds, ToB App Blueprint and App Plan, client-side rendering, server-side rendering, App maintenance, external page import, and scoped coding tasks. Enter a resource-writing path only when the user explicitly authorizes implementation and this turn has the minimum executable requirements.

- English: [SKILL.md](kooboo-coding/SKILL.md) · [references/](kooboo-coding/references) · 614 files
- Chinese: [zh/kooboo-coding/SKILL.md](zh/kooboo-coding/SKILL.md)

### `kooboo-jsonld`

Use when adding, inspecting, or debugging JSON-LD structured data in Kooboo pages, views, or layouts: k-json-ld, schema.org, Product, Article, Organization, BreadcrumbList, rich results, field mappings, or missing application/ld+json output. Covers template syntax and current rendering limitations.

- English: [SKILL.md](kooboo-jsonld/SKILL.md) · [references/](kooboo-jsonld/references) · 1 files
- Chinese: [zh/kooboo-jsonld/SKILL.md](zh/kooboo-jsonld/SKILL.md)

### `lighthouse-optimizer`

Analyze Lighthouse or PageSpeed results for a Kooboo site, map failed audits and DOM/resource evidence to Pages, Views, Layouts, Scripts, Styles, Images, APIs, or CodeBlocks, produce prioritized optimization recommendations, and implement fixes with Preview and Preflight verification only after explicit user authorization. Use for Lighthouse, Core Web Vitals, performance, accessibility, SEO, best-practices optimization, and read-only scheduled Lighthouse inspections.

- English: [SKILL.md](lighthouse-optimizer/SKILL.md) · [references/](lighthouse-optimizer/references) · 3 files
- Chinese: [zh/lighthouse-optimizer/SKILL.md](zh/lighthouse-optimizer/SKILL.md)

### `sandbox`

Use a hidden Kooboo Sandbox site to copy selected site objects, make isolated changes, execute KData or KScript, render templates, and verify page output without changing the current site.

- English: [SKILL.md](sandbox/SKILL.md) · [references/](sandbox/references) · 1 files
- Chinese: [zh/sandbox/SKILL.md](zh/sandbox/SKILL.md)

### `ui-ux-pro-max-pp`

Use when Kooboo AI has entered site-building SOP S2/S3 and needs a Page Plan, Design System, or page View prompts. Also use for single-page, component, and local UI optimization in an existing site.

- English: [SKILL.md](ui-ux-pro-max-pp/SKILL.md) · [references/](ui-ux-pro-max-pp/references) · 23 files
- Chinese: [zh/ui-ux-pro-max-pp/SKILL.md](zh/ui-ux-pro-max-pp/SKILL.md)
