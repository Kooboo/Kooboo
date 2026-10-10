# Site Users

> Menu: **Settings -> Site Users**  
> Deep link: `/_Admin/system/siteuser?SiteId={siteGuid}`

**Site Users** adds members from the current **Organization** to this site and assigns **site roles**. Added users can log into Kooboo admin and open this site; menus and actions follow **feature** permissions from [Roles](./roles.md).

::: tip Permissions
| Action | Permission |
|--------|------------|
| View user list | `siteUser` |
| **Add user** | `siteUser·edit` |
| Multi-select **Delete** | `siteUser·delete` |
:::

::: info Organization admins
**Organization admins** (`Organization.AdminUser`) manage all org sites by default—they do **not** appear in **Add user** dropdown and usually not in this list. Other org members must be added here to access this site admin (`WebsiteType.m` sites).
:::

## How to Open

1. In [Edit Menu](../navigation.md#permissions-and-edit-menu), enable **Settings -> Site Users**.  
2. Open **Settings -> Site Users** in the left sidebar.

<DocImage src="/cms/settings/settings-site-users-overview.png" alt="Site Users" width="1120" />

## Toolbar

| Control | Description |
|---------|-------------|
| **Add user** | Opens [add user dialog](#add-user); pick org member not yet on site and assign role |

<DocImage src="/cms/settings/settings-site-users-toolbar.png" alt="Site Users toolbar" width="1120" />

## User List

| Column | Description |
|--------|-------------|
| **Username** | Org account `userName` |
| **Email** | `email` |
| **Role** | Site `SiteRole` tag (e.g. `master`, `developer`, custom name) |

Multi-select **Delete** removes member from this site (`SiteUser/Deletes`); does **not** delete org account.

<DocImage src="/cms/settings/settings-site-users-list.png" alt="Site Users list" width="1120" />

Current UI has **no inline role edit**; delete and re-add, or update via API/script (below).

## Add User

<DocImage src="/cms/settings/settings-site-users-add-dialog.png" alt="Add user dialog" width="1120" />

| Field | Description |
|-------|-------------|
| **Available users** | Org members **not yet on this site** (`username<email>`); org admins excluded |
| **Role** | Site roles: `RolePermission` custom roles plus built-in **master**, **developer**, **contentmanager** |

Confirm calls `SiteUser/AddUser` with `UserId` and `SiteRole`; refreshes list.

## Roles and Menus

| Role type | Description |
|-----------|-------------|
| **Built-in** | `master` (full), `developer`, `contentmanager`; see `PermissionService.EmbeddedRoles` |
| **Custom** | Created in [Roles](./roles.md) with feature/action checkboxes |

After login, left menu is filtered by role presets; API and buttons follow role **Permissions**.

## Relationship to Roles

| | **Site Users (this page)** | [Roles](./roles.md) |
|--|---------------------------|---------------------|
| Role | **Who** is on the site with which **role name** | What each **role** may access |
| Typical ops | Add collaborators, remove leavers | Create “content editor” with `content`, etc. |

Configure roles first, then assign members here.

## Typical Workflow

1. Invite or create org members.  
2. Confirm or create roles in [Roles](./roles.md).  
3. **Add user** here with member and role.  
4. Have them log in; verify menus and key actions (publish, domain edit).  
5. On departure, **Delete** site user record.

## Script Side (Optional)

In Code, **`k.site.user.addUser`** can add users programmatically (user must exist in org):

```javascript
k.site.user.addUser({
  nameOrId: 'collaborator',
  role: 'developer'
})
```

Site package import/export may include `siteuser` entries; sync scenarios see Operations docs.

## Related

| Doc | Description |
|-----|-------------|
| [Settings overview](./index.md) | Menu overview |
| [Roles](./roles.md) | Roles and feature permissions |
| [Edit Menu](../navigation.md#permissions-and-edit-menu) | Left menu visibility |
