# k.page

> 当前 HTTP 请求所渲染页面的 `<title>` 与 `<meta>` 绑定

## 概述

`k.page` 在**当前请求的渲染上下文**中写入页头绑定，由布局/页面渲染阶段输出到 HTML 的 `<title>` 与 `<meta>`。适用于根据接口数据、用户状态动态改标题或 SEO/分享标签。

::: tip 与 k.site.pages、k.response.meta 的区别
| API | 作用 |
|-----|------|
| **`k.page`** | 当前页 `setTitle`、`setNameMeta`、`setPropertyMeta` 等（本页） |
| **`k.site.pages`** | 站点内 Page 资源的增删改查，见 [site/page.md](../site/page.md) |
| **`k.response.meta`** | 同一套页头绑定的另一入口：`meta.title`、`meta.setMeta(name, content)`，见 [k.response](../response/) |

`k.page.setNameMeta("description", "…")` 与 `k.response.meta.setMeta("description", "…")` 写入同一绑定列表；按团队习惯择一即可。
:::

须在**会参与页面 HTML 渲染**的脚本中调用（例如页面关联的 CodeBlock、布局脚本）。纯 API 返回 JSON 且不走页面模板的请求，调用通常不会影响响应体。

## TypeScript 定义（节选）

```ts
interface KPage {
  setTitle(value: string): void;
  setCharsetMeta(value: string): void;
  setNameMeta(name: string, content: string): void;
  setPropertyMeta(property: string, content: string): void;
  setHttpEquivMeta(equiv: string, content: string): void;
}
```

## setTitle()

设置或覆盖当前页的 `<title>` 文本。

```ts
k.page.setTitle("产品详情 - 我的站点")
```

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `value` | `string` | 是 | 标题文本 |

**返回：** `void`。

## setCharsetMeta()

设置文档字符集 meta（写入 `charset` 绑定）。

```ts
k.page.setCharsetMeta("utf-8")
```

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `value` | `string` | 是 | 字符集，空字符串时忽略 |

**返回：** `void`。

## setNameMeta()

设置 `<meta name="…" content="…">`。

```ts
k.page.setNameMeta("description", "站点描述")
k.page.setNameMeta("keywords", "kooboo, cms")
```

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `name` | `string` | 是 | `name` 属性 |
| `content` | `string` | 是 | `content` 属性 |

**返回：** `void`。

同名 `name` 已存在时会**更新** content，否则追加绑定。

## setPropertyMeta()

设置 `<meta property="…" content="…">`（Open Graph 等）。

```ts
k.page.setPropertyMeta("og:title", "分享标题")
k.page.setPropertyMeta("og:image", "https://example.com/cover.jpg")
```

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `property` | `string` | 是 | `property` 属性 |
| `content` | `string` | 是 | `content` 属性 |

**返回：** `void`。

## setHttpEquivMeta()

设置 `<meta http-equiv="…" content="…">`。

```ts
k.page.setHttpEquivMeta("refresh", "30;url=/next")
k.page.setHttpEquivMeta("X-UA-Compatible", "IE=edge")
```

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `equiv` | `string` | 是 | `http-equiv` 属性 |
| `content` | `string` | 是 | `content` 属性 |

**返回：** `void`。

## 示例：按路由参数设置 SEO

```ts
// 页面或布局 CodeBlock
const productName = k.request.queryString.name || "商品"
k.page.setTitle(productName + " - 商城")
k.page.setNameMeta("description", "购买 " + productName)
k.page.setPropertyMeta("og:title", productName)
```

## 相关文档

- [k.site.pages](../site/page.md) — 站点页面资源管理
- [k.response](../response/) — `k.response.meta` 读写页头绑定
- [k.request](../request/) — 当前请求参数
