# k.utils.uri

> URL parsing and query strings

## Overview

`k.utils.uri` provides `parse` and query parameter reading and writing; `parse` returns a `KUriBuilder` instance.

## parse()

| Parameter | Type | Required | Description |
|------|------|------|------|
| `uri` | `string` | Yes | Full URL to parse |

**Returns:** `KUriBuilder \| null`.

```ts
const builder = k.utils.uri.parse("https://example.com/path?q=1")
builder.setQueryParam("page", "2")
const url = builder.toUri()
```

## Convenience method

| method | illustrate |
|------|------|
| `setQueryParam(uri, name, value)` | Set query parameters and return new URL string |
| `getQueryParam(uri, name)` | Read query parameters |

| Parameter | Type | Required | Description |
|------|------|------|------|
| `uri` | `string` | Yes | Source URL |
| `name` | `string` | Yes | Query parameter name |
| `value` | `string` | Required for `setQueryParam` | Query parameter value |

| Method | Return |
|------|------|
| `setQueryParam(uri, name, value)` | `string \| null` |
| `getQueryParam(uri, name)` | `string \| null` |

```ts
const campaign = k.utils.uri.getQueryParam("https://example.com/?utm_campaign=spring", "utm_campaign")
const nextUrl = k.utils.uri.setQueryParam("/products?page=1", "page", "2")
```

## KUriBuilder

| member | illustrate |
|------|------|
| `scheme` / `host` / `port` / `path` / `query` | URI components |
| `userName` / `password` | Credentials |
| `setQueryParam(name, value)` | Modify query string |
| `getQueryParam(name)` | Read query parameters |
| `toUri()` | Output full URL string |

`KUriBuilder.setQueryParam()` returns `void`, `getQueryParam()` returns `string \| null`, and `toUri()` returns `string`.

## Related Docs

- [k.utils Overview](./index.md)
