# YlIcon

Import `{ YlIcon }` from `/yuanli.js` and register it. Props: `name: string` required (Lucide kebab-case), `size: number = 16` pixels. No events. Decorative SVG inherits currentColor and has aria-hidden; put the accessible name on its parent control.

```html
<YlIcon name="package" :size="20" />
<YlButton icon="plus" variant="primary">新增产品</YlButton>
<YlButton icon="pencil" variant="text" aria-label="编辑产品" />
```

Bundled Lucide 0.468.0 names (no network needed): `x`, `check`, `chevron-down`, `chevron-left`, `chevron-right`, `menu`, `bell`, `user-round`, `log-out`, `layout-grid`, `plus`, `search`, `rotate-ccw`, `refresh-cw`, `pencil`, `save`, `trash-2`, `package`, `warehouse`, `arrow-down-to-line`, `arrow-up-from-line`, `chart-no-axes-combined`, `settings`, `loader-circle`, `layout-dashboard`, `list-checks`, `bar-chart-3`, `message-square`, `clipboard-list`, `users`, `book-open`, `send`, `arrow-down-up`, `circle-check`, `circle-x`, `triangle-alert`, `info`.

Other valid names use the existing App Base `/lucide.js` loader and pinned CDN on demand; unavailable names/network failures leave the icon space empty and warn. Prefer the bundled names for common business flows. Vue owns the SVG; do not use data-lucide or call createIcons within these components, and do not draw substitute SVG or use emoji.

Use semantic icons for navigation, notifications/account, create/search/reset/edit/save/delete and in/out actions; keep useful text labels alongside them. Default 16px for controls and 20px for navigation. Pure text such as cancel and field labels does not need an icon.
