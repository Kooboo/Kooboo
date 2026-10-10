# Basic · Access Control Tab

> Tab: **Access Control** · `name=access`  
> Deep link: `/_Admin/system/settings?SiteId={siteGuid}&name=access`

Controls **who can access which paths** on the site: site type (public / member / private), path allow/deny lists, single sign-on, geo restrictions, request rate limits and blocks, CORS, and cloud **development password** preview mode.

<DocImage src="/cms/settings/settings-basic-access.png" alt="Access Control tab" width="1120" />

## Site Type

The **Site type** dropdown (`siteType`) sets the default access policy when no special path matches. Common values include **Public** (`p`), **Member** (`m`), **Private** (`o`), etc. (per `getTypes` API).

When the type is **not public**, the **Path mode** section appears:

| Field | Description |
|-------|-------------|
| **Path mode** | **Include paths** (only listed paths allowed) or **Exclude paths** (listed paths denied; others allowed) |
| **Path list** | Multiple path patterns; **Add** / **Delete** rows |
| **Single sign-on** | `ssoLogin`: works with SSO integration (must be configured under [Integrations](../integrations/)) |

## Visitor Country / Region Restrictions

Shown on **Kooboo online servers** (`isOnlineServer`). When enabled, select blocked countries/regions and a **page** for blocked visitors to land on.

## IP Request Rate Limiting

Group **IP request rate limiting** (`rateLimitSettings`):

| Item | Description |
|------|-------------|
| **Limit all requests** | When on, one limit applies site-wide; when off, configure **IP** and **User-Agent** separately with `withinSeconds` / `permitLimit` |
| IP / UA rules | Multi-row key-value editor; keys are IP or UA strings |

## Request Blocking

Group **Request blocking** (`accessLimitSettings`):

| List | Description |
|------|-------------|
| **IP blocklist** | Matching IPs are rejected |
| **User-Agent blocklist** | UA keyword match rejects the request |

## CORS

**CORS** toggle (`enableCORS`): browser CORS headers when cross-origin clients access site API/resources (see UI tooltip).

## Development Password (Online Public Cloud)

Shown when **online server, not private deployment**, and service tier allows:

| Item | Description |
|------|-------------|
| **Enable development password** | Toggles site status between `Published` and `Development` |
| **Development password** | Read-only password in `Development` state; visitors must enter it to preview |

This block may be hidden on private or local environments.

## Related

- [Basic Settings overview](./index.md)  
- [Operations · Visitor logs](../../operations/visitor-logs.md) (observe traffic; not the same as rate limits on this tab)  
- [Request Hooks](../request-hooks.md) (script-level interception)
