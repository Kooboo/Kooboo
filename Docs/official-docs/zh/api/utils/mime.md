# k.utils.mime

> 文件名与 MIME 类型互查

## 概述

根据扩展名或 MIME 类型查询映射关系。

## getMimeMapping()

根据文件名（含扩展名）返回 MIME 类型字符串。

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `fileName` | `string` | 是 | 文件名或路径，需包含扩展名 |

**返回：** `string`。

```ts
const mime = k.utils.mime.getMimeMapping("photo.jpg")
```

## getExtensions()

根据 MIME 类型返回可能的文件扩展名列表。

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `mimeType` | `string` | 是 | MIME 类型，如 `image/jpeg` |

**返回：** `string[]`。

```ts
const exts = k.utils.mime.getExtensions("image/jpeg")
```

## 相关文档

- [k.utils 概述](./index.md)
