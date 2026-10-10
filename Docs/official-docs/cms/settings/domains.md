# Domains

> Menu: **Settings -> Domains**  
> Deep link: `/_Admin/system/domains?SiteId={siteGuid}`

**Domains** configures **bindings** for the current site: map subdomains or ports to this site, with optional **redirect**, **language culture**, and **SSL**. The primary domain in the list usually matches [Basic Settings · Base URL](./basic/general.md#site-information); new bindings reload site binding info.

::: tip Permissions
| Action | Permission |
|--------|------------|
| View list | `domain` |
| **New binding**, enable **SSL**, toolbar **Force SSL** | `domain·edit` |
| Multi-select **Delete** | `domain·delete` |

Root domains come from org/server **available domains** (`getAvailableDomain`); local/private environments can also bind **ports**.
:::

## How to Open

1. In [Edit Menu](../navigation.md#permissions-and-edit-menu), enable **Settings -> Domains**.  
2. Open **Settings -> Domains** in the left sidebar.

<DocImage src="/cms/settings/settings-domains-overview.png" alt="Domains" width="1120" />

## Toolbar

| Control | Description |
|---------|-------------|
| **New binding** | Opens [new binding dialog](#new-binding) |
| **Force SSL** | Site `forceSSL`; toggling **saves site immediately** (same as [Basic Settings](./basic/general.md#site-information)). Turn off if CDN/proxy causes redirect loops |

<DocImage src="/cms/settings/settings-domains-toolbar.png" alt="Domains toolbar" width="1120" />

## Binding List

| Column | Description |
|--------|-------------|
| **Domain or port** | Domain bindings show `fullName` (subdomain + root); **default port binding** shows port number |
| **Redirect to** | 301/302 target binding `fullName` when visiting this binding (optional) |
| **Language** | Default culture (must exist under [Basic Settings · Multilingual](./basic/general.md#multilingual)) |
| **SSL enabled** | HTTPS certificate for domain bindings; once on, switch is **disabled** (cannot turn off in admin—delete binding instead) |

Multi-select **Delete** calls `Binding/Deletes`.

<DocImage src="/cms/settings/settings-domains-list.png" alt="Domain binding list" width="1120" />

### Enable SSL

For **non-port** bindings, turn on **SSL enabled**:

1. `Binding/verifySSL` validates domain and certificate environment  
2. On success, `Binding/SetSsl` writes and refreshes list  

On failure, **Failed** tag on the row; hover for `sslError`. Requires `domain·edit`.

## New Binding

<DocImage src="/cms/settings/settings-domains-new-binding.png" alt="New binding dialog" width="1120" />

### Binding Type

| Type | Environment | Description |
|------|-------------|-------------|
| **Domain** | All | Subdomain + root domain dropdown (from available domains) |
| **Port** | **Not online public cloud** (`!isOnlineServer`) | Bind site to a port (`defaultBinding`) |

Online servers hide **Bind to** type toggle; domain mode only.

### Domain Fields

| Field | Rules |
|-------|-------|
| **Subdomain** | Optional; subdomain rules (length 1–63, unique per site, etc.) |
| **Root domain** | Required; org available roots; prefix `.` or `-` controls join with subdomain (`sudDomainUseDash`) |

Full hostname is combined `fullName`.

### Advanced Settings

Expand arrow in dialog:

| Field | Description |
|-------|-------------|
| **Redirect to** | Pick target `fullName` from existing bindings |
| **Language** | From site `culture` dictionary |

<DocImage src="/cms/settings/settings-domains-new-binding-advanced.png" alt="New binding advanced settings" width="1120" />

Confirm calls `Binding/post`; on success closes dialog and refreshes list and site info.

## Relationship to Basic Settings

| Item | Relationship |
|------|--------------|
| **Base URL** | Should be the primary binding visitors use (including protocol) |
| **Force SSL** | Shared `site.forceSSL` with Basic Settings; either place updates the same field |
| **Multilingual** | Binding **Language** column uses cultures from Basic Settings |

## Typical Workflow

1. Ensure root domain is registered available at org level.  
2. **New binding**: subdomain + root (or port on local).  
3. Set **redirect** and **language** as needed.  
4. Point DNS to Kooboo server, then **enable SSL** in the list.  
5. Update [Basic Settings](./basic/general.md) **Base URL** to the primary address.

## Related

| Doc | Description |
|-----|-------------|
| [Settings overview](./index.md) | Menu overview |
| [Basic Settings](./basic/) | Base URL, multilingual |
| [Integrations](./integrations/) | Third-party config unrelated to domains |
