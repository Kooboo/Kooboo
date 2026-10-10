# SQL Logs

> Menu: **Database → SQL Logs**  
> Deep link: `/_Admin/database/sql-logs?SiteId={siteGuid}`

**SQL logs** show SQL statements executed by site scripts through **`k.DB.sqlite` / `k.DB.mysql` / `k.DB.sqlServer`**: statement text, parameters, duration, and database type for troubleshooting and auditing. This page is **read-only** observation; it does not delete or edit logs, and there is no corresponding `k.*` write API.

::: tip Permissions
Viewing this page and list data: `database·log` (the **log** action on the `database` feature).  
The **Enable** toggle on the toolbar right saves site settings and usually requires site **edit**-class permissions (same as saving site configuration).
:::

::: warning What is logged
| Logged | Not logged |
|--------|------------|
| SQL from `k.DB.sqlite.query` / `execute`, etc. | [IndexedDB tables](./table.md) (`k.DB.indexedDb`) operations |
| SQL from `k.DB.mysql`, `k.DB.sqlServer` | [Key-value storage](./key-value.md) (`k.DB.keyValue`) |
| SQL from admin **SQLite / MySQL / SQL Server** maintenance when routed through the same executor (if applicable) | Any SQL when **SQL logging** is disabled |

You must enable **SQL logging** on the site first (see [Enable SQL logging](#enable-sql-logging)); only SQL produced after that is written to the log.
:::

## How to Open

1. In [Edit Menu](../navigation.md#permissions-and-edit-menu), check **Database → SQL Logs**.
2. Open **Database → SQL Logs** in the left sidebar.

<DocImage src="/cms/database/sql-logs-overview.png" alt="SQL logs page" width="1120" />

## Enable SQL logging

Site-level switch **`enableSqlLog`**; both locations are equivalent (changing one is enough):

| Location | Path |
|----------|------|
| **This page toolbar** | **Enable** toggle on the right; saves the site immediately |
| **Site settings → Advanced** | `/_Admin/system/settings?SiteId={siteGuid}&name=advance`, **SQL logging** item |

When disabled, new script SQL is **not** written to the log; existing historical weekly data can still be viewed by week.

<DocImage src="/cms/database/sql-logs-toolbar.png" alt="SQL logs filters and enable toggle" width="1120" />

## Filters and search

Toolbar left to right:

| Control | Description |
|---------|-------------|
| **Week** | Select log file by **calendar week**; option text is `yyyy-MM-dd~yyyy-MM-dd` (week start/end). Only weeks where **the current site has entries** are listed; if none yet, defaults to **current week** |
| **Type** | Optional **SQLite**, **MySQL**, **SQLServer** (maps to `sqlite` / `mysql` / `sqlserver`); clear for all types |
| **Keyword** | Fuzzy match in **SQL body** (case-insensitive); ~1 second debounce after input; clear to refresh |
| **Enable** | See [Enable SQL logging](#enable-sql-logging) |

Changing **Week**, **Type**, or keyword re-requests `SqlLog/List` and returns to page 1.

## Log list

Default pagination **50** rows per page (`KTable` pagination at bottom).

| Column | Description |
|--------|-------------|
| **Type** | Database source, e.g. `sqlite`, `mysql`, `sqlserver` |
| **Date time** | Execution time (site timezone) |
| **Execution time** | Duration in **ms** |
| **Content** | SQL statement summary (truncated in table when long) |
| **Parameters** | Serialized parameter object; empty when none or `null` |
| Row **Details** | Opens [details dialog](#details-dialog) when data exists |

When the week has no records, the table shows **No data** (logging must be **enabled** and the site must have executed `k.DB.sqlite` etc.).

<DocImage src="/cms/database/sql-logs-list.png" alt="SQL log list" width="1120" />

### Details dialog

Click the row **eye** icon. Shows full **Type**, **Date time**, **Execution time**, and two full text blocks:

1. **SQL statement** (with line breaks)  
2. **Parameters** (JSON or other serialized form)

Use to copy the full statement or compare parameters in slow-query scenarios.

## How logs are produced

When scripts access relational databases and `EnableSqlLog` is true, the executor asynchronously writes to server log files partitioned by **week** (under site `AppData` `SqlLog`, filenames like `{yearWeek}.log`). Writes and list queries filter by **site ID**; you only see this site’s records.

Example (after logging is enabled):

```typescript
k.DB.sqlite.query("SELECT * FROM logs WHERE id = @id", { id: 1 })
k.DB.sqlite.execute("INSERT INTO logs (msg) VALUES (@msg)", { msg: "ok" })
```

For MySQL / SQL Server, configure connections under [Service integrations](../navigation.md) (`/system/config`), then execute via `k.DB.mysql` / `k.DB.sqlServer` to see logs of the matching **type**.

::: info Admin table maintenance
Whether [SQLite tables](./sqlite-table.md) list/column/data **admin APIs** share the same execution chain as script `k.DB.sqlite` depends on the environment. When troubleshooting script SQL, rely on **Type** and **Content** on this page; if admin actions produce no log, confirm execution goes through `k.DB.*` and logging is **enabled**.
:::

## Typical workflow

1. Turn on **SQL logging** under **Advanced** or on this page.  
2. Reproduce the issue (page or job that runs `k.DB.sqlite`, etc.).  
3. Select the **week**, narrow by **type** / **keyword**.  
4. Open **Details**, copy SQL and parameters, verify in Code or a database tool.  
5. Long-term enablement in production may increase I/O and disk use; turn off **Enable** after troubleshooting.

## Related

| Doc | Description |
|-----|-------------|
| [Database overview](./index.md) | Menus and permissions |
| [SQLite tables](./sqlite-table.md) | Built-in SQLite and CSV |
| [k.DB.sqlite](../../api/sqlite/index.md) | Script SQL API |
| [k.DB](../../api/database/index.md) | Database overview |
