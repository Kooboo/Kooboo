# SQL 日志

> 菜单：**数据库 → SQL 日志**  
> 深链：`/_Admin/database/sql-logs?SiteId={站点GUID}`

**SQL 日志**用于查看站点脚本通过 **`k.DB.sqlite` / `k.DB.mysql` / `k.DB.sqlServer`** 执行的 SQL 语句：含语句正文、参数、耗时与数据库类型，便于排障与审计。本页为 **只读** 观测，不提供删除或编辑日志；也无对应 `k.*` 写入 API。

::: tip 权限
查看本页及列表数据：`database·log`（`database` 功能的 **log** 动作）。  
工具栏右侧 **启用** 开关会保存站点设置，通常需站点 **编辑** 类权限（与保存站点配置一致）。
:::

::: warning 记录范围
| 会记录 | 不会记录 |
|--------|----------|
| `k.DB.sqlite.query` / `execute` 等触发的 SQL | [IndexedDB 表](./table.md)（`k.DB.indexedDb`）操作 |
| `k.DB.mysql`、`k.DB.sqlServer` 的 SQL | [键值存储](./key-value.md)（`k.DB.keyValue`） |
| 后台 **SQLite / MySQL / SQL Server 表** 维护时经脚本层执行的 SQL（若走同一执行器） | 未开启 **SQL 日志** 时的任何 SQL |

须先在站点开启 **SQL 日志**（见 [启用日志](#启用-sql-日志)），之后产生的 SQL 才会写入日志。
:::

## 如何打开

1. 在 [编辑菜单](../navigation.md#权限与编辑菜单) 中勾选 **数据库 → SQL 日志**。
2. 左侧 **数据库 → SQL 日志**。

<DocImage src="/cms/database/sql-logs-overview.png" alt="SQL 日志页" width="1120" />

## 启用 SQL 日志

站点级开关 **`enableSqlLog`**，两处等价（改一处即可）：

| 位置 | 路径 |
|------|------|
| **本页工具栏** | 右侧 **启用** 开关，切换后立即保存站点 |
| **站点设置 → 高级** | `/_Admin/system/settings?SiteId={站点GUID}&name=advance`，**SQL 日志** 项 |

关闭后，新的脚本 SQL **不再** 写入日志；已产生的历史周次数据仍可按周查看。

<DocImage src="/cms/database/sql-logs-toolbar.png" alt="SQL 日志筛选与启用开关" width="1120" />

## 筛选与搜索

工具栏从左到右：

| 控件 | 说明 |
|------|------|
| **周** | 按 **自然周** 选择日志文件；选项文案为 `yyyy-MM-dd~yyyy-MM-dd`（该周起止日期）。仅列出 **当前站点在该周有条目** 的周；若尚无记录，默认显示 **当前周** |
| **类型** | 可选 **SQLite**、**MySQL**、**SQLServer**（对应 `sqlite` / `mysql` / `sqlserver`）；清空表示全部类型 |
| **关键字** | 在 **SQL 正文** 中模糊匹配（不区分大小写）；输入后约 1 秒防抖再查询；清空即刷新 |
| **启用** | 见 [启用 SQL 日志](#启用-sql-日志) |

切换 **周**、**类型** 或关键字会重新请求 `SqlLog/List` 并回到第 1 页。

## 日志列表

分页默认 **50** 条/页（`KTable` 底部分页切换页码）。

| 列 | 说明 |
|----|------|
| **类型** | 数据库来源，如 `sqlite`、`mysql`、`sqlserver` |
| **日期时间** | 执行时间（站点时区展示） |
| **执行时间** | 耗时，单位 **ms** |
| **内容** | SQL 语句摘要（过长时表格内截断） |
| **参数** | 序列化后的参数对象；无参数或 `null` 时为空 |
| 行末 **详情** | 有数据时打开 [详情弹窗](#详情弹窗) |

当周尚无记录时，表格显示 **暂无数据**（须已 **启用** 日志且站点内有 `k.DB.sqlite` 等 SQL 执行）。

<DocImage src="/cms/database/sql-logs-list.png" alt="SQL 日志列表" width="1120" />

### 详情弹窗

点击行末 **眼睛** 图标打开。展示完整 **类型**、**日期时间**、**执行时间**，以及两段完整文本：

1. **SQL 语句**（可换行）  
2. **参数**（JSON 等序列化结果）

用于复制完整语句或在慢查询场景下对照参数。

## 日志如何产生

脚本访问关系型库时，若 `EnableSqlLog` 为真，执行器在每次 SQL 完成后异步写入服务端按 **周** 划分的日志文件（站点 `AppData` 下 `SqlLog` 目录，文件名如 `{年周}.log`）。写入与页面列表查询均按 **站点 ID** 过滤，仅能看到本站记录。

示例（开启日志后）：

```typescript
k.DB.sqlite.query("SELECT * FROM logs WHERE id = @id", { id: 1 })
k.DB.sqlite.execute("INSERT INTO logs (msg) VALUES (@msg)", { msg: "ok" })
```

MySQL / SQL Server 须在 [服务集成](../navigation.md)（`/system/config`）中配置连接后，通过 `k.DB.mysql` / `k.DB.sqlServer` 执行才会出现对应 **类型** 的日志。

::: info 与后台表维护
[SQLite 表](./sqlite-table.md) 列表、列与数据的 **后台 API** 与脚本 `k.DB.sqlite` 是否为同一条执行链，以实际环境为准。排障脚本 SQL 时，以本页 **类型** 与 **内容** 列为准；若后台操作未产生日志，请确认是否经由 `k.DB.*` 执行且已 **启用** 日志。
:::

## 典型工作流

1. 在 **高级** 或本页打开 **SQL 日志**。  
2. 复现问题（访问会执行 `k.DB.sqlite` 等的页面或定时任务）。  
3. 选择对应 **周**，按 **类型** / **关键字** 缩小范围。  
4. 打开 **详情** 复制 SQL 与参数，回到 Code 或数据库工具验证。  
5. 生产环境长期开启可能增加 I/O 与磁盘占用，排障结束后可关闭 **启用**。

## 相关

| 文档 | 说明 |
|------|------|
| [数据库概述](./index.md) | 菜单与权限 |
| [SQLite 表](./sqlite-table.md) | 内置 SQLite 与 CSV |
| [k.DB.sqlite](../../api/sqlite/index.md) | 脚本 SQL API |
| [k.DB](../../api/database/index.md) | 数据库总览 |
