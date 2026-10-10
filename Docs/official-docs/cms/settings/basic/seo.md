# Basic · SEO Tab

> Tab: **SEO** · `name=seo`  
> Deep link: `/_Admin/system/settings?SiteId={siteGuid}&name=seo`

Site-level **search engine and PWA** settings: Lighthouse optimization, **Sitemap** generation, **PWA** Manifest/Service Worker, **visitor log** toggle, block SEO crawlers, image **Alt** assist, and **Canonical URL** rules.

<DocImage src="/cms/settings/settings-basic-seo.png" alt="SEO tab" width="1120" />

## Lighthouse Optimization

Group **Lighthouse optimization** (`enableLighthouseOptimization`): enable built-in checks and sub-parameters (serialized as `lighthouseSettingsJson` on save). Auto-adjusts or injects HTML for common performance/SEO checks.

## Sitemap

Group **Sitemap** (`sitemapSettings.enable`):

| Field | Description |
|-------|-------------|
| **Path** | Public sitemap path (e.g. `/sitemap.xml`) |
| **Generation** | **Automatic** system generation or **Manual** TypeScript code |
| **Edit code** | Manual mode: Monaco editor for sitemap script (`sitemapSettings.code`) |

## PWA

Group **PWA**: configure **Manifest** (name, icons from [Media library](../../site/media.md), etc.) and **Service Worker** cache rules (Method, Pattern, ExpiresIn, etc.). Saves `pwa.manifest` / `pwa.serviceWorker` JSON.

## Other Switches

| Switch | Description |
|--------|-------------|
| **Visitor log** | `enableVisitorLog`: record visits ([Operations · Visitor logs](../../operations/visitor-logs.md)) |
| **Block SEO** | `blockingSeo`: restrict crawler indexing behavior |
| **Enable image Alt** | `enableImageAlt`: assist output or validate image `alt` |

## Canonical URL

**Canonical URL** group: rules for canonical links to reduce duplicate URL SEO impact (fields per UI).

## Related

- [Basic Settings overview](./index.md)  
- [Performance](./performance.md)  
- [Operations · Visitor logs](../../operations/visitor-logs.md)
