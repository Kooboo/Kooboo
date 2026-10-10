# k.utils.template

> 视图与页面渲染

## 概述

`k.utils.template` 在服务端渲染 Kooboo **View** 或 **Page** HTML。

## render()

渲染视图标记或视图 Id 字符串，返回 HTML。

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `view` | `string` | 是 | 视图标记、视图名称或视图 Id |

**返回：** `string`。

```ts
const html = k.utils.template.render("<view id=\"...\"></view>")
```

## executeView()

执行视图（副作用写入当前响应上下文，无字符串返回）。

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `viewNameOrId` | `string` | 是 | 视图名称或 Id |

**返回：** `void`。

```ts
k.utils.template.executeView("ProductCard")
```

## renderView()

按视图名称或 Id 渲染 HTML。

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `viewNameOrId` | `string` | 是 | 视图名称或 Id |

**返回：** `string \| null`。

```ts
const html = k.utils.template.renderView("ProductCard")
```

## renderPage()

按页面 URL 渲染整页 HTML。

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `url` | `string` | 是 | 站点页面 URL，可包含查询字符串 |

**返回：** `string`。

```ts
const pageHtml = k.utils.template.renderPage("/products")
```

## 相关文档

- [k.site.views](../site/view.md)
- [k.site.pages](../site/page.md)
