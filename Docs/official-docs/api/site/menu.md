# k.site.menus

> Reading and maintenance of the site's front-end navigation menu (the same set of data as the back-end **Development → Menu**)

## Overview

`k.site.menus` manages Kooboo **Menu** resources: menu groups, multi-level menu items created in the backend [Develop -> Menus](../../cms/development/menus.md), and HTML templates for output. The definition itself **will not** automatically appear on the page and must be referenced through this API or the `<menu>` component in **Layout / Page / View**.

::: tip Difference from "Edit Menu"
| object | Entrance | effect |
|------|------|------|
| **Develop → Menu** (this article API) | `/_Admin/development/menus` | Site navigation as seen by visitors |
| **Edit Menu** | User menu in the upper right corner | Control whether the function tree on the left side of Kooboo **Backstage** is displayed |

See [Site Admin Menu Overview](../../cms/navigation.md#edit-menu) for details.
:::

::: warning Difference from k.site.pages
| API | Purpose |
|-----|------|
| `k.site.menus` | Navigation menu tree (name, URL, sub-items, rendering template) |
| `k.site.pages` | Accessible page resources and routes |
:::

Menu item URLs usually point to a [Pages](../../cms/site/pages.md) route; multilingual names are maintained by language in the background and read with `getName(culture)` on the script side.

## TypeScript Definition

```ts
interface KMenus {
  get(nameOrId: string): Menu;
  create(name: string): Menu;
  list(): Menu[];
  remove(id: string): void;
}

interface Menu {
  id: string;
  name: string;
  url: string;
  source: string;
  children: Menu[];
  getName(culture: string): string;
  createSubMenu(name: string, url: string): void;
  updateSubMenu(
    subMenuId: string,
    name: string,
    url: string,
    culture?: string
  ): void;
  removeSubMenu(subMenuId: string): void;
}
```

## get()

Get the root menu object (with `children` tree) by menu **Name** or **Id** (GUID string).

| Parameter | Type | Required | Description |
|------|------|------|------|
| `nameOrId` | `string` | yes | Root menu name or ID |

**Returns:** `Menu | null`.

```ts
k.api.get(() => {
    const menu = k.site.menus.get("main")
    if (!menu) return null
    return {
        name: menu.name,
        topLevel: (menu.children || []).map((c) => ({
            name: c.name,
            url: c.url
        }))
    }
})
```

::: tip
`nameOrId` must be consistent with the **name** in the background list. The behavior when the menu does not exist depends on the runtime implementation. Please confirm that it has been created in the background before calling.
:::

## list()

Returns an array of all root menu objects of the site (excluding flat lists other than individually expanded subtrees).

**Parameters:** None.

**Returns:** `Menu[]`.

```ts
k.api.get(() => {
    const menus = k.site.menus.list()
    return { count: menus.length, names: menus.map((m) => m.name) }
})
```

## create()

Creates a new root menu; throws exception if name already exists.

| Parameter | Type | Required | Description |
|------|------|------|------|
| `name` | `string` | yes | Root menu name |

**Returns:** `Menu`.

```ts
k.api.post(() => {
    const name = "doc-menu-" + Date.now()
    const menu = k.site.menus.create(name)
    return { id: menu.id, name: menu.name }
})
```

## remove()

Delete the entire menu by root menu **Id** (GUID string).

| Parameter | Type | Required | Description |
|------|------|------|------|
| `id` | `string` | yes | Root menu ID |

**Returns:** `void`.

```ts
k.api.post(() => {
    const menu = k.site.menus.create("doc-menu-del")
    k.site.menus.remove(menu.id)
    return { removed: true }
})
```

## Menu example

For objects returned by `get()` / `create()` / `list()`, fields can be read and subkeys maintained at runtime (written back to the site database).

| Member | Description |
|------|------|
| `id` | Current node ID |
| `name` | Display name (default language) |
| `url` | Link; root menu item is usually `#` or empty |
| `children` | Array of submenu items |
| `source` | Fixed to `"menu"`, used for data tracking |
| `getName(culture)` | Display name in the specified language; fallback to `name` when there is no translation |

### createSubMenu(name, url)

Add a new sub-item under the current node and save it.

| Parameter | Type | Required | Description |
|------|------|------|------|
| `name` | `string` | yes | Submenu display name |
| `url` | `string` | yes | Submenu link |

**Returns:** `void`.

```ts
k.api.post(() => {
    const root = k.site.menus.get("main")
    root.createSubMenu("New Section", "/new-section")
    return { childCount: root.children.length }
})
```

### updateSubMenu(subMenuId, name, url, culture?)

Updates the name or URL by subkeyId; passing in `culture` only updates `Values` for that language, and also updates `name` for the default language.

| Parameter | Type | Required | Description |
|------|------|------|------|
| `subMenuId` | `string` | yes | Submenu ID |
| `name` | `string` | yes | New display name |
| `url` | `string` | yes | New link |
| `culture` | `string` | no | Culture code |

**Returns:** `void`.

```ts
k.api.post(() => {
    const root = k.site.menus.get("main")
    const child = root.children[0]
    root.updateSubMenu(child.id, "Updated", "/updated")
    return { updated: true }
})
```

### removeSubMenu(subMenuId)

Delete a submenu item by subitemId.

| Parameter | Type | Required | Description |
|------|------|------|------|
| `subMenuId` | `string` | yes | Submenu ID |

**Returns:** `void`.

```ts
k.api.post(() => {
    const root = k.site.menus.get("main")
    const child = root.children[0]
    root.removeSubMenu(child.id)
    return { removed: true }
})
```

## How to use the front desk

After the menu is configured in the background, choose any method in the template to output it (can be combined: components for the top bar and scripts for the footer).

### Render with the `<menu>` Component

Insert the Kooboo **Menu** component in **Layout** or **Page**: the tag name is `menu`, and `id` is the background menu **name**. Use the HTML templates and placeholders (`{href}`, `{anchortext}`, `{items}`, etc.) configured in [Develop -> Menus](../../cms/development/menus.md) when rendering, and automatically handle current-page highlighting and multilingual URLs.

```html
<!-- Render the full menu named main (no depth limit by default) -->
<menu id="main"></menu>

<!-- Render only the first 2 levels -->
<menu id="main" menulevel="2"></menu>
```

Dragging **Menu** into the components panel of the layout/page designer has the same effect. `id` On error or when the menu does not exist, the output at this location is empty.

### Render with `k.site.menus` and `k-for`

Read the menu in the server script and use [Template Binding Syntax](../../templateEngine/binding/index.md)'s `k-for` to render it. The structure is completely controlled by the template.

```html
<script env="server">
    var nav = k.site.menus.get("main");
    var items = nav && nav.children ? nav.children : [];
</script>
<nav class="site-nav">
    <ul>
        <li k-for="item in items">
            <a k-attribute="href {item.url}" k-content="item.name"></a>
        </li>
    </ul>
</nav>
```

Multi-level submenus can cover `item.children` with another layer of `k-for`, or split it into [View](../../templateEngine/view/index.md) fragments for recursive reference.

Example of multilingual display name:

```html
<script env="server">
    var culture = k.site.multilingual.currentCulture;
    var nav = k.site.menus.get("main");
    var items = (nav && nav.children ? nav.children : []).map(function (c) {
        return { name: c.getName(culture), url: c.url };
    });
</script>
<ul>
    <li k-for="item in items">
        <a k-attribute="href {item.url}" k-content="item.name"></a>
    </li>
</ul>
```

### Method 3: Maintain menu in Code/API

Calling `create`, `createSubMenu`, etc. in CodeBlock or `k.api` is suitable for installation wizards and migration scripts; for daily navigation, it is still recommended to maintain it in the background **Develop → Menu**.

## Related Docs

- [k.site](./) — Overview of site resources
- [Menus (CMS)](../../cms/development/menus.md) — Create menus, menu items, HTML templates, and permissions in the admin UI
- [Template Binding Syntax](../../templateEngine/binding/index.md) — `<script env="server">`, `k-for`, `k-content`
- [k.site.pages](./page.md) — Page resources that menu items often link to
- [Develop Overview](../../cms/development/index.md) — Development grouping and menu entry
