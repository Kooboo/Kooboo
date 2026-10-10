# 基础 · SEO Tab

> Tab：**SEO** · `name=seo`  
> 深链：`/_Admin/system/settings?SiteId={站点GUID}&name=seo`

站点级 **搜索引擎与 PWA** 配置：Lighthouse 优化项、**Sitemap** 生成、**PWA** Manifest/Service Worker、是否记录 **访问日志**、屏蔽 SEO 爬虫、图片 **Alt** 辅助，以及 **Canonical URL** 规则。

<DocImage src="/cms/settings/settings-basic-seo.png" alt="SEO Tab" width="1120" />

## Lighthouse 优化

分组 **Lighthouse 优化**（`enableLighthouseOptimization`）：按产品内置检查项列表逐项 **启用** 并配置子参数（保存时序列化为 `lighthouseSettingsJson`）。用于自动注入或调整 HTML 以满足常见性能/SEO 检测项。

## Sitemap

分组 **Sitemap**（`sitemapSettings.enable`）：

| 字段 | 说明 |
|------|------|
| **路径** | 站点地图对外路径（如 `/sitemap.xml`） |
| **生成方式** | **自动** 由系统生成，或 **手动** 编写 TypeScript 代码生成 |
| **编辑代码** | 手动模式下打开 Monaco 编辑器编写 sitemap 脚本（`sitemapSettings.code`） |

## PWA

分组 **PWA**：配置 **Manifest**（名称、图标等，图标可从 [媒体库](../../site/media.md) 选取）与 **Service Worker** 缓存规则（Method、Pattern、ExpiresIn 等）。保存时分别序列化 `pwa.manifest` / `pwa.serviceWorker` JSON。

## 其它开关

| 开关 | 说明 |
|------|------|
| **访问日志** | `enableVisitorLog`：是否记录访客访问（数据在 [运维 · 访问日志](../../operations/visitor-logs.md) 查看） |
| **阻止 SEO** | `blockingSeo`：限制搜索引擎抓取/索引行为 |
| **启用图片 Alt** | `enableImageAlt`：辅助输出或校验图片 `alt` 属性 |

## Canonical URL

**Canonical URL** 分组：配置规范链接生成规则，减少重复 URL 对 SEO 的影响（字段以界面为准）。

## 相关

- [基础设置概述](./index.md)  
- [性能](./performance.md)  
- [运维 · 访问日志](../../operations/visitor-logs.md)
