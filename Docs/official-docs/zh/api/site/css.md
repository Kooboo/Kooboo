# k.site.styles

> 站点 CSS 样式资源的增删改查、路由 URL 与版本日志

## 概述

`k.site.styles` 管理站点中的 **Style** 资源（独立 CSS 文件）。样式通过路由 URL 对外提供访问（如 `/site.css`），可在页面或布局中引用。

::: tip 与 layouts / views 的区别
`k.site.styles` 支持 **`getByUrl`**、**`getUrl`**、**`getAbsUrl`** 等路由相关 API（与 [k.site.pages](./page.md) 同类）；[k.site.layouts](./layout.md)、[k.site.views](./view.md) 无路由 API。
:::

侧边栏条目「css」即本 API。

## TypeScript 定义（节选）

```ts
interface StyleRepository {
  add(style: StyleInput): void;
  all(): Style[];
  get(nameOrId: string): Style | null;
  getByUrl(url: string): Style | null;
  update(style: Style): void;
  updateBody(nameOrId: string, body: string): void;
  delete(nameOrId: string): void;
  getUrl(id: string): string | null;
  getAbsUrl(id: string): string | null;
  getLogs(nameOrId: string): ChangeLog[] | null;
  getByLog(logId: number): Style | null;
}

interface StyleInput {
  name: string;
  body: string;
  url?: string;
}
```

## add()

创建样式并注册路由。未传 `url` 时默认为 `/{name}`，且若路径不含扩展名会自动追加样式扩展名（通常为 `.css`）。

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `style.name` | `string` | 是 | 样式名称（唯一） |
| `style.body` | `string` | 是 | CSS 内容 |
| `style.url` | `string` | 否 | 访问路径，如 `/assets/theme.css` |

**返回：** `void`。

```ts
k.api.post(() => {
    const stamp = Date.now().toString()
    const name = "ai-style-" + stamp
    const url = "/ai-style-" + stamp + ".css"

    k.site.styles.add({
        name,
        url,
        body: `.ai-style-${stamp} { color: #333; }`
    })

    const style = k.site.styles.getByUrl(url)
    return {
        verified: !!style && style.name === name,
        id: style?.id,
        route: style ? k.site.styles.getUrl(style.id) : null
    }
})
```

## all()

返回全部样式。

**参数：** 无。

**返回：** `Style[]`。

```ts
k.api.get(() => {
    const styles = k.site.styles.all()
    return { count: styles.length, names: styles.slice(0, 10).map((s) => s.name) }
})
```

## get()

按名称或 Id 获取样式。

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `nameOrId` | `string` | 是 | 样式名称或 Id |

**返回：** `Style | null`。

```ts
k.api.get(() => {
    const style = k.site.styles.get("site")
    return style ? { id: style.id, name: style.name, extension: style.extension } : null
})
```

## getByUrl()

按 **路由 URL** 获取样式（例如 `/main.css`）。

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `url` | `string` | 是 | 样式路由 URL |

**返回：** `Style | null`。

```ts
k.api.get(() => {
    const style = k.site.styles.getByUrl("/main.css")
    return style ? { id: style.id, name: style.name } : null
})
```

## update()

更新完整样式对象（通常先 `get` / `getByUrl` 再改 `body`）。

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `style` | `Style` | 是 | 修改后的完整样式对象 |

**返回：** `void`。

```ts
k.api.post(() => {
    const stamp = Date.now().toString()
    const name = "ai-style-upd-" + stamp
    const url = "/ai-style-upd-" + stamp + ".css"

    k.site.styles.add({ name, url, body: ".v1 {}" })
    const style = k.site.styles.getByUrl(url)
    style.body = ".v2 {}"
    k.site.styles.update(style)

    const after = k.site.styles.getByUrl(url)
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
| `nameOrId` | `string` | 是 | 样式名称或 Id |
| `body` | `string` | 是 | 新 CSS 内容 |

**返回：** `void`。

```ts
k.api.post(() => {
    const stamp = Date.now().toString()
    const name = "ai-style-body-" + stamp
    const url = "/ai-style-body-" + stamp + ".css"

    k.site.styles.add({ name, url, body: ".before {}" })
    k.site.styles.updateBody(name, ".after-body {}")

    const after = k.site.styles.getByUrl(url)
    return { verified: after?.body?.indexOf("after-body") >= 0 }
})
```

## delete()

按名称或 Id 删除。删除后用 `get(name)` 验证，勿在删除后依赖 `getByUrl`。

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `nameOrId` | `string` | 是 | 样式名称或 Id |

**返回：** `void`。

```ts
k.api.post(() => {
    const stamp = Date.now().toString()
    const name = "ai-style-del-" + stamp
    const url = "/ai-style-del-" + stamp + ".css"

    k.site.styles.add({ name, url, body: ".del {}" })
    const before = k.site.styles.getByUrl(url)

    k.site.styles.delete(name)
    const after = k.site.styles.get(name)

    return {
        verified: !!before && !after,
        hadBefore: !!before
    }
})
```

## getUrl() / getAbsUrl()

| 方法 | 参数 | 返回 | 说明 |
|------|------|------|------|
| `getUrl(id)` | `id: string` | `string | null` | 获取相对 URL |
| `getAbsUrl(id)` | `id: string` | `string | null` | 获取绝对 URL |

```ts
k.api.get(() => {
    const styles = k.site.styles.all()
    for (const style of styles) {
        const rel = k.site.styles.getUrl(style.id)
        if (!rel) continue
        const abs = k.site.styles.getAbsUrl(style.id)
        return { relative: rel, absolute: abs }
    }
    return null
})
```

仅注册了路由的样式能返回 URL；页面内嵌样式可能没有独立路由。

## getLogs() / getByLog()

| 方法 | 参数 | 返回 | 说明 |
|------|------|------|------|
| `getLogs(nameOrId)` | `nameOrId: string` | `ChangeLog[] | null` | 获取版本日志 |
| `getByLog(logId)` | `logId: number` | `Style | null` | 按日志 Id 获取历史版本 |

```ts
k.api.get(() => {
    const styles = k.site.styles.all()
    if (!styles.length) return { error: "no styles" }

    const logs = k.site.styles.getLogs(styles[0].id)
    return {
        verified: logs === null || Array.isArray(logs),
        logCount: logs ? logs.length : 0
    }
})
```

## Style 对象（常用字段）

| 字段 | 类型 | 说明 |
|------|------|------|
| id | `string` | 样式 Id |
| name | `string` | 名称 |
| body | `string` | CSS 内容 |
| extension | `string` | 扩展名，通常 `css` |
| media | `string` | 媒体查询（如 `screen`） |
| online | `boolean` | 是否上线 |
| version | `number` | 版本号 |

## 相关文档

- [k.site](./)
- [k.site.scripts](./js.md)
- [k.site.pages](./page.md)
