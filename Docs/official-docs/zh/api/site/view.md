# k.site.views

> 可复用视图（View）片段的增删改查与版本日志

## 概述

View 是可在页面或布局中通过 `<view id="视图名">` 引用的 HTML 片段，适合拆分 Header、卡片、列表等复用块。`k.site.views` 用于管理站点中的视图资源。

::: tip 与 pages 的区别
与 [k.site.layouts](./layout.md) 相同，`k.site.views` 提供 `add` / `all` / `get` / `update` / `updateBody` / `delete` 及版本日志等 API，**没有** `getByUrl` / `getUrl` / `getAbsUrl`（仅 [k.site.pages](./page.md) 支持按 URL 查找页面）。
:::

## TypeScript 定义（节选）

```ts
interface ViewRepository {
  add(view: ViewInput): void;
  all(): View[];
  get(nameOrId: string): View | null;
  update(view: View): void;
  updateBody(nameOrId: string, body: string): void;
  delete(nameOrId: string): void;
  getLogs(nameOrId: string): ChangeLog[] | null;
  getByLog(logId: number): View | null;
}

interface ViewInput {
  name: string;
  body: string;
}
```

## add()

创建视图。

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `view.name` | `string` | 是 | 视图名称，页面中用 `<view id="name">` 引用 |
| `view.body` | `string` | 是 | HTML 片段 |

**返回：** `void`。

```ts
k.api.post(() => {
    const stamp = Date.now().toString()
    const name = "ai-view-" + stamp

    k.site.views.add({
        name,
        body: `<div class="ai-view">${name}</div>`
    })

    const view = k.site.views.get(name)
    return {
        verified: !!view && view.name === name,
        id: view?.id
    }
})
```

## all()

返回全部视图。

**参数：** 无。

**返回：** `View[]`。

```ts
k.api.get(() => {
    const views = k.site.views.all()
    return { count: views.length, names: views.slice(0, 10).map((v) => v.name) }
})
```

## get()

按名称或 Id 获取视图。

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `nameOrId` | `string` | 是 | 视图名称或 Id |

**返回：** `View | null`。

```ts
k.api.get(() => {
    const view = k.site.views.get("hero")
    return view ? { id: view.id, name: view.name, bodyLength: view.body?.length } : null
})
```

## update()

更新完整视图对象。

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `view` | `View` | 是 | 修改后的完整视图对象 |

**返回：** `void`。

```ts
k.api.post(() => {
    const stamp = Date.now().toString()
    const name = "ai-view-upd-" + stamp

    k.site.views.add({ name, body: "<div>v1</div>" })
    const view = k.site.views.get(name)
    view.body = "<div>v2</div>"
    k.site.views.update(view)

    const after = k.site.views.get(name)
    return {
        verified: after?.body?.indexOf("v2") >= 0,
        hasV1: after?.body?.indexOf("v1") >= 0
    }
})
```

## updateBody()

仅更新 `body`。

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `nameOrId` | `string` | 是 | 视图名称或 Id |
| `body` | `string` | 是 | 新视图 HTML |

**返回：** `void`。

```ts
k.api.post(() => {
    const stamp = Date.now().toString()
    const name = "ai-view-body-" + stamp

    k.site.views.add({ name, body: "<div>before</div>" })
    k.site.views.updateBody(name, "<div>after-body</div>")

    const after = k.site.views.get(name)
    return { verified: after?.body?.indexOf("after-body") >= 0 }
})
```

## delete()

按名称或 Id 删除视图。删除后用 `get(name)` 验证。

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `nameOrId` | `string` | 是 | 视图名称或 Id |

**返回：** `void`。

```ts
k.api.post(() => {
    const stamp = Date.now().toString()
    const name = "ai-view-del-" + stamp

    k.site.views.add({ name, body: "<div>del</div>" })
    const before = k.site.views.get(name)

    k.site.views.delete(name)
    const after = k.site.views.get(name)

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
| `getByLog(logId)` | `logId: number` | `View | null` | 按日志 Id 获取历史版本 |

```ts
k.api.get(() => {
    const views = k.site.views.all()
    if (!views.length) return { error: "no views" }

    const logs = k.site.views.getLogs(views[0].id)
    return {
        verified: logs === null || Array.isArray(logs),
        logCount: logs ? logs.length : 0
    }
})
```

## View 对象（常用字段）

| 字段 | 类型 | 说明 |
|------|------|------|
| id | `string` | 视图 Id |
| name | `string` | 名称 |
| body | `string` | HTML 内容 |
| moduleId | `string` | 所属模块 Id（模块内视图时） |
| extension | `string` | 默认 `html` |
| online | `boolean` | 是否上线 |
| version | `number` | 版本号 |

## 在页面中引用

```html
<layout id="main">
  <placeholder id="Main">
    <view id="hero"></view>
  </placeholder>
</layout>
```

需已存在名为 `hero` 的视图，且 `k.site.views.get("hero")` 可返回对象。

## 相关文档

- [k.site](./)
- [k.site.layouts](./layout.md)
- [k.site.pages](./page.md)
