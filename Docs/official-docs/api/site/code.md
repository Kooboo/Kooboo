# k.site.codes

> Site code repository: API endpoint scripts and reusable CodeBlock additions, deletions, modifications and queries

## Overview

`k.site.codes` manages site **Code** resources. In the same `k.site.codes` warehouse, the purpose is distinguished by **`codeType`** on the object - this is the core classification of server-side rendering and routing:

| `codeType` (when writing) | Enumeration value (common when reading) | meaning | Typical uses |
|--------------------------|--------------------------|------|----------|
| **`"Api"`** | `4` or `"Api"` | API script | Use [k.api](../api/) to define the HTTP interface; **with routing URL**, available `getByUrl` / `getUrl` |
| **`"CodeBlock"`** | `10` or `"CodeBlock"` | code block | Reusable KScript modules (`export function`, etc.); **no independent routing**, referenced in pages/views |
| `"PageScript"` | `5` | Page script | Script embedded in the page |
| `"PaymentCallBack"` etc. | `7` etc. | other | Special types such as payment callbacks |

::: Representation of info codeType
Please pass the string **`"Api"`** / **`"CodeBlock"`** when `add`. On the object returned by `get` / `all`, `codeType` is often a **numeric enumeration** (such as `4`, `10`), or it may be a string. It is recommended to be compatible with both forms when filtering.
:::

::: warning API behaves differently than CodeBlock's `add`
- **`codeType: "CodeBlock"`**: Only need `name`, `body`; **Do not** pass `url` (site routing will not be registered).
- **`codeType: "Api"`** (or routable creation when CodeBlock is not explicitly written): `name`, `body` is required, and **`url`** (such as `/api/hello`) should be provided, the route will be registered, and the behavior is similar to [k.site.pages](./page.md) / [k.site.scripts](./js.md).
:::

::: The relationship between tip and k.api
The **API file** written in the console corresponds to the Code of `codeType === "Api"`; `k.api.get/post` in the file defines the subpath under the API. `k.site.codes` is used to do CRUD on these Codes at runtime, rather than replacing the `k.api` syntax itself.
:::

## TypeScript Definition

```ts
type CodeType =
  | 'Api'
  | 'CodeBlock'
  | 'PageScript'
  | 'PaymentCallBack'
  | 'Job'
  | /* Additional deprecated enum values exist */;

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

## Filter by codeType

`all()` returns all Codes and needs to be filtered by `codeType` in the script:

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

Create reusable code blocks (without routing).

| Parameter | Type | Required | Description |
|------|------|------|------|
| `code.name` | `string` | yes | CodeBlock name |
| `code.body` | `string` | yes | KScript source code |
| `code.codeType` | `string` | yes | Must be `"CodeBlock"` |

**Returns:** `void`.

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

`hasRoute` should be `false`: CodeBlock does not register standalone URLs.

## add() — API

Create API scripts and register routes.

| Parameter | Type | Required | Description |
|------|------|------|------|
| `code.name` | `string` | yes | API name (unique within site) |
| `code.body` | `string` | yes | Script containing `k.api.*` |
| `code.codeType` | `string` | yes | Must be `"Api"` |
| `code.url` | `string` | yes | API root path, such as `/api/demo` |

**Returns:** `void`.

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

## all()/get()

| Method | Parameters | Returns | Description |
|------|------|------|------|
| `all()` | None | `Code[]` | Returns all Code / CodeBlock resources |
| `get(nameOrId)` | `nameOrId: string` | `Code | null` | Gets a resource by name or ID |

```ts
k.api.get(() => {
    const code = k.site.codes.get("my-api")
    return code
        ? { name: code.name, codeType: code.codeType }
        : null
})
```

## getByUrl()

Only valid for **Api** (`codeType` is `"Api"` or `4`) and Code with registered routing.

| Parameter | Type | Required | Description |
|------|------|------|------|
| `url` | `string` | yes | API route URL |

**Returns:** `Code | null`.

```ts
k.api.get(() => {
    const api = k.site.codes.getByUrl("/api/hello")
    return api ? { name: api.name, codeType: api.codeType } : null
})
```

## update() / updateBody()

| Method | Parameters | Returns | Description |
|------|------|------|------|
| `update(code)` | `code: Code` | `void` | Updates the full Code object |
| `updateBody(nameOrId, body)` | `nameOrId: string`, `body: string` | `void` | Replaces only the source body |

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

`updateBody(nameOrId, body)` only replaces `body` and has the same usage as styles/scripts.

## delete()

Use `get(name)` to verify after deletion; **Do not** call `getByUrl` after deleting the API (it may be abnormal when the route remains, the same as [k.site.pages](./page.md)).

| Parameter | Type | Required | Description |
|------|------|------|------|
| `nameOrId` | `string` | yes | Code name or ID |

**Returns:** `void`.

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

| Method | Parameters | Returns | Description |
|------|------|------|------|
| `getUrl(id)` | `id: string` | `string | null` | Gets the relative URL; only APIs with routes have values |
| `getAbsUrl(id)` | `id: string` | `string | null` | Gets the absolute URL |

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

Consistent with other `k.site.*` text repositories.

| Method | Parameters | Returns | Description |
|------|------|------|------|
| `getLogs(nameOrId)` | `nameOrId: string` | `ChangeLog[] | null` | Gets version logs |
| `getByLog(logId)` | `logId: number` | `Code | null` | Gets a historical version by log ID |

```ts
k.api.get(() => {
    const codes = k.site.codes.all()
    if (!codes.length) return { error: "no codes" }

    const logs = k.site.codes.getLogs(codes[0].id)
    return { logCount: logs ? logs.length : 0 }
})
```

## Common Code Fields

| Field | Type | Description |
|------|------|------|
| ID | `string` | Code ID |
| name | `string` | name |
| body | `string` | Source code |
| codeType | `CodeType` | **`Api`** / **`CodeBlock`** etc. |
| scriptType | `string` | Script engine type (such as `Module`) |
| extension | `string` | Usually `.js` |
| cors | `boolean` | Whether the API enables CORS |
| parameters | `string[]` | API request parameter declaration |
| online | `boolean` | Is it online? |
| version | `number` | version number |

## Related Docs

- [k.api](../api/) — API route writing method
- [k.site](./)
- [k.site.scripts](./js.md) — External link JS resources (non-Code repository)
- [k.module](../module/) — module context
