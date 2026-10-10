# 基础设置

> 菜单：**站点设置 → 基础设置**  
> 深链：`/_Admin/system/settings?SiteId={站点GUID}`

**基础设置**是站点级配置的入口：通过顶部 **Tab** 切换六个面板，修改后须点击底部 **保存**（或快捷键保存）才会写入站点。与 [域名管理](../domains.md)、[服务集成](../integrations/) 等不同，本页直接编辑 `Site` 模型字段，而非独立的集成项列表。

::: tip 权限
查看与编辑：`site·edit`。导出站点包另需 `site·export`。
:::

## 如何打开

1. 在 [编辑菜单](../../navigation.md#权限与编辑菜单) 中勾选 **站点设置 → 基础设置**。  
2. 左侧 **站点设置 → 基础设置**。

<DocImage src="/cms/settings/settings-basic-tabs.png" alt="基础设置 Tab 栏" width="1120" />

## Tab 与文档

切换 Tab 时地址栏会带上查询参数 **`name`**；深链可直接打开某一 Tab。

| Tab | `name` | 文档 | 主要内容 |
|-----|--------|------|----------|
| 基础 | `basic` | [基础](./general.md) | 显示名、Base URL、SSL、SPA、**多语言**、导入/导出站点包 |
| 访问控制 | `access` | [访问控制](./access.md) | 站点类型、路径规则、限流、封禁、CORS、开发密码 |
| 性能 | `performance` | [性能](./performance.md) | JS/CSS 压缩与缓存、图片缓存、WebP、资源缓存 |
| SEO | `seo` | [SEO](./seo.md) | Lighthouse、Sitemap、PWA、访问日志开关、Canonical |
| 编辑器 | `editor` | [编辑器](./editor.md) | 富文本、内联设计、代码补全库、操作录屏 |
| 高级 | `advance` | [高级](./advanced.md) | 默认数据库、SQL/代码日志、Hook 总开关、自定义设置 |

深链示例：

```text
/_Admin/system/settings?SiteId={站点GUID}&name=basic
/_Admin/system/settings?SiteId={站点GUID}&name=advance
```

## 保存与离开

- 页面底部 **保存** 条（`KBottomBar`）提交当前所有 Tab 的未保存修改（`Site` POST）。  
- 支持快捷键 **保存**（与全局后台一致）。  
- 离开本页到其他菜单时，若有未保存修改会提示确认（`saveTip`）。

部分面板在保存前会执行钩子（如多语言、PWA、Lighthouse、限流 IP 列表序列化），以服务端最终写入为准。

## 建议阅读顺序

1. [基础](./general.md) — 站点标识与多语言  
2. [访问控制](./access.md) — 上线前安全与可见性  
3. [性能](./performance.md) · [SEO](./seo.md) — 按站点类型选读  
4. [编辑器](./editor.md) — 内容/页面编辑体验  
5. [高级](./advanced.md) — 与 [数据库](../../database/index.md)、[Hook](../request-hooks.md) 联动  

## 相关

| 文档 | 说明 |
|------|------|
| [站点设置概述](../index.md) | 全分组菜单 |
| [域名管理](../domains.md) | 绑定访问域名（与 Base URL 配合） |
| [开发 → SPA 多语言](../../development/spa-multilingual.md) | 前端 SPA 语言包（非本页多语言） |
| [k.label](../../../api/label/index.md) | 多语言 Label 脚本 API |
