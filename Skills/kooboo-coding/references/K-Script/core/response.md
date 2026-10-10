# k.response

> HTTP response handling — set response content, status codes, redirects, and more

## Overview

`k.response` provides methods for configuring HTTP responses, including writing content, setting headers, redirecting, and returning JSON.

::: warning Do not use this to return results to AI from run_kscript
`k.response.write` / `json` and related methods write the **HTTP response body** (for the browser or API client) and **do not** become the return value of the `run_kscript` tool; the AI tool receives only the JS `Evaluate` `return` value. For probe scripts, `return` an object.

Within an **API route handler**, prefer **`return`ing data** from the callback (the framework serializes it); use `k.response.*` only for streaming output, a special Content-Type, or non-JSON responses.
:::

## TypeScript definition

```ts
interface Response {
  meta: PageMeta;

  write(value: any): void;           // 输出内容
  setHeader(key: string, value: string): void;  // 设置响应头
  redirect(url: string, absolute?: boolean): void;  // 重定向
  json(value: any): void;            // 返回 JSON
  binary(contentType: string, bytes: number[]): void;  // 二进制响应
  binary(contentType: string, bytes: number[], filename: string): void;
  file(path: string, contentType?: string, fileName?: string): void;  // 文件响应
  statusCode(code: number): void;    // 设置状态码
  unauthorized(): void;               // 401 未授权
  notFound(): void;                  // 404 未找到
  execute(url: string): void;        // 执行其他 URL
}
```

## Methods

### write()

Writes content to the response body. Non-object values are written directly; objects are serialized as JSON.

```ts
k.api.get(() => {
    k.response.write("hello world")
    k.response.write(1234)

    const obj = { name: "kooboo" }
    k.response.write(obj)
})
// 输出: hello world1234{ "name": "kooboo" }
```

### json()

Serializes the value as JSON and writes it to the response body. In an API handler, returning the value is preferred; use this method when explicitly writing the response body. It does not itself guarantee a `Content-Type` header.

```ts
k.api.get(() => {
    k.response.json({
        success: true,
        data: { id: 1, name: "test" }
    })
})
// 输出: { "success": true, "data": { "id": 1, "name": "test" } }
```

### file()

Returns a file response.

The path is resolved through site File I/O. If the file does not exist, the method throws. When `contentType` or `fileName` is omitted, the runtime derives them from the file.

```ts
// 基础用法
k.api.get(() => {
    k.response.file("images/logo.png")
})

// 指定 Content-Type 和下载文件名
k.api.get(() => {
    k.response.file("docs/report.pdf", "application/pdf", "report-2024.pdf")
})
```

### binary()

Returns a binary data response.

When `filename` is provided, the runtime adds an attachment `Content-Disposition` header.

```ts
k.api.get(() => {
    const svgData = k.file.readBinary("files/logo.svg")
    k.response.binary("image/svg+xml", svgData)
})
```

### statusCode()

Sets the HTTP status code.

```ts
k.api.get(() => {
    k.response.statusCode(404)
    return "Not Found"
})
```

### unauthorized()

Returns a 401 Unauthorized response.

```ts
k.api.get(() => {
    k.response.unauthorized()
})
// HTTP 状态码: 401
```

### notFound()

Returns a 404 Not Found response.

```ts
k.api.get(() => {
    k.response.notFound()
})
```

This sets the status code to 404; it does not generate a response body.

### redirect()

Redirects to the specified URL.

```ts
// 相对路径
k.api.get(() => {
    k.response.redirect("/login")
})

// 绝对路径
k.api.get(() => {
    k.response.redirect("https://example.com")
})

// 特殊协议 (如微信)
k.api.get(() => {
    k.response.redirect("wechat://kooboo.com", true)
})
```


| Parameter | Type | Required | Description |
| ---------- | ------- | --- | ------------- |
| `url` | string | Yes | Target URL |
| `absolute` | boolean | No | Whether this is an absolute URL (including the protocol) |


### setHeader()

Sets a custom response header.

```ts
k.api.get(() => {
    k.response.setHeader("X-Custom-Header", "value")
    k.response.setHeader("Access-Control-Allow-Origin", "*")
    return "ok"
})
```

### execute()

Executes another URL in the current context and writes its result to the response.

```ts
// API: /api/base
k.api.get("base", () => {
    k.response.json({ base: "data" })
})

// API: /api/wrapper
k.api.get("wrapper", () => {
    k.response.execute("/api/base")
    k.response.write(" appended")
})
// 输出: { "base": "data" } appended
```

## Common use case

### File download

```ts
k.api.get("download", () => {
    const filePath = "exports/data.csv"
    k.response.file(filePath, "text/csv", "export.csv")
})
```

## Related documents

- [k.request](./request.md) - HTTP request handling
- [k.cookie](./cookie.md) - Cookie management
