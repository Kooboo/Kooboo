# k.api

> 提供一系列方法用于更方便地创建 API

`k.api` 提供 `get`、`post`、`put`、`delete` 四种 HTTP 方法支持，自动处理参数匹配和路由。

## k.api.get()

创建当前路由下的 GET API。

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `action` | `Function` | 是 | GET 请求命中时执行的回调；回调参数可按名称匹配 query string |
| `metas` | `RootMeta[]` | 否 | 参数来源与类型定义 |
| `resultHandler` | `Function` | 否 | 统一处理回调返回结果的函数 |

**返回：** `void`。回调函数的返回值会作为 API 响应。

### 基本用法

```ts
k.api.get(() => {
    return 'hello world'
})
```

### 获取查询参数

**方式一：通过回调参数直接获取**

```ts
k.api.get((page, limit, keyword) => {
    // eg. /api/v1/test?page=1&limit=10&keyword=test
    // return {page: 1, limit: 10, keyword: 'test'}
    return { page, limit, keyword }
})
```

**方式二：通过 k.request.queryString 获取**

```ts
k.api.get(() => {
    // eg. /api/v1/test?page=1&limit=10&keyword=test
    const query = k.request.queryString
    return query
})
```

### 创建子级路由

创建子级路由需要在路由声明时包含 `{action}` 占位符：

```ts
// 路由: /api/v1/test/list
k.api.get("list", (id) => {
    return [{ id: 123 }]
})
```

::: warning 注意执行顺序
`k.api.get(()=>{})` 是通配路由，当通配路由和子级路由同时存在时，**通配路由必须声明在子级路由下方**。

错误示例（子级路由不会被匹配）：

```ts
k.api.get(() => {
    return "通配"
})
k.api.get("list", () => {
    return "子级list"  // 永远不会执行
})
```

正确示例：

```ts
k.api.get("list", () => {
    return "子级list"
})
k.api.get(() => {
    return "通配"
})
```

:::

## k.api.post()

创建当前路由下的 POST API。

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `action` | `Function` | 是 | POST 请求命中时执行的回调；可接收 query、form 或 body 参数 |
| `metas` | `RootMeta[]` | 否 | 参数来源与类型定义 |
| `resultHandler` | `Function` | 否 | 统一处理回调返回结果的函数 |

**返回：** `void`。回调函数的返回值会作为 API 响应。

### 基本用法

```ts
k.api.post(() => {
    return 'hello world'
})
```

### 获取请求体 body

**方式一：通过回调参数直接获取**

```ts
k.api.post((body) => {
    return body
})
```

**方式二：通过 k.request.body 获取**

::: tip 注意
`k.request.body` 返回的是**原始字符串**，需要使用 `JSON.parse()` 解析
:::

```ts
k.api.post(() => {
    const body = JSON.parse(k.request.body)
    return body
})
```

### 创建子级路由

子级路由的使用方法与 GET 相同：

```ts
k.api.post("create", () => {
    const body = JSON.parse(k.request.body)
    return { success: true, data: body }
})
```

## k.api.put()

创建当前路由下的 PUT API。

使用方法与 `POST` 请求相同。

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `action` | `Function` | 是 | PUT 请求命中时执行的回调 |
| `metas` | `RootMeta[]` | 否 | 参数来源与类型定义 |
| `resultHandler` | `Function` | 否 | 统一处理回调返回结果的函数 |

**返回：** `void`。回调函数的返回值会作为 API 响应。

```ts
k.api.put(() => {
    const body = JSON.parse(k.request.body)
    return { success: true }
})
```

## k.api.delete()

创建当前路由下的 DELETE API。

使用方法与 `GET` 请求相同。

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `action` | `Function` | 是 | DELETE 请求命中时执行的回调 |
| `metas` | `RootMeta[]` | 否 | 参数来源与类型定义 |
| `resultHandler` | `Function` | 否 | 统一处理回调返回结果的函数 |

**返回：** `void`。回调函数的返回值会作为 API 响应。

```ts
k.api.delete((id) => {
    return { success: true, deleted: id }
})
```

## k.api.httpCode()

设置 API 的 HTTP 状态码，通常用于异常处理。

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `code` | `number` | 是 | HTTP 状态码 |
| `data` | `any` | 否 | 可选响应体数据 |

**返回：** `KApiResponse`。

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

## 快捷方法

内置的 HTTP 状态码快捷方法：

### k.api.ok()

返回 200 OK。

**参数：** 可选 `data: any`。

**返回：** `KApiResponse`。

```ts
k.api.get((id) => {
    return k.api.ok()
})
```

### k.api.badRequest()

返回 400 Bad Request。

**参数：** 无。

**返回：** `KApiResponse`。

```ts
k.api.post(() => {
    return k.api.badRequest()
})
```

### k.api.unauthorized()

返回 401 Unauthorized。

**参数：** 无。

**返回：** `KApiResponse`。

```ts
k.api.get((id) => {
    return k.api.unauthorized()
})
```

### k.api.forbidden()

返回 403 Forbidden。

**参数：** 无。

**返回：** `KApiResponse`。

```ts
k.api.get((id) => {
    return k.api.forbidden()
})
```

### k.api.notFound()

返回 404 Not Found。

**参数：** 无。

**返回：** `KApiResponse`。

```ts
k.api.get((id) => {
    return k.api.notFound()
})
```

## 异常处理封装示例

```ts
export function errorResponse(message: string, code = 400) {
    k.response.json({
        message,
        code,
        data: null
    })
    return k.api.httpCode(code)
}

// 使用
k.api.get((id) => {
    const item = db.find(id)
    if (!item) {
        return errorResponse('Item not found', 404)
    }
    return item
})
```
