# k.media

> Folder and file management of site media library (pictures)

## Overview

Image resources (site `Image` objects) in the `k.media` management console **Media Library** support subfolder browsing, creation, binary upload, query and deletion.

| API | storage | Typical uses |
|-----|------|----------|
| **`k.media`** | Media library (image metadata + binary) | Backend materials, frontend display URL, script upload images |
| **`k.file`** | Ordinary files in the site directory | File reading and writing of text, configuration, and non-media library paths |

The path starting with **`/`** indicates the relative path in the media library (such as `/images/banner.png`); if the leading `/` is not written, it will be automatically filled in.

## Data Structures

```ts
interface MediaFolderView {
  id: string;
  name: string;
  fullPath: string;
}

interface MediaFileViewModel {
  id: string;           // Can be used with k.media.get(id)
  name: string;
  filePath: string;
  height: number;
  width: number;
  size: string;         // Human-readable size, such as "12.5 KB"
  lastModified: string; // Date and time
  previewUrl: string;   // Access URL including the site BaseUrl
}
```

## subFolders()

List the **direct subfolders** under the specified folder (not recursively).

```ts
k.api.get("subFolders", () => {
    const folders = k.media.subFolders("/images")
    return { count: folders.length, folders }
})
```

| Parameter | Type | Required | Description |
|------|------|------|------|
| `folder` | `string` | yes | Parent folder path, such as `/` or `/images` |

**Return:** `MediaFolderView[]`

## folderFiles()

List **all files** in the specified folder (including files in subdirectories, collected recursively by the server).

```ts
k.api.get("folderFiles", () => {
    const files = k.media.folderFiles("/images")
    return { count: files.length }
})
```

| Parameter | Type | Required | Description |
|------|------|------|------|
| `folder` | `string` | yes | folder path |

**Return:** `void`

**Returns:** `MediaFileViewModel[]` (in descending order of `lastModified`)

## createFolder()

Create media subfolders.

```ts
k.api.post("createFolder", () => {
    const folder = k.media.createFolder("products", "/images")
    return { fullPath: folder.fullPath, name: folder.name }
})
```

| Parameter | Type | Required | Description |
|------|------|------|------|
| `folderName` | `string` | yes | New folder name (single paragraph) |
| `parentFolder` | `string` | yes | Parent path, such as `/images` or `/` |

**Return:** `MediaFolderView`

## deleteFolder()

Delete the media folder (and the objects under it managed by the media library, the behavior is consistent with console deletion).

```ts
k.api.post("deleteFolder", () => {
    k.media.deleteFolder("/images/temp")
    return { deleted: true }
})
```

| Parameter | Type | Required | Description |
|------|------|------|------|
| `folder` | `string` | yes | folder path |

**Return:** `void`

## get()

Gets individual file information by **Media Id (GUID String)** or **File Path**; returns `null` if not present.

```ts
k.api.get("get", () => {
    const byPath = k.media.get("/images/photo.jpg")
    const byId = byPath ? k.media.get(byPath.id) : null
    return { byPath, sameId: byId && byId.id === byPath?.id }
})
```

| Parameter | Type | Required | Description |
|------|------|------|------|
| `idOrFilePath` | `string` | yes | GUID or like `/images/photo.jpg` |

**Return:** `MediaFileViewModel | null`

## exists()

Check if the media file already exists.

```ts
k.api.get("exists", () => {
    return { exists: k.media.exists("/images/photo.jpg") }
})
```

| Parameter | Type | Required | Description |
|------|------|------|------|
| `filePath` | `string` | yes | file path |

**Return:** `boolean`

## writeBinary()

Write or update media file binary content.

```ts
k.api.post("upload", () => {
    const file = k.request.files[0]
    const path = "/images/" + file.fileName
    const ok = k.media.writeBinary(path, file.bytes, true)
    return { ok, info: ok ? k.media.get(path) : null }
})
```

| Parameter | Type | Required | Description |
|------|------|------|------|
| `filePath` | `string` | yes | Target path, such as `/images/banner.png` |
| `binary` | `number[]` | yes | file byte array |
| `overwrite` | `boolean` | yes | `true` overwrites the existing file; if `false` already exists, return `false` without writing. |

**Return:** `boolean` — successful writing is `true`

## delete()

Delete media files by Id or path.

```ts
k.api.post("delete", () => {
    k.media.delete("/images/photo.jpg")
    return { existsAfter: k.media.exists("/images/photo.jpg") }
})
```

| Parameter | Type | Required | Description |
|------|------|------|------|
| `idOrFilePath` | `string` | yes | GUID or file path |

**Return:** `void`

## Example: List and upload

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
