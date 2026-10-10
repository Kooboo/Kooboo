# k.media

For image resizing, follow [Platform thumbnails](../../Frontend/images-and-files.md#prefer-platform-thumbnails).

When mutating media assets, follow reference and cleanup rules in [Images and file management](../../Frontend/images-and-files.md); primitive write/delete examples are not complete business endpoints.

> Site media-library management — manage site images and media folders

## Overview

`k.media` manages site media files (images), including folder operations and file CRUD.

## Data structures

```ts
interface MediaFolderView {
  id: string;
  name: string;
  fullPath: string;
}

interface MediaFileViewModel {
  id: string;
  name: string;
  filePath: string;
  height: number;
  width: number;
  size: string;
  lastModified: DateTime; // currently not populated by the runtime adapter
  previewUrl: string;
}
```

## Methods

| Method | Signature | Description |
|------|------|------|
| `folderFiles` | `folderFiles(folder): MediaFileViewModel[]` | Get all files in a folder (not paginated) |
| `subFolders` | `subFolders(folder): MediaFolderView[]` | Get the subfolder list |
| `createFolder` | `createFolder(nameFolder, parentFolder): MediaFolderView` | Create a media folder |
| `deleteFolder` | `deleteFolder(folder): void` | Delete a media folder |
| `get` | `get(idOrFilePath): MediaFileViewModel \| null` | Get one media file by GUID or path |
| `delete` | `delete(idOrFilePath): void` | Delete a media file by GUID or path |
| `exists` | `exists(filePath): boolean` | Check whether a media file exists |
| `writeBinary` | `writeBinary(filePath, binary, overwrite): boolean` | Write binary data as a media file |

## Usage examples

### List folder contents

```ts
const files = k.media.folderFiles('/images')
const folders = k.media.subFolders('/images')
```

### Create a folder

```ts
const newFolder = k.media.createFolder('products', '/images')
```

### Upload a file

```ts
const file = k.request.files[0]
k.media.writeBinary('banner.jpg', file.bytes, true)
```

### Query and delete

```ts
k.media.exists('/images/photo.jpg')

// 获取文件信息
const info = k.media.get('/images/photo.jpg')
if (info) {
    const { filePath, lastModified, previewUrl, height, size, width } = info
}

k.media.delete('/images/photo.jpg')
k.media.deleteFolder('/images/old')

```

## Best practices

```ts
// /api/media/{action}

// 列出文件夹内容
k.api.get('/folderFiles', (folder) => {
    const files = k.media.folderFiles(folder).map(item => {
        const { name, filePath, lastModified, previewUrl, size, width, height } = item
        return { name, filePath, lastModified, previewUrl, size, width, height }
    })
    const folders = k.media.subFolders(folder).map(item => {
        const { fullPath, name } = item
        return { fullPath, name }
    })
    return { code: 200, data: { files, folders } }
})

// 创建文件夹
k.api.post('/createFolder', () => {
    const { nameFolder, parentFolder } = k.request
    const res = k.media.createFolder(nameFolder, parentFolder || '')
    return { code: 200, data: res }
})

// 上传文件
k.api.post('/uploadFile', () => {
    const { parentFolder } = k.request
    const files = k.request.files
    if (files.length > 0) {
        const file = files[0]
        const newFilePath = parentFolder ? `${parentFolder}/${file.fileName}` : file.fileName
        const fileInfo = k.media.writeBinary(newFilePath, file.bytes, true)
        return { code: 200, data: fileInfo }
    }
    return { code: 400, error: 'Invalid submission' }
})

// 检查文件是否存在
k.api.post('/exists', () => {
    const { fileName } = k.request
    return { code: 200, data: k.media.exists(fileName) }
})

// 获取文件详情
k.api.get('/info', () => {
    const { file } = k.request.queryString
    const { name, filePath, lastModified, previewUrl, height, size, width } = k.media.get(file)
    return { code: 200, data: { name, filePath, lastModified, previewUrl, height, size, width } }
})

// 批量删除
k.api.delete('/list', () => {
    try {
        const { deleteFolders, deleteFiles } = JSON.parse(k.request.body)
        if (Array.isArray(deleteFolders)) {
            deleteFolders.forEach(item => k.media.deleteFolder(item))
        }
        if (Array.isArray(deleteFiles)) {
            deleteFiles.forEach(item => k.media.delete(item))
        }
        return { code: 200 }
    } catch (error) {
        return { code: 400, error: 'Bad Request' }
    }
})

// 保存图片
k.api.post('/saveImage', () => {
    const { fullName } = k.request
    const files = k.request.files
    if (files.length > 0) {
        const file = files[0]
        const fileInfo = k.media.writeBinary(fullName, file.bytes, true)
        return { code: 200, data: fileInfo }
    }
    return { code: 400, error: 'Invalid submission' }
})
```

## Notes

- `k.media` operates on the site media library (Image objects), which is a different storage system from `k.file`
- `folderFiles()` returns all files in the specified folder (not paginated)
- The `overwrite` parameter of `writeBinary()` controls whether an existing file is overwritten
