---
name: kooboo-coding
description: >-
  Kooboo Site / App / Scoped Coding Router。用于在讨论、整站建设、ToB App Blueprint 与 App Plan、
  客户端渲染、服务端渲染、App 维护和局部编码之间分流。只有用户明确授权实施且本轮已有最低可执行需求时，
  才能进入会写入资源的路径。
---

# Kooboo Coding Router

本文件负责选择正确的 reference，并定义 Kooboo 编码的公共执行优先级和边界。公开 Site 建设读取 [Site Building 入口](references/Scenario/site-building/index.md)，再按阶段加载 planning/static/dynamic reference；新 ToB App 规划或 Milestone 实施读取 [App Building 入口](references/Scenario/app-building/index.md)。两条路径互斥，不能把尚未实现的 App 资源交付降级为普通建站。

用户要求 Vue App 多语言，或修改已经接入 `service.i18n.js` 的 App 时，加载 `kooboo-app-i18n`。该技能使用 Kooboo Label 配合 vue-i18n，仅在需要时安装运行时；站点开启多语言不代表 App 必须安装。

## 执行优先级与暂停条件

- **浏览器验证由用户明确触发。** 普通实施和修复使用源码读回、结构检查及要求的只读 Preview diagnostics；除非用户明确要求浏览器测试或操作，不主动执行浏览器/iframe 观察、元素查找/检查、导航、点击或设备切换。缺少浏览器观察不阻塞业务实施；未实测的视觉与交互如实标为未验证，不编造验收证据。此边界适用于所有 coding reference 中的验证要求。
- 适用的 Skill 要求优先于速度、实现便利和提前汇报。快速模式只改变规划默认值，不降低实施质量或验收标准。更高优先级指令和用户明确授权仍然有效；保留已有授权并应用契约中明确的例外，不重复索取确认。
- “必须、不得、禁止、只能、完成前”等要求在其适用范围和阶段内均为硬约束，不能自行理解成建议。根据当前任务、契约和明确例外判断适用性，不仅按关键词判断，也不能因实现困难编造豁免。
- 不确定是否符合规范时，暂停依赖该判断的写入，检查相关当前源码、文档或实际画面。复用已读证据，只补足解决当前疑问所需的信息；不得猜测、凭记忆套常见实现，或用“先临时做一下”绕过要求。
- 发现违规或前置条件未满足时，停止依赖该问题的后续功能扩展，先修复当前单元、回读并复验受影响的验收项，再进入下一阶段。必要时只撤销本轮引入的缺陷变更，保留用户其他改动。暂停当前阶段不等于结束已授权任务：问题解决后直接继续；只有确实缺少信息或授权时才询问用户。
- 保存成功、语法检查或通用 diagnostics 通过不等于功能完成。完成声明必须由当前任务/阶段的验收条件和足够的最新证据支持。缺少证据就保留未验证状态，不关闭 Todo/Milestone，不把未来工作清单当成交付。
- 不得用占位、演示或临时行为替代要求完成的实现。契约规定的阶段占位、已有授权的 mock 策略，仅在对应范围内有效，不能证明后续阶段已完成。新增的交付降级必须先说明差异并获得用户授权，不能先实施再解释。

站点与 App 共用交互规则：规划页面交互或编写／修改前端交互代码前，必须读取[前端交互指南](references/Frontend/interaction-guide.md)，包括既有页面的局部交互开发。本会话已完整读取且内容未变时不重复加载；纯样式调整不因此增加规划步骤或扩大修改范围。

网站与 App 共用图片/文件规则：涉及配图或文件上传、替换、删除、导入、生成时，先读取 [图片与文件管理](references/Frontend/images-and-files.md)。缺少样例配图使用 ImagePlaceholder；可编辑图片提供上传，不以粘贴链接替代；持久文件必须实现归属、引用和临时/无引用文件清理。按当前静态/动态阶段实施，不新增规划流程。

## 1. 先判断授权和任务类型

用户明确选择仅更新 Blueprint/App Plan 时，只授权对应规划的局部更新与一致性检查，完成后停止，不进入代码或资源写入。

分别判断：

1. 用户是否明确要求现在创建、搭建、生成、修改、修复或继续实施。
2. 是否能识别当前产品目标、核心业务结果或明确的局部对象。

未获得实施授权时，只讨论、解释或进行必要的只读检查，不创建或修改 Kooboo 资源。

### 既有对象修复优先

修复已有 App 或 Site 对象时，即使对话已有 Blueprint、App Plan 或未完成 Milestone，也先走本局部路径。本路径优先于下文技术文档读取和 App 规划门槛；授权和 Sandbox 边界仍然适用，不能用于实施尚未交付的功能或改变业务范围。

- 以用户最新症状或选中元素的 annotation 为目标。保留此前修复结果作为背景，不得用旧运行时问题的修复结果回答新的排列问题。用户在已授权工作中纠正范围时，直接继续修正后的任务，不重复索要修复授权。
- **简单 UI 调整不回规划**：仅修改间距、对齐、颜色、字体、尺寸、响应式排列或已有控件的展示位置，且不改变数据结构、业务含义、操作行为、权限、路由契约或功能范围时，直接读源码、修改、读回并复验目标 UI。适用于已有 App 内的元素，包括 Shell/Header；不读取、校验、创建或更新 Blueprint/App Plan，不运行会回读 App Plan 的 Preflight，也不更新 Milestone 清单。样式写在 Script 或共享组件中不改变这一判断，以实际变更语义为准。此例外优先于下文通用 Preflight 要求；保留既有 Completion Ledger 和待验对象，局部 UI 复验通过不代表整个 App/Milestone 验收通过。若源码检查发现需要改变业务契约，再按实际影响读取对应契约。
- 间距、对齐、大小、颜色、排列问题：检查选中元素、父级和相邻元素，以及所属标记和实际加载的 Style 规则。沿真实资源引用定位；客户端页面的标记可能位于 Script。补丁前通过 `read_object_text` 读取当前源码。已知可靠源码行号时，先读取覆盖目标及其所属结构的窗口，再按需扩大。窗口参数和安全补丁规则见[对象读取与更新](references/Functions/read-update.md)；要求完整读取的场景仍须覆盖全部内容。控制台无错误或 `uiAudit` 通过不能证明目标外观正确。
- 运行时错误：读取一次 issues，随后立即读取报错组件/Script 和相关依赖。沿失败表达式检查 import、声明、返回给模板的绑定及数据结构。仅在资源或数据请求相关时读取 Network，不用反复轮询相同报错代替源码检查。
- 复用上下文已有的 reference 和 Function Schema。只有具体契约未明确时才加载其他技术 reference；普通 CSS/JavaScript 修改不需要重读框架 overview、组件规范、交付规范或 App 规划文档。缺少源码工具时，通过 `use_function` 加载 `read_object_text` 和 `update_text_object`。
- 局部修复前不读取或校验 Blueprint/App Plan，除非目标行为依赖其中的业务契约。保留已有 App binding。除上述简单 UI 调整外，完成一组相关修复后，按 adapter 选定的 profile 对受影响对象执行 focused Preflight；上下文与验证边界遵循 [App Preflight](references/Preflight/app.md)，不编造证据绑定。不得为局部修复创建规划 Artifact、运行 capability 收口或完成无关 Milestone。
- `update_text_object` 匹配不到 `oldContent` 时，本批次没有保存：读回同一对象，基于当前源码重做补丁。运行时复验失败后，对照修改后的源码和新报错再决定下一次补丁；重复相同替换、重读文档或叠加猜测性兜底不算恢复策略。
- 保存后读回受影响源码，并在相同路由和 viewport 核对原始行为。预览 document 变化后使用全新诊断基线，丢弃旧 cursor。缺少运行时或视觉证据时明确说明，不得仅凭 Preflight 宣称完成。

对于需要改变业务定义或交付设计的需求，读取相关规划和资源，再进入需求对齐。影响未明或必要读取失败时保持只读。详见 `references/Scenario/app-building/requirement-review.md`。

其他任务路由：

| 任务                                                                                 | 入口                                                                      |
| ------------------------------------------------------------------------------------ | ------------------------------------------------------------------------- |
| 新建或大范围改造公开内容站、营销站或商城前台                                         | `references/Scenario/site-building/index.md`                              |
| 已授权的 ToB App / 后台业务规划                                                      | `references/Scenario/app-building/index.md`                               |
| 客户端渲染应用，或明确要求 Vue 3 CDN、无构建 Vue、Page/Style/Script 浏览器直运行应用 | `references/Frontend/cdn-vue/index.md`（内部实现 reference）              |
| 单个 Page、View、API、KScript 或明确故障                                             | 对应 Frontend / Backend / K-Script / Debug reference                      |
| Sandbox、测试站或隔离环境中的站点实施与验证                                          | 先调用 `load_skill("sandbox")`，再按具体对象读取本 Skill 的对应 reference |
| 仅讨论方案                                                                           | 保持 discussion，不写入资源                                               |

用户明确描述 Vue 3 CDN、无构建 Vue 或“Vue 3 + CDN/import map + Kooboo Page/Style/Script”时，必须先读取 `references/Frontend/cdn-vue/index.md`，再按其导航渐进加载子文档。它是客户端渲染的内部技术 reference，不是用户需要了解或确认的产品名称；App Building 必须先满足所选实施路径的范围校验。

用户明确要求在 Sandbox、测试站或隔离环境中实施或验证站点变更时，必须加载独立的 `sandbox` Skill。`sandbox` Skill 负责 Sandbox 的创建、复制、Context 切换、验证和清理边界；具体 Page、View、KData、KScript、API 等实现仍按本 Skill 的对象 reference 执行。Sandbox 授权不等于正式站写入授权，不能省略目标站点确认，也不能把 Sandbox 验证结果描述为正式站已上线。

一旦当前对话已经按 ToB App 生成或读取 Blueprint，后续的“继续”“下一步”“开始实施”等表达仍属于同一个 App Building 上下文，不得重新路由成公开 Site。用户明确要求修复既有对象时，按上文局部修复路径处理，包括属于该 App 的对象；独立公开 Site 任务也可重新路由。

## 2. ToB App Building 边界

进入 App 规划或业务实施前，先读取 [App Building 入口](references/Scenario/app-building/index.md)，按其发现规则确认现有规划与资源，再根据用户明确的推进方式选择路径。不能仅凭产品领域、系统类型、模块数量或熟悉的案例推断必须创建规划。新会话不意味着重新创建规划；规划读取失败不等于不存在。既有对象修复仍优先走上文局部路径。

- 当 Blueprint 和 App Plan 均确认不存在，且用户选择 P1 快速模式或明确要求直接交付时，使用轻量实施；已有规划按当前已保存状态续作。
- 规划编写不授权资源实施。只更新规划的请求完成后停止；实施必须已有用户授权，并通过所选范围的实施校验。
- App 与 Site Building 互斥，不能借用 Site Building 绕过规划或实施门槛。
- 已有 App 的业务定义或交付设计变化前读取 [需求对齐](references/Scenario/app-building/requirement-review.md)；新 App 首次创建不调用需求对齐，沿用 `propose_blueprint` → 用户审核 → `save_blueprint` 流程；Blueprint 语义对象的创建与更新按对象使用专用保存工具，遵循 [Blueprint 保存协议](references/Scenario/app-building/blueprint.md)；普通局部 UI 修复不因此回到规划。

按当前阶段加载：Blueprint 写入前读 [Blueprint](references/Scenario/app-building/blueprint.md)，App Plan 写入前读 [App Plan](references/Scenario/app-building/app-plan.md)，带规划实施前读 [客户端交付](references/Frontend/cdn-vue/delivery.md)；无规划实施读 [轻量实施](references/Scenario/app-building/lightweight.md)。App Plan 使用服务端强类型 Function 保存，执行状态独立；这些专题分别定义对象协议、失败恢复和交付要求，不提前加载无关阶段。

客户端 App 写资源前读取 [客户端实现入口](references/Frontend/cdn-vue/index.md)；选择或安装设计系统前读取 [设计系统](references/Frontend/app-design-systems.md)。沿入口加载布局与交付专题，安装成功不能代替业务交付。

## 3. Site 与局部编码路由

Site 全流程遵循 `references/Scenario/site-building/index.md`，不要因仓库存在 App Blueprint 而把公开页面的局部修改强制升级为 App 建设。

新局部实施按对象读取；既有对象修复优先走上文路径，只读取尚未明确的契约：

| 类型                                  | 必读文档                                                                                                                                                                         |
| ------------------------------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| 客户端渲染 Vue Page                   | `references/Frontend/cdn-vue/index.md` + `references/Frontend/code-style.md`；发生冲突时以该专项 reference 对完整 HTML、依赖和模块结构的规则为准                                 |
| Page / Layout / View / Style / Script | `references/Frontend/code-style.md` + 对应 Frontend reference                                                                                                                    |
| `k.*` KScript                         | `references/K-Script/index.md` + 对应模块                                                                                                                                        |
| Backend API / CodeBlock               | `references/Backend/index.md`                                                                                                                                                    |
| Function 参数不确定                   | `references/function-guide.md`                                                                                                                                                   |
| 多语言                                | `references/Scenario/multilingual-site-guide.md`                                                                                                                                 |
| Commerce 顾客流程                     | `references/K-Script/commerce/customer.md` + `references/Scenario/commerce-customer-login-flow.md`                                                                               |
| Commerce 顾客地址 / Checkout 收货地址 | `references/Scenario/commerce-customer-address-flow.md` + `references/K-Script/commerce/address.md` + `references/K-Script/commerce/types/customer.md`                           |
| Commerce 商城动态化                   | `references/Scenario/site-building/commerce-dynamicization.md` + 对应 Commerce / k-data reference                                                                                |
| Commerce 结算支付                     | `references/Scenario/commerce-checkout-payment-flow.md` + `references/Scenario/commerce-customer-address-flow.md` + `references/K-Script/payment/index.md` + 对应 provider guide |
| 故障 / 报错                           | 当前对象 reference + `references/Preflight/index.md`；先建立复现基线、收集最小证据并执行聚焦 Preflight。用户显式选择 `/debug` 时，由独立 Debug skill 负责完整诊断闭环            |
| 交付前检查                            | `references/Preflight/index.md`                                                                                                                                                  |

若局部请求改变已有 Blueprint 中的业务定义，先更新受影响规划；未建立规划的 App 遵循轻量范围与 Memory 契约，不仅为本次变更而创建 Blueprint。

## 4. 写入前检查

每次工具执行前做最小必要的内部复核。写入、删除和代码执行必须明确以下各项；读取操作只需确认能解决当前需要且不越过阶段边界。复用未变化的上下文，不重复读取所有 reference，也不新增规划产物：

1. 用户已授权本轮实施。
2. 适用的 Skill、任务路径（Site、App 规划/实施或局部对象）及当前阶段。
3. 已读取哪个任务入口和对象 reference；既有对象修复则说明支持本次补丁的当前源码和证据。
4. 本次操作适用的必须项、禁止项、前置条件和契约中明确的例外。
5. 本次工具调用是否越过阶段边界或扩大已有授权范围。
6. 当前证据是否足以支持本次操作、阶段推进或完成声明，还有哪些内容需要检查。

必要答案缺失时，暂停依赖它的操作并补足具体证据，不猜测、不降低标准，也不把这项内部复核变成需要用户补充的清单。

## 5. 通用编码红线

1. 渐进读取：只加载当前子任务需要的 reference，并复用上下文已有内容。
2. `k.*` API 必须先查真实文档或源码，不编造方法名和参数。
3. 创建或修改对象后，能读回的对象必须读回关键内容验证。
4. 影响数据结构、权限、业务流程或跨资源关系时，先形成结构化约定。
5. 出现 4xx / 5xx、预览失败、Network failure 或明确报错时，先明确原路由、设备、输入、操作、预期和实际结果，读取受影响对象的当前源码。HTTP 500、SSR/模板故障或 Preview 不可用时首轮运行 focused Preflight；已定位的客户端运行时错误先检查对应 Script，在修复批次完成后运行 focused Preflight；若用户显式选择 `/debug`，再由该 skill 编排 Preview、Network 与已知错误 playbook。
6. 除非用户明确要求，页面内容和控件默认不使用 Emoji。
7. 除上述简单 UI 调整外，资源实施完成后读取 `references/Preflight/index.md`，再通过 `run_skill_script` 执行 Preflight；建站对话不读取 `scripts/preflight-scan.js` 或 `references/Preflight/rules/*.yaml` 实现源码。
8. `run_kscript` 通过 `return` 或末尾表达式返回结果，不用 `k.response.write/json` 代替返回值。
9. 创建或修改公开 UI 前读取 [UI 诊断](references/Preflight/index.md#公开-ui-诊断)，按其检查点收集只读证据；浏览器操作仍遵循上文授权边界。

## 6. 导航

- [Site Building 入口](references/Scenario/site-building/index.md)
- [App Building](references/Scenario/app-building/index.md)
- [场景索引](references/Scenario/index.md)
- [Frontend 索引](references/Frontend/index.md)
- [Frontend Code Style](references/Frontend/code-style.md)
- [客户端渲染内部实现规范](references/Frontend/cdn-vue/index.md)
- [App 内置设计系统](references/Frontend/app-design-systems.md)
- [Backend](references/Backend/index.md)
- [App Base 服务](references/Backend/app-base-services.md)：以已安装的用户与通知 API 为实际参照；真实数据、权限敏感 API 或交付前必须移除全部服务端 mock 分支并验证匿名 401，不能只修改前端配置。
- [KScript](references/K-Script/index.md)
- [Function Guide](references/function-guide.md)
- [Function References](references/Functions/index.md)
- [Preflight](references/Preflight/index.md)
