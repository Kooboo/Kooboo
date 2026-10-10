# k.utils.screenShot

> 网页与页面截图

## 概述

`k.utils.screenShot` 对绝对 URL、站点页面或 HTML 字符串生成 PNG 字节数组。

## take()

对**绝对 URL** 截图；支持 `width`、`height`、`fullPage` 及缩放参数重载。

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `absoluteUrl` | `string` | 是 | 可公网访问的绝对 URL |
| `width` | `number` | 否 | 视口宽度，默认 1366 |
| `height` | `number` | 否 | 视口高度，默认 768 |
| `scaleWidth` | `number` | 否 | 缩放输出宽度 |
| `scaleHeight` | `number` | 否 | 缩放输出高度 |
| `fullPage` | `boolean` | 否 | 是否截取完整页面 |

**返回：** `number[] \| null`，PNG 字节。

```ts
const png = k.utils.screenShot.take("https://www.kooboo.com", 1280, 720, true)
```

## takeLocalPageUrl() / takePage()

对站点**相对路径**或**页面名称**截图。

| 方法 | 参数 | 返回 | 说明 |
|------|------|------|------|
| `takeLocalPageUrl(url, width?, height?, fullPage?)` | `string, number?, number?, boolean?` | `number[] \| null` | 按站点相对 URL 截图 |
| `takePage(pageName, width?, height?, fullPage?)` | `string, number?, number?, boolean?` | `number[] \| null` | 按页面名称截图 |

```ts
const png = k.utils.screenShot.takeLocalPageUrl("/products", 1280, 720, true)
k.file.writeBinary("shots/products.png", png)
```

## takeFromHTML()

对 HTML 字符串渲染后截图。

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `htmlCode` | `string` | 是 | 要渲染截图的 HTML |
| `width` | `number` | 否 | 视口宽度 |
| `height` | `number` | 否 | 视口高度 |
| `scaleWidth` | `number` | 否 | 缩放输出宽度 |
| `scaleHeight` | `number` | 否 | 缩放输出高度 |
| `fullPage` | `boolean` | 否 | 是否截取完整页面 |

**返回：** `number[] \| null`，PNG 字节。

```ts
const html = "<html><body><h1>Invoice</h1></body></html>"
const png = k.utils.screenShot.takeFromHTML(html, 800, 600, false)
k.file.writeBinary("shots/invoice.png", png)
```

## 相关文档

- [k.utils.puppeteer](./puppeteer.md)
