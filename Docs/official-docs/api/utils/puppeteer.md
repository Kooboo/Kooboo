# k.utils.puppeteer

> Headless browser screenshot (non-online environment only)

## Overview

`k.utils.puppeteer` Use Puppeteer to screenshot the URL and save it to a site file.

::: warning environment restrictions
Calling **Kooboo Online Server** (`IsOnlineServer`) will throw an exception and is only suitable for self-built/local environments.
:::

## screenshot()

```ts
k.utils.puppeteer.screenshot("https://example.com", "shots/page.png", {
    width: 1366,
    height: 768,
    fullPage: true,
    delaySeconds: 3
})
```

| Parameter | Type | Required | Description |
|------|------|------|------|
| `url` | `string` | Yes | Target URL |
| `savePath` | `string` | Yes | Save path inside the site |
| `options` | `ScreenshotOptions` | No | Screenshot options |

### ScreenshotOptions

| Property | Type | Description |
|------|------|------|
| `delaySeconds` | `number` | Wait time in seconds, default 3 |
| `width` | `number` | Viewport width, default 1366 |
| `height` | `number` | Viewport height, default 768 |
| `fullPage` | `boolean` | Whether to capture the full page |

**Returns:** `void`; the screenshot is written to `savePath`.

## Related Docs

- [k.utils.screenShot](./screenShot.md) — Built-in screenshot service
