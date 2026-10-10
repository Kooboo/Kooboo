# 静态交付（S4–S5）

本文仅在 Page Plan、站点 milestones 与 Design System 已完成后读取。

## S4 当前批次实现

只创建当前 `resolvedBrief.pages`。不得因为最终目标包含交易、账户或支付，就继续生成当前批次之外的页面。下一批次必须重新建立 todos，并复用 `resolvedBrief` 仅替换 `pages`。

“静态”只表示暂不接后端数据、鉴权、持久化和业务 API。移动菜单、tabs、accordion、gallery、静态筛选、hover/focus、reveal 等能在 DOM 内闭环的交互必须在首版完成。

### 站点级准备

1. 在创建第一个站点资源前调用 `get_site_info` 检查当前站点名称。仅当 `DisplayName` 为空或明显是 `ai_site_260813103`、`untitled`、纯编号或时间戳等系统临时名称时，才尝试调用 `update_site_settings`，把 `DisplayName` 更新为 `resolvedBrief.projectName`；已有有意义名称时保持不变，不修改内部 `Website.Name`、域名或路径。名称读取或更新失败时记录该限制并继续当前 S4 资源创建，不得阻塞站点交付。
2. 按需读取 [前端创建函数](../../Functions/frontend-create.md)、[代码风格](../../Frontend/code-style.md)、[Layout](../../Frontend/layout.md) 和 [设计变量](../design-variable-extraction.md)。
3. `create_layout`：Layout 引用共享 Header/Footer，并声明 `k-placeholder`。
4. `create_style`，必要时 `create_script`：创建全局变量、样式和共享静态交互。
5. 对 Page Plan `sharedBlocks[]` 一次调用 `view-batch`，逐项生成、创建并读回共享 View。

共享 View 只由 Layout 引用，不能再次放入 Page placeholder。

### 单页流水线

建议按 Page Plan 顺序逐页交付，首页或核心页优先。这是生成指导，不是单个对象写入的前置条件：

1. `page-item` 取得有序 `blockRefs[]` 与 `compositionGroups[]`；
2. 在 `create_page` 前设置 Page URL：需要独立访问的详情页使用 `/blog/{slug}` 或 `/product/{seoName}` 这类路径模板；query string 保留给列表状态和其它非身份参数。占位参数名必须与详情页 `k-data` 暴露的值一致；
3. 需要尽早预览时，`create_page` 可以先写入 Layout 包裹的轻量骨架；Views 已存在时可直接挂载。Page 负责普通 HTML/CSS 容器及跨 View composition wrapper；
4. 预览可用时读取 `read_preview_diagnostics(mode="issues")`，检查计算样式、溢出、裁切、重叠，以及 `uiAudit.colorAudit` 中的 Design System 配色漂移。定位并修复对应 Page/Layout/View/Style，再复检。诊断不可用时记录未验证，不阻止资源创建。`uiAudit.passed=true` 只提供通用运行时证据，不证明视觉质量或任务已完成；
5. 一次 `view-batch` 取得本页权威 prompts；
6. 模型结合 UUPM 组件建议完成整页构图，需要整页构图参考时，调用 `read_skill_resource`，参数为 `{"skillName":"ui-ux-pro-max-pp","resourceName":"references/page-composition.md"}`。每个 item 使用 `baseContract + 当前 item.prompt` 独立执行：生成 → `create_view` → `read_object_text` → Prompt Conformance Gate → 必要时 `update_text_object`。当前 Page 的 Views 全部存在后，必须立即按 Page Plan 顺序把每个空 `data-block` 骨架替换为对应的 `<view id="...">` 引用，同时保留必要的 composition wrapper；随后读回 Page，确认不再存在任何空 `data-block`。完成 milestone 或宣称交付前必须完成上述装配。只有装配读回完成后才运行 Preflight 并刷新真实 Page 路由。
7. 装配后读取 Page 与 1–3 个关键 View 做小步精修。渲染态视觉检查可用且已获授权时，在真实 Page 路由上检查桌面与移动端的焦点层级、图片裁切、标题换行、区块衔接和裁切问题。只返修最小责任 View/Page/CSS，并复核受影响结果。视觉 Preview 不可用时标记视觉验收未验证，继续其他页面与 capability Preflight；不得从源码或诊断结果推断视觉通过。

默认不能先铺完所有页面的 View 再创建 Page，也不能在 View URL 上验收用户页面。只有用户明确要求单文件 Page 时，才允许把最终 block 直接内联，但仍须先取得对应 `view-batch` prompt。

对象创建不依赖最后读取的 `page-item`、骨架回执或预览诊断结果。共享 Header/Footer Views 可独立于 Page 创建。读取多个页面规划不限制下一次可以创建哪个 Page。单独检查已持久化 Page 装配、引用资源和实际运行结果，修复真实问题后再声明完成。既有 capability Preflight 和完成证据要求继续保留。

### Page Composition Gate

`compositionGroups[]` 只规定跨 View 的任务信息架构，不限制具体设计方案。AI 可以调整列宽、gap、容器、sticky、边界和视觉处理，但 Page 必须承担 sibling View 的组合关系：

- `product-gallery + product-summary`：桌面端商品媒体主区与购买摘要相邻；
- `cart-items + checkout-summary`：桌面端购物车商品主区与金额/结算侧栏相邻；
- `checkout-form + checkout-summary`：桌面端结算表单主区与订单复核侧栏相邻；
- `order-status + checkout-summary`：桌面端订单状态主区与金额复核侧栏相邻。

移动端按 `stack-primary-first` 堆叠。除非用户明确要求其他结构，或内容/设备约束有充分理由，命中 group 的成员不得成为两个连续的桌面全宽 section。View prompt 中的 primary/aside placement 约束必须落实：View 在分配列内自适应，不自行创建跨 sibling wrapper，也不复制 sibling 的主职责。

两个成员 View 完成后读取 Page 并确认：它们位于同一个普通 HTML wrapper；桌面端存在真实 grid/flex 主区 + aside；移动端主区在前；购买、金额或复核行动没有被推到无关长内容之后。仍为桌面端上下全宽堆叠时视为 **FAIL**，先修改 Page HTML/CSS。

### Prompt Conformance Gate

逐 View 核对：

- View 是否落实 UUPM 组件职责，并与整页构图协调；
- required content 是否齐全；
- avoid patterns 是否未命中；
- grid/bento 中跨列、跨行或被同排元素拉伸的卡片，内部可见 wrapper 是否同步填满；内容不足时应缩小 span 或退出 stretch，不能留下空白卡片内部；
- 图片、数据和交互是否符合真实能力；
- 是否存在编造的电话、地址、价格、评价、认证或提交行为。

任一核心项失败，先修复并重新读回；普通 Preflight PASS 不能替代 prompt 合规。

### 静态交付证据

- 图片 URL 按需读取 [ImagePlaceholder](../../Functions/image-placeholder.md)，不编造图床地址；
- Page/Layout/View/Style/Script 创建前只读取当前对象所需 reference；
- 交付前读取 [Preflight](../../Preflight/index.md)，以 `deliveryStage: "static-first"` 扫描本批对象；
- 清单进度顺序：先对本批运行最终 capability Preflight，再调用 `update_site_milestones` 保存完整清单，只把已验证的条目标记为 `completed=true`；
- capability Preflight 返回 `site.ui-runtime` 证据要求时，针对最终渲染页面调用 `read_preview_diagnostics(mode="issues", verificationRequirementId="site.ui-runtime")`。只有 `uiAudit.passed=true` 且 `actionableErrorCount=0` 才能作为通用 UI 运行时回归证据；它不能替代任务特定的视觉验收，也不能单独证明用户报告的样式问题已修复。失败时继续修复，不得结束任务；
- 单独报告实际渲染态视觉检查的范围与结果；不可用时明确标记视觉质量未验证。源码读回、Prompt Conformance、`uiAudit` 和 Preflight 都不能单独证明最终构图达到目标视觉质量；
- active `BLOCKER` 必须修复，deferred 规则留到对应动态阶段。

## S5 静态迭代

接收反馈后优先局部修改 View，其次调整 Page block 顺序，最后才改 Layout 或全局 Style。已接入 AI Chat Completion Ledger 时，每轮可显式用 `mode: "focused"`，adapter 会补齐 dirty 对象，模型负责补充既有直接消费者。完成本批静态交付时必须以 `mode: "capability"` 扫描一次；adapter 会权威补齐本会话全部成功写入对象，模型再补充属于本批能力但未修改的既有消费者。

用户未要求动态化时可以在静态确认版结束。存在自然下一步时使用 `show_next_recommended_action`，推荐本身不构成授权。

获取规划和组件建议的 `search.js` 调用独立于页面骨架和预览诊断，可在诊断未完成时执行；预览问题通过页面修复与复检处理。
