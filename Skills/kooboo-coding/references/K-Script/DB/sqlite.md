# SQLite Guide

`k.DB.sqlite` accesses the site's built-in SQLite database. The site-specific database path is created by the runtime; no connection string is required in KScript.

## When to use

- Store or query site-local relational data that is not Commerce or CMS content.
- Use SQL filtering, sorting, aggregation, joins, or explicit schema definitions.
- Keep database access behind a Service/Utils module when the API layer only needs business operations.

Prefer `k.commerce.*` for commerce data and `k.content.*` for CMS content. For AI App schema preparation, use the database preparation Functions when that workflow is active; the KScript examples below are runtime usage and explicit SQL initialization.

## SQL methods

### getTables()

Returns table names read from SQLite's `sqlite_master` table.

```ts
return k.DB.sqlite.getTables()
```

### query(sql, params?)

Runs a query and returns an array of row objects. It returns `[]` when there are no rows.

```ts
const rows = k.DB.sqlite.query(
  "SELECT id, name FROM users WHERE status = @status ORDER BY id DESC",
  { status: 1 }
)
return rows
```

### get(sql, params?)

Runs a query and returns the first row, or `null` when there are no rows.

```ts
const user = k.DB.sqlite.get(
  "SELECT id, name FROM users WHERE id = @id",
  { id: userId }
)
```

### value(sql, params?)

Runs a query and returns the first column of the first row, or `null` when there are no rows.

```ts
const total = k.DB.sqlite.value("SELECT COUNT(*) FROM users") || 0
```

### execute(sql, params?)

Runs a non-query statement and returns the provider's affected-row count. The value is not a general success boolean: `CREATE` statements can return `0`, while `INSERT`, `UPDATE`, and `DELETE` depend on the database operation.

```ts
const affected = k.DB.sqlite.execute(
  "UPDATE users SET status = @status WHERE id = @id",
  { status: 1, id: userId }
)
```

### transaction(action)

Runs the callback inside a transaction and commits when the callback completes. If the callback throws, the exception propagates and the transaction is not committed.

```ts
k.DB.sqlite.transaction(() => {
  k.DB.sqlite.execute(
    "INSERT INTO users(name) VALUES(@name)",
    { name: "A" }
  )
  k.DB.sqlite.execute(
    "INSERT INTO users(name) VALUES(@name)",
    { name: "B" }
  )
})
```

## Parameters

Use an object with named parameters and `@name` placeholders. Parameter names may be supplied with or without the leading `@`; values are bound rather than interpolated into SQL.

```ts
const ids = [1, 2, 3]
const rows = k.DB.sqlite.query(
  "SELECT id, name FROM users WHERE id IN (@ids)",
  { ids }
)
```

Array parameters are expanded for `@ids` and `IN (@ids)`. An empty array expands to a condition that matches no rows. Do not concatenate user input into SQL. Table and column identifiers cannot be bound as parameters; validate them against an allowlist before constructing SQL.

## Table wrapper

`getTable(name)` returns a lazy table wrapper; it does not check whether the physical table already exists, so `!!k.DB.sqlite.getTable(name)` is not an existence test.

Operations such as `add`, `all`, `find`, and `query` initialize a missing table through the table wrapper. `add` can also add columns inferred from the object and generates an `_id` value for the default schema. Use explicit `CREATE TABLE` SQL when the schema must be controlled and reviewed.

```ts
const users = k.DB.sqlite.getTable("users")
const id = users.add({ name: "A", status: 1 })
const user = users.get(id)
```

The table wrapper is useful for simple dynamic tables. For stable application schemas, prefer explicit SQL or the App database-preparation workflow rather than allowing business requests to evolve the schema implicitly.

## Explicit initialization

This creates a table only when it does not already exist; it does not validate or upgrade an existing schema.

```ts
k.DB.sqlite.execute(`
  CREATE TABLE IF NOT EXISTS logs (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    level TEXT,
    message TEXT,
    createdAt TEXT
  )
`)
```

## Pagination and failure handling

Use bound `LIMIT` and `OFFSET` values after validating the requested range. Database errors propagate from KScript, so catch them at the API or Service boundary and return the site's established error shape.

```ts
const page = Math.max(1, Number(k.request.queryString.get("page")) || 1)
const pageSize = Math.min(100, Math.max(1, Number(k.request.queryString.get("pageSize")) || 20))
const items = k.DB.sqlite.query(
  "SELECT id, level, message FROM logs ORDER BY id DESC LIMIT @pageSize OFFSET @offset",
  { pageSize, offset: (page - 1) * pageSize }
)
return { page, pageSize, items }
```

## Related documents

- [DB module navigation](index.md)
- [KScript index](../index.md)
- [Data Layer Guide](../../Backend/data-layer-guide.md)
