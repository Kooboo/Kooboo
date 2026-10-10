# Styles

> Menu: **Development → Styles**  
> Deep link: `/_Admin/development/styles?SiteId={siteGuid}`

Manage site **CSS** resources: standalone `.css` files (**external**), **embedded styles** from pages/layouts, **inline style rules** from the designer, and **style groups** merging multiple external styles. References: [Style template engine](../../templateEngine/css/index.md).

::: tip Permissions
List and edit: `style` (view / `edit` / `delete`). Tab bar **Settings** (site compress, UnoCSS, media split, etc.) requires `site·edit`. Row **Version** requires `site·log`.
:::

## How to Open

1. In [Edit Menu](../navigation.md#edit-menu), enable **Development → Styles**.
2. Open **Development → Styles** in the left sidebar.

<DocImage src="/cms/development/styles-overview.png" alt="Styles page: external tab overview" width="1120" />

## Tab bar and site settings

| Tab | query `name` | Description |
|------|----------------|------|
| **External** | `external` (default) | Standalone CSS; create, upload, online search |
| **Embedded** | `embedded` | CSS saved with pages, Layouts, etc.; edit and delete only |
| **Inline styles** | `inline` | CSS rule blocks from page designer; edit declarations in dialog |
| **Group** | `group` | Merge multiple external styles into one reference |

Tab bar right **Settings** opens [style site settings dialog](#style-site-settings-dialog).

<DocImage src="/cms/development/styles-tabs.png" alt="Style tabs and settings" width="1120" />

## External tab

### Toolbar

| Button | Permission | Description |
|------|------|------|
| **New style** | `style·edit` | Enter [edit style](#edit-style) |
| **Upload style** | `style·edit` | Upload `text/css` file; confirm overwrite on duplicate |
| **Search** | `style·edit` | Online package search; refresh list after install |

<DocImage src="/cms/development/styles-external-toolbar.png" alt="External styles toolbar" width="1120" />

### List table

| Column | Description |
|----|------|
| **Name** | Enter edit |
| **Preview** | Route path; open style URL in new tab |
| **Referenced by** | Reference relationships |
| **Last modified** | Save time |
| **Version** | [Version history](../operations/site-logs.md) |

Supports multi-select **Delete**.

<DocImage src="/cms/development/styles-external-table.png" alt="External styles list" width="1120" />

### Style site settings dialog

**Settings** saves site-level style delivery and toolchain options:

| Area | Field / description |
|------|-------------|
| **JS/CSS compress** | `enableJsCssCompress` |
| **JS/CSS browser cache** | `enableJsCssBrowerCache` |
| **Atomic CSS (unocss)** | `unocssSettings.enable`; when on: **Disable SSR**, **Reset default styles**, JSON **Config** (must be valid JSON) |
| **Split CSS by media query** | `enableCssSplitByMedia`; when on: mobile **max width**, desktop **min width** (split/trim media rules) |

<DocImage src="/cms/development/styles-settings-dialog.png" alt="Style site settings dialog" width="1120" />

## Embedded tab

Lists **embedded styles**. No create toolbar; click name to edit. Edit page shows **Embedded style**, name not editable.

| Column | Description |
|----|------|
| **Name** | Enter edit |
| **Referenced by** | Owning object |
| **Last modified** | Save time |

<DocImage src="/cms/development/styles-embedded-table.png" alt="Embedded styles list" width="1120" />

## Inline styles tab

Manage **CSS rules** (selector + declarations), often from visual editing. Click **Style** column name for [inline style edit dialog](#inline-style-edit-dialog).

| Column | Description |
|----|------|
| **Style** | Rule name/selector summary, click to edit |
| **Owner type** | e.g. Page, View (`ownerType`) |
| **Site object** | Owning object name (`ownerName`) |
| **Last modified** | Save time |

<DocImage src="/cms/development/styles-inline-table.png" alt="Inline styles list" width="1120" />

### Inline style edit dialog

Edit multiple **property : value** pairs as `{ ... }`:

| Action | Description |
|------|------|
| Property dropdown | Common CSS properties, filterable or custom |
| Value input | Property value; color picker for color properties; **media library** for background images |
| **+** | Add declaration |
| Delete | Remove current row |

Confirm writes rule and refreshes list.

<DocImage src="/cms/development/styles-inline-dialog.png" alt="Inline style edit dialog" width="1120" />

## Style group tab

Merge multiple **external** styles into one resource group (`typeName: Style`).

### Toolbar

| Button | Permission | Description |
|------|------|------|
| **New group** | `style·edit` | Open [style group dialog](#style-group-dialog) |

<DocImage src="/cms/development/styles-group-toolbar.png" alt="Style groups toolbar" width="1120" />

### List table

| Column | Description |
|----|------|
| **Name** | Open group dialog |
| **Items** | Style count in group |
| **Preview** | Group preview URL |
| **Referenced by** | Reference relationships |
| **Last modified** | Save time |

<DocImage src="/cms/development/styles-group-table.png" alt="Style groups list" width="1120" />

### Style group dialog

| Area | Description |
|------|------|
| **Name** | Required on create, unique; locked after save |
| **Add style** | Pick external styles not yet in group |
| **List** | Styles in group; sort, remove |

<DocImage src="/cms/development/styles-group-dialog.png" alt="Style group dialog" width="1120" />

## Edit style

Path: `/_Admin/development/style/edit?SiteId=...`

From **External** **New style** or name click.

<DocImage src="/cms/development/styles-edit-overview.png" alt="Style edit: CSS editor" width="1120" />

### Header (external style)

| Control | Description |
|------|------|
| **Style name** | Required on create, unique; disabled after save |
| **Format** | Format CSS (shortcut available) |

Embedded: **Embedded style** title + **Format** only.

<DocImage src="/cms/development/styles-edit-header.png" alt="Style edit header" width="1120" />

### Code editor

- Language: **CSS**, supports **KScript**; external styles support code references (`code-reference-type: style`).
- **Save** / **Save and return** / unsaved leave prompt.

## Related

| Doc | Description |
|------|------|
| [Development overview](./index.md) | Development menu index |
| [Scripts](./scripts.md) | Site JS, similar tab structure |
| [Layouts](./layouts.md) | Can produce embedded styles |
| [Style template engine](../../templateEngine/css/index.md) | References and syntax |
