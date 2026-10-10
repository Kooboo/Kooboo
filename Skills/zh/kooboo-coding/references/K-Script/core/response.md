# k.response

> HTTP 响应处理 - 设置响应内容、状态码、重定向等

## 概述

`k.response` 提供设置 HTTP 响应的各种方法，包括返回内容、设置头信息、重定向、返回 JSON 等。

::: warning 不要用于 run_kscript 向 AI 返回结果
`k.response.write` / `json` 等写入的是 **HTTP 响应体**（给浏览器或 API 客户端），**不会**成为 `run_kscript` 工具的返回值；AI 工具只接收 JS `Evaluate` 的 `return` 值。探测脚本请 `return` 对象。

在 **API 路由处理器**内，优先在回调里 **`return` 数据**（由框架序列化）；仅当需要流式输出、特殊 Content-Type 或非 JSON 时才用 `k.response.*`。
:::

## TypeScript 定义

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

## 方法

### write()

输出内容到响应体。非对象类型会直接输出，对象会被序列化为 JSON。

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

将值序列化为 JSON 并写入响应体。在 API 处理器中通常应直接返回值；只有需要显式写响应体时才使用此方法。该方法本身不保证设置 `Content-Type` 响应头。

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

返回文件响应。

路径通过站点 File I/O 解析；文件不存在时会抛出异常。未传 `contentType` 或 `fileName` 时，运行时会根据文件推导。

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

返回二进制数据响应。

传入 `filename` 时，运行时会添加附件形式的 `Content-Disposition` 响应头。

```ts
k.api.get(() => {
    const svgData = k.file.readBinary("files/logo.svg")
    k.response.binary("image/svg+xml", svgData)
})
```

### statusCode()

设置 HTTP 状态码。

```ts
k.api.get(() => {
    k.response.statusCode(404)
    return "Not Found"
})
```

### unauthorized()

返回 401 未授权响应。

```ts
k.api.get(() => {
    k.response.unauthorized()
})
// HTTP 状态码: 401
```

### notFound()

返回 404 未找到响应。

```ts
k.api.get(() => {
    k.response.notFound()
})
```

该方法只设置 404 状态码，不生成响应体。

### redirect()

重定向到指定 URL。

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


| 参数         | 类型      | 必填  | 描述            |
| ---------- | ------- | --- | ------------- |
| `url`      | string  | 是   | 目标 URL        |
| `absolute` | boolean | 否   | 是否为绝对路径（包含协议） |


### setHeader()

设置自定义响应头。

```ts
k.api.get(() => {
    k.response.setHeader("X-Custom-Header", "value")
    k.response.setHeader("Access-Control-Allow-Origin", "*")
    return "ok"
})
```

### execute()

在当前上下文中执行另一个 URL，并将结果写入响应。

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

## 常见

### 文件下载

```ts
k.api.get("download", () => {
    const filePath = "exports/data.csv"
    k.response.file(filePath, "text/csv", "export.csv")
})
```

## 相关文档

- [k.request](request.md) - HTTP 请求处理
- [k.cookie](cookie.md) - Cookie 管理
