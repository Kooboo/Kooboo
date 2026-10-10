# k.api

> 定义 API 路由的 GET、POST、PUT 和 DELETE 处理器。

## 作用与执行方式

`k.api` 用于定义 API 处理器。运行时只有在当前请求的方法和路由都匹配时才会执行处理器；调用 `k.api.get/post/put/delete` 不会立即把处理器结果作为 `run_kscript` 工具的返回值。

- 在 API 或 CodeBlock 文件中，用 `k.api.*` 包裹路由处理逻辑。
- 在 `run_kscript` 中，应直接在顶层 `return`，不要注册 API 处理器。
- `k.api.get("detail", ...)` 这类命名 action 会匹配包含 `{action}` 的 API 路由；`"detail/{id}"` 这类嵌套路由可以把 `id` 作为处理器参数接收。
- 未使用元数据的处理器参数会先读取查询参数，再读取同名路由参数。名为 `body` 的参数会尽可能解析 JSON，否则返回表单数据。

## TypeScript 定义

```ts
type ApiHandler = (...args: any[]) => any
type ApiResultHandler = (code: number, data: string | null) => any

interface ApiMethod {
  (action: ApiHandler): void
  (action: ApiHandler, metas: RootMeta[]): void
  (action: ApiHandler, resultHandler: ApiResultHandler): void
  (action: ApiHandler, metas: RootMeta[], resultHandler: ApiResultHandler): void
  (actionName: string, action: ApiHandler): void
  (actionName: string, action: ApiHandler, metas: RootMeta[]): void
  (actionName: string, action: ApiHandler, resultHandler: ApiResultHandler): void
  (actionName: string, action: ApiHandler, metas: RootMeta[], resultHandler: ApiResultHandler): void
}

interface KApi {
  get: ApiMethod
  post: ApiMethod
  put: ApiMethod
  delete: ApiMethod

  ok(): KApiResponse
  ok(data: any): KApiResponse
  httpCode(code: number): KApiResponse
  httpCode(code: number, data: any): KApiResponse
  redirect(url: string): KApiResponse
  badRequest(): KApiResponse
  unauthorized(): KApiResponse
  forbidden(): KApiResponse
  notFound(): KApiResponse
}

interface KApiResponse {
  code: number
  data: string | null
}

interface RootMeta {
  name: string
  required?: boolean
  type?: "Boolean" | "String" | "Number" | "Object" | "Array"
  min?: number
  max?: number
  pattern?: string
  validator?: (value: any) => boolean
  from?: "QueryString" | "Header" | "Cookie" | "Form" | "Body"
  children?: ChildMeta[]
}

interface ChildMeta extends RootMeta {}
```

`from` 默认为 `QueryString`，没有读取路由值的选项。对于路由参数，应省略元数据，或使用 `k.request.get(name)` 读取。

## 定义处理器

```ts
k.api.get(() => ({ message: "hello" }))

// API 路由：/api/users/{action}
k.api.get("detail", (id) => {
  if (!id) return k.api.badRequest()
  return k.api.ok({ id })
})

// API 路由：/api/users/{action}；请求：/api/users/detail/42
k.api.put("detail/{id}", (id, body) => {
  if (!id || !body) return k.api.badRequest()
  return k.api.ok({ id, body })
})
```

没有命名 action 的路由使用不带 `actionName` 的重载。创建或基于请求体更新通常使用 `post`，替换或更新通常使用 `put`，删除使用 `delete`；无论哪种方法，运行时仍要求它与当前 HTTP 请求方法匹配。

## 参数校验

元数据会在处理器执行前转换并校验回调参数。

```ts
k.api.post((body) => {
  return k.api.ok({ name: body.name })
}, [
  {
    name: "body",
    from: "Body",
    type: "Object",
    required: true,
    children: [
      { name: "name", type: "String", required: true, min: 1 },
      { name: "age", type: "Number", min: 0 }
    ]
  }
])
```

- `required` 校验缺失的值或对象属性；它本身不会拒绝空字符串。
- `Number`、`Boolean`、`Object` 和 `Array` 会转换或校验输入；数字字符串和布尔字符串可以被转换。
- 对于 `Array`，需要校验或转换数组元素时应提供 `children`；不要假设未配置的数组会自动递归校验元素。
- `min` 和 `max` 对数字限制数值，对 `type: "String"` 限制字符串长度。
- `pattern` 作用于字符串，`validator` 必须返回 `true`。
- 当前运行时会将处理器中的校验异常或 JavaScript 异常转换为 HTTP 400，其他异常转换为 HTTP 500。不要把内部异常文本当作稳定的公开 API 契约。

## 响应辅助方法

处理器直接返回普通值时，运行时会将其序列化为响应体。需要明确状态码或响应体时，使用 `KApiResponse` 辅助方法。

```ts
k.api.get(() => k.api.ok({ items: [] }))

k.api.post(() => {
  const body = JSON.parse(k.request.body)
  if (!body.name) return k.api.httpCode(400, { error: "name is required" })
  return k.api.httpCode(201, { created: true, name: body.name })
})

k.api.get("download", () => k.api.redirect("/files/report.pdf"))
k.api.get("missing", () => k.api.notFound())
```

- `ok()` 返回 200 且没有 `data`；`ok(value)` 返回 200，并将 `value` 序列化为 JSON。
- `httpCode(code, value)` 返回指定状态码，并将 `value` 序列化为 JSON；`httpCode(code)` 不带响应数据。
- `redirect(url)` 返回 302，并把 URL 作为重定向目标。
- `badRequest`、`unauthorized`、`forbidden` 和 `notFound` 分别设置对应状态码，但不会生成默认响应体。
- `k.response.json()` 直接写入 HTTP 响应，与 `run_kscript` 的 JavaScript 返回值是两条不同路径；在 API 处理器中，除非确实需要直接写响应，否则优先 `return` 数据。

## 自定义结果处理器

可选的结果处理器接收最终状态码，以及字符串形式的响应数据（也可能是 `null`）。如果它返回值，运行时会将该值序列化后写入 HTTP 响应。

```ts
k.api.get(() => ({ id: 1 }), (code, data) => ({
  success: code < 400,
  code,
  data: code < 400 ? data : null
}))
```

## 相关文档

- [k.request](./request.md) - HTTP 请求信息和路由参数
- [k.response](./response.md) - 直接处理 HTTP 响应
- [k.security](./security.md) - JWT 和哈希工具
