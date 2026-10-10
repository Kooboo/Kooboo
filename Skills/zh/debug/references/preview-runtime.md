# Preview Runtime Debugging

AI Chat 右侧 Site Preview 的运行时排障与复验流程。本文件是 [Kooboo Debug SOP](triage.md) 的 Preview 专项 reference，只负责 Browser Control、Runtime Log 和 Network 的组合使用。

---

## 1. 能力与边界

### Browser Control

用于打开页面、设置设备、观察可交互语义树、查找元素、执行允许的交互并等待结果：

- `browser_control`
- `navigate_iframe`
- `set_iframe_device`
- `observe_iframe`
- `observe_iframe_changes`
- `find_iframe_elements`
- `inspect_iframe_element`
- `act_on_iframe_element`
- `wait_for_iframe`
- `set_iframe_files`

它用于复现和验证 UI，不授权通过后台写入制造成功结果。

### Runtime Log

`read_preview_diagnostics` 可读取经过限制和脱敏的：

- 基于最终 DOM 与 `getComputedStyle` 的自动 UI audit，包括 stylesheet 未应用、浏览器默认样式、设计背景/品牌配色基线明显漂移、横向溢出、文本裁切和严重 sibling overlap；颜色基线存在时可从 `uiAudit.colorAudit` 读取主背景采样和品牌色命中情况；
- `console.log / info / debug / warn / error`；
- `window.error`；
- `unhandledrejection`；
- Script / Style / Image 等资源加载错误。

默认从 `mode="issues"` 开始；只有错误和警告不足以解释问题时才使用 `mode="debug"`。
读取结果中的 `uiAudit.passed=false` 时，先处理 `source="ui-audit"` finding 和 Style/Link 资源错误；不能用“没有 console error”覆盖 UI audit 失败。

### Network

`read_preview_network` 可读取：

- fetch / XHR；
- Script、Stylesheet、Image、Font、Media 等资源请求；
- pending、success、HTTP error、network error、timeout、aborted、opaque；
- URL、方法、状态码、耗时、重定向、内容类型、响应大小和 fetch / XHR initiator stack。

安全边界：不返回请求体、响应体、Cookie、Authorization Header 和敏感 URL 参数。若根因依赖响应 JSON 内容，继续读取服务端日志、接口源码或其他经过授权和脱敏的真实数据，不得假装已看到响应体。

---

## 2. 两条标准复现路线

### 2.1 首屏 / 白屏 / 启动失败

启动期错误同时需要运行态 early capture 和静态快速分诊，不得串行等待 iframe 恢复后才检查源码：

1. `navigate_iframe(reload=true)` 重新加载当前页面；
2. 同时对当前 Page、绑定 Layout 和挂载 View 运行 Preflight，并读取 [Kooboo 常见错误快速通道](common-errors.md)；
3. Preview Agent 在有限等待内恢复时，使用 `observe_iframe` 获取新 document 的页面状态；
4. `read_preview_diagnostics(mode="issues")`；
5. `read_preview_network(mode="issues")`；
6. 根据 Preflight `knownErrors`、堆栈、失败资源、失败 API 和 initiator 读取对应源码。

如果 reload 后仍返回 `frame_not_ready`、`agentReady=false`，或 Log / Network unavailable，立即停止 Browser Control 重试并继续 Preflight 与源码检查。命中确定性已知错误后，不得为了等待 iframe 而延迟修复判断。

不要在 reload 前保存 cursor 后继续使用；document 改变后 cursor 和 nodeId 都属于旧页面。

### 2.2 点击 / 填写 / 保存等交互故障

1. 打开目标 URL，必要时设置目标 device；
2. `observe_iframe` 建立 DOM baseline；
3. 分别读取一次 Log 和 Network，保存返回的 `nextCursor`；
4. 使用 `find_iframe_elements` 查找语义元素；
5. 必要时 `inspect_iframe_element` 确认状态；
6. 使用 `act_on_iframe_element` 重放用户操作；
7. 使用 `wait_for_iframe` 等待预期 UI、隐藏状态或 URL 变化；
8. 同 document 内调用 `observe_iframe_changes`；
9. 使用各自的 `afterCursor` 读取本次操作产生的新 Log 和 Network；
10. 将 DOM、Log、Network 和源码关联后确认根因。

---

## 3. Cursor、document 与 nodeId

### Log cursor

```json
{
  "documentId": "...",
  "seq": 12
}
```

### Network cursor

```json
{
  "documentId": "...",
  "revision": 18
}
```

必须遵守：

- Log 和 Network cursor 不能互换；
- reload、导航、设备切换导致 iframe document 改变后，重新读取完整 baseline；
- document 改变后丢弃旧 nodeId；
- `observe_iframe_changes` 只适合同一个 document 内的增量变化；
- 返回 `cursorReset` 或 full observation 时，以新 document 为准，不把它当成工具失败；
- 查询结果被截断时，先增加针对性的 `search`、`states`、`types` 或 `levels`，不优先扩大到无界读取。

---

## 4. Browser Control 使用规则

- 优先语义 role、accessible name、text、tag，不猜 CSS selector。
- `observe_iframe` 用于初始 baseline；已有明确目标时可直接 `find_iframe_elements`。
- action 后优先 `observe_iframe_changes`，不要重复读取整个页面。
- loading、弹窗、延迟文本和 URL 跳转使用 `wait_for_iframe`，不立即重复点击。
- action 返回 `navigated=true` 后丢弃旧 nodeId；新 Agent 未就绪不表示点击失败。
- stale nodeId 时重新 observe / find，不重复使用旧引用。
- hover 菜单使用 `hover` action，不用 click 猜测。
- responsive 故障使用 `set_iframe_device` 还原相同设备后再复现。
- 上传文件必须先通过 `ask_user_input(kind="upload", upload.target="iframe")` 获得用户批准的 `fileId`，再调用 `set_iframe_files`。

### 副作用边界

以下操作执行前必须确认或使用明确测试数据：

- 登录提交或账户注册；
- 创建订单、提交购物车、支付；
- 删除、发布、发送邮件 / 短信；
- 改变用户、权限、密码或真实业务状态。

不能通过后台 API、数据库写入或 `run_kscript` 绕过 UI，使浏览器步骤看起来成功。

---

## 5. Log 与 Network 的关联

| 观察 | 下一步 |
|---|---|
| JS stack 指向具体 Script 行 | 读取对应 Script / View，核对调用上下文 |
| XHR / fetch 返回 4xx | 核对 URL、方法、凭据模式、路由、参数来源和权限 |
| XHR / fetch 返回 5xx | 结合时间、URL 查询服务端 Error Log，再读 API / Service |
| 请求长期 pending | 查看 timeout、duration、initiator，确认是否重复触发或等待未结束 |
| 同一 URL 重复请求 | 使用 all 模式和 initiator stack 查找重复 effect / event / retry |
| Script / CSS / Image 失败 | 核对最终 URL、重定向、资源路径与 Page / Layout 引用 |
| DOM 无变化且无错误 | 检查事件是否绑定、元素是否 disabled、条件分支是否提前返回 |
| DOM 已变化但用户看不到 | 检查 visibility、层级、样式、viewport 和滚动位置 |

Network 没有 response body 时，不根据状态码臆造服务端返回内容。

---

## 6. 临时 Instrumentation

只有现有 DOM、Log、Network 和源码仍不足以区分假设时才加入。

### 浏览器侧

使用稳定且可搜索的前缀：

```js
console.debug("AI_DEBUG cart-submit", { cartId, itemCount });
```

随后通过：

```text
read_preview_diagnostics(mode="debug", search="AI_DEBUG")
```

只记录判断分支需要的最小字段，不记录密码、Token、Cookie、支付信息或完整用户数据。

### 服务端 KScript

使用 `k.logger`，不得使用 `console.*`。取得证据后删除临时日志，并重新执行同路径复验。

---

## 7. 修复后的标准复验

1. 保存修改；
2. reload 原 URL；
3. 恢复原 device；
4. `observe_iframe` 建立新 document baseline；
5. 重新查找元素，不复用旧 nodeId；
6. 重放相同输入和操作；
7. 等待相同结果条件；
8. 确认预期 DOM / URL / 文案 / 状态出现；
9. 读取新 document 的 Log issues；
10. 读取新 document 的 Network issues；
11. 确认临时 instrumentation 已移除。

“没有 console error”不等于通过；预期行为必须实际出现。

---

## 8. 最小案例：保存后一直 Loading

### 复现

1. 导航到表单页面并设置用户出现问题时的 device；
2. observe 页面，找到表单、保存按钮和 loading 元素；
3. 读取 Log / Network baseline cursor；
4. 填写与用户一致的最小输入；
5. 点击保存；
6. 等待 loading hidden 或成功文案 visible；
7. 超时后读取 DOM changes；
8. 增量读取 Log / Network。

### 定位

- 若 Network 没有请求：检查事件绑定、校验和提前返回；
- 若请求 pending：检查重复提交、Promise 未结束和服务端耗时；
- 若 4xx：检查参数、路由、凭据和权限；
- 若 5xx：结合服务端 Error Log 定位 API / Service；
- 若请求成功但 UI 仍 loading：检查 response handling、异常分支和 finally 状态恢复。

### 复验

修改后 reload 同一页面，使用相同输入重放，确认：

- loading 正常结束；
- 成功状态实际出现；
- 没有相关 Log issue；
- 没有失败、超时或重复 Network 请求。
