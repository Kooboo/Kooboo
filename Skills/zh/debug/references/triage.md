# Kooboo Debug SOP

Kooboo 站点开发（Layout / Page / View、模板 SSR、Script、KScript、API / Service）场景下的统一故障分诊与修复入口。

目标不是尽快给出一个“可能的原因”，而是完成一个可审查的闭环：

> 明确症状 → 最小复现 → 采集证据 → 确认根因 → 最小修复 → 同路径复验

当故障发生在 AI Chat 右侧站点预览中，同时读取 [Preview Runtime Debugging](preview-runtime.md)。白屏、HTTP 500、模板 SSR 或 Page / Layout / View 未渲染时，还要读取 [Kooboo 常见错误快速通道](common-errors.md)。

本 reference 负责诊断编排。涉及 Kooboo 对象契约或资源写入时，先加载 `kooboo-coding`，再通过 `read_skill_resource("kooboo-coding", "...")` 读取下文链接的技术 reference。

---

## 1. 适用范围与触发

满足任一条件，即视为故障上下文；在给出根因结论或修改资源前，必须遵循本文：

- 构建或站点预览失败；
- HTTP 4xx / 5xx、网络失败、超时或请求长期 pending；
- 浏览器控制台、服务端日志中出现明确错误或堆栈；
- 页面白屏、交互无响应、数据不显示、保存不生效；
- 行为与已确认需求明显不符，且用户意图是修复而不是改变需求；
- 用户使用“坏了 / 不对 / 不工作 / 报错 / 排查并修复”等明确故障语义；
- 用户为当前请求显式选择了 `/debug` skill。

以下不强制进入完整 Debug 闭环：

- 纯文案变更或局部视觉问题（包括间距、对齐、大小、颜色、排列错误）；按 `kooboo-coding` 的局部修复路径检查并复验目标元素，只有证据需要时才扩展到运行时排障；
- 没有故障语义的正常功能开发；
- 用户只要求解释某段现有代码。

边界不清时，先做只读取证，不先修改。

---

## 2. 授权边界

`/debug` 用于改变当前请求的工作重点，不自动扩大写入授权。

| 用户表达 | 允许行为 |
|---|---|
| “分析原因”“帮我看看为什么” | 只读取证、定位并说明，不修改资源 |
| “排查并修复”“解决这个问题” | 确认根因后允许实施最小修复，并必须复验 |
| 只选择 `/debug`，没有提交具体问题 | 等待用户描述问题，不主动修改 |

涉及登录提交、创建真实订单、支付、删除、发送邮件 / 短信、修改用户或其他有业务副作用的操作时，即使用户已授权修复，也要在执行该副作用前获得明确确认或使用明确的测试数据。

---

## 3. 强制红线

- 禁止无复现、无报错信息就大面积重写。
- 禁止把症状直接当根因，例如“列表为空”不等于“查询一定错误”。
- 禁止连续叠加多个猜测式补丁；复验失败后回到证据阶段。
- 禁止未通过 `read_object_text` 读取当前对象内容就覆盖 Page / View / Layout / Script / Style / API / CodeBlock。`oldContent` 未找到时必须读回该对象再重做补丁，不能继续猜原文或行号。复验失败后对照当前源码和新报错，不依赖此前生成的版本。
- 未核实映射前，不能将生成代码或 bundle 的行号当作站点对象源码位置。位置已核实时，先读取覆盖失败表达式及其所属结构的窗口，证据不足再扩大。原始补丁文本与绝对行号规则见[对象读取与更新](../../kooboo-coding/references/Functions/read-update.md)。局部读取不能证明整个对象正确。
- 禁止为了让 UI 看起来成功而直接制造数据库、订单、购物车、用户或登录状态。
- 禁止臆造 `k.*` API；新增或保留的调用必须在对应 KScript reference 中核实。
- 禁止臆造 `k.req.services` / `k.request.services`；API 调用 Service 只能通过真实的 `import` / `export` 契约。
- 禁止持续把完整 Log、Network 记录推入会话；只读取当前假设需要的最小范围。
- 禁止在完成后遗留临时 `AI_DEBUG`、`console.*` 或额外 `k.logger` instrumentation。
- 禁止未走同一路径复验就宣称“已修复”。

---

## 4. 默认排障闭环

### 4.1 建立问题基线

先确定：

1. 目标 URL / 路由；
2. 设备或 viewport；
3. 用户执行的操作和输入；
4. 预期结果；
5. 实际结果；
6. 是否稳定复现。

已有信息足够时直接推进，不逐项向用户重复提问。只有缺失信息会实质改变复现路径或带来风险时才询问。

### 4.2 最小复现

- 将复现路径压缩到能稳定触发症状的最少步骤。
- Preview 可用时优先由 Browser Control 自动执行安全步骤。
- 白屏或首屏加载失败要重新加载一次，以便捕获启动期 Log 和 Network。
- 没有明确客户端报错的白屏、HTTP 500 或模板 SSR 失败必须在首轮证据采集中运行 Preflight，并读取当前 Page、绑定 Layout 和挂载 View；不要等待 Preview Agent 恢复后才做静态检查。
- Preflight 返回 `knownErrors` 时，先按其中的 `playbook` 执行确定性快速路径，再考虑通用假设。
- 偶发问题要保留相同路由、设备、输入和操作顺序，不在每次尝试中同时改变多个变量。

### 4.3 证据采集

按症状选择证据，不机械读取所有工具：

| 症状 | 优先证据 |
|---|---|
| 白屏、JS 崩溃、组件未渲染 | Preview Log issues、堆栈、相关 Script / View |
| 点击无响应、弹窗不出现、状态未更新 | Browser Control 重放、DOM changes、Preview Log |
| 数据为空、保存失败、接口报错 | Preview Network issues、服务端 Error Log、API / Service 源码 |
| 请求慢、一直 loading、重复请求 | Preview Network all、pending、duration、initiator stack |
| 图片 / CSS / JS / 字体失败 | Network resource 类型、Log resource error |
| 仅移动端或特定尺寸失败 | 设置相同 device 后重放 |
| KScript / CodeBlock 报错 | 服务端错误原文、对应对象源码、KScript reference |

Preview 的具体工具顺序、cursor 和 document 切换语义见 [Preview Runtime Debugging](preview-runtime.md)。

### 4.4 根因确认

修改前至少应能形成以下链条：

```text
现象
→ 可重复的触发步骤
→ 直接运行证据
→ 能解释该证据的真实源码或配置
→ 一个可被修复和复验的根因
```

直接证据至少包括一项：

- 明确异常堆栈或服务端错误；
- 失败 / 超时 / 重复的网络请求；
- DOM 状态与预期不符；
- 请求、日志和源码之间可对应的调用路径；
- 真实响应结构与读取字段不一致。

“可能是缓存”“可能是接口”“可能是框架问题”不属于已确认根因。

### 4.5 最小修复

- 先读取当前对象源码，复用上下文已有 reference，只有具体契约未明确时才读取其他文档。报错指向客户端组件时直接检查其 Script、import、setup 返回绑定、模板和相关数据结构，不再开启一轮框架文档阅读。
- 一次只修改一个已被证据支持的根因或一类同源问题。
- 不顺带重构无关资源。
- 创建 / 更新对象的函数参数不确定时先读 [Function Guide](../../kooboo-coding/references/function-guide.md)。
- 修改 KScript 时二次核对每个新增或修改的 `k.*` 调用。
- 若现有证据仍不足，可添加最小临时 instrumentation：
  - 浏览器侧使用可搜索的 `AI_DEBUG` 前缀；
  - 服务端 KScript 使用 `k.logger`，不能使用 `console.*`；
  - 取得证据后立即移除。

### 4.6 同路径复验

修复后重复修复前的：

- 同一个 URL；
- 同一个设备；
- 同样的输入；
- 同样的操作顺序；
- 同样的等待条件。

完成标准必须同时满足：

1. 用户期望行为正确出现；
2. 原故障不再复现；
3. 相关 Log / Network / 服务端错误消失；
4. 没有引入明显的新回归；
5. 临时 instrumentation 已清理。

如果无法完成其中一项，必须说明缺失的验证证据。

---

## 5. Kooboo 故障地图

### 5.1 预览空白、布局乱、模板绑定不生效

优先检查：

- 先运行 Preflight；命中 `knownErrors` 时读取 [Kooboo 常见错误快速通道](common-errors.md)，其中 `KOOB-CDATA-WRAPPER` 属于立即处理的高置信根因；
- `k-content`、`k-if`、`k-for`、`k-attribute`、`<k-data>` 是否处于 `env="server"` 继承链；
- `<script env="server">` 中 `import`、`type="module"`、`export` 是否配套；
- HTML 标签、属性引号和嵌套是否完整；
- Layout / Page / View 占位符、引用和命名是否一致；
- Network 是否存在 Script / Style / Image 资源失败。

继续读取：

- [Template Binding Syntax](../../kooboo-coding/references/Frontend/template-binding-syntax.md)
- [Layout](../../kooboo-coding/references/Frontend/layout.md)
- [Page](../../kooboo-coding/references/Frontend/page.md)
- [View](../../kooboo-coding/references/Frontend/view.md)

### 5.2 KScript / CodeBlock 异常

优先检查：

- 堆栈是否指向具体对象和行号；
- 服务端是否误用 `console.*`；
- `k.request` / `k.response` 的读取与返回契约；
- `k.*` 方法是否真实存在；
- 动态化是否本应使用 `<k-data>` + `<query>`；
- `undefined` / `is not a function` 是否源于错误 API 路径。

新增或修改 `k.*` 前：

1. 从 [KScript Index](../../kooboo-coding/references/K-Script/index.md) 定位模块；
2. 在模块子文档中查找同一调用路径；
3. 无命中则视为当前文档能力未定义，不发明近邻名称试错。

### 5.3 API / Service / Data 问题

优先检查：

- Network 中的 URL、HTTP 方法、状态码和耗时；
- `{action}`、路由和调用方是否一致；
- 请求触发位置与实际 API 是否对应；
- API、Service、Data 层职责是否混写；
- 返回结构与前端读取字段是否一致；
- 权限、数据源或环境配置是否被误判为代码错误。

继续读取：

- [Backend Guide](../../kooboo-coding/references/Backend/index.md)
- [API Layer Guide](../../kooboo-coding/references/Backend/api-layer-guide.md)
- [Service Layer Guide](../../kooboo-coding/references/Backend/service-layer-guide.md)
- [Data Layer Guide](../../kooboo-coding/references/Backend/data-layer-guide.md)
- [Function Guide](../../kooboo-coding/references/function-guide.md)

---

## 6. 两种 Automation 必须区分

### Preview Browser Control

`browser_control`、`observe_iframe`、`find_iframe_elements`、`act_on_iframe_element` 等用于在右侧 Preview 中复现和复验用户操作，是 Debug 主链路的一部分。

### Site Automation

`list_automation` / `execute_automation` 用于列出和执行站点已有 Automation 任务。只有故障本身属于某个 Automation 任务时才使用；`execute_automation` 需要审批，不能代替普通页面操作或浏览器测试。

---

## 7. 状态与交付措辞

必须区分：

- **已复现**：已经稳定观察到症状；
- **已确认根因**：证据与源码形成闭环；
- **已完成修改**：资源已经更新，但还没有完成同路径复验；
- **已修复并验证**：同路径复验通过，相关错误未再出现；
- **无法复验**：缺少权限、数据、外部服务、用户操作或环境条件；
- **阻塞**：当前能力无法继续取得必要证据。

最终结果至少说明：根因、修改对象、复验路径、复验结果和剩余风险。
