# kfile Guide — File operations

For image resizing, follow [Platform thumbnails](../../Frontend/images-and-files.md#prefer-platform-thumbnails).

Before implementing business upload, replacement or deletion, follow [Images and file management](../../Frontend/images-and-files.md). Examples below illustrate file primitives, not deployable upload/delete APIs: business endpoints must validate ownership and references through server-side asset IDs, never derive a deletion path directly from a client URL.

## Module navigation

Choose the relevant section based on the user's goal:

| User goal | Read |
|----------|----------|
| Read/write, copy, delete, or rename files | This page (basic file operations) |
| Folder operations (create, list, delete) | This page (folder operations) |
| Resumable upload of large files | [Resumable uploads](./resumable.md) |
| Confirm type field structures | [File Types](./types/file.md) |

## Path concepts

File operations in Kooboo use three path formats. **Do not mix them:**

| Concept | Format | Example | Use |
|------|------|------|------|
| **fullName** | Internal storage path | `resumes/file.pdf` | Input to `exists()`, `delete()`, `write()`, `read()`, `url()`, and most other functions |
| **url / relativeUrl** | Web-relative URL | `/__kb/kfile/resumes/file.pdf` | Return value of `url()`, `get().url` / `get().relativeUrl`; use it in the database or a frontend `<a href>` |
| **absoluteUrl** | Absolute URL | `https://domain/__kb/kfile/resumes/file.pdf` | `get().absoluteUrl`; use it for external sharing |

> Conversion: `url = '/__kb/kfile/' + fullName`, `fullName = url.split('/__kb/kfile/').at(-1)`

::: danger k.file and the media library (k.media) use different storage
`k.file.*` (including `read` / `readBinary` / `write` / `writeBinary`) **only** reads and writes real files under the site's **FileIO root** (`GetFileIORoot` → `…/{站点名}/__FileIO`, exposed at `/__kb/kfile/...`).

Implementation-wise, `readBinary` / `writeBinary` work like `read` / `write`: they first combine `GetFullName(fullName)` with `RootFolder`, then call `File.ReadAllBytes` / `File.WriteAllBytes` (see `FileIO.cs`). They **do not** read the `SiteDb` media library, `/media`, product images, `previewUrl`, or similar paths.

Therefore:

- Passing a media-library `filePath`, `previewUrl`, or product image URL to `k.file.readBinary(...)` usually means the file **cannot be found** on disk, so it returns `null` (or writes to the wrong location).
- For media-library binaries, use **`k.media.writeBinary(filePath, bytes, overwrite)`**; read media metadata with `k.media.get` / `folderFiles` (the content is in the library, not a FileIO file).

`readBinary` **does not** automatically strip the `/__kb/kfile/` prefix like `get`/`load`; always pass a **fullName** (such as `uploads/a.pdf`), not a complete Web URL.
:::

## write()

Writes text content to a file. An existing file is overwritten.

```ts
k.api.post("write", () => {
    k.file.write("test-folder/test.txt", "hello world")
    const content = k.file.read("test-folder/test.txt")
    return { content: content }
})
```

**Parameters:**

| Parameter | Type | Required | Description |
|------|------|------|------|
| fullName | `string` | Yes | Internal storage path, such as `resumes/file.txt` |
| content | `string` | Yes | File content |
| withAuthorInfo | `boolean` | No | Whether to include author information; defaults to `false` |

**Returns:** `FileInfo`

## read()

Reads a file's text content.

```ts
k.api.get("read", () => {
    return k.file.read("test-folder/test.txt")
})
```

**Parameter:** `fullName: string` — internal storage path

**Returns:** `string`

## readBinary()

Reads binary content from an existing file in **FileIO (`__FileIO` / `/__kb/kfile`)**. The path must be this module's **fullName**, not a media-library path or external URL.

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

**Parameter:** `fullName: string` — FileIO internal path (**do not** pass the `/__kb/kfile/` prefix; `readBinary` does not strip it automatically like `get` does)

**Returns:** `number[]`; returns `null` when the path is invalid or `File.Exists` is false (**does not throw**)

## writeBinary()

Writes a byte array to the **FileIO** directory and creates the target if it does not exist. It likewise **cannot** be used to “update an image in the media library.”

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

**Parameters:**

| Parameter | Type | Description |
|------|------|------|
| fullName | `string` | FileIO internal path (under `__FileIO` only) |
| binary | `number[]` | File bytes |

**Returns:** `FileInfo` (may be `null` on failure, empty content, or when `GetFullName` returns null)

## append()

Appends text content to a file. Creates the file if it does not exist.

```ts
k.api.post("append", () => {
    k.file.append("test-folder/append.txt", "追加的内容")
})
```

**Parameters:** `fullName: string` — internal storage path; `content: string`

**Returns:** `void`

## exists()

Checks whether a file exists.

```ts
k.api.get("exists", () => {
    return k.file.exists("test-folder/test.txt")
})
```

**Parameter:** `fullName: string` — internal storage path, **not a URL!**

**Returns:** `boolean`

## get()

Gets file information.

```ts
k.api.get("get", () => {
    return k.file.get("test-folder/test.txt")
})
```

**Parameters:** `fullName: string` or a `/__kb/kfile/...` relative URL; `includeAuthor?: boolean`

**Returns:** [FileInfo](./types/file.md)

## url()

Converts an internal storage path to a web-accessible relative URL.

```ts
k.api.get("url", () => {
    const url = k.file.url("image.jpg")
    return { url: url }
})
```

**Parameter:** `fullName: string` — internal storage path, such as `"image.jpg"` or `"folder/sub/file.pdf"`

**Returns:** `string` — web-relative URL, such as `"/__kb/kfile/image.jpg"` or `"/__kb/kfile/folder/sub/file.pdf"`

## rename()

Renames a file.

```ts
k.api.post("rename", () => {
    k.file.rename("test-folder/old.txt", "test-folder/new.txt")
})
```

**Parameters:** `oldFullName: string` — original internal storage path; `newFullName: string` — new internal storage path

**Returns:** `void`

## copy()

Copies a file or directory.

```ts
k.api.post("copy", () => {
    k.file.copy("test-folder/source.txt", "test-folder/dest.txt")
})
```

**Parameters:** `oldFullName: string` — source internal storage path; `newFullName: string` — destination internal storage path

**Returns:** `void`

## delete()

Deletes a file.

```ts
k.api.post("delete", () => {
    k.file.delete("test-folder/to-delete.txt")
})
```

**Parameter:** `fullName: string` — internal storage path, **not a URL!**

**Returns:** `void`

## toValidPath()

Converts the input to a valid file path.

```ts
k.api.get("toValidPath", () => {
    return k.file.toValidPath("folder/file@name.txt")
})
```

**Parameter:** `input: string`

**Returns:** `string`

## folderFiles()

Gets all files in the specified folder.

```ts
k.api.get("folderFiles", () => {
    const files = k.file.folderFiles("test-folder")
    return { count: files.length }
})
```

**Parameters:** `folder: string`, `options?: SearchFolderOptions`

**Returns:** `FileInfo[]`

## subFolders()

Gets all subfolders under the specified folder.

```ts
k.api.get("subFolders", () => {
    const folders = k.file.subFolders("test-folder")
    return { count: folders.length }
})
```

**Parameter:** `folder?: string` — folder path; defaults to the root directory

**Returns:** `FolderInfo[]`

## folderExists()

Checks whether a folder exists.

```ts
k.api.get("folderExists", () => {
    return k.file.folderExists("test-folder")
})
```

**Parameter:** `path: string`

**Returns:** `boolean`

## createFolder()

Creates a folder.

```ts
k.api.post("createFolder", () => {
    k.file.createFolder("new-folder")
})
```

**Parameters:** `folder: string`, `parentFolder?: string`

**Returns:** `void`

## renameFolder()

Renames a folder.

```ts
k.api.post("renameFolder", () => {
    k.file.renameFolder("old-folder", "new-folder")
})
```

**Parameters:** `oldFolder: string`, `newFolder: string`

**Returns:** `void`

## deleteFolder()

Deletes a folder and all of its child files and subfolders.

```ts
k.api.post("deleteFolder", () => {
    k.file.deleteFolder("folder-to-delete")
})
```

**Parameter:** `folder: string`

**Returns:** `void`

## getAllFiles()

Gets all files.

```ts
k.api.get("getAllFiles", () => {
    const files = k.file.getAllFiles()
    return { count: files.length }
})
```

**Parameter:** `includeAuthor?: boolean`

**Returns:** `FileInfo[]`

## UploadFile.save()

Saves an uploaded file to disk. Each file object in `k.request.files` has a `save()` method.

`file.fileName` is the original file name sent by the browser during upload (such as `"resume.pdf"`). `save()` writes to the `fullName` you provide; generate a unique name yourself when avoiding collisions.

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

**Parameter:** `fullName: string` — internal storage path, such as `"resumes/timestamp_name.pdf"`

**Returns:** `FileInfo` (same as above)

## Utility function

### getUniqueFilePath()

Common utility function for avoiding duplicate file names. If the file already exists, it automatically adds a suffix such as `(1)` or `(2)` to the file name.

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

## Typical workflow example

**Upload a file → save its URL → derive fullName from the URL and delete it**

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

## Boundaries

- Resumable uploads → [Resumable uploads](./resumable.md)
- Confirming type fields → [File Types](./types/file.md)
