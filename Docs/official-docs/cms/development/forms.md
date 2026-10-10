# Forms

> Menu: **Development → Forms**  
> Deep link: `/_Admin/development/forms?SiteId={siteGuid}`

Manage site **form** definitions and submission data: **external** forms can be created and edited here with HTML templates; **embedded** forms are created with pages/objects and viewed/configured here. Submissions are in [form data sub-page](#form-data-sub-page).

::: tip Permissions
Menu permission: `form` (view). **Create form**, edit, settings, delete submissions require `form·edit` / `form·delete` (per table/button). External list **Version** requires `site·log`.
:::

## How to Open

1. In [Edit Menu](../navigation.md#edit-menu), enable **Development → Forms**.
2. Open **Development → Forms** in the left sidebar.

Default **External** tab; switching tabs updates URL `name=external|embedded`.

<DocImage src="/cms/development/forms-overview.png" alt="Forms page overview (external tab)" width="1120" />

## Tabs

| Tab | query `name` | Description |
|------|----------------|------|
| **External** | `external` (default) | Standalone forms; **Create form** available |
| **Embedded** | `embedded` | Forms embedded in pages/modules; cannot create here |

<DocImage src="/cms/development/forms-tabs.png" alt="Forms tabs: external and embedded" width="1120" />

## External tab

### Toolbar

| Button | Permission | Description |
|------|------|------|
| **Create form** | `form·edit` | Enter [edit form](#edit-form) (new) |

<DocImage src="/cms/development/forms-external-toolbar.png" alt="External forms toolbar" width="1120" />

### List table

| Column | Description |
|----|------|
| **Name** | Click to [edit form](#edit-form) |
| **Data** | Number badge = submission count; click for [form data sub-page](#form-data-sub-page) |
| **Referenced by** | Reference relationships |
| **Last modified** | Save time |
| **Actions** | **Settings** ([form settings dialog](#form-settings-dialog)), **Version** (`site·log`) |

Supports multi-select **Delete** (`form·delete`).

<DocImage src="/cms/development/forms-external-table.png" alt="External forms list" width="1120" />

## Embedded tab

No **Create** button. Columns same as external (**Name** opens edit with `type=embedded`; edit page shows **Embedded form** and name is read-only). Row end only **Settings**, no **Version** icon.

<DocImage src="/cms/development/forms-embedded-table.png" alt="Embedded forms list" width="1120" />

### Form settings dialog

**Settings** on external/embedded list (**Form settings**):

| Item | Description |
|----|------|
| **Enable** | Whether submission handling is enabled for this form |
| **Method** | `get` / `post` |
| **Redirect to** | After submit: **Refresh page** (`RefreshSelf()`) or site [page](../site/pages.md) path, or manual entry |
| **Submit method** | Pick from `availableSubmitters` (e.g. email, external URL) |
| Submit method fields | e.g. **SubmitToExternal** needs receiver URL; other methods show matching dropdowns |

## Edit form

Path: `/_Admin/development/form/edit?SiteId=...` (new without `id`; edit with `id`; embedded with `type=embedded`)

<DocImage src="/cms/development/forms-edit-overview.png" alt="Form edit: HTML editor" width="1120" />

### Header

| Control | Description |
|------|------|
| **Form name** | Only **new external** forms: editable and unique; locked after save or for embedded |
| **Format** | Format HTML template |

<DocImage src="/cms/development/forms-edit-header.png" alt="Form edit header" width="1120" />

### Code editor

- Language: **HTML**, supports **KScript**.
- **Save** / **Save and return**; unsaved leave prompts.

## Form data sub-page

Path: `/_Admin/development/form-values?SiteId=...&id={FormId}`

Breadcrumb: **Forms → Data**. Table columns are **field names** from each submission (dynamic), with pagination and multi-select **Delete**.

## Related

| Doc | Description |
|------|------|
| [Development overview](./index.md) | Development menu index |
| [Pages](../site/pages.md) | Embedded form sources in pages |
| [URL](./urls.md) | Form submission paths |
