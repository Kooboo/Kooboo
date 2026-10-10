# k.google

> Google PageSpeed/Lighthouse 与 Firebase 服务

## 概述

`k.google` 汇总 Google 相关能力。PageSpeed/Lighthouse 方法直接位于 `k.google`，Firebase Admin 能力位于 `k.google.firebase`。

| 能力 | KScript 入口 | 说明 |
|------|--------------|------|
| PageSpeed / Lighthouse | `k.google.lHRJson()` 等根方法 | 生成、解析与保存 Lighthouse 报告 |
| Firebase | `k.google.firebase` | Firebase 身份验证与用户管理 |

## lHRJson() / lHRUrl()

对绝对 URL 生成 Lighthouse JSON 或在线报告链接。

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `absoluteUrl` | `string` | 是 | 要分析的绝对 URL |
| `mobileDevice` | `boolean` | 是 | 兼容参数；当前报告请求不会根据该值切换策略 |

**返回：** `lHRJson()` 返回 Lighthouse JSON 字符串；`lHRUrl()` 返回可打开的报告 URL。

```ts
const json = k.google.lHRJson("https://www.kooboo.com", false)
const report = k.google.parseJson(json)
const summary = report.getSummary()

return {
    performance: summary.performance,
    accessibility: summary.accessibility,
    seo: summary.seo
}
```

如果只需要可打开的 Lighthouse 报告链接：

```ts
const reportUrl = k.google.lHRUrl("https://www.kooboo.com", true)
return { reportUrl }
```

## lHRUrlFromJson()

将已有 Lighthouse JSON 转换为可在 Kooboo 中打开的报告 URL。

```ts
const json = k.google.lHRJson("https://www.kooboo.com", false)
const reportUrl = k.google.lHRUrlFromJson(json)
return { reportUrl }
```

## parseJson() / saveJson() / getJson()

解析 Lighthouse JSON、保存到系统并凭 Id 取回。

| 方法 | 参数 | 返回 | 说明 |
|------|------|------|------|
| `parseJson(json)` | `string` | `FullLHRObj` | 解析 Lighthouse JSON |
| `saveJson(json)` | `string` | `Guid` | 保存 JSON 并返回 Id |
| `getJson(id)` | `Guid` | `string \| null` | 根据 Id 读取已保存 JSON |

```ts
const json = k.google.lHRJson("https://www.kooboo.com", false)
const id = k.google.saveJson(json)
const saved = k.google.getJson(id)

return {
    id,
    saved: !!saved,
    summary: k.google.parseJson(saved).getSummary()
}
```

## shortLhrReportUrl()

由 JSON 生成站点内的短报告 URL。

```ts
const json = k.google.lHRJson("https://www.kooboo.com", false)
const url = k.google.shortLhrReportUrl(json)

return {
    url,
    fullUrl: k.site.info.makeAbsUrl(url)
}
```

## 相关文档

- [k.utils.ninjible](../utils/ninjible.md) — 站点迁移与 Lighthouse 对比
