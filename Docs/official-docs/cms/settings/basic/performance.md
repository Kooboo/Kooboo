# Basic · Performance Tab

> Tab: **Performance** · `name=performance`  
> Deep link: `/_Admin/system/settings?SiteId={siteGuid}&name=performance`

Optimizes static asset delivery: **JS/CSS minification**, browser caching, **image** cache policy, automatic **WebP** conversion, and on private/local servers **resource cache** (styles, scripts, images, content).

<DocImage src="/cms/settings/settings-basic-performance.png" alt="Performance tab" width="1120" />

## JS / CSS / Video

| Switch | Description |
|--------|-------------|
| **JS/CSS minification** | Compress scripts and styles on publish |
| **JS/CSS browser cache** | Cache headers for JS/CSS responses |
| **Video browser cache** | Cache headers for video assets |

## Image Browser Cache

Separate group **Image browser cache** (`enableImageBrowserCache`), one of two modes:

| Mode | Description |
|------|-------------|
| **Version control** | `imageCacheDays=0`: invalidate via version/query; good for frequently updated images |
| **Fixed days** | Cache TTL in **days** (≥1) |

## Automatic WebP Conversion

**Automatically convert images to WebP** (`automateCovertImageToWebp`): generate WebP on upload/processing to reduce size (see tooltip).

## Resource Cache (Private / Local Server)

When the environment is **not online public cloud** or is a **private server**, the **Resource cache** group appears:

| Item | Description |
|------|-------------|
| Master switch | `enableResourceCache` |
| Types | **Styles**, **Scripts**, **Images**, **Content** (bit flags `resourceCaches`) |

Server-side cache for rendered output or assets to reduce repeat work.

## Related

- [Basic Settings overview](./index.md)  
- [SEO](./seo.md) (Lighthouse optimization and performance checks)
