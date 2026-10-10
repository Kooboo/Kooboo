# API Layer Guide

## Route Design

Group related operations under one API with an `{action}` route. Use `k.api.get("/", ...)` and `k.api.put("/", ...)` for the base path, and named callbacks for operations. Add only a final GET notFound catch-all; do not add fallback handlers for other methods. See [App Base services](app-base-services.md).

### URL and Route Relationship

The `Url` parameter passed to `create_api` determines the route patterns allowed in the file:

| URL definition | Meaning | Example |
|---------|------|------|
| `/api/auth` | Single route; only the catch-all form is allowed | `k.api.get(()=>{})` → GET /api/auth |
| `/api/auth/{action}` | Dynamic subroutes are allowed | `k.api.post("login",()=>{})` → POST /api/auth/login |

### Dynamic Routes

To define multiple subroutes in one API file, such as `/api/auth/login` and `/api/auth/logout`, the `Url` must include {action} when the API is created:

```ts
// create_api 时设置 Url: /api/auth/{action}

// POST /api/auth/login
k.api.post("login", () => {
    return { action: "login" }
})

// POST /api/auth/logout
k.api.post("logout", () => {
    return { action: "logout" }
})

// GET /api/auth
k.api.get("/", () => {
    return { message: "auth endpoint" }
})

k.api.get(() => k.api.notFound())
```

### Route Order

Register named callbacks, including `"/"` for the base path, before the final GET 404 catch-all. Do not infer the path from an `action` query parameter. Do not add POST, PUT or DELETE fallback callbacks.

```ts
k.api.get("/", () => { return { message: "auth endpoint" } })
k.api.get("status", () => { return { available: true } })
k.api.post("login", () => { return "login" })

k.api.get(() => k.api.notFound())
```

### `{action}` Check

If the `Url` passed to `create_api` does not contain `{action}`:
- Catch-all routes such as `k.api.get(()=>{})` are allowed.
- **Do not** use dynamic subroutes such as `k.api.get("sub", ...)`; they will not take effect.

## Parameter Access

### Query String (GET/DELETE parameters)

Prefer named callback parameters. Convert strings before validating; read `k.request` explicitly only when selecting a particular source.

```ts
// URL: /api/products/{action}
k.api.get("/", (id: string | null, page: string | null) => {
    const pageNumber = page == null ? 1 : Number(page)

    if (!Number.isInteger(pageNumber) || pageNumber < 1) {
        return k.api.httpCode(400, { message: "Invalid page" })
    }

    const result = { id, page: pageNumber }

    return result
})

k.api.get(() => k.api.notFound())
```

### Body (POST/PUT Parameters)

Receive it directly in the callback parameter:

```ts
// @k-url /api/user/{action}

k.api.post("create-user", (body) => {
    const { userName, password, email } = body
    return { success: true, data: { userName } }
})
```

## Response Format

Returning an object directly automatically serializes it as JSON. (A Utils wrapper is recommended for a consistent response format.)

```ts
k.api.get(() => {
    return { success: true, data: { id: 1 } }
})
```

### Response Helpers

| Method | Purpose |
|------|------|
| `k.response.json(obj)` | Return JSON |
| `k.response.setHeader(key, value)` | Set a response header |
| `k.response.redirect(url)` | Redirect |
| `k.response.notFound()` | Return 404 |
| `k.response.unauthorized()` | Return 401 |

### HTTP Status Codes

| Method | Status code |
|------|--------|
| `k.api.ok()` | 200 |
| `k.api.badRequest()` | 400 |
| `k.api.unauthorized()` | 401 |
| `k.api.notFound()` | 404 |

::: warning `k.api.xxx()` Does Not Accept Arguments
`k.api.ok()` / `k.api.badRequest()` / `k.api.unauthorized()` / `k.api.notFound()` **accept no arguments** and must be called on their own.

**❌ Incorrect:**
```ts
return k.api.unauthorized(result.error);  // 不支持传参，会报错
```

**✅ Correct:**
```ts
k.response.json({ message: result.error, success: false })
return k.api.unauthorized();
```
:::

## Mock API for App items

When authorized implementation explicitly uses a mock API, declare business fixtures directly in the corresponding API code object and return them as `data`. list/get returns fixtures; create/update/delete and similar writes only return a success signal and do not actually change data. This does not require a Service CodeBlock merely to move constants:

```ts
const requests = [
  { id: "req-001", title: "Office equipment purchase", status: "pending" }
]

k.api.get(() => {
  return { success: true, data: requests }
})

k.api.post(() => {
  return { success: true }
})

k.api.put(() => {
  return { success: true }
})

k.api.delete(() => {
  return { success: true }
})
```

- Fixtures must follow the Blueprint business-object, attribute, state, and App Plan Data Model semantics;
- list/get may read minimal query parameters and apply deterministic filtering; write endpoints may return `{ success: true }` only;
- Kooboo API scripts re-run on every request, so module-level mutations do not survive. That is expected; do not switch to SQLite or change the item to `new-store` for that reason;
- Do not implement complex business rules, permissions, or persistence in a mock API; do not call `k.DB.sqlite` or create tables for that item;
- The frontend calls this API through an HTTP service and treats write success as the signal. Do not also generate a frontend `mock.js`, duplicate the same fixtures, or use localStorage as a write substitute;
- A `strategy=new-store` item in the same Milestone may use real storage. When this item later moves to real data, preserve the URL and response contract and move complex logic into the Service/Data layers.

## Object Name for create_api

When `create_api` creates an **API code object**, its `name` should be a **short lowercase identifier** such as `product`, `auth`, or `init-sqlite`, consistent with the create_api guidance in [Backend Object Creation](../Functions/backend-create.md).

Names in the form **`Services.xxx` / `Utils.xxx`** are only for **Service / utility CodeBlocks** created with `create_codeblock`; do not use `Services.xxx` as the API object's `name`.

## Calling a Service (Only Supported Method)

The API layer references functions exported by other CodeBlocks **only through a static `import` at the top of the file**. Business logic belongs in the Service; the API only handles parameters and the response.

**✅ Correct:**

```ts
import { getCurrentAppUser } from "./Services.user"

k.api.get(() => {
    return getCurrentAppUser()
})
```

**❌ Incorrect (Kooboo does not support this syntax; do not write it):**

```ts
k.api.get(() => {
    return k.req.services.getCurrentAppUser()
})
```

- **Do not use** `k.req.services`, `k.request.services`, or any other “attach a Service to the request object and call it by name” form that is not documented in the KScript references.
- Service files must use `export function ...`; a top-level function without `export` cannot be imported by the API `import`.

## CodeBlock Organization

The API layer imports Services and Utils:

```ts
import { getProductByList } from "./Services.product"
import { successResponse } from "./Utils.response"

k.api.get("list", () => {
    const { list, pager } = getProductByList()
    return successResponse({
        list,
        pager: {
            total,
            page,
            page_size
        }
    })
})
```

## HTTP Methods

| Method | Purpose | Route suffix |
|------|------|----------|
| `k.api.get()` | Query or retrieve data | ?querystring |
| `k.api.post()` | Create a resource | + body |
| `k.api.put()` | Update a resource | + body |
| `k.api.delete()` | Delete a resource | + id parameter |

## Redirects

```ts
// 相对路径
k.api.get(() => {
    k.response.redirect("/login")
})

// 绝对路径
k.api.get(() => {
    k.response.redirect("https://example.com")
})
```

| Parameter | Type | Required | Description |
|------|------|------|------|
| url | string | Yes | Target URL |
| absolute | boolean | No | Whether the path is absolute (includes a protocol) |

## Form Data

Read submitted form data, typically for file-upload scenarios:

```ts
k.api.post(() => {
    const data = k.request.form
    return { received: data }
})
```

::: tip
Form data is usually used with file uploads. For a pure file-upload scenario, use the [KFile module](../K-Script/file/index.md).
:::

## Notes

1. **Do not write business logic in the API layer**; only read parameters, call the Service, and return the result. A `mock-api` delivery may return in-code fixtures as described above, with writes returning only a success signal, but must not grow into SQLite or another persistent implementation.
2. **Validate parameters**: check that required parameters are present.
3. **Handle errors**: return an appropriate HTTP status code.
4. **Call Service logic only through `import`** (see “Calling a Service” above); do not manipulate a real data source directly in the API layer or invent `k.req.services` and similar forms. In-code fixtures and write success signals for `mock-api` are the exception described above.
5. **Use `k.logger` for debug output**; do not use `console.log` / `console.error` (the Kooboo server does not support console methods).
6. **Pass no arguments to `k.api.xxx()`**: status methods (`ok` / `badRequest` / `unauthorized` / `notFound`) accept no arguments; set the response body separately with `k.response.json()`.
