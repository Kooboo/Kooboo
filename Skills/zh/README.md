# Kooboo AI 技能包

> **本目录面向 AI。** 这里存放 Kooboo AI 助手在运行时加载的技能包，用于判断如何建设和修改
> Kooboo 站点与应用。它**不是面向用户的文档**——想读 Kooboo 文档请从
> [`Docs/official-docs/zh/`](../../Docs/official-docs/zh) 开始。
>
> 英文原版：[English](../README.md) · 全仓库的机器可读地图：[llms.txt](../../llms.txt)

共 11 个技能、256 个文件。每个技能是一个目录，包含 `SKILL.md`（分流、
优先级与硬性规则）与 `references/`（仅在任务需要时才加载的详细材料）。

## 技能一览

| 技能 | 适用范围 | 文件数 |
| --- | --- | --- |
| [browser-control](browser-control/SKILL.md) | Browser control for the expanded right-side site preview, including observation, interaction, navigation, upload, and verification. / 控制右侧展开的站点预览，完成观察、交互、导航、上传和验证。 | 1 |
| [debug](debug/SKILL.md) | Diagnose and fix problems in the current Kooboo site through an evidence-first workflow using Preview diagnostics, Network, Browser Control, source inspection, and Preflight. / 排查并修复当前 Kooboo 站点中的白屏、报错、网络失败、交互异常和渲染问题。 | 4 |
| [history](history/SKILL.md) | Inspect Kooboo CMS history, list and filter change records, view object versions, compare revisions, undo selected changes, restore a website to a historical point, or check out a historical state as another website. Use… | 1 |
| [kooboo-ai-task-automation](kooboo-ai-task-automation/SKILL.md) | Use when the user wants to create or refine a Kooboo AI Task Job, fixed routine, scheduled task, or chat-created automation from natural language, including write-capable jobs such as scheduled posting, content sync, cle… | 3 |
| [kooboo-app-i18n](kooboo-app-i18n/SKILL.md) | 使用 Kooboo Label 配合 vue-i18n 为 Vue AI App 添加或维护多语言，包括语言切换、菜单与标题翻译、组件库语言。用户要求 App 国际化，或修改已接入该能力的 App 时使用；单语言 App 无需安装。 | 4 |
| [kooboo-cms-guide](kooboo-cms-guide/SKILL.md) | Use when guiding users to Kooboo CMS admin URLs, show_action_button, get_site_info (siteId, baseUrl), or JWT/payment/SMS/OAuth setup links. Always use documented deep links with correct path and query params. | 85 |
| [kooboo-coding](kooboo-coding/SKILL.md) | Kooboo Site / App / Scoped Coding Router。用于在讨论、整站建设、ToB App Blueprint 与 App Plan、 客户端渲染、服务端渲染、App 维护和局部编码之间分流。只有用户明确授权实施且本轮已有最低可执行需求时， 才能进入会写入资源的路径。 | 149 |
| [kooboo-jsonld](kooboo-jsonld/SKILL.md) | 用于在 Kooboo 页面、视图或布局中添加、检查和排查 JSON-LD 结构化数据：k-json-ld、schema.org、Product、Article、Organization、BreadcrumbList、富媒体搜索结果、字段映射以及 application/ld+json 未输出。涵盖模板语法和当前渲染限制。 | 1 |
| [lighthouse-optimizer](lighthouse-optimizer/SKILL.md) | 分析 Kooboo 站点的 Lighthouse 或 PageSpeed 结果，将失败 audits 及 DOM/资源证据定位到 Page、View、Layout、Script、Style、Image、API 或 CodeBlock，生成按优先级排列的优化建议，并且只在用户明确授权后实施修复和执行 Preview、Preflight 验证。用于 Lighthouse、Core Web Vitals、performance、accessi… | 2 |
| [sandbox](sandbox/SKILL.md) | Use a hidden Kooboo Sandbox site to copy selected site objects, make isolated changes, execute KData or KScript, render templates, and verify page output without changing the current site. / 使用隐藏的 Kooboo Sandbox 复制指定站点对象… | 1 |
| [ui-ux-pro-max-pp](ui-ux-pro-max-pp/SKILL.md) | Use when Kooboo AI 已进入建站 SOP S2/S3，需要 Page Plan、Design System、页面 View prompt； 也用于已有站点的单页、组件和局部 UI 优化。 | 5 |

## 技能详情

### `browser-control`

Browser control for the expanded right-side site preview, including observation, interaction, navigation, upload, and verification. / 控制右侧展开的站点预览，完成观察、交互、导航、上传和验证。

- 中文：[SKILL.md](browser-control/SKILL.md) · [references/](browser-control/references) · 共 1 个文件
- 英文原版：[../browser-control/SKILL.md](../browser-control/SKILL.md)

### `debug`

Diagnose and fix problems in the current Kooboo site through an evidence-first workflow using Preview diagnostics, Network, Browser Control, source inspection, and Preflight. / 排查并修复当前 Kooboo 站点中的白屏、报错、网络失败、交互异常和渲染问题。

- 中文：[SKILL.md](debug/SKILL.md) · [references/](debug/references) · 共 4 个文件
- 英文原版：[../debug/SKILL.md](../debug/SKILL.md)

### `history`

Inspect Kooboo CMS history, list and filter change records, view object versions, compare revisions, undo selected changes, restore a website to a historical point, or check out a historical state as another website. Use for audit, recovery, rollback, revision comparison, and historical checkout tasks involving k.history or native History functions. / 用于查看、比较、撤销、恢复或检出 Kooboo CMS 历史记录。

- 中文：[SKILL.md](history/SKILL.md) · [references/](history/references) · 共 1 个文件
- 英文原版：[../history/SKILL.md](../history/SKILL.md)

### `kooboo-ai-task-automation`

Use when the user wants to create or refine a Kooboo AI Task Job, fixed routine, scheduled task, or chat-created automation from natural language, including write-capable jobs such as scheduled posting, content sync, cleanup, SEO updates, reports, and inspections.

- 中文：[SKILL.md](kooboo-ai-task-automation/SKILL.md) · [references/](kooboo-ai-task-automation/references) · 共 3 个文件
- 英文原版：[../kooboo-ai-task-automation/SKILL.md](../kooboo-ai-task-automation/SKILL.md)

### `kooboo-app-i18n`

使用 Kooboo Label 配合 vue-i18n 为 Vue AI App 添加或维护多语言，包括语言切换、菜单与标题翻译、组件库语言。用户要求 App 国际化，或修改已接入该能力的 App 时使用；单语言 App 无需安装。

- 中文：[SKILL.md](kooboo-app-i18n/SKILL.md) · [references/](kooboo-app-i18n/references) · 共 4 个文件
- 英文原版：[../kooboo-app-i18n/SKILL.md](../kooboo-app-i18n/SKILL.md)

### `kooboo-cms-guide`

Use when guiding users to Kooboo CMS admin URLs, show_action_button, get_site_info (siteId, baseUrl), or JWT/payment/SMS/OAuth setup links. Always use documented deep links with correct path and query params.

- 中文：[SKILL.md](kooboo-cms-guide/SKILL.md) · [references/](kooboo-cms-guide/references) · 共 85 个文件
- 英文原版：[../kooboo-cms-guide/SKILL.md](../kooboo-cms-guide/SKILL.md)

### `kooboo-coding`

Kooboo Site / App / Scoped Coding Router。用于在讨论、整站建设、ToB App Blueprint 与 App Plan、 客户端渲染、服务端渲染、App 维护和局部编码之间分流。只有用户明确授权实施且本轮已有最低可执行需求时， 才能进入会写入资源的路径。

- 中文：[SKILL.md](kooboo-coding/SKILL.md) · [references/](kooboo-coding/references) · 共 149 个文件
- 英文原版：[../kooboo-coding/SKILL.md](../kooboo-coding/SKILL.md)

### `kooboo-jsonld`

用于在 Kooboo 页面、视图或布局中添加、检查和排查 JSON-LD 结构化数据：k-json-ld、schema.org、Product、Article、Organization、BreadcrumbList、富媒体搜索结果、字段映射以及 application/ld+json 未输出。涵盖模板语法和当前渲染限制。

- 中文：[SKILL.md](kooboo-jsonld/SKILL.md) · [references/](kooboo-jsonld/references) · 共 1 个文件
- 英文原版：[../kooboo-jsonld/SKILL.md](../kooboo-jsonld/SKILL.md)

### `lighthouse-optimizer`

分析 Kooboo 站点的 Lighthouse 或 PageSpeed 结果，将失败 audits 及 DOM/资源证据定位到 Page、View、Layout、Script、Style、Image、API 或 CodeBlock，生成按优先级排列的优化建议，并且只在用户明确授权后实施修复和执行 Preview、Preflight 验证。用于 Lighthouse、Core Web Vitals、performance、accessibility、SEO、best practices 优化，以及只读的定时 Lighthouse 巡检。

- 中文：[SKILL.md](lighthouse-optimizer/SKILL.md) · [references/](lighthouse-optimizer/references) · 共 2 个文件
- 英文原版：[../lighthouse-optimizer/SKILL.md](../lighthouse-optimizer/SKILL.md)

### `sandbox`

Use a hidden Kooboo Sandbox site to copy selected site objects, make isolated changes, execute KData or KScript, render templates, and verify page output without changing the current site. / 使用隐藏的 Kooboo Sandbox 复制指定站点对象、 隔离修改并验证 KData、KScript、模板和页面输出。

- 中文：[SKILL.md](sandbox/SKILL.md) · [references/](sandbox/references) · 共 1 个文件
- 英文原版：[../sandbox/SKILL.md](../sandbox/SKILL.md)

### `ui-ux-pro-max-pp`

Use when Kooboo AI 已进入建站 SOP S2/S3，需要 Page Plan、Design System、页面 View prompt； 也用于已有站点的单页、组件和局部 UI 优化。

- 中文：[SKILL.md](ui-ux-pro-max-pp/SKILL.md) · [references/](ui-ux-pro-max-pp/references) · 共 5 个文件
- 英文原版：[../ui-ux-pro-max-pp/SKILL.md](../ui-ux-pro-max-pp/SKILL.md)
