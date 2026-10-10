# k.net.url

> 简便的同步 HTTP 请求（字符串 / 对象 / 二进制）

## 概述

`k.net.url` 对远程 URL 发起 **同步** HTTP 调用，按方法名直接返回字符串、`object` 或 `byte[]`。写法简短，适合脚本内快速抓取内容。

::: tip 何时用 httpClient，何时用 url
- **代理转发、复杂 Header、multipart、需要 `KHttpResponse` 落盘或状态码判断** → 使用 [k.net.httpClient](./httpclient.md)（更完整、可配置超时与批量任务）。
- **快速拉取远程图片或文件的二进制** → `k.net.url.getAsBinary(url)` 往往最直接。
- **一行 GET 文本或 JSON** → `k.net.url.get` / `k.net.url.getJson`。
:::

同一方法通常提供四种鉴权/头部形态（除仅 URL 外）：

| 形态 | 参数 | 说明 |
|------|------|------|
| 无鉴权 | `url` | 仅 URL |
| Bearer | `url`, `token` | 自动加 Bearer |
| Basic | `url`, `username`, `password` | HTTP Basic |
| 自定义头 | `url`, `headers` | 字典，如 `{ Authentication: "Bearer xxx" }` |

以下各节方法名均支持上表四种重载（文档不重复列出）。

## GET

| 方法 | 返回 | 说明 |
|------|------|------|
| `get` | `string` | 响应正文文本 |
| `getAsObject` | `object` | 将响应解析为 JavaScript 对象 |
| `getAsBinary` | `number[]` | 响应字节（**拉取图片等二进制**） |
| `getJson` | `object` | 将响应按 JSON 解析 |

```ts
k.api.get("fetchImage", () => {
    const bytes = k.net.url.getAsBinary("https://www.kooboo.com/img/logo-white.png")
    return { length: bytes ? bytes.length : 0 }
})

k.api.get("fetchJson", () => {
    const data = k.net.url.getJson("https://httpbin.org/json")
    return { hasSlides: data && !!data.slideshow }
})
```

## POST

| 方法 | 返回 | 说明 |
|------|------|------|
| `post` | `string` | POST 字符串 body |
| `postAsObject` | `object` | 响应解析为对象 |
| `postAsBinary` | `object` | POST 后按对象形态返回（见运行时行为） |
| `postData` | `string` | POST 序列化对象（表单/JSON 由实现决定） |
| `postDataAsObject` | `object` | `postData` 且响应为对象 |
| `postform` | `string` | `application/x-www-form-urlencoded` 表单 POST |

```ts
k.api.post("echo", () => {
    const body = k.net.url.post("https://httpbin.org/post", "hello=world")
    return { containsHello: body && body.indexOf("hello") > -1 }
})
```

## PUT / PATCH / DELETE

| 动词 | 字符串响应 | 对象响应 |
|------|------------|----------|
| PUT | `put` | `putAsObject` |
| PATCH | `patch` | `patchAsObject` |
| DELETE | `delete` | `deleteAsObject` |

`patchData` 与 `postData` 类似，用于提交对象 body。

## 其它

| 方法 | 说明 |
|------|------|
| `downloadZip(url)` | 下载 ZIP（具体行为以控制台/站点配置为准，使用前请在目标环境验证） |

## 示例：代理与二进制分工

```ts
// 推荐：代理第三方 API
k.api.post("proxyApi", () => {
    const content = k.net.httpClient.createJsonContent(k.request.body)
    const res = k.net.httpClient.send(k.request.queryString.target, "post", content, {
        Authorization: k.request.headers.authorization
    })
    return { status: res.statusCode, body: res.bodyString() }
})

// 简便：拉取远程图片字节再写入媒体库
k.api.post("importLogo", () => {
    const bytes = k.net.url.getAsBinary("https://www.kooboo.com/img/logo-white.png")
    k.media.writeBinary("/import/logo-white.png", bytes, true)
    return { imported: k.media.exists("/import/logo-white.png") }
})
```

## 相关文档

- [k.net.httpClient](./httpclient.md)
- [k.media](../media/)
