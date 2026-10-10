# 数据库

> 菜单：左侧 **数据库** 分组（须在 **编辑菜单** 中勾选显示）  
> 路由前缀：`/_Admin/database/...?SiteId={站点GUID}`

**数据库**分组用于管理站点内的多种数据存储：**IndexedDB 动态表**、表关系、**键值存储**，以及 **SQLite / MySQL / SQL Server** 外部表结构浏览与数据维护；**SQL 日志** 用于查看脚本与后台产生的 SQL 记录。脚本侧统一通过 [k.DB](../../api/database/index.md) 访问（SQLite 见 [k.DB.sqlite](../../api/sqlite/index.md)）。

::: tip 与内容（Content）的区别
| | **数据库** | **内容 → 内容** |
|--|------------|-----------------|
| 模型 | 自定义表结构、KV、外部库表 | 内容类型 + 内容夹 |
| 典型用途 | 业务表、缓存键值、对接已有 SQL 库 | 页面可绑定的 CMS 条目 |
| API | `k.DB` | `k.content` |

二者可配合使用，勿混为同一套建模。
:::

::: info 高级菜单
数据库分组及子项在编辑菜单中标记为 **advanced**。未勾选时左侧不显示本分组。
:::

## 如何打开

1. 在 [编辑菜单](../navigation.md#权限与编辑菜单) 中勾选 **数据库** 及其需要的子项。
2. 左侧展开 **数据库**。

## 子菜单（与后台顺序一致）

| 菜单 | 路径 | 权限（feature） | 文档 |
|------|------|-----------------|------|
| IndexedDB 表 | `/database/table` | `database` | [IndexedDB 表](./table.md)（本篇） |
| IndexedDB 表关系 | `/database/table-relation` | `tableRelation` | [表关系](./table-relation.md) |
| 键值存储 | `/database/key-value` | `keyValue` | [键值存储](./key-value.md) |
| SQLite 表 | `/database/sqlite-table` | `database` | [SQLite 表](./sqlite-table.md) |
| MySQL 表 | `/database/mysql-table` | `database` | [MySQL 表](./mysql-table.md) |
| SQL Server 表 | `/database/sqlserver-table` | `database` | [SQL Server 表](./sqlserver-table.md) |
| SQL 日志 | `/database/sql-logs` | `database·log` | [SQL 日志](./sql-logs.md) |

### 开发参考（KScript API）

| 菜单 | API 文档 |
|------|----------|
| IndexedDB 表 | [k.DB.indexedDb](../../api/indexed-db/index.md) |
| SQLite / MySQL / SQL Server 表 | [k.DB](../../api/database/index.md) · [k.DB.sqlite](../../api/sqlite/index.md) |
| 键值存储 | [k.DB](../../api/database/index.md)（KV 接口，详见子篇） |
| SQL 日志 | 只读观测，无独立 `k.*` CRUD |

## 无侧栏菜单的编辑路由

以下页面从表列表进入，面包屑仍归属对应表类型菜单（`dbType` 查询参数决定高亮 **IndexedDB / SQLite / MySQL / SQL Server**）：

| 路由 | 说明 |
|------|------|
| `/database/table/columns` | 编辑表字段（列定义） |
| `/database/table/data` | 浏览、筛选表数据 |
| `/database/table/edit-data` | 新建或编辑单条数据 |

深链示例：

```text
/_Admin/database/table/columns?SiteId={站点GUID}&id={表Id}&dbType=IndexedDb
/_Admin/database/table/data?SiteId={站点GUID}&id={表Id}&dbType=Sqlite
```

`dbType` 取值与后台一致（如 `IndexedDb`、`Sqlite`、`Mysql`、`SqlServer`），以地址栏为准。

## 建议阅读顺序

若从站点数据建模入手，常见顺序为：

1. **[IndexedDB 表](./table.md)** — 站点内默认动态表  
2. **[表关系](./table-relation.md)** — 多表关联（依赖已有 IndexedDB 表）  
3. **[键值存储](./key-value.md)** — 简单 KV  
4. **SQLite / MySQL / SQL Server** — 按项目是否接入外部库选读  
5. **[SQL 日志](./sql-logs.md)** — 排障与审计  

## 相关

| 文档 | 说明 |
|------|------|
| [站点后台菜单总览](../navigation.md) | 完整菜单树 |
| [开发](../development/index.md) | Code 中调用 `k.DB` |
| [k.DB](../../api/database/index.md) | 脚本 API |
| [k.DB.sqlite](../../api/sqlite/index.md) | SQLite 查询与表操作 |
