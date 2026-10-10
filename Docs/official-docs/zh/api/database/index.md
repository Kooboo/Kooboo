# k.DB

> 数据库操作 - 支持多种数据库的统一 CRUD 接口

## 概述

`k.DB` 提供对 Kooboo 内置数据库的访问，支持：

- **IndexedDB（站点动态表）** — 后台 [IndexedDB 表](../../cms/database/table.md) 建表，脚本使用 `k.DB.indexedDb.{表名}`
- **SQLite** — 开箱即用，无需配置
- **MySQL / SQL Server** — 须在服务集成中配置连接字符串（后台见 [MySQL 表](../../cms/database/mysql-table.md)、[SQL Server 表](../../cms/database/sqlserver-table.md)）

## 子模块

| 模块 | 说明 |
|------|------|
| [k.DB.indexedDb](../indexed-db/) | 站点 IndexedDB 动态表（对象 CRUD） |
| [k.DB.keyValue](../key-value/) | 站点键值对 |
| [k.DB.sqlite](../sqlite/) | SQLite（SQL） |


## TypeScript 定义

```ts
interface KDB {
    /** 站点 IndexedDB 动态表（表名为属性） */
    indexedDb: IDatabase;
    sqlite: SQLiteDB;
    mysql: MysqlDatabase;
    sqlServer: SqlServerDatabase;
}

interface SQLiteDB {
    /** 获取数据库内所有表名 */
    getTables(): string[];
    /** 获取指定表对象 */
    getTable(name: string): ITable;
    /** 查询数据，返回数组 */
    query(sql: string, params?: object): any[];
    /** 执行写操作，返回数字（1=成功） */
    execute(sql: string, params?: object): number;
    /** 获取查询操作符集合 */
    operators(): Operators;
    /** 在事务中执行多个操作 */
    transaction(action: Function): void;
}
```

## 选型建议

- **后台已建 IndexedDB 表、希望用对象读写** → [k.DB.indexedDb](../indexed-db/)
- **简单字符串键值、无表结构** → [k.DB.keyValue](../key-value/)
- **需要手写 SQL、或操作 SQLite 文件表** → [k.DB.sqlite](../sqlite/)
- **对接 MySQL / SQL Server** → 配置连接后使用 `k.DB.mysql` / `k.DB.sqlServer`，后台见 [CMS 数据库](../../cms/database/index.md)

## 相关文档

- [k.DB.indexedDb](../indexed-db/) — IndexedDB 动态表 API
- [k.DB.keyValue](../key-value/) — 键值存储 API
- [k.DB.sqlite](../sqlite/) — SQLite 数据库操作
- [k.content](../content/) — 内容管理
- [CMS：数据库](../../cms/database/index.md) — 后台表结构、键值与 [SQL 日志](../../cms/database/sql-logs.md)
