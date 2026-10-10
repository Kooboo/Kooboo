# k.DB.sqlite

> SQLite database operations - lightweight database that requires no configuration

## Overview

`k.DB.sqlite` provides SQLite database access capabilities out of the box, without the need to manually configure the connection string. Commonly used for lightweight data storage, query and scripted data processing within the site.

## TypeScript Definition

```ts
interface SQLiteDB {
    getTables(): string[];
    getTable(name: string): ITable;
    query(sql: string, params?: object): any[];
    execute(sql: string, params?: object): number;
    operators(): Operators;
    transaction(action: Function): void;
}
```

## Methods

### getTables()

Returns all table names in the current SQLite database.

**Parameters**: none.

**return**

- `string[]` table name array

```ts
k.api.get(() => {
    const tables = k.DB.sqlite.getTables()
    return { tables }
})
```

### getTable(name)

Get the specified table object (`ITable`) for chain query or table-level operations.

**parameter**

- `name: string` table name

**return**

- `ITable` table object

```ts
k.api.get(() => {
    const userTable = k.DB.sqlite.getTable("users")
    return { hasTable: !!userTable }
})
```

### query(sql, params?)

Execute the SQL query statement and return the array result (`any[]`).

**parameter**

- `sql: string` SQL query statement
- `params?: object` optional parameter object, bound using `@name` placeholder

**return**

- `any[]` Query result array; if there is no match, an empty array is returned `[]`

```ts
k.api.get(() => {
    const result = k.DB.sqlite.query("SELECT 1 as id, 'hello' as name")
    return { result }
})
// Returns: [{ "id": 1, "name": "hello" }]
```

**Query with parameters:**

```ts
k.api.get(() => {
    const users = k.DB.sqlite.query(
        "SELECT * FROM users WHERE status = @status",
        { status: 1 }
    )
    return { users }
})
```

### execute(sql, params?)

Execute write SQL (such as `CREATE`, `INSERT`, `UPDATE`, `DELETE`). Returns a numeric result, usually `1` for success.

**parameter**

- `sql: string` Write operation SQL statement
- `params?: object` optional parameter object, bound using `@name` placeholder

**return**

- `number` execution status value (successfully returns `1` in the example)

```ts
k.api.post(() => {
    // Create table
    k.DB.sqlite.execute("CREATE TABLE IF NOT EXISTS users (id INTEGER PRIMARY KEY, name TEXT)")

    // Insert data
    k.DB.sqlite.execute(
        "INSERT INTO users (name) VALUES (@name)",
        { name: "kooboo" }
    )

    return { success: true }
})
```

### operators()

Returns a set of query condition operators (`Operators`), used to dynamically construct conditional expressions.

**Parameters**: none.

**return**

- `Operators` query condition operator set

```ts
k.api.get(() => {
    const ops = k.DB.sqlite.operators()
    return { hasEqual: !!ops.eq }
})
```

### transaction(action)

A set of database operations is performed within a transaction, with statements within `action` running within the same transaction context.

**parameter**

- `action: Function` callback function to execute within the transaction

**return**

- `void`

```ts
k.api.post(() => {
    k.DB.sqlite.transaction(() => {
        k.DB.sqlite.execute(
            "INSERT INTO users(name) VALUES (@name)",
            { name: "A" }
        )
        k.DB.sqlite.execute(
            "INSERT INTO users(name) VALUES (@name)",
            { name: "B" }
        )
    })

    return { success: true }
})
```

**UPDATE Example:**

```ts
k.api.post(() => {
    const result = k.DB.sqlite.execute(
        "UPDATE users SET name = @name WHERE id = @id",
        { name: "updated", id: 1 }
    )
    return { result } // Usually returns 1 on success
})
```

**DELETE Example:**

```ts
k.api.post(() => {
    const result = k.DB.sqlite.execute(
        "DELETE FROM users WHERE id = @id",
        { id: 1 }
    )
    return { result } // Usually returns 1 on success
})
```

## Common usage

### Initialize table structure

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

### Pagination query example

```ts
k.api.get(() => {
    const page = 1
    const size = 20
    const offset = (page - 1) * size

    const items = k.DB.sqlite.query(
        "SELECT id, level, message, createdAt FROM logs ORDER BY id DESC LIMIT @size OFFSET @offset",
        { size, offset }
    )

    return { page, size, items }
})
```

## Notes

1. **Parameterized query**: Use `@param` placeholder to avoid string concatenation SQL
2. **Create the table first and then operate**: Make sure the table structure has been created before executing business SQL
3. **Return value semantics**: `query()` returns an array; `execute()` returns a numerical status
4. **Exception handling**: It is recommended to use `try/catch` to wrap database calls in API processing logic

## Related Docs

- [k.DB](../database/) - Database overview and entry
- [CMS: SQLite Tables](../../cms/database/sqlite-table.md) - Backend table structure and CSV import
- [k.DB.indexedDb](../indexed-db/) - IndexedDB dynamic table
- [k.content](../content/) - Content Management API
