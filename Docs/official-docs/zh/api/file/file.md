# k.file

> 文件操作

## 概述

`k.file` 提供站点内**磁盘目录**上文件的操作（读写、复制、删除等）。Kooboo 后台 **没有** 与 `k.file` 一一对应的可视化文件管理页。

::: tip 与「内容 → 文件」菜单
后台 **内容 → 文件**（`/content/files`）管理的是 **CMS 文件库（CmsFile）**，可通过站点 URL 访问；与 `k.file` **不是同一套存储**，且目前 **没有** `k.xxx` API 操作 CMS 文件库。操作说明见 [后台：文件](../../cms/content/files.md)。
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


| 参数             | 类型        | 必填  | 说明                  |
| -------------- | --------- | --- | ------------------- |
| fileName       | `string`  | 是   | 文件路径                |
| content        | `string`  | 是   | 文件内容                |
| withAuthorInfo | `boolean` | 否   | 是否包含作者信息，默认 `false` |


**返回：** `[FileInfo](./file-info.md#fileinfo-属性)`

## read()

读取文件的文本内容。

```ts
k.api.get("read", () => {
    return k.file.read("test-folder/test.txt")
})
```

**参数：**


| 参数       | 类型       | 必填  | 说明   |
| -------- | -------- | --- | ---- |
| fileName | `string` | 是   | 文件路径 |


**返回：** `string` - 文件文本内容

## append()

追加文本内容到文件。如果文件不存在则创建。

```ts
k.api.post("append", () => {
    k.file.append("test-folder/append.txt", "追加的内容")
    const content = k.file.read("test-folder/append.txt")
    return { content: content }
})
```

**参数：**


| 参数       | 类型       | 必填  | 说明    |
| -------- | -------- | --- | ----- |
| fileName | `string` | 是   | 文件路径  |
| content  | `string` | 是   | 追加的内容 |


**返回：** `void`

## exists()

检查文件是否存在。

```ts
k.api.get("exists", () => {
    return k.file.exists("test-folder/test.txt")
})
```

**参数：**


| 参数       | 类型       | 必填  | 说明   |
| -------- | -------- | --- | ---- |
| fileName | `string` | 是   | 文件路径 |


**返回：** `boolean`

## get()

获取文件信息。

```ts
k.api.get("get", () => {
    return k.file.get("test-folder/test.txt")
})
```

**参数：**


| 参数            | 类型        | 必填  | 说明                  |
| ------------- | --------- | --- | ------------------- |
| fileName      | `string`  | 是   | 文件路径                |
| includeAuthor | `boolean` | 否   | 是否包含作者信息，默认 `false` |


**返回：** `[FileInfo](./file-info.md#fileinfo-属性)`

## url()

获取文件的相对访问路径。

```ts
k.api.get("url", () => {
    return k.file.url("image.jpg")
})
```

**参数：**


| 参数       | 类型       | 必填  | 说明       |
| -------- | -------- | --- | -------- |
| fileName | `string` | 是   | 文件名或相对路径 |


**返回：** `string` - 相对 URL 路径

## rename()

重命名文件。

```ts
k.api.post("rename", () => {
    k.file.write("test-folder/old.txt", "content")
    k.file.rename("test-folder/old.txt", "test-folder/new.txt")
    const newExists = k.file.exists("test-folder/new.txt")
    const oldExists = k.file.exists("test-folder/old.txt")
    return { newExists: newExists, oldExists: oldExists }
})
```

**参数：**


| 参数      | 类型       | 必填  | 说明    |
| ------- | -------- | --- | ----- |
| oldName | `string` | 是   | 原文件路径 |
| newName | `string` | 是   | 新文件路径 |


**返回：** `void`

## copy()

复制文件或目录。

```ts
k.api.post("copy", () => {
    k.file.write("test-folder/source.txt", "content")
    k.file.copy("test-folder/source.txt", "test-folder/dest.txt")
    const destExists = k.file.exists("test-folder/dest.txt")
    return { destExists: destExists }
})
```

**参数：**


| 参数      | 类型       | 必填  | 说明   |
| ------- | -------- | --- | ---- |
| oldName | `string` | 是   | 源路径  |
| newName | `string` | 是   | 目标路径 |


**返回：** `void`

## delete()

删除文件。

```ts
k.api.post("delete", () => {
    k.file.write("test-folder/to-delete.txt", "content")
    k.file.delete("test-folder/to-delete.txt")
    const exists = k.file.exists("test-folder/to-delete.txt")
    return { deleted: !exists }
})
```

**参数：**


| 参数       | 类型       | 必填  | 说明   |
| -------- | -------- | --- | ---- |
| fileName | `string` | 是   | 文件路径 |


**返回：** `void`

## toValidPath()

将输入转换为有效的文件路径。

```ts
k.api.get("toValidPath", () => {
    return k.file.toValidPath("folder/file@name.txt")
})
```

**参数：**


| 参数    | 类型       | 必填  | 说明   |
| ----- | -------- | --- | ---- |
| input | `string` | 是   | 输入路径 |


**返回：** `string`

## 相关文档

- [k.file/folder](./file-folder.md) - 文件夹操作
- [k.file/resumable](./file-resumable.md) - 断点续传
- [k.site](../site/) - 站点管理

