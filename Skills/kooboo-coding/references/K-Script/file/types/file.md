# kfile Types — Type definitions

## FileInfo

File information object.

| Property | Type | Description |
|------|------|------|
| name | `string` | File name only, such as `"file.pdf"` |
| fullName | `string` | Internal storage path, such as `"resumes/file.pdf"` |
| size | `number` | File size in bytes |
| stringSize | `string` | Human-readable file size, such as `"1.5 MB"` |
| url | `string` | Relative URL (same as `relativeUrl`), such as `"/__kb/kfile/resumes/file.pdf"` |
| relativeUrl | `string` | Relative URL, such as `"/__kb/kfile/resumes/file.pdf"` |
| absoluteUrl | `string` | Absolute URL, such as `"https://domain/__kb/kfile/resumes/file.pdf"` |
| lastModified | `Date` | Last-modified time |
| authorUserName | `string` | Author user name (returned only when `includeAuthor` is `true`) |

## FolderInfo

Folder information object.

| Property | Description | Type |
|------|------|------|
| name | Folder name | `string` |
| fullName | Full folder name | `string` |

## SearchFolderOptions

Folder search options.

| Property | Description | Type |
|------|------|------|
| includeAuthor | Whether to include author information | `boolean` |
| includeAllDirectories | Whether to include all subdirectories | `boolean` |
| searchPattern | Search pattern; defaults to `*.*` | `string` |

## ResumableUploadTask

Resumable upload task object.

| Property | Description | Type |
|------|------|------|
| id | Task ID | `string` |
| name | File name | `string` |
| size | File size | `number` |
| chunkSize | Effective chunk size in bytes; values below `128 KiB` are raised at creation time | `number` |
| requireChunks | Current list of unsaved chunk indexes; indexes start at `1` | `number[]` |
| timestamp | Timestamp | `number` |

### build()

Builds a file from all chunks.

```ts
const fileInfo = task.build("final-video.mp4")
```

**Parameter:** `fileName?: string` — destination file name or internal relative storage path; defaults to the original task name

**Returns:** `FileInfo`; throws if any expected chunk is missing

### saveChunk()

Saves chunk data.

```ts
task.saveChunk(chunkData, chunkIndex)
task.saveChunk(uploadedFile, chunkIndex)
```

**Parameters:**
- `bytes: number[]` — chunk data
- `file: UploadFile` — uploaded chunk from `k.request.files`
- `chunk: number` — chunk index

**Returns:** `void`

`chunk` is not range-checked by the runtime. Upload only indexes currently returned by `task.requireChunks`.
