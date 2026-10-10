# k.file/resumable

> 断点续传

## 概述

`k.file.resumableUpload` 提供大文件分片上传功能，支持断点续传。

## create()

创建断点续传任务。

```ts
k.api.post("create", () => {
    const task = k.file.resumableUpload.create("video.mp4", 10 * 1024 * 1024, 1024 * 1024)
    return {
        id: task.id,
        name: task.name,
        size: task.size,
        requireChunks: task.requireChunks
    }
})
```

**参数：**

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| name | `string` | 是 | 文件名 |
| size | `number` | 是 | 文件大小（字节） |
| chunkSize | `number` | 否 | 分块大小，默认 `1024 * 1024` |

**返回：** [`ResumableUploadTask`](#resumableuploadtask-属性)

## get()

获取断点续传任务。

```ts
k.api.post("get", () => {
    const task = k.file.resumableUpload.create("video.mp4", 1024 * 1024, 256 * 1024)
    const retrieved = k.file.resumableUpload.get(task.id)
    return { id: retrieved.id, name: retrieved.name }
})
```

**参数：**

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| id | `string` | 是 | 任务 ID |

**返回：** [`ResumableUploadTask`](#resumableuploadtask-属性)

## remove()

删除断点续传任务。

```ts
k.api.post("remove", () => {
    const task = k.file.resumableUpload.create("video.mp4", 1024 * 1024, 256 * 1024)
    k.file.resumableUpload.remove(task.id)
    const retrieved = k.file.resumableUpload.get(task.id)
    return { removed: !retrieved }
})
```

**参数：**

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| id | `string` | 是 | 任务 ID |

**返回：** `void`

## ResumableUploadTask

断点续传任务对象。

### 属性

| 属性 | 说明 | 类型 |
|------|------|------|
| id | 任务 ID | `string` |
| name | 文件名 | `string` |
| size | 文件大小 | `number` |
| chunkSize | 分块大小 | `number` |
| requireChunks | 需要的分块索引列表 | `number[]` |
| timestamp | 时间戳 | `number` |

### build()

从所有分块构建文件。

```ts
k.api.post("build", () => {
    const task = k.file.resumableUpload.get(taskId)
    const fileInfo = task.build("final-video.mp4")
    return fileInfo
})
```

**参数：**

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| fileName | `string` | 否 | 新文件名，默认使用原任务名 |

**返回：** [`FileInfo`](./file-info.md#fileinfo-属性)

### saveChunk()

保存分块数据。

```ts
k.api.post("saveChunk", () => {
    const task = k.file.resumableUpload.get(taskId)
    task.saveChunk(chunkData, chunkIndex)
    return { saved: true }
})
```

**参数：**

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| bytes | `number[]` | 是 | 分块数据 |
| chunk | `number` | 是 | 分块索引 |

**返回：** `void`

## 相关文档

- [k.file](./file.md) - 核心文件操作
- [k.file/folder](./file-folder.md) - 文件夹操作
- [k.site](../site/) - 站点管理
