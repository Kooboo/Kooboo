# k.utils.puppeteer

> 无头浏览器截图（仅非线上环境）

## 概述

`k.utils.puppeteer` 使用 Puppeteer 对 URL 截图并保存到站点文件。

::: warning 环境限制
在 **Kooboo 线上服务器**（`IsOnlineServer`）调用会抛出异常，仅适合自建/本地环境。
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

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `url` | `string` | 是 | 目标地址 |
| `savePath` | `string` | 是 | 站点内保存路径 |
| `options` | `ScreenshotOptions` | 否 | 截图选项 |

### ScreenshotOptions

| 属性 | 类型 | 说明 |
|------|------|------|
| `delaySeconds` | `number` | 等待秒数，默认 3 |
| `width` | `number` | 视口宽度，默认 1366 |
| `height` | `number` | 视口高度，默认 768 |
| `fullPage` | `boolean` | 是否截取完整页面 |

**返回：** `void`，截图会写入 `savePath`。

## 相关文档

- [k.utils.screenShot](./screenShot.md) — 内置截图服务
