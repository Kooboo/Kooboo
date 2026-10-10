# k.file.resumableUpload

仅在大文件或断点续传场景使用此 API。普通图片或文件表单应使用常规文件 API。完成文件的引用和清理请遵循[图片与文件管理](../../Frontend/images-and-files.md)。

## 运行时边界

- `create()` 分配的分片索引从 `1` 开始，不是从 `0` 开始。
- `requireChunks` 是当前尚未保存的分片列表。实现断点续传时，每次上传后都应重新读取任务。
- 运行时不会自动按 TTL 清理被放弃的任务或分片；调用方负责安排过期清理，并校验认证、任务归属、文件大小/类型和目标路径。
- `saveChunk()` 不会强制校验索引是否属于任务的预期范围；只能上传 `requireChunks` 返回的索引。
- `remove()` 的意图是删除临时任务目录，但当前实现使用 `File.Exists` 检查目录，因而可能什么也不做。在运行时实现修复前，不要把它当作可靠的清理机制。

## create(name, size, chunkSize?)

创建任务并持久化元数据。默认分块大小为 `1 MiB`；小于 `128 KiB` 的值会被提升为 `128 KiB`。不要假设请求的大小一定被接受，应使用返回任务中的 `chunkSize` 和 `requireChunks`。

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

按 ID 读取任务。元数据文件不存在时会抛出异常。

```ts
const task = k.file.resumableUpload.get(taskId)
return {
  taskId: task.id,
  requireChunks: task.requireChunks
}
```

## saveChunk(bytesOrFile, chunk)

将一个分片保存到任务临时目录。参数可以是 `number[]`/二进制字节数组，也可以是 `k.request.files` 中的上传文件对象。

大分片应使用 `multipart/form-data`，避免把二进制内容转换为 JSON 数组：

```ts
// 查询参数：taskId 和 chunk；请求体：multipart/form-data
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

对于较小或已经在内存中的数据，也可以调用 `task.saveChunk(bytes, chunkIndex)`。API 本身不会校验任务归属、分片大小或索引范围；这些校验由外层接口负责。

## build(fileName?)

只有所有预期分片都已保存时，才能构建最终文件。可选的 `fileName` 会通过站点 File I/O 解析，可能是内部相对存储路径，不只是纯文件名；省略时使用原任务名称。成功后会删除临时任务目录，并返回 `FileInfo`。

```ts
const task = k.file.resumableUpload.get(taskId)
if (task.requireChunks.length > 0) {
  return k.api.httpCode(409, { error: "upload is incomplete" })
}

const fileInfo = task.build("uploads/video.mp4")
return { url: fileInfo.relativeUrl }
```

缺少分片或目标文件操作失败时，`build()` 会抛出异常。成功后仍应校验目标路径，并将完成文件绑定到预期的业务记录。

## 类型与生命周期说明

任务对象包含 `id`、`name`、`size`、`chunkSize`、`requireChunks` 和 `timestamp`。`timestamp` 记录创建时间；此 API 不会利用它自动使任务过期。

普通上传请使用[基础文件操作](index.md)。任务对象结构和 `saveChunk` 重载见[文件类型](types/file.md)。
