# Settings

> Menu: left sidebar **Settings** group (enable in **Edit Menu**)
> Route prefix: `/_Admin/system/...?SiteId={siteGuid}`

**Settings** configures site basics, domains, third-party **integrations**, admin users and permissions, Hooks, Cookie, and more. Shares `/system` prefix with [Operations Center](../operations/index.md) but **different left menu group** (`menu.name` is `system`, label “Settings”).

::: info Advanced menu
The entire **Settings** group is `advanced: true` on routes—hidden by default; enable in [Edit Menu](../navigation.md#permissions-and-edit-menu). **Roles**, **Request Hooks**, **Action Hooks**, and **AI Service** sub-items are also `advanced`.
:::

::: tip Overlap with Operations Center
| Menu | Path | Notes |
|------|------|-------|
| **AI Service** | `/system/ai-service` | **Same page** as Operations Center entry; single doc at [AI Service](./ai-service.md) with cross-links |

Other Operations menus (visitor logs, sync, etc.): [Operations Center](../operations/index.md).
:::

## How to Open

1. In [Edit Menu](../navigation.md#permissions-and-edit-menu), enable **Settings** and needed sub-items.
2. Expand **Settings** in the left sidebar.

## Submenus (admin order)

| Menu | Path | Permission (feature) | Doc | Status |
|------|------|----------------------|-----|--------|
| Basic Settings | `/system/settings` | `site·edit` | [Basic Settings](./basic/) | ✅ |
| Domains | `/system/domains` | `domain` | [Domains](./domains.md) | ✅ |
| Integrations | `/system/config` | `config` | [Integrations](./integrations/) | ✅ |
| Site Users | `/system/siteuser` | `siteUser` | [Site Users](./site-users.md) | ✅ |
| Roles | `/system/roles` | `role` | [Roles](./roles.md) | ✅ |
| Request Hooks | `/system/front-events` | `frontEvents` | [Request Hooks](./request-hooks.md) | ✅ |
| Action Hooks | `/system/backend-events` | `backendEvents` | [Action Hooks](./action-hooks.md) | ✅ |
| Cookie Management | `/system/cookie` | `site·edit` | [Cookie](./cookie.md) | ✅ |
| AI Service | `/system/ai-service` | `ai` | [AI Service](./ai-service.md) | ✅ |

Deep link examples:

```text
/_Admin/system/settings?SiteId={siteGuid}&name=basic
/_Admin/system/config?SiteId={siteGuid}&group=Database
/_Admin/system/front-events/edit?SiteId={siteGuid}&name=RouteFinding&display=RouteFinding
/_Admin/system/backend-events/edit?SiteId={siteGuid}&name=ContentUpdated&display=ContentUpdated
```

## Sub-routes Without Sidebar Menu

| Route | Entered from | Documented in |
|-------|--------------|---------------|
| `/system/settings?name={tab}` | Basic Settings tab switch | [Basic Settings](./basic/) tab docs |
| `/system/front-events/edit` | Request Hooks list | [Request Hooks](./request-hooks.md) |
| `/system/backend-events/edit` | Action Hooks list | [Action Hooks](./action-hooks.md) |

### Basic Settings Tabs (`name` query)

| `name` | Tab label | Main topics |
|--------|-----------|-------------|
| `basic` | General | Display name, Base URL, Force SSL, SPA, **multilingual**, import/export package |
| `access` | Access Control | IP / region restrictions, etc. |
| `performance` | Performance | Cache, compression, etc. |
| `seo` | SEO | Site-level SEO switches |
| `editor` | Editor | Admin editor preferences |
| `advance` | Advanced | Default database, SQL log, constraint fix, etc. (ties to [SQL logs](../database/sql-logs.md)) |

Multilingual lives in the **General** tab `Multilingual` section—not a separate menu; see [Basic · General Tab](./basic/general.md#multilingual). Do not confuse with [Development -> SPA multilingual](../development/spa-multilingual.md).

## Integrations (`config`) Preview

**Integrations** page folds **Group** sections for `CoreSetting` items; see [Integrations](./integrations/).

| Group | Doc |
|-------|-----|
| **Database** | [Database](./integrations/database.md) |
| **Payment** | [Payment](./integrations/payment.md) |
| **SMS** | [SMS](./integrations/sms.md) |
| **OAuth2** | [OAuth2](./integrations/oauth2.md) |
| **Storage** | [Storage](./integrations/storage.md) |
| **Others** | [Others](./integrations/others.md) |

Payment integrations are **not** duplicated in Commerce settings—use **Payment** group here.

## Suggested Configuration Order

Typical implementation path:

1. **[Basic Settings](./basic/)** — six tab docs (multilingual, SQL log switch, etc.)
2. **[Domains](./domains.md)** — bindings, primary domain, SSL
3. **[Integrations](./integrations/)** — group docs; Database / Payment cross-link to API
4. **[Site Users](./site-users.md)** · **[Roles](./roles.md)** — admin accounts and feature permissions
5. **[Request Hooks](./request-hooks.md)** · **[Action Hooks](./action-hooks.md)** — including edit sub-pages
6. **[Cookie](./cookie.md)**
7. **[AI Service](./ai-service.md)** — cross-link [Operations Center](../operations/index.md)

## Development Reference (KScript / API)

| Admin capability | API doc |
|------------------|---------|
| Integrations CoreSetting | `k.config` / site settings APIs (per item) |
| Payment | [k.payment](../../api/payment/index.md) |
| Database connections | [k.DB](../../api/database/index.md) |
| Multilingual / Label | [k.label](../../api/label/index.md) |
| JWT, etc. | APIs for each integration |

CMS docs cover **admin click paths** and form fields; script signatures and examples stay in API docs.

## Related

| Doc | Description |
|-----|-------------|
| [Site admin menu overview](../navigation.md) | Full menu tree |
| [Database](../database/index.md) | Table management after Database integration |
| [Operations Center](../operations/index.md) | Observation and collaboration under `/system` |
