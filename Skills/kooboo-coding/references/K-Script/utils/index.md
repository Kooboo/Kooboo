# k.utils

> General utility set — string/time/URI, format conversion, image processing, and more

## Overview

`k.utils` aggregates common Kooboo server-side helper capabilities. Access all submodules through `k.utils.*`.

Read this index before use, then open subdocuments as needed. **Do not invent method names**.

## Module navigation

### Basics and text

| Document | Description |
| --- | --- |
| [Basics](basics.md) | `sleep` and byte/string conversion |
| [String / time / URI / MIME](string-date-uri.md) | `string`, `date`, `uri`, `mime` |

### Media and conversion

| Document | Description |
| --- | --- |
| [Format conversion](converter.md) | HTML→PDF, GIF→WebP, Office→HTML |
| [Image processing](image.md) | Scaling, watermarking, and format conversion |
| [Screenshots](screenshot.md) | Remote URLs / site pages / HTML screenshots |

### Documents and compression

| Document | Description |
| --- | --- |
| [Office / Excel](office.md) | `.xls` / `.xlsx` read/write and export |
| [XML / DOM](xml-dom.md) | XML construction and HTML parsing |
| [Compression](compression.md) | gzip, zip |

### Integration and operations

| Document | Description |
| --- | --- |
| [SMS](sms.md) | Alibaba Cloud / Tencent Cloud / China Mobile |
| [ClientJS](client-js.md) | Injecting variables and Monaco dependency fragments into the browser |
| [Templates / inline HTML](template-inline.md) | View rendering and Page inline content |
| [CDN](cdn.md) | CDN cache switches |

### Not documented separately (check runtime as needed)

| Module | Description |
| --- | --- |
| `CRM` | `k.utils.CRM`; customer and order queries; no dedicated reference |
| `puppeteer` | Browser tasks and screenshots; may be unavailable on online servers; no dedicated reference |
| `community` | `k.utils.community`; community package sharing and search; no dedicated reference |
| `ninjible` | `k.utils.ninjible`; site import tasks; no dedicated reference |
| `sitemap` | `k.utils.sitemap`; sitemap generation; no dedicated reference |


## Usage principles

1. **Binary data** — Pass images, PDFs, and similar data as `number[]`, together with `k.file.readBinary` / `k.file.writeBinary`.
2. **k.file paths** — The write parameter is **fullName**; browsers access it with `k.file.url(fullName)` (including `/__kb/kfile/`). See the [File module](../file/index.md).
3. **Casing** — `k.utils.CDN` uses PascalCase. Network IP helpers are under `k.net.IP`.
4. **Check subdocuments first** — Use the corresponding reference as the authority for method signatures.
