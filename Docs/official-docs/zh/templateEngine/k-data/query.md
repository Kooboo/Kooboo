# query 数据源

> `<query>` 的 `source`、`resource`、`action` 组合说明

通用属性见 [标签语法](./tags.md#query)。下文按数据源分类；`action` 均为 `item` / `list` / `paged` 之一。

::: warning 示例约定
`<query>` 必须写在 `<k-data>` 内，并显式写 `</query>`。模板要使用查询结果时，在该 `<query>` 上添加 `export` 属性。除明确包含 `<k-data>` 的完整示例外，下文单独出现的 `<query>` 代码块都是查询片段，只用于展示数据源特有属性。
:::

## content

`resource` = **内容类型（Content Folder）名称**。

### list

下面是一个可以直接复制的完整列表查询示例：

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

### 增加字段选择和固定过滤

`select` 用于限制返回字段，`where` 用于过滤结果：

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

### item

根据 URL 参数查询单条内容。`item` 需要 `id`。下面先用 `<let>` 读取 `/detail?id=...` 中的参数，再执行查询：

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

### paged

分页查询使用 `action="paged"`：

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
    共 <span k-content="article_page.total"></span> 条，
    第 <span k-content="article_page.pageIndex"></span> /
    <span k-content="article_page.pageCount"></span> 页
</p>
```

分页查询结果包含：

| 字段 | 说明 |
|------|------|
| `list` | 当前页数据数组 |
| `total` | 总条数 |
| `pageIndex` | 当前页码 |
| `pageSize` | 每页大小 |
| `pageCount` | 总页数 |

## commerce

`resource` 仅支持固定值：`product`、`category`、`cart`、`order`。

### product

| action | 说明 |
|--------|------|
| `item` | `id` 为商品 id 或 **seoName** |
| `list` / `paged` | `where` 支持 `id`、`title`、`featuredImage`、`active`、`seoName`、`tags` 等；`where.category` 为分类 id 或 seoName（**含子分类**） |

```html
<query
    as="products"
    source="commerce"
    resource="product"
    action="list"
    where="{category:'men-shirt'}"
></query>
```

### category

| action | 说明 |
|--------|------|
| `item` | `id` 为分类 id 或 seoName |
| `list` / `paged` | `where` 支持 `id`、`title`、`seoName`、`parentId`、`active`、`tags` 等 |

### cart

- 仅 **`action="item"`**
- 可不传 `id`，使用当前上下文购物车 id

```html
<query as="cart_detail" source="commerce" resource="cart" action="item"></query>
```

### order

| action | 限制 |
|--------|------|
| `item` | 需 `id` |
| `list` / `paged` | **不支持** `where`；**不支持** `order` / `order-desc`（按创建时间倒序） |

## sqlite / mysql / sqlserver

三者的 `<query>` 属性相同；`resource` = **表名**。

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

## indexeddb

与数据库查询相同，`resource` = 对象表名。

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

## culture（多语言链接）

`source="culture"`，`resource` 为要查询的**相对路径**；不传时默认当前请求路径。

### item

- `id`：语言 code；不传则用当前上下文语言
- 返回：`{ key, name, url, isActive }` 或 `null`

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

### list

返回上述结构的数组，用于语言切换列表。

```html
<k-data>
    <query export as="culture_urls" source="culture" resource="/products/detail" action="list"></query>
</k-data>

<div k-for="item in culture_urls">
    <a k-attribute="href {item.url}" k-content="item.name"></a>
</div>
```

## 相关文档

- [k-data 概述](./)
- [条件表达式](./condition.md) — `where` / `test`
- [k.content](../../api/content/index.md) · [commerce](../../api/commerce/commerce.md) — KScript API
