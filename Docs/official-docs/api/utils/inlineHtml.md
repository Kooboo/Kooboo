# k.utils.inlineHtml

> Inline a page or URL resource as a data URI

## Overview

`k.utils.inlineHtml` Converts external resources (such as images, CSS backgrounds) in pages or relative URLs to inline data URIs for easy export of single-file HTML.

## convertPageInline()

Convert by **Page Name**.

| Parameter | Type | Required | Description |
|------|------|------|------|
| `pageName` | `string` | Yes | Page name |

**Returns:** `string \| null`, HTML with resources inlined; returns `null` when the page does not exist.

```ts
const html = k.utils.inlineHtml.convertPageInline("Home")
```

## convertPageUrlInline()

Convert by **Page Relative URL**.

| Parameter | Type | Required | Description |
|------|------|------|------|
| `pageRelativeUrl` | `string` | Yes | Page relative URL, such as `/about` |

**Returns:** `string \| null`, HTML with resources inlined; returns `null` when the page does not exist.

```ts
const html = k.utils.inlineHtml.convertPageUrlInline("/about")
```

## Related Docs

- [k.site.pages](../site/page.md)
