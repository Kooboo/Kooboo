# k-data Basic Syntax

## 1. Goal

`k-data` expresses data flow with tags and attributes, avoiding JS-like template logic and making it easier for:

- Stable parsing and SSR rendering by the server executor
- AI generation to follow a fixed structure and reduce semantic drift

**For SSR dynamicization, k-data is the default data-access method** (see S9 in `Scenario/site-building/dynamic-delivery.md`). Use `<script env="server">` only for server logic that k-data cannot express or for `type="module"` imports; ordinary template containers do not need `env="server"`.

---

## 1.1 Data-Access Decision (Required for Dynamicization)

When implementing SSR data binding, **first determine whether k-data can express it, then consider an env=server script**.

### Use k-data (Default)

- `source="commerce"` / `content` / `sqlite` / `mysql` / `sqlserver` / `culture` query (list, paged, or item) using `<query>`
- `source="queryString"` / `cookie` / `header` / `path` / `form` input using `<let>`
- `<map>` for shaping lists, `<when>` for selecting a query branch, and `<let use="...">` for date, amount, or math operations
- The template layer consumes only variables exposed through `<query export>` or `<let export>`

### Fall Back to `<script env="server">` (The Reason Must Be Clear)

- `import` of a CodeBlock / Service (requires `type="module"` + `export`)
- A called `k.*` API has **no** corresponding k-data `source` / `resource` (for example, some site-level utility APIs)
- Complex imperative orchestration that cannot be decomposed into a declarative query/let chain

### Anti-Pattern (Forbidden)

```html
<!-- ❌ 动态化时用脚本直接查库，应改为 k-data query -->
<script env="server">
    var products = k.commerce.product.list({ page: 1 })
</script>
<div k-for="item in products">
    <span k-content="item.title"></span>
</div>
```

```html
<!-- ✅ 同等语义应用 k-data -->
<k-data>
    <query as="products" source="commerce" resource="product" action="list" export></query>
</k-data>
<div k-for="item in products">
    <span k-content="item.title"></span>
</div>
```

Complete the data flow inside `<k-data>` whenever possible. The template layer should directly consume variables exposed by `export` using native `href` interpolation, `k-for`, `k-content`, or `k-attribute`. Do not add `env="server"` to an outer element just to enable template binding.

---

## 2. Tag Overview

Top-level container:

- `k-data`

Variables, request reads, and function calls:

- `let`

Queries and shaping:

- `query`
- `map`

Conditional branches:

- `when`
- `else-if`
- `else`

Errors and redirects:

- `try`
- `catch`
- `redirect`

Variable exposure:

- The `export` attribute of `query` / `let`

---

## 3. Global Authoring Conventions

1. Express external data sources consistently with `source` / `resource`.
2. Express variables, field paths, and request parameter names consistently with `from`.
3. Use `page-number` and `page-size` consistently for pagination.
4. When mapping a paged query result (`action="paged"`), use `map from="xxx.list"`.
5. When a unique identifier is known and the semantic goal is one detail record, use `action="item"` directly. Use `id="变量名"` for a content item's name / ID and `where` for a custom-field lookup. Do not run `list` and then simulate a detail query with `[0]`.
6. Use `select` only when field projection is actually needed; omit it for a complete object. An `item` query by content name / ID returns the complete object, so detail pages usually do not need `select`.
7. Kooboo page route parameters such as `/post/{slug}` are already in the rendering data context; read them with `<let as="slug" from="slug"></let>`. `source="path"` maps to ASP.NET `Request.RouteValues`, not the default source for Kooboo page route parameters.
8. The template layer consumes only variables exposed by the `export` attribute on `query` / `let`.
9. Put authenticated or high-risk queries in a consistent `try/catch` structure where appropriate.
10. Use native `href` with `{...}` interpolation for link URLs, `k-attribute` for other template attribute bindings, and `k-content` for value bindings.
11. Use `<let from="xxx[0]" as="yyy">` on a `list` result only when the business meaning is specifically “the first item in an ordered result”; it does not replace a unique-key `item` query.
12. `when` / `else-if` / `else` are internal `k-data` tags and may appear only in a branch structure inside `<k-data>`; use `k-if` / `k-else-if` / `k-else` for template-layer conditional rendering.
13. `{...}` in native `href` and `k-attribute` is only for variable / field-path interpolation, not math or conditional expressions. Precompute values with `<let use="...">` inside `<k-data>`.
14. `query` is not a void element and must have an explicit `</query>`; do not use `<query ... />`, or later `k-data` steps may be parsed as children of `query`.
15. `query` must declare non-empty `as`, `source`, `resource`, and `action`; when a required attribute is missing or the source/action is unsupported, the runtime silently does not create the result variable.
16. `export` is a Boolean attribute for `query` / `let`; write `export` directly ( `export="true"` is also supported). `export="false"` and an omitted attribute do not expose the variable.
17. `export` exposes the variable named by the current tag's `as`, so an `export`-enabled `query` / `let` must declare a non-empty `as`.
18. `query`, `try`, `catch`, and `redirect` must be inside `<k-data>`; DataEvaluator parses only the contents of `<k-data>`.

---

## 4. Syntax

## `let`

`let` defines variables, reads request context, extracts fields, or calls a function through `use`.

```html
<let as="keyword" source="queryString" from="keyword" default=""></let>
<let as="token" source="cookie" from="id" default="123456"></let>
<let as="products" default="[]"></let>
<let from="token" as="payload" use="jwtDecode"></let>
<let from="payload.userId" as="userId"></let>
<let from="create_time" as="createdDate" use="dateFormat" params="'yyyy-MM-dd'"></let>
<let from="create_time" as="displayText" use="dateFormat" params="{format:'yyyy-MM-dd',fallback:'--'}"></let>
```

| Attribute | Required | Description |
| --------- | ---- | ---------------------------------------------- |
| `as` | Yes | Output variable name |
| `from` | No | Variable name, field path, or external input data |
| `source` | No | External input source such as `queryString` / `cookie` / `header` / `path` / `form` |
| `use` | No | Function name (see [functions.md](functions.md)) |
| `params` | No | Function arguments, passed as a variable name or JSON5 string |
| `default` | No | Default value, passed as a variable name or JSON5 string |
| `export` | No | Boolean attribute; exposes the `as` variable to the template layer |

## `query`

Executes a query and binds the result variable.

```html
<query
    as="product_page"
    source="commerce"
    resource="product"
    action="paged"
    select="id,title,featuredImage,variants"
    where="{title:{$contains:'hello'}}"
    page-number="page_number"
    page-size="page_size"
    order-desc="price"
></query>
```

| Attribute | Required | Description |
| ------------- | ---- | -------------------------------------------- |
| `as` | Yes | Query result variable name |
| `source` | Yes | Data source such as `content` / `commerce` / `sqlite` / `mysql` \ `sqlserver` / `indexeddb` / `culture` |
| `resource` | Yes | Resource name such as `product` / `payment_records` |
| `action` | Yes | Query action; examples use `paged` / `item` / `list` |
| `select` | No | Field list |
| `where` | No | JSON5 filter expression (see [condition.md](condition.md)) |
| `order` | No | Ascending field such as `create_time` |
| `order-desc` | No | Descending field such as `price` |
| `page-number` | No | Page-number variable or integer |
| `page-size` | No | Page-size variable or integer |
| `export` | No | Boolean attribute; exposes the `as` result to the template layer |

For details, see [query.md](query.md).

## `map`

Processes an array or object item by item and outputs a new result.

```html
<map from="product_page.items" as="products">
    <let from="id" as="id" ></let>
    <let from="title" as="title" ></let>
    <let from="featuredImage" as="featuredImage" ></let>
</map>
```

```html
<map from="filters" as="filter_keys">
    <let default="123" as="key" ></let>
</map>
```

```html
<map as="new_user">
    <let default="23" as="age" ></let>
    <let default="'jobs'" as="name" ></let>
</map>
```

| Attribute | Required | Description |
| ------ | ---- | ---------------- |
| `as` | Yes | Output variable name |
| `from` | No | Source array or object variable; when empty, returns a new object |

## `when` / `else-if` / `else`

Conditional branch structure.

```html
<when test="{mode:'published'}">
    <query ... ></query>
    <else-if test="{mode:'unpublished'}">
        <query ... ></query>
    </else-if>
    <else>
        <query ... ></query>
    </else>
</when>
```

| Attribute | Description |
| ------ | ------------- |
| `test` | JSON5 condition expression (see [condition.md](condition.md)) |

Notes:

- `when` / `else-if` / `else` are valid only inside `<k-data>`.
- To control whether template-layer DOM is rendered, use `k-if` / `k-else-if` / `k-else`; do not put `k-data` branch tags directly in the template layer.

## `try` / `catch`

Exception-handling structure.

```html
<try>
    <query ... ></query>
</try>
<catch>
    <let as="has_error" default="true"></let>
</catch>
```

## `redirect`

Redirects.

```html
<redirect to="'/login'"></redirect>
```

| Attribute | Description |
| ------ | ------------- |
| `to` | Variable name or JSON5 string |

## `export` Attribute

Add `export` to a `query` or `let` that the template layer needs to use. It exposes the variable named by the current tag's `as`.

```html
<query as="products" source="commerce" resource="product" action="list" export></query>
<let as="keyword" source="queryString" from="keyword" default="''" export></let>
```

## Template-Layer Attribute Binding

Use native `href` with `{...}` interpolation for link URLs. Use `k-attribute` to assign other template-layer attributes and keep the syntax consistent with `k-for` / `k-if` / `k-content`.
For complete syntax and examples, see [template-binding-syntax.md](../template-binding-syntax.md).

Common examples:

```html
<!-- Link URLs use native href -->
<a href="{linkHref}">About Us</a>
<a href="/detail/{itemId}">Detail</a>

<!-- Other dynamic attributes use k-attribute -->
<img k-attribute="src imageUrl">

<!-- Separate multiple attributes with ; -->
<img k-attribute="src imageUrl; alt 'Logo'">

<!-- Conditional class object syntax -->
<div k-attribute="class {active: isActive; checked: isChecked}">Content</div>

<!-- Static and dynamic classes coexist and are merged during rendering -->
<a class="text-xs font-medium px-2 py-1"
   k-attribute="class {'text-white bg-blue-600': isActive, 'text-gray-500': !isActive}">
  Button
</a>
```

Conventions:

- Write link URLs through native `href`; use `{variable}` or `{object.field}` for dynamic values.
- Use `k-attribute` for single or multiple non-`href` attribute bindings, separating multiple attributes with `;`.
- Quote literal strings, such as `alt 'logo'`, so they are not parsed as variables.
- Contents inside brace interpolation in native `href` or `k-attribute` should be variable names or field paths such as `{product.id}` and `{product.title}`.
- Do not write expressions such as `{pageIndex - 1}` or `{price * qty}` directly in native `href` or `k-attribute`; precompute them in `<k-data>` with `<let use="add|subtract|multiply|divide|modulo">`, then bind the result variable.
- Static `class="..."` and `k-attribute="class {...}"` can coexist; rendering merges them automatically without either overriding the other.
- New documentation and examples should use native `href` for links and `k-attribute` for other dynamic attributes.

---

## 5. Minimal Complete Example

```html
<k-data>
    <let as="keyword" source="queryString" from="keyword" default="''"></let>
    <let as="page_number" source="queryString" from="page_number" default="1"></let>
    <let as="page_size" source="queryString" from="page_size" default="10"></let>
    <query
        as="query_result"
        source="content"
        resource="user"
        action="paged"
        select="id,age,name,birthday"
        where="{age:{$gt:23},name:{$contains:'lily'}}"
        page-number="page_number"
        page-size="page_size"
        order-desc="create_time"
        export
    ></query>
</k-data>
<div>
    <div k-for="user in query_result.list">
        <p k-content="user.name"></p>
        <p k-content="user.birthday"></p>
    </div>
    <p>
        共 <span k-content="query_result.total"></span> 条，
        第 <span k-content="query_result.pageIndex"></span> / <span k-content="query_result.pageCount"></span> 页，
        每页 <span k-content="query_result.pageSize"></span> 条
    </p>
</div>
```
