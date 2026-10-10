# 运维中心

> 菜单：左侧 **运维中心** 分组（须在 **编辑菜单** 中勾选显示）  
> 路由前缀：`/_Admin/system/...?SiteId={站点GUID}`

**运维中心**用于站点运行观测、协作与自动化：访问与行为数据、错误与日志、资源与 AB 实验、搜索/自动化及 AI 服务等。与 [站点设置](../navigation.md#站点设置) 同属 `/system` 路径前缀，但 **左侧菜单分组不同**（运维 vs 站点设置）。

::: tip 与「控制面板」
[控制面板](../site/dashboard.md) 上的 PV 趋势为概览；更完整的访客分析见 [访问统计](./visitor-logs.md)。
:::

## 如何打开

1. 在 [编辑菜单](../navigation.md#权限与编辑菜单) 中勾选 **运维中心** 及其子项（多项标有 `advanced`）。
2. 左侧展开 **运维中心**。

## 子菜单（与后台顺序一致）

| 菜单 | 路径 | 文档 |
|------|------|------|
| 访问统计 | `/system/visitor-logs` | [访问统计](./visitor-logs.md) |
| 行为分析 | `/system/user-behavior-analysis` | 暂未单独成篇 |
| 资源守护 | `/system/resource-guardian` | [资源守护](./resource-guardian.md) |
| 页面交互 | `/system/page-interaction` | [页面交互](./page-interaction.md) |
| AB 测试 | `/system/ab-testing` | [AB 测试](./ab-testing.md) |
| 错误监控 | `/system/error-monitoring` | 暂未单独成篇 |
| 操作日志 | `/system/site-logs` | [操作日志](./site-logs.md) |
| 协作同步 | `/system/sync` | [协作同步](./sync.md) |
| 搜索 | `/system/search` | 暂未单独成篇 |
| 自动化 | `/system/automation` | 暂未单独成篇 |
| AI 服务 | `/system/ai-service` | [AI 服务](../settings/ai-service.md) |

::: info AI 服务与站点设置
**AI 服务** 在 **运维中心** 与 **站点设置** 中各有一个菜单项，打开为同一页面 `/_Admin/system/ai-service`。文档只维护一份：[站点设置 · AI 服务](../settings/ai-service.md)，两处互相链接。
:::

## 建议阅读顺序

按菜单顺序阅读即可；若刚开通站点，常见路径为：

1. [访问统计](./visitor-logs.md) — 确认已启用采集并查看流量  
2. 错误监控、操作日志 — 排障  
3. 行为分析、页面交互、AB 测试 — 运营优化  
4. 协作同步、搜索、自动化、AI 服务 — 按项目需要  

## 相关

| 文档 | 说明 |
|------|------|
| [站点后台菜单总览](../navigation.md) | 完整菜单树 |
| [控制面板](../site/dashboard.md) | 站点概览 PV |
| [基础设置](../navigation.md#站点设置) | 域名、服务集成等（非本分组） |
