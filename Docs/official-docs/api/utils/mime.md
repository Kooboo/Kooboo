# k.utils.mime

> Check file name and MIME type

## Overview

Query mappings based on extension or MIME type.

## getMimeMapping()

Returns a MIME type string based on the filename (including extension).

| Parameter | Type | Required | Description |
|------|------|------|------|
| `fileName` | `string` | Yes | File name or path, including extension |

**Returns:** `string`.

```ts
const mime = k.utils.mime.getMimeMapping("photo.jpg")
```

## getExtensions()

Returns a list of possible file extensions based on MIME type.

| Parameter | Type | Required | Description |
|------|------|------|------|
| `mimeType` | `string` | Yes | MIME type, such as `image/jpeg` |

**Returns:** `string[]`.

```ts
const exts = k.utils.mime.getExtensions("image/jpeg")
```

## Related Docs

- [k.utils Overview](./index.md)
