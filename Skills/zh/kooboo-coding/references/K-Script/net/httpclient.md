# k.net.httpClient

> 出站 HTTP 请求 — JSON/表单/文件上传、响应解析、批量并发下载

## 概述

KScript 通过 `k.net.httpClient` 封装 .NET `HttpClient`，用于在 API、CodeBlock 或 `<script env="server">` 中调用**外部 HTTP 服务**。与 `k.net.url`（Curl）相比，`httpClient` 提供更结构化的请求体构建、响应对象和**批量并行**任务。

典型场景：

1. **调用第三方 REST API**（JSON POST、Bearer 头等）
2. **上传 multipart 表单**（含站点内文件二进制）
3. **下载远程文件**到站点 `FileIO` 路径
4. **批量拉取**多个 URL（并行 `send(batchRequest)`）

::: tip 与 k.net.url 的选择
| 场景 | 推荐 |
|------|------|
| 简单 GET/POST 字符串、Basic/Bearer 快捷参数、Cookie 会话 | `k.net.url` |
| JSON/表单/multipart 请求体、读响应头、保存到 FileIO、批量任务 | `k.net.httpClient` |
:::

## TypeScript 定义

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

## 快速开始

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

### 读取响应头与类型

```ts
const response = k.net.httpClient.send('https://api.example.com/data', 'get')

const values = response.getHeader('X-Request-Id')
const mediaType = response.contentType?.mediaType
const fileName = response.contentDisposition?.fileName
```

## 构建请求体

### createJsonContent(body)

序列化对象为 `application/json` 请求体。

```ts
const content = k.net.httpClient.createJsonContent({ q: 'kooboo' })
content.setHeader('X-Custom', '1')
```

### createStringContent(body)

纯文本请求体（默认 `text/plain`，可通过 `setHeader` 设置 `Content-Type`）。

```ts
const content = k.net.httpClient.createStringContent('<xml>...</xml>')
content.setHeader('Content-Type', 'application/xml')
```

### createFormUrlEncodedContent(body)

`application/x-www-form-urlencoded`；字典值会 `ToString()`。

```ts
const content = k.net.httpClient.createFormUrlEncodedContent({
  grant_type: 'client_credentials',
  client_id: 'app'
})
const response = k.net.httpClient.send('https://auth.example.com/token', 'post', content)
```

### createMultipartFormDataContent()

`multipart/form-data`，用于字段 + 文件混合上传。

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

| 方法 | 说明 |
|------|------|
| `add(name, value)` | 添加普通表单字段 |
| `addFile(name, fileName, binary, contentType?)` | 添加文件 part；`binary` 为字节数组（如 `k.file.readBinary`） |

## send(url, method, content?, headers?, timeout?)

发起**同步阻塞**的 HTTP 请求。每次调用通过 `IHttpClientFactory` 创建独立 `HttpClient` 实例。

| 参数 | 说明 |
|------|------|
| `url` | 完整 URL（含 query） |
| `method` | `get` / `post` / `put` / `delete` / `options`；未识别时按 `get` |
| `content` | 可选，由上述 `create*Content` 生成 |
| `headers` | 可选，键值对写入**请求头**（非 Content 头） |
| `timeout` | 可选，超时秒数；设置 `HttpClient.Timeout` |

```ts
const response = k.net.httpClient.send(
  'https://httpbin.org/put',
  'put',
  k.net.httpClient.createStringContent('ok'),
  { 'X-Trace': '1' },
  60
)
```

::: warning 阻塞调用
`send()` 在服务端线程上同步等待远程响应。在 API 中调用外部慢接口会占用请求线程，建议设置合理 `timeout`，批量场景优先用 `createBatchRequest()`。
:::

## KHttpResponse

| 成员 / 方法 | 说明 |
|-------------|------|
| `success` | HTTP 状态码是否为 2xx（`IsSuccessStatusCode`） |
| `statusCode` | 数字状态码，如 `200`、`404` |
| `getHeader(name)` | 先查响应头，再查 Content 头；无则返回 `[]` |
| `contentDisposition` | 解析 `Content-Disposition`（含 `filename*` UTF-8） |
| `contentType` | `mediaType`、`charSet` |
| `bodyString()` | 读取正文为字符串（消费响应流） |
| `bodyBinary()` | 读取正文为 `byte[]` |
| `save(path)` / `saveBinary(path)` | 将正文写入站点 **FileIO** 路径（相对路径经 `GetFullName` 解析） |

```ts
const response = k.net.httpClient.send('https://cdn.example.com/asset.zip', 'get')

if (response.success) {
  const info = response.save('downloads/asset.zip')
  return { path: info.fullName, size: info.size }
}
```

::: tip 响应流只能读一次
`bodyString()`、`bodyBinary()`、`save()` 都会读取响应 Content 流。同一次 `send` 的返回值上不要混用多种读法。
:::

## 批量请求 createBatchRequest()

用于**并行**执行多个出站请求；`send(batchRequest)` 阻塞直到全部任务完成（内部 `Task.WaitAll`）。

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

### RequestTask 字段

| 字段 | 说明 |
|------|------|
| `url` / `method` | 由 `addTask(url, method)` 设置 |
| `timeout` | 单任务超时（秒），默认 **45** |
| `content` | 可选请求体 |
| `header` | 可选请求头字典 |
| `responseType` | 成功后如何消费响应：`String`、`Binary`、`File`（**区分大小写**） |
| `savePath` | `responseType === 'File'` 时**必填**，传给 `response.save()` |
| `isSuccess` | 请求无异常且 HTTP 为 2xx 时为 `true` |
| `statusCode` | 最后一次响应状态码（失败时也可能有值） |
| `errorMessage` | 异常或 非 2xx 时的错误信息 |
| `stringResult` / `binaryResult` / `fileResult` | 按 `responseType` 填充 |

::: warning responseType 大小写
源码使用 `switch (ResponseType)` 匹配 `"String"`、`"Binary"`、`"File"`。写 `"string"` 不会填充结果。
:::

## 在 API / CodeBlock 中使用

出站 HTTP 通常写在 **API** 或 **CodeBlock** 中，由前端或其它 KScript 通过 `k.api.url()` 触发。

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

## 注意事项

| 要点 | 说明 |
|------|------|
| 同步模型 | `send(url, …)` 与 `send(batch)` 均阻塞当前 KScript 执行线程 |
| 超时 | 单次 `send` 的 `timeout` 为秒；批量任务用 `task.timeout`（默认 45s） |
| 成功判定 | `response.success` / `task.isSuccess` 仅看 HTTP 2xx，业务错误 JSON 需自行解析 body |
| 请求头 vs Content 头 | `send` 的 `headers` 只加在 `HttpRequestMessage.Headers`；`Content-Type` 等应设在 `KHttpContent.setHeader` |
| 文件路径 | `save` / 批量 `savePath` 走站点 FileIO，路径规则与 `k.file` 一致 |
| 日志 | 使用 `k.logger`，不要使用 `console.log` |
| 与 Curl 重复 | 同一逻辑不要混用 `k.net.url.post` 与 `httpClient` 两套风格，团队内选一种即可 |

## 实现参考（源码）

| 类型 | 路径 |
|------|------|
| `KHttpClient` | `Kooboo.Sites/Scripting/Global/NET/HttpClient/KHttpClient.cs` |
| `KHttpResponse` | `Kooboo.Sites/Scripting/Global/NET/HttpClient/KHttpResponse.cs` |
| `KBatchRequest` / `RequestTask` | `Kooboo.Sites/Scripting/Global/NET/HttpClient/KBatchRequestTask.cs` |
| 挂载到 `k.net` | `Kooboo.Sites/Scripting/Global/NET/KNET.cs` → `HttpClient` 属性（脚本中为 `httpClient`） |

框架内 SSR Fetch 查询（`k-data` 路线）同样使用 `KHttpClient.Send`，见 `Kooboo.Sites/Render/Queries/QueryDefinitions/Fetch/`。

## 相关文档

- [k.net 索引](index.md)
- [k.net.webSocket](websocket.md) — 入站实时连接（与出站 HTTP 互补）
- [k.api](../core/api.md) — API 路由
- [k.file](../file/index.md) — 读取本地文件供 `addFile` 使用
- [k.logger](../core/logger.md) — 日志记录
