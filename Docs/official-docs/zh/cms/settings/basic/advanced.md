# 基础 · 高级 Tab

> Tab：**高级** · `name=advance`  
> 深链：`/_Admin/system/settings?SiteId={站点GUID}&name=advance`

面向开发与运维的 **底层开关**：默认关系型数据库、`k.DB.sql` 指向、**SQL 日志** 与 **代码日志**、保存时约束修复、**Hook** 总开关、断点续传下载，以及站点 **自定义键值** 设置。

<DocImage src="/cms/settings/settings-basic-advanced.png" alt="高级 Tab" width="1120" />

## 默认数据库

**默认数据库**（`defaultDatabase`）决定 `k.DB.sql` 解析到哪一套关系型库：

| 值 | 说明 |
|----|------|
| **Auto** | 自动：优先级 **MySQL > SQL Server > SQLite**（须在 [服务集成](../integrations/) 配好连接） |
| **Sqlite** / **Mysql** / **SqlServer** | 固定指向对应库 |

详见 [k.DB](../../../api/database/index.md) 与 [数据库](../../database/index.md) 各表篇。

## SQL 日志

**SQL 日志**（`enableSqlLog`）：与 [数据库 → SQL 日志](../../database/sql-logs.md) 页右侧开关 **等价**，控制是否记录 `k.DB.sqlite` / `mysql` / `sqlServer` 的 SQL。

## 代码日志

分组 **代码日志**（`codeLogSettings`）：

| 字段 | 说明 |
|------|------|
| **启用** | 是否记录 KScript 代码日志 |
| **级别** | Debug / Information / Warning / Error / Critical |
| **保留天数** | 日志保留 `keepDays`（≥1） |

列表查询见 [开发 → 代码日志](../../development/code-log.md)。

## 约束与其它开关

| 开关 | 说明 |
|------|------|
| **保存时约束修复** | `enableConstraintFixOnSave`：保存内容等资源时自动修复部分约束冲突 |
| **请求 Hook** | `enableFrontEvents`：是否启用 [请求 Hook](../request-hooks.md) 机制（仍须在 Hook 页配置具体规则） |
| **操作 Hook** | `enableBackendEvents`：是否启用 [操作 Hook](../action-hooks.md) |
| **断点续传下载** | `continueDownload`：大文件下载续传（部分环境显示） |

## 自定义设置

**自定义设置**（`customSettings`）：键值对编辑器，供站点或模块读取的扩展配置（`KeyValueEditor`）。键名须符合产品约定；脚本可通过站点 API 或约定名访问，勿与 [键值存储](../../database/key-value.md)（`k.DB.keyValue`）混淆。

## 相关

- [基础设置概述](./index.md)  
- [SQL 日志](../../database/sql-logs.md)  
- [服务集成](../integrations/)（Database 连接）
