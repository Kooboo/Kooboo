# 建站规划阶段（S0–S3）

本文只在已通过 [建站流程入口](index.md) 后读取，负责从执行就绪检查到 Design System，不包含任何站点资源创建步骤。

## S0 执行就绪检查

同时满足两道闸门：

- 用户明确要求现在创建、搭建、生成、修改或继续实施；
- 新站能确定业务目标与首个交付范围，改造能确定对象与期望结果。

空站点、行业描述、能力咨询或“快速做”本身都不构成实施授权。缺少条件时只收敛最大的缺口。

## S1 起步模式

S0 通过后必须用 `ask_user_input` 选择：

- P1 快速模式：推荐项；补充实现默认值，S2/S3 不逐项追问；
- P2 详细模式：一次一问收敛关键决策。

`buildMode` 只能来自这次交互。取得有效值后，先用 `update_todos` 建立仅包含 S2 的当前待办，再进入规划。

## S2 Site Brief 与 Page Plan

### Site Brief 当前契约

| 字段 | 约束 |
|---|---|
| `verticalText` | 用户可读业务描述 |
| `verticalKeywords` | 英文行业检索词字符串，空格分隔 |
| `siteType` | 与当前请求匹配的有效站型 |
| `tone` | 0–2 项；只保留有语义依据的调性 |
| `themeMode` | `light` / `dark` |
| `contentLocale` | `zh` / `en` |
| `pages` | 可选；仅在用户明确指定当前批次页面时传入 |
| `commerceModel` | 仅电商必填：`catalog-contact` / `online-checkout` |
| `projectName` | 用户已指定时传入；否则由脚本生成 |
| `confirmedCategory` | 默认留空；用户明确确认后，取 `classification.category` 或候选 `productType` 原值，不使用 `siteType` |
| 设计方向 | 自然语言设计决策放入 `creativeDirection` 和 `palettePreference` |
| `designVariantSeed` | 只能是不可读语义的 UUID 或脚本返回的 `dvs-*`；禁止设计描述、行业词、项目名或占位值 |

AI Chat 首次调用只传 `mode: "page-plan"` 与 `brief`，并省略 `brief.designVariantSeed`。权威会话上下文由 AI Chat 注入；AI Chat 外部调用则在顶层传真实 `sessionId`，或把明确 UUID 作为顶层 `designVariantSeed`。Page Plan 返回后，以 `resolvedBrief` 作为后续唯一 Brief。

### 首批页面选择

未显式传入 `pages` 时，脚本从完整候选结构中选择满足最小体验闭环的首批页面：

1. 一个主要入口；
2. 一个浏览或核心能力/内容证明路径；
3. 一个评估或行动闭环；
4. 首批通常不超过 3 页、约 9 个页面专属 blocks；共享 Header/Footer 单独计算；
5. 依赖真实业务状态的任务页默认进入后续 milestone，除非用户明确指定当前批次；
6. 信任、联系和详情类页面根据当前站点是否仍缺对应闭环决定。

`pages` 只用于显式批次覆盖。未传时，脚本把选中页面写入 `resolvedBrief.pages`；后续批次复用同一 `resolvedBrief`，只替换 `pages`。

### URL 与详情页路由契约

为每个需要独立访问的列表/详情体验规划 URL 形状：

- 详情页的规范身份使用路径参数。文章优先使用 `/blog/{slug}`，Commerce 商品优先使用 `/product/{seoName}`。
- 查询字符串用于列表状态，例如搜索、筛选、排序和分页（如 `/products?category=coffee&page=2`）；当路径参数可以表达详情记录身份时，不要把规范身份放进 query string。
- Page 的 `url`、列表/详情链接和详情页 `k-data` 使用完全一致的占位参数名。`/blog/{slug}` 在渲染上下文中读取为 `slug`，`/product/{seoName}` 读取为 `seoName`。
- 只有需要兼容既有公开 URL 时才保留原 URL；新详情页默认采用路径优先约定，并使用稳定的 slug/`seoName`，不能使用展示标题或数组索引。

### P1 / P2 行为

- P1：满足前置条件后零提问生成 Brief 与 Page Plan；`classification.categoryConfidence` 表示行业证据的强弱，先结合原始请求判断；只有歧义会改变业务内容或功能范围时才做一次确认。medium 置信度本身不要求提问。
- P2：按目标、受众、范围、内容、设计、交互、动态意向的缺口一次一问；Page Plan 需用户确认。
- 两种模式都必须简要展示 Brief 与 Page Plan Summary，不把完整选择过程或内部页面角色元数据写入对话。

### S2 停止条件

- Brief 的必填字段有效；Page Plan 已返回 `resolvedBrief`、页面与 blocks；
- 当前批次可交付，设计与内容策略可执行；
- 电商的最终交易意图已通过 `commerceModel` 明确，但不因此扩张当前批次；
- 已读取 [能力目录](capability-catalog.md)，并基于当前站点主干路径调用 `update_site_milestones`。

## S3 Design System

先完成 S2 todos，再切换为 S3 todos。加载 `ui-ux-pro-max-pp` 后，以 Page Plan 的 `resolvedBrief` 调用 `mode: "design-system"`。

- 输出只作为全站视觉变量、布局策略和组件语气的权威来源；
- 若包含 motion、tabs、accordion、gallery、menu 等意图，S4 前按需读取 [Motion 与静态交互](../../Frontend/motion.md)；
- P1 展示设计摘要后直接进入 S4；P2 等用户确认；
- 进入 S4 前完成 S3 todos，并读取 [静态交付](static-delivery.md)。
