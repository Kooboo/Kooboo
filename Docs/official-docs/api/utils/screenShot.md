# k.utils.screenShot

> Web pages and page screenshots

## Overview

`k.utils.screenShot` Generates a PNG byte array for an absolute URL, site page, or HTML string.

## take()

Screenshot of **absolute URL**; supports `width`, `height`, `fullPage` and scaling parameter overloading.

| Parameter | Type | Required | Description |
|------|------|------|------|
| `absoluteUrl` | `string` | Yes | Publicly accessible absolute URL |
| `width` | `number` | No | Viewport width, default 1366 |
| `height` | `number` | No | Viewport height, default 768 |
| `scaleWidth` | `number` | No | Scaled output width |
| `scaleHeight` | `number` | No | Scaled output height |
| `fullPage` | `boolean` | No | Whether to capture the full page |

**Returns:** `number[] \| null`, PNG bytes.

```ts
const png = k.utils.screenShot.take("https://www.kooboo.com", 1280, 720, true)
```

## takeLocalPageUrl() / takePage()

Take a screenshot of the **relative path** or **page name** of the site.

| Method | Parameter | Return | Description |
|------|------|------|------|
| `takeLocalPageUrl(url, width?, height?, fullPage?)` | `string, number?, number?, boolean?` | `number[] \| null` | Capture by site-relative URL |
| `takePage(pageName, width?, height?, fullPage?)` | `string, number?, number?, boolean?` | `number[] \| null` | Capture by page name |

```ts
const png = k.utils.screenShot.takeLocalPageUrl("/products", 1280, 720, true)
k.file.writeBinary("shots/products.png", png)
```

## takeFromHTML()

Screenshot of HTML string after rendering.

| Parameter | Type | Required | Description |
|------|------|------|------|
| `htmlCode` | `string` | Yes | HTML to render and capture |
| `width` | `number` | No | Viewport width |
| `height` | `number` | No | Viewport height |
| `scaleWidth` | `number` | No | Scaled output width |
| `scaleHeight` | `number` | No | Scaled output height |
| `fullPage` | `boolean` | No | Whether to capture the full page |

**Returns:** `number[] \| null`, PNG bytes.

```ts
const html = "<html><body><h1>Invoice</h1></body></html>"
const png = k.utils.screenShot.takeFromHTML(html, 800, 600, false)
k.file.writeBinary("shots/invoice.png", png)
```

## Related Docs

- [k.utils.puppeteer](./puppeteer.md)
