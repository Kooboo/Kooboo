# k.utils.dom

> HTML 解析与图片提取

## 概述

`k.utils.dom`（`DocumentObjectModel`）解析 HTML 并提取文本或图片信息。

## parse()

解析 HTML 为内部 `Document`，并挂到实例上。

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `html` | `string` | 是 | 要解析的 HTML 字符串 |

**返回：** `DocumentObjectModel`，可继续读取 `document`、`hasImage`、`images`。

```ts
const dom = k.utils.dom.parse("<html><body><img src=\"/a.png\" /></body></html>")
```

## getText()

从 HTML 字符串提取纯文本。

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `html` | `string` | 是 | 要提取文本的 HTML 字符串 |

**返回：** `string`。

```ts
const text = k.utils.dom.getText("<p>Hello</p>")
```

## hasImage

解析后是否包含 `<img>`（属性访问）。

**类型：** `boolean`。需先调用 `parse()` 得到实例。

```ts
const dom = k.utils.dom.parse("<p><img src=\"/logo.png\" alt=\"Logo\"></p>")
return { hasImage: dom.hasImage }
```

## images

解析后的图片列表，项含 `src`、`alt`。

**类型：** `{ src: string, alt: string }[]`。需先调用 `parse()` 得到实例。

```ts
const dom = k.utils.dom.parse("<img src=\"/logo.png\" alt=\"Logo\">")
return dom.images.map(img => ({
    src: img.src,
    alt: img.alt
}))
```

## 相关文档

- [k.utils.string](./string.md)
