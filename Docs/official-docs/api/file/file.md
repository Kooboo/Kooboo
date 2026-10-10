# k.file

> File operations

## Overview

`k.file` provides file operations (read, write, copy, delete, etc.) on the **disk directory** within the site. The Kooboo backend **does not** have a visual file management page that corresponds to `k.file` one-to-one.

::: tip and "Content → File" menu
The background **Content -> File** (`/content/files`) manages the **CMS file library (CmsFile)**, which can be accessed through the site URL; it is **not** the same storage set as `k.file`, and there is currently **no** `k.xxx` API to operate the CMS file library. See [Admin: Files](../../cms/content/files.md) for operating instructions.
:::

## write()

Write text content to a file. Overwrites the file if it already exists.

```ts
k.api.post("write", () => {
    k.file.write("test-folder/test.txt", "hello world")
    const content = k.file.read("test-folder/test.txt")
    return { content: content }
})
```

**parameter:**


| parameter             | type        | Required  | illustrate                  |
| --------------- | ---------- | --- | ------------------- |
| fileName       | `string`  | yes   | file path                |
| content        | `string`  | yes   | File content                |
| withAuthorInfo | `boolean` | no   | Whether to include author information, default `false` |


**Return:** `[FileInfo](./file-info.md#fileinfo-property)`

## read()

Read the text content of the file.

```ts
k.api.get("read", () => {
    return k.file.read("test-folder/test.txt")
})
```

**parameter:**


| parameter       | type       | Required  | illustrate   |
| -------- | -------- | --- | ---- |
| fileName | `string` | yes   | file path |


**Returns:** `string` - file text content

## append()

Appends text content to a file. Create the file if it does not exist.

```ts
k.api.post("append", () => {
    k.file.append("test-folder/append.txt", "appended content")
    const content = k.file.read("test-folder/append.txt")
    return { content: content }
})
```

**parameter:**


| parameter       | type       | Required  | illustrate    |
| -------- | -------- | --- | ----- |
| fileName | `string` | yes   | file path  |
| content  | `string` | yes   | Additional content |


**Return:** `void`

## exists()

Check if the file exists.

```ts
k.api.get("exists", () => {
    return k.file.exists("test-folder/test.txt")
})
```

**parameter:**


| parameter       | type       | Required  | illustrate   |
| -------- | -------- | --- | ---- |
| fileName | `string` | yes   | file path |


**Return:** `boolean`

## get()

Get file information.

```ts
k.api.get("get", () => {
    return k.file.get("test-folder/test.txt")
})
```

**parameter:**


| parameter            | type        | Required  | illustrate                  |
| ------------- | ---------- | --- | ------------------- |
| fileName      | `string`  | yes   | file path                |
| includeAuthor | `boolean` | no   | Whether to include author information, default `false` |


**Return:** `[FileInfo](./file-info.md#fileinfo-property)`

## url()

Get the relative access path of the file.

```ts
k.api.get("url", () => {
    return k.file.url("image.jpg")
})
```

**parameter:**


| parameter       | type       | Required  | illustrate       |
| -------- | -------- | --- | -------- |
| fileName | `string` | yes   | File name or relative path |


**Returns:** `string` - Relative URL path

## rename()

Rename the file.

```ts
k.api.post("rename", () => {
    k.file.write("test-folder/old.txt", "content")
    k.file.rename("test-folder/old.txt", "test-folder/new.txt")
    const newExists = k.file.exists("test-folder/new.txt")
    const oldExists = k.file.exists("test-folder/old.txt")
    return { newExists: newExists, oldExists: oldExists }
})
```

**parameter:**


| parameter      | type       | Required  | illustrate    |
| ------- | -------- | --- | ----- |
| oldName | `string` | yes   | Original file path |
| newName | `string` | yes   | new file path |


**Return:** `void`

## copy()

Copy a file or directory.

```ts
k.api.post("copy", () => {
    k.file.write("test-folder/source.txt", "content")
    k.file.copy("test-folder/source.txt", "test-folder/dest.txt")
    const destExists = k.file.exists("test-folder/dest.txt")
    return { destExists: destExists }
})
```

**parameter:**


| parameter      | type       | Required  | illustrate   |
| ------- | -------- | --- | ---- |
| oldName | `string` | yes   | source path  |
| newName | `string` | yes   | target path |


**Return:** `void`

## delete()

Delete files.

```ts
k.api.post("delete", () => {
    k.file.write("test-folder/to-delete.txt", "content")
    k.file.delete("test-folder/to-delete.txt")
    const exists = k.file.exists("test-folder/to-delete.txt")
    return { deleted: !exists }
})
```

**parameter:**


| parameter       | type       | Required  | illustrate   |
| -------- | -------- | --- | ---- |
| fileName | `string` | yes   | file path |


**Return:** `void`

## toValidPath()

Convert input to a valid file path.

```ts
k.api.get("toValidPath", () => {
    return k.file.toValidPath("folder/file@name.txt")
})
```

**parameter:**


| parameter    | type       | Required  | illustrate   |
| ----- | -------- | --- | ---- |
| input | `string` | yes   | Enter path |


**Return:** `string`

## Related Docs

- [k.file/folder](./file-folder.md) - Folder operations
- [k.file/resumable](./file-resumable.md) - Resume download from breakpoint
- [k.site](../site/) - Site Management
