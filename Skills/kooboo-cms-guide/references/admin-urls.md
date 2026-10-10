# Kooboo Admin Deep-link Index

> Site-level deep-link format: `/_Admin{路由}?SiteId={站点GUID}`
> `{站点GUID}` comes from `get_site_info` → `siteSettings.siteId`; the query parameter is always **`SiteId`** (capital S).

## Account-level (without SiteId)

| Page | Path |
|------|------|
| My sites | `/_Admin/` |
| Login | `/_Admin/login` |
| Create site | `/_Admin/create` |
| Import site | `/_Admin/create/import` |
| Clone site | `/_Admin/create/clone` |
| List within a folder | `/_Admin/?currentFolder={文件夹名}` |

## Top bar and common entry points

| Entry point | Deep link |
|------|------|
| Dashboard | `/_Admin/site?SiteId={站点GUID}` |
| Media library | `/_Admin/site/media?SiteId={站点GUID}` |
| Page list | `/_Admin/site/pages?SiteId={站点GUID}` |
| Page settings (HTML) | `/_Admin/page/setting?SiteId={站点GUID}&id={页面Id}` |
| Page code editor | `/_Admin/page/edit?SiteId={站点GUID}&id={页面Id}` |
| Inline editing | `/_Admin/inline-design?SiteId={站点GUID}&path={页面路径}&id={页面Id}&type={页面类型}` |

## Content

| Menu | Deep link |
|------|------|
| Content folders and entries | `/_Admin/content/contents?SiteId={站点GUID}` |
| Content entries (by folder) | `/_Admin/content/textContentsByFolder?SiteId={站点GUID}&folder={内容文件夹Id}` |
| Edit content entry | `/_Admin/content/content?SiteId={站点GUID}&folder={内容文件夹Id}&id={条目Id}` |
| Content types | `/_Admin/content/contentTypes?SiteId={站点GUID}` |
| Edit content type | `/_Admin/content/contentType?SiteId={站点GUID}&id={类型Id}` |
| Labels | `/_Admin/content/labels?SiteId={站点GUID}` |
| HTML fragments | `/_Admin/content/htmlblocks?SiteId={站点GUID}` |
| Edit HTML fragment | `/_Admin/content/htmlBlock/edit?SiteId={站点GUID}&id={片段Id}` |
| Files | `/_Admin/content/files?SiteId={站点GUID}` |
| Element attributes | `/_Admin/content/text?SiteId={站点GUID}` |
| User options | `/_Admin/content/useroptions?SiteId={站点GUID}` |

## Commerce

| Menu | Deep link |
|------|------|
| Product management | `/_Admin/commerce/product-management?SiteId={站点GUID}` |
| Product types | `/_Admin/commerce/product-types?SiteId={站点GUID}` |
| Product categories | `/_Admin/commerce/product-categories?SiteId={站点GUID}` |
| Carts | `/_Admin/commerce/carts?SiteId={站点GUID}` |
| Customer management | `/_Admin/commerce/customers?SiteId={站点GUID}` |
| Orders | `/_Admin/commerce/orders?SiteId={站点GUID}` |
| Sales statistics | `/_Admin/commerce/sale-stats?SiteId={站点GUID}` |
| Discounts | `/_Admin/commerce/discounts?SiteId={站点GUID}` |
| Currencies | `/_Admin/commerce/currencies?SiteId={站点GUID}` |
| Shipping | `/_Admin/commerce/shippings?SiteId={站点GUID}` |
| Membership | `/_Admin/commerce/loyalty?SiteId={站点GUID}` |
| Taxes | `/_Admin/commerce/taxes?SiteId={站点GUID}` |
| Notifications | `/_Admin/commerce/notification?SiteId={站点GUID}` |
| Commerce settings | `/_Admin/commerce/settings?SiteId={站点GUID}` |

::: warning Payment keys are not in commerce settings
Payment-channel keys are only in **Site settings → Service integrations → Payment**: `/_Admin/system/config?SiteId={站点GUID}&group=Payment`
:::

## Operations center

Route prefix: `/_Admin/system/...?SiteId={站点GUID}` (shares the `/system` prefix with “Site settings”, but uses a different left-menu group).

| Menu | Deep link |
|------|------|
| Visitor analytics | `/_Admin/system/visitor-logs?SiteId={站点GUID}` |
| Resource guardian | `/_Admin/system/resource-guardian?SiteId={站点GUID}` |
| Page interaction | `/_Admin/system/page-interaction?SiteId={站点GUID}` |
| A/B testing | `/_Admin/system/ab-testing?SiteId={站点GUID}` |
| Operation logs | `/_Admin/system/site-logs?SiteId={站点GUID}` |
| Collaboration sync | `/_Admin/system/sync?SiteId={站点GUID}` |
| AI service | `/_Admin/system/ai-service?SiteId={站点GUID}` |

## Development

Route prefix: `/_Admin/development/...?SiteId={站点GUID}`

| Menu | Deep link |
|------|------|
| Components (Views) | `/_Admin/development/views?SiteId={站点GUID}` |
| Layouts | `/_Admin/development/layouts?SiteId={站点GUID}` |
| Scripts | `/_Admin/development/scripts?SiteId={站点GUID}` |
| Styles | `/_Admin/development/styles?SiteId={站点GUID}` |
| Code | `/_Admin/development/code?SiteId={站点GUID}` |
| Edit code | `/_Admin/development/code/edit?SiteId={站点GUID}` |
| Code logs | `/_Admin/development/code-log?SiteId={站点GUID}` |
| Code search | `/_Admin/development/code-search?SiteId={站点GUID}` |
| URL | `/_Admin/development/urls?SiteId={站点GUID}` |
| Forms | `/_Admin/development/forms?SiteId={站点GUID}` |
| Menus | `/_Admin/development/menus?SiteId={站点GUID}` |
| Authentication | `/_Admin/development/authentication?SiteId={站点GUID}` |
| Open API | `/_Admin/development/openapis?SiteId={站点GUID}` |
| SPA multilingual | `/_Admin/development/spamultilingual?SiteId={站点GUID}` |
| Modules | `/_Admin/development/modules?SiteId={站点GUID}` |
| Scheduled jobs | `/_Admin/development/jobs?SiteId={站点GUID}` |

## Database

Route prefix: `/_Admin/database/...?SiteId={站点GUID}`

| Menu | Deep link |
|------|------|
| IndexedDB tables | `/_Admin/database/table?SiteId={站点GUID}` |
| Table relations | `/_Admin/database/table-relation?SiteId={站点GUID}` |
| Key-value store | `/_Admin/database/key-value?SiteId={站点GUID}` |
| SQLite tables | `/_Admin/database/sqlite-table?SiteId={站点GUID}` |
| MySQL tables | `/_Admin/database/mysql-table?SiteId={站点GUID}` |
| SQL Server tables | `/_Admin/database/sqlserver-table?SiteId={站点GUID}` |
| SQL logs | `/_Admin/database/sql-logs?SiteId={站点GUID}` |

When no external database is configured, the MySQL / SQL Server onboarding page redirects to `/_Admin/system/config?SiteId={站点GUID}&group=Database`.

## Site settings

Route prefix: `/_Admin/system/...?SiteId={站点GUID}`

| Menu | Deep link |
|------|------|
| Basic settings | `/_Admin/system/settings?SiteId={站点GUID}` |
| Domain management | `/_Admin/system/domains?SiteId={站点GUID}` |
| Service integrations | `/_Admin/system/config?SiteId={站点GUID}` |
| User management | `/_Admin/system/siteuser?SiteId={站点GUID}` |
| Roles and permissions | `/_Admin/system/roles?SiteId={站点GUID}` |
| Request hooks | `/_Admin/system/front-events?SiteId={站点GUID}` |
| Operation hooks | `/_Admin/system/backend-events?SiteId={站点GUID}` |
| Cookie management | `/_Admin/system/cookie?SiteId={站点GUID}` |
| AI service | `/_Admin/system/ai-service?SiteId={站点GUID}` |

### Basic settings tabs (`name` query parameter)

| `name` | Contents |
|--------|------|
| `basic` | Display name, Base URL, SSL, SPA, **multilingual** |
| `access` | IP / regional access control |
| `performance` | Caching, compression |
| `seo` | Site-level SEO |
| `editor` | Admin editor preferences |
| `advance` | Default database, SQL log switch, and so on |

Example: `/_Admin/system/settings?SiteId={站点GUID}&name=basic`

### Service integration groups (`group` query parameter)

| `group` | Typical integrations |
|---------|------------|
| `Database` | Mysql, SqlServer, Mongo, Redis, Sqlite |
| `Payment` | Alipay, WeChatV3, Paypal, StripePayment, and so on |
| `SMS` | AliSMS, TencentSMSSetting |
| `OAuth2` | GoogleLoginSetting, FacebookLoginSetting, and so on |
| `Storage` | Object storage |
| `Others` | **JwtSetting**, FirebaseSetting |

Example: `/_Admin/system/config?SiteId={站点GUID}&group=Payment`

### Hook editor subpages

| Type | Deep link |
|------|------|
| Edit request hook | `/_Admin/system/front-events/edit?SiteId={站点GUID}&name={事件名}&display={显示名}` |
| Edit operation hook | `/_Admin/system/backend-events/edit?SiteId={站点GUID}&name={事件名}&display={显示名}` |

## Standalone features (not in the main left menu)

| Feature | Deep link |
|------|------|
| AI App Builder | `/_Admin/ai-app-builder?SiteId={站点GUID}` |
| AI Chat | `/_Admin/ai-chat/overview?SiteId={站点GUID}` |

## Easily confused paths

| Feature you want | Correct deep link | Common error |
|--------------|----------|----------|
| JWT Secret | `.../system/config?...&group=Others` | Only `config?SiteId=` without group; or use `commerce/settings` |
| Payment-channel keys | `.../system/config?...&group=Payment` | Use `commerce/settings` or `commerce/payment` |
| Multilingual switch | `.../system/settings?...&name=basic` | Write `development/spamultilingual` (that is SPA multilingual) |
| Access-control rules | `.../development/authentication?...` | Write `system/config` or `system/settings` |
| Code logs (KScript) | `.../development/code-log?...` | Write `system/site-logs` (that is the admin operation log) |
| Label text | `.../content/labels?...` | Write `content/text` (that is element attributes) |
