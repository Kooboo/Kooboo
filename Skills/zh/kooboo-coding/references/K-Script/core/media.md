# k.media

图片尺寸处理遵循 [平台缩略图](../../Frontend/images-and-files.md#优先使用平台缩略图)。

修改媒体资产时遵循 [图片与文件管理](../../Frontend/images-and-files.md) 的引用与清理约定；不要将底层写入/删除示例直接作为完整业务接口。

> 站点媒体库管理 - 管理站点图片和媒体文件夹

## 概述

`k.media` 用于管理站点的媒体文件（图片），支持文件夹操作和文件 CRUD。

## 数据结构

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
  lastModified: DateTime; // 当前 runtime 适配器不会填充该字段
  previewUrl: string;
}
```

## 方法

| 方法 | 签名 | 说明 |
|------|------|------|
| `folderFiles` | `folderFiles(folder): MediaFileViewModel[]` | 获取文件夹下所有文件（不分页） |
| `subFolders` | `subFolders(folder): MediaFolderView[]` | 获取子文件夹列表 |
| `createFolder` | `createFolder(nameFolder, parentFolder): MediaFolderView` | 创建媒体文件夹 |
| `deleteFolder` | `deleteFolder(folder): void` | 删除媒体文件夹 |
| `get` | `get(idOrFilePath): MediaFileViewModel \| null` | 按 GUID 或文件路径获取单个媒体文件 |
| `delete` | `delete(idOrFilePath): void` | 按 GUID 或文件路径删除媒体文件 |
| `exists` | `exists(filePath): boolean` | 检查媒体文件是否存在 |
| `writeBinary` | `writeBinary(filePath, binary, overwrite): boolean` | 写入二进制数据作为媒体文件 |

## 使用示例

### 列出文件夹内容

```ts
const files = k.media.folderFiles('/images')
const folders = k.media.subFolders('/images')
```

### 创建文件夹

```ts
const newFolder = k.media.createFolder('products', '/images')
```

### 上传文件

```ts
const file = k.request.files[0]
k.media.writeBinary('banner.jpg', file.bytes, true)
```

### 查询与删除

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

## 最佳实践

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

## 注意事项

- `k.media` 操作的是站点媒体库（Image 对象），与 `k.file` 是不同的存储系统
- `folderFiles()` 返回指定文件夹下的所有文件（不分页）
- `writeBinary()` 的 `overwrite` 参数控制是否覆盖已有文件
