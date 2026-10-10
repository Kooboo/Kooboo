# Basic Settings

> Menu: **Settings -> Basic Settings**  
> Deep link: `/_Admin/system/settings?SiteId={siteGuid}`

**Basic Settings** is the entry for site-level configuration: six **tabs** at the top; click bottom **Save** (or shortcut) to persist changes to the `Site` model. Unlike [Domains](../domains.md) or [Integrations](../integrations/), this page edits `Site` fields directly, not separate integration lists.

::: tip Permissions
View and edit: `site·edit`. Export site package also requires `site·export`.
:::

## How to Open

1. In [Edit Menu](../../navigation.md#permissions-and-edit-menu), enable **Settings -> Basic Settings**.  
2. Open **Settings -> Basic Settings** in the left sidebar.

<DocImage src="/cms/settings/settings-basic-tabs.png" alt="Basic Settings tab bar" width="1120" />

## Tabs and Docs

Switching tabs sets query parameter **`name`**; deep links open a specific tab.

| Tab | `name` | Doc | Main topics |
|-----|--------|-----|-------------|
| General | `basic` | [General](./general.md) | Display name, Base URL, SSL, SPA, **multilingual**, import/export package |
| Access Control | `access` | [Access Control](./access.md) | Site type, path rules, rate limits, blocks, CORS, dev password |
| Performance | `performance` | [Performance](./performance.md) | JS/CSS minify and cache, image cache, WebP, resource cache |
| SEO | `seo` | [SEO](./seo.md) | Lighthouse, Sitemap, PWA, visitor log toggle, Canonical |
| Editor | `editor` | [Editor](./editor.md) | Rich text, inline design, code suggestions, operation recording |
| Advanced | `advance` | [Advanced](./advanced.md) | Default database, SQL/code logs, Hook switches, custom settings |

Deep link examples:

```text
/_Admin/system/settings?SiteId={siteGuid}&name=basic
/_Admin/system/settings?SiteId={siteGuid}&name=advance
```

## Save and Leave

- Bottom **Save** bar (`KBottomBar`) submits unsaved changes from all tabs (`Site` POST).  
- **Save** keyboard shortcut matches global admin.  
- Leaving with unsaved changes prompts confirmation (`saveTip`).

Some panels run hooks before save (multilingual, PWA, Lighthouse, rate-limit IP serialization, etc.); server write is final.

## Suggested Reading Order

1. [General](./general.md) — site identity and multilingual  
2. [Access Control](./access.md) — security and visibility before go-live  
3. [Performance](./performance.md) · [SEO](./seo.md) — as needed by site type  
4. [Editor](./editor.md) — content/page editing experience  
5. [Advanced](./advanced.md) — ties to [Database](../../database/index.md) and [Hooks](../request-hooks.md)  

## Related

| Doc | Description |
|-----|-------------|
| [Settings overview](../index.md) | Full settings menu |
| [Domains](../domains.md) | Bind access domains (with Base URL) |
| [Development -> SPA multilingual](../../development/spa-multilingual.md) | Front-end SPA language packs (not this page’s multilingual) |
| [k.label](../../../api/label/index.md) | Multilingual Label script API |
