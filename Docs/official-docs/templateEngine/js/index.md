# Script

> Routed `.js` resources for shared utilities, ES Modules, and cooperation with `<script>` tags inside Pages.

## What It Is

**Script** is a separately stored JavaScript file in a Kooboo site. Once registered, it is accessible through a **URL** such as `/assets/app.js`. It is a good fit for:

- utility functions shared across many pages,
- a Vue or React entry module,
- `/scripts/layout.js` in an `importmap` plus `type="module"` architecture,
- or frontend logic that should not be pasted into every Page.

In KScript, Scripts are managed by **`k.site.scripts`**. See [k.site.scripts](../../api/site/js.md).

::: tip Difference from Code or API resources
- **Script**: a static JS file loaded by URL in the browser
- **Code (Api)**: a server-side route plus KScript, see [k.site.codes](../../api/site/code.md)
- **`<script env="server">` inside a Page**: server-side KScript, not a Script resource
:::

## Referencing It in Templates

Inside a Layout or Page:

```html
<script src="/scripts/layout.js" type="module"></script>
```

The path is the **url** configured when the Script was created. If not configured, the default is `/{name}.js`. On the server, `k.site.scripts.getUrl(id)` returns the relative path.

### `async` and `defer`

When the resource is created, you can configure `async` and `defer`, which become attributes on the rendered tag. See the API docs for details.

## Two Script Responsibilities

| Type | Syntax | Runtime |
|------|--------|---------|
| Server-side KScript | `<script env="server">` | Server render |
| Frontend JS | A standalone Script resource, or `<script>` / `<script type="module">` inside a Page | Browser |

Put `env="server"` directly on the KScript tag instead of relying on an ancestor. Recommended split: prepare data near the top of a View or Page with `<script env="server">`, and keep interactive behavior at the bottom of the View or in Script resources.

## ES Module + `importmap` Pattern

In multi-page sites, a common pattern is to declare an import map in the **Layout** `<head>` and boot the app from a `type="module"` script near the end of the **Page**:

```html
<!-- Layout head -->
<script type="importmap">
{
  "imports": {
    "vue": "https://cdn.jsdelivr.net/npm/vue@3/dist/vue.esm-browser.prod.js"
  }
}
</script>

<!-- Page footer -->
<script type="module">
import { createApp } from 'vue'
import { store } from '/scripts/layout.js'
createApp({ /* ... */ }).mount('#app')
</script>
```

Here `/scripts/layout.js` is a site **Script** resource. It can `export` symbols that other Pages later `import`.

::: tip No build tool required
Native browser ES Modules plus `importmap` are enough to organize a medium or large frontend, even without Webpack or Vite.
:::

## `type="module"` and Server-side CodeBlocks

On the server, a module script with `env="server"` can `import` other CodeBlocks and `export` values back to the template. See [Template Binding Syntax](../binding/#type-module).

## Related

- [Layout](../layout/) - a common place for `importmap`
- [Page](../page/)
- [Style](../css/)
- [k.site.scripts](../../api/site/js.md)
