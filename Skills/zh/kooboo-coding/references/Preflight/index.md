# Preflight Core 与 Profile 路由

Preflight 是 Kooboo 编码任务的强制交付闸门。共享 scanner core 统一负责对象读取、focused/capability 范围、规则执行、结构回执以及 Completion Ledger 集成；交付语义由 adapter 权威选择的 profile 决定：

- [Site Preflight](site.md)：Site Brief/Page Plan 交付、公开站点阶段、Commerce、Payment、多语言行为和收尾检查。
- [App Preflight](app.md)：带规划 App 的 Plan 绑定，以及轻量 App 共用的结构 profile，并附对应的结构和交付证据。

轻量 App 由 adapter 选择公共 `site` 结构 profile。这不代表转入 Site Building，也不允许绕过已有规划。

模型不能自行选择最终 profile 或编造证据绑定。adapter 默认使用 site 结构 profile；服务端当前 Plan 读取成功后选择 App profile 及会话内证据上下文。带规划与无规划上下文、失效和恢复见 [App Preflight](app.md)。设计读取不授予实施权限。

执行时读取本文和当前 profile 契约，再通过 `run_skill_script` 运行 `scripts/preflight-scan.js`。`scripts/preflight-scan.js` 与 `references/Preflight/rules/*.yaml` 是执行实现，不通过 `read_skill_resource` 加载进对话上下文；服务端只在当前可信执行脚本确认为该 scanner 时允许它自行加载适用规则，其他脚本和直接工具调用仍返回 execution-only 提示。

## 简单 UI 调整例外

`kooboo-coding` 已识别为与业务无关的简单 UI 调整，不要求执行本交付扫描；直接读回修改内容并在原路由、viewport 复验目标元素的外观及已有交互。不得为了样式修复恢复 App binding、回读 Blueprint/App Plan 或触发 capability 收口。此例外优先于下文“任何 repair pass”的通用要求。已有 Ledger 的待验对象仍保留，整体交付闸门不等于本次局部 UI 结果，不能因此声称 App 已完成。

## 什么时候执行

以下状态中，只要创建或修改了代码/站点对象，在最终回复用户之前都必须执行 Preflight：

- S4 静态站点实现
- S5 静态迭代
- S9 动态实现
- S10 动态迭代
- 任何修改代码或站点对象的 bug fix / repair pass

如果存在任何 `BLOCKER`，不要宣称任务已经完成。必须先修复。若当前轮次无法修复，必须明确报告阻塞项、影响对象和无法修复的原因。

## 调用节拍：聚焦扫描 vs 能力级扫描

默认模式是 `capability`，保证未接入权威 dirty scope 的调用方不会静默漏扫。需要缩小迭代成本时，调用方必须显式选择 `focused`：

- 每轮（每个用户反馈或写入批次）只传 **dirty 对象 + 直接消费者**，并用 `dirty` 参数标记本轮写入的对象；不要每轮把整个能力切片的全部对象传入。
- 把一个完整 Slice 或一组相关修复视为一个写入批次：先完成相关对象的写入和读回，再执行一次 focused Preflight；不要在同一批次内每创建一个 Page、Style、Script、API 或 CodeBlock 就单独扫描一次。
- `mode: "focused"`（显式）：扫描 dirty 对象与直接消费者。只有可信 adapter 显式传入 `useCache: true` 和上下文匹配的 `cache` 时，非跨对象、非 Commerce artifact 才可复用；否则仍读取并扫描。
- `mode: "capability"`：关闭缓存复用，对当前交付范围做全量重扫。AI Chat adapter 会把 Completion Ledger 中本会话所有成功持久化的站点对象权威合并进 `artifacts`，模型只能补充未写入但属于同一能力的直接消费者或既有对象，不能缩减已跟踪范围。
- focused 扫描不用于宣称阶段完成：`completionGate.focused=true`、`claimAllowed=false`，并附带一条 `missingEvidence`，要求先运行一次 capability 扫描。

### dirty 集合

写入后由 AI Chat Completion Ledger 记录标准 `site_write` 回执，并由 adapter 向 Preflight 权威注入 `dirty`；模型不提交或覆盖该集合。adapter 还会自动把 dirty 对象补入 focused `artifacts`，扫描器保留 `dirty_scope_incomplete` 作为第二层校验。聚焦扫描的模型调用示例：

```json
{
  "skillName": "kooboo-coding",
  "scriptName": "scripts/preflight-scan.js",
  "arguments": {
    "taskType": "dynamic-page",
    "deliveryStage": "commerce-runtime",
    "layers": ["frontend", "kscript", "integration"],
    "mode": "focused",
    "artifacts": [
      { "name": "home", "objectType": "Page", "layoutName": "main" }
    ]
  }
}
```

实际执行时 adapter 会把 `product_list` dirty View 合并回 `artifacts`。输出中的 `directConsumers` 会列出本批对象中引用 dirty 对象的一方；下一轮聚焦扫描应把引用方一起传入。adapter 只能权威补齐已写入对象，不能凭空推断所有未修改的消费者，因此模型仍需把当前能力的既有直接消费者放进调用；focused 永远不能作为终检。

### 显式缓存安全边界

- 缓存默认关闭，扫描器不会通过 `k.file` 读取或写入站点资源；缓存应由可信 adapter 在模型上下文之外管理。
- 只有 `mode: "focused"`、`useCache: true`、显式 `cache`、规则版本一致、扫描上下文一致、对象未 dirty、Size 双侧可比且相等时才可能复用。
- Page / Layout / View / Style 和任何 Commerce artifact 始终重新读取内容，因为资源引用、Layout/Page 装配、Products bundle 与运行证据判断需要真实源码。
- Size 只是低成本启发式，同长度改写仍可能漏失效；阶段收尾必须使用 `mode: "capability"` 全量扫描。
- 规则版本（扫描器 `RULE_VERSION`）变化时缓存整体失效。**修改任何规则 YAML、custom check 或扫描逻辑时，必须递增 `RULE_VERSION`**，否则旧缓存会复用旧规则的结果。

### 新增参数与输出

| 参数 | 默认 | 说明 |
| --- | --- | --- |
| `mode` | `capability` | `focused` / `capability` |
| `dirty` | `[]` | Adapter 注入的本轮写入对象列表（`nameOrId` + `objectType`）；模型不提交 |
| `cache` | 无 | 可信 adapter 显式注入的可选缓存对象或 JSON；扫描器不负责持久化 |
| `useCache` | `false` | 是否允许使用显式缓存；只有 focused 模式生效 |

输出新增：`scanSummary`（mode、ruleVersion、`cacheEnabled`、每对象状态与计数、`dirtyCount`/`artifactCount`；focused 且空 dirty 且 artifacts≥2 时带 `warnings: ["focused_without_dirty"]`）、`cachePersisted: false`、`directConsumers`（dirty 对象在本批中的引用方），以及仅在结构检查依赖不可用时出现的 `skippedChecks`。`structuralReceipt.complete` 表示这些结构检查是否全部执行。focused 未宣称完成时 `nextActions.beforeClaimCompletion` 提示先跑 capability。

完整 scanner 返回会持久化，供 Completion Ledger 校验 receipt、对象 hash 与 evidence requirement。发送后续模型请求时，历史压缩层只保留最新 Preflight 的可行动字段：结论与 finding、skipped checks、receipt 的 passed/complete/mode/ruleVersion/evidence requirements、聚合 gate、计数型 scan summary、next actions 和 direct consumers；重复的 `verifiedArtifacts` 与逐对象正常扫描明细不再重复进入模型上下文。更早的 Preflight 结果已被后续扫描取代，只保留 superseded 标记；历史中已经读取过的 scanner/rule 实现正文也替换为执行提示。该压缩不修改持久化原始证据。

## 已知错误路由

扫描结果可能包含顶层 `knownErrors`，并在对应 finding 中包含 `knownError`：

```json
{
  "knownErrors": [
    {
      "id": "KOOB-CDATA-WRAPPER",
      "priority": "immediate",
      "playbook": {
        "skillName": "debug",
        "resourceName": "references/common-errors.md",
        "anchor": "3-koob-cdata-wrapper"
      },
      "ruleIds": ["PF-UNIVERSAL-004"],
      "artifacts": ["Page:home"]
    }
  ]
}
```

这表示扫描器命中了可确定性处理的常见错误。用户显式选择 `/debug` 时，使用 `playbook.skillName` 和 `playbook.resourceName` 调用 `read_skill_resource`，再按 `playbook.anchor` 定位对应章节；处理该错误后再生成其他假设。`priority="immediate"` 表示不应继续等待 Preview 或重复 Browser Control。当前已接入的条目与成熟度见 Debug skill 的 [Kooboo 常见错误快速通道](../../../debug/references/common-errors.md)。

## 必须调用的脚本

最终回复前，使用当前 skill 调用扫描脚本：

```json
{
  "skillName": "kooboo-coding",
  "scriptName": "scripts/preflight-scan.js",
  "arguments": {
    "taskType": "site-page",
    "deliveryStage": "static-first",
    "layers": ["frontend", "design", "kooboo-template"],
    "mode": "capability",
    "artifacts": [
      {
        "name": "main",
        "objectType": "Layout"
      },
      {
        "name": "home",
        "objectType": "Page",
        "layoutName": "main"
      }
    ]
  }
}
```

`artifacts` 默认只需要传对象引用，避免把完整代码内容塞进函数参数消耗大量 token。扫描脚本会在内部调用 `read_object_text` 获取最新内容。

**能力识别与跨对象检查**：扫描器先读取对象内容和平台元数据，再按能力加载规则。App/Site 是场景约束；Page/Layout/View 装配、资源引用、表单和 KScript/API 是共享能力。k-data 与 Commerce+k-data 当前仅用于 Site。`layers` 和 `taskType` 是描述性信息，省略它们不能关闭已识别能力的检查。详见[能力与证据边界](capabilities.md)。

已保存 Page 的 `layoutName` 和真实 Route 从 `list_site_object` 读取；源码中的 `<layout>` 用于识别和交叉核对。绑定信息不可用时报告 `skippedChecks`，不静默跳过。插槽名必须非空且匹配，绑定 Layout 和引用 View 必须存在。重复 View 和空 data-block 属于装配建议：可复用 View 与客户端挂载点不能仅凭形状被判为失败。

资源检查解析相对 URL、Page 的 `<base href>`、CSS URL、静态及字符串形式的动态模块导入，以及扫描范围内 Page 的 import map。相对 URL 正确解析到真实 Route 时合法。缺少所属 Page/URL/import map、计算型动态 import 或必要清单时，报告未完成检查。外部资源和动态模板值需要运行证据；静态通过不能证明模块已成功加载。

`findings` 记录可判定的结构问题，`advisories` 记录需要结合用户意图、实际数据流或运行表现核实的建议。建议不改变结构 PASS，不代表其内容已验证通过。明确的私钥材料仍按 BLOCKER 处理，输出脱敏；仅凭变量名猜测的凭据作为建议。结构所需信息缺失时，`structuralReceipt.complete=false`，完成门禁保持关闭。

推荐字段：

| 字段 | 必填 | 说明 |
| --- | --- | --- |
| `name` | 是 | 对象名称，例如 `home`、`main.css`、`Services.product` |
| `nameOrId` | 否 | 如果要用 GUID 或不同于展示名的标识，可传此字段；优先级高于 `name` |
| `objectType` | 是 | `Page` / `Layout` / `View` / `Style` / `Script` / `Api` / `CodeBlock` |
| `content` | 否 | 仅用于尚未保存的临时代码片段或测试场景；常规 Kooboo 对象不要传 |
| `url` | 否 | Api 等路由对象的真实 Url。已保存对象通常会通过 `list_site_object` 自动补充；扫描未保存的命名 action API 时必须显式传入，以校验 `{action}` 路由契约 |
| `layoutName` | 否 | 已保存 Page 从平台读取；未保存内容可提供预期绑定。普通 App 与 Site 均执行适用的模板装配检查。 |
| `multilingual` | 否 | 是否启用平台多语言以 get_site_info 为准，模型字段不能覆盖平台配置。 |
| `commerceModel` | 否 | `catalog-contact` / `online-checkout`；可在顶层传入作为所有 artifact 默认值，也可逐 artifact 指定 |
| `commerceCapability` | 否 | 明确的 Commerce 能力上下文，例如 auth/checkout/payment/orders；普通同名 App 页面不会因此被自动归为商城。 |
| `commerceSurface` | 否 | 对应 Commerce 页面/接口职责的显式上下文。仅在实际 Commerce API/query 或显式 Commerce 上下文存在时识别；不以 login/orders/account 等名称单独启用。 |
| `deliveryStage` | Commerce 必填 | `static-first` / `commerce-runtime` / `payment-runtime`；决定哪些规则属于当前交付阶段 |

如果更新了已有对象，脚本会读取 Kooboo 中保存后的最新内容。若对象尚未保存但仍需扫描，可以显式传 `content`。

Commerce Preflight 必须显式传 `deliveryStage`：

- `static-first`：执行通用静态规则；Commerce 动态规则进入 `deferred`，不阻塞静态交付。
- `commerce-runtime`：激活商品、分类、PDP、登录、Header、Cart、Checkout 和订单归属规则；支付 provider 规则仍 deferred。
- `payment-runtime`：激活支付端点、provider callback / checkStatus 和真实订单支付状态规则。

扫描器先按实际能力筛选规则，再扫描适用规则，再按阶段拆分 active/deferred；`result` 只由 active findings 决定。`deferred` 只返回后续阶段和唯一规则 ID，不重复完整 finding。

Preflight Core 必须产出合法 `structuralReceipt`：结构规则是否无活动 `BLOCKER` 的 `passed`、结构检查是否全部执行的 `complete`、扫描 mode、规则版本、已验证对象及其内容 hash，以及带稳定 `id` 和 `runtime | business` 层级的 `evidenceRequirements[]`。`passed=true` 但 `complete=false` 表示结构检查因依赖能力不可用而未做完，不是完整 PASS。AI Chat 只接受 `kooboo-coding` 的 `scripts/preflight-scan.js` 产生的完整 receipt；字段缺失、对象 shape 非法、同名其他 skill 脚本或只有兼容 `completionGate` 的旧返回都会阻塞完成。

AI Chat Verification Ledger 聚合 `site_write`、`structuralReceipt` 和后续受控运行证据，Gate Evaluator 才决定最终 `claimAllowed`。完成某条 runtime requirement 的全部步骤后，在最后一个成功的 Preview/浏览器断言上提交该 receipt 返回的 `verificationRequirementId`；adapter 会确认它属于最新 `structuralReceipt`、来源是已注册浏览器工具且层级为 runtime，再记录 evidence。模型不能提交 `level`，也不能通过脚本返回同形 JSON 铸造证据。business producer 未接入前支付完成 gate 保持关闭。任一新写入会失效旧 runtime/business receipt。

Ledger 会保留本会话成功写入对象的 `trackedArtifacts`。focused PASS 只清除 dirty 标记，不缩小该范围；capability receipt 必须覆盖所有 tracked artifact 且内容 hash 一致，否则 adapter 聚合 gate 仍为 false。保存 SiteMilestone 清单只是进度记录：它不会清空或重置 tracked scope、Preflight 回执及 runtime/business 证据。Scanner 返回的兼容 `completionGate` 只用于人类可读提示，不能代替 `structuralReceipt`。

`completionGate` 固定保留 `claimAllowed`、`maxCompletionLevel`、`verifiedArtifacts[]`；`missingEvidence`、`unresolvedErrors` 仅在非空时返回。消费者必须把缺失数组视为 `[]`。

如果当前对象属于多语言站点，建议在 artifact 上额外传 `multilingual: true`。这样扫描器会额外拦截：

- 手写 `href="/products?lang=en"` / `href="/products?culture=en"`
- 手写 `href="/en/products"` 这类语言前缀链接
- 直接 `location.href = "/cart"` 或 `k.response.redirect("/checkout")` 这类未通过 `k.site.multilingual.getUrl(...)` 生成的站内跳转

## taskType 取值

选择最接近当前任务的类型：

| Task Type | 适用场景 |
| --- | --- |
| `site-page` | Layout、Page、View、静态区块、落地页 |
| `style` | 只修改 CSS / Tailwind |
| `script` | 前端 JavaScript |
| `api` | API 对象 |
| `codeblock` | Service / Utils CodeBlock |
| `dynamic-page` | 同时涉及前端、KScript、k-data、API 或数据源的页面 |
| `form` | 表单、提交、登录/注册、联系流程 |
| `data` | 数据源、content、sqlite、commerce 或结构设计 |
| `fix` | bug 修复或返工 |
| `all` | 范围较大或混合任务 |

## layers 取值

把本轮实际涉及的层都传进去：

- `universal`
- `frontend`
- `design`
- `kooboo-template`
- `kscript`
- `backend`
- `data`
- `forms`
- `security`
- `accessibility`
- `performance`
- `integration`

扫描器会始终包含 `universal` 规则。其余 layer 需要按本轮改动主动传入，这样规则选择才足够准确。

## 结果策略

扫描器会按 rule ID 聚合问题；同一规则在多个对象的命中放入 `occurrences[]`，不重复 title、fix、pattern 和 tags。严重程度语义：

- `BLOCKER`：交付阻塞，必须修复后才能完成。
- `HIGH`：高风险，能修就修；若保留，需要在 residual risk 中说明。
- `MEDIUM`：质量问题，属于当前范围时应修复。
- `LOW`：优化建议。

最终回复规则：

- 如果 `result` 是 `FAIL`：先修复 blockers，再重新运行 Preflight。
- 如果 `result` 是 `PASS_WITH_WARNINGS`：结构检查可交付，但要说明仍保留的高风险项，并继续服从 `completionGate`。
- 如果 `result` 是 `PASS`：只表示当前启用的结构规则通过；只有 `completionGate.claimAllowed=true` 才能宣称阶段完成。

## 建站任务收尾检查

本节适用于 Site Building 任务。App 完成流程遵循 [App Preflight](app.md)：带规划 App 绑定 Plan；轻量 App 复用 Site 结构规则，不采用 Site Building 收尾阶段。

建站任务通过 Preflight 后，在最终回复用户前还必须执行一次非脚本型收尾检查：

1. 若当前会话 todos 仍有未完成项，先调用 `update_todos` 更新为准确状态。
2. 当用户可感知的阶段已完成时，用 `update_site_milestones` 记录已完成的清单条目。清单只是进度记录：验证仍待完成时，不要把条目标记为 `completed=true`。
3. 若存在自然下一步，直接调用默认函数 `show_next_recommended_action`；优先展示用户要求接下来交付的未完成清单条目。只有多个能力可独立开始且都有 Site Brief/Page Plan 证据时才展示 2-3 项。
4. 若当前只是修复 blocker、局部返工或用户没有自然下一步，不强行推荐。

非 FAIL 结果会返回最小 `nextActions`：始终包含 `always: [update_todos]` 与 `whenNaturalNextStep: [show_next_recommended_action]`；仅当 `completionGate.claimAllowed=true` 时才包含 `whenStageComplete: [update_site_milestones]`。它不是 finding，不改变 `PASS` / `PASS_WITH_WARNINGS` / `FAIL`。

`show_next_recommended_action` 不是 skill script。错误示例，禁止复制：

```text
错误：run_skill_script({
  "skillName": "kooboo-coding",
  "scriptName": "show_next_recommended_action",
  "arguments": { "actions": [] }
})
```

应直接调用默认函数 `show_next_recommended_action`，参数为 `{ "actions": [...] }`。

## 规则来源

规则位于 `references/Preflight/rules/`，按能力而非对象文件名分组：

- `common.yaml`：完整性、链接、内容包裹与内容策略建议。
- `frontend.yaml`、`forms.yaml`：前端表现、可访问性和表单。
- `template.yaml`、`assembly.yaml`：共享模板装配与页面骨架建议。
- `k-data.yaml`：Site 的 k-data 语法与数据绑定。
- `kscript.yaml`、`multilingual.yaml`：平台 API 和已启用的平台多语言路由。
- `data-contract.yaml`、`security.yaml`：数据策略核实、秘密信息及代码安全。
- `commerce.yaml`、`commerce-template.yaml`、`payment.yaml`：Commerce、Commerce+k-data、支付渠道能力。
- `assets.yaml`：资源引用、图片来源建议、import map。

每条规则显式声明 `profiles`、`capability`，组合能力声明 `requires`。扫描器统一筛选普通规则和跨对象规则；`advisory: true` 与明确的结构问题分开输出。`requiredStage` 只用于相应交付阶段，已经出现的危险支付状态写入不被静态阶段隐藏。新增规则必须有正例、反例与明确的适用边界；缺少信息应报告未完成或建议，不伪造确定性结论。

## 公开 UI 诊断

遵循根 Skill 的浏览器授权边界。此处仅定义已有只读 diagnostics 的检查点与证据含义，不授予浏览器操作权限。

创建或修改公开 UI 时，静态源码与 Preflight 不能替代浏览器最终样式。第一个可浏览页面形成后必须读取一次 `read_preview_diagnostics(mode="issues")`；最终 capability Preflight 返回 `site.ui-runtime` 时，必须针对最终渲染页面带该 `verificationRequirementId` 再读取一次。`uiAudit` 会检查 CSS 丢失、严重溢出，并在设计系统或 CSS 变量提供颜色基线时检查大面积背景偏移和品牌配色是否退化为黑白；`colorAudit` 会返回基线来源、实际主背景、偏移比例与品牌色命中数。`passed=true` 仍只表示这些有限规则未发现阻断错误，不能证明用户要求的具体视觉、布局、间距或样式修复已经正确。针对用户报告的 UI 问题，必须单独核对目标元素和原始预期，禁止仅凭 `uiAudit.passed` 声称已修复。`uiAudit.passed=false`、UI audit error 或 Stylesheet/Link 加载失败时先修复，不继续扩展页面，也不声称完成。
