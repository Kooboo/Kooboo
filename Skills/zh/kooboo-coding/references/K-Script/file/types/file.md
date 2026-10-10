# kfile Types 类型定义

## FileInfo

文件信息对象。

| 属性 | 类型 | 说明 |
|------|------|------|
| name | `string` | 纯文件名，如 `"file.pdf"` |
| fullName | `string` | 内部存储路径，如 `"resumes/file.pdf"` |
| size | `number` | 文件大小（字节） |
| stringSize | `string` | 文件大小（可读格式），如 `"1.5 MB"` |
| url | `string` | 相对 URL（同 `relativeUrl`），如 `"/__kb/kfile/resumes/file.pdf"` |
| relativeUrl | `string` | 相对 URL，如 `"/__kb/kfile/resumes/file.pdf"` |
| absoluteUrl | `string` | 完整 URL，如 `"https://domain/__kb/kfile/resumes/file.pdf"` |
| lastModified | `Date` | 最后修改时间 |
| authorUserName | `string` | 作者用户名（仅当 `includeAuthor` 为 `true` 时返回） |

## FolderInfo

文件夹信息对象。

| 属性 | 说明 | 类型 |
|------|------|------|
| name | 文件夹名称 | `string` |
| fullName | 完整文件夹名称 | `string` |

## SearchFolderOptions

文件夹搜索选项。

| 属性 | 说明 | 类型 |
|------|------|------|
| includeAuthor | 是否包含作者信息 | `boolean` |
| includeAllDirectories | 是否包含所有子目录 | `boolean` |
| searchPattern | 搜索模式，默认 `*.*` | `string` |

## ResumableUploadTask

断点续传任务对象。

| 属性 | 说明 | 类型 |
|------|------|------|
| id | 任务 ID | `string` |
| name | 文件名 | `string` |
| size | 文件大小 | `number` |
| chunkSize | 实际分块大小（字节）；创建时小于 `128 KiB` 的值会被提升 | `number` |
| requireChunks | 当前尚未保存的分块索引列表；索引从 `1` 开始 | `number[]` |
| timestamp | 时间戳 | `number` |

### build()

从所有分块构建文件。

```ts
const fileInfo = task.build("final-video.mp4")
```

**参数：** `fileName?: string` - 目标文件名或内部相对存储路径，默认使用原任务名

**返回：** `FileInfo`；缺少预期分块时会抛出异常

### saveChunk()

保存分块数据。

```ts
task.saveChunk(chunkData, chunkIndex)
task.saveChunk(uploadedFile, chunkIndex)
```

**参数：**
- `bytes: number[]` - 分块数据
- `file: UploadFile` - `k.request.files` 中的上传分块
- `chunk: number` - 分块索引

**返回：** `void`

运行时不会校验 `chunk` 是否在有效范围内；只能上传 `task.requireChunks` 当前返回的索引。
