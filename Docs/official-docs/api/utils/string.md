# k.utils.string

> String and HTML text processing (`KText`)

## Overview

The script path is **`k.utils.string`** (corresponding to `KText` type).

## Methods

| Method | Description |
|------|------|
| `removeHtml(input)` | Remove HTML tags |
| `subString(input, start, count)` | Intercept substring |
| `sementicSubString(input, start, count)` | Intercept according to semantic boundaries |
| `replace(input, oldValue, newValue)` | replace |
| `remove(input, valueToRemove)` | Delete specified substring |

| Parameter | Type | Required | Description |
|------|------|------|------|
| `input` | `string` | Yes | Source string |
| `start` | `number` | Yes | Start index; values below 0 are treated as 0 |
| `count` | `number` | Yes | Length to take |
| `oldValue` | `string` | Yes | Old value to replace, case-insensitive |
| `newValue` | `string` | Yes | New value |
| `valueToRemove` | `string` | Yes | Value to remove, case-insensitive |

**Returns:** `string`.

```ts
const plain = k.utils.string.removeHtml("<p>Hi</p>")
const part = k.utils.string.subString("hello world", 0, 5)
const semantic = k.utils.string.sementicSubString("Kooboo builds websites", 0, 12)
const normalized = k.utils.string.replace("Hello KOOboo", "kooboo", "Kooboo")
const removed = k.utils.string.remove("SKU-001-draft", "-draft")
```

## Related Docs

- [k.utils.dom](./dom.md) — HTML document parsing
