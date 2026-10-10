# k.file - data structure

> Data structure definition

## FileInfo Property

| Property | Description | Type |
|------|------|------|
| name | file name | `string` |
| fullName | full file name | `string` |
| size | File size (bytes) | `number` |
| stringSize | File size (string format) | `string` |
| relativeUrl | Relative URL | `string` |
| absoluteUrl | Absolute URL | `string` |
| url | URL | `string` |
| lastModified | last modified time | `Date` |
| authorUserName | Author username | `string` |

## FolderInfo Property

| Property | Description | Type |
|------|------|------|
| name | folder name | `string` |
| fullName | Full folder name | `string` |

## SearchFolderOptions Property

| Property | Description | Type |
|------|------|------|
| includeAuthor | Whether to include author information | `boolean` |
| includeAllDirectories | Whether to include all subdirectories | `boolean` |
| searchPattern | Search mode, default `*.*` | `string` |
