# k.DB

> Database operations - unified CRUD interface supporting multiple databases

## Overview

`k.DB` provides access to Kooboo's built-in database, supporting:

- **IndexedDB (site dynamic table)** — the background [IndexedDB Tables](../../cms/database/table.md) page creates the table, and the script uses `k.DB.indexedDb.{table name}`
- **SQLite** — works out of the box, no configuration required
- **MySQL / SQL Server** — The connection string must be configured in the service integration (see [MySQL Tables](../../cms/database/mysql-table.md) and [SQL Server Tables](../../cms/database/sqlserver-table.md) in the admin UI)

## Submodules

| module | Description |
|------|------|
| [k.DB.indexedDb](../indexed-db/) | Site IndexedDB dynamic table (object CRUD) |
| [k.DB.keyValue](../key-value/) | Site key-value pairs |
| [k.DB.sqlite](../sqlite/) | SQLite(SQL) |


## TypeScript definition

```ts
interface KDB {
    /** Site IndexedDB dynamic tables (table name as property) */
    indexedDb: IDatabase;
    sqlite: SQLiteDB;
    mysql: MysqlDatabase;
    sqlServer: SqlServerDatabase;
}

interface SQLiteDB {
    /** Get all table names in the database */
    getTables(): string[];
    /** Get the specified table object */
    getTable(name: string): ITable;
    /** Query data and return an array */
    query(sql: string, params?: object): any[];
    /** Execute a write operation and return a number (1 = success) */
    execute(sql: string, params?: object): number;
    /** Get the query operator set */
    operators(): Operators;
    /** Execute multiple operations in a transaction */
    transaction(action: Function): void;
}
```

## Selection suggestions

- **The IndexedDB table has been built in the background and we hope to use objects to read and write** → [k.DB.indexedDb](../indexed-db/)
- **Simple string key value, no table structure** → [k.DB.keyValue](../key-value/)
- **Requires handwriting SQL or operating SQLite file tables** → [k.DB.sqlite](../sqlite/)
- **Connecting to MySQL / SQL Server** → After configuring the connection, use `k.DB.mysql` / `k.DB.sqlServer`; see [CMS Database](../../cms/database/index.md)

## Related Docs

- [k.DB.indexedDb](../indexed-db/) — IndexedDB dynamic table API
- [k.DB.keyValue](../key-value/) — Key-value storage API
- [k.DB.sqlite](../sqlite/) — SQLite database operations
- [k.content](../content/) — Content Management
- [CMS: Database](../../cms/database/index.md) — background table structure, key values ​​and [SQL Logs](../../cms/database/sql-logs.md)
