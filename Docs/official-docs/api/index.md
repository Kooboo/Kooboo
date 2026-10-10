# API Details

Complete reference for Kooboo server-side `k.*` APIs, including authentication, content, databases, commerce, and site resources.

## Getting Started

| Doc | Description |
|-----|-------------|
| [API Getting Started](./getting-started/) | KScript basics and the shape of the global `k` object |
| [Authentication and Authorization](./getting-started/auth.md) | Login, sessions, cookies, and access control |

## KScript APIs

| Module | Doc | Description |
|--------|-----|-------------|
| `k.account` | [account/](./account/) | Platform login, current user, organization, and OAuth |
| `k.api` | [api/](./api/) | API route helpers |
| `k.cache` | [cache/](./cache/) | Site-level in-memory cache |
| `k.commerce` | [commerce/](./commerce/commerce.md) | Commerce APIs including product, cart, and order pages |
| `k.content` | [content/](./content/) | Content types and content items |
| `k.cookie` | [cookie/](./cookie/) | Cookies |
| `k.DB` | [database/](./database/), [indexed-db/](./indexed-db/), [key-value/](./key-value/), [sqlite/](./sqlite/) | Database access, IndexedDB tables, key-value storage, and SQLite |
| `k.file` | [file/](./file/file.md) | Files in the site directory |
| `k.google` | [google/](./google/) | PageSpeed/Lighthouse and Firebase |
| `k.label` | [label/](./label/) | Multilingual `k.t` and `k.label` |
| `k.logger` | [logger/](./logger/) | Code logs |
| `k.mail` | [mail/](./mail/) | Sending mail, IMAP, SES, and parsing |
| `k.media` | [media/](./media/) | Media library files and folders |
| `k.module` | [module/](./module/) | Script modules, config, tasks, and packaging |
| `k.openApi` | [openapi/](./openapi/) | External OpenAPI definitions, auth, and calls |
| `k.net` | [net/](./net/) | HTTP, WebSocket, DNS, IP, Ping, SSH, SFTP, and Telnet |
| `k.page` | [page/](./page/) | Current page title and meta |
| `k.paramConfig` | [param-config/](./param-config/) | Structured JSON parameter config |
| `k.payment` | [payment/](./payment/) | Payment integrations |
| `k.request` | [request/](./request/) | HTTP request data |
| `k.response` | [response/](./response/) | HTTP response handling |
| `k.security` | [security/](./security/) | Encryption and hashing |
| `k.session` | [session/](./session/) | Session state |
| `k.site` | [site/](./site/) | Pages, layouts, views, scripts, styles, codes, and menus |
| `k.state` | [state/](./state/) | Per-request view state |
| `k.utils` | [utils/](./utils/) | Utility modules |
| SPA multilingual | [spa-multilingual/](./spa-multilingual/) | `/_spa/lang/{lang}` dictionaries |

## `k.site` Sub-Resources

| Member | Doc |
|--------|-----|
| `k.site.pages` | [page.md](./site/page.md) |
| `k.site.layouts` | [layout.md](./site/layout.md) |
| `k.site.views` | [view.md](./site/view.md) |
| `k.site.scripts` | [js.md](./site/js.md) |
| `k.site.styles` | [css.md](./site/css.md) |
| `k.site.codes` | [code.md](./site/code.md) |
| `k.site.menus` | [menu.md](./site/menu.md) |
| `k.site.runJob` | [job.md](./site/job.md) |

## `k.file` Subpages

Subpages: [folder](./file/file-folder.md), [resumable](./file/file-resumable.md), [data structures](./file/file-info.md)

## Related Sections

| Scenario | Next doc |
|----------|----------|
| You need to prepare content models or site settings first | [Admin CMS](../cms/index.md) |
| You need Page / Layout / View rendering rules first | [Template Engine](../templateEngine/index.md) |
