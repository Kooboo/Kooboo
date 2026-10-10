# k.utils.inlineHtml

> 将页面或 URL 资源内联为 data URI

## 概述

`k.utils.inlineHtml` 把页面或相对 URL 中的外部资源（如图片、CSS 背景）转为内联 data URI，便于导出单文件 HTML。

## convertPageInline()

按**页面名称**转换。

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `pageName` | `string` | 是 | 页面名称 |

**返回：** `string \| null`，资源已内联的 HTML；页面不存在时返回 `null`。

```ts
const html = k.utils.inlineHtml.convertPageInline("Home")
```

## convertPageUrlInline()

按**页面相对 URL** 转换。

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `pageRelativeUrl` | `string` | 是 | 页面相对 URL，如 `/about` |

**返回：** `string \| null`，资源已内联的 HTML；页面不存在时返回 `null`。

```ts
const html = k.utils.inlineHtml.convertPageUrlInline("/about")
```

## 相关文档

- [k.site.pages](../site/page.md)
