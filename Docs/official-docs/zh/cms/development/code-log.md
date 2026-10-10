# 代码日志

> 菜单：**开发 → 代码日志**  
> 深链：`/_Admin/development/code-log?SiteId={站点GUID}`

查看站点 **KScript / 代码执行** 产生的运行日志：按日志级别、时间、TraceId、关键字筛选，并查看单条详情。与 [操作日志](../operations/site-logs.md)（内容变更审计）不同，本页面向 **运行时诊断**。

列表中的每一条记录，均来自脚本中的 [`k.logger`](../../api/logger/index.md) 调用；后台不提供手工「新建日志」，只负责查询、筛选与查看详情。

## 日志来源（k.logger）

在 Code、API、页面脚本等 KScript 中写入日志，例如：

```ts
k.logger.information('Payment.Callback', `orderId=${orderId}`)
k.logger.error('Payment.Callback', JSON.stringify(err))
```

| `k.logger` 方法 | 后台页签 / `level` | 列表「分类」列 | 列表「消息」列 |
|-----------------|-------------------|----------------|----------------|
| `debug()` | 调试 · `Debug` | 第 1 个参数 `category`（可省略） | 消息正文 |
| `information()` | 信息 · `Information` | 同上 | 同上 |
| `warning()` | 警告 · `Warning` | 同上 | 同上 |
| `error()` | 错误 · `Error` | 同上 | 同上 |
| `critical()` | 严重 · `Critical` | 同上 | 同上 |

只传一个参数时，该参数为 **消息**，**分类** 为空。对象须先 `JSON.stringify()` 再写入。

同一 HTTP 请求内的多条日志往往共用 **TraceId**（便于在后台按 TraceId 串联排查）。脚本侧也可用 [`k.logger.query()`](../../api/logger/index.md) 按条件查询，参数与下文筛选栏、分页字段一致。

页头 **启用**（`codeLogSettings.enable`）关闭后，站点不再采集新的 `k.logger` 日志；已有历史周次数据仍可切换 **周** 查看。

::: tip 权限
菜单权限：`code·log`。须在编辑菜单中勾选 **开发 → 代码日志**（`advanced`）。
:::

## 如何打开

1. 在 [编辑菜单](../navigation.md#权限与编辑菜单) 中勾选 **开发 → 代码日志**。
2. 左侧 **开发 → 代码日志**。

<DocImage src="/cms/development/code-log-overview.png" alt="代码日志页总览" width="1120" />

## 页头

| 控件 | 说明 |
|------|------|
| **启用** | `codeLogSettings.enable`；切换后立即 `saveSite`，关闭后不再写入/采集 `k.logger` 新日志 |
| **周** | 选择日志分区周次（`weekname`，选项来自 `getWeeks`）；切换后刷新列表 |

<DocImage src="/cms/development/code-log-header.png" alt="代码日志页头：启用与周次" width="1120" />

## 日志级别页签

横向页签筛选 `level`（内容为隐藏式 tabs，仅切换级别）：

| 页签 | 查询 `level` |
|------|----------------|
| **全部** | 不传级别 |
| **调试** | `Debug` |
| **信息** | `Information` |
| **警告** | `Warning` |
| **错误** | `Error` |
| **严重** | `Critical` |

<DocImage src="/cms/development/code-log-tabs.png" alt="代码日志级别页签" width="1120" />

## 筛选栏

| 控件 | 说明 |
|------|------|
| **日期时间范围** | `startDate` / `endDate`（格式化为站点时间后查询） |
| **TraceId** | 精确追踪一次请求链路（输入防抖刷新） |
| **关键字** | `keyword`，匹配日志正文等（防抖） |

<DocImage src="/cms/development/code-log-filters.png" alt="代码日志筛选：时间与 TraceId、关键字" width="1120" />

## 日志列表

分页表格（默认每页 30 条，`creationDate` 排序）：

| 列 | 说明 |
|----|------|
| **级别** | `level` |
| **日期时间** | `creationDate` |
| **TraceId** | 请求追踪 Id |
| **分类** | `category` |
| **消息** | `message` 摘要（过长截断） |
| **详情** | 眼睛图标，打开 [详情弹窗](#详情弹窗) |

<DocImage src="/cms/development/code-log-table.png" alt="代码日志列表" width="1120" />

### 详情弹窗

展示 **TraceId**、**日期时间**、**级别**、**分类**，以及完整 **消息** 正文（`<pre>` 换行保留）。

<DocImage src="/cms/development/code-log-detail-dialog.png" alt="代码日志详情弹窗" width="1120" />

## 相关

| 文档 | 说明 |
|------|------|
| [k.logger](../../api/logger/index.md) | 记录与查询日志（开发必读） |
| [开发概述](./index.md) | 开发菜单索引 |
| [代码](./code.md) | 代码文件编辑 |
| [操作日志](../operations/site-logs.md) | 站点对象变更审计 |
