# k.utils

> 通用工具集 — 字符串/时间/URI、格式转换、图片处理等

## 概述

`k.utils` 聚合 Kooboo 服务端常用的辅助能力。子模块均通过 `k.utils.*` 访问。

使用前请先读本索引，再按需打开子文档。**禁止臆造方法名**。

## 模块导航

### 基础与文本

| 文档 | 说明 |
| --- | --- |
| [基础方法](basics.md) | `sleep`、字节与字符串互转 |
| [字符串 / 时间 / URI / MIME](string-date-uri.md) | `string`、`date`、`uri`、`mime` |

### 媒体与转换

| 文档 | 说明 |
| --- | --- |
| [格式转换](converter.md) | HTML→PDF、GIF→WebP、Office→HTML |
| [图片处理](image.md) | 缩放、水印、格式转换 |
| [截图](screenshot.md) | 远程 URL / 本站页面 / HTML 截图 |

### 文档与压缩

| 文档 | 说明 |
| --- | --- |
| [Office / Excel](office.md) | `.xls` / `.xlsx` 读写与导出 |
| [XML / DOM](xml-dom.md) | XML 构建、HTML 解析 |
| [压缩](compression.md) | gzip、zip |

### 集成与运维

| 文档 | 说明 |
| --- | --- |
| [短信](sms.md) | 阿里云 / 腾讯云 / 中国移动 |
| [ClientJS](client-js.md) | 向浏览器注入变量、Monaco 依赖片段 |
| [模板 / 内联 HTML](template-inline.md) | View 渲染、Page 内联 |
| [CDN](cdn.md) | CDN 缓存开关 |

### 未单独成文（按需查运行时）

| 模块 | 说明 |
| --- | --- |
| `CRM` | `k.utils.CRM`；顾客和订单查询；暂无独立 reference |
| `puppeteer` | 浏览器任务和截图；在线服务器可能不可用；暂无独立 reference |
| `community` | `k.utils.community`；社区包分享与搜索；暂无独立 reference |
| `ninjible` | `k.utils.ninjible`；站点导入任务；暂无独立 reference |
| `sitemap` | `k.utils.sitemap`；站点地图生成；暂无独立 reference |


## 使用原则

1. **二进制数据** — 图片、PDF 等以 `number[]` 传递，配合 `k.file.readBinary` / `k.file.writeBinary`。
2. **k.file 路径** — 写入参数为 **fullName**；浏览器访问用 `k.file.url(fullName)`（含 `/__kb/kfile/`）。详见 [File 模块](../file/index.md)。
3. **大小写** — `k.utils.CDN` 为 PascalCase；网络 IP 工具位于 `k.net.IP`。
4. **先查子文档** — 方法签名以对应 reference 为准。
