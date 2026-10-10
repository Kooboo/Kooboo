# k.site.scripts

> 站点 JavaScript 资源的增删改查、路由 URL 与版本日志

## 概述

`k.site.scripts` 管理站点中的 **Script** 资源（独立 JS 文件）。脚本通过路由 URL 对外提供访问（如 `/app.js`），可在页面或布局中引用。

::: tip 与 layouts / views 的区别
`k.site.scripts` 支持 **`getByUrl`**、**`getUrl`**、**`getAbsUrl`** 等路由相关 API（与 [k.site.pages](./page.md) 同类）；[k.site.layouts](./layout.md)、[k.site.views](./view.md) 无路由 API。
:::

侧边栏条目「js」即本 API。

## TypeScript 定义（节选）

```ts
interface ScriptRepository {
  add(script: ScriptInput): void;
  all(): Script[];
  get(nameOrId: string): Script | null;
  getByUrl(url: string): Script | null;
  update(script: Script): void;
  updateBody(nameOrId: string, body: string): void;
  delete(nameOrId: string): void;
  getUrl(id: string): string | null;
  getAbsUrl(id: string): string | null;
  getLogs(nameOrId: string): ChangeLog[] | null;
  getByLog(logId: number): Script | null;
}

interface ScriptInput {
  name: string;
  body: string;
  url?: string;
  async?: boolean;
  defer?: boolean;
}
```

## add()

创建脚本并注册路由。未传 `url` 时默认为 `/{name}`，且若路径不含扩展名会自动追加脚本扩展名（通常为 `.js`）。

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `script.name` | `string` | 是 | 脚本名称（唯一） |
| `script.body` | `string` | 是 | JavaScript 内容 |
| `script.url` | `string` | 否 | 访问路径，如 `/assets/app.js` |
| `script.async` | `boolean` | 否 | 是否 async 加载 |
| `script.defer` | `boolean` | 否 | 是否 defer 加载 |

**返回：** `void`。

```ts
k.api.post(() => {
    const stamp = Date.now().toString()
    const name = "ai-script-" + stamp
    const url = "/ai-script-" + stamp + ".js"

    k.site.scripts.add({
        name,
        url,
        body: `console.log("ai-script-${stamp}");`
    })

    const script = k.site.scripts.getByUrl(url)
    return {
        verified: !!script && script.name === name,
        id: script?.id,
        route: script ? k.site.scripts.getUrl(script.id) : null
    }
})
```

## all()

返回全部脚本。

**参数：** 无。

**返回：** `Script[]`。

```ts
k.api.get(() => {
    const scripts = k.site.scripts.all()
    return { count: scripts.length, names: scripts.slice(0, 10).map((s) => s.name) }
})
```

## get()

按名称或 Id 获取脚本。

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `nameOrId` | `string` | 是 | 脚本名称或 Id |

**返回：** `Script | null`。

```ts
k.api.get(() => {
    const script = k.site.scripts.get("main")
    return script ? { id: script.id, name: script.name, extension: script.extension } : null
})
```

## getByUrl()

按 **路由 URL** 获取脚本（例如 `/main.js`）。

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `url` | `string` | 是 | 脚本路由 URL |

**返回：** `Script | null`。

```ts
k.api.get(() => {
    const script = k.site.scripts.getByUrl("/main.js")
    return script ? { id: script.id, name: script.name } : null
})
```

## update()

更新完整脚本对象。

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `script` | `Script` | 是 | 修改后的完整脚本对象 |

**返回：** `void`。

```ts
k.api.post(() => {
    const stamp = Date.now().toString()
    const name = "ai-script-upd-" + stamp
    const url = "/ai-script-upd-" + stamp + ".js"

    k.site.scripts.add({ name, url, body: "// v1" })
    const script = k.site.scripts.getByUrl(url)
    script.body = "// v2"
    k.site.scripts.update(script)

    const after = k.site.scripts.getByUrl(url)
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
| `nameOrId` | `string` | 是 | 脚本名称或 Id |
| `body` | `string` | 是 | 新 JavaScript 内容 |

**返回：** `void`。

```ts
k.api.post(() => {
    const stamp = Date.now().toString()
    const name = "ai-script-body-" + stamp
    const url = "/ai-script-body-" + stamp + ".js"

    k.site.scripts.add({ name, url, body: "// before" })
    k.site.scripts.updateBody(name, "// after-body")

    const after = k.site.scripts.getByUrl(url)
    return { verified: after?.body?.indexOf("after-body") >= 0 }
})
```

## delete()

按名称或 Id 删除。删除后用 `get(name)` 验证。

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `nameOrId` | `string` | 是 | 脚本名称或 Id |

**返回：** `void`。

```ts
k.api.post(() => {
    const stamp = Date.now().toString()
    const name = "ai-script-del-" + stamp
    const url = "/ai-script-del-" + stamp + ".js"

    k.site.scripts.add({ name, url, body: "// del" })
    const before = k.site.scripts.getByUrl(url)

    k.site.scripts.delete(name)
    const after = k.site.scripts.get(name)

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
    const scripts = k.site.scripts.all()
    for (const script of scripts) {
        const rel = k.site.scripts.getUrl(script.id)
        if (!rel) continue
        const abs = k.site.scripts.getAbsUrl(script.id)
        return { relative: rel, absolute: abs }
    }
    return null
})
```

仅注册了路由的脚本能返回 URL。

## getLogs() / getByLog()

| 方法 | 参数 | 返回 | 说明 |
|------|------|------|------|
| `getLogs(nameOrId)` | `nameOrId: string` | `ChangeLog[] | null` | 获取版本日志 |
| `getByLog(logId)` | `logId: number` | `Script | null` | 按日志 Id 获取历史版本 |

```ts
k.api.get(() => {
    const scripts = k.site.scripts.all()
    if (!scripts.length) return { error: "no scripts" }

    const logs = k.site.scripts.getLogs(scripts[0].id)
    return {
        verified: logs === null || Array.isArray(logs),
        logCount: logs ? logs.length : 0
    }
})
```

## Script 对象（常用字段）

| 字段 | 类型 | 说明 |
|------|------|------|
| id | `string` | 脚本 Id |
| name | `string` | 名称 |
| body | `string` | JS 内容 |
| extension | `string` | 扩展名，通常 `js` |
| async | `boolean` | 是否 async |
| defer | `boolean` | 是否 defer |
| crossOrigin | `string` | CORS 属性 |
| online | `boolean` | 是否上线 |
| version | `number` | 版本号 |

## 相关文档

- [k.site](./)
- [k.site.codes](./code.md) — API 与 CodeBlock（`codeType`）
- [k.site.styles](./css.md)
- [k.site.pages](./page.md)
