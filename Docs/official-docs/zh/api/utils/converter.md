# k.utils.converter

> 调用 Kooboo 转换服务（HTML/PDF、Office、GIF 等）

## 概述

`k.utils.converter` 通过 Kooboo 云端转换服务处理文档，依赖 `UrlSetting.Converter` 服务可用。

## htmlToPdf()

将 HTML 转为 PDF 字节数组；可选 `appendTexts` 页眉页脚配置。

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `htmlcode` | `string` | 是 | 要转换的 HTML 字符串 |
| `appendTexts` | `AppendText[]` | 否 | 页眉、页脚等追加文字配置 |

**返回：** `number[]`，PDF 文件字节。

```ts
const pdf = k.utils.converter.htmlToPdf("<div>Hello</div>")
k.file.writeBinary("out/report.pdf", pdf)
```

## gifToWebp()

GIF 转 WebP，参数为 GIF 字节数组。

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `gif` | `number[]` | 是 | GIF 图片字节 |

**返回：** `number[]`，WebP 图片字节。

```ts
const gif = k.file.readBinary("media/banner.gif")
const webp = k.utils.converter.gifToWebp(gif)

k.file.writeBinary("media/banner.webp", webp)
return k.file.get("media/banner.webp")
```

## officeToHTML()

上传 Office 文件字节，返回 HTML 字符串；关联资源会同步进站点数据库。

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `officebytes` | `number[]` | 是 | Office 文件字节 |
| `filename` | `string` | 是 | 原始文件名，需要包含扩展名 |

**返回：** `string`，转换后的 HTML。

```ts
const html = k.utils.converter.officeToHTML(file.bytes, file.fileName)
```

## officeToCleanHTML()

与 `officeToHTML` 类似，输出经清理的 HTML（具体规则以运行时为准）。

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `officebytes` | `number[]` | 是 | Office 文件字节 |
| `filename` | `string` | 是 | 原始文件名，需要包含扩展名 |

**返回：** `string`，转换并清理后的 HTML。

```ts
const file = k.request.files[0]
const cleanHtml = k.utils.converter.officeToCleanHTML(file.bytes, file.fileName)

k.file.write("imports/imported-document.html", cleanHtml)
```

## 相关文档

- [k.utils.office](./office.md) — 本地 Excel 读写
- [k.file](../file/file.md)
