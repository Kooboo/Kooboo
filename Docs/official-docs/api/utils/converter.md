# k.utils.converter

> Call Kooboo conversion services (HTML/PDF, Office, GIF, etc.)

## Overview

`k.utils.converter` processes documents through the Kooboo cloud conversion service, which relies on the `UrlSetting.Converter` service being available.

## htmlToPdf()

Convert HTML to PDF byte array; optional `appendTexts` header and footer configuration.

| Parameter | Type | Required | Description |
|------|------|------|------|
| `htmlcode` | `string` | Yes | HTML string to convert |
| `appendTexts` | `AppendText[]` | No | Header, footer, or other appended text configuration |

**Returns:** `number[]`, PDF file bytes.

```ts
const pdf = k.utils.converter.htmlToPdf("<div>Hello</div>")
k.file.writeBinary("out/report.pdf", pdf)
```

## gifToWebp()

GIF to WebP, parameter is GIF byte array.

| Parameter | Type | Required | Description |
|------|------|------|------|
| `gif` | `number[]` | Yes | GIF image bytes |

**Returns:** `number[]`, WebP image bytes.

```ts
const gif = k.file.readBinary("media/banner.gif")
const webp = k.utils.converter.gifToWebp(gif)

k.file.writeBinary("media/banner.webp", webp)
return k.file.get("media/banner.webp")
```

## officeToHTML()

Upload Office file bytes and return HTML string; associated resources will be synchronized into the site database.

| Parameter | Type | Required | Description |
|------|------|------|------|
| `officebytes` | `number[]` | Yes | Office file bytes |
| `filename` | `string` | Yes | Original file name, including extension |

**Returns:** `string`, converted HTML.

```ts
const html = k.utils.converter.officeToHTML(file.bytes, file.fileName)
```

## officeToCleanHTML()

Similar to `officeToHTML`, outputs sanitized HTML (specific rules are subject to runtime).

| Parameter | Type | Required | Description |
|------|------|------|------|
| `officebytes` | `number[]` | Yes | Office file bytes |
| `filename` | `string` | Yes | Original file name, including extension |

**Returns:** `string`, converted and cleaned HTML.

```ts
const file = k.request.files[0]
const cleanHtml = k.utils.converter.officeToCleanHTML(file.bytes, file.fileName)

k.file.write("imports/imported-document.html", cleanHtml)
```

## Related Docs

- [k.utils.office](./office.md) — Local Excel reading and writing
- [k.file](../file/file.md)
