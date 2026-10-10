---
name: ui-ux-pro-max-pp
description: >-
  Use when Kooboo AI 已进入建站 SOP S2/S3，需要 Page Plan、Design System、页面 View prompt；
  也用于已有站点的单页、组件和局部 UI 优化。
---

# UI/UX Pro Max++ Router

通过 `run_skill_script` 执行 `scripts/search.js`，参数只放在 `arguments`。ToB App 使用独立的 `kooboo-coding` App Building 流程及其内置设计系统 Reference，不使用这个 Site Building 规划器。新建整站必须先加载 `kooboo-coding` 并完成 S0/S1；顺序为 `page-plan` → milestones → `design-system` → `page-item` / `view-batch`。

每次 `run_skill_script` 都完整提供三个外层字段：`skillName: "ui-ux-pro-max-pp"`、`scriptName: "scripts/search.js"` 和 `arguments`。`mode`、`brief` 与 mode 专用参数放入 `arguments`；并行调用的每一项也要保留完整外层字段。例如：`{"skillName":"ui-ux-pro-max-pp","scriptName":"scripts/search.js","arguments":{"mode":"help"}}`。

## 当前 mode

| mode                     | 输入                                                                                  |
| ------------------------ | ------------------------------------------------------------------------------------- |
| `page-plan`              | `brief`                                                                               |
| `design-system`          | `brief`                                                                               |
| `page-item`              | `brief`、`pageName`                                                                   |
| `view-batch`             | `brief`、`blockRefs`                                                                  |
| `prompt-recommendations` | `query` 或 `brief`，以及 `sectionType`、`pageName`、可选 `limit` / `excludePromptIds` |
| `search`                 | `query`、可选 `domain` / `limit`                                                      |
| `stack`                  | `query`、`stack`                                                                      |
| `help`                   | 无附加字段                                                                            |

每次只传一个 mode 允许的字段。结构化 mode 直接返回对象；参数错误统一返回：

```json
{ "status": "error", "code": "invalid_arguments", "message": "..." }
```

## Site Brief 最小契约

必填：`verticalText`、字符串 `verticalKeywords`、`siteType`、0–2 项 `tone`、`themeMode`、`contentLocale`。`contentLocale` 只使用 `zh` 或 `en`。`pages` 为可选的当前批次覆盖项；默认首批页面由脚本按页面角色、最小体验闭环和统一预算选择，并写回 `resolvedBrief.pages`。

电商 `commerceModel` 由脚本按明确交易意图归一化：只出现“售卖 / 商城 / 商店 / 商品”等目录意图时使用 `catalog-contact`；只有当前请求或批次明确包含购物车、结算、在线下单或支付时才使用 `online-checkout`。后续调用复用 `resolvedBrief.commerceModel`，不要在模型侧重新猜测。

`siteType`：`ecommerce-catalog`、`ecommerce-luxury`、`ecommerce-florist`、`local-service`、`portfolio-creative`、`saas-product`、`content-publication`、`landing-leadgen`、`general-multipage`。

用户指定品牌名时传 `projectName`。`confirmedCategory` 默认留空；只有用户明确确认或纠正行业分类时，才使用返回的 `classification.category` 或 `topCandidates[].productType` 原值。`siteType` 是站点结构类型，不能复制到该字段。后续复用 `resolvedBrief`，不要重新分类。置信度是内部诊断；只有分类不确定会改变业务内容或功能范围时才向用户确认。

seed 所有权是硬约束：

- 自然语言设计决策只放入 `creativeDirection` 和 `palettePreference`。
- `designVariantSeed` 只能是不可读语义的 UUID 或脚本返回的 `dvs-*` 标识，禁止填写设计描述、行业词、项目名或占位值。
- AI Chat 首次调用 Page Plan 时只传 `mode + brief` 并省略 `brief.designVariantSeed`，由 AI Chat 注入权威会话上下文；AI Chat 外部调用则在顶层传真实 `sessionId`，或把明确 UUID 作为顶层 `designVariantSeed`。
- 后续调用只复用脚本返回的 `resolvedBrief`。

## 按需 Reference

- S2 Page Plan 与首批范围：[Site Planning](references/site-planning.md)
- S3/S4 Design System、Page Item、View Batch：[Site Implementation](references/site-implementation.md)
- 单页替换、搜索与组件推荐：[Local UI](references/local-ui.md)

## 完成边界

Page Plan 只确认当前批次页面和 blocks；Design System 只提供全站视觉变量；Page Item 提供装配顺序与跨 View 的 Page composition groups；View Batch 是 View 结构、内容和能力 Prompt 的唯一权威来源。模型结合 UUPM 组件 prompt 建议与按需[整页构图参考](references/page-composition.md)自主构图，并保留上述契约。Preflight 结构通过不等于运行态、视觉质量或业务结果已验证。

`view-batch` 的 `blockRefs` 优先使用紧凑字符串数组，例如 `["site-header", "site-footer"]`，也接受带 `blockRef` 的规划对象数组。`brief.pages` 优先复用 `resolvedBrief.pages` 字符串，也接受带 `name` 的页面引用对象。每个数组使用同一种表示，ID 必须唯一且非空。对象仅按 ID 选择条目，其他元数据不覆盖规划。
