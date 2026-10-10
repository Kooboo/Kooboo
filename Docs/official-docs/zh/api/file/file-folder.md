# k.file/folder

> 文件夹操作

## 概述

`k.file` 提供文件夹管理相关操作，包括文件夹的创建、重命名、删除以及文件列表获取等。

## folderFiles()

获取指定文件夹内的所有文件。

```ts
k.api.get("folderFiles", () => {
    const files = k.file.folderFiles("test-folder")
    return { count: files.length }
})
```

**参数：**

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| folder | `string` | 是 | 文件夹路径 |
| options | [`SearchFolderOptions`](./file-info.md#searchfolderoptions-属性) | 否 | 搜索选项 |

**返回：** [`FileInfo[]`](./file-info.md#fileinfo-属性)

## subFolders()

获取指定文件夹下的所有子文件夹。

```ts
k.api.get("subFolders", () => {
    const folders = k.file.subFolders("test-folder")
    return { count: folders.length }
})
```

**参数：**

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| folder | `string` | 否 | 文件夹路径，默认根目录 |

**返回：** [`FolderInfo[]`](./file-info.md#folderinfo-属性)

## folderExists()

检查文件夹是否存在。

```ts
k.api.get("folderExists", () => {
    return k.file.folderExists("test-folder")
})
```

**参数：**

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| path | `string` | 是 | 文件夹路径 |

**返回：** `boolean`

## createFolder()

创建文件夹。

```ts
k.api.post("createFolder", () => {
    k.file.createFolder("new-folder")
    const folders = k.file.subFolders()
    const exists = folders.some(f => f.name === "new-folder")
    return { created: exists }
})
```

**参数：**

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| folder | `string` | 是 | 文件夹名称 |
| parentFolder | `string` | 否 | 父文件夹路径 |

**返回：** `void`

## renameFolder()

重命名文件夹。

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

**参数：**

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| oldFolder | `string` | 是 | 原文件夹路径 |
| newFolder | `string` | 是 | 新文件夹路径 |

**返回：** `void`

## deleteFolder()

删除文件夹及其所有子文件和子文件夹。

```ts
k.api.post("deleteFolder", () => {
    k.file.createFolder("folder-to-delete")
    k.file.deleteFolder("folder-to-delete")
    const exists = k.file.folderExists("folder-to-delete")
    return { deleted: !exists }
})
```

**参数：**

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| folder | `string` | 是 | 文件夹路径 |

**返回：** `void`

## getAllFiles()

获取所有文件。

```ts
k.api.get("getAllFiles", () => {
    const files = k.file.getAllFiles()
    return { count: files.length }
})
```

**参数：**

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| includeAuthor | `boolean` | 否 | 是否包含作者信息，默认 `false` |

**返回：** [`FileInfo[]`](./file-info.md#fileinfo-属性)

## 相关文档

- [k.file](./file.md) - 核心文件操作
- [k.file/resumable](./file-resumable.md) - 断点续传
- [k.site](../site/) - 站点管理
