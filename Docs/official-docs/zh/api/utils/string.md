# k.utils.string

> 字符串与 HTML 文本处理（`KText`）

## 概述

脚本路径为 **`k.utils.string`**（对应 `KText` 类型）。

## 方法

| 方法 | 说明 |
|------|------|
| `removeHtml(input)` | 去除 HTML 标签 |
| `subString(input, start, count)` | 截取子串 |
| `sementicSubString(input, start, count)` | 按语义边界截取 |
| `replace(input, oldValue, newValue)` | 替换 |
| `remove(input, valueToRemove)` | 删除指定子串 |

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `input` | `string` | 是 | 原始字符串 |
| `start` | `number` | 是 | 起始位置；小于 0 时按 0 处理 |
| `count` | `number` | 是 | 截取长度 |
| `oldValue` | `string` | 是 | 要替换的旧字符串，忽略大小写 |
| `newValue` | `string` | 是 | 新字符串 |
| `valueToRemove` | `string` | 是 | 要删除的字符串，忽略大小写 |

**返回：** `string`。

```ts
const plain = k.utils.string.removeHtml("<p>Hi</p>")
const part = k.utils.string.subString("hello world", 0, 5)
const semantic = k.utils.string.sementicSubString("Kooboo builds websites", 0, 12)
const normalized = k.utils.string.replace("Hello KOOboo", "kooboo", "Kooboo")
const removed = k.utils.string.remove("SKU-001-draft", "-draft")
```

## 相关文档

- [k.utils.dom](./dom.md) — HTML 文档解析
