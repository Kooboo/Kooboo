# k.request

> HTTP request information — access data from the client request

## Overview

`k.request` provides access to HTTP request information, including query parameters, form data, uploaded files, and client details.

## TypeScript definition

```ts
interface Request {
  queryString: KDictionary;      // URL 查询参数
  form: KDictionary;             // Form 表单数据
  files: UploadFile[];           // 上传的文件列表
  body: string;                  // 请求体文本
  culture: string;                // 文化/语言设置
  model: any;                    // 请求模型
  method: string;                // HTTP 方法 (GET/POST/PUT/DELETE)
  clientIp: string;              // 客户端 IP 地址
  clientCountry: CountryLocationModel; // 客户端国家信息
  headers: KDictionary;          // 请求头
  url: string;                   // 当前请求 URL
  host: string;                  // 主机名
  page: Page;                    // 当前页面对象

  get(key: string): string;      // 获取动态路由参数
  setCulture(culture: string): void;
  resetCulture(): void;
}

interface UploadFile {
  bytes: number[];               // 文件二进制内容
  contentType: string;           // MIME 类型
  fileName: string;              // 文件名
  name: string;                  // 表单字段名
  save(filename: string): FileInfo; // 保存文件
}
```

## Properties

### queryString

Gets URL query parameters.

```ts
// GET /api/users?page=1&limit=10
k.api.get(() => {
    const page = k.request.queryString.get("page")   // "1"
    const limit = k.request.queryString.get("limit")  // "10"
    return { page, limit }
})
// 返回: { "page": "1", "limit": "10" }
```

### form

Gets form data from POST/PUT requests.

```ts
k.api.post(() => {
    const name = k.request.form.get("name")
    return { name }
})
```

### files

Gets the list of uploaded files.

```ts
k.api.post(() => {
    if (k.request.files.length > 0) {
        k.request.files.forEach((item) => {
            // 保存到 Kfile
            item.save(item.fileName)
        })
    }
    return { uploaded: k.request.files.length }
})
```

### headers

Gets HTTP request headers.

```ts
k.api.get(() => {
    const headers = k.request.headers
    return {
        accept: headers.get("Accept"),
        userAgent: headers.get("UserAgent"),
        token: headers.get("Authorization")
    }
})
```

Header lookup is case-insensitive, and `get()` also accepts names with hyphens removed (for example, `UserAgent` matches the `User-Agent` header).

### clientIp

Gets the client IP address as a string.

```ts
k.api.get(() => {
    return { ip: k.request.clientIp }
})
// 返回: { "ip": "120.36.245.209" }
```

### body

Gets the request body as text (typically used for POST/PUT).

::: tip Note
`k.request.body` returns the **raw string** and must be parsed manually:
```ts
const data = JSON.parse(k.request.body)
```
:::

```ts
k.api.post(() => {
    const body = k.request.body
    return { body }
})
// 返回请求体的原始文本
```

### model

Provides a convenient request model. If the body is valid JSON and there are no form or query parameters, it is parsed into the model. Otherwise, form fields and query parameters are combined into an object; form fields take precedence when a key appears in both. An invalid or empty JSON body does not produce a parsed body model.

```ts
k.api.post(() => {
    const model = k.request.model
    return { model }
})
```

**Parsing JSON data:**
```ts
k.api.post(() => {
    const { username, password } = JSON.parse(k.request.body || '{}')
    return { username }
})
```

### method

Gets the HTTP request method.

```ts
k.api.get(() => {
    return { method: k.request.method }
})
// 返回: { "method": "GET" }
```

### url / host

Gets the current request URL and host name.

```ts
k.api.get(() => {
    return {
        url: k.request.url,    // "/api/users"
        host: k.request.host   // "example.kooboo.cn"
    }
})
```

## Methods

### get(key)

Gets dynamic route parameters.

```ts
// 假设 api 的 url 设置为 /api/user/{action}

// 假设请求 url 为 /api/user/123
k.api.get("{id}", (id: string) => {
    return { id }
})
// return: { id: "123" }
```

::: warning Precedence
If a `请求参数 (queryString)` and a `动态路由参数` have the same name, the query parameter takes precedence.
```ts
// 假设 api 的 url 设置为 /api/user/{action}
// 假设请求 url 为 /api/user/123?id=456
k.api.get("{id}", (id: string) => {
    return { id }
})
// return: { id: "456" }
```
:::

## Complete examples

### File upload API

```ts
// 前端 api 请求应为 multipart/form-data 格式
k.api.post(() => {
    if (k.request.files.length === 0) {
        return { error: "No file uploaded" }
    }

    const file = k.request.files[0]
    const savedFile = file.save(file.fileName)

    return {
        code: 200,
        data: {
            name: savedFile.name,
            size: savedFile.size,
            url: savedFile.relativeUrl
        }
    }
})
```

### Get client information

```ts
k.api.get(() => {
    return {
        ip: k.request.clientIp,
        countryCode: k.request.clientCountry?.countryCode,
        method: k.request.method,
        url: k.request.url,
        userAgent: k.request.headers.get("UserAgent")
    }
})
```

## Related documents

- [k.response](./response.md) - HTTP response handling
- [k.cookie](./cookie.md) - Cookie management
