# Site Implementation

## Design System

使用 Page Plan 的 `resolvedBrief`：

```json
{"mode":"design-system","brief":{}}
```

输出只保留实现需要的 `classification`、`pattern`、`style`、`colors`、`typography`、`keyEffects` 和 `antiPatterns`。默认配色以 `colors` 为权威；用户后续明确确认的变更必须同步到 Layout、CSS 变量、Tailwind 与 View。

## Page Item

```json
{"mode":"page-item","brief":{},"pageName":"home"}
```

返回 `version`、`pageName`、有序 `blockRefs[]`。只有当前页存在跨 View 的主栏/侧栏组合时，额外返回非空 `compositionGroups[]`，由 Page 负责组合 wrapper；普通页面不返回该字段。占位符由 `uupm-${blockRef}` 推导，共享 View 从 Page Plan `sharedBlocks[]` 单独处理。

`compositionGroups[]` 描述由 Page 普通 HTML/CSS 承担的跨 View 关系：

```json
{
  "groupRef":"product-detail-product-decision",
  "blockRefs":["product-detail-product-gallery","product-detail-product-summary"],
  "primaryBlockRef":"product-detail-product-gallery",
  "asideBlockRef":"product-detail-product-summary",
  "desktopTopology":"primary-aside",
  "responsiveStrategy":"stack-primary-first",
  "layoutIntent":"Place product media and the purchase summary side by side on desktop..."
}
```

Page 不限于连续放置 `<view>`。可以使用 `main`、`section`、`div`、`aside`、grid/flex wrapper、标题和 breadcrumbs，再把独立 View 放进对应区域。composition 只固定任务信息架构，不固定列宽、gap、sticky、圆角或视觉风格：

- `product-gallery + product-summary`：桌面端商品媒体主区与购买摘要相邻。
- `cart-items + checkout-summary`：桌面端购物车商品主区与金额/结算侧栏相邻。
- `checkout-form + checkout-summary`：桌面端结算表单主区与订单复核侧栏相邻。
- `order-status + checkout-summary`：桌面端订单状态主区与金额复核侧栏相邻。

移动端按 `stack-primary-first` 堆叠。用户明确要求其他结构，或内容/设备约束有充分理由时可以调整；仅因 View 被拆分、生成方便或创意调性不同，不得退化为桌面端上下排列的两个全宽 section。

## View Batch

```json
{"mode":"view-batch","brief":{},"blockRefs":["home-hero","home-features"]}
```

顶层 `baseContract` 只出现一次；每个 item 包含 `blockRef`、`recommendedViewName` 和差异化 `prompt`。该结果仍是 View 边界、block 顺序、required content、composition role、图片/数据/交互策略、语言、色板与真实能力约束的权威来源。

使用 `baseContract + 当前 item.prompt` 生成对应 View，保留 UUPM 的组件积木式建议，并结合用户要求统一整页主次、媒体、区块衔接与移动端层级。需要整页设计参考时，按需读取[整页构图](page-composition.md)。模型直接完成构图与生成，无需额外提交设计说明或取得验证回执。每个 View 独立执行：生成 → 创建 → 读回 → Prompt Conformance Gate → 必要时更新 → Preflight。

命中 composition group 时，item prompt 会说明当前 View 的 primary/aside 角色。View 应在分配列内保持流式宽度，不创建跨 sibling 的 Page wrapper，不复制 sibling 的职责；Page 负责两者的并排关系。商品摘要有独立 Gallery sibling 时，不再创建第二张占主导地位的商品大图。

Gate 至少核对组件职责、required content、avoid patterns、图片/数据真实性、交互真实性，以及 View 与整页构图是否协调。不得只看 Page Plan 摘要或行业常识生成通用 View。最终 HTML 只包含实际页面内容，不包含设计过程说明。
