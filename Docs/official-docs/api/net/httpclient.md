# k.net.httpClient

> Outbound HTTP: unified `send`, multiple request bodies, and batch tasks

## Overview

`k.net.httpClient` encapsulates outbound requests based on `HttpClient` and supports JSON, forms, multipart and parallel batch downloads. When **proxy forwarding third-party interface** in the API, this module is used first.

The return value is **`KHttpResponse`**, and the results are read through `success`, `statusCode`, `bodyString()`, etc.

## Create request body

| Method | Description |
|------|------|
| `createJsonContent(body)` | `application/json` |
| `createStringContent(body)` | plain text |
| `createFormUrlEncodedContent(dict)` | `application/x-www-form-urlencoded` |
| `createMultipartFormDataContent()` | multipart, see below |

```ts
const json = k.net.httpClient.createJsonContent({ name: "kooboo" })
const text = k.net.httpClient.createStringContent("plain text")
const form = k.net.httpClient.createFormUrlEncodedContent({ key: "value" })

const multipart = k.net.httpClient.createMultipartFormDataContent()
multipart.add("name", "jobs")
multipart.addFile("sticker", "1.png", k.file.readBinary("1.png"))
```

## send()

Send a single HTTP request.

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

| Parameter | Type | Required | Description |
|------|------|------|------|
| `url` | `string` | yes | Full URL |
| `method` | `string` | yes | `get`, `post`, `put`, `delete`, `options` (not case sensitive) |
| `content` | `KHttpContent` | no | Request body; usually omitted for GET |
| `headers` | `object` | no | Additional request header key-value pairs |
| `timeout` | `number` | no | Timeout seconds |

**Return:** `KHttpResponse`

### KHttpResponse common members

| Member | Description |
|------|------|
| `success` | HTTP success status code |
| `statusCode` | Numeric status code |
| `bodyString()` | Response body text |
| `bodyBinary()` | Response body byte array |
| `getHeader(name)` | Read response headers (array of strings) |
| `contentType` | `{ mediaType, charSet }` |
| `contentDisposition` | `{ fileName, size }` |
| `save(path)` / `saveBinary(path)` | Write response to site `k.file` path |

```ts
const response = k.net.httpClient.send("https://example.com/file.zip", "get")
if (response.success) {
    response.save("downloads/archive.zip")
}
```

## Bulk request

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

`RequestTask` Common fields:

| Field | Description |
|------|------|
| `responseType` | `"String"`, `"Binary"`, `"File"` |
| `savePath` | Required when `responseType` is `"File"`, relative site file path |
| `isSuccess` | Was the mission successful? |
| `statusCode` | HTTP status code |
| `errorMessage` | Error message on failure |
| `stringResult` / `binaryResult` / `fileResult` | Store results by type |

## Related Docs

- [k.net Overview](./index.md)
- [k.net.url](./url.md) — Simple GET binary and other scenarios
- [k.net.webSocket](./websocket.md)
