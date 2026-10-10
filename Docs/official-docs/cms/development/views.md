# Views

> Menu: **Development → Views**  
> Deep link: `/_Admin/development/views?SiteId={siteGuid}`

**Views** are reusable **HTML template fragments** in the site, referenced by [pages](../site/pages.md), [layouts](./layouts.md), etc. Maintain View name and template body in admin; syntax and rendering: [View template engine](../../templateEngine/view/index.md).

::: tip Permissions
Menu permission: `view` (view). Create, edit, delete, copy require `view·edit` / `delete`. Row **Version** requires `site·log`.
:::

## How to Open

1. In [Edit Menu](../navigation.md#edit-menu), enable **Development → Views**.
2. Open **Development → Views** in the left sidebar.

<DocImage src="/cms/development/views-overview.png" alt="Views list overview" width="1120" />

## Toolbar

| Button | Permission | Description |
|------|------|------|
| **New view** | `view·edit` | Enter [edit page](#edit-view) (new, empty `id`) |

<DocImage src="/cms/development/views-toolbar.png" alt="Views list toolbar: new view" width="1120" />

## List table

| Column | Description |
|----|------|
| **Name** | Click name to edit; name locked on edit page after save |
| **Referenced by** | Tags for pages, Layouts, etc. referencing this View |
| **Preview** | Relative preview path; click opens full preview URL in new tab |
| **Last modified** | Last save time |
| **Version** | [Version history](../operations/site-logs.md) (`site·log`) |

Supports multi-select **Delete** (`view·delete`). With **one row** selected, toolbar shows **Copy** ([copy view dialog](#copy-view-dialog)).

<DocImage src="/cms/development/views-list-table.png" alt="Views list table" width="1120" />

### Copy view dialog

Select one row, click **Copy**, enter new **Name** (default `_Copy` suffix), confirm to duplicate and refresh.

<DocImage src="/cms/development/views-copy-dialog.png" alt="Copy view dialog" width="1120" />

## Edit view

Path: `/_Admin/development/view/edit?SiteId=...` (new without `id`; edit with `&id={ViewId}`)

From list name or **New view**. Full-screen **development edit** (no left site menu): header properties and actions, code editor, save bar.

<DocImage src="/cms/development/views-edit-overview.png" alt="View edit: Monaco editor and footer" width="1120" />

### Header

| Control | Description |
|------|------|
| **View name** | Required on create, unique (1–50 chars, same naming as site resources); locked after save |
| **Settings** | Open [settings dialog](#settings-dialog) (page element, fields, cache, etc.) |
| **Format** | Format HTML in editor |

<DocImage src="/cms/development/views-edit-header.png" alt="View edit header: name, settings, format" width="1120" />

### Code editor

- Language: **HTML**, supports **KScript** and code references (`code-reference-type: view`).
- Shortcut: **Save** (same as footer save).
- Unsaved leave prompts.

### Footer

| Button | Description |
|------|------|
| **Return** | Back to view list |
| **Save** | Save; first save on new adds `id` to URL |
| **Save and return** | Save then return to list |

### Settings dialog

**Settings** on edit page; confirm writes to current edit model (footer **Save** still required to persist).

| Area | Description |
|------|------|
| **As page element** | When on, usable as draggable block in page designer |
| **Display name** / **Preview image** | Only when "as page element": name and cover in designer |
| **Fields** | When page element: field editor for designer-configurable properties (defaults, dynamic options, etc.) |
| **Enable cache** | When on: cache dimensions and duration |
| **Cache by device** | Separate cache per device type (hover tooltip explains rules) |
| **Cache by language** | Multilingual sites: per culture |
| **Cache by version** | **All** / **Self** / **Custom**; custom can check **Development**, **Content**, **Commerce**, etc. |
| **Cache time (minutes)** | Cache TTL |
| **Cache query keys** | Query string params in cache key (hover tooltip) |

<DocImage src="/cms/development/views-setting-dialog.png" alt="View settings: page element and cache" width="1120" />

## Related

| Doc | Description |
|------|------|
| [Development overview](./index.md) | Development menu and sub-items index |
| [Pages](../site/pages.md) | Page tree and View references |
| [Inline edit](../site/pages-inline-edit.md) | Inline HTML on pages |
| [View template engine](../../templateEngine/view/index.md) | View syntax and concepts |
| [Layouts](./layouts.md) | Page shell and `@RenderBody()` |
