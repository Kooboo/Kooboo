# Kooboo 客户端渲染 Vue 开发规范

> 命名边界：目录名只用于兼容既有技术路由。面向普通用户统一称为“客户端渲染”，不得在回复、确认问题、里程碑名称或下一步推荐中展示内部目录名、Vue 版本、import map 或 Page/Style/Script 对象组合；用户主动询问技术细节时才说明具体实现。

使用 Kooboo 的 Page、Style、Script 对象构建由浏览器直接运行的 Vue 3 CSR 界面。业务代码保持标准 HTML、CSS 和 JavaScript ES Modules，不依赖 `.vue`、Vite、Webpack、Node 构建产物或虚拟文件系统。

本文规定的是客户端渲染 Vue 在 Kooboo 中的技术实现方式，不是某一种产品流程。它可以用于正式业务界面、后台、仪表盘、内部工具、已有页面改造或交互原型。

AppBlueprint、Page Plan、视觉设计稿、API 契约等上游产物可以作为输入，但都不是进入本目录的前置条件。上游业务流程继续决定“做什么”，本目录只决定“在 Kooboo 中怎样写 Vue”。

## 适用场景

- 页面以客户端交互为主，不依赖搜索引擎抓取完整业务内容；
- 希望使用 Vue 响应式、组件、computed 和生命周期；
- 代码需要直接保存为 Kooboo Page、Style、Script 对象；
- 不需要 `.vue` SFC、npm 插件编译、TypeScript 编译或打包器转换。

以下情况优先选择其它方案：

- 公开内容、SEO 和服务端首屏输出优先：使用 Kooboo SSR / View / KScript；
- 只需少量交互：使用原生 JavaScript，避免引入完整 Vue；
- 必须使用 SFC、复杂 npm 生态、编译期插件或代码分包：使用正式构建产物，再按静态资源方式部署；
- 任务核心是 API、Database、Permission 或认证：同时读取对应 Backend / KScript / 场景规范，本目录不替代后端设计。

## Kooboo 对象模型

默认使用：

- 一个无 Layout 的完整 HTML Page；
- 一个变量 Style：`kooboo-var.css`，或由 `create_app_base` 安装的 `/design-tokens.css`；
- 一个普通 Style：`app.css`；
- 一个启动 Script：`main.js`；
- 一个根组件 Script：`app.js`；
- 按职责拆分的 store、service 和 component Script。

新建客户端 App 已有已保存的 Milestone 清单，并选定内置设计系统时，先调用一次 `create_app_base`，不要重复创建 Page、token Style、基础 Style、Vue/Vue Router 运行时、Hash Router、启动/根组件 Script、`/_Admin/login` 接入、当前用户 bootstrap 或最小站内通知基础设施。有规划时使用 `settings.designSystem`，无规划时使用轻量 App 路径的直接配置。把返回的基础对象作为起点，只新增业务专属资源。

Kooboo 对象不是磁盘文件。Script Name 保持扁平，并统一使用 `/<Name>` 作为 URL，使 Name 与 URL 直接对应：

| 类型 | Name | URL |
|---|---|---|
| Page | `index` | `/` |
| Style | `app.css` | `/app.css` |
| Script | `store.task.js` | `/store.task.js` |
| Script | `screen.task-list.js` | `/screen.task-list.js` |

Script URL 直接在完整 Name 前加 `/`，不再把点号逻辑组转换成虚拟目录；Page 和 API 仍使用有业务含义的 Route。完整规则见 [overview.md](overview.md)。

创建业务 Script 前读取 [project-structure.md](project-structure.md) 的标准结构与命名规则。工具模块使用 `util.*`；store/composable 按需创建。

## 默认技术基线

| 层 | 默认选型 |
|---|---|
| Vue | Vue 3 full build，使用官方文档推荐的 unpkg ESM 地址，通过 import map 加载 |
| Tailwind | Tailwind v3，使用 Kooboo 托管 CDN |
| 组件 | 普通 JavaScript 对象，`setup()` + 静态 `template` |
| 模块 | 原生 `.js` ES Modules，站内根绝对 URL |
| 状态 | 组件 `ref/reactive` 或模块级 store |
| 数据 | 静态数据、localStorage 或已存在的 HTTP API |
| UI 框架 | 可选；必须验证 Vue 版本、CDN/ESM 加载和安装方式 |
| 图标 | 按需求选择一套经过验证的图标库 |

生产环境把 Vue URL 切换到同一官方 Vue 3 通道的 `vue.esm-browser.prod.js`。必须保持 Vue 3 full build、Kooboo Tailwind v3 和本文要求的加载形态；不能改用 runtime-only build。

## 核心硬约束

- Page 是可独立运行的完整 HTML 文档，不写 Layout、View、placeholder、`env="server"` 或其它服务端模板指令。
- Style 和 Script 使用真实 Kooboo 对象；不要生成 `package.json`、`src/`、`.vue`、构建配置等无法直接保存的工程文件。
- Vue 组件直接导出普通对象，不使用 `defineComponent()`；template 静态写在模块内。
- 编写 App 自有组件前，读取[组件契约与预览来源元数据](components.md#组件契约)。
- 禁止远程加载、运行时拼接 template，禁止 `v-html` 渲染不可信内容。
- 站内 CSS、Script 和 import 使用根绝对 URL，并保留 `.js` 扩展名。
- Vue 使用本文指定的官方 CDN 通道；Tailwind 固定使用 Kooboo 托管的 v3 CDN。其它第三方依赖按项目兼容性决定是否锁定版本。UI 框架必须复用同一份 Vue，不能加载第二个 Vue runtime。
- 创建或更新前检查同类型 Name 和全站 Route，不能用随机后缀掩盖冲突。
- store 只保存共享状态；组件局部草稿、弹窗和临时输入保留在组件内。
- service 只在存在真实 HTTP 调用时创建；禁止空 service 和“以后可能用到”的占位模块。
- App Shell 必须保持单例。新增导航或品牌区前先检查当前根组件，并沿用现有 Shell 的职责。同一视口内，当前 App 的 Logo 和名称只能出现在一个身份区；不得在基础 Shell 内再嵌套一套完整 TopNav、Sidenav、品牌区或 App Shell。只有两处明确代表不同实体（例如平台与当前工作区），并通过不同名称清楚表达层级时，才允许同时出现。
- 前端隐藏按钮不等于权限控制；安全、认证和授权必须由服务端实现。
- 写入后读回关键对象，执行适用的 Preflight，并通过实际 Page URL 验证网络、控制台和交互。

## 推荐实施顺序

1. 确认页面职责、现有对象、数据来源和运行边界；
2. 设计 Page / Style / Script 对象清单及依赖方向；
3. 验证 Vue、UI 框架、图标、字体等外部依赖；
4. 按“Style → store/service → 叶子组件 → 组合组件 → app.js → main.js → Page”创建；
5. 更新已有应用时按“叶子 → 直接消费者 → 根入口”修改；
6. 读回对象、执行 Preflight、访问实际 Page URL 验收。

详细步骤见 [delivery.md](delivery.md)。

## 文档导航

| 文档 | 职责 |
|---|---|
| [overview.md](overview.md) | 浏览器架构、Name/URL、完整 HTML 入口、导航和导入规则 |
| [project-structure.md](project-structure.md) | 前端结构、命名、职责边界与依赖方向 |
| [components.md](components.md) | 普通对象组件、props/emits、生命周期和拆分规则 |
| [ui-foundation.md](ui-foundation.md) | UI 依赖兼容性、图标和 Style 技术契约 |
| [delivery.md](delivery.md) | 发现、创建、更新、读回、Preflight 和运行验收流程 |
| [quality.md](quality.md) | 状态、表单、可访问性、安全、网络和最终检查 |

App Base 的所有业务 HTTP 调用必须遵循 [请求来源绑定](delivery.md)：defineService 定义服务，useServices 在组件 setup 绑定一次；不能从组件直接调用未绑定方法。
