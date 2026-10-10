# JSON5 Condition Expressions

> Used by `<query where="...">` and `<when test="...">`.

Instead of SQL-like strings, use JSON5 objects to describe filtering and branching conditions.

## Supported Operators

Only these 10 operators are supported:

`$and`, `$or`, `$eq`, `$ne`, `$gt`, `$gte`, `$lt`, `$lte`, `$contains`, `$startswith`

## Basic Forms

Field shorthand, equivalent to `$eq`:

```json5
{ status: 'published' }
```

Explicit operator:

```json5
{ age: { $gte: 18 } }
```

Combined form:

```json5
{
  $and: [
    { status: { $eq: 'published' } },
    { title: { $contains: keyword } }
  ]
}
```

## Operator Reference

| Operator | Meaning | Example |
|----------|---------|---------|
| `$eq` | Equals | `{ id: { $eq: '123' } }` |
| `$ne` | Not equal | `{ status: { $ne: 'deleted' } }` |
| `$gt` | Greater than | `{ score: { $gt: 90 } }` |
| `$gte` | Greater than or equal | `{ price: { $gte: 100 } }` |
| `$lt` | Less than | `{ stock: { $lt: 10 } }` |
| `$lte` | Less than or equal | `{ level: { $lte: 3 } }` |
| `$contains` | Contains, often for strings | `{ title: { $contains: 'hello' } }` |
| `$startswith` | Prefix match | `{ code: { $startswith: 'SKU-' } }` |
| `$and` | All conditions must match | `{ $and: [ {...}, {...} ] }` |
| `$or` | Any condition may match | `{ $or: [ {...}, {...} ] }` |

Conditions can reference variables defined by `<let>`, such as `keyword` or `min_age`.

## Using Conditions in `query`

A fixed equality condition:

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

A condition can also reference a URL parameter defined by `<let>`:

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

Use `$and` or `$or` to combine multiple conditions:

```json5
{
  $and: [
    { status: 'published' },
    { title: { $contains: keyword } }
  ]
}
```

## Using Conditions in `when`

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

## Related

- [Tag Syntax](./tags.md)
- [Query Sources](./query.md)
