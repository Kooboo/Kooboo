# Layouts

> Menu: **Development → Layouts**  
> Deep link: `/_Admin/development/layouts?SiteId={siteGuid}`

**Layouts** define the site **page shell** (HTML skeleton, `@RenderBody()` placeholder, shared header/footer). Pages bind a Layout in [page settings](../site/pages-setting.md) or page properties; template syntax: [Layout template engine](../../templateEngine/layout/index.md).

::: tip Permissions
Menu permission: `layout` (view). Create, edit, delete, copy require `layout·edit` / `delete`. Row **Version** requires `site·log`.
:::

## How to Open

1. In [Edit Menu](../navigation.md#edit-menu), enable **Development → Layouts**.
2. Open **Development → Layouts** in the left sidebar.

<DocImage src="/cms/development/layouts-overview.png" alt="Layouts list overview" width="1120" />

## Toolbar

| Button | Permission | Description |
|------|------|------|
| **New layout** | `layout·edit` | Enter [edit page](#edit-layout) (new, no `id`) |

<DocImage src="/cms/development/layouts-toolbar.png" alt="Layouts list toolbar: new layout" width="1120" />

## List table

| Column | Description |
|----|------|
| **Name** | Click to edit; name locked on edit page after save |
| **Referenced by** | Tags for pages etc. referencing this Layout |
| **Last modified** | Last save time |
| **Version** | [Version history](../operations/site-logs.md) (`site·log`) |

Unlike [Views](./views.md), the layout list has **no preview column** (preview on pages that bind the Layout).

Supports multi-select **Delete** (`layout·delete`). With **one row** selected, toolbar shows **Copy** ([copy layout dialog](#copy-layout-dialog)).

<DocImage src="/cms/development/layouts-list-table.png" alt="Layouts list table" width="1120" />

### Copy layout dialog

After selecting one row, click **Copy**, enter new **Name** (default `_Copy` suffix), confirm to duplicate and refresh list.

<DocImage src="/cms/development/layouts-copy-dialog.png" alt="Copy layout dialog" width="1120" />

## Edit layout

Path: `/_Admin/development/layout/edit?SiteId=...` (new without `id`; edit with `&id={LayoutId}`)

From list name or **New layout**. Full-screen **development edit** layout: header with **layout name** and **Format**, HTML editor, save bar. Layout edit has **no Settings** dialog (no "as page element", cache, etc.).

<DocImage src="/cms/development/layouts-edit-overview.png" alt="Layout edit: Monaco editor and footer" width="1120" />

### Header

| Control | Description |
|------|------|
| **Layout name** | Required on create, unique (1–50 chars); input disabled after save |
| **Format** | Format HTML in editor |

<DocImage src="/cms/development/layouts-edit-header.png" alt="Layout edit header: name and format" width="1120" />

### Code editor

- Language: **HTML**, supports **KScript** and code references (`code-reference-type: layout`).
- Typical body includes page placeholder (e.g. `@RenderBody()`); see [Layout template engine](../../templateEngine/layout/index.md).
- Shortcut: **Save**; unsaved leave prompts.

### Footer

| Button | Description |
|------|------|
| **Return** | Back to layout list |
| **Save** | Save; first save on new adds `id` to URL |
| **Save and return** | Save then return to list |

## Related

| Doc | Description |
|------|------|
| [Development overview](./index.md) | Development menu and sub-items index |
| [Views](./views.md) | Reusable View fragments |
| [Pages](../site/pages.md) | Page tree and Layout binding |
| [Page settings](../site/pages-setting.md) | Default page Layout, etc. |
| [Layout template engine](../../templateEngine/layout/index.md) | Layout syntax and concepts |
