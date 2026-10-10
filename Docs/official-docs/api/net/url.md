# k.net.url

> Easy synchronous HTTP requests (string/object/binary)

## Overview

`k.net.url` initiates a **synchronous** HTTP call to the remote URL and directly returns a string, `object` or `byte[]` according to the method name. The writing method is short and suitable for quickly grabbing content within the script.

::: tip When to use httpClient and when to use url
- **Proxy forwarding, complex header, multipart, require `KHttpResponse` disk placement or status code judgment** → use [k.net.httpClient](./httpclient.md) (more complete, configurable timeout and batch tasks).
- **Quickly pull the binary of a remote image or file** → `k.net.url.getAsBinary(url)` is often the most straightforward.
- **One line of GET text or JSON** → `k.net.url.get` / `k.net.url.getJson`.
:::

The same method usually provides four authentication/header forms (besides just URL):

| form | Parameter | Description |
|------|------|------|
| No authentication | `url` | URL only |
| Bearer | `url`, `token` | Automatically add Bearer |
| Basic | `url`, `username`, `password` | HTTP Basic |
| Custom header | `url`, `headers` | Dictionary, such as `{ Authentication: "Bearer xxx" }` |

The method names in the following sections all support the four overloads in the above table (the document does not list them repeatedly).

## GET

| Method | Return | Description |
|------|------|------|
| `get` | `string` | Response body text |
| `getAsObject` | `object` | Parse the response into a JavaScript object |
| `getAsBinary` | `number[]` | Response bytes (**Pull images and other binary**) |
| `getJson` | `object` | Parse the response as JSON |

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

| Method | Return | Description |
|------|------|------|
| `post` | `string` | POST string body |
| `postAsObject` | `object` | Response parsed into object |
| `postAsBinary` | `object` | Return in object form after POST (see runtime behavior) |
| `postData` | `string` | POST serialized object (form/JSON is implementation determined) |
| `postDataAsObject` | `object` | `postData` and the response is an object |
| `postform` | `string` | `application/x-www-form-urlencoded` Form POST |

```ts
k.api.post("echo", () => {
    const body = k.net.url.post("https://httpbin.org/post", "hello=world")
    return { containsHello: body && body.indexOf("hello") > -1 }
})
```

## PUT/PATCH/DELETE

| verb | string response | object response |
|------|----------|----------|
| PUT | `put` | `putAsObject` |
| PATCH | `patch` | `patchAsObject` |
| DELETE | `delete` | `deleteAsObject` |

`patchData` is similar to `postData` and is used to submit the object body.

## other

| Method | Description |
|------|------|
| `downloadZip(url)` | Download ZIP (specific behavior is subject to console/site configuration, please verify in the target environment before use) |

## Example: Agent vs. Binary Division of Labor

```ts
// Recommended: proxy a third-party API
k.api.post("proxyApi", () => {
    const content = k.net.httpClient.createJsonContent(k.request.body)
    const res = k.net.httpClient.send(k.request.queryString.target, "post", content, {
        Authorization: k.request.headers.authorization
    })
    return { status: res.statusCode, body: res.bodyString() }
})

// Quick path: fetch remote image bytes and write them to the media library
k.api.post("importLogo", () => {
    const bytes = k.net.url.getAsBinary("https://www.kooboo.com/img/logo-white.png")
    k.media.writeBinary("/import/logo-white.png", bytes, true)
    return { imported: k.media.exists("/import/logo-white.png") }
})
```

## Related Docs

- [k.net.httpClient](./httpclient.md)
- [k.media](../media/)
