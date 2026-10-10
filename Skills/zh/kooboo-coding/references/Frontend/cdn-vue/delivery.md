# Kooboo 客户端渲染 Vue 实施流程

本文定义如何把一个客户端渲染 Vue 界面创建或更新为 Kooboo Page、Style、Script 对象。它不规定产品必须来自 Blueprint，也不限制应用只能使用 mock 数据。

## 请求来源绑定（App Base）

业务 Service 使用 `defineService(import.meta.url, methods)`；每个发起请求的 Screen、弹窗和 Layout 必须在自己的 setup 中通过 `useServices` 绑定一次。不要在组件中直接导入 `listAssetRecords` 等未绑定的命名方法，也不要直接调用模块级 `assetService.listAssetRecords()`，否则只会生成 X-Kooboo-Service，丢失页面、路由和发起资源。新业务服务只导出 Service 对象，不复制 auth/notification 为旧应用保留的命名函数导出。

```js
import { useServices } from '/request-context.js'
import { assetService } from '/service.asset.js'

// Inside the requesting Screen/component setup():
const { asset } = useServices(import.meta.url, { asset: assetService })
// Event handlers, watches, pagination and onMounted use this bound instance:
const data = await asset.listAssetRecords({ page: 1, pageSize: 10 })
```

每个真正请求数据的弹窗也要绑定自己的 import.meta.url，不能拿父页面的 Source 冒充弹窗。跨页面保留的 Shell 使用 `useServices(import.meta.url, services, { currentRoute: true })`。Service 内部使用注入的 request，不读取 Vue 组件或手写 Header。组件外通过 bindServices 显式绑定 source；示例与完整契约见 [project-structure.md](project-structure.md)。

业务叶子路由必须有稳定字符串 name；有 App Plan 时保留 meta.screenId，无 App Plan 时 Screen 回退到路由名。读回代码时确认调用链确实使用绑定实例。运行时验收若已获授权，分别查看页面加载、分页、弹窗、Shell 通知请求，确认 X-Kooboo-Screen / Route / Source / Service；启动请求只有 Source / Service 是正常的。仅出现 Service 不代表追踪接入完成。

## D0：确认运行边界

实施前明确：

- 新建应用、改造已有 Page，还是修改单个组件；
- Page Name、访问 URL、语言和是否必须保留现有 Route；
- 是否需要 SEO 或服务端首屏内容；
- 数据来自 mock API、静态值、localStorage 还是已有 HTTP API；
- 是否涉及登录、权限、上传、支付或其它服务端能力；
- 浏览器兼容范围和是否允许依赖外部 CDN。

若任务需要服务端能力，本目录继续负责 Vue 前端，但必须同时使用对应的后端规范。不能用前端状态或按钮隐藏替代服务端实现。

## D1：发现已有对象

写入前：

1. 列出相关 Page、Style、Script；
2. 检查目标 Name 是否在同类型中存在；
3. 检查目标 URL 是否与任意对象 Route 冲突；
4. 更新对象时先读取现有内容、URL、引用关系和用途；
5. 判断是复用、最小更新、增加业务前缀还是停止确认。

不能用随机 Name 或 URL 绕过冲突，也不能在未读现有内容时覆盖 `index`、`main.js`、`app.js`、`app.css` 等常见对象。

## D2：设计对象清单

按 [结构规范](project-structure.md) 建立本次对象清单，至少写明：

- `objectType`；
- `name`；
- `url`；
- `operation`：create / update；
- `responsibility`；
- `imports[]`；
- `consumers[]`。

对象清单是实施辅助，不是必须持久化的产品模型。小型局部修改可以只保留受影响对象；完整应用必须先让“当前交付切片”的依赖闭合，不要求首轮列出或创建尚未实施的全部 screen 组件。

模块依赖遵循 [结构规范](project-structure.md) 的依赖方向。

## D3：验证外部依赖

写业务对象前先确认：

- Vue 使用官方文档推荐的 Vue 3 full build CDN 通道；
- Tailwind、UI 框架、图标和字体 URL 可访问；
- ESM 包的导出名、import specifier 和安装方式真实可用；
- UI 框架复用 import map 中的 Vue，没有加载第二份 Vue；
- 字体和图标依赖的子资源没有 CORS 或 404；
- 最小组件能在完整 HTML Page 中挂载、渲染和响应交互。

依赖检查规则见 [ui-foundation.md](ui-foundation.md)。失败时更换加载方式或依赖，不要把未验证的猜测写进业务组件。

## D4：首次创建

没有 Blueprint/App Plan 的 App 使用[轻量 App 实施](../../Scenario/app-building/lightweight.md)：读取已保存的 SiteMilestone 清单，通过直接配置安装同一个 App Base，再按依赖顺序实施当前能力。Screen 保存并读回后逐步增加路由和导航，不要求完整页面清单或 Plan manifest。下文 App Plan 专属步骤仅用于带规划 App；公共资源与设计系统约定仍然适用。

普通独立前端任务首次创建按依赖从叶到根。若当前任务来自当前 App Plan 中用户授权的交付范围，则使用下方“首页优先的渐进交付”覆盖本节的全量创建顺序。

### App Plan：首页优先的渐进交付

首次 App UI 写入前必须读取 [App Base 布局交付](app-layout.md)。写入并回读 CSS 和 Shell 后直接继续默认 Screen 的依赖实施。依赖就绪前保留 Route 占位，不在业务实施前插入浏览器布局验收。保留底座已有 setup，并核对组件结构；读过 CSS 不代表验收通过。

1. 用 `read_app_plan({})` 读取当前 Plan，选定完整授权的 DeliverySlice/对象范围，为业务结果建立 Todo。选择适合任务且有路由的入口 Screen；保留已有入口，除非审核需求要求改变。
2. 以 DataContract 和 Service Operation 为接口设计，实施已明确的数据策略。mock API 样本留在 HTTP API 中，成功写入信号不代表真实持久化；真实存储需要实际读写验证。
3. 按 [设计系统消费规则](../app-design-systems.md) 安装 App Base 与运行库，并按 [App 布局](app-layout.md) 接入布局。随后继续业务 Screen，安装本身不构成交付。
4. `/app.css`、`/layout.default.js` 已落库并回读后，把默认 Screen 作为一个依赖闭包创建：API → service/store → 叶子组件 → 新建该 Screen 专属的 Vue 根组件 Script。该根组件必须按 App Plan 的 Blocks、Actions 和主要任务组织自己的 `setup`、template、信息层级与交互组合。允许共享设计系统原子组件、叶子业务组件、service、store 和 composable；不允许多个 `screen.*.js` 只把标题、字段、数据源或文案传给同一个包办页头、指标、内容区、表格和表单的整页工厂。若两个 Route 的根页面仅配置不同而 DOM 骨架与主要工作流相同，后一个仍视为未实现。所有本地 import 必须先成功落库并读回，再只替换 `/router.js` 中当前 Screen 的 `PlannedScreen` 引用。禁止改写或复用托管的共享 `/component.planned-screen.js`，否则全部未实现 Route 都会显示同一个已实现页面；新 Shell 也不得 import 尚未创建的 Screen 模块。
5. 任一依赖失败都保留占位，不更新消费者。静态和动态 import 都遵守此规则，以避免生成过程中的模块错误；禁止先写完全部组件，再集中更新 Router 或根组件。
6. 读回 Screen 与它的依赖闭包，确认 Route 指向 Screen 专属根组件，并核对该组件实际覆盖当前 Screen 的 Blocks、Actions、loading/empty/error/success 状态，而不是通用整页工厂的配置壳；通过有限只读 Preview diagnostics 获取可用的运行问题；除非用户明确要求浏览器测试，不主动执行浏览器观察、元素检查、点击、导航或设备切换。未实测的视觉和交互如实标为未验证。随后对剩余 Screen 逐个重复第 4–6 步，不增加逐页 Preflight 扫描。
7. App Base 不是交付物或停止点。产品 UI 不展示脚手架/运行状态；除非有明确外部阻塞，否则继续完成整个授权范围。未选定的 Plan 范围不自动获得实施授权。
8. 完成前读回当前 Plan，核对选定范围的可达 Screen、Action、Service Operation、DataContract 及真实数据行为。部分实现或未验证的 Todo 保持未完成，不将执行状态写入 Plan。

普通独立前端任务的全量首次创建顺序如下：

1. `create_style`：`kooboo-var.css`（确实需要设计变量时）；
2. `create_style`：`app.css`；
3. `create_script`：service（仅存在真实 HTTP 调用时）；
4. `create_script`：store；
5. `create_script`：叶子组件；
6. `create_script`：组合组件和 screen；
7. `create_script`：`app.js`；
8. `create_script`：`main.js`；
9. `create_page`：完整 HTML 入口，省略 `layoutName`；
10. 逐个读回关键对象，核对 URL、imports、exports、组件注册和外部资产。

Page 创建前，被入口直接引用的 Style 和 Script Route 必须已经存在。不要为了“结构完整”创建空 store、service 或组件。

## D5：更新已有应用

先确定最小影响面：

1. 修改叶子对象；
2. 更新直接消费者；
3. 最后更新 `app.js`、`main.js` 或 Page；
4. 每个批次读回已修改对象；
5. 刷新并检查实际 Page URL。登录或诊断不可用导致无法验证时，按[实施与验收](../../Scenario/app-building/site-milestones.md#运行时验证)保留验证待补，继续已授权实施；实际失败仍须在依赖工作前修复。

特殊情况：

- 修改模块 URL：先创建并验证新 URL，再更新所有消费者，最后处理旧对象；
- 修改 export 或组件名：同批更新 import、注册名和 template 标签；
- 修改 store action：检索全部调用者，不能只改定义；
- 修改 API 契约：同步更新 service、store、loading/error 和 UI 消费者；
- 删除对象：先确认无消费者，再按 Kooboo 删除规范执行。

## D6：Preflight

完成一项局部变更后才执行 focused Preflight，不按对象或 Screen 重复扫描；只有完整页面交付或 App Milestone 收尾才执行 capability Preflight。示例：

```json
{
  "taskType": "all",
  "layers": ["frontend", "design", "accessibility", "security", "performance"],
  "mode": "capability",
  "artifacts": [
    { "name": "index", "objectType": "Page" },
    { "name": "app.css", "objectType": "Style" },
    { "name": "main.js", "objectType": "Script" },
    { "name": "app.js", "objectType": "Script" }
  ]
}
```

按本次对象清单补齐实际写入对象。无 Layout Page 不传 `layoutName`。Preflight PASS 只证明扫描范围内的结构，没有替代浏览器运行验收。

## D7：实际 Page URL 验收

至少验证：

- Page、CSS、ES Modules、CDN 和 API 请求无意外 4xx/5xx；
- Vue 只加载一份并成功 mount；
- 控制台无模块解析、template 编译和运行时错误；
- 默认界面、导航、表单和核心操作可用；
- loading、empty、error 和成功反馈符合实际数据模式；
- localStorage 恢复、损坏回退和 reset 可用（若使用）；
- HTTP 请求的超时、失败、重复提交和未授权状态有处理（若使用）；
- 320px、平板和桌面宽度可操作；
- 键盘、focus、Escape、label 和错误提示可验证。

源码读回或 Preflight PASS 不能替代实际 Page URL。若环境无法访问实际 URL，必须明确报告未完成运行验收。

## 完成回复

最终报告：

- 创建或更新的 Page、Style、Script；
- 实际入口 URL；
- 数据来源和服务端依赖；
- 已验证的主要交互；
- Delivery Slice 状态，逐个点名部分实现或未验证的 Slice；
- 未验证项、已知限制或后续工作。

带规划 App 只有覆盖整个授权范围才可宣称完成；否则报告部分实现，并继续剩余授权工作。

授权写入 Site 时，应实际创建或更新对象，不默认在回复中粘贴全部源码。只有用户明确要求“仅输出代码、不写 Site”时，才按对象 Name 和 URL 输出完整内容。

后续授权 App 变更保留已安装 Shell 和入口 Screen，仅对审核范围执行依赖与路由替换，不重建布局或实施无关能力。
