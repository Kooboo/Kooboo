# k.media

> 站点媒体库（图片）的文件夹与文件管理

## 概述

`k.media` 管理控制台 **媒体库** 中的图片资源（站点 `Image` 对象），支持子文件夹浏览、创建、上传二进制、查询与删除。

| API | 存储 | 典型用途 |
|-----|------|----------|
| **`k.media`** | 媒体库（图片元数据 + 二进制） | 后台素材、前台展示 URL、脚本上传图片 |
| **`k.file`** | 站点目录下的普通文件 | 文本、配置、非媒体库路径的文件读写 |

路径以 **`/`** 开头表示媒体库内相对路径（如 `/images/banner.png`）；未写前导 `/` 时会自动补上。

## 数据结构

```ts
interface MediaFolderView {
  id: string;
  name: string;
  fullPath: string;
}

interface MediaFileViewModel {
  id: string;           // 可用于 k.media.get(id)
  name: string;
  filePath: string;
  height: number;
  width: number;
  size: string;         // 可读大小，如 "12.5 KB"
  lastModified: string; // 日期时间
  previewUrl: string;   // 带站点 BaseUrl 的访问地址
}
```

## subFolders()

列出指定文件夹下的**直接子文件夹**（不递归）。

```ts
k.api.get("subFolders", () => {
    const folders = k.media.subFolders("/images")
    return { count: folders.length, folders }
})
```

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `folder` | `string` | 是 | 父文件夹路径，如 `/` 或 `/images` |

**返回：** `MediaFolderView[]`

## folderFiles()

列出指定文件夹下的**全部文件**（含子目录中的文件，由服务端递归收集）。

```ts
k.api.get("folderFiles", () => {
    const files = k.media.folderFiles("/images")
    return { count: files.length }
})
```

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `folder` | `string` | 是 | 文件夹路径 |

**返回：** `void`

**返回：** `MediaFileViewModel[]`（按 `lastModified` 降序）

## createFolder()

创建媒体子文件夹。

```ts
k.api.post("createFolder", () => {
    const folder = k.media.createFolder("products", "/images")
    return { fullPath: folder.fullPath, name: folder.name }
})
```

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `folderName` | `string` | 是 | 新文件夹名称（单段） |
| `parentFolder` | `string` | 是 | 父路径，如 `/images` 或 `/` |

**返回：** `MediaFolderView`

## deleteFolder()

删除媒体文件夹（及其下由媒体库管理的对象，行为与控制台删除一致）。

```ts
k.api.post("deleteFolder", () => {
    k.media.deleteFolder("/images/temp")
    return { deleted: true }
})
```

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `folder` | `string` | 是 | 文件夹路径 |

**返回：** `void`

## get()

按 **媒体 Id（GUID 字符串）** 或 **文件路径** 获取单个文件信息；不存在时返回 `null`。

```ts
k.api.get("get", () => {
    const byPath = k.media.get("/images/photo.jpg")
    const byId = byPath ? k.media.get(byPath.id) : null
    return { byPath, sameId: byId && byId.id === byPath?.id }
})
```

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `idOrFilePath` | `string` | 是 | GUID 或如 `/images/photo.jpg` |

**返回：** `MediaFileViewModel | null`

## exists()

检查媒体文件是否已存在。

```ts
k.api.get("exists", () => {
    return { exists: k.media.exists("/images/photo.jpg") }
})
```

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `filePath` | `string` | 是 | 文件路径 |

**返回：** `boolean`

## writeBinary()

写入或更新媒体文件二进制内容。

```ts
k.api.post("upload", () => {
    const file = k.request.files[0]
    const path = "/images/" + file.fileName
    const ok = k.media.writeBinary(path, file.bytes, true)
    return { ok, info: ok ? k.media.get(path) : null }
})
```

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `filePath` | `string` | 是 | 目标路径，如 `/images/banner.png` |
| `binary` | `number[]` | 是 | 文件字节数组 |
| `overwrite` | `boolean` | 是 | `true` 覆盖已有文件；`false` 且已存在时返回 `false` 且不写入 |

**返回：** `boolean` — 写入成功为 `true`

## delete()

按 Id 或路径删除媒体文件。

```ts
k.api.post("delete", () => {
    k.media.delete("/images/photo.jpg")
    return { existsAfter: k.media.exists("/images/photo.jpg") }
})
```

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `idOrFilePath` | `string` | 是 | GUID 或文件路径 |

**返回：** `void`

## 示例：列出并上传

```ts
k.api.get("browse", () => {
    const folder = k.request.queryString.folder || "/"
    return {
        folders: k.media.subFolders(folder),
        files: k.media.folderFiles(folder)
    }
})

k.api.post("upload", () => {
    const file = k.request.files[0]
    if (!file) {
        return { error: "no file" }
    }
    const parent = k.request.form.parentFolder || "/uploads"
    const path = parent.replace(/\/$/, "") + "/" + file.fileName
    const ok = k.media.writeBinary(path, file.bytes, true)
    return { ok, file: ok ? k.media.get(path) : null }
})
```
