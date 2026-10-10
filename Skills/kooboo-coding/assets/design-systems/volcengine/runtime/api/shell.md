# YlShell

Import `{ YlShell } from '/yuanli.js'`; register in `components`.

Props: `appName: string` required; `navigation: NavigationItem[] = []`; `activePath: string = ''`; `navigationLabel = '主导航'`; `menuLabel = '打开导航'`; `closeLabel = '关闭导航'`.
Desktop sidebar: expanded by default, with no built-in collapse button. `collapsed: boolean | undefined = undefined`; omit for internal state, or bind `v-model:collapsed` for caller ownership. Event: `update:collapsed(boolean)`. Component ref exposes `collapsed`, `toggleSidebar()` and `setSidebarCollapsed(boolean)`. These fully hide/restore the desktop aside; hidden controls are not keyboard targets. No icon rail, hover flyout or persistence is provided.
YlShell composes YlLayout/YlHeader/YlAside/YlMain. Those base containers can be installed independently for other App structures.
The App chooses whether it needs desktop collapse, and owns the trigger's location, icon, size and wording. Both `actions` and default slots receive `{collapsed, toggleSidebar, setSidebarCollapsed, sidebarTriggerAttrs}`. `sidebarTriggerAttrs` provides aria-expanded, aria-controls and an aria-label from `collapseLabel='收起侧栏'` / `expandLabel='展开侧栏'`; bind it to the App's button and add the click handler. Place the restore action somewhere still available when the sidebar is hidden. The title has no reserved desktop toggle space. Mobile drawer navigation remains separate and available via its menu button; desktop collapsed state does not hide it.
`NavigationItem = {label:string,path?:string,key?:string,group?:string,icon?:string,disabled?:boolean,children?:NavigationItem[]}`. Leaves require a path; parents may omit it. Use stable keys (or unique path/label within each sibling list); supply an acyclic tree. Root group labels and existing flat arrays remain supported. Nested group fields are ignored.
Event: `navigate(path)` for ordinary left clicks; modified clicks retain native anchor behavior. Supply valid same-App route paths from the Plan/Router, not untrusted external URLs. Slots: `actions` for account/notifications; default for RouterView.

Optional App-owned action (one example placement, not a standard Shell control):

```html
<YlShell :app-name="appName" :navigation="navigation">
  <template #actions="{collapsed, toggleSidebar, sidebarTriggerAttrs}">
    <YlButton v-bind="sidebarTriggerAttrs" variant="text" @click="toggleSidebar">
      {{ collapsed ? '显示导航' : '隐藏导航' }}
    </YlButton>
  </template>
  <RouterView />
</YlShell>
```

For a trigger elsewhere in the App, bind `v-model:collapsed` to shared state or call the exposed methods through a component ref. Skip the control entirely when the business does not need collapse. A custom desktop-only trigger should use the App's responsive CSS to hide below the Shell's 760px breakpoint.

Without path, a parent is an expand/collapse button. With path, it is a route link with a separate expand button. Disabled parents disable their descendants. Ancestors of an exact activePath match open on initial render, route change or navigation data change; users can collapse them afterwards. Expansion is internal, shared by desktop/mobile. Tab visits visible links/buttons; Enter activates links, Enter/Space toggles buttons. Arrow up/down and Home/End move among visible enabled items; Right expands or enters children; Left collapses or returns to the parent. These are nested navigation lists, not an ARIA tree widget. Directory toggles keep mobile navigation open; route selection closes it.

```js
const navigation = [
  { path: '/dashboard', label: '概览', icon: 'layout-dashboard' },
  { key: 'inventory', label: '库存管理', icon: 'warehouse', children: [
    { path: '/products', label: '产品', icon: 'package' },
    { key: 'movements', label: '出入库', children: [
      { path: '/receipts', label: '入库记录' },
      { path: '/issues', label: '出库记录' }
    ] }
  ] }
]
```

```html
<YlShell class="app-root" :app-name="appName" :navigation="navigation" :active-path="route.path" @navigate="router.push($event)">
  <template #actions><!-- real account/notification controls --></template>
  <RouterView />
</YlShell>
```

The optional `create_app_base(shellTemplate="yuanli-workspace")` adapter supplies this binding plus account/logout and notification panels. Prefer that installation when it fits the Plan. The account panel is an anchored non-modal popover; its logout calls the installed auth service. Default Screen routes remain placeholders until business implementation.

Owns one identity, compact 48px-minimum top navigation, 200px desktop grouped sidebar, white content with insets and gray navigation. Below 760px, desktop navigation is hidden and the menu trigger opens a native modal navigation panel. Clicking a route or changing activePath closes mobile navigation. Long App names can wrap; the Shell fills the viewport using an auto-height header and a remaining-height workspace. Main content scrolls independently; desktop navigation stays in place and scrolls only when its own items overflow. Changing activePath resets main scroll to the top without resetting sidebar scroll. This is the workspace preset, not a constraint on other App layout patterns. Navigation uses 20px Lucide icons from item.icon (kebab-case), with layout-grid as fallback. The installed adapter reads route meta.navigationIcon. Known choices may be supplied at creation through create_app_base.navigationIcons keyed by exact App Plan Screen ID; omitted choices can be completed when authoring the Shell. Set semantic names such as package, warehouse and chart-no-axes-combined. Mobile menu, notifications, account and logout also use Lucide icons; panel triggers carry aria-controls. Do not add another identity/sidebar/page title around it.

`navigationLoading=false` replaces desktop navigation with a noninteractive YlSkeleton while the caller prepares navigation. Turn it off on completion or failure. The installed workspace adapter enables it only for a planned route with no navigation entries. It does not add a sidebar toggle.
