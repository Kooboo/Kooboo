# k.google

> Google PageSpeed/Lighthouse and Firebase services

## Overview

`k.google` groups Google-related capabilities. PageSpeed/Lighthouse methods are available directly on `k.google`, while Firebase Admin capabilities are available through `k.google.firebase`.

| Capability | KScript Entry | Description |
|------------|---------------|-------------|
| PageSpeed / Lighthouse | Root methods such as `k.google.lHRJson()` | Generate, parse, and store Lighthouse reports |
| Firebase | `k.google.firebase` | Firebase authentication and user management |

## lHRJson() / lHRUrl()

Generate Lighthouse JSON or an online report URL for an absolute URL.

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `absoluteUrl` | `string` | Yes | Absolute URL to analyze |
| `mobileDevice` | `boolean` | Yes | Compatibility parameter; the current report request does not switch strategy based on this value |

**Returns:** `lHRJson()` returns a Lighthouse JSON string; `lHRUrl()` returns an openable report URL.

```ts
const json = k.google.lHRJson("https://www.kooboo.com", false)
const report = k.google.parseJson(json)
const summary = report.getSummary()

return {
    performance: summary.performance,
    accessibility: summary.accessibility,
    seo: summary.seo
}
```

If you only need an openable Lighthouse report URL:

```ts
const reportUrl = k.google.lHRUrl("https://www.kooboo.com", true)
return { reportUrl }
```

## lHRUrlFromJson()

Convert existing Lighthouse JSON into a report URL that can be opened in Kooboo.

```ts
const json = k.google.lHRJson("https://www.kooboo.com", false)
const reportUrl = k.google.lHRUrlFromJson(json)
return { reportUrl }
```

## parseJson() / saveJson() / getJson()

Parse Lighthouse JSON, store it, and retrieve it by ID.

| Method | Parameter | Return | Description |
|--------|-----------|--------|-------------|
| `parseJson(json)` | `string` | `FullLHRObj` | Parse Lighthouse JSON |
| `saveJson(json)` | `string` | `Guid` | Store JSON and return its ID |
| `getJson(id)` | `Guid` | `string \| null` | Read stored JSON by ID |

```ts
const json = k.google.lHRJson("https://www.kooboo.com", false)
const id = k.google.saveJson(json)
const saved = k.google.getJson(id)

return {
    id,
    saved: !!saved,
    summary: k.google.parseJson(saved).getSummary()
}
```

## shortLhrReportUrl()

Generate a site-local short report URL from JSON.

```ts
const json = k.google.lHRJson("https://www.kooboo.com", false)
const url = k.google.shortLhrReportUrl(json)

return {
    url,
    fullUrl: k.site.info.makeAbsUrl(url)
}
```

## Related Docs

- [k.utils.ninjible](../utils/ninjible.md) — Site migration and Lighthouse comparison
