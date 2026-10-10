# Query Sources

> How `<query>` combines `source`, `resource`, and `action`.

For common attributes, see [Tag Syntax](./tags.md#query). The sections below are organized by data source. In all cases, `action` is one of `item`, `list`, or `paged`.

::: warning Example convention
`<query>` must be inside `<k-data>` and must include an explicit `</query>` end tag. Add the `export` attribute to a `<query>` whose result is used by the template. Unless an example explicitly includes `<k-data>`, a standalone `<query>` block below is only a query fragment that demonstrates source-specific attributes.
:::

## `content`

`resource` is the **content type name** (Content Folder).

### `list`

The following is a complete list-query example that you can copy:

```html
<k-data>
    <query
        as="articles"
        source="content"
        resource="article"
        action="list"
        export
    ></query>
</k-data>

<ul>
    <li k-for="article in articles">
        <span k-content="article.title"></span>
    </li>
</ul>
```

### Add field selection and a fixed filter

Use `select` to limit returned fields and `where` to filter results:

```html
<k-data>
    <query
        as="articles"
        source="content"
        resource="article"
        action="list"
        select="id,title"
        where="{title:'News'}"
        export
    ></query>
</k-data>
```

### `item`

Query one item from a URL parameter. `item` requires an `id`. This example reads the parameter from `/detail?id=...` before running the query:

```html
<k-data>
    <let as="article_id" source="queryString" from="id" />
    <query
        as="article"
        source="content"
        resource="article"
        action="item"
        id="article_id"
        select="id,title,body"
        export
    ></query>
</k-data>

<article k-if="article">
    <h1 k-content="article.title"></h1>
    <div k-content="article.body"></div>
</article>
```

### `paged`

Use `action="paged"` for a paged query:

```html
<k-data>
    <query
        as="article_page"
        source="content"
        resource="article"
        action="paged"
        page-number="1"
        page-size="10"
        order-desc="create_time"
        export
    ></query>
</k-data>

<div k-for="article in article_page.list">
    <h2 k-content="article.title"></h2>
</div>
<p>
    Total: <span k-content="article_page.total"></span>,
    page <span k-content="article_page.pageIndex"></span> /
    <span k-content="article_page.pageCount"></span>
</p>
```

A paged result contains:

| Field | Description |
|-------|-------------|
| `list` | Array of records for the current page |
| `total` | Total record count |
| `pageIndex` | Current page number |
| `pageSize` | Page size |
| `pageCount` | Total page count |

## `commerce`

`resource` only supports these fixed values: `product`, `category`, `cart`, and `order`.

### `product`

| action | Description |
|--------|-------------|
| `item` | `id` is a product id or **seoName** |
| `list` / `paged` | `where` supports fields such as `id`, `title`, `featuredImage`, `active`, `seoName`, and `tags`. `where.category` accepts a category id or seoName and includes child categories |

```html
<query
    as="products"
    source="commerce"
    resource="product"
    action="list"
    where="{category:'men-shirt'}"
></query>
```

### `category`

| action | Description |
|--------|-------------|
| `item` | `id` is a category id or seoName |
| `list` / `paged` | `where` supports fields such as `id`, `title`, `seoName`, `parentId`, `active`, and `tags` |

### `cart`

- Only supports **`action="item"`**
- `id` is optional and defaults to the current-context cart id

```html
<query as="cart_detail" source="commerce" resource="cart" action="item"></query>
```

### `order`

| action | Restriction |
|--------|-------------|
| `item` | Requires `id` |
| `list` / `paged` | Does **not** support `where`, `order`, or `order-desc`; results are sorted by create time descending |

## `sqlite` / `mysql` / `sqlserver`

These three sources share the same `<query>` attributes. `resource` is the **table name**.

```html
<k-data>
    <let as="record_id" source="queryString" from="id" />
    <query
        as="payment_record"
        source="sqlite"
        resource="payment_records"
        action="item"
        id="record_id"
        select="id,amount,status"
        export
    ></query>
</k-data>
```

```html
<query
    as="user_paged"
    source="mysql"
    resource="users"
    action="paged"
    select="id,name,email"
    where="{name:{$contains:'tom'}}"
    page-number="1"
    page-size="10"
    order="name"
></query>
```

## `indexeddb`

Similar to a database query. `resource` is the object store name.

```html
<query
    as="draft_paged"
    source="indexeddb"
    resource="drafts"
    action="paged"
    where="{status:{$eq:'published'}}"
    page-number="1"
    page-size="10"
    order-desc="id"
></query>
```

## `culture`

Used for multilingual links. Set `source="culture"`. `resource` is the **relative path** to inspect. If omitted, the current request path is used.

### `item`

- `id`: culture code. If omitted, the current-context culture is used.
- Returns `{ key, name, url, isActive }` or `null`

```html
<k-data>
    <let as="lang" source="queryString" from="lang" />
    <query
        as="current_culture"
        source="culture"
        resource="/products/detail"
        action="item"
        id="lang"
        export
    ></query>
</k-data>

<a k-attribute="href {current_culture.url}" k-content="current_culture.name"></a>
```

### `list`

Returns an array of the same structure, which is useful for a language switcher.

```html
<k-data>
    <query export as="culture_urls" source="culture" resource="/products/detail" action="list"></query>
</k-data>

<div k-for="item in culture_urls">
    <a k-attribute="href {item.url}" k-content="item.name"></a>
</div>
```

## Related

- [k-data Overview](./)
- [Condition Expressions](./condition.md) - `where` and `test`
- [k.content](../../api/content/index.md) and [commerce](../../api/commerce/commerce.md) - KScript APIs
