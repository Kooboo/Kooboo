# k.site.layouts

> 站点布局（Layout）资源的增删改查与版本日志

## 概述

布局是页面的 HTML 骨架，通过 `k-placeholder` 定义占位区域；页面在 `body` 里用 `<layout id="布局名">` 引用。`k.site.layouts` 管理布局仓库中的 Layout 对象。

::: tip 与 pages 的区别
`k.site.layouts` 提供 `add` / `all` / `get` / `update` / `updateBody` / `delete` 及版本日志等 API，**没有** `getByUrl` / `getUrl` / `getAbsUrl`（仅 [k.site.pages](./page.md) 支持按 URL 查找与生成链接）。
:::

## TypeScript 定义（节选）

```ts
interface LayoutRepository {
  add(layout: LayoutInput): void;
  all(): Layout[];
  get(nameOrId: string): Layout | null;
  update(layout: Layout): void;
  updateBody(nameOrId: string, body: string): void;
  delete(nameOrId: string): void;
  getLogs(nameOrId: string): ChangeLog[] | null;
  getByLog(logId: number): Layout | null;
}

interface LayoutInput {
  name: string;
  body: string;
}
```

## add()

创建布局。

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `layout.name` | `string` | 是 | 布局名称，页面引用时用 `layout id` |
| `layout.body` | `string` | 是 | HTML，含 `k-placeholder` 或占位 div |

**返回：** `void`。

```ts
k.api.post(() => {
    const stamp = Date.now().toString()
    const name = "ai-layout-" + stamp

    k.site.layouts.add({
        name,
        body: `<!DOCTYPE html><html><body><div k-placeholder="Main">Layout ${name}</div></body></html>`
    })

    const layout = k.site.layouts.get(name)
    return {
        verified: !!layout && layout.name === name,
        id: layout?.id
    }
})
```

## all()

返回全部布局。

**参数：** 无。

**返回：** `Layout[]`。

```ts
k.api.get(() => {
    const layouts = k.site.layouts.all()
    return { count: layouts.length, names: layouts.slice(0, 5).map((l) => l.name) }
})
```

## get()

按名称或 Id 获取布局。

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `nameOrId` | `string` | 是 | 布局名称或 Id |

**返回：** `Layout | null`。

```ts
k.api.get(() => {
    const layout = k.site.layouts.get("main")
    return layout ? { id: layout.id, name: layout.name } : null
})
```

## update()

更新完整布局对象（通常先 `get` 再改 `body`）。

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `layout` | `Layout` | 是 | 修改后的完整布局对象 |

**返回：** `void`。

```ts
k.api.post(() => {
    const stamp = Date.now().toString()
    const name = "ai-layout-upd-" + stamp

    k.site.layouts.add({
        name,
        body: `<div k-placeholder="Main">v1</div>`
    })

    const layout = k.site.layouts.get(name)
    layout.body = `<div k-placeholder="Main">v2</div>`
    k.site.layouts.update(layout)

    const after = k.site.layouts.get(name)
    return {
        verified: after?.body?.indexOf("v2") >= 0,
        hasV1: after?.body?.indexOf("v1") >= 0
    }
})
```

## updateBody()

仅更新 `body` 字段。

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `nameOrId` | `string` | 是 | 布局名称或 Id |
| `body` | `string` | 是 | 新布局 HTML |

**返回：** `void`。

```ts
k.api.post(() => {
    const stamp = Date.now().toString()
    const name = "ai-layout-body-" + stamp

    k.site.layouts.add({ name, body: "<div>before</div>" })
    k.site.layouts.updateBody(name, "<div>after-body</div>")

    const after = k.site.layouts.get(name)
    return { verified: after?.body?.indexOf("after-body") >= 0 }
})
```

## delete()

按名称或 Id 删除布局。

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `nameOrId` | `string` | 是 | 布局名称或 Id |

**返回：** `void`。

::: warning
删除后用 `get(nameOrId)` 确认即可，不要依赖不存在的 `getByUrl`。
:::

```ts
k.api.post(() => {
    const stamp = Date.now().toString()
    const name = "ai-layout-del-" + stamp

    k.site.layouts.add({ name, body: "<div>del</div>" })
    const before = k.site.layouts.get(name)

    k.site.layouts.delete(name)
    const after = k.site.layouts.get(name)

    return {
        verified: !!before && !after,
        hadBefore: !!before
    }
})
```

## getLogs() / getByLog()

| 方法 | 参数 | 返回 | 说明 |
|------|------|------|------|
| `getLogs(nameOrId)` | `nameOrId: string` | `ChangeLog[] | null` | 获取版本日志 |
| `getByLog(logId)` | `logId: number` | `Layout | null` | 按日志 Id 获取历史版本 |

```ts
k.api.get(() => {
    const layouts = k.site.layouts.all()
    if (!layouts.length) return { error: "no layouts" }

    const logs = k.site.layouts.getLogs(layouts[0].id)
    return {
        verified: logs === null || Array.isArray(logs),
        logCount: logs ? logs.length : 0
    }
})
```

`getByLog(logId)` 按版本日志 Id 还原布局快照，用法与 [k.site.pages](./page.md) 相同。

## Layout 对象（常用字段）

| 字段 | 类型 | 说明 |
|------|------|------|
| id | `string` | 布局 Id |
| name | `string` | 名称 |
| body | `string` | HTML 内容 |
| extension | `string` | 通常为 `html` |
| online | `boolean` | 是否上线 |
| version | `number` | 版本号 |

## 与页面配合

页面 `body` 引用布局示例：

```html
<layout id="main">
  <placeholder id="Main">
    <h1>页面内容</h1>
  </placeholder>
</layout>
```

需先在 `k.site.layouts` 中存在名为 `main` 的布局，且布局 body 内定义了 `k-placeholder="Main"`。

## 相关文档

- [k.site](./)
- [k.site.pages](./page.md)
- [k.site.views](./view.md)
