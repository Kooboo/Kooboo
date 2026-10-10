# k.utils.community

> Kooboo community resource sharing and search

## Overview

`k.utils.community` Upload/search/download shared packages (templates, modules, etc., distinguished by `type`) to the Kooboo community.

## shareBinary() / share()

Upload resources: `shareBinary` uses byte array; `share` uses Base64 string.

| Method | Description |
|------|------|
| `shareBinary(type, title, description, fileBytes, thumbnailBytes)` | Upload byte arrays directly |
| `share(type, title, description, binaryBase64, thumbnailBase64)` | Upload Base64 strings |

| Parameter | Type | Required | Description |
|------|------|------|------|
| `type` | `string` | Yes | Package type, such as `template` or `module`; max length is 50 characters |
| `title` | `string` | Yes | Community package title |
| `description` | `string` | No | Package description |
| `fileBytes` | `number[]` | Yes | Package bytes |
| `thumbnailBytes` | `number[]` | No | Thumbnail bytes |
| `binaryBase64` | `string` | Yes | Package Base64 string |
| `thumbnailBase64` | `string` | No | Thumbnail Base64 string |

**Returns:** `ShareResponse`; commonly used fields include `isSuccess`, `isTitleExists`, `isNotAuthorized`, `message`, and `packageId`.

```ts
const packageBytes = k.file.readBinary("packages/product-template.zip")
const coverBytes = k.file.readBinary("packages/product-template.png")

const result = k.utils.community.shareBinary(
    "template",
    "Product landing template",
    "A reusable product landing page package.",
    packageBytes,
    coverBytes
)

if (!result.isSuccess) {
    throw new Error(result.message || "Share package failed")
}

return result.packageId
```

Use `share()` when the package is already encoded as Base64:

```ts
const packageBase64 = k.security.toBase64(k.file.readBinary("packages/module.zip"))
const thumbnailBase64 = k.security.toBase64(k.file.readBinary("packages/module.png"))

const result = k.utils.community.share(
    "module",
    "Member tools",
    "Reusable member center scripts.",
    packageBase64,
    thumbnailBase64
)
```

## search()

| Overload | Description |
|------|------|
| `search(type)` | List by type |
| `search(type, keyword, skip, count)` | Keyword page search |

| Parameter | Type | Required | Description |
|------|------|------|------|
| `type` | `string` | Yes | Package type |
| `keyword` | `string` | No | Search keyword |
| `skip` | `number` | No | Number of records to skip |
| `count` | `number` | No | Number of records to return; the simple overload usually returns 30 |

**Returns:** `CommunityViewModel[]`.

```ts
const firstPage = k.utils.community.search("template", "product", 0, 20)

return firstPage.map(item => ({
    id: item.id,
    title: item.title,
    packageUrl: item.packageUrl
}))
```

## downloadFile()

Press `packageUrl` to download the package bytes.

| Parameter | Type | Required | Description |
|------|------|------|------|
| `packageUrl` | `string` | Yes | Package URL, starting with `http://`, `https://`, or `//` |

**Returns:** `number[]`, the downloaded file bytes.

```ts
const matches = k.utils.community.search("template", "product", 0, 1)
if (matches.length === 0) {
    return null
}

const bytes = k.utils.community.downloadFile(matches[0].packageUrl)
k.file.writeBinary("downloads/community-template.zip", bytes)
return k.file.get("downloads/community-template.zip")
```

## delete()

Delete community entry by Id.

| Parameter | Type | Required | Description |
|------|------|------|------|
| `id` | `string` | Yes | Community package Id |

**Returns:** `DeleteResponse`; commonly used fields include `isSuccess`, `isNotAuthorized`, `isPackageNotFound`, and `message`.

```ts
const result = k.utils.community.delete(packageId)
if (!result.isSuccess && !result.isPackageNotFound) {
    throw new Error(result.message || "Delete package failed")
}
```

## Related Docs

- [k.utils Overview](./index.md)
