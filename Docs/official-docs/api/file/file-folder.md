# k.file/folder

> Folder operations

## Overview

`k.file` provides folder management-related operations, including folder creation, renaming, deletion, and file list acquisition.

## folderFiles()

Get all files in the specified folder.

```ts
k.api.get("folderFiles", () => {
    const files = k.file.folderFiles("test-folder")
    return { count: files.length }
})
```

**parameter:**

| Parameter | Type | Required | Description |
|------|------|------|------|
| folder | `string` | yes | folder path |
| options | [SearchFolderOptions](./file-info.md#searchfolderoptions-property) | no | Search options |

**Return:** [FileInfo[]](./file-info.md#fileinfo-property)

## subFolders()

Get all subfolders under the specified folder.

```ts
k.api.get("subFolders", () => {
    const folders = k.file.subFolders("test-folder")
    return { count: folders.length }
})
```

**parameter:**

| Parameter | Type | Required | Description |
|------|------|------|------|
| folder | `string` | no | Folder path, default root directory |

**Return:** [FolderInfo[]](./file-info.md#folderinfo-property)

## folderExists()

Check if the folder exists.

```ts
k.api.get("folderExists", () => {
    return k.file.folderExists("test-folder")
})
```

**parameter:**

| Parameter | Type | Required | Description |
|------|------|------|------|
| path | `string` | yes | folder path |

**Return:** `boolean`

## createFolder()

Create folder.

```ts
k.api.post("createFolder", () => {
    k.file.createFolder("new-folder")
    const folders = k.file.subFolders()
    const exists = folders.some(f => f.name === "new-folder")
    return { created: exists }
})
```

**parameter:**

| Parameter | Type | Required | Description |
|------|------|------|------|
| folder | `string` | yes | folder name |
| parentFolder | `string` | no | Parent folder path |

**Return:** `void`

## renameFolder()

Rename the folder.

```ts
k.api.post("renameFolder", () => {
    k.file.createFolder("old-folder")
    k.file.renameFolder("old-folder", "new-folder")
    const folders = k.file.subFolders()
    const newExists = folders.some(f => f.name === "new-folder")
    const oldExists = folders.some(f => f.name === "old-folder")
    return { newExists: newExists, oldExists: oldExists }
})
```

**parameter:**

| Parameter | Type | Required | Description |
|------|------|------|------|
| oldFolder | `string` | yes | Original folder path |
| newFolder | `string` | yes | new folder path |

**Return:** `void`

## deleteFolder()

Delete the folder and all its sub-files and sub-folders.

```ts
k.api.post("deleteFolder", () => {
    k.file.createFolder("folder-to-delete")
    k.file.deleteFolder("folder-to-delete")
    const exists = k.file.folderExists("folder-to-delete")
    return { deleted: !exists }
})
```

**parameter:**

| Parameter | Type | Required | Description |
|------|------|------|------|
| folder | `string` | yes | folder path |

**Return:** `void`

## getAllFiles()

Get all files.

```ts
k.api.get("getAllFiles", () => {
    const files = k.file.getAllFiles()
    return { count: files.length }
})
```

**parameter:**

| Parameter | Type | Required | Description |
|------|------|------|------|
| includeAuthor | `boolean` | no | Whether to include author information, default `false` |

**Return:** [FileInfo[]](./file-info.md#fileinfo-property)

## Related Docs

- [k.file](./file.md) - core file operations
- [k.file/resumable](./file-resumable.md) - Resume download from breakpoint
- [k.site](../site/) - Site Management
