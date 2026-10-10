# Integrations · Database

> Group: **Database**  
> Deep link: `/_Admin/system/config?SiteId={siteGuid}&group=Database`

The **Database** group configures **external database and cache** connections plus built-in **SQLite** options. After save, scripts use **`k.DB.mysql`**, **`k.DB.sqlServer`**, **`k.DB.mongo`**, **`k.DB.redis`**, etc.; if unset or invalid, matching [Database](../../database/index.md) menus show onboarding pages.

<DocImage src="/cms/settings/settings-integrations-database.png" alt="Database group" width="1120" />

## Integration Items

| Name | Description | Admin menu |
|------|-------------|------------|
| **Mysql** | MySQL **ConnectionString** | [MySQL tables](../../database/mysql-table.md) |
| **SqlServer** | SQL Server **ConnectionString** | [SQL Server tables](../../database/sqlserver-table.md) |
| **Mongo** | MongoDB connection string | Script `k.DB.mongo` |
| **Redis** | Redis **ConnectionString** | Script `k.DB.redis` |
| **Sqlite** | Built-in SQLite options (e.g. **ForeignKeyConstraint**); not a connection string | [SQLite tables](../../database/sqlite-table.md) (out of the box) |

When editing **Mysql** / **SqlServer**, dialog Alerts often include connection string examples per Kooboo built-in docs, e.g.:

```text
Server=...;Port=3306;Database=...;Uid=...;Pwd=...
Data Source=...;Initial Catalog=...;User ID=...;Pwd=...
```

## Default Database

[Basic Settings · Advanced](../basic/advanced.md#default-database) **Default database** (`defaultDatabase`) decides where **`k.DB.sql`** points: **Auto** priority is MySQL > SQL Server > SQLite.

## SQL Log

Whether relational SQL is written to [SQL logs](../../database/sql-logs.md) is controlled by site **enableSqlLog** (Advanced tab or SQL log page), independent of Database group config.

## Related

- [Integrations overview](./index.md)  
- [k.DB](../../../api/database/index.md) · [k.DB.sqlite](../../../api/sqlite/index.md)
