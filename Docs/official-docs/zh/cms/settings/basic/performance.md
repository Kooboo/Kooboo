# 基础 · 性能 Tab

> Tab：**性能** · `name=performance`  
> 深链：`/_Admin/system/settings?SiteId={站点GUID}&name=performance`

优化静态资源交付：**JS/CSS 压缩**、浏览器缓存、**图片**缓存策略、自动 **WebP** 转换，以及在私有/本地服务器上启用 **资源缓存**（样式/脚本/图片/内容）。

<DocImage src="/cms/settings/settings-basic-performance.png" alt="性能 Tab" width="1120" />

## JS / CSS / 视频

| 开关 | 说明 |
|------|------|
| **JS/CSS 压缩** | 发布时压缩脚本与样式体积 |
| **JS/CSS 浏览器缓存** | 为 JS/CSS 响应设置浏览器缓存头 |
| **视频浏览器缓存** | 为视频资源设置浏览器缓存 |

## 图片浏览器缓存

独立分组 **图片浏览器缓存**（`enableImageBrowserCache`），模式二选一：

| 模式 | 说明 |
|------|------|
| **版本控制** | `imageCacheDays=0`：通过版本号/query 失效，适合频繁更新图片 |
| **固定天数** | 设置缓存有效 **天数**（≥1） |

## 自动转 WebP

**自动将图片转为 WebP**（`automateCovertImageToWebp`）：上传或处理图片时生成 WebP 以减小体积（见 Tooltip）。

## 资源缓存（私有/本地服务器）

当环境为 **非在线公有云** 或 **私有服务器** 时显示 **资源缓存** 分组：

| 项 | 说明 |
|----|------|
| 总开关 | `enableResourceCache` |
| 勾选类型 | **样式**、**脚本**、**图片**、**内容**（位标志 `resourceCaches`） |

用于在服务器侧缓存渲染结果或资源，减轻重复计算。

## 相关

- [基础设置概述](./index.md)  
- [SEO](./seo.md)（Lighthouse 优化与性能检测相关）
