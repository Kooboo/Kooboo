# Modules

> Menu: **Development → Modules**  
> Deep link: `/_Admin/development/modules?SiteId={siteGuid}`

**Modules** are installable Kooboo extensions: package **views, APIs, scripts, static assets**, and `module.config` in an isolated directory, expose capabilities via site routes, and can be edited in **dev mode**. Suitable for e-commerce plugins, form components, admin panels, and other reusable packages.

::: tip Permissions
List and dev mode: `module` (view). **Create**, import, online toggle, settings, export, share require `module·edit`; multi-select **Delete** requires `module·delete`.
:::

::: info Difference from site Code / View
| | **Modules** | **Development → Code / Views** |
|--|----------|-------------------------|
| Structure | Isolated directory (`view/`, `api/`, `code/`, etc.) + `module.config` | Single site resource records |
| Routes | Auto-register `/_ModuleName/{part}` | Per page / Code routes |
| Config | `module.config` + admin **Settings** dialog | No unified module config |
| Admin entry | `module.config` **`menu`** → site left menu (iframe) | None |
| Scripts | `k.module.*` (module context only) | `k.site.codes`, etc. |

Script API: **[k.module](../../api/module/index.md)**.
:::

## How to Open

1. In [Edit Menu](../navigation.md#edit-menu), enable **Development → Modules**.
2. Open **Development → Modules** in the left sidebar.

<DocImage src="/cms/development/modules-overview.png" alt="Modules list overview" width="1120" />

## Module list

### Toolbar

| Button | Permission | Description |
|------|------|------|
| **Create** | `module·edit` | [New module dialog](#new-module-dialog) |
| **Import** | `module·edit` | Upload module **zip** ([import dialog](#import-dialog)) |
| **Search** | `module·edit` | Kooboo **app store** online search and install |

<DocImage src="/cms/development/modules-toolbar.png" alt="Modules list toolbar" width="1120" />

### List table

| Column / action | Description |
|-----------|------|
| **Name** | Click name to edit in **dev mode** (`dev-mode?activity=modules&moduleId=...`) |
| **Online** | Toggle whether module is active (`ScriptModule/UpdateStatus`) |
| **Backend** | If module has `backend/` views, **Manage** button (different from **`menu` sidebar** below — [Mount site admin menu](#mount-site-admin-menu-menu)) |
| **Settings** | [Module settings dialog](#module-settings-dialog) (`settingDefines` from `module.config`) |
| **Readme** | Renders root `Readme.md` (Markdown) |
| **Export** | Download current module zip |
| **Share** | Share to Kooboo package repository (confirmation required) |

Supports multi-select **Delete**.

<DocImage src="/cms/development/modules-list-table.png" alt="Modules list" width="1120" />

## New module dialog

Enter **Name** (1–50 chars, letter or digit first). Creates module record and default directory (`module.config`, `Readme.md`, etc.); click name in list to continue in dev mode.

<DocImage src="/cms/development/modules-add-dialog.png" alt="New module dialog" width="1120" />

## Import dialog

| Field | Description |
|------|------|
| **Name** | Name after import; duplicate prompts **replace** confirmation |
| **File** | Module **zip** |

On success refreshes list; Monaco module cache cleared for new code.

<DocImage src="/cms/development/modules-import-dialog.png" alt="Import module dialog" width="1120" />

## Module settings dialog

**Settings** reads root **`module.config`** (JSON). `settingDefines` describes admin fields (`input`, `number`, `select`, `switch`, `textarea`, etc.); save writes site-side module **Settings** (runtime: `k.module.config`).

Same file's **`menu`** mounts module to site admin left menu (next section), unrelated to **Settings** dialog — edit `module.config` in dev mode.

When never saved, form initializes from `settingDefines` `defaultValue`.

<DocImage src="/cms/development/modules-setting-dialog.png" alt="Module settings dialog" width="1120" />

## Readme

**Readme** icon opens `Readme.md` Markdown preview for installers.

<DocImage src="/cms/development/modules-readme-dialog.png" alt="Module Readme" width="1120" />

## Edit in dev mode

Click list **Name** (or dev mode sidebar **Modules** activity). Left groups by resource type:

| Type | Directory | Purpose |
|------|------|------|
| **root** | Root files | `module.config`, `Readme.md`, `event.js`, task scripts, etc. (root files usually not deletable) |
| **view** | `view/` | Module **pages** (SSR/templates) |
| **api** | `api/` | Module **API** scripts (`ModuleApi:` reference) |
| **code** | `code/` | Reusable **scripts** (`module:` reference, e.g. `main` / `index`) |
| **css** / **js** | Static assets | Styles and scripts |
| **img** / **file** | Binary | Images and other files; upload supported |
| **backend** | `backend/` | Module **admin UI** (list **Manage** button) |

Text files edit in right **tabs** (Monaco); **Save**, **Refresh**, **Preview** for views, etc. **+** beside group title creates files; binary types support **Upload**.

<DocImage src="/cms/development/modules-devmode.png" alt="Dev mode: edit module files" width="1120" />

Dev mode top bar **More** returns to this list; **Return** exits current module file tree.

## Mount site admin menu (`menu`)

Add **`menu`** object in **`module.config`** to show module as its own entry in **site admin left menu** (not the same as list **Manage**). On site admin login, Kooboo scans **online** modules' `module.config`; modules with both **`name`** and **`menu`** inject into sidebar.

Clicking loads module page in main area via **iframe** (route `module-menu/:module`, query `url` = resolved module view address).

### Requirements

| Condition | Description |
|------|------|
| Module **online** | List **Online** toggle on |
| `module.config` has **`name`** | Package name, matches module resource name |
| **`menu` configured** | See table below |
| Refresh admin | After `menu` change, re-enter site admin or refresh to reload `moduleMenus` |

### `menu` fields

| Field | Required | Description |
|------|------|------|
| `name` | Yes | Sidebar display name |
| `nameTranslation` | No | Multilingual text, keys are language codes (e.g. `zh`, `en`) for current admin user language |
| `parent` | No | Under a **top-level admin menu**; value is that menu's `menu.name` in routes (e.g. `content`, `commerce`) — [menu overview](../navigation.md) |
| `url` | No | Module **view** relative path (e.g. `dashboard.html`); empty uses default start view |
| `icon` | No | Icon: usually filename under `img/`, resolved as `/_ModuleName/img/{filename}?SiteId=...` |
| `children` | No | Sub-menu array; each item has `name`, `nameTranslation`, `url` (relative view path) |

With `children`, sidebar shows expandable parent; each child row; child `url` resolves to `/_ModuleName/{path}?SiteId={siteGuid}`.

### Example config

```json
{
  "name": "my_shop_plugin",
  "version": "1.0.0",
  "description": "Shop extension",
  "menu": {
    "name": "Shop Plugin",
    "nameTranslation": {
      "zh": "店铺插件",
      "en": "Shop Plugin"
    },
    "parent": "commerce",
    "url": "admin/dashboard.html",
    "icon": "plugin-icon.svg",
    "children": [
      {
        "name": "Order sync",
        "url": "admin/orders.html"
      },
      {
        "name": "Inventory",
        "url": "admin/stock.html"
      }
    ]
  },
  "settingDefines": []
}
```

- Example adds **Shop Plugin** under **Commerce** (`parent: "commerce"`), default `view/admin/dashboard.html`.  
- `view/admin/*.html` can read **Settings** via `k.module.config`.  
- If `url` starts with `http://` or `https://`, module path is not prepended (external admin link).

### vs list **Manage** button

| | **`menu` sidebar** | List **Manage** |
|--|-----------------|---------------|
| Config | `module.config` → `menu` | Module `backend/` views + `backendViewUrl` from admin API |
| Entry | Daily site admin left menu | **Development → Modules** list row |
| Typical use | Operators using module features | Developers opening module backend during dev |

Both can coexist: operators use sidebar, developers use list **Manage** for `backend/`.

## Routes and access

On create, Kooboo registers routes like:

```text
/_ModuleName/{part}
```

`{part}` is view, api, static path, etc. (module renderer). On name conflict, system tries `ModuleName2`, `ModuleName3`, … until available.

In module scripts, current module root URL is **`k.module.baseUrl`** (module execution context only).

## Typical workflow

1. **Create** or **Import** / **Search** install.  
2. In **dev mode**, write `view/`, `api/`, `code/`, maintain `module.config`, `Readme.md`.  
3. For operator admin, configure **`menu`** and matching `view/` pages in `module.config`.  
4. Open **Settings** in list, fill `settingDefines` (maps to `k.module.config`).  
5. Turn **Online** on; refresh site admin for sidebar; verify front-end `/_ModuleName/...`.  
6. **Export** zip or **Share** to package repo for other sites.

## Related

| Doc | Description |
|------|------|
| [k.module](../../api/module/index.md) | `config`, `task`, `localFile`, `module.config` conventions |
| [Site admin menu overview](../navigation.md) | Top-level `name` values for `menu.parent` |
| [Code](./code.md) | Site-level CodeBlock / API |
| [Development overview](./index.md) | Development group index |
| [Development · Jobs](./jobs.md) | Site `k.site.runJob` (different from module `k.module.task`) |
