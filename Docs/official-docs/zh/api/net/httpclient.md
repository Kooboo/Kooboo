# k.net.httpClient

> 出站 HTTP：统一 `send`、多种请求体与批量任务

## 概述

`k.net.httpClient` 基于 `HttpClient` 封装出站请求，支持 JSON、表单、multipart 与并行批量下载。在 API 中**代理转发第三方接口**时，优先使用本模块。

返回值为 **`KHttpResponse`**，通过 `success`、`statusCode`、`bodyString()` 等读取结果。

## 创建请求体

| 方法 | 说明 |
|------|------|
| `createJsonContent(body)` | `application/json` |
| `createStringContent(body)` | 纯文本 |
| `createFormUrlEncodedContent(dict)` | `application/x-www-form-urlencoded` |
| `createMultipartFormDataContent()` | multipart，见下 |

```ts
const json = k.net.httpClient.createJsonContent({ name: "kooboo" })
const text = k.net.httpClient.createStringContent("plain text")
const form = k.net.httpClient.createFormUrlEncodedContent({ key: "value" })

const multipart = k.net.httpClient.createMultipartFormDataContent()
multipart.add("name", "jobs")
multipart.addFile("sticker", "1.png", k.file.readBinary("1.png"))
```

## send()

发送单次 HTTP 请求。

```ts
k.api.post("proxy", () => {
    const content = k.net.httpClient.createJsonContent({ q: "hello" })
    const response = k.net.httpClient.send("https://httpbin.org/post", "post", content)
    return {
        ok: response.success,
        status: response.statusCode,
        body: response.bodyString()
    }
})
```

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `url` | `string` | 是 | 完整 URL |
| `method` | `string` | 是 | `get`、`post`、`put`、`delete`、`options`（不区分大小写） |
| `content` | `KHttpContent` | 否 | 请求体；GET 通常省略 |
| `headers` | `object` | 否 | 额外请求头键值对 |
| `timeout` | `number` | 否 | 超时秒数 |

**返回：** `KHttpResponse`

### KHttpResponse 常用成员

| 成员 | 说明 |
|------|------|
| `success` | HTTP 是否成功状态码 |
| `statusCode` | 数字状态码 |
| `bodyString()` | 响应正文文本 |
| `bodyBinary()` | 响应正文字节数组 |
| `getHeader(name)` | 读取响应头（字符串数组） |
| `contentType` | `{ mediaType, charSet }` |
| `contentDisposition` | `{ fileName, size }` |
| `save(path)` / `saveBinary(path)` | 将响应写入站点 `k.file` 路径 |

```ts
const response = k.net.httpClient.send("https://example.com/file.zip", "get")
if (response.success) {
    response.save("downloads/archive.zip")
}
```

## 批量请求

```ts
const batch = k.net.httpClient.createBatchRequest()
const imgTask = batch.addTask("https://www.kooboo.com/img/logo-white.png", "get")
imgTask.responseType = "File"
imgTask.savePath = "logo-white.png"

const jsonTask = batch.addTask("https://httpbin.org/json", "get")
jsonTask.responseType = "String"

k.net.httpClient.send(batch)

return {
    imageOk: batch.tasks[0].isSuccess,
    jsonSnippet: batch.tasks[1].stringResult?.slice(0, 80)
}
```

`RequestTask` 常用字段：

| 字段 | 说明 |
|------|------|
| `responseType` | `"String"`、`"Binary"`、`"File"` |
| `savePath` | `responseType` 为 `"File"` 时必填，相对站点文件路径 |
| `isSuccess` | 任务是否成功 |
| `statusCode` | HTTP 状态码 |
| `errorMessage` | 失败时的错误信息 |
| `stringResult` / `binaryResult` / `fileResult` | 按类型存放结果 |

## 相关文档

- [k.net 概述](./index.md)
- [k.net.url](./url.md) — 简便 GET 二进制等场景
- [k.net.webSocket](./websocket.md)
