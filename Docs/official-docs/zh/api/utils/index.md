# k.utils

> KScript 工具集：按子模块划分的辅助 API

## 概述

`k.utils` 聚合站点脚本常用的工具方法与子对象。除下文列出的**根方法**外，每个子模块对应独立文档页（路径与脚本属性名一致，便于按字母检索）。

## 根方法

| 方法 | 说明 |
|------|------|
| `sleep(ms)` | 阻塞当前线程指定毫秒 |
| `bytesToString(bytes, encoding)` | 字节数组按编码转字符串 |
| `stringToBytes(content, encoding)` | 字符串按编码转字节数组 |

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `ms` | `number` | 是 | 暂停毫秒数 |
| `bytes` | `number[]` | 是 | 要转换的字节数组 |
| `content` | `string` | 是 | 要转换的文本 |
| `encoding` | `string` | 是 | 字符编码，如 `utf-8` |

| 方法 | 返回 |
|------|------|
| `sleep(ms)` | `void` |
| `bytesToString(bytes, encoding)` | `string` |
| `stringToBytes(content, encoding)` | `number[]` |

```ts
k.utils.sleep(100)
```

编码转换常用于处理远程下载、压缩包或文本文件的字节数组：

```ts
const bytes = k.utils.stringToBytes("Hello Kooboo", "utf-8")
k.file.writeBinary("exports/hello.txt", bytes)

const saved = k.file.readBinary("exports/hello.txt")
const text = k.utils.bytesToString(saved, "utf-8")

return { text }
```

## 子模块（按字母序）

| 子模块 | 文档 |
|--------|------|
| `CDN` | [cdn](./cdn.md) |
| `clientJS` | [clientJS](./clientJS.md) |
| `community` | [community](./community.md) |
| `compression` | [compression](./compression.md) |
| `converter` | [converter](./converter.md) |
| `CRM` | [crm](./crm.md) |
| `date` | [date](./date.md) |
| `dom` | [dom](./dom.md) |
| `image` | [image](./image.md) |
| `inlineHtml` | [inlineHtml](./inlineHtml.md) |
| `mime` | [mime](./mime.md) |
| `ninjible` | [ninjible](./ninjible.md) |
| `office` | [office](./office.md) |
| `puppeteer` | [puppeteer](./puppeteer.md) |
| `screenShot` | [screenShot](./screenShot.md) |
| `sitemap` | [sitemap](./sitemap.md) |
| `sms` | [sms](./sms.md) |
| `string` | [string](./string.md) |
| `template` | [template](./template.md) |
| `uri` | [uri](./uri.md) |
| `xml` | [xml](./xml.md) |

## 相关文档

- [k.net](../net/) — IP、Ping、SSH、SFTP 与 Telnet
- [k.google](../google/) — PageSpeed/Lighthouse 与 Firebase
- [k.file](../file/file.md) — 站点文件读写
