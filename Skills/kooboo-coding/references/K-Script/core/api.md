# k.api

> Define GET, POST, PUT, and DELETE handlers for API routes.

## Scope and execution

`k.api` defines API handlers. The runtime invokes a handler only when the current request method and route match; calling `k.api.get/post/put/delete` does not immediately return the handler result to the `run_kscript` tool.

- In an API or CodeBlock file, wrap the route body in `k.api.*`.
- In `run_kscript`, return a value at the top level instead of registering an API handler.
- A named action such as `k.api.get("detail", ...)` is matched against an API route containing `{action}`. A nested action such as `"detail/{id}"` can receive `id` as a callback parameter.
- A handler parameter without metadata reads query parameters first, then route values with the same name. A `body` parameter is parsed as JSON when possible; otherwise form data is returned.

## TypeScript definition

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

`from` defaults to `QueryString`; it does not have a route-value option. For route parameters, omit metadata or read the value through `k.request.get(name)`.

## Define handlers

```ts
k.api.get(() => ({ message: "hello" }))

// API route: /api/users/{action}
k.api.get("detail", (id) => {
  if (!id) return k.api.badRequest()
  return k.api.ok({ id })
})

// API route: /api/users/{action}; request: /api/users/detail/42
k.api.put("detail/{id}", (id, body) => {
  if (!id || !body) return k.api.badRequest()
  return k.api.ok({ id, body })
})
```

For a route without a named action, use the overload without `actionName`. Use `post` for creation or body-based updates, `put` for replacement/update semantics, and `delete` for deletion; the runtime still requires the HTTP method to match the current request.

## Validate parameters

Metadata converts and validates callback parameters before invoking the handler.

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

- `required` rejects a missing value or object property; it does not reject an empty string by itself.
- `Number`, `Boolean`, `Object`, and `Array` convert or validate the incoming value. Numeric strings and boolean strings can be converted.
- For `Array`, provide `children` when item validation or conversion is needed; do not assume an unconfigured array receives recursive item validation.
- `min` and `max` constrain numeric values, or string length when `type` is `String`.
- `pattern` applies to strings, and `validator` must return `true`.
- Validation or JavaScript errors from the handler currently produce HTTP 400; other exceptions currently produce HTTP 500. Do not rely on internal exception text as a stable public API contract.

## Response helpers

Returning a plain value from the handler serializes it as the response body. Use `KApiResponse` helpers when the status code or response body must be explicit.

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

- `ok()` returns status 200 with no `data`; `ok(value)` returns status 200 with `value` serialized as JSON.
- `httpCode(code, value)` returns the status and JSON-serialized value. `httpCode(code)` has no response data.
- `redirect(url)` returns status 302 and uses the URL as the redirect target.
- `badRequest`, `unauthorized`, `forbidden`, and `notFound` set their respective status codes without generating a default response body.
- `k.response.json()` writes directly to the HTTP response. It is separate from the JavaScript value returned by `run_kscript`; inside an API handler, return data unless direct response writing is required.

## Custom result handler

The optional result handler receives the final status code and response data as a string (or `null`). If it returns a value, the runtime serializes that value and writes it to the HTTP response.

```ts
k.api.get(() => ({ id: 1 }), (code, data) => ({
  success: code < 400,
  code,
  data: code < 400 ? data : null
}))
```

## Related documents

- [k.request](./request.md) - HTTP request information and route parameters
- [k.response](./response.md) - Direct HTTP response handling
- [k.security](./security.md) - JWT and hashing utilities
