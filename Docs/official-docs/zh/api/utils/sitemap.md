# k.utils.sitemap

> 构建 XML Sitemap

## 概述

`k.utils.sitemap` 链式生成 [sitemaps.org](https://www.sitemaps.org/) 格式 XML。

## create()

返回可 `append` 的构建器。

**参数：** 无。

**返回：** `Sitemap` 构建器。

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

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `loc` | `string` | 是 | 页面绝对 URL |
| `options` | `AppendOptions` | 否 | Sitemap 条目配置 |

### AppendOptions

| 属性 | 类型 | 说明 |
|------|------|------|
| `lastmod` | `string` | 最后修改日期 |
| `changefreq` | `string` | 更新频率：`always`、`hourly`、`daily`、`weekly`、`monthly`、`yearly`、`never` |
| `priority` | `number` | 优先级，范围 `0.0` 到 `1.0` |
| `alternates` | `Record<string, string>` | 语言到 URL 的映射 |

**返回：** `Sitemap`，可继续链式调用。

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

输出 XML 字符串。

**参数：** 无。

**返回：** `string`。

```ts
const xml = k.utils.sitemap.create()
    .append("https://www.example.com/", { priority: 1 })
    .build()

k.response.setHeader("Content-Type", "application/xml")
return xml
```

## 相关文档

- [k.utils 概述](./index.md)
