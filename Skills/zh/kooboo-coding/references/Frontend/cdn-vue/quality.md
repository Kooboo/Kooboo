# 工程质量与验收

客户端渲染 Vue 的完成口径不是“对象已经创建”，而是 Page、Style、Script 依赖闭合，实际 Page URL 能加载并完成目标交互，且前端能力没有被误写成服务端安全保证。

## 状态与派生数据

- 原始共享数据保存在 store；搜索、筛选、排序和统计使用 `computed()` 派生；
- 组件临时状态使用局部 `ref()` / `reactive()`，不要把所有状态都放入 store；
- action 使用领域动词，例如 `createTask`、`updateTask`、`loadTasks`；
- JavaScript 中访问 ref 使用 `.value`，template 中不写 `.value`；
- `v-for` 使用稳定领域 ID，禁止数组下标 key；
- 排序前复制数组，避免意外修改 store。

## 数据来源

### 静态数据与 localStorage

- 静态数据的刷新行为必须明确；
- localStorage key 包含应用名和 schema 版本；
- `JSON.parse` 使用 `try/catch`，并验证数据形状；
- 提供损坏回退和 reset；
- 写入失败不能继续显示“已保存”；
- 不保存密钥、长期 token、真实凭据或不必要的敏感信息。

### HTTP API

- URL、method、headers、body 和响应结构来自真实契约，不猜测；
- 检查 `response.ok`，不能把 4xx/5xx 当成功 JSON；
- 使用明确 loading / error / empty / success 状态；
- 组件卸载或请求被替换时，按需使用 `AbortController`；
- 保存中禁用重复提交，失败时保留用户输入；
- 401/403 只反映服务端结果，前端按钮隐藏不能代替权限；
- 不在 service 中吞掉错误或返回假成功数据。

## 表单与业务动作

- 输入值在提交边界统一 `trim()`；
- 必填、日期、数字范围和枚举值在 action 前校验；
- 错误信息位于对应控件附近，并用 `aria-describedby` 关联；
- 提交失败保留草稿；成功后再清理和关闭；
- 新增与编辑字段一致时复用表单组件；
- 破坏性操作需要确认和清晰结果反馈；
- 不使用随机延迟伪装真实网络；需要演示 loading 时使用可重复、明确标记的状态。

## 模态框与焦点

模态框至少满足：

- `role="dialog"`；
- `aria-modal="true"`；
- `aria-labelledby` 指向可见标题；
- 打开后聚焦合适控件；
- Escape 关闭；
- 卸载时移除键盘监听；
- 关闭后尽可能把焦点还给触发元素；
- 遮罩关闭策略不能导致未保存内容意外丢失。

复杂模态框需要 focus trap。使用 UI 框架时优先使用其官方 Dialog，并验证相同的焦点和关闭行为。

## 可访问性

- 每个表单控件有可见 label，`for` 与 `id` 一致；
- 按钮显式写 `type="button"` 或 `type="submit"`；
- 图标按钮有 `aria-label`，装饰性图标有 `aria-hidden="true"`；
- 操作使用原生 `<button>`，不用可点击 `<div>` 模拟；
- 状态不能只依赖颜色表达；
- loading 区域按需使用 `aria-busy`，异步消息按需使用 `aria-live`；
- 触摸目标约 40×40 CSS 像素或更大；
- focus 可见；
- 尊重 `prefers-reduced-motion`。

## 响应式

- 320px 宽度下仍可完成主要操作；
- 表格在窄屏采用横向滚动、卡片化或字段折叠，不直接裁掉关键操作；
- 侧栏有明确移动端展开/关闭方式；
- 模态框不超出视口，可滚动且关闭按钮可见；
- 长标题、标签、URL 和错误信息有换行或截断策略；
- 同时检查手机、平板和桌面宽度。

## 错误处理

- `main.js` 设置 `app.config.errorHandler`；
- 每个组件保留明确 `name`；
- 异步 action 在 `finally` 中恢复 loading；
- 不允许空 `catch {}`；
- 启动失败时控制台保留可定位信息，页面显示静态 fallback；
- 一个复杂模块的失败不应拖垮多个无关功能，按职责拆分 Script。

## 安全边界

- 禁止 `v-html` 处理不可信内容；
- 用户文本通过 Vue 普通插值输出；
- 不把不可信文本拼入 template、事件表达式、style 或脚本 URL；
- 外部链接使用 `target="_blank"` 时添加 `rel="noopener noreferrer"`；
- URL 输入限制允许的协议和用途；
- 前端源码不保存服务端 secret；
- 前端校验、路由守卫和按钮隐藏都不是认证或授权实现。

## 模块与依赖一致性

- Name 不包含 `/`；新建 Script 的 URL 必须等于 `/` 加完整 Name，已有 Route 除非明确纳入迁移范围否则保持不变；
- 对象清单的 create/update 与实际写入一致；
- import、export、组件注册、template 标签、event 和 action 名一致；
- 不存在循环 import；
- Vue 使用官方文档推荐的 unpkg Vue 3 full build 通道；
- Tailwind 使用 Kooboo 托管的 v3 CDN；
- Vue 使用指定官方通道，Tailwind 固定为 v3；UI 框架、图标和其它 CDN 资产采用经过验证的版本策略；
- 页面没有加载第二份 Vue；
- JavaScript 条件类名使用完整静态字符串，不拼接 Tailwind 片段；
- 不创建空 store、service 或组件。

## Preflight

- 局部改动使用 focused 模式；完整应用使用 capability 模式；
- artifacts 覆盖本次实际 Page、Style、Script；
- 无 Layout Page 不传 `layoutName`；
- 处理 active BLOCKER，不能只摘录 PASS；
- Preflight PASS 不替代必需的只读 Preview 诊断；浏览器操作仍需用户主动开启。

## 实际 Page URL 验收

浏览器验收至少覆盖：

1. Page、Style、Script、CDN 和 API 请求；
2. Vue mount 和默认界面；
3. 导航或 screen 切换；
4. 列表、空状态和错误状态；
5. 表单校验、提交和重复提交保护；
6. loading、成功和失败反馈；
7. localStorage 刷新恢复与损坏回退（若使用）；
8. HTTP 失败、401/403、取消或超时策略（若使用）；
9. 键盘、焦点、Escape 和 label；
10. 手机、平板和桌面宽度；
11. 控制台无模块解析、template 编译和运行时错误。

CDN 或 API 的实际网络请求不可用时，不得只核对源码 URL 后宣称完成。

## 最终检查清单

- [ ] Page 是完整 HTML，未绑定 Layout 或混入服务端模板指令
- [ ] Name、URL、对象类型和依赖关系正确；每个新建 Script 使用 `/<Name>`
- [ ] Vue full build 和 Tailwind 使用指定官方 CDN 通道，外部依赖版本策略明确
- [ ] Page 没有加载第二份 Vue
- [ ] `main.js`、`app.js`、store、service 和组件职责清晰
- [ ] service 只在真实 HTTP 调用存在时创建
- [ ] 组件声明 name、props、emits、setup 和静态 template
- [ ] 状态、表单、错误、空状态和 loading 可用
- [ ] 安全与权限没有仅靠前端实现
- [ ] 响应式和键盘操作通过
- [ ] Preflight 无 active BLOCKER
- [ ] 实际 Page URL、Network 和 Console 已验证

## AI 交付行为

用户授权写入 Kooboo Site 时：

1. 先确认对象范围和 create/update；
2. 调用对应 Functions 创建或更新对象；
3. 每批写入后读回关键对象；
4. 完成 Preflight 和实际 URL 验收；
5. 最终报告对象、入口、数据来源和验证结果，不默认粘贴完整源码。

只有用户明确要求“仅输出代码、不写 Site”时，才逐项标明对象类型、Name、URL 并输出完整代码。
