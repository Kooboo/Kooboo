# Basic · Advanced Tab

> Tab: **Advanced** · `name=advance`  
> Deep link: `/_Admin/system/settings?SiteId={siteGuid}&name=advance`

Low-level switches for development and operations: default relational database, `k.DB.sql` target, **SQL log** and **code log**, constraint fix on save, **Hook** master switches, resumable downloads, and site **custom key-value** settings.

<DocImage src="/cms/settings/settings-basic-advanced.png" alt="Advanced tab" width="1120" />

## Default Database

**Default database** (`defaultDatabase`) decides where **`k.DB.sql`** resolves:

| Value | Description |
|-------|-------------|
| **Auto** | Priority **MySQL > SQL Server > SQLite** (connections must be set under [Integrations](../integrations/)) |
| **Sqlite** / **Mysql** / **SqlServer** | Fixed target |

See [k.DB](../../../api/database/index.md) and [Database](../../database/index.md) table docs.

## SQL Log

**SQL log** (`enableSqlLog`): equivalent to the switch on [Database -> SQL Logs](../../database/sql-logs.md); controls logging for `k.DB.sqlite` / `mysql` / `sqlServer`.

## Code Log

Group **Code log** (`codeLogSettings`):

| Field | Description |
|-------|-------------|
| **Enable** | Record KScript code logs |
| **Level** | Debug / Information / Warning / Error / Critical |
| **Retention days** | `keepDays` (≥1) |

List queries: [Development -> Code log](../../development/code-log.md).

## Constraints and Other Switches

| Switch | Description |
|--------|-------------|
| **Constraint fix on save** | `enableConstraintFixOnSave`: auto-fix some constraint conflicts when saving content and other resources |
| **Request Hooks** | `enableFrontEvents`: enable [Request Hooks](../request-hooks.md) (rules still configured on the Hook page) |
| **Action Hooks** | `enableBackendEvents`: enable [Action Hooks](../action-hooks.md) |
| **Resumable download** | `continueDownload`: large file resume (shown in some environments) |

## Custom Settings

**Custom settings** (`customSettings`): key-value editor for site or module extension config (`KeyValueEditor`). Keys must follow product conventions; scripts read via site API or agreed names—do not confuse with [Key-value storage](../../database/key-value.md) (`k.DB.keyValue`).

## Related

- [Basic Settings overview](./index.md)  
- [SQL logs](../../database/sql-logs.md)  
- [Integrations](../integrations/) (Database connections)
