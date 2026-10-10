# KbElementShell

Optional Kooboo workspace preset built with native Element Plus navigation and drawer.

Kind: component. Import `KbElementShell` from `/element-plus.js`. Shared setup and state ownership: [runtime guide](../usage.md).

## Usage and state ownership

Props: appName (required string), navigation (array, default []), activePath (string, default ''), collapsed (boolean, default false), navigationLabel ('Navigation'), menuLabel ('Open navigation'). Navigation items: { path, label, group?, icon?: Lucide kebab-case, disabled? }. Emits navigate(path), update:collapsed(boolean). default slot is main content; actions slot receives { collapsed, toggle }. Exposes toggle() and openNavigation(). Desktop collapse is controlled and has no forced trigger; render one in actions when needed. Mobile drawer state is independent. Header stays above independent sidebar/main scrollers; route changes reset main scroll. For nested menus, top navigation or editors, compose native ElContainer/Header/Aside/Main/Footer and ElMenu/ElSubMenu instead of forcing this preset. Styling lives in /element-plus.adapters.css (skill resource runtime/element-plus.adapters.css), separate from upstream /element-plus.css. The .kb-ep-shell-main container owns RouterView outer padding: 24px on desktop and 16px at widths up to 767px; do not duplicate this inset on business Screens. Read the small adapter stylesheet when inspecting Shell layout. Put deliberate business overrides in /app.css, never edit managed runtime CSS.

```js
h(KbElementShell, { appName, navigation, activePath, onNavigate: path => router.push(path) }, { default: () => h(RouterView) });
```


`navigationLoading: boolean = false` shows a noninteractive skeleton in desktop/mobile navigation. The default template enables it for planned routes with no navigation entries. Turn it off on completion or failure.
