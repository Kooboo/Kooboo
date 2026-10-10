# k.api

> Provides a series of methods to create APIs more conveniently

`k.api` provides support for four HTTP methods: `get`, `post`, `put`, and `delete`, and automatically handles parameter matching and routing.

## k.api.get()

Create a GET API under the current route.

| Parameter | Type | Required | Description |
|------|------|------|------|
| `action` | `Function` | yes | Callback executed when the GET request matches; callback parameters can match query string names |
| `metas` | `RootMeta[]` | no | Parameter source and type definitions |
| `resultHandler` | `Function` | no | Function for normalizing callback results |

**Returns:** `void`. The callback return value becomes the API response.

### Basic Usage

```ts
k.api.get(() => {
    return 'hello world'
})
```

### Read Query Parameters

**Method 1: Obtain directly through callback parameters**

```ts
k.api.get((page, limit, keyword) => {
    // eg. /api/v1/test?page=1&limit=10&keyword=test
    // return {page: 1, limit: 10, keyword: 'test'}
    return { page, limit, keyword }
})
```

**Method 2: Obtain through k.request.queryString**

```ts
k.api.get(() => {
    // eg. /api/v1/test?page=1&limit=10&keyword=test
    const query = k.request.queryString
    return query
})
```

### Create Child Routes

Creating a child route requires including the `{action}` placeholder in the route declaration:

```ts
// Route: /api/v1/test/list
k.api.get("list", (id) => {
    return [{ id: 123 }]
})
```

::: warning Pay attention to the execution order
`k.api.get(()=>{})` is a wildcard route. When a wildcard route and a sub-level route exist at the same time, the **wildcard route must be declared below the sub-level route**.

Error example (child routes will not be matched):

```ts
k.api.get(() => {
    return "wildcard"
})
k.api.get("list", () => {
    return "child list"  // Never runs
})
```

Correct example:

```ts
k.api.get("list", () => {
    return "child list"
})
k.api.get(() => {
    return "wildcard"
})
```

:::

## k.api.post()

Create a POST API under the current route.

| Parameter | Type | Required | Description |
|------|------|------|------|
| `action` | `Function` | yes | Callback executed when the POST request matches; can receive query, form, or body parameters |
| `metas` | `RootMeta[]` | no | Parameter source and type definitions |
| `resultHandler` | `Function` | no | Function for normalizing callback results |

**Returns:** `void`. The callback return value becomes the API response.

### Basic Usage

```ts
k.api.post(() => {
    return 'hello world'
})
```

### Read the Request Body

**Method 1: Obtain directly through callback parameters**

```ts
k.api.post((body) => {
    return body
})
```

**Method 2: Obtain through k.request.body**

::: tip note
`k.request.body` returns the **original string** and needs to be parsed using `JSON.parse()`
:::

```ts
k.api.post(() => {
    const body = JSON.parse(k.request.body)
    return body
})
```

### Create Child Routes

Child routes are used in the same way as GET:

```ts
k.api.post("create", () => {
    const body = JSON.parse(k.request.body)
    return { success: true, data: body }
})
```

## k.api.put()

Create a PUT API under the current route.

Usage is the same as `POST` request.

| Parameter | Type | Required | Description |
|------|------|------|------|
| `action` | `Function` | yes | Callback executed when the PUT request matches |
| `metas` | `RootMeta[]` | no | Parameter source and type definitions |
| `resultHandler` | `Function` | no | Function for normalizing callback results |

**Returns:** `void`. The callback return value becomes the API response.

```ts
k.api.put(() => {
    const body = JSON.parse(k.request.body)
    return { success: true }
})
```

## k.api.delete()

Create a DELETE API under the current route.

Usage is the same as `GET` request.

| Parameter | Type | Required | Description |
|------|------|------|------|
| `action` | `Function` | yes | Callback executed when the DELETE request matches |
| `metas` | `RootMeta[]` | no | Parameter source and type definitions |
| `resultHandler` | `Function` | no | Function for normalizing callback results |

**Returns:** `void`. The callback return value becomes the API response.

```ts
k.api.delete((id) => {
    return { success: true, deleted: id }
})
```

## k.api.httpCode()

Set the HTTP status code of the API, usually used for exception handling.

| Parameter | Type | Required | Description |
|------|------|------|------|
| `code` | `number` | yes | HTTP status code |
| `data` | `any` | no | Optional response body data |

**Returns:** `KApiResponse`.

```ts
k.api.get(() => {
    k.response.json({
        message: 'Something went wrong',
        code: 400,
        data: null
    })
    return k.api.httpCode(400)
})
```

## Shortcut Methods

Built-in HTTP status code shortcut methods:

### k.api.ok()

Return 200 OK.

**Parameters:** Optional `data: any`.

**Returns:** `KApiResponse`.

```ts
k.api.get((id) => {
    return k.api.ok()
})
```

### k.api.badRequest()

Returns 400 Bad Request.

**Parameters:** None.

**Returns:** `KApiResponse`.

```ts
k.api.post(() => {
    return k.api.badRequest()
})
```

### k.api.unauthorized()

Returns 401 Unauthorized.

**Parameters:** None.

**Returns:** `KApiResponse`.

```ts
k.api.get((id) => {
    return k.api.unauthorized()
})
```

### k.api.forbidden()

Returns 403 Forbidden.

**Parameters:** None.

**Returns:** `KApiResponse`.

```ts
k.api.get((id) => {
    return k.api.forbidden()
})
```

### k.api.notFound()

Returns 404 Not Found.

**Parameters:** None.

**Returns:** `KApiResponse`.

```ts
k.api.get((id) => {
    return k.api.notFound()
})
```

## Error Handling Helper Example

```ts
export function errorResponse(message: string, code = 400) {
    k.response.json({
        message,
        code,
        data: null
    })
    return k.api.httpCode(code)
}

// Usage
k.api.get((id) => {
    const item = db.find(id)
    if (!item) {
        return errorResponse('Item not found', 404)
    }
    return item
})
```
