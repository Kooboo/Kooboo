# 浏览器运行架构

## 两层结构

客户端渲染 Vue 应用保持后台 Script 对象扁平，并让每个 Script 使用对应的 `/<Name>` 浏览器 URL。Name 中的点号表达逻辑分组，不再创建虚拟目录。

```text
后台对象                           浏览器 URL

Page:   index                     /
Style:  kooboo-var.css            /kooboo-var.css
Style:  app.css                   /app.css
Script: main.js                   /main.js
Script: app.js                    /app.js
Script: store.app-store.js        /store.app-store.js
Script: component.shell.js        /component.shell.js
Script: component.feature-list.js /component.feature-list.js
```

这些不是真实磁盘文件或目录。前导 `/` 让对应 Script URL 保持根绝对形式，因此原生 ES Module 仍可按标准方式加载。

## Name 与 URL

### Name

- 只使用字母、数字、点、下划线和连字符，最长 64 字符。
- 不包含 `/`。
- 在同一对象类型中必须唯一。
- 单应用站点默认使用简短名称，如 `main.js`、`app.js`、`app.css`。
- 创建前若发现同名对象，再增加最小业务前缀；不要默认给每个文件重复添加项目名。

建议用点号表达后台列表中的逻辑分组：

```text
store.task-store.js
component.task-list.js
component.task-form-modal.js
```

### URL

- 必须以 `/` 开头。
- Script URL 必须等于 `/` 加完整对象 Name；代码职责分组通过 Name 中的点号表达。
- CSS 与 JavaScript 已由对象类型区分，URL 不需要再增加 `/css/`、`/js/` 或 `/assets/<project>/`。
- Route 是全站共享命名空间，Style、Script、Page 的 URL 仍必须彼此唯一。
- 站内资源统一使用根绝对 URL，避免页面路由和末尾斜杠影响相对路径解析。

```text
/app.css
/kooboo-var.css
/main.js
/app.js
/store.task-store.js
/component.task-list.js
/component.task-form-modal.js
```

## 标准 HTML 入口

入口是一个无 Layout 的完整 HTML Page，内容只使用标准 HTML：

```html
<!doctype html>
<html lang="zh-CN">
<head>
  <meta charset="UTF-8">
  <meta name="viewport" content="width=device-width, initial-scale=1.0">
  <meta name="color-scheme" content="light dark">
  <title>应用名称</title>

  <link rel="stylesheet" href="/kooboo-var.css">
  <link rel="stylesheet" href="/app.css">
  <script src="https://www.kooboo.com/tailwindcss.js"></script>

  <script type="importmap">
  {
    "imports": {
      "vue": "https://unpkg.com/vue@3/dist/vue.esm-browser.js"
    }
  }
  </script>
</head>
<body>
  <div id="app" v-cloak></div>
  <noscript>此应用需要启用 JavaScript。</noscript>
  <script type="module" src="/main.js"></script>
</body>
</html>
```

不要写 `<layout>`、`<placeholder>`、`<view>`、`k-placeholder`、`env="server"` 或其它服务端模板标签。

上述是不使用 UI 框架和图标库时的最小入口。若界面需要 UI 框架、图标或字体，再增加已经验证、锁定精确版本的 CSS、import map 或 ESM 资产。所有外部依赖都应在编写业务组件前完成最小冒烟验证。

`app.css` 必须包含 `[v-cloak] { display: none; }` 或等价规则，避免 Vue mount 前显示未编译 template。入口 Page 不内联大段业务 CSS 或组件代码；它只负责文档壳、外部依赖、Style 和启动模块。

## Vue full build

开发时使用 Vue 官方文档推荐的 unpkg Vue 3 full build：

```text
https://unpkg.com/vue@3/dist/vue.esm-browser.js
```

生产时切换到同一 Vue 3 通道的 prod build：

```text
https://unpkg.com/vue@3/dist/vue.esm-browser.prod.js
```

不要换成 `vue.runtime.esm-browser.js`。普通 JavaScript 组件依赖浏览器端模板编译器来处理 `template` 字符串。

## 导入规则

Vue 从 import map 导入：

```javascript
import {
  computed,
  createApp,
  nextTick,
  onMounted,
  onUnmounted,
  reactive,
  ref,
  watch
} from 'vue'
```

站内模块使用根绝对 URL：

```javascript
import App from '/app.js'
import { state, createTask } from '/store.task-store.js'
import TaskList from '/component.task-list.js'
```

根绝对导入比 `./` 和 `../` 更适合路由对象：

- 不依赖当前 Page URL。
- 不依赖模块 URL 的虚拟层级。
- 调整一个模块的 URL 时，引用位置更容易检索。
- 不需要为站内模块额外配置 import map 别名。

App Base 预置固定版本的 Vue、Vue Router 与 Lucide；这不是“优先零依赖”的限制。可以按界面需要增加通过 [ui-foundation.md](ui-foundation.md) 兼容性检查的 UI 框架和专项能力库。优化目标是交付速度与稳定性，不是尽量减少依赖数量。

## 单 Page 导航

客户端渲染 App 默认是使用 Vue Router 和 Hash History 的单 Page SPA。Screen 映射为 Vue Route，不是独立 Kooboo Page。

- 每个 App Plan Screen 都用稳定 path 与 params 映射到 `/router.js`；
- 保证浏览器前进后退、刷新恢复、深链接和未知 Route 回退；
- 默认要求 Kooboo 登录，只有明确公开的 Screen 才设置 `meta.public=true`；
- 不为每个 Screen 创建空 Kooboo Page，也不用布尔 screen 切换代替 Route。

导航项、Screen ID、Route Record 和组件注册必须逐项一致。

## 样式技术边界

页面视觉遵循用户要求或上游设计规范。本目录不规定具体配色、字体、圆角或布局风格，只保留浏览器运行约束：Tailwind 使用 Kooboo 托管的 v3 CDN，UI 依赖通过兼容性闸门，`kooboo-var.css` 保持纯变量文件，其余样式写入 `app.css` 或组件 class。

禁止在 JavaScript 中拼接不完整的 Tailwind 类名：

```javascript
// 错误：扫描器无法可靠识别完整类名
const colorClass = `bg-${color}-500`
```

```javascript
// 正确：由视觉方案给出的完整类名显式枚举
const colorClass = computed(() => ({
  high: 'priority priority--high',
  medium: 'priority priority--medium',
  low: 'priority priority--low'
})[priority.value])
```

## 浏览器执行流程

```text
请求 Page URL
  → 加载 /kooboo-var.css、/app.css、Kooboo Tailwind v3、图标库与已选 UI 框架资产
  → 解析 Vue import map
  → 加载 /main.js
  → 通过根绝对 URL 加载 app、store 和组件模块
  → createApp(App)
  → 按 uiFoundation 安装 UI 框架（如有）
  → 设置全局错误处理
  → mount('#app')
  → Vue full build 编译静态 template
  → 从静态数据、localStorage 或 HTTP API 初始化应用状态
```

启动失败必须在控制台保留可定位错误。若入口壳需要向用户显示加载失败提示，可以使用 Page 中的静态 fallback 元素，并只在 Vue 成功 mount 后移除；不要用空白页面掩盖模块错误。

## 模块依赖方向

推荐依赖保持单向：

```text
Page
  → main.js
    → app.js
      → screen / shell components
        → leaf components
      → store
        → service（仅真实 HTTP 调用）
```

- store 不导入组件；
- 叶子组件不导入 `app.js`；
- 组件通过 props、emits 或明确 store action 协作；
- service 不导入 store 或组件；
- 避免循环 import；
- `main.js` 不承载领域逻辑；
- 外部依赖只在约定并验证的位置导入或安装。

## 实施边界

- 只创建真实需要的 Page、Style 和 Script 对象，不假设存在磁盘目录。
- Script Name 保持扁平，并统一路由到 `/<Name>`；例如 `store.task-store.js` 使用 `/store.task-store.js`。
- 组件直接导出普通 JavaScript 对象，使用静态 template；不要使用 `defineComponent()`、动态 template 或 `v-html`。
- 使用包含模板编译器的 Vue full build。
- Vue 使用指定官方 CDN 通道，Tailwind 使用 Kooboo 托管的 v3 CDN；其它 CDN 依赖采用经过验证的版本策略。
- 站内资源使用根绝对 URL。
- HTML 是浏览器可直接执行的完整文档，不依赖服务端模板指令。
- screen 导航默认留在单 Page 内，不把内部状态伪装成真实 Route。
- CDN 资源不可用会导致应用无法启动；交付验收必须观察实际网络请求，不能只核对 URL 字符串。
