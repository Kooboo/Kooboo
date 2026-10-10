# k.response

> HTTP response processing - set response content, status code, redirection, etc.

## Overview

`k.response` provides various methods for setting HTTP responses, including returning content, setting header information, redirecting, returning JSON, etc.

## TypeScript definition

```ts
interface Response {
  meta: PageMeta;

  write(value: any): void;           // Write content
  setHeader(key: string, value: string): void;  // Set response header
  redirect(url: string, absolute?: boolean): void;  // Redirect
  json(value: any): void;            // Return JSON
  renderView(ViewBody: string): void; // Render view
  binary(contentType: string, bytes: number[]): void;  // Binary response
  binary(contentType: string, bytes: number[], filename: string): void;
  file(path: string, contentType?: string, fileName?: string): void;  // File response
  statusCode(code: number): void;    // Set status code
  unauthorized(): void;               // 401 unauthorized
  notFound(): void;                  // 404 not found
  execute(url: string): void;        // Execute another URL
}
```

## Methods

### write()

Output content to the response body. Non-object types will be output directly, and objects will be serialized into JSON.

```ts
k.api.get(() => {
    k.response.write("hello world")
    k.response.write(1234)

    const obj = { name: "kooboo" }
    k.response.write(obj)
})
// Output: hello world1234{ "name": "kooboo" }
```

| Parameter | Type | Required | Description |
|------|------|------|------|
| `value` | `any` | yes | Content to write to the response body |

**Returns:** `void`.

### json()

Output the response in JSON format. `Content-Type: application/json` is automatically set.

```ts
k.api.get(() => {
    k.response.json({
        success: true,
        data: { id: 1, name: "test" }
    })
})
// Output: { "success": true, "data": { "id": 1, "name": "test" } }
```

| Parameter | Type | Required | Description |
|------|------|------|------|
| `value` | `any` | yes | Value to serialize as JSON |

**Returns:** `void`.

### file()

Return file response.

```ts
// Basic usage
k.api.get(() => {
    k.response.file("images/logo.png")
})

// Specify Content-Type and download filename
k.api.get(() => {
    k.response.file("docs/report.pdf", "application/pdf", "report-2024.pdf")
})
```

| Parameter | Type | Required | Description |
|------|------|------|------|
| `path` | `string` | yes | Site file path |
| `contentType` | `string` | no | Response Content-Type |
| `fileName` | `string` | no | File name used for download |

**Returns:** `void`.

### binary()

Returns a binary data response.

```ts
k.api.get(() => {
    const svgData = k.file.readBinary("files/logo.svg")
    k.response.binary("image/svg+xml", svgData)
})
```

| Parameter | Type | Required | Description |
|------|------|------|------|
| `contentType` | `string` | yes | Response Content-Type |
| `bytes` | `number[]` | yes | Binary byte array |
| `filename` | `string` | no | Download file name |

**Returns:** `void`.

### statusCode()

Set HTTP status code.

```ts
k.api.get(() => {
    k.response.statusCode(404)
    return "Not Found"
})
```

| Parameter | Type | Required | Description |
|------|------|------|------|
| `code` | `number` | yes | HTTP status code |

**Returns:** `void`.

### unauthorized()

Returns a 401 Unauthorized response.

```ts
k.api.get(() => {
    // statusCode(401) is already set internally
    k.response.unauthorized()
})
// HTTP status code: 401
// Message: "Unauthorized access"
```

**Parameters:** None.

**Returns:** `void`.

### notFound()

Returns 404 Response Not Found.

```ts
k.api.get(() => {
    k.response.notFound()
})
```

**Parameters:** None.

**Returns:** `void`.

### redirect()

Redirect to the specified URL.

```ts
// Relative path
k.api.get(() => {
    k.response.redirect("/login")
})

// Absolute path
k.api.get(() => {
    k.response.redirect("https://example.com")
})

// Custom protocol (such as WeChat)
k.api.get(() => {
    k.response.redirect("wechat://kooboo.com", true)
})
```

| Parameter | Type | Required | Description |
|------|------|------|------|
| `url` | string | yes | Target URL |
| `absolute` | boolean | no | Whether it is an absolute path (including protocol) |

**Returns:** `void`.

### setHeader()

Set custom response headers.

```ts
k.api.get(() => {
    k.response.setHeader("X-Custom-Header", "value")
    k.response.setHeader("Access-Control-Allow-Origin", "*")
    return "ok"
})
```

| Parameter | Type | Required | Description |
|------|------|------|------|
| `key` | `string` | yes | Response header name |
| `value` | `string` | yes | Response header value |

**Returns:** `void`.

### renderView()

Renders an HTML string containing `<view>` at the current output position. Commonly used to output View in `<head>`** of **Layout/Page (`<view id="...">` cannot be written directly in the head).

```ts
// Inside <script env="server"> (template rendering, not an API route)
k.response.renderView("<view id='tailwind'></view>")
```

| Parameter | Type | Required | Description |
|------|------|------|------|
| `ViewBody` | `string` | yes | Tagged string containing `<view id="view name">` |

**Returns:** `void`.

The abbreviation can also be used in the template: `<script env="server" view="tailwind"></script>` (`view` is the View resource name). See [Template Engine - Referencing a View Inside `<head>`](../../templateEngine/view/index.md#referencing-a-view-inside-head) for details.

### execute()

Execute another URL in the current context and write the result to the response.

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
// Output: { "base": "data" } appended
```

| Parameter | Type | Required | Description |
|------|------|------|------|
| `url` | `string` | yes | URL to execute in the current context |

**Returns:** `void`.

## Common usage

### CORS cross-domain settings

```ts
k.api.options(() => {
    k.response.setHeader("Access-Control-Allow-Origin", "*")
    k.response.setHeader("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE")
    k.response.setHeader("Access-Control-Allow-Headers", "Content-Type")
})
```

### File download

```ts
k.api.get("download", () => {
    const filePath = "exports/data.csv"
    k.response.file(filePath, "text/csv", "export.csv")
})
```

## Related Docs

- [k.request](../request/) - HTTP request handling
- [k.cookie](../cookie/) - Cookie Management
- [k.session](../session/) - session state
