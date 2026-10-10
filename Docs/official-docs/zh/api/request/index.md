# k.request

> HTTP 请求信息 - 获取客户端请求的各种数据

## 概述

`k.request` 提供获取 HTTP 请求信息的接口，包括查询参数、表单数据、文件上传、客户端信息等。

## TypeScript 定义

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
  bytes: string;                 // 文件二进制内容 (Base64)
  contentType: number;           // MIME 类型
  fileName: string;              // 文件名
  name: string;                  // 表单字段名
  save(filename: string): FileInfo; // 保存文件
}
```

## 属性

### queryString

获取 URL 查询参数。

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

获取 POST/PUT 请求的表单数据。

```ts
k.api.post(() => {
    const data = k.request.form
    return { received: data }
})
```

### files

获取上传的文件列表。

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

获取 HTTP 请求头。

```ts
k.api.get(() => {
    const headers = k.request.headers
    return {
        accept: headers.get("Accept"),
        userAgent: headers.get("UserAgent"),
        cookie: headers.get("Cookie")
    }
})
```

### clientIp

获取客户端 IP 地址（仅支持 IPv4）。

```ts
k.api.get(() => {
    return { ip: k.request.clientIp }
})
// 返回: { "ip": "120.36.245.209" }
```

### body

获取请求体文本内容（通常用于 POST/PUT）。

::: tip 注意
`k.request.body` 返回的是**原始字符串**，需要手动解析：
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

**解析 JSON 数据：**
```ts
k.api.post(() => {
    const { username, password } = JSON.parse(k.request.body || '{}')
    return { username }
})
```

### method

获取 HTTP 请求方法。

```ts
k.api.get(() => {
    return { method: k.request.method }
})
// 返回: { "method": "GET" }
```

### url / host

获取当前请求的 URL 和主机名。

```ts
k.api.get(() => {
    return {
        url: k.request.url,    // "/api/users"
        host: k.request.host   // "example.kooboo.cn"
    }
})
```

## 方法

### get(key)

获取动态路由参数。

```ts
// 假设路由: /api/users/{id}
/api/users/123

k.api.get(() => {
    const id = k.request.get("id")  // "123"
    return { id }
})
```

::: warning 优先级说明
如果 `请求参数 (queryString)` 和 `动态路由参数` 重名，`k.request.get()` 优先返回请求参数。
:::

## 相关文档

- [k.response](../response/) - HTTP 响应处理
- [k.cookie](../cookie/) - Cookie 管理
- [k.session](../session/) - 会话状态
