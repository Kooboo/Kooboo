# SQL Server Tables

> Menu: **Database → SQL Server Tables**  
> Deep link: `/_Admin/database/sqlserver-table?SiteId={siteGuid}`

**SQL Server tables** let you browse and manage tables in a connected **external SQL Server** database from the admin: create tables, maintain columns and data, and **import / export CSV**. Configure the **SqlServer** connection string under [Service integrations](../navigation.md) first; scripts use **`k.DB.sqlServer`** for SQL or table objects. Child routes must include **`dbType=SqlServer`**; the left menu highlights **SQL Server Tables**.

::: tip Permissions
Same as [SQLite tables](./sqlite-table.md): `database` (view), `database·edit`, `database·delete`.  
Configuring the connection string requires **Service integrations** `config·edit`.
:::

::: info Difference from MySQL / SQLite
| | **SQL Server tables** | **MySQL tables** | **SQLite tables** |
|--|----------------------|------------------|-------------------|
| Data source | External SQL Server | External MySQL | Site built-in SQLite |
| Service integration item | **Database → SqlServer** | **Database → Mysql** | Not required |
| `dbType` | `SqlServer` | `MySql` | `Sqlite` |
| Script | `k.DB.sqlServer` | `k.DB.mysql` | `k.DB.sqlite` |
| SQL log type | `sqlserver` | `mysql` | `sqlite` |

Column editing and data browsing **share the same pages** as [MySQL tables](./mysql-table.md) and [SQLite tables](./sqlite-table.md) (relational DB UI is consistent; only IndexedDB column settings include primary key/unique/auto-increment).
:::

## Configure SQL Server connection

1. Open **Site settings → Service integrations**: `/_Admin/system/config?SiteId={siteGuid}`  
2. Expand the **Database** group and edit **SqlServer**. See [Service integrations · Database](../settings/integrations/database.md).  
3. Enter the **connection string** (ConnectionString) and save.

Example format (follow Kooboo built-in guidance):

```text
Data Source=127.0.0.1;Initial Catalog=mydb;User ID=sa;Pwd=123
```

You can also click **Configure now** on the **SQL Server Tables** unconfigured guide page to jump to `/_Admin/system/config?SiteId={siteGuid}&group=Database`.

::: warning Security
Connection strings contain credentials; do not commit them to public repositories. Use least-privilege database users in production. Kooboo applies common TLS certificate options when connecting (e.g. `TrustServerCertificate`); exact behavior depends on the deployment driver.
:::

## How to Open

1. In [Edit Menu](../navigation.md#permissions-and-edit-menu), check **Database → SQL Server Tables**.  
2. Complete [Configure SQL Server connection](#configure-sql-server-connection).  
3. Open **Database → SQL Server Tables** in the left sidebar.

When not configured or the connection is invalid, the page shows an error and **Configure now**; no table list is shown.

<DocImage src="/cms/database/sqlserver-table-unconfigured.png" alt="SQL Server unconfigured guide" width="1120" />

After a successful connection, list, columns, data, CSV import, and other UIs **share the same pages** as [SQLite tables](./sqlite-table.md) (only `dbType=SqlServer` and menu highlight differ). Screenshots below use SQLite as an example; SQL Server looks the same once configured.

## Table List

| Action | Permission | Description |
|--------|------------|-------------|
| **Create table** | `database·edit` | Create table in SQL Server and register admin schema |
| Click **Name** | `database` | Open [table data](#table-data) |
| **Settings** (gear) | `database` | Open [column settings](#column-settings) |
| Multi-select **Delete** | `database·delete` | Delete selected physical tables (use with care) |

Table name rules match IndexedDB / SQLite: 1–50 characters, letter or digit start, letters and digits only.

<DocImage src="/cms/database/sqlite-table-overview.png" alt="Table list (same UI as SQLite tables)" width="1120" />

<DocImage src="/cms/database/sqlite-table-toolbar.png" alt="Create table toolbar" width="1120" />

<DocImage src="/cms/database/sqlite-table-list.png" alt="Table name list example" width="1120" />

### Create table dialog

<DocImage src="/cms/database/sqlite-table-create-dialog.png" alt="Create table dialog" width="1120" />

## Column Settings

Route example:

```text
/_Admin/database/table/columns?SiteId={siteGuid}&table={tableName}&dbType=SqlServer
```

Same as [SQLite column settings](./sqlite-table.md#column-settings): list shows **column name**, **control type**, **index**; column editor has no **length**, **auto-increment**, or **primary key/unique** (IndexedDB only). Save updates SQL Server table structure and admin control mapping.

<DocImage src="/cms/database/sqlite-table-columns.png" alt="Column settings (same UI as SQLite tables)" width="1120" />

<DocImage src="/cms/database/sqlite-table-column-dialog.png" alt="Column editor dialog" width="1120" />

## Table Data

Route example:

```text
/_Admin/database/table/data?SiteId={siteGuid}&table={tableName}&dbType=SqlServer
```

### Toolbar

| Button | Permission | Description |
|--------|------------|-------------|
| **New data** | `database·edit` | Open edit page |
| **Export data** | `database` | Export CSV |
| **Import data** | `database·edit` | [CSV import](#csv-import) |
| **Settings** | — | Jump to column settings |

<DocImage src="/cms/database/sqlite-table-data-toolbar.png" alt="Table data toolbar" width="1120" />

<DocImage src="/cms/database/sqlite-table-data.png" alt="Table data list" width="1120" />

### CSV import

Fields and flow match [SQLite CSV import](./sqlite-table.md#csv-import): skip rows, upload `.csv`, field mapping (required/unique), optional overwrite.

<DocImage src="/cms/database/sqlite-table-import-dialog.png" alt="CSV import dialog" width="1120" />

## Edit Data

Route example:

```text
/_Admin/database/table/edit-data?SiteId={siteGuid}&table={tableName}&dbType=SqlServer
```

Form rendered by column **control type**; **Save** / **Cancel** returns to the data list.

## Script access (development)

```typescript
const rows = k.DB.sqlServer.query(
  "SELECT * FROM orders WHERE id = @id",
  { id: 1 }
)
k.DB.sqlServer.execute(
  "INSERT INTO orders (order_no, amount) VALUES (@no, @amt)",
  { no: "A1001", amt: 99 }
)
const table = k.DB.sqlServer.getTable("orders")
```

When **Default database** under **Site settings → Advanced** is set to **SqlServer**, `k.DB.sql` and `k.DB.sqlServer` point to the same instance.

See **[k.DB](../../api/database/index.md)** (`sqlServer` shares relational APIs with `mysql` / `sqlite`: `query` / `execute` / `getTable` / `procedure`, etc.).

::: tip SQL logs
With site **SQL logging** enabled, statements from `k.DB.sqlServer` can be filtered by type **SQLServer** in [SQL logs](./sql-logs.md).
:::

## Typical workflow

1. Configure **SqlServer** under **Service integrations** and verify connectivity.  
2. Open **SQL Server Tables**, **Create table** or manage existing database tables.  
3. **Settings** for columns and control types; **New data** or **Import CSV**.  
4. Read/write in Code with `k.DB.sqlServer`; check [SQL logs](./sql-logs.md) when troubleshooting.

## Related

| Doc | Description |
|-----|-------------|
| [MySQL tables](./mysql-table.md) | Similar external MySQL |
| [SQLite tables](./sqlite-table.md) | Built-in database; no connection |
| [k.DB](../../api/database/index.md) | Script API |
| [SQL logs](./sql-logs.md) | Execution records |
| [Database overview](./index.md) | Menu overview |
