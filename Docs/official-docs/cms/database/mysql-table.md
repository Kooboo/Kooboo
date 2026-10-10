# MySQL Tables

> Menu: **Database → MySQL Tables**  
> Deep link: `/_Admin/database/mysql-table?SiteId={siteGuid}`

**MySQL tables** let you browse and manage tables in an connected **external MySQL** database from the admin: create tables, maintain columns and data, and **import / export CSV**. Configure the **Mysql** connection string under [Service integrations](../navigation.md) first; scripts use **`k.DB.mysql`** for SQL or table objects. Child routes must include **`dbType=MySql`**; the left menu highlights **MySQL Tables**.

::: tip Permissions
Same as [SQLite tables](./sqlite-table.md): `database` (view), `database·edit`, `database·delete`.  
Configuring the connection string requires **Service integrations** `config·edit`.
:::

::: info Difference from SQLite tables
| | **MySQL tables** | **SQLite tables** |
|--|------------------|-------------------|
| Data source | External MySQL (connection string) | Site built-in SQLite file |
| Prerequisite | [Service integrations](#configure-mysql-connection) **Database → Mysql** | No configuration |
| When not configured | Guided page + **Configure now** | Lists tables directly |
| Column / data UI | Same as SQLite (no primary key/unique columns; has CSV) | Same |
| Script | `k.DB.mysql` | `k.DB.sqlite` |
| SQL log type | `mysql` | `sqlite` |

Column editing and data browsing share `views/database/table/*` routes with IndexedDB / SQLite.
:::

## Configure MySQL connection

1. Open **Site settings → Service integrations**: `/_Admin/system/config?SiteId={siteGuid}`  
2. Expand the **Database** group and edit **Mysql**. See [Service integrations · Database](../settings/integrations/database.md).  
3. Enter the **connection string** (ConnectionString) and save.

Example format (follow Kooboo built-in guidance):

```text
Server=myServerAddress;Port=3306;Database=myDataBase;Uid=myUsername;Pwd=myPassword;
```

You can also click **Configure now** on the **MySQL Tables** unconfigured guide page to jump to `/_Admin/system/config?SiteId={siteGuid}&group=Database` (Database group expanded automatically).

::: warning Security
Connection strings contain credentials; do not commit them to public repositories. Use least-privilege database users in production.
:::

## How to Open

1. In [Edit Menu](../navigation.md#permissions-and-edit-menu), check **Database → MySQL Tables**.  
2. Complete [Configure MySQL connection](#configure-mysql-connection).  
3. Open **Database → MySQL Tables** in the left sidebar.

When not configured or the connection is invalid, the page shows an error and **Configure now**; no table list is shown.

<DocImage src="/cms/database/mysql-table-unconfigured.png" alt="MySQL unconfigured guide" width="1120" />

After a successful connection, list, columns, data, CSV import, and other UIs **share the same pages** as [SQLite tables](./sqlite-table.md) (only `dbType=MySql` and menu highlight differ). Screenshots below use SQLite as an example; MySQL looks the same once configured.

## Table List

| Action | Permission | Description |
|--------|------------|-------------|
| **Create table** | `database·edit` | Create table in MySQL and register admin schema |
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
/_Admin/database/table/columns?SiteId={siteGuid}&table={tableName}&dbType=MySql
```

Same as [SQLite column settings](./sqlite-table.md#column-settings): list shows **column name**, **control type**, **index** (no primary key/unique columns); column editor has no **length**, **auto-increment**, or **primary key/unique** (IndexedDB only). Save updates MySQL table structure and admin control mapping.

<DocImage src="/cms/database/sqlite-table-columns.png" alt="Column settings (same UI as SQLite tables)" width="1120" />

<DocImage src="/cms/database/sqlite-table-column-dialog.png" alt="Column editor dialog" width="1120" />

## Table Data

Route example:

```text
/_Admin/database/table/data?SiteId={siteGuid}&table={tableName}&dbType=MySql
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
/_Admin/database/table/edit-data?SiteId={siteGuid}&table={tableName}&dbType=MySql
```

Form rendered by column **control type**; **Save** / **Cancel** returns to the data list.

## Script access (development)

```typescript
const rows = k.DB.mysql.query(
  "SELECT * FROM orders WHERE id = @id",
  { id: 1 }
)
k.DB.mysql.execute(
  "INSERT INTO orders (order_no, amount) VALUES (@no, @amt)",
  { no: "A1001", amt: 99 }
)
const table = k.DB.mysql.getTable("orders")
```

When **Default database** under **Site settings → Advanced** is set to **Mysql**, `k.DB.sql` and `k.DB.mysql` point to the same instance.

See **[k.DB](../../api/database/index.md)** (`mysql` and `sqlite` share relational APIs: `query` / `execute` / `getTable`, etc.).

::: tip SQL logs
With site **SQL logging** enabled, statements from `k.DB.mysql` can be filtered by type **MySQL** in [SQL logs](./sql-logs.md).
:::

## Typical workflow

1. Configure **Mysql** under **Service integrations** and verify connectivity.  
2. Open **MySQL Tables**, **Create table** or sync existing database tables (tables already in the database appear in the list).  
3. **Settings** for columns and control types; **New data** or **Import CSV**.  
4. Read/write in Code with `k.DB.mysql`; check [SQL logs](./sql-logs.md) when troubleshooting.

## Related

| Doc | Description |
|-----|-------------|
| [SQLite tables](./sqlite-table.md) | Built-in database; no connection |
| [SQL Server tables](./sqlserver-table.md) | Similar external database |
| [k.DB](../../api/database/index.md) | Script API |
| [SQL logs](./sql-logs.md) | Execution records |
| [Database overview](./index.md) | Menu overview |
