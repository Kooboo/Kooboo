# Site Admin Menu Overview

> Organized by the Kooboo site-admin left sidebar. UI labels follow the language of your logged-in admin.

::: tip Complete outer steps first
If you are still on **My Sites** (`/_Admin/`), read [Login and site list](./getting-started/login-and-site-list.md) first, click **Manage** on a site, then use the menus below.
:::

## Account Level (No SiteId)

| Page | Path |
|------|------|
| My Sites | `/` |
| Login | `/login` |
| Create Site | `/create` |
| Import Site | `/create/import` |
| Clone Site | `/create/clone` |
| List inside folder | `/?currentFolder={folderName}` |

See [Login and site list](./getting-started/login-and-site-list.md) and [Create a site](./getting-started/create-site.md).

## Site-Level Deep Link Format

```text
/_Admin{routePath}?SiteId={siteGuid}
```

Replace `{siteGuid}` with the `SiteId` in the address bar. Paths in the tables below are relative to `/_Admin` (and require `SiteId`).

## Top Bar and Common Entry Points

| Entry | Path | Notes |
|------|------|------|
| Dashboard (home) | `/site` | Site overview; see [Dashboard](./site/dashboard.md) |
| Media Library | `/site/media` | Images and media; see [Media Library](./site/media.md) |
| Pages | `/site/pages` | Page list and editing; see [Pages](./site/pages.md) |

## Content

Left sidebar group: **Content** (some items appear only after you enable them in **Edit Menu**)

| Menu | Path | Notes |
|------|------|------|
| Content | `/content/contents` | Content folders and entries; maps to `k.content.{folderName}` |
| Content Types | `/content/contentTypes` | Field models; create before content folders |
| Labels | `/content/labels` | Multilingual copy; maps to `k.label` / `k.t` |
| HTML Blocks | `/content/htmlblocks` | Reusable HTML blocks; see [HTML Blocks](./content/html-blocks.md) |
| Files | `/content/files` | CMS file library; see [Files](./content/files.md) |
| Tag Attributes | `/content/text` | Template `k-config`; see [Tag Attributes](./content/tag-attributes.md) |
| Param Config | `/content/useroptions` | Structured site parameters; see [Param Config](./content/user-options.md) |

Related docs: [Content overview](./content/index.md) · [Content types](./content/content-types.md) · [Content list](./content/contents-folders-list.md) · [Labels](./content/labels.md) · [HTML Blocks](./content/html-blocks.md) · [Files](./content/files.md) · [Tag Attributes](./content/tag-attributes.md) · [Param Config](./content/user-options.md)

## Commerce

Left sidebar group: **Commerce** (`advanced`)

| Menu | Path | Docs |
|------|------|------|
| Product Management | `/commerce/product-management` | [List](./commerce/product-management.md) · [Detail](./commerce/product-management-detail.md) |
| Product Types | `/commerce/product-types` | [Product Types](./commerce/product-types.md) |
| Product Categories | `/commerce/product-categories` | [Product Categories](./commerce/product-categories.md) |
| Carts | `/commerce/carts` | [Carts](./commerce/carts.md) |
| Customers | `/commerce/customers` | [Customers](./commerce/customers.md) |
| Orders | `/commerce/orders` | [Orders](./commerce/orders.md) |
| Sale Stats | `/commerce/sale-stats` | [Sale Stats](./commerce/sale-stats.md) |
| Discounts | `/commerce/discounts` | [Discounts](./commerce/discounts.md) |
| Currencies | `/commerce/currencies` | [Currencies](./commerce/currencies.md) |
| Shipping | `/commerce/shippings` | [Shipping](./commerce/shippings.md) |
| Loyalty | `/commerce/loyalty` | [Loyalty](./commerce/loyalty.md) |
| Taxes | `/commerce/taxes` | [Taxes](./commerce/taxes.md) |
| Notification | `/commerce/notification` | [Notification](./commerce/notification.md) |
| Settings | `/commerce/settings` | [Settings](./commerce/settings.md) |

Overview: [Commerce](./commerce/index.md) · Dev reference: [k.commerce](../api/commerce/commerce.md)

## Operations Center

Left sidebar group: **Operations Center** (route prefix `/system`, different group from **Site Settings**)

| Menu | Path | Docs |
|------|------|------|
| Visitor Logs | `/system/visitor-logs` | [Visitor Logs](./operations/visitor-logs.md) |
| Behavior Analysis | `/system/user-behavior-analysis` | No dedicated page yet |
| Resource Guardian | `/system/resource-guardian` | [Resource Guardian](./operations/resource-guardian.md) |
| Page Interaction | `/system/page-interaction` | [Page Interaction](./operations/page-interaction.md) |
| AB Testing | `/system/ab-testing` | [AB Testing](./operations/ab-testing.md) |
| Error Monitoring | `/system/error-monitoring` | No dedicated page yet |
| Site Logs | `/system/site-logs` | [Site Logs](./operations/site-logs.md) |
| Sync | `/system/sync` | [Sync](./operations/sync.md) |
| Search | `/system/search` | No dedicated page yet |
| Automation | `/system/automation` | No dedicated page yet |
| AI Service | `/system/ai-service` | [AI Service](./settings/ai-service.md) |

Overview: [Operations Center](./operations/index.md)

## Development

Left sidebar group: **Development** (`advanced`)

| Menu | Path | Docs |
|------|------|------|
| Views | `/development/views` | [Views](./development/views.md) |
| Layouts | `/development/layouts` | [Layouts](./development/layouts.md) |
| Scripts | `/development/scripts` | [Scripts](./development/scripts.md) |
| Styles | `/development/styles` | [Styles](./development/styles.md) |
| Code | `/development/code` | [Code](./development/code.md) |
| Code Log | `/development/code-log` | [Code Log](./development/code-log.md) |
| Code Search | `/development/code-search` | [Code Search](./development/code-search.md) |
| URL | `/development/urls` | [URL](./development/urls.md) |
| Forms | `/development/forms` | [Forms](./development/forms.md) |
| Menus | `/development/menus` | [Menus](./development/menus.md) |
| Authentication | `/development/authentication` | [Authentication](./development/authentication.md) |
| Open API | `/development/openapis` | [Open API](./development/openapis.md) |
| SPA Multilingual | `/development/spamultilingual` | [SPA Multilingual](./development/spa-multilingual.md) |
| Modules | `/development/modules` | [Modules](./development/modules.md) |
| Scheduled Jobs | `/development/jobs` | [Scheduled Jobs](./development/jobs.md) |

Overview: [Development](./development/index.md) (includes Template Engine / API cross-reference)

::: info Module-injected sidebar menus
Published modules can configure **`menu`** in **`module.config`** to attach custom entries under a **top-level group**. Set `parent` to that group's **`menu.name`** in routing (e.g. `content`, `commerce`, `development`, `system`, `database`). See [Modules · Mount site admin menu](./development/modules.md#mount-site-admin-menu-menu).
:::

Page designer routes (no left-menu item): `/page/edit`, `/page/design`, etc. Open from [Pages](./site/pages.md) list.

## Database

Left sidebar group: **Database** (`advanced`)

| Menu | Path | Docs |
|------|------|------|
| IndexedDB Table | `/database/table` | [IndexedDB Table](./database/table.md) |
| IndexedDB Relations | `/database/table-relation` | [Table Relations](./database/table-relation.md) |
| Key-Value | `/database/key-value` | [Key-Value](./database/key-value.md) |
| SQLite Table | `/database/sqlite-table` | [SQLite Table](./database/sqlite-table.md) |
| MySQL Table | `/database/mysql-table` | [MySQL Table](./database/mysql-table.md) |
| SQL Server Table | `/database/sqlserver-table` | [SQL Server Table](./database/sqlserver-table.md) |
| SQL Logs | `/database/sql-logs` | [SQL Logs](./database/sql-logs.md) |

Overview: [Database](./database/index.md) · Dev reference: [k.DB](../api/database/index.md) · [k.DB.sqlite](../api/sqlite/index.md)

## Site Settings

Left sidebar group: **Site Settings** (also `/system` prefix, different menu group from **Operations Center**)

| Menu | Path | Docs |
|------|------|------|
| Basic Settings | `/system/settings` | [Basic Settings](./settings/basic/) |
| Domains | `/system/domains` | [Domains](./settings/domains.md) |
| Integrations | `/system/config` | [Integrations](./settings/integrations/) |
| Site Users | `/system/siteuser` | [Site Users](./settings/site-users.md) |
| Roles | `/system/roles` | [Roles](./settings/roles.md) |
| Request Hooks | `/system/front-events` | [Request Hooks](./settings/request-hooks.md) |
| Action Hooks | `/system/backend-events` | [Action Hooks](./settings/action-hooks.md) |
| Cookie | `/system/cookie` | [Cookie](./settings/cookie.md) |
| AI Service | `/system/ai-service` | [AI Service](./settings/ai-service.md) |

Overview: [Site Settings](./settings/index.md) · Integration deep link example: `/_Admin/system/config?SiteId=...&group=Payment`

## Standalone Features (Not in Main Left Menu)

| Feature | Path |
|------|------|
| AI App Builder | `/ai-app-builder` |
| AI Chat | `/ai-chat/overview` |

## Permissions and Edit Menu

- Whether a submenu appears also depends on site role and feature permissions (`meta.menu.permission`).
- Routes marked `advanced: true` (e.g. **Files**, **HTML Blocks**, **Tag Attributes**) are **hidden** from the left sidebar by default; enable them in **Edit Menu**.

### How to Open Edit Menu

1. Open any site-admin page (e.g. [Dashboard](./site/dashboard.md)).
2. Click **Edit Menu** at the bottom of the left sidebar (pencil icon, `editMenu`).
3. In the panel, check or uncheck items under each group; the hint says to hide infrequently used menus.
4. After closing the panel, the left sidebar updates immediately.

<DocImage src="/cms/navigation-edit-menu.png" alt="Edit Menu: choose Content, Site Settings, and other submenu items for the sidebar" width="1120" />

After enabling **Content → Files**, **HTML Blocks**, etc., the matching entries appear under **Content**; see each sub-doc for details.
