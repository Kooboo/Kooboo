# Database

> Menu: **Database** group in the left sidebar (must be enabled in **Edit Menu**)  
> Route prefix: `/_Admin/database/...?SiteId={siteGuid}`

The **Database** group manages multiple data stores within a site: **IndexedDB dynamic tables**, table relations, **key-value storage**, and browsing and data maintenance for external **SQLite / MySQL / SQL Server** tables. **SQL Logs** shows SQL records produced by scripts and the admin backend. Scripts access these stores uniformly through [k.DB](../../api/database/index.md) (for SQLite see [k.DB.sqlite](../../api/sqlite/index.md)).

::: tip Difference from Content
| | **Database** | **Content → Content** |
|--|--------------|----------------------|
| Model | Custom table schemas, KV, external DB tables | Content types + content folders |
| Typical use | Business tables, cache keys, integrating existing SQL databases | CMS entries bindable to pages |
| API | `k.DB` | `k.content` |

The two can be used together; do not treat them as one modeling system.
:::

::: info Advanced menu
The Database group and its sub-items are marked **advanced** in Edit Menu. If they are not checked, this group does not appear in the left sidebar.
:::

## How to Open

1. In [Edit Menu](../navigation.md#permissions-and-edit-menu), check **Database** and the sub-items you need.
2. Expand **Database** in the left sidebar.

## Submenus (same order as the admin)

| Menu | Route | Permission (feature) | Doc |
|------|-------|----------------------|-----|
| IndexedDB Tables | `/database/table` | `database` | [IndexedDB tables](./table.md) (this section) |
| IndexedDB Table Relations | `/database/table-relation` | `tableRelation` | [Table relations](./table-relation.md) |
| Key-Value Storage | `/database/key-value` | `keyValue` | [Key-value storage](./key-value.md) |
| SQLite Tables | `/database/sqlite-table` | `database` | [SQLite tables](./sqlite-table.md) |
| MySQL Tables | `/database/mysql-table` | `database` | [MySQL tables](./mysql-table.md) |
| SQL Server Tables | `/database/sqlserver-table` | `database` | [SQL Server tables](./sqlserver-table.md) |
| SQL Logs | `/database/sql-logs` | `database·log` | [SQL logs](./sql-logs.md) |

### Development reference (KScript API)

| Menu | API doc |
|------|---------|
| IndexedDB tables | [k.DB.indexedDb](../../api/indexed-db/index.md) |
| SQLite / MySQL / SQL Server tables | [k.DB](../../api/database/index.md) · [k.DB.sqlite](../../api/sqlite/index.md) |
| Key-value storage | [k.DB](../../api/database/index.md) (KV APIs; see child doc) |
| SQL logs | Read-only observation; no dedicated `k.*` CRUD |

## Edit routes without sidebar menu items

The following pages are opened from table lists. Breadcrumbs still belong to the corresponding table-type menu (`dbType` query parameter highlights **IndexedDB / SQLite / MySQL / SQL Server**):

| Route | Description |
|-------|-------------|
| `/database/table/columns` | Edit table columns (column definitions) |
| `/database/table/data` | Browse and filter table data |
| `/database/table/edit-data` | Create or edit a single row |

Deep-link examples:

```text
/_Admin/database/table/columns?SiteId={siteGuid}&id={tableId}&dbType=IndexedDb
/_Admin/database/table/data?SiteId={siteGuid}&id={tableId}&dbType=Sqlite
```

`dbType` values match the admin (e.g. `IndexedDb`, `Sqlite`, `Mysql`, `SqlServer`); confirm in the address bar.

## Suggested reading order

If you are modeling site data from scratch, a common order is:

1. **[IndexedDB tables](./table.md)** — default dynamic tables within the site  
2. **[Table relations](./table-relation.md)** — multi-table associations (requires existing IndexedDB tables)  
3. **[Key-value storage](./key-value.md)** — simple KV  
4. **SQLite / MySQL / SQL Server** — read as needed depending on external database integration  
5. **[SQL logs](./sql-logs.md)** — troubleshooting and auditing  

## Related

| Doc | Description |
|-----|-------------|
| [Site admin menu overview](../navigation.md) | Full menu tree |
| [Development](../development/index.md) | Calling `k.DB` from Code |
| [k.DB](../../api/database/index.md) | Script API |
| [k.DB.sqlite](../../api/sqlite/index.md) | SQLite queries and table operations |
