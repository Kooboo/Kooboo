# JSON5 条件表达式

> 用于 `<query where="...">` 与 `<when test="...">`

不写 SQL 字符串，使用 JSON5 对象描述过滤与分支条件。

## 支持的操作符

仅支持以下 10 个操作符：

`$and`、`$or`、`$eq`、`$ne`、`$gt`、`$gte`、`$lt`、`$lte`、`$contains`、`$startswith`

## 基础写法

**字段直写**（等价 `$eq`）：

```json5
{ status: 'published' }
```

**显式操作符**：

```json5
{ age: { $gte: 18 } }
```

**组合**：

```json5
{
  $and: [
    { status: { $eq: 'published' } },
    { title: { $contains: keyword } }
  ]
}
```

## 操作符说明

| 操作符 | 含义 | 示例 |
|--------|------|------|
| `$eq` | 等于 | `{ id: { $eq: '123' } }` |
| `$ne` | 不等于 | `{ status: { $ne: 'deleted' } }` |
| `$gt` | 大于 | `{ score: { $gt: 90 } }` |
| `$gte` | 大于等于 | `{ price: { $gte: 100 } }` |
| `$lt` | 小于 | `{ stock: { $lt: 10 } }` |
| `$lte` | 小于等于 | `{ level: { $lte: 3 } }` |
| `$contains` | 包含（常用于字符串） | `{ title: { $contains: 'hello' } }` |
| `$startswith` | 前缀 | `{ code: { $startswith: 'SKU-' } }` |
| `$and` | 全部成立 | `{ $and: [ {...}, {...} ] }` |
| `$or` | 任一成立 | `{ $or: [ {...}, {...} ] }` |

条件里可引用 `<let>` 定义的变量名（如 `keyword`、`min_age`）。

## 在 query 中使用

固定等值条件：

```html
<k-data>
    <query
        as="articles"
        source="content"
        resource="article"
        action="list"
        where="{title:'News'}"
        export
    ></query>
</k-data>
```

条件也可以引用 `<let>` 读取的 URL 参数：

```html
<k-data>
    <let as="keyword" source="queryString" from="keyword" default="''" />
    <query
        as="articles"
        source="content"
        resource="article"
        action="list"
        where="{title:{$contains:keyword}}"
        export
    ></query>
</k-data>
```

使用 `$and` / `$or` 组合多个条件：

```json5
{
  $and: [
    { status: 'published' },
    { title: { $contains: keyword } }
  ]
}
```

## 在 when 中使用

```html
<k-data>
    <let as="status" source="queryString" from="status" default="'draft'" />
    <when test="{status:'published'}">
        <let as="message" default="'Published'" export />
        <else>
            <let as="message" default="'Not published'" export />
        </else>
    </when>
</k-data>
```

## 相关文档

- [标签语法](./tags.md)
- [query 数据源](./query.md)
