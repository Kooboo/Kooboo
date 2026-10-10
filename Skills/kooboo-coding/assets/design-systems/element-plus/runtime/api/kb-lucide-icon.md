# KbLucideIcon

Vue-rendered SVG using the pinned App Base Lucide loader.

Kind: component. Import `KbLucideIcon` from `/element-plus.js`. Shared setup and state ownership: [runtime guide](../usage.md).

## Usage and state ownership

Props: name (required Lucide kebab-case string), size (number|string, default 18). Uses /lucide.js loadLucide() and renders nodes with Vue h; it never calls createIcons on Vue-owned DOM. Glyphs are decorative aria-hidden; the owning button needs an accessible label. Initial glyph availability follows the App Base CDN loader and must be checked in the deployment environment.

```js
h(ElButton, { 'aria-label': 'Search' }, { default: () => h(KbLucideIcon, { name: 'search' }) });
```
