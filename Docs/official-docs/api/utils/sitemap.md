# k.utils.sitemap

> Build XML Sitemap

## Overview

`k.utils.sitemap` chain generates [sitemaps.org](https://www.sitemaps.org/) format XML.

## create()

Returns a builder that can be `append`.

**Parameters:** None.

**Returns:** `Sitemap` builder.

```ts
const xml = k.utils.sitemap.create()
    .append("https://www.example.com/page1", {
        lastmod: "2024-01-01",
        changefreq: "weekly",
        priority: 0.8
    })
    .append("https://www.example.com/page2", {
        alternates: { en: "https://www.example.com/en", zh: "https://www.example.com/zh" }
    })
    .build()
```

## append()

| Parameter | Type | Required | Description |
|------|------|------|------|
| `loc` | `string` | Yes | Page absolute URL |
| `options` | `AppendOptions` | No | Sitemap entry options |

### AppendOptions

| Property | Type | Description |
|------|------|------|
| `lastmod` | `string` | Last modified date |
| `changefreq` | `string` | Update frequency: `always`, `hourly`, `daily`, `weekly`, `monthly`, `yearly`, `never` |
| `priority` | `number` | Priority, from `0.0` to `1.0` |
| `alternates` | `Record<string, string>` | Mapping from language to URL |

**Returns:** `Sitemap`, allowing chained calls.

```ts
const sitemap = k.utils.sitemap.create()
sitemap.append("https://www.example.com/blog/hello", {
    lastmod: "2024-01-01",
    changefreq: "weekly",
    priority: 0.7
})

return sitemap.build()
```

## build()

Output XML string.

**Parameters:** None.

**Returns:** `string`.

```ts
const xml = k.utils.sitemap.create()
    .append("https://www.example.com/", { priority: 1 })
    .build()

k.response.setHeader("Content-Type", "application/xml")
return xml
```

## Related Docs

- [k.utils Overview](./index.md)
