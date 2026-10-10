# k.request

> HTTP request information - obtain various data requested by the client

## Overview

`k.request` provides an interface for obtaining HTTP request information, including query parameters, form data, file upload, client information, etc.

## TypeScript definition

```ts
interface Request {
  queryString: KDictionary;      // URL query parameters
  form: KDictionary;             // Form data
  files: UploadFile[];           // Uploaded files
  body: string;                  // Request body text
  culture: string;                // Culture/language setting
  model: any;                    // Request model
  method: string;                // HTTP method (GET/POST/PUT/DELETE)
  clientIp: string;              // Client IP address
  clientCountry: CountryLocationModel; // Client country information
  headers: KDictionary;          // Request headers
  url: string;                   // Current request URL
  host: string;                  // Host name
  page: Page;                    // Current page object

  get(key: string): string;      // Get dynamic route parameters
  setCulture(culture: string): void;
  resetCulture(): void;
}

interface UploadFile {
  bytes: string;                 // File binary content (Base64)
  contentType: number;           // MIME type
  fileName: string;              // File name
  name: string;                  // Form field name
  save(filename: string): FileInfo; // Save file
}
```

## Properties

### queryString

Get URL query parameters.

```ts
// GET /api/users?page=1&limit=10
k.api.get(() => {
    const page = k.request.queryString.get("page")   // "1"
    const limit = k.request.queryString.get("limit")  // "10"
    return { page, limit }
})
// Returns: { "page": "1", "limit": "10" }
```

### form

Get form data for POST/PUT requests.

```ts
k.api.post(() => {
    const data = k.request.form
    return { received: data }
})
```

### files

Get the list of uploaded files.

```ts
k.api.post(() => {
    if (k.request.files.length > 0) {
        k.request.files.forEach((item) => {
            // Save to KFile
            item.save(item.fileName)
        })
    }
    return { uploaded: k.request.files.length }
})
```

### headers

Get HTTP request headers.

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

Get the client IP address (IPv4 only).

```ts
k.api.get(() => {
    return { ip: k.request.clientIp }
})
// Returns: { "ip": "120.36.245.209" }
```

### body

Get the request body text content (usually used for POST/PUT).

::: tip note
`k.request.body` returns the **original string** and needs to be parsed manually:
```ts
const data = JSON.parse(k.request.body)
```
:::

```ts
k.api.post(() => {
    const body = k.request.body
    return { body }
})
// Returns the raw request body text
```

**Parse JSON data:**
```ts
k.api.post(() => {
    const { username, password } = JSON.parse(k.request.body || '{}')
    return { username }
})
```

### method

Get the HTTP request method.

```ts
k.api.get(() => {
    return { method: k.request.method }
})
// Returns: { "method": "GET" }
```

### url/host

Get the URL and hostname of the current request.

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

Get dynamic routing parameters.

```ts
// Assume the route is: /api/users/{id}
/api/users/123

k.api.get(() => {
    const id = k.request.get("id")  // "123"
    return { id }
})
```

::: warning priority description
If `query string parameters (queryString)` and `dynamic route parameters` have the same name, `k.request.get()` will return the request parameters first.
:::

## Related Docs

- [k.response](../response/) - HTTP response handling
- [k.cookie](../cookie/) - Cookie Management
- [k.session](../session/) - session state
