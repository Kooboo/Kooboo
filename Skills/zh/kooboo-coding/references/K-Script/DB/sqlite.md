# SQLite 指南

`k.DB.sqlite` 访问站点内置的 SQLite 数据库。数据库路径由运行时按站点创建，KScript 不需要配置连接字符串。

## 适用场景

- 存储或查询不属于 Commerce 或 CMS 内容的站点本地关系型数据。
- 需要使用 SQL 的筛选、排序、聚合、连接或显式表结构。
- API 层只处理业务流程，将数据库访问封装到 Service/Utils 模块中。

Commerce 数据优先使用 `k.commerce.*`，CMS 内容优先使用 `k.content.*`。AI App 的表结构准备应在对应工作流中使用数据库准备 Functions；下面的 KScript 示例说明运行时用法和显式 SQL 初始化方式。

## SQL 方法

### getTables()

从 SQLite 的 `sqlite_master` 表读取并返回表名数组。

```ts
return k.DB.sqlite.getTables()
```

### query(sql, params?)

执行查询并返回行对象数组。没有结果时返回 `[]`。

```ts
const rows = k.DB.sqlite.query(
  "SELECT id, name FROM users WHERE status = @status ORDER BY id DESC",
  { status: 1 }
)
return rows
```

### get(sql, params?)

执行查询并返回第一行；没有结果时返回 `null`。

```ts
const user = k.DB.sqlite.get(
  "SELECT id, name FROM users WHERE id = @id",
  { id: userId }
)
```

### value(sql, params?)

执行查询并返回第一行第一列；没有结果时返回 `null`。

```ts
const total = k.DB.sqlite.value("SELECT COUNT(*) FROM users") || 0
```

### execute(sql, params?)

执行非查询语句，并返回数据库驱动报告的受影响行数。它不是通用的成功布尔值：`CREATE` 可能返回 `0`，而 `INSERT`、`UPDATE` 和 `DELETE` 的结果取决于具体操作。

```ts
const affected = k.DB.sqlite.execute(
  "UPDATE users SET status = @status WHERE id = @id",
  { status: 1, id: userId }
)
```

### transaction(action)

在事务中执行回调，回调正常完成后提交。如果回调抛出异常，异常会继续向外抛出，事务不会提交。

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

## 参数绑定

使用带命名参数的对象，并在 SQL 中使用 `@name` 占位符。参数名可以带或不带开头的 `@`；运行时会绑定参数，不要把值直接拼接到 SQL 中。

```ts
const ids = [1, 2, 3]
const rows = k.DB.sqlite.query(
  "SELECT id, name FROM users WHERE id IN (@ids)",
  { ids }
)
```

数组参数支持 `@ids` 和 `IN (@ids)` 展开；空数组会展开为不匹配任何行的条件。不要把用户输入拼接进 SQL。表名和列名不能作为参数绑定；必须拼接时，先使用白名单校验标识符。

## 表包装器

`getTable(name)` 返回惰性表包装器，不会检查物理表是否已存在，因此 `!!k.DB.sqlite.getTable(name)` 不能用来判断表是否存在。

通过表包装器调用 `add`、`all`、`find`、`query` 等操作时，缺失的表会被初始化。`add` 还可以根据对象推断并增加列，并为默认表结构生成 `_id`。如果表结构必须受控并可审阅，应使用显式 `CREATE TABLE` SQL。

```ts
const users = k.DB.sqlite.getTable("users")
const id = users.add({ name: "A", status: 1 })
const user = users.get(id)
```

表包装器适合简单的动态表。对于稳定的应用表结构，应优先使用显式 SQL 或 App 数据库准备流程，不要让普通业务请求隐式演进表结构。

## 显式初始化

下面的语句只在表不存在时创建表，不会校验或升级已有表结构。

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

## 分页与失败处理

验证请求范围后，使用绑定的 `LIMIT` 和 `OFFSET`。数据库异常会从 KScript 继续向外抛出，因此应在 API 或 Service 边界捕获，并返回站点既有的错误结构。

```ts
const page = Math.max(1, Number(k.request.queryString.get("page")) || 1)
const pageSize = Math.min(100, Math.max(1, Number(k.request.queryString.get("pageSize")) || 20))
const items = k.DB.sqlite.query(
  "SELECT id, level, message FROM logs ORDER BY id DESC LIMIT @pageSize OFFSET @offset",
  { pageSize, offset: (page - 1) * pageSize }
)
return { page, pageSize, items }
```

## 相关文档

- [DB 模块导航](index.md)
- [KScript 索引](../index.md)
- [数据层指南](../../Backend/data-layer-guide.md)
