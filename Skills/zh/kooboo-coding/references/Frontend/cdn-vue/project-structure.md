# Kooboo 对象与模块结构

客户端渲染 Vue 应用由 Kooboo Page、Style、Script、API、CodeBlock 对象组成，不存在真实磁盘目录。结构设计必须同时描述后台 Name 和浏览器 URL。可复用 `app-base` 源资产按对应类型拆到 `Page/`、`Style/`、`Script/`、`API/`、`CodeBlock/` 文件夹，避免混淆 Kooboo 对象类型。

## 前端标准结构

以下按仓库源码类型分组，按需创建。设计系统资源使用安装器返回的名称。

```text
Page/
  index.html
Style/
  app-base.css
  <design-system>.css
  app.css
Script/
  main.js
  app.js
  app-config.js
  router.js
  request-context.js
  vue.js
  vue-router.js
  lucide.js
  <design-system>.js
  layout.default.js
  layout.<name>.js
  screen.asset-list.js
  screen.asset-records.js
  component.asset-form-dialog.js
  component.asset-detail-dialog.js
  component.asset-action-dialog.js
  component.page-head.js
  service.http.js
  service.bootstrap.js
  service.auth.js
  service.notification.js
  service.asset.js
  store.asset.js
  util.format.js
  util.asset.js
  composable.<name>.js
```

| 类型 | 职责 | 边界 |
|---|---|---|
| `main.js` / `app.js` | 启动、插件、应用上下文、根 RouterView | 不放业务列表、表单和接口实现 |
| `app-config.js` | 浏览器可见的应用配置 | 不包含密钥或服务端身份策略 |
| `router.js` | 路由、稳定 name、Screen 标识和布局映射、守卫 | 不实现业务请求；守卫不替代服务端授权 |
| `request-context.js` | 请求来源捕获与 Service 绑定 | 页面和组件绑定一次，不逐请求手写 Header |
| `layout.*.js` | Shell、导航、账号、通知、子路由出口 | 不承载具体业务 Screen |
| `screen.*.js` | 路由页面入口、页面状态、查询和操作编排 | 一个 Screen 有明确入口；复杂局部交互拆成组件 |
| `component.*.js` | 表单、弹窗、表格等局部交互 | 明确 props、事件和请求归属，不伪装成通用整页工厂 |
| `service.*.js` | API 参数、请求、响应适配；http/bootstrap 等底座职责按现有契约保留 | 业务 Service 按业务域拆分，不操作 UI，不导入组件或 store |
| `store.*.js` | 跨组件共享状态、缓存和动作 | 页面局部状态留在 Screen/组件；需请求时接收绑定后的 Service |
| `util.*.js` | 纯函数、格式化、常量映射和简单业务判断 | 不依赖 Vue、Router、HTTP、DOM，不持有可变共享状态 |
| `composable.*.js` | 可复用的 Vue 响应式逻辑 | 不重复承担 Service 职责；请求通过调用方绑定的 Service 执行 |

`util.format.js` 放通用日期、金额格式化；`util.asset.js` 放资产状态映射和简单业务判断。页面独有的小函数留在页面内，有复用需求时再提取。

文件名采用职责前缀加具体业务名称，业务名称使用 kebab-case。避免 `common.js`、`helpers.js`、`service.all.js` 等含糊命名，以及 `service.asset-service.js` 等重复职责后缀。

业务模块直接 import 所需文件；设计系统组件使用官方入口，如 `/yuanli.js`。

新增资源遵循本规范。保留已有资源名称；确需改名时同步更新 URL、import 和路由引用。

## 实施清单

完整应用开始写入前，建立最小对象清单：

| 字段 | 说明 |
|---|---|
| `objectType` | Page / Style / Script / API / CodeBlock |
| `name` | Kooboo 后台扁平 Name |
| `url` | 浏览器 Route |
| `operation` | create / update |
| `responsibility` | 单一职责 |
| `imports[]` | 直接依赖 |
| `consumers[]` | 直接消费者 |

若上游已有 Blueprint、Page Plan、设计稿或 API 契约，可以在清单中保存引用，但客户端渲染方案不要求固定的上游模型。

通用示例：

| 类型 | Name | URL | 职责 |
|---|---|---|---|
| Page | `index` | `/` | 完整 HTML 入口 |
| Style | `kooboo-var.css` | `/kooboo-var.css` | 可管理的 CSS 变量 |
| Style | `app.css` | `/app.css` | reset、主题、组件和响应式样式 |
| Script | `vue.js` | `/vue.js` | 固定版本 Vue CDN 导出 |
| Script | `vue-router.js` | `/vue-router.js` | 本地内置的固定版本 Vue Router 浏览器 ESM bundle |
| Script | `app-config.js` | `/app-config.js` | 公开 App 配置；身份模式由服务端控制 |
| Script | `router.js` | `/router.js` | Hash Route 与 Kooboo 登录守卫 |
| Script | `lucide.js` | `/lucide.js` | 固定版本 Lucide 加载与图标渲染 |
| Script | `main.js` | `/main.js` | 初始化、插件安装、错误处理、mount |
| Script | `app.js` | `/app.js` | 应用上下文与根 RouterView |
| Script | `layout.default.js` | `/layout.default.js` | 默认父路由布局和子级 RouterView |
| Script | `store.task.js` | `/store.task.js` | 共享状态、computed、actions |
| Script | `service.task.js` | `/service.task.js` | 真实 HTTP 请求与响应归一化 |
| Script | `screen.task-list.js` | `/screen.task-list.js` | 列表 screen |
| Script | `component.task-form.js` | `/component.task-form.js` | 新增/编辑表单 |
| API | `app.user` | `/api/app/user/{action}` | Current user / 当前用户 |
| CodeBlock | `ENV` | — | Server-owned App Base defaults / 服务端默认配置 |
| CodeBlock | `Services.user` | — | Identity and user profile / 身份与用户资料 |
| API | `app.notifications` | `/api/app/notifications/{action}` | 当前用户的站内通知 |

只有存在真实 HTTP 调用时才创建 service。使用静态数据或 localStorage 时，不要为未来接口创建空 service。

App Base 的 CDN 地址不放在 Page 中。Page 的 import map 通过 `/vue.js` 与 `/vue-router.js` 声明 `vue` 和 `vue-router`。Vue Router Script 在本地保存固定版本、经 `esm.sh` 转换的浏览器 ESM bundle，只把 `vue` 保留为 external import，从而复用 `/vue.js`；不要再加入 global 构建、运行时 CDN 依赖、运行时 Script 注入或第二份 Vue。Lucide 不加入 imports，而是由独立的 `/lucide.js` 加载。`main.js` 在 mount 后调用 `observeLucideIcons()`：它先渲染首屏图标，再监听 Vue 后续插入的非 SVG `data-lucide` 节点，并在下一动画帧自动批量渲染。业务组件不需要在每次 `nextTick()` 后手动重复扫描。

用户与通知 API 分别负责自己的数据，前端 service 在预览和真实模式均走 HTTP。身份和 mock 分支由服务端控制；接口契约及真实数据或交付前必须删除全部服务端 mock 分支的规范见 [App Base 服务](../../Backend/app-base-services.md)。`loadAppBootstrap` 仅在前端组合独立结果；`AppUsers` 不保存密码。业务服务通过 `./Services.notification` 创建通知。

## 命名规则

### 默认简短命名

单一应用站点优先使用：

```text
main.js
app.js
app.css
store.task.js
screen.task-list.js
```

创建前发现同名对象，或一个 Site 承载多个独立应用时，才增加最小业务前缀。不要默认给所有对象重复添加项目名，也不要使用随机后缀。

### Name 与 URL

| 逻辑组 | Name | URL |
|---|---|---|
| screen | `screen.<name>.js` | `/screen.<name>.js` |
| store | `store.<name>.js` | `/store.<name>.js` |
| util | `util.<name>.js` | `/util.<name>.js` |
| composable | `composable.<name>.js` | `/composable.<name>.js` |
| service | `service.<name>.js` | `/service.<name>.js` |
| component | `component.<name>.js` | `/component.<name>.js` |
| layout | `layout.<name>.js` | `/layout.<name>.js` |
| worker | `worker.<name>.js` | `/worker.<name>.js` |

Name 不包含 `/`。Script URL 统一为 `/` 加完整 Name，并在全站 Route 命名空间中唯一；Page 和 API 仍使用有业务含义的 Route，不套用此 Script 约定。

## 模块依赖方向

```text
Page → main.js → app.js / router.js
router.js → layout.* / screen.*
layout.* → component.* / service.* / composable.*
screen.* → component.* / store.* / service.* / util.* / composable.*
component.* → store.* / service.* / util.* / composable.*
store.* → service.* / util.*
composable.* → util.* / 调用方绑定的 service
service.* → service.http.js / util.*
```

硬性边界：

- `main.js` 是唯一启动入口；
- `app.js` 只负责上下文与根路由出口，布局放在 `layout.*.js`；
- component 可以读取 store 或调用明确 action；
- store 可以调用 service；
- service 只处理 HTTP 和数据归一化，不导入 Vue 组件或 store；
- store、service、component 之间不得形成循环 import；
- Page 不内联大段业务 JavaScript。

## main.js

`main.js` 负责初始化、安装已验证插件、设置全局错误处理并挂载：

```javascript
import { createApp } from 'vue'
import App from '/app.js'
import { createAppRouter } from '/router.js'
import { loadAppBootstrap } from '/service.bootstrap.js'
import { installPreviewSource } from '/preview-source.js'

const bootstrap = await loadAppBootstrap()
const app = createApp(App, { bootstrap })
const router = createAppRouter(bootstrap)
installPreviewSource(app, router)

app.config.errorHandler = (error, instance, info) => {
  console.error('Vue runtime error', {
    error,
    componentName: instance?.$options?.name || 'UnknownComponent',
    info
  })
}

app.use(router)
await router.isReady()
app.mount('#app')
```

规则：

- UI 框架在 `createApp(App)` 后、`mount()` 前安装；
- 不猜测插件导出名或安装方法；
- 初始化失败要保留控制台诊断，并让入口 fallback 呈现可理解错误；
- 不在 `main.js` 编写列表、表单或业务规则。

保留脚手架中的 bootstrap 与 Router 初始化。业务 store 可以在 mount 前追加自己的初始化，但不能替换这条基础流程。

在编辑器预览 iframe 中，`main.js` 会在 Router 就绪并挂载后注册 `window.__koobooPreviewRouter` 并发送本地 `kooboo:preview-router-ready` 事件。预览桥从 Router 读取页面列表并通过现有受校验的消息通道提供地址栏切换，App 不自行监听跨窗口消息。此接入仅在 `kooboobridge=true` 且页面位于 iframe 中时启用。已有站点的 main.js 不会自动覆盖；需要此能力时，按当前模板补充注册代码并刷新预览，不重建业务路由。

## app.js

根组件只提供应用上下文和路由出口。默认布局、其他布局与页面由 router.js 的嵌套路由组织，详见 [布局契约](app-layout.md#路由拥有布局)。

```javascript
// app.js
import { provide } from 'vue'
export default {
  name: 'App',
  koobooSource: { moduleUrl: import.meta.url },
  props: { bootstrap: { type: Object, required: true } },
  setup(props) { provide('app.bootstrap', props.bootstrap) },
  template: '<RouterView />'
}
```

```javascript
// router.js (all referenced components must already exist)
export const appRoutes = [
  { path: '/', component: ReaderLayout, children: [
    { path: '', component: HomeScreen, meta: { public: true } },
    { path: 'articles/:id', component: ArticleScreen, meta: { public: true } }
  ] },
  { path: '/admin', component: DefaultLayout, children: [
    { path: '', component: AdminScreen }
  ] },
  { path: '/login', component: LoginScreen, meta: { public: true } }
]
```

## screen 与导航

详情页参数与可见记录编号遵循 [展示编号与详情页标识](../code-style.md#展示编号与详情页标识)：使用稳定、唯一、简短易读的编号或 slug，避免 GUID 出现在地址中；列表链接与详情 API 必须共同采用该标识契约。此规则不改变 hash/history 模式。

客户端渲染 App 默认使用 Vue Router 的 `createWebHashHistory()`，不为每个 screen 创建 Kooboo Page：

```text
Page /
  App → RouterView
    layout.default → RouterView
      dashboard / task-list / task-detail / task-form
    other layout → RouterView
      other Screens
    standalone Screen
```

每个 App Plan Screen 都映射到 `/router.js` 中的一个 Route，catch-all 回退保持在最后。Route 使用 bootstrap 提供的登录状态保护：mock 模式在本地提供该状态，Kooboo 模式下未登录 Route 只跳转 `/_Admin/login`。只有明确公开的 Screen 才设置 `meta.public=true`。Route Guard 只是导航体验，不代替 API 权限，受保护的真实 API 仍必须检查 `k.account.isLogin`。

不得用 `showList` / `showDetail` / `activeScreen` 等布尔切换代替 Route。Screen 标识、选中记录 ID、浏览器前进后退、刷新恢复和深链接都使用 path 与 params；组件内部 Tab、Dialog 和临时 Panel 仍可使用 `ref` 或 store。

## 状态边界

| 状态 | 建议位置 | 示例 |
|---|---|---|
| 服务端或领域共享数据 | 模块级 store | tasks、users、currentAccount |
| 跨组件 UI 状态 | 模块级 store 或根 App | theme、currentScreen |
| 派生数据 | `computed()` | filteredTasks、summary |
| 表单草稿 | 表单组件 | draft、fieldErrors |
| 临时弹窗状态 | 直接消费者 | isOpen、activeItemId |
| DOM 引用 | 当前组件 `ref()` | inputEl、dialogEl |

不要复制第二份可变领域数组用于搜索或排序。使用 computed 派生，并在排序前复制数组。

## store

store 使用模块级 `reactive` / `ref`，只导出只读消费所需状态和明确 action：

```javascript
import { computed, reactive } from 'vue'

export const state = reactive({
  tasks: [],
  loading: false,
  error: null
})

export const taskCount = computed(() => state.tasks.length)

export function createTask(input) {
  const title = input.title.trim()
  if (!title) throw new Error('任务标题不能为空')

  state.tasks.unshift({
    id: crypto.randomUUID(),
    title,
    createdAt: new Date().toISOString()
  })
}
```

action 使用领域动词，不提供含糊的 `setData`。组件不要直接修改共享数组或对象。

## localStorage

使用 localStorage 时：

- key 带应用名和 schema 版本，例如 `task-board.tasks.v1`；
- `JSON.parse` 放在 `try/catch`；
- 验证顶层类型和关键字段；
- 定义损坏回退、迁移或 reset 行为；
- 写入失败时显示可理解反馈；
- 不保存密钥、长期 token、真实凭据或不必要的敏感信息。

初始化数据只在 key 不存在时写入，不能每次刷新覆盖用户数据。

## 请求来源与 AI 定位

组件在 setup 中使用 `useServices(import.meta.url, { task: taskService })` 一次性绑定所需服务，随后直接调用业务方法，不在每个调用点 capture 或传递 context。业务 Service 用 `defineService(import.meta.url, { ...methods })` 定义；方法的第一个参数是框架注入的 request，其余参数才是业务调用参数。模块路径和方法名自动生成 Service 标识，不逐个手写。统一请求仍由 `/service.http.js` 完成，不另建 fetch 封装。

```javascript
// screen.task-list.js
import { ref } from 'vue'
import { useServices } from '/request-context.js'
import { taskService } from '/service.task.js'

export default {
  name: 'TaskList',
  koobooSource: { moduleUrl: import.meta.url },
  setup() {
    const tasks = ref([])
    const { task } = useServices(import.meta.url, { task: taskService })
    async function refresh() {
      tasks.value = await task.listTasks()
    }
    return { tasks, refresh }
  }
  // Keep the component's template here.
}
```

```javascript
// service.task.js
import { defineService } from '/service.http.js'

export const taskService = defineService(import.meta.url, {
  listTasks(request, { signal } = {}) {
    return request('/api/app/tasks', { signal })
  }
})
```

| Header | 来源 |
|---|---|
| `X-Kooboo-Screen` | 路由的 `meta.screenId`，保留 App Plan 原始 Screen ID；无 Plan 时回退到路由名 |
| `X-Kooboo-Route` | 路由 `name`，业务叶子路由使用稳定字符串名称 |
| `X-Kooboo-Source` | useServices 的 import.meta.url 所对应的 Script 路径 |
| `X-Kooboo-Service` | defineService 的模块路径 + `#` + 方法名 |

每次 Service 方法被调用时自动捕获上下文，并为该次调用生成独立 request 闭包；方法内部的多个请求、await 和重试共享快照，并发调用互不覆盖。默认保留 setup 时的页面身份。跨页面保留的 Layout/组件使用第三个参数 `{ currentRoute: true }`，捕获每次调用时的路由；同一组件跨路由复用且不会重新挂载时也使用该选项。不要用全局当前组件或调用堆栈推断来源。

组件外使用 `bindServices({ auth: authService }, { source: new URL(import.meta.url).pathname })` 绑定一次；不伪造 Screen/Route。组件应使用绑定后的服务；原有 auth/notification 命名函数导出仍可调用，但只携带 Service 标识。store/action 若负责调用服务，应接收绑定后的服务实例，而不是重新导入未绑定的方法。若多个 Service 调用必须共用操作开始时的来源，可用 `useRequestContext(...).capture()` 捕获一次，再通过 `bindServices` 绑定；这属于特殊情况，不作为普通请求模板。

标识只接受 1–256 个非空格可打印 ASCII 字符，缺少或不符合约定的字段省略。不传标题、完整 URL、query、用户数据或完整引用树。HTTP 根据绑定的上下文为同源请求注入来源 Header，不限制 API 路径；跨域请求不携带这些字段。HTTP/网络错误的 `error.request` 保留 method、path 和 context。Header 只供诊断，不用于授权。

请求排查按 [定向读取流程](../../Functions/read-update.md) 定位 Source、Service 和路由，再沿直接依赖扩大读取范围。

已安装 App 不会被 create_app_base 覆盖升级。迁移时先新增 request-context Script，再成套更新 HTTP、Service 和调用方，保留已有业务逻辑。

## service

业务 Service 使用上文的 `defineService`，负责：

- API URL、method 和业务参数；
- 业务响应校验与归一化；
- `AbortSignal` 透传；
- 不把错误伪装成成功数据。

store 负责 loading、error、缓存和领域状态；组件负责用户反馈。认证和权限仍由服务端决定。

## Style 对象

推荐：

```text
kooboo-var.css   只保存 DesignPanel 可管理变量
app.css          reset、基础规则、主题、组件和响应式样式
```

若页面不需要可管理设计变量，可以不创建空的 `kooboo-var.css`，并从 Page 移除对应 `<link>`。具体契约见 [ui-foundation.md](ui-foundation.md)。

`service.bootstrap.js` owns startup composition. Notifications are optional: `appENV.notificationsEnabled = false` disables notification HTTP calls and hides workspace-shell controls. The browser adapter is sourced from `assets/app-base/Script/service.notification.js`; its backend belongs to `assets/backend-templates/notifications`. / `service.bootstrap.js` 负责启动组装；关闭通知开关后不请求通知接口，并隐藏工作区 Shell 的通知入口。

新业务 Service 仅导出 defineService 返回的对象；不要复制底座为旧应用保留的未绑定命名方法导出。未绑定方法首次调用会输出缺失调用来源的诊断，但不会阻断原有业务。
