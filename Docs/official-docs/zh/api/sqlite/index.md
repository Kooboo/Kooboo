# k.DB.sqlite

> SQLite 数据库操作 - 无需配置的轻量级数据库

## 概述

`k.DB.sqlite` 提供 SQLite 数据库访问能力，开箱即用，无需手动配置连接字符串。常用于站点内轻量数据存储、查询与脚本化数据处理。

## TypeScript 定义

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

## 方法

### getTables()

返回当前 SQLite 数据库中的所有表名。

**参数**：无。

**返回**

- `string[]` 表名数组

```ts
k.api.get(() => {
    const tables = k.DB.sqlite.getTables()
    return { tables }
})
```

### getTable(name)

获取指定表对象（`ITable`），用于链式查询或表级操作。

**参数**

- `name: string` 表名

**返回**

- `ITable` 表对象

```ts
k.api.get(() => {
    const userTable = k.DB.sqlite.getTable("users")
    return { hasTable: !!userTable }
})
```

### query(sql, params?)

执行 SQL 查询语句，返回数组结果（`any[]`）。

**参数**

- `sql: string` SQL 查询语句
- `params?: object` 可选参数对象，使用 `@name` 占位符绑定

**返回**

- `any[]` 查询结果数组；无匹配时返回空数组 `[]`

```ts
k.api.get(() => {
    const result = k.DB.sqlite.query("SELECT 1 as id, 'hello' as name")
    return { result }
})
// 返回: [{ "id": 1, "name": "hello" }]
```

**带参数查询：**

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

执行写操作 SQL（如 `CREATE`、`INSERT`、`UPDATE`、`DELETE`）。返回数字结果，通常 `1` 表示成功。

**参数**

- `sql: string` 写操作 SQL 语句
- `params?: object` 可选参数对象，使用 `@name` 占位符绑定

**返回**

- `number` 执行状态数值（示例中成功返回 `1`）

```ts
k.api.post(() => {
    // 创建表
    k.DB.sqlite.execute("CREATE TABLE IF NOT EXISTS users (id INTEGER PRIMARY KEY, name TEXT)")

    // 插入数据
    k.DB.sqlite.execute(
        "INSERT INTO users (name) VALUES (@name)",
        { name: "kooboo" }
    )

    return { success: true }
})
```

### operators()

返回查询条件运算符集合（`Operators`），用于动态构建条件表达式。

**参数**：无。

**返回**

- `Operators` 查询条件运算符集合

```ts
k.api.get(() => {
    const ops = k.DB.sqlite.operators()
    return { hasEqual: !!ops.eq }
})
```

### transaction(action)

在事务中执行一组数据库操作，`action` 内的语句在同一事务上下文中运行。

**参数**

- `action: Function` 事务中执行的回调函数

**返回**

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

**UPDATE 示例：**

```ts
k.api.post(() => {
    const result = k.DB.sqlite.execute(
        "UPDATE users SET name = @name WHERE id = @id",
        { name: "updated", id: 1 }
    )
    return { result } // 成功时通常返回 1
})
```

**DELETE 示例：**

```ts
k.api.post(() => {
    const result = k.DB.sqlite.execute(
        "DELETE FROM users WHERE id = @id",
        { id: 1 }
    )
    return { result } // 成功时通常返回 1
})
```

## 常见用法

### 初始化表结构

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

### 分页查询示例

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

## 注意事项

1. **参数化查询**：使用 `@param` 占位符，避免字符串拼接 SQL
2. **先建表再操作**：业务 SQL 执行前应确保表结构已创建
3. **返回值语义**：`query()` 返回数组；`execute()` 返回数值状态
4. **异常处理**：建议在 API 处理逻辑中使用 `try/catch` 包裹数据库调用

## 相关文档

- [k.DB](../database/) - 数据库总览与入口
- [CMS：SQLite 表](../../cms/database/sqlite-table.md) - 后台表结构与 CSV 导入
- [k.DB.indexedDb](../indexed-db/) - IndexedDB 动态表
- [k.content](../content/) - 内容管理 API
