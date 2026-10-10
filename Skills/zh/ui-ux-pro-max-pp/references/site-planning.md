# Site Planning

## 调用

```json
{
  "skillName":"ui-ux-pro-max-pp",
  "scriptName":"scripts/search.js",
  "arguments":{"mode":"page-plan","brief":{}}
}
```

AI Chat 首次生成 Page Plan 时，模型必须省略 `brief.designVariantSeed` 和顶层会话字段，由 AI Chat 注入权威 `sessionId`。AI Chat 外部调用应在顶层传真实 `sessionId`，或把明确 UUID 作为顶层 `designVariantSeed`。seed 是不透明的身份值，不是设计文案；描述性方向放入 `brief.creativeDirection`。本次调用后原样复用 `resolvedBrief`，以其中脚本返回的 `dvs-*` seed 为权威值。

Site Brief 必须从当前请求生成。`pages` 未传时，脚本在内部候选结构上使用页面角色选择首批范围：

`brief.contentLocale` 只使用 `zh` 或 `en`，并由脚本以小写形式写回 `resolvedBrief`。

1. 主要入口；
2. 浏览或核心证明路径；
3. 评估或行动闭环；
4. 通常不超过 3 页、约 9 个页面专属 blocks；
5. 依赖真实业务状态的任务页默认后置；
6. 信任、联系和详情类页面只在当前闭环需要时进入。

页面角色、评分和未选候选属于脚本内部规划元数据，不进入 Page Plan，也不要求模型手工提交。用户明确指定当前批次时才传 `pages`，脚本严格按该数组生成。

## Page Plan v3

```json
{
  "version":3,
  "resolvedBrief":{},
  "decisionReceipts":[],
  "classification":{},
  "sharedBlocks":[{"blockRef":"site-header","label":"Header 导航","promptId":"..."}],
  "pages":[{
    "name":"home",
    "title":"首页",
    "purpose":"...",
    "blocks":[{"blockRef":"home-hero","label":"Hero 主视觉","promptId":"..."}]
  }]
}
```

共享 Header/Footer 只出现在 `sharedBlocks[]`。Page Plan 不携带完整 prompt、装配占位符、选择诊断或执行策略。

后续批次复用 `resolvedBrief`，只替换 `pages` 后重新调用 `page-plan`；保持项目名、seed 和设计方向。`commerceModel` 表达站点最终交易能力，不自动扩大当前批次。首次规划时脚本执行确定性分流：模糊的“售卖 / 商城 / 商店”归入 `catalog-contact`；只有明确的购物车、结算、在线下单或支付意图才进入 `online-checkout`。

## 首页构图默认

电商、内容刊和本地服务首页优先摄影主导或搜索/编辑式首屏，而不是营销落地页积木：

- 电商首页默认 `hero` + `featured-products`，首批不再默认塞入 `trust-bar` / 中段 `cta` 墙。
- Hero 采样对 `split-left` / `split-reverse` 营销分栏降权，对全幅叠字、编辑格、lookbook、品类或搜索主导拓扑加权。
- Anti-slop 将左文案右图视为默认 AI 落地页特征；静默全幅裸图墙仍然禁止。

## Brief 复用与分类证据

后续调用将返回的 `resolvedBrief` 直接作为 `brief`，优先复用其紧凑的 `pages` 字符串，例如 `["home","products","product-detail"]`。也接受带 `name` 的页面引用对象数组，仅按名称选择当前批次，其他元数据不覆盖规划。每个数组使用同一种表示，名称必须唯一且非空。`resolvedBrief.pages` 始终返回规范化字符串。保留返回的 seed。

关键词证据过少时，分类回退为 `classification.category="General"`，不套用无关行业的设计。`topCandidates` 仅供诊断，不代表已确认选择。结合原始请求判断是否需要澄清会影响业务内容或功能范围的问题；任务已经明确时，继续遵循该任务的页面结构与设计要求，不要求用户选择内部分类标签。

行业关键词不代表奢侈品牌定位。通过 `tone`、`creativeDirection`、`palettePreference` 表达视觉意图。
