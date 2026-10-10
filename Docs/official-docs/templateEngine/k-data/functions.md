# k-data Built-in Functions

> Functions available through `<let use="functionName" params="...">`.

## Invocation Pattern

Each `<let>` performs one transformation on an existing value:

```html
<let from="create_time" as="createdDate" use="dateFormat" params="'yyyy-MM-dd'" />
<let from="title" as="shortTitle" use="truncate" params='{"length":20,"ellipsis":"..."}' />
<let from="amount" as="amountText" use="currencyFormat" params='{"currency":"USD"}' />
<let from="page_number" as="prev_page" use="subtract" params="1" />
```

| Attribute | Description |
|-----------|-------------|
| `from` | Input value or field path |
| `as` | Output variable name |
| `use` | Function name from the table below |
| `params` | JSON5: a single value, an object, or a format string wrapped in single quotes |

When a function fails, most return `null` or the original input instead of throwing into the template layer.

## Function Summary

| Category | Functions |
|----------|-----------|
| Auth | `jwtDecode` |
| Date | `dateFormat` |
| Currency | `currencyFormat` |
| Math | `add`, `subtract`, `multiply`, `divide`, `modulo` |
| Text | `trim`, `lowercase`, `uppercase`, `truncate` |

## `dateFormat`

| `params` | Description |
|----------|-------------|
| `'"yyyy-MM-dd"'` | Format template |
| `'{"format":"yyyy-MM-dd","fallback":"--"}'` | Format template with fallback |

Input can be a `DateTime`, a parseable date string, and similar values.

## `currencyFormat`

| `params` | Description |
|----------|-------------|
| omitted | Defaults to `CNY` |
| `'"USD"'` or `'{"currency":"EUR"}'` | Sets the currency code |

If the input cannot be converted to a number, the original value is returned.

## `add` / `subtract` / `multiply` / `divide` / `modulo`

| Function | `params` | Description |
|----------|----------|-------------|
| Any of them | `'"3"'`, `'2'`, or `'{"value":5}'` | Right-hand operand |
| `divide`, `modulo` | Right-hand operand is `0` | Returns `null` |

Example for pagination links. Do not write `{pageIndex - 1}` directly inside `k-attribute`:

```html
<let as="page_number" source="queryString" from="page_number" default="1" />
<let from="page_number" as="prev_page_number" use="subtract" params="1" />
<let from="page_number" as="next_page_number" use="add" params="1" />
<a k-attribute="href /products?page_number={prev_page_number}">Previous</a>
```

## `trim` / `lowercase` / `uppercase` / `truncate`

```html
<let from="keyword" as="normalizedKeyword" use="trim" />
<let from="title" as="shortTitle" use="truncate" params='{"length":20,"ellipsis":"..."}' />
```

For `truncate`, `length <= 0` returns an empty string.

## `jwtDecode`

Parses a JWT and optionally verifies its signature through `params`. A failed parse can be combined with a condition and redirect.

```html
<k-data>
    <let as="token" source="cookie" from="access_token" default="''" />
    <let from="token" as="payload" use="jwtDecode" params='{"secret":"your-secret"}' export />
    <when test="{payload:{$eq:null}}">
        <redirect to="'/login'" />
    </when>
</k-data>
```

- Failure cases such as an empty token, invalid format, or failed verification return `null`.
- `when test` uses a JSON5 condition, not an ordinary expression such as `payload == null`.

## Related

- [Tag Syntax](./tags.md#let)
- [Condition Expressions](./condition.md)
