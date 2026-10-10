# k.site.codes

> 站点代码仓库：API 端点脚本与可复用 CodeBlock 的增删改查

## 概述

`k.site.codes` 管理站点 **Code** 资源。同一条 `k.site.codes` 仓库中，通过对象上的 **`codeType`** 区分用途——这是服务端渲染与路由的核心分类：

| `codeType`（写入时） | 枚举值（读取时常见） | 含义 | 典型用途 |
|---------------------|---------------------|------|----------|
| **`"Api"`** | `4` 或 `"Api"` | API 脚本 | 使用 [k.api](../api/) 定义 HTTP 接口；**有路由 URL**，可用 `getByUrl` / `getUrl` |
| **`"CodeBlock"`** | `10` 或 `"CodeBlock"` | 代码块 | 可复用 KScript 模块（`export function` 等）；**无独立路由**，在页面/视图中引用 |
| `"PageScript"` | `5` | 页面脚本 | 嵌入页面的脚本 |
| `"PaymentCallBack"` 等 | `7` 等 | 其它 | 支付回调等专用类型 |

::: info codeType 的表示形式
`add` 时请传字符串 **`"Api"`** / **`"CodeBlock"`**。`get` / `all` 返回的对象上，`codeType` 常为**数字枚举**（如 `4`、`10`），也可能是字符串，筛选时建议同时兼容两种形式。
:::

::: warning Api 与 CodeBlock 的 `add` 行为不同
- **`codeType: "CodeBlock"`**：只需 `name`、`body`；**不要**传 `url`（不会注册站点路由）。
- **`codeType: "Api"`**（或未显式写 CodeBlock 时的可路由创建）：需 `name`、`body`，并应提供 **`url`**（如 `/api/hello`），会注册路由，行为与 [k.site.pages](./page.md) / [k.site.scripts](./js.md) 类似。
:::

::: tip 与 k.api 的关系
在控制台里写的 **API 文件** 对应 `codeType === "Api"` 的 Code；文件内的 `k.api.get/post` 定义该 API 下的子路径。`k.site.codes` 用于在运行时对这些 Code 做 CRUD，而不是替代 `k.api` 语法本身。
:::

## TypeScript 定义（节选）

```ts
type CodeType =
  | 'Api'
  | 'CodeBlock'
  | 'PageScript'
  | 'PaymentCallBack'
  | 'Job'
  | /* 另有已废弃枚举值 */;

interface CodeRepository {
  add(code: CodeInput): void;
  all(): Code[];
  get(nameOrId: string): Code | null;
  getByUrl(url: string): Code | null;
  update(code: Code): void;
  updateBody(nameOrId: string, body: string): void;
  delete(nameOrId: string): void;
  getUrl(id: string): string | null;
  getAbsUrl(id: string): string | null;
  getLogs(nameOrId: string): ChangeLog[] | null;
  getByLog(logId: number): Code | null;
}

interface CodeInput {
  name: string;
  body: string;
  codeType: CodeType;
  url?: string;
  scriptType?: string;
  cors?: boolean;
}
```

## 按 codeType 筛选

`all()` 返回全部 Code，需在脚本中按 `codeType` 过滤：

```ts
k.api.get(() => {
    const all = k.site.codes.all()
    const isApi = (c) => c.codeType === "Api" || c.codeType === 4
    const isCodeBlock = (c) => c.codeType === "CodeBlock" || c.codeType === 10
    return {
        apiCount: all.filter(isApi).length,
        codeBlockCount: all.filter(isCodeBlock).length
    }
})
```

## add() — CodeBlock

创建可复用代码块（无路由）。

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `code.name` | `string` | 是 | 代码块名称 |
| `code.body` | `string` | 是 | KScript 源码 |
| `code.codeType` | `string` | 是 | 必须为 `"CodeBlock"` |

**返回：** `void`。

```ts
k.api.post(() => {
    const stamp = Date.now().toString()
    const name = "ai-cb-" + stamp

    k.site.codes.add({
        name,
        codeType: "CodeBlock",
        body: `export function aiCb_${stamp}() { return "${stamp}"; }`
    })

    const block = k.site.codes.get(name)
    return {
        verified: !!block && (block.codeType === "CodeBlock" || block.codeType === 10),
        id: block?.id,
        hasRoute: block ? !!k.site.codes.getUrl(block.id) : false
    }
})
```

`hasRoute` 应为 `false`：CodeBlock 不注册独立 URL。

## add() — Api

创建 API 脚本并注册路由。

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `code.name` | `string` | 是 | API 名称（站点内唯一） |
| `code.body` | `string` | 是 | 含 `k.api.*` 的脚本 |
| `code.codeType` | `string` | 是 | 必须为 `"Api"` |
| `code.url` | `string` | 是 | API 根路径，如 `/api/demo` |

**返回：** `void`。

```ts
k.api.post(() => {
    const stamp = Date.now().toString()
    const name = "ai-api-" + stamp
    const url = "/api/ai-code-" + stamp

    k.site.codes.add({
        name,
        url,
        codeType: "Api",
        body: `k.api.get("ping", () => ({ stamp: "${stamp}" }))`
    })

    const api = k.site.codes.getByUrl(url)
    const route = api ? k.site.codes.getUrl(api.id) : null

    return {
        verified: !!api && (api.codeType === "Api" || api.codeType === 4) && route === url,
        id: api?.id,
        route
    }
})
```

## all() / get()

| 方法 | 参数 | 返回 | 说明 |
|------|------|------|------|
| `all()` | 无 | `Code[]` | 返回全部 Code / CodeBlock |
| `get(nameOrId)` | `nameOrId: string` | `Code | null` | 按名称或 Id 获取 |

```ts
k.api.get(() => {
    const code = k.site.codes.get("my-api")
    return code
        ? { name: code.name, codeType: code.codeType }
        : null
})
```

## getByUrl()

仅对 **Api**（`codeType` 为 `"Api"` 或 `4`）且已注册路由的 Code 有效。

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `url` | `string` | 是 | API 路由 URL |

**返回：** `Code | null`。

```ts
k.api.get(() => {
    const api = k.site.codes.getByUrl("/api/hello")
    return api ? { name: api.name, codeType: api.codeType } : null
})
```

## update() / updateBody()

| 方法 | 参数 | 返回 | 说明 |
|------|------|------|------|
| `update(code)` | `code: Code` | `void` | 更新完整 Code 对象 |
| `updateBody(nameOrId, body)` | `nameOrId: string`, `body: string` | `void` | 仅替换源码正文 |

```ts
k.api.post(() => {
    const stamp = Date.now().toString()
    const name = "ai-api-upd-" + stamp
    const url = "/api/ai-code-upd-" + stamp

    k.site.codes.add({
        name,
        url,
        codeType: "Api",
        body: 'k.api.get(() => "v1")'
    })

    const api = k.site.codes.getByUrl(url)
    api.body = 'k.api.get(() => "v2")'
    k.site.codes.update(api)

    const after = k.site.codes.getByUrl(url)
    return { verified: after?.body?.indexOf("v2") >= 0 }
})
```

`updateBody(nameOrId, body)` 仅替换 `body`，用法与 styles/scripts 相同。

## delete()

删除后用 `get(name)` 验证；**不要**在删除 Api 后调用 `getByUrl`（路由残留时可能异常，与 [k.site.pages](./page.md) 相同）。

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `nameOrId` | `string` | 是 | Code 名称或 Id |

**返回：** `void`。

```ts
k.api.post(() => {
    const stamp = Date.now().toString()
    const name = "ai-api-del-" + stamp
    const url = "/api/ai-code-del-" + stamp

    k.site.codes.add({
        name,
        url,
        codeType: "Api",
        body: 'k.api.get(() => "del")'
    })
    const before = k.site.codes.getByUrl(url)

    k.site.codes.delete(name)
    const after = k.site.codes.get(name)

    return { verified: !!before && !after }
})
```

## getUrl() / getAbsUrl()

| 方法 | 参数 | 返回 | 说明 |
|------|------|------|------|
| `getUrl(id)` | `id: string` | `string | null` | 获取相对 URL，仅对有路由的 API 有值 |
| `getAbsUrl(id)` | `id: string` | `string | null` | 获取绝对 URL |

```ts
k.api.get(() => {
    const apis = k.site.codes.all().filter((c) => c.codeType === "Api" || c.codeType === 4)
    for (const api of apis) {
        const rel = k.site.codes.getUrl(api.id)
        if (!rel) continue
        const abs = k.site.codes.getAbsUrl(api.id)
        return { relative: rel, absolute: abs, name: api.name }
    }
    return null
})
```

## getLogs() / getByLog()

与其它 `k.site.*` 文本仓库一致。

| 方法 | 参数 | 返回 | 说明 |
|------|------|------|------|
| `getLogs(nameOrId)` | `nameOrId: string` | `ChangeLog[] | null` | 获取版本日志 |
| `getByLog(logId)` | `logId: number` | `Code | null` | 按日志 Id 获取历史版本 |

```ts
k.api.get(() => {
    const codes = k.site.codes.all()
    if (!codes.length) return { error: "no codes" }

    const logs = k.site.codes.getLogs(codes[0].id)
    return { logCount: logs ? logs.length : 0 }
})
```

## Code 对象（常用字段）

| 字段 | 类型 | 说明 |
|------|------|------|
| id | `string` | Code Id |
| name | `string` | 名称 |
| body | `string` | 源码 |
| codeType | `CodeType` | **`Api`** / **`CodeBlock`** 等 |
| scriptType | `string` | 脚本引擎类型（如 `Module`） |
| extension | `string` | 通常 `.js` |
| cors | `boolean` | API 是否启用 CORS |
| parameters | `string[]` | API 请求参数声明 |
| online | `boolean` | 是否上线 |
| version | `number` | 版本号 |

## 相关文档

- [k.api](../api/) — API 路由写法
- [k.site](./)
- [k.site.scripts](./js.md) — 外链 JS 资源（非 Code 仓库）
- [k.module](../module/) — 模块上下文
