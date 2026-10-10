# k.file.resumableUpload

Use this API only for large-file or resumable uploads. Ordinary image/file forms should use the regular file APIs. For completed-file references and cleanup, follow [Images and file management](../../Frontend/images-and-files.md).

## Runtime boundaries

- `create()` assigns chunk indexes starting at `1`, not `0`.
- `requireChunks` is the current list of chunks that have not been saved. Re-read the task after each upload when implementing resume logic.
- The runtime does not provide TTL cleanup for abandoned tasks or chunks. The caller must arrange expiry cleanup and must validate authentication, task ownership, file size/type, and the destination path.
- `saveChunk()` accepts arbitrary indexes and does not enforce the task's expected range. Upload only indexes returned by `requireChunks`.
- `remove()` is intended to delete the temporary task directory, but the current implementation checks a directory with `File.Exists`; it may therefore be a no-op. Do not treat it as reliable cleanup until the runtime implementation is fixed.

## create(name, size, chunkSize?)

Creates a task and persists its metadata. The default chunk size is `1 MiB`; values below `128 KiB` are raised to `128 KiB`. Use the returned `chunkSize` and `requireChunks` rather than assuming the requested value was accepted.

```ts
const task = k.file.resumableUpload.create(
  "video.mp4",
  10 * 1024 * 1024,
  1024 * 1024
)
return {
  taskId: task.id,
  chunkSize: task.chunkSize,
  requireChunks: task.requireChunks
}
```

## get(id)

Loads a task by ID. It throws when the metadata file does not exist.

```ts
const task = k.file.resumableUpload.get(taskId)
return {
  taskId: task.id,
  requireChunks: task.requireChunks
}
```

## saveChunk(bytesOrFile, chunk)

Saves one chunk to the task's temporary directory. It accepts either a `number[]`/binary byte array or an uploaded file object from `k.request.files`.

Use `multipart/form-data` for large chunks so the binary data is not converted into a JSON array:

```ts
// Query parameters: taskId and chunk; body: multipart/form-data
const taskId = k.request.queryString.get("taskId")
const chunkIndex = Number(k.request.queryString.get("chunk"))
const file = k.request.files[0]

if (!taskId || !Number.isInteger(chunkIndex) || !file) {
  return k.api.badRequest()
}

const task = k.file.resumableUpload.get(taskId)
if (!task.requireChunks.includes(chunkIndex)) {
  return k.api.httpCode(400, { error: "unexpected chunk" })
}

task.saveChunk(file, chunkIndex)
return { saved: true, requireChunks: task.requireChunks }
```

For small or already-buffered data, `task.saveChunk(bytes, chunkIndex)` is also available. The API does not itself verify task ownership, chunk size, or index range; the surrounding endpoint must do so.

## build(fileName?)

Builds the final file only when every expected chunk has been saved. The optional `fileName` is resolved through site File I/O and may be an internal relative storage path, not only a bare file name. If omitted, the original task name is used. On success, the temporary task directory is deleted and the method returns `FileInfo`.

```ts
const task = k.file.resumableUpload.get(taskId)
if (task.requireChunks.length > 0) {
  return k.api.httpCode(409, { error: "upload is incomplete" })
}

const fileInfo = task.build("uploads/video.mp4")
return { url: fileInfo.relativeUrl }
```

`build()` throws when chunks are missing or the destination operation fails. Validate the destination path and bind the completed file to the intended business record after success.

## Type and lifecycle notes

The task exposes `id`, `name`, `size`, `chunkSize`, `requireChunks`, and `timestamp`. `timestamp` records creation time; this API does not use it to expire tasks automatically.

For regular uploads, use [Basic file operations](index.md). For the task object shape and `saveChunk` overloads, see [File Types](types/file.md).
