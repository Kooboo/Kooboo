# k.site.pages

> 站点页面的增删改查、路由 URL 与版本日志

## 概述

`k.site.pages` 管理 Kooboo **Page** 资源（页面 HTML/布局占位、路由、版本）。与全局 [k.page](../page/) 不同：后者只改**当前请求**正在渲染页的 Meta，不负责站点页面仓库。

::: tip 与 k.page 的区别
| API | 用途 |
|-----|------|
| `k.site.pages` | 站点内所有页面的 CRUD、按 URL 查找 |
| `k.page` | 当前页 `setTitle` / `setMeta` 等 |
:::

## TypeScript 定义（节选）

```ts
interface PageRepository {
  add(page: PageInput): void;
  all(): Page[];
  get(nameOrId: string): Page | null;
  getByUrl(url: string): Page | null;
  update(page: Page): void;
  updateBody(nameOrId: string, body: string): void;
  delete(nameOrId: string): void;
  getUrl(id: string): string | null;
  getAbsUrl(id: string): string | null;
  getLogs(nameOrId: string): ChangeLog[] | null;
  getByLog(logId: number): Page | null;
  getUrls(nameOrId: string): Record<string, string>;
  updateRoute(id: string, url: string, culture?: string): void;
}

interface PageInput {
  name: string;
  body: string;
  url: string;
  layoutName?: string;
}
```

## add()

创建页面并注册路由。未传 `url` 时默认 `/{name}`。

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `page.name` | `string` | 是 | 页面名称（唯一） |
| `page.body` | `string` | 是 | 页面内容，可为 `<layout>` 占位结构 |
| `page.url` | `string` | 是 | 访问路径，如 `/about` |
| `page.layoutName` | `string` | 否 | 关联布局名称 |

**返回：** `void`。

```ts
k.api.post(() => {
    const stamp = Date.now().toString()
    const name = "doc-check-" + stamp
    const url = "/doc-check-" + stamp

    k.site.pages.add({
        name,
        url,
        body: `<layout id="main"><placeholder id="Main"><h1>${name}</h1></placeholder></layout>`
    })

    const page = k.site.pages.getByUrl(url)
    return {
        created: !!page,
        id: page?.id,
        name: page?.name,
        url: page ? k.site.pages.getUrl(page.id) : null
    }
})
```

## all()

返回站点全部页面对象数组。

**参数：** 无。

**返回：** `Page[]`。

```ts
k.api.get(() => {
    const pages = k.site.pages.all()
    return { count: pages.length, names: pages.slice(0, 5).map((p) => p.name) }
})
```

## get()

按 **名称** 或 **Id**（GUID 字符串）获取页面。

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `nameOrId` | `string` | 是 | 页面名称或 Id |

**返回：** `Page | null`。

```ts
k.api.get(() => {
    const page = k.site.pages.get("home")
    return page ? { id: page.id, name: page.name } : null
})
```

## getByUrl()

按 **路由 URL** 获取页面（会解析路由别名）。

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `url` | `string` | 是 | 页面路由 URL |

**返回：** `Page | null`。

```ts
k.api.get(() => {
    const page = k.site.pages.getByUrl("/")
    return page ? { id: page.id, name: page.name } : null
})
```

## update()

用完整页面对象更新（通常先 `get` / `getByUrl` 再改字段）。

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `page` | `Page` | 是 | 修改后的完整页面对象 |

**返回：** `void`。

```ts
k.api.post(() => {
    const stamp = Date.now().toString()
    const url = "/doc-upd-" + stamp
    k.site.pages.add({
        name: "doc-upd-" + stamp,
        url,
        body: "<layout id=\"main\"><placeholder id=\"Main\"><h1>v1</h1></placeholder></layout>"
    })

    const page = k.site.pages.getByUrl(url)
    page.body = "<layout id=\"main\"><placeholder id=\"Main\"><h1>v2</h1></placeholder></layout>"
    k.site.pages.update(page)

    const after = k.site.pages.getByUrl(url)
    return {
        verified: after?.body?.indexOf("v2") >= 0,
        snippet: after?.body?.substring(0, 80)
    }
})
```

## updateBody()

仅更新 `body` 字段。

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `nameOrId` | `string` | 是 | 页面名称或 Id |
| `body` | `string` | 是 | 新页面主体 HTML |

**返回：** `void`。

```ts
k.api.post(() => {
    const stamp = Date.now().toString()
    const url = "/doc-body-" + stamp
    k.site.pages.add({
        name: "doc-body-" + stamp,
        url,
        body: "<h1>before</h1>"
    })

    const page = k.site.pages.getByUrl(url)
    k.site.pages.updateBody(page.id, "<h1>after</h1>")

    const after = k.site.pages.getByUrl(url)
    return {
        verified: after?.body?.indexOf("after") >= 0
    }
})
```

## delete()

按名称或 Id 删除页面。

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `nameOrId` | `string` | 是 | 页面名称或 Id |

**返回：** `void`。

::: warning
删除后请用 `get(nameOrId)` 确认；对已删页面调用 `getByUrl` 在部分站点上可能因路由残留触发运行时错误。
:::

```ts
k.api.post(() => {
    const stamp = Date.now().toString()
    const url = "/doc-del-" + stamp
    const name = "doc-del-" + stamp

    k.site.pages.add({ name, url, body: "<h1>del</h1>" })
    const before = k.site.pages.getByUrl(url)

    k.site.pages.delete(name)
    const after = k.site.pages.get(name)

    return {
        hadPage: !!before,
        deleted: !!before && !after
    }
})
```

## getUrl() / getAbsUrl()

根据页面 **Id** 获取相对路径或绝对 URL。

| 方法 | 参数 | 返回 | 说明 |
|------|------|------|------|
| `getUrl(id)` | `id: string` | `string | null` | 获取相对 URL |
| `getAbsUrl(id)` | `id: string` | `string | null` | 获取绝对 URL |

```ts
k.api.get(() => {
    const page = k.site.pages.all()[0]
    if (!page) return { error: "no pages" }
    return {
        id: page.id,
        relative: k.site.pages.getUrl(page.id),
        absolute: k.site.pages.getAbsUrl(page.id)
    }
})
```

## getLogs() / getByLog()

`getLogs` 返回指定页面的变更日志；`getByLog` 按日志 Id 还原该版本对象。

| 方法 | 参数 | 返回 | 说明 |
|------|------|------|------|
| `getLogs(nameOrId)` | `nameOrId: string` | `ChangeLog[] | null` | 获取版本日志 |
| `getByLog(logId)` | `logId: number` | `Page | null` | 按日志 Id 获取历史版本 |

```ts
k.api.get(() => {
    const page = k.site.pages.all()[0]
    if (!page) return { error: "no pages" }
    const logs = k.site.pages.getLogs(page.id)
    return { logCount: logs ? logs.length : 0 }
})
```

## getUrls()

返回页面在各文化下的路由路径字典。

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `nameOrId` | `string` | 是 | 页面名称或 Id |

**返回：** `Record<string, string>`。

```ts
k.api.get(() => {
    const page = k.site.pages.all()[0]
    if (!page) return { error: "no pages" }
    return k.site.pages.getUrls(page.id)
})
```

## updateRoute()

更新页面在指定文化下的路由路径。

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| id | `string` | 是 | 页面 Id |
| url | `string` | 是 | 新路径 |
| culture | `string` | 否 | 文化代码，默认站点主文化 |

**返回：** `void`。

```ts
// 多语言站点中调整某语言下的 URL
k.api.post(() => {
    const page = k.site.pages.get("about")
    if (!page) return { error: "page not found" }
    k.site.pages.updateRoute(page.id, "/about-us", "en")
    return k.site.pages.getUrls(page.id)
})
```

## Page 对象（常用字段）

| 字段 | 类型 | 说明 |
|------|------|------|
| id | `string` | 页面 Id |
| name | `string` | 名称 |
| body | `string` | 页面主体 |
| layoutName | `string` | 布局名 |
| url | — | 通过 `getUrl(id)` 获取路由 |
| online | `boolean` | 是否上线 |
| version | `number` | 版本号 |

## 相关文档

- [k.site](./) — 站点资源总览
- [k.site.layouts](./layout.md) — 布局
- [k.page](../page/) — 当前页 Meta
