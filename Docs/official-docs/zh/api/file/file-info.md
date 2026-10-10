# k.file - 数据结构

> 数据结构定义

## FileInfo 属性

| 属性 | 说明 | 类型 |
|------|------|------|
| name | 文件名 | `string` |
| fullName | 完整文件名 | `string` |
| size | 文件大小（字节） | `number` |
| stringSize | 文件大小（字符串格式） | `string` |
| relativeUrl | 相对 URL | `string` |
| absoluteUrl | 绝对 URL | `string` |
| url | URL | `string` |
| lastModified | 最后修改时间 | `Date` |
| authorUserName | 作者用户名 | `string` |

## FolderInfo 属性

| 属性 | 说明 | 类型 |
|------|------|------|
| name | 文件夹名称 | `string` |
| fullName | 完整文件夹名称 | `string` |

## SearchFolderOptions 属性

| 属性 | 说明 | 类型 |
|------|------|------|
| includeAuthor | 是否包含作者信息 | `boolean` |
| includeAllDirectories | 是否包含所有子目录 | `boolean` |
| searchPattern | 搜索模式，默认 `*.*` | `string` |
