# k-data

> Declare data flow with HTML tags inside a template. This is the recommended approach and replaces deprecated `k-query`.

## Overview

**k-data** uses plain HTML tags such as `<k-data>`, `<let>`, and `<query>` to describe a full server-side data flow: read request values, query data, shape the result, and expose variables. Add the `export` attribute to the `<query>` or `<let>` that creates a value needed by the template.

Compared with the old single-attribute **`k-query`** syntax, k-data:

- has a fixed structure that is easier for AI generation and code review,
- supports branches, exceptions, redirects, and field mapping,
- and uses **JSON5 conditions** for `where` and `test` instead of SQL-like strings.

::: tip Division of responsibility with template bindings
| Layer | Syntax | Responsibility |
|------|--------|----------------|
| Inside `<k-data>` | `<let>`, `<query>`, `<when>` and friends | Prepare data |
| Template DOM | `k-content`, `k-for`, `k-if`, `k-attribute` | Render UI |

See [Template Binding Syntax](../binding/) for the rendering layer.
:::

## Minimal Runnable Example

This example does one thing: query `article` content and display each title.

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

This is the smallest complete data flow:

1. `<query>` fetches data and stores the result in `articles`.
2. The `export` attribute on `<query>` exposes `articles` to the template.
3. `k-for` iterates over the result and `k-content` displays each title.

::: tip Before you run it
The site must have a Content Folder named `article` with a `title` field. Replace these names with the resource and field names used by your site.
:::

## Related Topics

| Topic | Description | Documentation |
|-------|-------------|---------------|
| Fields and filters | Control query results with `select` and `where` | [Query Sources](./query.md#content) |
| Request input | Read values from URLs, cookies, headers, and other request sources | [Tag Syntax · let](./tags.md#let) |
| Pagination and sorting | Use `paged`, `page-number`, and `order-desc` | [Query Sources · paged](./query.md#paged) |
| Mapping and formatting | Reshape fields and format display values | [Tag Syntax · map](./tags.md#map), [Built-in Functions](./functions.md) |
| Conditions and errors | Branches, error handling, and redirects | [Condition Expressions](./condition.md), [Tag Syntax](./tags.md) |

## Authoring Conventions

1. Read external input with `source` plus `from`, such as `queryString` or `cookie`.
2. Use `source`, `resource`, and `action` for queries. Standardize pagination with `page-number` and `page-size`.
3. For `action="paged"`, map the list with `from="result.list"`.
4. In the template layer, use only variables exposed with the `export` attribute.
5. `<when>`, `<else-if>`, and `<else>` belong only inside `<k-data>`. In the DOM layer, use `k-if`, `k-else-if`, and `k-else`.
6. The `{...}` syntax in `k-attribute` only interpolates variables. It does not perform calculations. Use `<let use="add|subtract|...">` first, then bind the result.
7. `<query>` is not an HTML void element. Always include `</query>`; do not write `<query ... />`.

## Tag Summary

| Tag | Responsibility |
|-----|----------------|
| `k-data` | Root container for the data flow |
| `let` | Variables, request input, or built-in function calls |
| `query` | Query content, commerce, database, and related sources |
| `map` | Map over an array or object |
| `when` / `else-if` / `else` | Conditional branches inside `k-data` |
| `try` / `catch` | Exception handling |
| `redirect` | Server-side redirect |

`export` is a boolean attribute on `<query>` and `<let>` that exposes their `as` variable to the template layer.

See [Tag Syntax](./tags.md) for details.

## Docs in This Section

| Topic | Description |
|-------|-------------|
| [Tag Syntax](./tags.md) | `let`, `query`, `map`, branching, and exposing variables |
| [Query Sources](./query.md) | `content`, `commerce`, `sqlite`, `mysql`, `culture`, and more |
| [Built-in Functions](./functions.md) | `dateFormat`, `jwtDecode`, math, and text helpers |
| [Condition Expressions](./condition.md) | JSON5 `where` and `test` syntax |

## Relationship to KScript APIs

| Scenario | Doc |
|----------|-----|
| Declarative data fetching inside a template | This `k-data` section |
| CRUD or business logic in script code | [k.content](../../api/content/index.md), [k.DB](../../api/database/index.md), [k.commerce](../../api/commerce/commerce.md) and related APIs |

## Related

- [Page](../page/) and [View](../view/)
- [Template Binding Syntax](../binding/)
