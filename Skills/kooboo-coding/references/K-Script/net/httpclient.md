# k.net.httpClient

> Outbound HTTP requests — JSON/forms/file uploads, response parsing, and concurrent batch downloads

## Overview

KScript wraps .NET `HttpClient` as `k.net.httpClient` for calling **external HTTP services** from an API, CodeBlock, or `<script env="server">`. Compared with `k.net.url` (Curl), `httpClient` provides structured request-body builders, response objects, and **parallel batch** tasks.

Typical scenarios:

1. **Call a third-party REST API** (JSON POST, Bearer headers, and so on)
2. **Upload a multipart form** (including binary files from the site)
3. **Download a remote file** to a site `FileIO` path
4. **Fetch multiple URLs** in parallel (`send(batchRequest)`)

::: tip Choosing between k.net.url and httpClient
| Scenario | Recommendation |
|------|------|
| Simple GET/POST strings, Basic/Bearer shortcuts, or cookie sessions | `k.net.url` |
| JSON/form/multipart bodies, response headers, FileIO saves, or batch tasks | `k.net.httpClient` |
:::

## TypeScript definition

```ts
interface KHttpClient {
  createJsonContent(body: object): KHttpContent;
  createStringContent(body: string): KHttpContent;
  createFormUrlEncodedContent(body: Record<string, unknown>): KHttpContent;
  createMultipartFormDataContent(): KMultipartHttpContent;
  createBatchRequest(): KBatchRequest;

  /** 单次请求；method 不区分大小写；未知 method 按 get 处理 */
  send(
    url: string,
    method: string,
    content?: KHttpContent,
    headers?: Record<string, unknown>,
    timeout?: number
  ): KHttpResponse;

  /** 批量并行请求；阻塞直到全部任务结束 */
  send(request: KBatchRequest): void;
}

interface KHttpContent {
  getHeader(name: string): string[];
  setHeader(name: string, value: string): void;
}

interface KMultipartHttpContent extends KHttpContent {
  add(name: string, value: string): void;
  addFile(name: string, fileName: string, binary: number[], contentType?: string): void;
}

interface KHttpResponse {
  success: boolean;
  statusCode: number;
  contentDisposition?: { fileName?: string; size?: number };
  contentType?: { mediaType: string; charSet?: string };
  getHeader(name: string): string[];
  bodyString(): string;
  bodyBinary(): number[];
  save(path: string): FileInfo;
  saveBinary(path: string): FileInfo;
}

interface KBatchRequest {
  addTask(url: string, method: string): RequestTask;
  readonly tasks: RequestTask[];
}

interface RequestTask {
  url: string;
  method: string;
  timeout: number; // 默认 45（秒）
  content?: KHttpContent;
  header?: Record<string, unknown>;
  /** `"String"` | `"Binary"` | `"File"`（区分大小写） */
  responseType?: string;
  savePath?: string;
  isSuccess: boolean;
  statusCode: number;
  errorMessage?: string;
  stringResult?: string;
  binaryResult?: number[];
  fileResult?: FileInfo;
}
```

## Quick start

### JSON POST

```ts
k.api.post('/proxy/user', () => {
  const content = k.net.httpClient.createJsonContent({
    name: 'alex',
    age: 23
  })
  const response = k.net.httpClient.send(
    'https://api.example.com/users',
    'post',
    content,
    undefined,
    30
  )

  if (!response.success) {
    k.api.httpCode(response.statusCode)
    return { error: response.bodyString() }
  }

  return JSON.parse(response.bodyString())
})
```

### Read response headers and types

```ts
const response = k.net.httpClient.send('https://api.example.com/data', 'get')

const values = response.getHeader('X-Request-Id')
const mediaType = response.contentType?.mediaType
const fileName = response.contentDisposition?.fileName
```

## Build request bodies

### createJsonContent(body)

Serializes an object as an `application/json` request body.

```ts
const content = k.net.httpClient.createJsonContent({ q: 'kooboo' })
content.setHeader('X-Custom', '1')
```

### createStringContent(body)

Creates a plain-text request body (defaults to `text/plain`; use `setHeader` to set `Content-Type`).

```ts
const content = k.net.httpClient.createStringContent('<xml>...</xml>')
content.setHeader('Content-Type', 'application/xml')
```

### createFormUrlEncodedContent(body)

Creates an `application/x-www-form-urlencoded` body; dictionary values are converted with `ToString()`.

```ts
const content = k.net.httpClient.createFormUrlEncodedContent({
  grant_type: 'client_credentials',
  client_id: 'app'
})
const response = k.net.httpClient.send('https://auth.example.com/token', 'post', content)
```

### createMultipartFormDataContent()

Creates `multipart/form-data` for mixed field and file uploads.

```ts
const content = k.net.httpClient.createMultipartFormDataContent()
content.add('name', 'jobs')
content.addFile(
  'avatar',
  'photo.png',
  k.file.readBinary('uploads/photo.png'),
  'image/png'
)

const response = k.net.httpClient.send('https://api.example.com/upload', 'post', content)
```

| Method | Description |
|------|------|
| `add(name, value)` | Adds a regular form field |
| `addFile(name, fileName, binary, contentType?)` | Adds a file part; `binary` is a byte array (for example, from `k.file.readBinary`) |

## send(url, method, content?, headers?, timeout?)

Sends a **synchronous, blocking** HTTP request. Each call creates an independent `HttpClient` through `IHttpClientFactory`.

| Parameter | Description |
|------|------|
| `url` | Complete URL (including the query string) |
| `method` | `get` / `post` / `put` / `delete` / `options`; unrecognized values are treated as `get` |
| `content` | Optional; created by one of the `create*Content` methods above |
| `headers` | Optional; key-value pairs are written to **request headers** (not Content headers) |
| `timeout` | Optional timeout in seconds; sets `HttpClient.Timeout` |

```ts
const response = k.net.httpClient.send(
  'https://httpbin.org/put',
  'put',
  k.net.httpClient.createStringContent('ok'),
  { 'X-Trace': '1' },
  60
)
```

::: warning Blocking call
`send()` waits synchronously for the remote response on the server thread. Calling a slow external service from an API occupies the request thread; set a reasonable `timeout` and prefer `createBatchRequest()` for batch scenarios.
:::

## KHttpResponse

| Member / method | Description |
|-------------|------|
| `success` | Whether the HTTP status is 2xx (`IsSuccessStatusCode`) |
| `statusCode` | Numeric status code, such as `200` or `404` |
| `getHeader(name)` | Checks response headers, then Content headers; returns `[]` when absent |
| `contentDisposition` | Parses `Content-Disposition` (including UTF-8 `filename*`) |
| `contentType` | `mediaType` and `charSet` |
| `bodyString()` | Reads the body as a string (consumes the response stream) |
| `bodyBinary()` | Reads the body as `byte[]` |
| `save(path)` / `saveBinary(path)` | Writes the body to a site **FileIO** path (relative paths are resolved with `GetFullName`) |

```ts
const response = k.net.httpClient.send('https://cdn.example.com/asset.zip', 'get')

if (response.success) {
  const info = response.save('downloads/asset.zip')
  return { path: info.fullName, size: info.size }
}
```

::: tip The response stream can be read only once
`bodyString()`, `bodyBinary()`, and `save()` all read the response Content stream. Do not mix multiple readers on the result of the same `send` call.
:::

## Batch requests with createBatchRequest()

Executes multiple outbound requests **in parallel**; `send(batchRequest)` blocks until all tasks complete (internally `Task.WaitAll`).

```ts
const batch = k.net.httpClient.createBatchRequest()

const img = batch.addTask('https://cdn.example.com/1.png', 'get')
img.responseType = 'File'
img.savePath = 'cache/1.png'
img.timeout = 30

const meta = batch.addTask('https://api.example.com/info.json', 'get')
meta.responseType = 'String'
meta.header = { Accept: 'application/json' }

k.net.httpClient.send(batch)

if (batch.tasks[0].isSuccess) {
  k.logger.information('httpClient', batch.tasks[0].fileResult?.fullName ?? '')
} else {
  k.logger.error('httpClient', batch.tasks[0].errorMessage ?? '')
}

const jsonText = batch.tasks[1].stringResult
```

### RequestTask fields

| Field | Description |
|------|------|
| `url` / `method` | Set by `addTask(url, method)` |
| `timeout` | Per-task timeout in seconds; defaults to **45** |
| `content` | Optional request body |
| `header` | Optional request-header dictionary |
| `responseType` | How to consume a successful response: `String`, `Binary`, or `File` (**case-sensitive**) |
| `savePath` | **Required** when `responseType === 'File'`; passed to `response.save()` |
| `isSuccess` | `true` when the request has no exception and the HTTP status is 2xx |
| `statusCode` | Last response status code (may also have a value on failure) |
| `errorMessage` | Error message for an exception or non-2xx response |
| `stringResult` / `binaryResult` / `fileResult` | Populated according to `responseType` |

::: warning responseType is case-sensitive
The source uses `switch (ResponseType)` to match `"String"`, `"Binary"`, and `"File"`. Writing `"string"` does not populate a result.
:::

## Use in an API / CodeBlock

Outbound HTTP is usually written in an **API** or **CodeBlock**, triggered by the frontend or another KScript through `k.api.url()`.

```ts
// CodeBlock: Services.externalApi
export function fetchPartnerProfile(userId: string) {
  const url = `https://partner.example.com/users/${userId}`
  const response = k.net.httpClient.send(url, 'get', undefined, {
    Authorization: 'Bearer ' + process.env.PARTNER_TOKEN
  })

  if (!response.success) {
    throw new Error(`partner api ${response.statusCode}: ${response.bodyString()}`)
  }

  return JSON.parse(response.bodyString())
}
```

```ts
// Api: /api/partner/profile
k.api.get('/profile', () => {
  const id = k.request.queryString.get('id')
  if (!id) {
    k.api.httpCode(400)
    return { error: 'missing id' }
  }
  return k.module.import('Services.externalApi').fetchPartnerProfile(id)
})
```

## Notes

| Point | Description |
|------|------|
| Synchronous model | Both `send(url, …)` and `send(batch)` block the current KScript execution thread |
| Timeout | `timeout` for a single `send` is in seconds; batch tasks use `task.timeout` (45s by default) |
| Success check | `response.success` / `task.isSuccess` checks only HTTP 2xx; parse business-error JSON from the body yourself |
| Request headers vs Content headers | `send` `headers` are added only to `HttpRequestMessage.Headers`; set `Content-Type` and similar headers on `KHttpContent.setHeader` |
| File paths | `save` / batch `savePath` use site FileIO, with the same path rules as `k.file` |
| Logging | Use `k.logger`, not `console.log` |
| Avoid duplicate Curl styles | Do not mix `k.net.url.post` and `httpClient` for the same logic; choose one style for the team |

## Implementation references (source)

| Type | Path |
|------|------|
| `KHttpClient` | `Kooboo.Sites/Scripting/Global/NET/HttpClient/KHttpClient.cs` |
| `KHttpResponse` | `Kooboo.Sites/Scripting/Global/NET/HttpClient/KHttpResponse.cs` |
| `KBatchRequest` / `RequestTask` | `Kooboo.Sites/Scripting/Global/NET/HttpClient/KBatchRequestTask.cs` |
| Mounted on `k.net` | `Kooboo.Sites/Scripting/Global/NET/KNET.cs` → `HttpClient` property (exposed as `httpClient` in scripts) |

SSR Fetch queries inside the framework (the `k-data` route) also use `KHttpClient.Send`; see `Kooboo.Sites/Render/Queries/QueryDefinitions/Fetch/`.

## Related documents

- [k.net index](index.md)
- [k.net.webSocket](websocket.md) — inbound real-time connections (complementary to outbound HTTP)
- [k.api](../core/api.md) — API routes
- [k.file](../file/index.md) — read local files for `addFile`
- [k.logger](../core/logger.md) — logging
