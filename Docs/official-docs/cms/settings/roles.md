# Roles

> Menu: **Settings -> Roles** (advanced menu)  
> Deep link: `/_Admin/system/roles?SiteId={siteGuid}`

**Roles** defines which admin **features** and **actions** (view / edit / delete, etc.) each **site role** may use. After assigning roles on [Site Users](./site-users.md), menu visibility, buttons, and API calls follow this configuration.

::: tip Permissions
| Action | Permission |
|--------|------------|
| View role list | `role` |
| **Add role**, edit permissions, **Save** | `role·edit` |
| Multi-select **Delete** custom roles | `role·delete` |
:::

::: info Advanced menu
Route `advanced: true`; besides enabling **Settings**, **Roles** must appear in [Edit Menu](../navigation.md#permissions-and-edit-menu) (or use `master` / full-access roles).
:::

## How to Open

1. In [Edit Menu](../navigation.md#permissions-and-edit-menu), enable **Settings -> Roles**.  
2. Open **Settings -> Roles** in the left sidebar.

<DocImage src="/cms/settings/settings-roles-overview.png" alt="Roles" width="1120" />

## Toolbar

| Control | Description |
|---------|-------------|
| **Add role** | Opens [add role dialog](#add-or-edit-role) for custom role and permissions |

<DocImage src="/cms/settings/settings-roles-toolbar.png" alt="Roles toolbar" width="1120" />

## Role List

| Column | Description |
|--------|-------------|
| **Role name** | `name` as tag |
| **Edit** | Pencil opens permission dialog |

List includes **built-in** and **custom** roles, newest first. Built-ins **`master`**, **`developer`**, **`contentmanager`** cannot be multi-selected for delete, but **Edit** and save to site `RolePermission` store still work.

<DocImage src="/cms/settings/settings-roles-list.png" alt="Role list" width="1120" />

### Built-in Roles (Overview)

| Role | Description |
|------|-------------|
| **master** | All registered features by default (full selection) |
| **developer** | Dev features: pages, Code, database, integrations, Hooks, etc. |
| **contentmanager** | Content and commerce ops: content, media, products, etc. |

Exact feature sets per Kooboo version `PermissionService.EmbeddedRoles`; **Edit** in admin to view or adjust.

### Delete Restrictions

- **Built-in roles** cannot be deleted.  
- **Custom roles** referenced by [Site Users](./site-users.md) `SiteRole` fail delete (*The role is in use and cannot be deleted*).

## Add or Edit Role

<DocImage src="/cms/settings/settings-roles-add-dialog.png" alt="Add role dialog" width="1120" />

Editing shows **Edit role**; **Role name** is **disabled** (no rename). New roles need unique name (1–50 chars, not conflicting with built-ins).

<DocImage src="/cms/settings/settings-roles-edit-dialog.png" alt="Edit role permissions" width="1120" />

### Permission Panel

| Area | Description |
|------|-------------|
| **Select all** | Toggle all permissions for role |
| **Feature column** | Each `feature` (e.g. `pages`, `content`, `domain`); labels from admin i18n |
| **Single checkbox** | Features with one action |
| **Dropdown features** | Label opens **View / Edit / Delete** (and `debug`, etc.); per-feature select all |

Checking **Edit**, **Delete**, or **debug** auto-checks **View** for that feature; **View** cannot be unchecked while sub-items remain selected.

Save: `Role/post` → `RolePermissionRepository`.

## Features and Admin Menus

Permissions from `PermissionService.GetList()` aggregating API `[Permission]` attributes; new versions may add features. Examples:

| Feature (partial) | Admin capability |
|-------------------|------------------|
| `pages` | Page design and routing |
| `content` / `contentType` | Content and content types |
| `database` | Database menu |
| `config` | [Integrations](./integrations/) |
| `domain` | [Domains](./domains.md) |
| `siteUser` / `role` | [Site Users](./site-users.md), this page |
| `frontEvents` / `backendEvents` | [Request Hooks](./request-hooks.md), [Action Hooks](./action-hooks.md) |
| `commerce*` series | Commerce submenus |

Left menu also filtered by front-end role presets (ops/designer/developer); API and buttons follow **Permissions** here.

## Relationship to Site Users

| | **Roles (this page)** | [Site Users](./site-users.md) |
|--|----------------------|-------------------------------|
| Answers | What a role **can do** | **Who** has the role |
| Order | Usually **define** roles first | **Then** add members and assign roles |

## Typical Workflow

1. Review built-in `developer` / `contentmanager`; **Edit** if needed.  
2. If no preset fits, **Add role** (e.g. `content-editor`), check features, **Save**.  
3. On [Site Users](./site-users.md), add collaborators and assign roles.  
4. Have them log in; adjust permissions here if blocked.  
5. Before deleting a role, reassign or remove all [Site Users](./site-users.md) references.

## Related

| Doc | Description |
|-----|-------------|
| [Settings overview](./index.md) | Menu overview |
| [Site Users](./site-users.md) | Members and role binding |
| [Edit Menu](../navigation.md#permissions-and-edit-menu) | Menu visibility and advanced items |
