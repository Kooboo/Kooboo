# 动态交付（S6–S10）

只有用户主动要求动态化，或明确采纳动态化推荐后才读取本文。

## S6 触发

未确认动态化时停留在静态阶段。推荐卡片不是默认同意，不能据此创建数据结构、API、登录、订单或支付能力。

## S7 数据源决策

先输出实体到数据源的映射，并声明跨源关联键与 SSR 路线：

- Commerce 的 Product、Category、Variant、Cart、Customer、Order 默认以 `commerce` 为事实源；
- 普通内容使用 `content`，自定义关系型数据使用 `sqlite`，外部数据使用 `fetch`；
- SSR 默认 `<k-data>`；只有 k-data 无法表达的导入或专用 `k.*` 能力才使用 `env="server"`；
- 电商必须检查 Products、PDP、Header、Cart、Checkout、Order Result、Account/Orders 的共享状态和真实消费者，并读取 [Commerce 动态化契约](commerce-dynamicization.md)。

未完成数据源映射不得进入实现。

## S8 数据结构

映射包含 `sqlite` 或 `content` 时，先确认字段、约束和关联，再创建结构。Content 建模读取 `references/K-Script/content/function_guide.md`；实际 CRUD 再读取对应模块 reference。仅使用 `commerce` / `fetch` 时不创建平行数据模型。

## S9 动态实现

保持静态版的主要 DOM、topology、视觉层级和响应式结构，原地绑定真实数据与行为。

1. 轨道 A：优先 `<k-data>` 完成首屏查询、筛选、分页、字段计算和条件分支；
2. 轨道 B：API + 前端 JS 处理提交、持久化、鉴权、购物车、订单和支付；
3. API / CodeBlock 按需读取 [Backend 创建函数](../../Functions/backend-create.md) 与 Backend reference；
4. 动态图片仍使用 [ImagePlaceholder](../../Functions/image-placeholder.md) 或真实业务资源；
5. Commerce 交付以 `deliveryStage: "commerce-runtime"` 验证，支付阶段以 `payment-runtime` 验证；Preflight PASS 只证明结构，`completionGate.missingEvidence` 非空时不得完成 milestone。

静态菜单、tabs、gallery、hover 或 reveal 不属于本阶段，不能借动态化补做首版应有交互。

## S10 动态迭代

按用户反馈小步修改，每轮只把实际变更对象标记 dirty；已接入 AI Chat Completion Ledger 时，可显式用 `mode: "focused"`，adapter 会补齐 dirty 对象，模型负责补充既有直接消费者。每轮重新取得持久化回执和必要的同路径运行证据；完成某条 runtime requirement 的全部步骤后，只在最后一个成功浏览器断言上携带其 `verificationRequirementId`。完成整个动态能力阶段（或进入 commerce-runtime / payment-runtime 里程碑）时，必须以 `mode: "capability"` 扫描一次；adapter 会补齐本会话全部成功写入对象，模型再补充属于该能力但未修改的既有消费者。支付 Provider、callback、权限、订单状态等必须有业务结果证据，不能用结构扫描代替；最后一次写入后的 Checkout 表单可见性、API 子路由、订单归属和支付状态都属于同路径证据。只完成结构检查时保持 `structurally verified`；同路径运行已通过但 provider/admin 业务结果仍缺失时最多为 `runtime verified`。
