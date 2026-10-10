# k.utils.dom

> HTML parsing and image extraction

## Overview

`k.utils.dom` (`DocumentObjectModel`) parses HTML and extracts text or image information.

## parse()

Parses HTML to internal `Document` and hooks it to the instance.

| Parameter | Type | Required | Description |
|------|------|------|------|
| `html` | `string` | Yes | HTML string to parse |

**Returns:** `DocumentObjectModel`; read `document`, `hasImage`, or `images` from the returned object.

```ts
const dom = k.utils.dom.parse("<html><body><img src=\"/a.png\" /></body></html>")
```

## getText()

Extract plain text from HTML string.

| Parameter | Type | Required | Description |
|------|------|------|------|
| `html` | `string` | Yes | HTML string to extract text from |

**Returns:** `string`.

```ts
const text = k.utils.dom.getText("<p>Hello</p>")
```

## hasImage

Whether `<img>` (attribute access) is included after parsing.

**Type:** `boolean`. Call `parse()` first and read it from the returned object.

```ts
const dom = k.utils.dom.parse("<p><img src=\"/logo.png\" alt=\"Logo\"></p>")
return { hasImage: dom.hasImage }
```

## images

The parsed image list contains `src` and `alt`.

**Type:** `{ src: string, alt: string }[]`. Call `parse()` first and read it from the returned object.

```ts
const dom = k.utils.dom.parse("<img src=\"/logo.png\" alt=\"Logo\">")
return dom.images.map(img => ({
    src: img.src,
    alt: img.alt
}))
```

## Related Docs

- [k.utils.string](./string.md)
