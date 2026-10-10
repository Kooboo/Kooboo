# Browser Runtime Architecture

## Two-layer structure

A client-rendered Vue application keeps admin Script objects flat and gives each Script the matching browser URL `/<Name>`. Dots in the Name express the logical group without creating virtual folders.

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

These are not disk files or folders. The leading `/` makes the matching Script URL root-absolute, so native ES Modules can still be loaded in the standard way.

## Name and URL

### Name

- Use only letters, digits, dots, underscores, and hyphens, with a maximum length of 64 characters.
- Do not include `/`.
- Names must be unique within the same object type.
- For a single-application site, use short names by default, such as `main.js`, `app.js`, and `app.css`.
- If a same-name object is found before creation, add the smallest necessary business prefix; do not repeat the project name in every file by default.

Use dots to express logical groups in the admin list:

```text
store.task-store.js
component.task-list.js
component.task-form-modal.js
```

### URL

- Must begin with `/`.
- For Scripts, must equal `/` plus the exact object Name; use dots in the Name to express responsibility groups.
- CSS and JavaScript are already distinguished by object type, so URLs do not need `/css/`, `/js/`, or `/assets/<project>/`.
- Route is a site-wide shared namespace; Style, Script, and Page URLs must still be unique from one another.
- Use root-absolute URLs for site resources to avoid page routes and trailing slashes affecting relative-path resolution.

```text
/app.css
/kooboo-var.css
/main.js
/app.js
/store.task-store.js
/component.task-list.js
/component.task-form-modal.js
```

## Standard HTML entry

The entry is a complete HTML Page without a Layout and uses only standard HTML:

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

Do not write `<layout>`, `<placeholder>`, `<view>`, `k-placeholder`, `env="server"`, or other server-template tags.

The above is the minimum entry when no UI framework or icon library is used. If the interface needs a UI framework, icons, or fonts, add CSS, import-map, or ESM assets that have been verified and pinned to exact versions. Every external dependency should pass a minimum smoke test before business components are written.

`app.css` must contain `[v-cloak] { display: none; }` or an equivalent rule to prevent an uncompiled template from appearing before Vue mounts. The entry Page must not inline large blocks of business CSS or component code; it is responsible only for the document shell, external dependencies, Styles, and the startup module.

## Vue full build

For development, use the unpkg Vue 3 full build recommended by the official Vue documentation:

```text
https://unpkg.com/vue@3/dist/vue.esm-browser.js
```

In production, switch to the prod build on the same Vue 3 channel:

```text
https://unpkg.com/vue@3/dist/vue.esm-browser.prod.js
```

Do not switch to `vue.runtime.esm-browser.js`. Plain JavaScript components rely on the browser-side template compiler to process `template` strings.

## Import rules

Import Vue from the import map:

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

Use root-absolute URLs for site modules:

```javascript
import App from '/app.js'
import { state, createTask } from '/store.task-store.js'
import TaskList from '/component.task-list.js'
```

Root-absolute imports are better suited to route objects than `./` and `../`:

- They do not depend on the current Page URL.
- They do not depend on the virtual hierarchy of a module URL.
- Import sites are easier to find when a module URL changes.
- They do not require additional import-map aliases for site modules.

The App Base preloads pinned Vue, Vue Router, and Lucide resources; this is not a “prefer zero dependencies” restriction. Add UI frameworks and specialist capability libraries that pass the compatibility checks in [ui-foundation.md](ui-foundation.md) when the interface needs them. Optimize for delivery speed and stability rather than minimizing the dependency count.

## Single-Page navigation

A client-rendered App is a single-Page SPA backed by Vue Router and hash history by default. A Screen maps to a Vue Route and is not a separate Kooboo Page.

- Map every App Plan Screen to `/router.js` with stable paths and params;
- Keep browser forward/back, refresh recovery, deep links, and unknown-route fallback working;
- Require Kooboo login by default and set `meta.public=true` only for explicitly public Screens;
- Do not create an empty Kooboo Page for every Screen or replace routes with boolean screen switches.

Navigation items, Screen IDs, route records, and component registrations must match item by item.

## Styling technical boundary

Page visuals follow user requirements or upstream design guidance. This directory does not prescribe specific colors, fonts, radii, or layout styles; it retains only browser-runtime constraints: Tailwind uses the Kooboo-hosted v3 CDN, UI dependencies pass the compatibility gate, `kooboo-var.css` remains a variables-only file, and other styles go into `app.css` or component classes.

Do not concatenate incomplete Tailwind class names in JavaScript:

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

## Browser execution flow

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

Startup failures must leave actionable errors in the console. If the entry shell needs to show a load-failure message, use a static fallback element in the Page and remove it only after Vue mounts successfully; do not hide module errors behind a blank page.

## Module dependency direction

Keep dependencies one-way by default:

```text
Page
  → main.js
    → app.js
      → screen / shell components
        → leaf components
      → store
        → service（仅真实 HTTP 调用）
```

- The store does not import components;
- Leaf components do not import `app.js`;
- Components coordinate through props, emits, or explicit store actions;
- Services do not import the store or components;
- Avoid circular imports;
- `main.js` does not contain domain logic;
- Import or install external dependencies only at agreed and verified locations.

## Implementation boundaries

- Create only the Page, Style, and Script objects that are truly needed; do not assume disk directories exist.
- Keep Script Names flat and route each one at `/<Name>`; for example, `store.task-store.js` uses `/store.task-store.js`.
- Components directly export plain JavaScript objects with static templates; do not use `defineComponent()`, dynamic templates, or `v-html`.
- Use a Vue full build that includes the template compiler.
- Use the specified official CDN channel for Vue and the Kooboo-hosted v3 CDN for Tailwind; use a verified version strategy for other CDN dependencies.
- Use root-absolute URLs for site resources.
- HTML must be a complete document executable directly by the browser and must not depend on server-template directives.
- Keep screen navigation inside the single Page by default; do not disguise internal state as a real Route.
- Unavailable CDN assets prevent the application from starting; delivery acceptance must observe actual network requests rather than only checking URL strings.
