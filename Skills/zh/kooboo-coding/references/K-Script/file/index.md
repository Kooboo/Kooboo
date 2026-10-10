# kfile Guide 文件操作

图片尺寸处理遵循 [平台缩略图](../../Frontend/images-and-files.md#优先使用平台缩略图)。

实现上传、替换或删除等业务文件操作前遵循 [图片与文件管理](../../Frontend/images-and-files.md)。下文是底层文件操作示例，不是可直接发布的上传/删除 API；业务接口必须按服务端 asset ID 校验归属和引用，不得从客户端 URL 推导路径后直接删除。

## 模块导航

根据用户目标，选择对应的内容：

| 用户目标 | 查看内容 |
|----------|----------|
| 文件读写、复制、删除、重命名 | 当前页面（基础文件操作） |
| 文件夹操作（创建、列表、删除） | 当前页面（文件夹操作） |
| 断点续传大文件上传 | [断点续传](resumable.md) |
| 确认类型字段结构 | [File Types](types/file.md) |

## 路径概念

Kooboo 中文件操作涉及三种路径格式，**请勿混用**：

| 概念 | 格式 | 示例 | 用途 |
|------|------|------|------|
| **fullName** | 内部存储路径 | `resumes/file.pdf` | `exists()` `delete()` `write()` `read()` `url()` 等大多数函数的输入参数 |
| **url / relativeUrl** | Web 相对 URL | `/__kb/kfile/resumes/file.pdf` | `url()` 的返回值、`get().url` / `get().relativeUrl`，用于存入数据库或前端 `<a href>` |
| **absoluteUrl** | 完整 URL | `https://domain/__kb/kfile/resumes/file.pdf` | `get().absoluteUrl`，用于外部分享 |

> 转换关系：`url = '/__kb/kfile/' + fullName`，`fullName = url.split('/__kb/kfile/').at(-1)`

::: danger k.file 与媒体库（k.media）不是同一套存储
`k.file.*`（含 `read` / `readBinary` / `write` / `writeBinary`）**只**读写站点 **FileIO 根目录**下的真实磁盘文件（`GetFileIORoot` → `…/{站点名}/__FileIO`，对外 URL 为 `/__kb/kfile/...`）。

实现上 `readBinary` / `writeBinary` 与 `read` / `write` 相同：先 `GetFullName(fullName)` 拼到 `RootFolder`，再 `File.ReadAllBytes` / `File.WriteAllBytes`（见 `FileIO.cs`）。**不会**去读 `SiteDb` 媒体库、`/media`、商品图、`previewUrl` 等路径。

因此：

- 把媒体库里的 `filePath`、`previewUrl`、商品主图 URL 传给 `k.file.readBinary(...)` → 通常在磁盘上**找不到文件**，返回 `null`（或写入到错误位置）。
- 媒体库二进制请用 **`k.media.writeBinary(filePath, bytes, overwrite)`**；读取媒体元数据用 `k.media.get` / `folderFiles`（内容在库中，不是 FileIO 文件）。

`readBinary` **不会**像 `get`/`load` 那样自动剥掉 `/__kb/kfile/` 前缀；请始终传 **fullName**（如 `uploads/a.pdf`），不要传完整 Web URL。
:::

## write()

写入文本内容到文件。如果文件已存在则覆盖。

```ts
k.api.post("write", () => {
    k.file.write("test-folder/test.txt", "hello world")
    const content = k.file.read("test-folder/test.txt")
    return { content: content }
})
```

**参数：**

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| fullName | `string` | 是 | 内部存储路径，如 `resumes/file.txt` |
| content | `string` | 是 | 文件内容 |
| withAuthorInfo | `boolean` | 否 | 是否包含作者信息，默认 `false` |

**返回：** `FileInfo`

## read()

读取文件的文本内容。

```ts
k.api.get("read", () => {
    return k.file.read("test-folder/test.txt")
})
```

**参数：** `fullName: string` — 内部存储路径

**返回：** `string`

## readBinary()

读取 **FileIO（`__FileIO` / `/__kb/kfile`）** 中已有文件的二进制内容。路径必须是本模块的 **fullName**，不能是媒体库路径或外链 URL。

```ts
k.api.get("read-binary", () => {
    // ✓ 文件此前已通过 k.file.writeBinary 或上传保存到 FileIO
    return k.file.readBinary("uploads/report.pdf")
})
```

```ts
// ✗ 错误：媒体库 previewUrl / 商品图 URL 不在 __FileIO 磁盘目录
// k.file.readBinary(product.imageUrl)  // → 多为 null
```

**参数：** `fullName: string` — FileIO 内部路径（**不要**传 `/__kb/kfile/` 前缀；`readBinary` 不会像 `get` 那样自动剥离）

**返回：** `number[]`；路径非法或 `File.Exists` 为 false 时返回 `null`（**不抛错**）

## writeBinary()

将字节数组写入 **FileIO** 目录；目标不存在时创建。同样**不能**用来“更新媒体库里的图片”。

```ts
k.api.post("write-binary", () => {
    if (k.request.files.length > 0) {
        const file = k.request.files[0]
        return k.file.writeBinary("uploads/" + file.fileName, file.bytes)
    }
})
```

```ts
// 媒体库图片 → 用 k.media.writeBinary(filePath, file.bytes, true)
```

**参数：**

| 参数 | 类型 | 说明 |
|------|------|------|
| fullName | `string` | FileIO 内部路径（仅 `__FileIO` 下） |
| binary | `number[]` | 文件字节 |

**返回：** `FileInfo`（失败、空内容或 `GetFullName` 返回 null 时可能为 `null`）

## append()

追加文本内容到文件。如果文件不存在则创建。

```ts
k.api.post("append", () => {
    k.file.append("test-folder/append.txt", "追加的内容")
})
```

**参数：** `fullName: string` — 内部存储路径, `content: string`

**返回：** `void`

## exists()

检查文件是否存在。

```ts
k.api.get("exists", () => {
    return k.file.exists("test-folder/test.txt")
})
```

**参数：** `fullName: string` — 内部存储路径，**不是 URL！**

**返回：** `boolean`

## get()

获取文件信息。

```ts
k.api.get("get", () => {
    return k.file.get("test-folder/test.txt")
})
```

**参数：** `fullName: string` 或 `/__kb/kfile/...` 相对 URL，`includeAuthor?: boolean`

**返回：** [FileInfo](types/file.md)

## url()

将内部存储路径转换为 Web 可访问的相对 URL。

```ts
k.api.get("url", () => {
    const url = k.file.url("image.jpg")
    return { url: url }
})
```

**参数：** `fullName: string` — 内部存储路径，如 `"image.jpg"` 或 `"folder/sub/file.pdf"`

**返回：** `string` — Web 相对 URL，如 `"/__kb/kfile/image.jpg"` 或 `"/__kb/kfile/folder/sub/file.pdf"`

## rename()

重命名文件。

```ts
k.api.post("rename", () => {
    k.file.rename("test-folder/old.txt", "test-folder/new.txt")
})
```

**参数：** `oldFullName: string` — 原内部存储路径, `newFullName: string` — 新内部存储路径

**返回：** `void`

## copy()

复制文件或目录。

```ts
k.api.post("copy", () => {
    k.file.copy("test-folder/source.txt", "test-folder/dest.txt")
})
```

**参数：** `oldFullName: string` — 源内部存储路径, `newFullName: string` — 目标内部存储路径

**返回：** `void`

## delete()

删除文件。

```ts
k.api.post("delete", () => {
    k.file.delete("test-folder/to-delete.txt")
})
```

**参数：** `fullName: string` — 内部存储路径，**不是 URL！**

**返回：** `void`

## toValidPath()

将输入转换为有效的文件路径。

```ts
k.api.get("toValidPath", () => {
    return k.file.toValidPath("folder/file@name.txt")
})
```

**参数：** `input: string`

**返回：** `string`

## folderFiles()

获取指定文件夹内的所有文件。

```ts
k.api.get("folderFiles", () => {
    const files = k.file.folderFiles("test-folder")
    return { count: files.length }
})
```

**参数：** `folder: string`, `options?: SearchFolderOptions`

**返回：** `FileInfo[]`

## subFolders()

获取指定文件夹下的所有子文件夹。

```ts
k.api.get("subFolders", () => {
    const folders = k.file.subFolders("test-folder")
    return { count: folders.length }
})
```

**参数：** `folder?: string` — 文件夹路径，默认根目录

**返回：** `FolderInfo[]`

## folderExists()

检查文件夹是否存在。

```ts
k.api.get("folderExists", () => {
    return k.file.folderExists("test-folder")
})
```

**参数：** `path: string`

**返回：** `boolean`

## createFolder()

创建文件夹。

```ts
k.api.post("createFolder", () => {
    k.file.createFolder("new-folder")
})
```

**参数：** `folder: string`, `parentFolder?: string`

**返回：** `void`

## renameFolder()

重命名文件夹。

```ts
k.api.post("renameFolder", () => {
    k.file.renameFolder("old-folder", "new-folder")
})
```

**参数：** `oldFolder: string`, `newFolder: string`

**返回：** `void`

## deleteFolder()

删除文件夹及其所有子文件和子文件夹。

```ts
k.api.post("deleteFolder", () => {
    k.file.deleteFolder("folder-to-delete")
})
```

**参数：** `folder: string`

**返回：** `void`

## getAllFiles()

获取所有文件。

```ts
k.api.get("getAllFiles", () => {
    const files = k.file.getAllFiles()
    return { count: files.length }
})
```

**参数：** `includeAuthor?: boolean`

**返回：** `FileInfo[]`

## UploadFile.save()

保存上传的文件到磁盘。`k.request.files` 中的每个文件对象都有 `save()` 方法。

`file.fileName` 为上传时浏览器传入的原始文件名（如 `"resume.pdf"`）。`save()` 会写入调用方传入的 `fullName`；如需避免重名，必须由调用方自行生成唯一文件名。

```ts
k.api.post("upload", () => {
    const file = k.request.files[0]

    // 方式一：指定固定文件名（不保留原文件名）
    const saved1 = file.save("products/cover.jpg")
    // saved1.relativeUrl → "/__kb/kfile/products/cover.jpg"
    // saved1.fullName    → "products/cover.jpg"

    // 方式二：组合时间戳 + 原始文件名，避免冲突且保留原文件名
    const timestamp = new Date().getTime()
    const saved2 = file.save('resumes/' + timestamp + '_' + file.fileName)
    // saved2.fullName    → "resumes/1734567890_resume.pdf"
    // saved2.relativeUrl → "/__kb/kfile/resumes/1734567890_resume.pdf"

    return { url: saved2.relativeUrl }
})
```

**参数：** `fullName: string` — 内部存储路径，如 `"resumes/timestamp_name.pdf"`

**返回：** `FileInfo`（同上）

## 实用函数

### getUniqueFilePath()

避免文件重名冲突的常用工具函数。如果文件已存在，则在文件名后自动添加 `(1)`、`(2)` 等序号。

```ts
/**
 * 获取不重复的文件路径，如果文件已存在则自动添加序号
 * @param {string} path 目录路径
 * @param {string} filename 文件名
 * @returns {string} 不冲突的完整文件路径
 */
function getUniqueFilePath(path, filename) {
    if (k.file.exists(`${path}/${filename}`)) {
        const nameParts = filename.split('.')
        if (nameParts.length > 1) {
            const lastPartIndex = nameParts.length - 2
            let baseName = nameParts[lastPartIndex]
            // 检查是否已有序号
            if (/\([0-9]+\)$/.test(baseName)) {
                nameParts[lastPartIndex] = baseName.replace(
                    /\(([0-9]+)\)$/,
                    (match, num) => `(${Number(num) + 1})`
                )
            } else {
                nameParts[lastPartIndex] = baseName + '(1)'
            }
        } else {
            nameParts[0] = nameParts[0] + '(1)'
        }
        const newFileName = nameParts.join('.')
        return getUniqueFilePath(path, newFileName)
    } else {
        return `${path}/${filename}`
    }
}

// 使用示例：同一文件重复上传时自动避免覆盖
const file = k.request.files[0]
const uniquePath = getUniqueFilePath('products', file.fileName)
// 第一次：  uniquePath → "products/resume.pdf"
// 第二次：  uniquePath → "products/resume(1).pdf"
// 第三次：  uniquePath → "products/resume(2).pdf"
const saved = file.save(uniquePath)
```

## 典型流程示例

**文件上传 → 保存 URL → 从 URL 反解 fullName 后删除**

```ts
// 1. 接收上传文件，使用 getUniqueFilePath 避免重名冲突
const file = k.request.files[0]
const uniquePath = getUniqueFilePath('products', file.fileName)
const saved = file.save(uniquePath)
const url = saved.relativeUrl  // "/__kb/kfile/products/resume.pdf"

// 2. 存入内容库
k.content.Product.add({ coverUrl: url })

// 3. 从 URL 反解 fullName 后删除
const fullName = url.split('/__kb/kfile/').at(-1)
if (k.file.exists(fullName)) {
    k.file.delete(fullName)
}
```

## 边界说明

- 断点续传 → [断点续传](resumable.md)
- 类型字段确认 → [File Types](types/file.md)
