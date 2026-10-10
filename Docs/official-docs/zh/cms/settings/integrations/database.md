# 服务集成 · Database

> Group：**Database**  
> 深链：`/_Admin/system/config?SiteId={站点GUID}&group=Database`

**Database** 分组配置站点可用的 **外部数据库与缓存** 连接，以及内置 **SQLite** 的少量选项。保存后，脚本通过 **`k.DB.mysql`**、**`k.DB.sqlServer`**、**`k.DB.mongo`**、**`k.DB.redis`** 等访问；未配置或连接无效时，对应 [数据库](../../database/index.md) 菜单会显示引导页。

<DocImage src="/cms/settings/settings-integrations-database.png" alt="Database 分组" width="1120" />

## 集成项

| 名称 | 说明 | 后台菜单 |
|------|------|----------|
| **Mysql** | MySQL **ConnectionString** | [MySQL 表](../../database/mysql-table.md) |
| **SqlServer** | SQL Server **ConnectionString** | [SQL Server 表](../../database/sqlserver-table.md) |
| **Mongo** | MongoDB 连接字符串 | 脚本 `k.DB.mongo` |
| **Redis** | Redis **ConnectionString** | 脚本 `k.DB.redis` |
| **Sqlite** | 内置 SQLite 选项（如 **ForeignKeyConstraint**）；非连接串 | [SQLite 表](../../database/sqlite-table.md)（开箱即用） |

编辑 **Mysql** / **SqlServer** 时，弹窗 Alert 通常含连接串示例，格式以 Kooboo 内置说明为准，例如：

```text
Server=...;Port=3306;Database=...;Uid=...;Pwd=...
Data Source=...;Initial Catalog=...;User ID=...;Pwd=...
```

## 默认数据库

[基础设置 · 高级](../basic/advanced.md#默认数据库) 中 **默认数据库**（`defaultDatabase`）决定 **`k.DB.sql`** 指向哪一套关系型库：**Auto** 时优先级为 MySQL > SQL Server > SQLite。

## SQL 日志

关系型库 SQL 是否写入 [SQL 日志](../../database/sql-logs.md) 由站点 **enableSqlLog** 控制（高级 Tab 或 SQL 日志页），与 Database 分组配置独立。

## 相关

- [服务集成概述](./index.md)  
- [k.DB](../../../api/database/index.md) · [k.DB.sqlite](../../../api/sqlite/index.md)
