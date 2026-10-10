# Scripts

> Menu: **Development → Scripts**  
> Deep link: `/_Admin/development/scripts?SiteId={siteGuid}`

Manage site **JavaScript** resources: standalone `.js` files (**external**), **embedded scripts** from pages/layouts, and **script groups** merging multiple external scripts. Syntax and references: [Script template engine](../../templateEngine/js/index.md).

::: tip Permissions
List and edit: `script` (view / `edit` / `delete`). Tab bar right **Settings** (site JS/CSS compress and browser cache) requires `site·edit`. Row **Version** requires `site·log`.
:::

## How to Open

1. In [Edit Menu](../navigation.md#edit-menu), enable **Development → Scripts**.
2. Open **Development → Scripts** in the left sidebar.

<DocImage src="/cms/development/scripts-overview.png" alt="Scripts page: external tab overview" width="1120" />

## Tab bar and site settings

| Tab | Description |
|------|------|
| **External** | Standalone scripts under site root; create, upload, online search install |
| **Embedded** | Scripts saved with pages, Layouts, etc.; edit body and delete only, cannot create here |
| **Group** | Combine multiple external scripts into one reference ([script groups](#group-tab)) |

Tab bar right **Settings** opens [script site settings dialog](#script-site-settings-dialog) (site config, not per-script).

<DocImage src="/cms/development/scripts-tabs.png" alt="Script tabs: external, embedded, group, settings" width="1120" />

## External tab

Default **External** tab.

### Toolbar

| Button | Permission | Description |
|------|------|------|
| **New script** | `script·edit` | Enter [edit script](#edit-script) (new) |
| **Upload script** | `script·edit` | Upload local `.js` etc.; confirm overwrite on duplicate name |
| **Search** | `script·edit` | Online package search; install community/template scripts and refresh list |

<DocImage src="/cms/development/scripts-external-toolbar.png" alt="External scripts toolbar" width="1120" />

### List table

| Column | Description |
|----|------|
| **Name** | Click to edit |
| **Preview** | Route path; click opens script URL in new tab |
| **Referenced by** | Reference tags |
| **Last modified** | Save time |
| **Version** | [Version history](../operations/site-logs.md) |

Supports multi-select **Delete**.

<DocImage src="/cms/development/scripts-external-table.png" alt="External scripts list" width="1120" />

### Script site settings dialog

**Settings** → two site toggles (affects site-wide script/style delivery):

| Toggle | Field | Description |
|------|------|------|
| **JS/CSS compress** | `enableJsCssCompress` | Compress JS/CSS on publish |
| **JS/CSS browser cache** | `enableJsCssBrowerCache` | Enable browser cache policy |

<DocImage src="/cms/development/scripts-settings-dialog.png" alt="Script site settings: compress and browser cache" width="1120" />

## Embedded tab

Example path: `/_Admin/development/scripts?SiteId=...&name=embedded`

Lists **embedded scripts**. No create/upload toolbar; click **Name** to edit. Edit header shows **Embedded script**, **no** editable name field (name from owning page/resource).

| Column | Description |
|----|------|
| **Name** | Enter edit |
| **Referenced by** | Owning page, etc. |
| **Last modified** | Save time |

<DocImage src="/cms/development/scripts-embedded-table.png" alt="Embedded scripts list" width="1120" />

## Group tab

Example path: `/_Admin/development/scripts?SiteId=...&name=group`

Merge multiple **external** scripts into one resource group; pages reference the group to load scripts in order.

### Toolbar

| Button | Permission | Description |
|------|------|------|
| **New group** | `script·edit` | Open [script group dialog](#script-group-dialog) |

<DocImage src="/cms/development/scripts-group-toolbar.png" alt="Script groups toolbar" width="1120" />

### List table

| Column | Description |
|----|------|
| **Name** | Click to open group dialog |
| **Items** | Script count in group |
| **Preview** | Group preview URL |
| **Referenced by** | Reference relationships |
| **Last modified** | Save time |

<DocImage src="/cms/development/scripts-group-table.png" alt="Script groups list" width="1120" />

### Script group dialog

New or edit group:

| Area | Description |
|------|------|
| **Name** | Required on create, unique; locked after save |
| **Add script** | Dropdown of external scripts not yet in group (name and path) |
| **List** | Scripts in group; drag sort, remove items |

Confirm saves group and refreshes list.

<DocImage src="/cms/development/scripts-group-dialog.png" alt="Script group dialog: name and script list" width="1120" />

## Edit script

Path: `/_Admin/development/script/edit?SiteId=...`

From **External** **New script** or name click. Full-screen development edit layout.

<DocImage src="/cms/development/scripts-edit-overview.png" alt="Script edit: JavaScript editor" width="1120" />

### Header (external script)

| Control | Description |
|------|------|
| **Script name** | Required on create, unique (1–64 chars); disabled after save |
| **Format** | Format JavaScript |

Embedded shows **Embedded script** title and **Format** only ([embedded tab](#embedded-tab)).

<DocImage src="/cms/development/scripts-edit-header.png" alt="Script edit header" width="1120" />

### Code editor

- Language: **JavaScript**, supports **KScript**; external scripts support code references.
- **Save** / **Save and return** / unsaved leave prompt, same as [Layouts](./layouts.md) edit.

## Related

| Doc | Description |
|------|------|
| [Development overview](./index.md) | Development menu index |
| [Styles](./styles.md) | Site CSS (includes inline styles tab) |
| [Layouts](./layouts.md) | Can produce embedded scripts |
| [Script template engine](../../templateEngine/js/index.md) | References and syntax |
