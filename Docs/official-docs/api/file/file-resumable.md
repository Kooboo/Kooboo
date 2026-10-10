# k.file/resumable

> Resume upload from breakpoint

## Overview

`k.file.resumableUpload` provides the function of uploading large files in parts and supports resumed uploading at breakpoints.

## create()

Create a resumable upload task.

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

**parameter:**

| Parameter | Type | Required | Description |
|------|------|------|------|
| name | `string` | yes | file name |
| size | `number` | yes | File size (bytes) |
| chunkSize | `number` | no | Chunk size, default `1024 * 1024` |

**Return:** [ResumableUploadTask](#resumableuploadtask)

## get()

Get the resume upload task.

```ts
k.api.post("get", () => {
    const task = k.file.resumableUpload.create("video.mp4", 1024 * 1024, 256 * 1024)
    const retrieved = k.file.resumableUpload.get(task.id)
    return { id: retrieved.id, name: retrieved.name }
})
```

**parameter:**

| Parameter | Type | Required | Description |
|------|------|------|------|
| ID | `string` | yes | Task ID |

**Return:** [ResumableUploadTask](#resumableuploadtask)

## remove()

Delete the resume upload task.

```ts
k.api.post("remove", () => {
    const task = k.file.resumableUpload.create("video.mp4", 1024 * 1024, 256 * 1024)
    k.file.resumableUpload.remove(task.id)
    const retrieved = k.file.resumableUpload.get(task.id)
    return { removed: !retrieved }
})
```

**parameter:**

| Parameter | Type | Required | Description |
|------|------|------|------|
| ID | `string` | yes | Task ID |

**Return:** `void`

## ResumableUploadTask

The task object of resuming the upload after breakpoint.

### Properties

| Property | Description | Type |
|------|------|------|
| ID | Task ID | `string` |
| name | file name | `string` |
| size | file size | `number` |
| chunkSize | Chunk size | `number` |
| requireChunks | Required block index list | `number[]` |
| timestamp | Timestamp | `number` |

### build()

Build files from all chunks.

```ts
k.api.post("build", () => {
    const task = k.file.resumableUpload.get(taskId)
    const fileInfo = task.build("final-video.mp4")
    return fileInfo
})
```

**parameter:**

| Parameter | Type | Required | Description |
|------|------|------|------|
| fileName | `string` | no | New file name, the original task name is used by default |

**Return:** [FileInfo](./file-info.md#fileinfo-property)

### saveChunk()

Save chunked data.

```ts
k.api.post("saveChunk", () => {
    const task = k.file.resumableUpload.get(taskId)
    task.saveChunk(chunkData, chunkIndex)
    return { saved: true }
})
```

**parameter:**

| Parameter | Type | Required | Description |
|------|------|------|------|
| bytes | `number[]` | yes | Chunked data |
| chunk | `number` | yes | block index |

**Return:** `void`

## Related Docs

- [k.file](./file.md) - core file operations
- [k.file/folder](./file-folder.md) - Folder operations
- [k.site](../site/) - Site Management
