# Kooboo Objects and Module Structure

A client-rendered Vue application consists of Kooboo Page, Style, Script, API, and CodeBlock objects; no real disk directories exist. The design must describe both the admin Name and the browser URL. The reusable `app-base` source assets are separated into matching `Page/`, `Style/`, `Script/`, `API/`, and `CodeBlock/` folders so their Kooboo object type remains explicit.

## Standard frontend structure

The example groups repository sources by object type. Create resources as needed and use the design-system resource names returned by the installer.

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

| Type | Responsibility | Boundary |
|---|---|---|
| `main.js` / `app.js` | Startup, plugins, application context, root RouterView | No business lists, forms, or API implementations |
| `app-config.js` | Browser-visible application configuration | No secrets or server-owned identity policy |
| `router.js` | Routes, stable names, Screen identity, layout mapping, guards | No business requests; guards do not replace server authorization |
| `request-context.js` | Request source capture and service binding | Bind once per requesting component; no per-request header boilerplate |
| `layout.*.js` | Shell, navigation, account, notifications, child route outlet | Does not implement individual business Screens |
| `screen.*.js` | Route page entry, page state, queries, operation orchestration | Each Screen has an explicit entry; extract complex local interactions into components |
| `component.*.js` | Local forms, dialogs, tables, and interactions | Explicit props, events, and request ownership; no generic whole-page factory |
| `service.*.js` | API arguments, requests, response adaptation; retain existing http/bootstrap base contracts | Split business services by business area; no UI operations or imports from components/stores |
| `store.*.js` | Cross-component shared state, caches, and actions | Keep local state in the Screen/component; accept bound services for requests |
| `util.*.js` | Pure functions, formatting, constant mappings, simple business predicates | No Vue, Router, HTTP, DOM, or mutable shared state |
| `composable.*.js` | Reusable Vue reactive logic | Do not duplicate service responsibilities; requests use caller-bound services |

`util.format.js` contains general date/currency formatting; `util.asset.js` contains asset status mappings and simple business predicates. Keep page-specific helpers in the page and extract them when reused.

Use a responsibility prefix and a specific business name in kebab-case. Avoid vague names such as `common.js`, `helpers.js`, and `service.all.js`, and redundant role suffixes such as `service.asset-service.js`.

Import business modules directly from their files. Import design-system components from official entry points such as `/yuanli.js`.

Apply these conventions to new resources. Preserve existing names; when renaming is necessary, update URLs, imports, and route references together.

## Implementation list

Create a minimal object list before writing a complete application:

| Field | Description |
|---|---|
| `objectType` | Page / Style / Script / API / CodeBlock |
| `name` | Flat Kooboo admin Name |
| `url` | Browser Route |
| `operation` | create / update |
| `responsibility` | Single responsibility |
| `imports[]` | Direct dependencies |
| `consumers[]` | Direct consumers |

If upstream artifacts such as a Blueprint, Page Plan, design, or API contract exist, references may be kept in the list, but client-rendered solutions do not require a fixed upstream model.

Generic example:

| Type | Name | URL | Responsibility |
|---|---|---|---|
| Page | `index` | `/` | Complete HTML entry |
| Style | `kooboo-var.css` | `/kooboo-var.css` | Manageable CSS variables |
| Style | `app.css` | `/app.css` | Reset, theme, component, and responsive styles |
| Script | `vue.js` | `/vue.js` | Pinned Vue CDN exports |
| Script | `vue-router.js` | `/vue-router.js` | Vendored pinned Vue Router browser ESM bundle |
| Script | `app-config.js` | `/app-config.js` | Public App configuration; identity mode is server-owned |
| Script | `router.js` | `/router.js` | Hash routes and Kooboo login guard |
| Script | `lucide.js` | `/lucide.js` | Pinned Lucide loading and icon rendering |
| Script | `main.js` | `/main.js` | Initialization, plugin installation, error handling, mount |
| Script | `app.js` | `/app.js` | Application context and root RouterView |
| Script | `layout.default.js` | `/layout.default.js` | Default parent-route layout and child RouterView |
| Script | `store.task.js` | `/store.task.js` | Shared state, computed values, and actions |
| Script | `service.task.js` | `/service.task.js` | Real HTTP requests and response normalization |
| Script | `screen.task-list.js` | `/screen.task-list.js` | List screen |
| Script | `component.task-form.js` | `/component.task-form.js` | Create/edit form |
| API | `app.user` | `/api/app/user/{action}` | Current user / 当前用户 |
| CodeBlock | `ENV` | — | Server-owned App Base defaults / 服务端默认配置 |
| CodeBlock | `Services.user` | — | Identity and user profile / 身份与用户资料 |
| API | `app.notifications` | `/api/app/notifications/{action}` | Optional notifications template / 可选通知模板 |

Create a service only when a real HTTP call exists. With static data or localStorage, do not create an empty service for a future API.

For an App Base, keep CDN URLs out of the Page. The Page import map declares `vue` and `vue-router` through `/vue.js` and `/vue-router.js`. The Vue Router Script contains the pinned `esm.sh`-converted browser ESM bundle locally, with only `vue` left as an external import so it reuses `/vue.js`; do not add a global build, runtime CDN dependency, runtime Script injection, or a second Vue runtime. Lucide is not added to imports and loads through the separate `/lucide.js`. After mount, `main.js` calls `observeLucideIcons()`: it renders the initial icons, then watches for non-SVG `data-lucide` nodes inserted by later Vue updates and batches rendering on the next animation frame. Business components do not need to rescan manually after every `nextTick()`.

User and notification APIs are separate. Frontend services always use HTTP in both preview and real mode. Authentication and mock identity are server-owned; read [App Base services](../../Backend/app-base-services.md) for API contracts and the mandatory removal of all server mock branches before real data or delivery. `loadAppBootstrap` composes the separate results on the client only. `AppUsers` never stores passwords. Business services create notifications through `./Services.notification`.

## Naming rules

### Default short names

For a single-application site, prefer:

```text
main.js
app.js
app.css
store.task.js
screen.task-list.js
```

Add the smallest business prefix only when a same-name object is found before creation or one Site hosts multiple independent applications. Do not repeat the project name in every object by default or use random suffixes.

### Name and URL

| Logical group | Name | URL |
|---|---|---|
| screen | `screen.<name>.js` | `/screen.<name>.js` |
| store | `store.<name>.js` | `/store.<name>.js` |
| util | `util.<name>.js` | `/util.<name>.js` |
| composable | `composable.<name>.js` | `/composable.<name>.js` |
| service | `service.<name>.js` | `/service.<name>.js` |
| component | `component.<name>.js` | `/component.<name>.js` |
| layout | `layout.<name>.js` | `/layout.<name>.js` |
| worker | `worker.<name>.js` | `/worker.<name>.js` |

Name must not contain `/`. A Script URL is `/` plus its exact Name and must be unique in the site-wide Route namespace. Page and API URLs remain semantic routes rather than following this Script convention.

## Module dependency direction

```text
Page → main.js → app.js / router.js
router.js → layout.* / screen.*
layout.* → component.* / service.* / composable.*
screen.* → component.* / store.* / service.* / util.* / composable.*
component.* → store.* / service.* / util.* / composable.*
store.* → service.* / util.*
composable.* → util.* / caller-bound service
service.* → service.http.js / util.*
```

Hard boundaries:

- `main.js` is the only startup entry;
- `app.js` owns context and the root route outlet; `layout.*.js` owns layouts;
- Components may read the store or call explicit actions;
- The store may call services;
- Services handle only HTTP and data normalization and do not import Vue components or the store;
- The store, services, and components must not form circular imports;
- The Page must not inline large blocks of business JavaScript.

## main.js

`main.js` initializes the app, installs verified plugins, sets global error handling, and mounts it:

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

Rules:

- Install the UI framework after `createApp(App)` and before `mount()`;
- Do not guess plugin export names or installation methods;
- Preserve console diagnostics on initialization failure and show an understandable error in the entry fallback;
- Do not write list, form, or business rules in `main.js`.

Keep the scaffold's bootstrap and router initialization. Business stores may add their own initialization before mount without replacing this base flow.

Inside the editor preview iframe, `main.js` registers `window.__koobooPreviewRouter` after router readiness and mount, then dispatches the local `kooboo:preview-router-ready` event. The preview agent reads page routes and handles address selection through its existing validated message channel; the App does not listen for cross-window commands. Registration requires both `kooboobridge=true` and an iframe. Existing main.js objects are preserved: add the registration from the current template and refresh preview when enabling this capability, without rebuilding business routes.

## app.js

The root only provides application context and the route outlet. Nested routes in router.js compose default/other layouts and Screens; see the [layout contract](app-layout.md#layouts-belong-to-routes).

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

## Screens and navigation

Detail parameters and visible record identifiers follow [Display identifiers and detail URLs](../code-style.md#display-identifiers-and-detail-urls): use stable, unique, short readable codes or slugs instead of GUIDs in addresses, with the same contract in list links and detail APIs. This does not change hash/history mode.

A client-rendered App uses Vue Router with `createWebHashHistory()` by default and does not create a Kooboo Page for every screen:

```text
Page /
  App → RouterView
    layout.default → RouterView
      dashboard / task-list / task-detail / task-form
    other layout → RouterView
      other Screens
    standalone Screen
```

Map every App Plan Screen to one route in `/router.js`. Keep the catch-all fallback last. Routes are protected by the bootstrap authentication state; mock mode supplies that state locally, while Kooboo mode redirects unauthenticated routes only to `/_Admin/login`. Set `meta.public=true` only for an explicitly public Screen. Route guards are navigation UX, not API authorization, so protected real APIs must still check `k.account.isLogin`.

Do not replace routes with `showList` / `showDetail` / `activeScreen` boolean switches. Use route paths and params for durable screen identity, selected record IDs, browser forward/back, refresh recovery, and deep links. Component-local tabs, dialogs, and temporary panels may still use `ref` or a store.

## State boundaries

| State | Recommended location | Examples |
|---|---|---|
| Server or shared domain data | Module-level store | tasks, users, currentAccount |
| Cross-component UI state | Module-level store or root App | theme, currentScreen |
| Derived data | `computed()` | filteredTasks, summary |
| Form draft | Form component | draft, fieldErrors |
| Temporary dialog state | Direct consumer | isOpen, activeItemId |
| DOM reference | Current component `ref()` | inputEl, dialogEl |

Do not copy a second mutable domain array for searching or sorting. Derive with computed values and copy the array before sorting.

## store

Use module-level `reactive` / `ref` in the store and export only the state needed for read-only consumption and explicit actions:

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

Use domain verbs for actions and do not provide an ambiguous `setData`. Components must not modify shared arrays or objects directly.

## localStorage

When using localStorage:

- Include the app name and schema version in the key, such as `task-board.tasks.v1`;
- Put `JSON.parse` inside `try/catch`;
- Validate the top-level type and key fields;
- Define corruption fallback, migration, or reset behavior;
- Show understandable feedback when a write fails;
- Do not store secrets, long-lived tokens, real credentials, or unnecessary sensitive information.

Write initial data only when the key does not exist; do not overwrite user data on every refresh.

## Request sources and AI code location

In setup, bind services once with `useServices(import.meta.url, { task: taskService })`, then call ordinary business methods without capturing or passing context at each call site. Define services with `defineService(import.meta.url, { ...methods })`. Each method receives the framework-injected request function as its first parameter; subsequent parameters are business arguments. The module URL and method name generate the Service identifier automatically. `/service.http.js` remains the shared HTTP boundary; do not create another fetch wrapper.

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

| Header | Source |
|---|---|
| `X-Kooboo-Screen` | Route `meta.screenId`, preserving the original App Plan Screen ID; otherwise the route name |
| `X-Kooboo-Route` | Route `name`; use stable string names for business leaf routes |
| `X-Kooboo-Source` | Script path from useServices' import.meta.url |
| `X-Kooboo-Service` | defineService module path + `#` + method name |

Each service invocation captures its own context and receives a separate request closure. Requests, awaits, and retries within that invocation share the snapshot; concurrent calls cannot overwrite it. By default, page identity is bound during setup. Persistent layouts/components pass `{ currentRoute: true }` as the third argument to capture the route at each invocation; also use it when a component is reused across routes without remounting. Never infer callers using a global current component or stack traces.

Outside components, bind once using `bindServices({ auth: authService }, { source: new URL(import.meta.url).pathname })`; do not invent Screen/Route values. Components should use bound services. Existing auth/notification named exports remain callable but carry only the Service identifier. Stores/actions that execute requests should receive a bound service instance rather than re-importing unbound methods. When multiple service calls must share the operation's initial source, capture once with `useRequestContext(...).capture()` and use `bindServices`; this is an advanced case, not the default request pattern.

Identifiers accept only 1–256 printable non-space ASCII characters; missing or invalid fields are omitted. Do not send titles, full URLs, queries, user data, or dependency trees. HTTP adds source headers from the bound context to same-origin requests regardless of API path; cross-origin requests omit them. HTTP/network errors preserve method, path, and context in `error.request`. Headers are diagnostic hints, never authorization.

For request debugging, follow [targeted reads](../../Functions/read-update.md) to locate Source, Service, and routes, then expand along direct dependencies.

create_app_base preserves existing App resources rather than upgrading them. For migration, add the request-context Script first, then update HTTP, services, and callers together while preserving existing business logic.

## service

Business services use `defineService` as shown above and are responsible for:

- API URLs, methods, and business arguments;
- Business response validation and normalization;
- Passing through `AbortSignal`;
- Never disguising errors as successful data.

The store handles loading, error, caching, and domain state; components handle user feedback. Authentication and permissions remain server decisions.

## Style objects

Recommended:

```text
kooboo-var.css   只保存 DesignPanel 可管理变量
app.css          reset、基础规则、主题、组件和响应式样式
```

If the page does not need manageable design variables, do not create an empty `kooboo-var.css`, and remove its corresponding `<link>` from the Page. See [ui-foundation.md](ui-foundation.md) for the exact contract.

`service.bootstrap.js` owns startup composition. Notifications are optional: `appENV.notificationsEnabled = false` disables notification HTTP calls and hides workspace-shell controls. The browser adapter is sourced from `assets/app-base/Script/service.notification.js`; its backend belongs to `assets/backend-templates/notifications`. / `service.bootstrap.js` 负责启动组装；关闭通知开关后不请求通知接口，并隐藏工作区 Shell 的通知入口。

New business services export only the object returned by defineService; do not copy the unbound named exports retained by the base for legacy apps. The first unbound call per method emits a missing-caller diagnostic without blocking existing business behavior.
