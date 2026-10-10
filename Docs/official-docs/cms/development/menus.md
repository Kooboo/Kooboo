# Menus

> Menu: **Development → Menus**  
> Deep link: `/_Admin/development/menus?SiteId={siteGuid}`

Manage site **navigation menu** definitions: create menu groups, maintain multi-level **menu items** (name, link, multilingual), and configure output HTML **templates** (placeholders for child items). After saving, reference in **Layout / Page / View** for the visitor site — see [Front-end usage](#front-end-usage).

::: info Not the admin left sidebar
This page is **visitor site** navigation data, unrelated to [Edit Menu](../navigation.md#edit-menu) (Kooboo admin feature tree).
:::

::: tip Permissions
Menu permission: `menu` (view). **New menu**, edit items, drag sort, delete items, change template require `menu·edit`; batch delete menus require `menu·delete`. Row **Version** requires `site·log`.
:::

## How to Open

1. In [Edit Menu](../navigation.md#edit-menu), enable **Development → Menus**.
2. Open **Development → Menus** in the left sidebar.

<DocImage src="/cms/development/menus-overview.png" alt="Menus list overview" width="1120" />

## Menu list

### Toolbar

| Button | Permission | Description |
|------|------|------|
| **New menu** | `menu·edit` | Open [new menu dialog](#new-menu-dialog) |

<DocImage src="/cms/development/menus-toolbar.png" alt="Menus list toolbar" width="1120" />

### List table

| Column | Description |
|----|------|
| **Name** | Click to [edit menu](#edit-menu) |
| **Referenced by** | Reference relationships |
| **Last modified** | Save time |
| **Version** | [Version history](../operations/site-logs.md) |

Supports multi-select **Delete** entire menu (`menu·delete`).

<DocImage src="/cms/development/menus-list-table.png" alt="Menus list" width="1120" />

### New menu dialog

Enter **Name** (required, unique, 1–50 chars), confirm to create and return to list (click name to edit).

<DocImage src="/cms/development/menus-add-dialog.png" alt="New menu dialog" width="1120" />

## Edit menu

Path: `/_Admin/development/menu/edit?SiteId=...&id={MenuId}`

Breadcrumb: **Menus → {menu name}**. Card header shows menu name; right **Edit template** opens root [menu template dialog](#menu-template-dialog) (same as template button on items with children).

<DocImage src="/cms/development/menus-edit-overview.png" alt="Edit menu: menu item tree" width="1120" />

### Menu item tree

- Top and child levels as **draggable** cards: left **Name** and **URL**, right action icons.
- **Drag** (`menu_move_handler`) reorders siblings; save calls `swap` API.
- No children: **Add** child, **Edit**, **Delete**.
- With children: **Edit template** (sub-menu container), **Add** child, **Edit**, **Delete**; bottom **+** adds sibling.

If no items yet, page prompts **Create now** for the first item.

<DocImage src="/cms/development/menus-edit-items.png" alt="Menu items list and actions" width="1120" />

### Menu item dialog

**Add menu item** or row **Edit** (title **Menu item**):

| Field | Description |
|------|------|
| **Name** | Multilingual sites: multiple **Name** rows per [multilingual](../site/pages.md) config; default language is primary name |
| **URL** | Dropdown of **page** routes (`routesByType('Page')`), filterable or manual; empty saves as `#` |

Top-right **language selector** switches language column being edited.

<DocImage src="/cms/development/menus-item-dialog.png" alt="Menu item dialog: name and URL" width="1120" />

### Menu template dialog

**Edit template** (title **Edit template**). Defines HTML output for items (including children):

| Area | Description |
|------|------|
| Top / bottom container | HTML fragments above/below `{items}` in `subItemContainer` |
| Middle template | `subItemTemplate`, HTML for one menu item |
| Placeholder tags | Click to insert: `AnchorText` → `{anchortext}`, `Href` → `{href}`, `SubItems` → `{items}`, `ActiveClass` → `{activeclass:className}`, `ParentId` → `{parentid}`, `CurrentId` → `{currentid}` |
| **Preview** | Right side live HTML from current template and menu data |

<DocImage src="/cms/development/menus-template-dialog.png" alt="Menu template edit and preview" width="1120" />

## Front-end usage

After saving in admin, use one of the following in shared **Layout** or **Page / View** that needs navigation (no separate "enable" switch).

### `<menu>` component (uses templates from this section)

Insert **Menu** component in HTML; `id` is the menu **Name** from the table (e.g. `main`). Output structure follows `{href}`, `{anchortext}`, `{items}`, etc. from [menu template dialog](#menu-template-dialog); current page highlight is automatic.

```html
<menu id="main"></menu>
```

Optional `menulevel` limits depth (e.g. `menulevel="2"`). Dragging **Menu** from designer component panel is equivalent.

### `k.site.menus` + `k-for` (custom structure)

Read the menu in `<script env="server">`, then build CSS-specific navigation HTML with the template binding `k-for` directive.

```html
<script env="server">
    var nav = k.site.menus.get("main");
    var items = nav && nav.children ? nav.children : [];
</script>
<ul>
    <li k-for="item in items">
        <a k-attribute="href {item.url}" k-content="item.name"></a>
    </li>
</ul>
```

Methods, `createSubMenu`, multilingual `getName`, etc.: **[k.site.menus](../../api/site/menu.md)**. Do not use deprecated `k-query` / `v-query` for menus.

### Script maintenance (optional)

Install wizards, migrations, etc. may call `k.site.menus.create`, `menu.createSubMenu` in Code / `k.api`; day-to-day ops should use this page.

## Related

| Doc | Description |
|------|------|
| [k.site.menus](../../api/site/menu.md) | KScript API: `get`, `list`, item maintenance, front-end examples |
| [Template binding](../../templateEngine/binding/index.md) | `k-for`, `k-content`, `k-attribute` |
| [Development overview](./index.md) | Development menu index |
| [Pages](../site/pages.md) | Menu item URLs often point to pages |
| [URL](./urls.md) | Site paths and routing |
| [Edit Menu](../navigation.md#edit-menu) | Admin left feature tree (not this doc) |
