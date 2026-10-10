# k.utils.community

> Kooboo 社区资源分享与搜索

## 概述

`k.utils.community` 向 Kooboo 社区上传/搜索/下载共享包（模板、模块等，由 `type` 区分）。

## shareBinary() / share()

上传资源：`shareBinary` 使用字节数组；`share` 使用 Base64 字符串。

| 方法 | 说明 |
|------|------|
| `shareBinary(type, title, description, fileBytes, thumbnailBytes)` | 直接上传字节数组 |
| `share(type, title, description, binaryBase64, thumbnailBase64)` | 上传 Base64 字符串 |

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `type` | `string` | 是 | 资源类型，如 `template`、`module`；最长 50 个字符 |
| `title` | `string` | 是 | 社区资源标题 |
| `description` | `string` | 否 | 资源说明 |
| `fileBytes` | `number[]` | 是 | 资源包字节 |
| `thumbnailBytes` | `number[]` | 否 | 缩略图字节 |
| `binaryBase64` | `string` | 是 | 资源包 Base64 字符串 |
| `thumbnailBase64` | `string` | 否 | 缩略图 Base64 字符串 |

**返回：** `ShareResponse`，常用字段包括 `isSuccess`、`isTitleExists`、`isNotAuthorized`、`message`、`packageId`。

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

使用 `share()` 时先把文件转为 Base64：

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

| 重载 | 说明 |
|------|------|
| `search(type)` | 按类型列出 |
| `search(type, keyword, skip, count)` | 关键字分页搜索 |

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `type` | `string` | 是 | 资源类型 |
| `keyword` | `string` | 否 | 搜索关键字 |
| `skip` | `number` | 否 | 跳过条数 |
| `count` | `number` | 否 | 返回条数；默认调用通常取 30 条 |

**返回：** `CommunityViewModel[]`。

```ts
const firstPage = k.utils.community.search("template", "product", 0, 20)

return firstPage.map(item => ({
    id: item.id,
    title: item.title,
    packageUrl: item.packageUrl
}))
```

## downloadFile()

按 `packageUrl` 下载包字节。

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `packageUrl` | `string` | 是 | 资源包 URL，支持 `http://`、`https://` 或 `//` 开头 |

**返回：** `number[]`，下载到的文件字节。

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

按 Id 删除社区条目。

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `id` | `string` | 是 | 社区资源 Id |

**返回：** `DeleteResponse`，常用字段包括 `isSuccess`、`isNotAuthorized`、`isPackageNotFound`、`message`。

```ts
const result = k.utils.community.delete(packageId)
if (!result.isSuccess && !result.isPackageNotFound) {
    throw new Error(result.message || "Delete package failed")
}
```

## 相关文档

- [k.utils 概述](./index.md)
