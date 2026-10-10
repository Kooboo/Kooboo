---
name: kooboo-cms-guide
description: Use when guiding users to Kooboo CMS admin URLs, show_action_button, get_site_info (siteId, baseUrl), or JWT/payment/SMS/OAuth setup links. Always use documented deep links with correct path and query params.
---

# Kooboo CMS Configuration Path Guide

When a user must complete Kooboo CMS admin configuration before the current solution can work, use `show_action_button` to display a navigation button instead of describing the click path only in text.

Before building a link, **always** call `get_site_info` first; from the returned `siteSettings`, read **`siteId`** and **`baseUrl`** (never invent them).

When the path is uncertain, first consult the table with `read_skill_resource("kooboo-cms-guide", "references/admin-urls.md")`; never assemble it from memory.

## Page-knowledge references

When the page preset has already injected a page reference, use the injected content directly and do not read it again. Read a linked topic reference only when the user's follow-up asks for details beyond the core page reference.

- Basic settings core knowledge: `references/system/settings/basic.md`
- Access control core knowledge: `references/system/settings/access.md`
- Performance settings core knowledge: `references/system/settings/performance.md`
- SEO settings core knowledge: `references/system/settings/seo.md`
- Editor settings core knowledge: `references/system/settings/editor.md`
- Advanced settings core knowledge: `references/system/settings/advance.md`
- SPA routing and 404 behavior: `references/system/settings/spa-routing.md`
- Multilingual configuration: `references/system/settings/multilingual.md`
- Site package import and export: `references/system/settings/import-export.md`
- Content entries and folders: `references/content/contents.md`
- Content types: `references/content/content-types.md`
- Multilingual Label text: `references/content/labels.md`
- HTML blocks: `references/content/html-blocks.md`
- General files: `references/content/files.md`
- Tag attributes: `references/content/tag-attributes.md`
- User options: `references/content/user-options.md`
- Media library and image editing: `references/content/media-library.md`
- Dashboard: `references/site-structure/dashboard.md`
- Pages: `references/site-structure/pages.md`
- Page editors: `references/site-structure/page-editors.md`
- Layouts: `references/site-structure/layouts.md`
- Views: `references/site-structure/views.md`
- Menus: `references/site-structure/menus.md`
- URLs and routing: `references/site-structure/urls.md`
- Frontend scripts: `references/development/scripts.md`
- Frontend styles: `references/development/styles.md`
- KScript code: `references/development/code.md`
- Forms: `references/development/forms.md`
- Authentication rules: `references/development/authentication.md`
- Code logs: `references/development/code-log.md`
- Code search: `references/development/code-search.md`
- Open API: `references/development/openapis.md`
- SPA multilingual: `references/development/spa-multilingual.md`
- Scheduled jobs: `references/development/jobs.md`
- Form values: `references/development/form-values.md`
- Modules and dynamic module menus: `references/development/modules.md`
- Development-mode multi-resource workspace: `references/development/dev-mode.md`
- Domain management: `references/system/domains.md`
- Site users: `references/system/site-users.md`
- Roles and permissions: `references/system/roles.md`
- Request hooks: `references/system/front-events.md`
- Action hooks: `references/system/backend-events.md`
- Visitor logs: `references/operations/visitor-logs.md`
- User behavior analytics: `references/operations/user-behavior-analysis.md`
- Resource protection: `references/operations/resource-guardian.md`
- Page interaction: `references/operations/page-interaction.md`
- Error monitoring: `references/operations/error-monitoring.md`
- A/B testing: `references/operations/ab-testing.md`
- Cookie management: `references/operations/cookie-management.md`
- Site operation logs: `references/operations/site-logs.md`
- Collaboration sync: `references/operations/collaboration-sync.md`
- AI service: `references/operations/ai-service.md`
- Full-text search index: `references/operations/search-index.md`
- Automation: `references/operations/automation.md`
- Service integrations overview: `references/integrations/overview.md`
- Database integration: `references/integrations/database.md`
- Payment integration: `references/integrations/payment.md`
- SMS integration: `references/integrations/sms.md`
- OAuth2 integration: `references/integrations/oauth2.md`
- JWT and Firebase: `references/integrations/others.md`
- Site IndexedDB tables: `references/database/indexeddb-tables.md`
- IndexedDB table relations: `references/database/table-relations.md`
- Key-Value Store: `references/database/key-value.md`
- SQLite, MySQL, and SQL Server tables: `references/database/relational-tables.md`
- SQL logs: `references/database/sql-logs.md`
- Product management: `references/commerce/products.md`
- Product types: `references/commerce/product-types.md`
- Product categories: `references/commerce/categories.md`
- Carts and admin checkout: `references/commerce/carts.md`
- Commerce customers: `references/commerce/customers.md`
- Orders: `references/commerce/orders.md`
- Sales statistics: `references/commerce/sale-stats.md`
- Discounts: `references/commerce/discounts.md`
- Currencies: `references/commerce/currencies.md`
- Membership levels: `references/commerce/memberships.md`
- Members and point details: `references/commerce/members.md`
- Earn-points configuration: `references/commerce/earn-points.md`
- Redeem-points configuration: `references/commerce/redeem-points.md`
- Physical shipping: `references/commerce/express-shipping.md`
- Digital delivery: `references/commerce/digital-shipping.md`
- Taxes: `references/commerce/taxes.md`
- Notifications: `references/commerce/notifications.md`
- Commerce settings: `references/commerce/settings.md`

## Link types

| Type | When to use | Format |
|------|----------|------|
| **Admin configuration page** | CMS features such as site settings, development, content, commerce, and databases | `/_Admin{路由}?SiteId={siteId}`; add `&group=`, `&name=`, and so on when needed |
| **Public site page** | Preview pages, visitor-facing routes, and absolute addresses that must match the domain | `{baseUrl}{路径}` (a complete `http`/`https` URL) |

### `siteId` and `baseUrl`

- **`siteId`**: The site GUID. The admin deep-link query parameter is always **`SiteId`** (capital S), for example `?SiteId={siteId}`; use `&SiteId={siteId}` when a query string already exists.
- **`baseUrl`**: The public root address of the site (scheme + host, possibly with a path prefix). A relative path `/` resolves against the AI console domain and **is not** the visitor site domain; public pages must use `baseUrl` to build an absolute URL.

## Path guardrails (common errors)

Check the following rules before calling `show_action_button` to avoid navigating to the wrong page:

1. **Case**: The admin prefix is always `/_Admin/` (capital `A`), not `/_admin/`, `/admin/`, or `/Admin/`.
2. **Site-level paths must include `SiteId`**: Except for account-level pages (`/_Admin/login`, `/_Admin/create`), every site admin path must include `SiteId={siteId}`.
3. **Do not confuse the two `/system` entry points**:
   - `/_Admin/system/settings` → **Basic settings** (multilingual, SEO, performance, and other tabs)
   - `/_Admin/system/config` → **Service integrations** (Database / Payment / SMS / OAuth2 / Others, and so on)
4. **Service integrations require `group`**: When landing on a specific group, add `&group={GroupName}`; do not write only `/_Admin/system/config?SiteId=...` and make the user find the group:
   - JWT → `group=Others` (item name `JwtSetting`)
   - Payments → `group=Payment` (Alipay, WeChatV3, and others are here; configure keys **not** in commerce settings)
   - SMS → `group=SMS`
   - OAuth2 social login → `group=OAuth2`
   - External databases → `group=Database`
   - Object storage → `group=Storage`
5. **Do not confuse commerce “settings” with payment integration**:
   - Payment-channel keys → `/_Admin/system/config?SiteId=...&group=Payment`
   - Commerce-wide settings (units, checkout payment-method switches, field models) → `/_Admin/commerce/settings?SiteId=...`
   - Currencies → `/_Admin/commerce/currencies?SiteId=...`
6. **Authentication rules ≠ JWT configuration ≠ login page**:
   - JWT Secret → `/_Admin/system/config?SiteId=...&group=Others`
   - Access-control rules (JWT validation / KScript) → `/_Admin/development/authentication?SiteId=...`
   - Kooboo built-in login page → `/_Admin/login` (account-level, normally without `SiteId`)
7. **The multilingual switch is a Basic settings tab**, not a separate menu: `/_Admin/system/settings?SiteId=...&name=basic` (multilingual is in the Multilingual section of the Basic tab).
8. **Label text** → `/_Admin/content/labels?SiteId=...` (multilingual must be enabled first).
9. **Code logs** → `/_Admin/development/code-log?SiteId=...` (not the operations log `site-logs`).
10. **Never invent paths**: such as `/settings/...`, `/cms/...`, `/kooboo/...`, or an English slug obtained by translating a Chinese menu label.

## show_action_button

Parameters:

- `message`: Optional; briefly explain why this page must be opened.
- `label`: Required; use clear action-oriented button text.
- `url`: Required. A complete `http`/`https` link or a Kooboo relative path beginning with `/`.

## Common scenarios (validated deep links)

Replace `{siteId}` using `get_site_info`, which returns `siteSettings.siteId`.

### Service integrations

| Scenario | Admin menu path | Deep link |
|------|--------------|------|
| JWT Secret | Site settings → Service integrations → Others → JwtSetting | `/_Admin/system/config?SiteId={siteId}&group=Others` |
| WeChat Pay / Alipay, and so on | Site settings → Service integrations → Payment | `/_Admin/system/config?SiteId={siteId}&group=Payment` |
| Alibaba Cloud SMS | Site settings → Service integrations → SMS | `/_Admin/system/config?SiteId={siteId}&group=SMS` |
| OAuth2 social login | Site settings → Service integrations → OAuth2 | `/_Admin/system/config?SiteId={siteId}&group=OAuth2` |
| MySQL / SQL Server connection | Site settings → Service integrations → Database | `/_Admin/system/config?SiteId={siteId}&group=Database` |
| Object storage | Site settings → Service integrations → Storage | `/_Admin/system/config?SiteId={siteId}&group=Storage` |

### Site settings

| Scenario | Deep link |
|------|------|
| Basic settings (including multilingual) | `/_Admin/system/settings?SiteId={siteId}&name=basic` |
| SEO | `/_Admin/system/settings?SiteId={siteId}&name=seo` |
| Access control | `/_Admin/system/settings?SiteId={siteId}&name=access` |
| Performance | `/_Admin/system/settings?SiteId={siteId}&name=performance` |
| Advanced (including the SQL log switch) | `/_Admin/system/settings?SiteId={siteId}&name=advance` |
| Domain management | `/_Admin/system/domains?SiteId={siteId}` |
| Cookie management | `/_Admin/system/cookie?SiteId={siteId}` |
| AI service | `/_Admin/system/ai-service?SiteId={siteId}` |

### Content and multilingual

| Scenario | Deep link |
|------|------|
| Multilingual Label text | `/_Admin/content/labels?SiteId={siteId}` |
| Content types | `/_Admin/content/contentTypes?SiteId={siteId}` |
| Content folders and entries | `/_Admin/content/contents?SiteId={siteId}` |
| HTML fragments | `/_Admin/content/htmlblocks?SiteId={siteId}` |

### Development

| Scenario | Deep link |
|------|------|
| Authentication rules | `/_Admin/development/authentication?SiteId={siteId}` |
| Code (API / CodeBlock) | `/_Admin/development/code?SiteId={siteId}` |
| Code logs | `/_Admin/development/code-log?SiteId={siteId}` |
| Code search | `/_Admin/development/code-search?SiteId={siteId}` |
| Forms | `/_Admin/development/forms?SiteId={siteId}` |
| URL routes | `/_Admin/development/urls?SiteId={siteId}` |
| Open API | `/_Admin/development/openapis?SiteId={siteId}` |
| SPA multilingual | `/_Admin/development/spamultilingual?SiteId={siteId}` |
| Scheduled jobs | `/_Admin/development/jobs?SiteId={siteId}` |

### Commerce

| Scenario | Deep link |
|------|------|
| Currencies | `/_Admin/commerce/currencies?SiteId={siteId}` |
| Commerce-wide settings | `/_Admin/commerce/settings?SiteId={siteId}` |
| Product management | `/_Admin/commerce/product-management?SiteId={siteId}` |
| Orders | `/_Admin/commerce/orders?SiteId={siteId}` |
| Membership levels | `/_Admin/commerce/loyalty?SiteId={siteId}` |

### Site pages

| Scenario | Deep link |
|------|------|
| Dashboard | `/_Admin/site?SiteId={siteId}` |
| Page list | `/_Admin/site/pages?SiteId={siteId}` |
| Media library | `/_Admin/site/media?SiteId={siteId}` |

## Examples

JWT configuration (note `group=Others`):

```json
{
  "message": "JWT 签发与校验依赖站点 JwtSetting，请先在后台配置 Secret。",
  "label": "配置 JWT",
  "url": "/_Admin/system/config?SiteId={siteId}&group=Others"
}
```

WeChat Pay configuration (note `group=Payment`, not commerce settings):

```json
{
  "message": "微信支付商户密钥在「服务集成 → Payment」中配置。",
  "label": "配置微信支付",
  "url": "/_Admin/system/config?SiteId={siteId}&group=Payment"
}
```

Multilingual and Labels:

```json
{
  "message": "请先在基础设置开启多语言，再在 Label 页维护各语言文案。",
  "label": "开启多语言",
  "url": "/_Admin/system/settings?SiteId={siteId}&name=basic"
}
```

## Complete path index

For the remaining menus and subroutes, see [references/admin-urls.md](references/admin-urls.md) (source: the Kooboo CMS official `navigation.md` and module deep links).
