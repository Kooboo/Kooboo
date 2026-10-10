# k-data Functions

This document defines the semantics of built-in functions callable through `let use="..."` in `k-data`.
The goal is to cover common SSR presentation scenarios (authentication parsing, date formatting, amount and text processing, and basic arithmetic). **Prefer completing declarative logic inside k-data rather than degrading it to `<script env="server">`** (see [basic.md](basic.md) “Data-Source Route Decision”).

---

## Call Form

```html
<let as="token" source="cookie" from="access_token" default='""' />
<let from="token" as="payload" use="jwtDecode" />
<let from="payload.userId" as="userId" />
<let from="create_time" as="createDateText" use="dateFormat" params='"yyyy-MM-dd"' />
<let from="create_time" as="displayText" use="dateFormat" params='{"format":"yyyy-MM-dd","fallback":"--"}' />
<let from="amount" as="amountText" use="currencyFormat" params='{"currency":"USD"}' />
<let from="title" as="shortTitle" use="truncate" params='{"length":20,"ellipsis":"..."}' />
<let from="amount" as="total" use="add" params='"12.5"' />
<let from="total" as="discounted" use="subtract" params='{"value":2.5}' />
<let from="discounted" as="taxed" use="multiply" params='{"value":1.06}' />
<let from="taxed" as="avg" use="divide" params='2' />
<let from="avg" as="remainder" use="modulo" params='3' />
```

Conventions:

- `from`: input variable name, field path, or request-parameter / cookie name
- `source`: external input source such as `querystring` / `cookie` / `header` / `path` / `form`
- `as`: output variable name
- `use`: function name (must be defined in this document)
- `params`: function parameters as serialized JSON; when strings, arrays, or objects need inner double quotes, wrap the outer value in single quotes
- For one parameter, pass a JSON value directly; for multiple parameters, pass a JSON object whose keys are parameter names and values are parameter values

---

## Function Overview

| Category | Functions |
| --- | --- |
| Authentication | `jwtDecode` |
| Date | `dateFormat` |
| Currency/number | `currencyFormat` |
| Math | `add`、`subtract`、`multiply`、`divide`、`modulo` |
| Text | `trim`、`lowercase`、`uppercase`、`truncate` |

---

## Authentication Functions

## `jwtDecode`

Purpose:

- Parse a user identifier from a JWT token for subsequent sqlite / business queries.

Input:

- The token string referenced by `from`.

Parameters:

- `params` is optional.
- When `params` is a string, it is used as the verification secret.
- When `params` is an object, `secret` or `key` may be used as the secret field.

Output:

- On success, write the parsed result object to the `as` variable (in the example, `payload`).

Failure behavior:

- When the token is empty, malformed, fails verification, or cannot be parsed, return `null` without throwing.

Example:

```html
<k-data>
    <let as="token" source="cookie" from="access_token" default='""' />
    <let from="token" as="payload" use="jwtDecode" params='{"secret":"your-256-bit-secret"}' />
    <when test="{payload:{$eq:null}}">
        <redirect to="'/login'" />
    </when>
</k-data>
```

---

## Date Functions

## `dateFormat`

Purpose:

- Format a time value into a specified display format.

Input:

- `from` points to `DateTime`, `DateTimeOffset`, a parseable date string, or a value convertible to a date.

Parameters:

- Pass a format-template string in `params`, such as `'"yyyy-MM-dd"'`, `'"yyyy-MM-dd HH:mm"'`, or `'"yyyy-MM-dd HH:mm:ss"'`.
- An object parameter is also supported: `params='{"format":"yyyy-MM-dd","fallback":"--"}'`.

Output:

- Normally returns the formatted date string.
- When `format` is invalid or the value cannot be converted, return `fallback` when provided, or the original input.

Example:

```html
<let from="create_time" as="createdDate" use="dateFormat" params='"yyyy-MM-dd"' />
<let from="update_time" as="updatedAtText" use="dateFormat" params='"yyyy-MM-dd HH:mm:ss"' />
<let from="publish_time" as="publishDateText" use="dateFormat" params='{"format":"yyyy-MM-dd","fallback":"--"}' />
```

---

## Currency/Number Functions

## `currencyFormat`

Input:

- A number or a numeric string.

Parameters:

- When `params` is a string, use it as the currency code (such as `"USD"`).
- When `params` is an object, support a `currency` or `code` field (such as `{"currency":"EUR"}`).
- When no currency code is passed, default to `CNY`.

Output:

- Format using the currency's default decimal places.
- Known currencies output a symbol (such as `$1,234.50` or `¥1,234.50`).
- Unknown currencies output a code prefix (such as `ABC 1,234.50`).
- Return the original input when it cannot be converted to a number.

Example:

```html
<let from="amount" as="amountText" use="currencyFormat" />
<let from="amount" as="amountUsd" use="currencyFormat" params='"USD"' />
<let from="amount" as="amountEur" use="currencyFormat" params='{"currency":"EUR"}' />
```

---

## Math Functions

Applicable functions: `add`、`subtract`、`multiply`、`divide`、`modulo`。

Purpose:

- Perform basic arithmetic between the `from` input and a second operand.

Input:

- `from` is the left operand and supports a number or a convertible numeric string.

Parameters:

- When `params` is a single value, use it as the right operand, such as `params='"3"'` or `params='2'`.
- When `params` is an object, support `value` / `right` / `operand` as the right-operand field.

Output:

- Return a `decimal` result when the operation succeeds.
- Return the original `from` when either operand cannot be converted to a number.
- Return `null` without throwing when the right operand is `0` for `divide` or `modulo`.

Example:

```html
<let from="price" as="plusFee" use="add" params='{"value":5}' />
<let from="plusFee" as="minusCoupon" use="subtract" params='2' />
<let from="minusCoupon" as="withTax" use="multiply" params='"1.13"' />
<let from="withTax" as="avg" use="divide" params='3' />
<let from="orderNo" as="bucket" use="modulo" params='10' />
```

Pagination, previous/next-page links, and similar scenarios should also be precomputed in `k-data` before being bound in the template:

```html
<let as="page_number" source="queryString" from="page_number" default="1" />
<let from="page_number" as="prev_page_number" use="subtract" params="1" />
<let from="page_number" as="next_page_number" use="add" params="1" />
<a href="/products?page_number={prev_page_number}">上一页</a>
<a href="/products?page_number={next_page_number}">下一页</a>
```

Do not write expressions such as `product_paged.pageIndex - 1` or `total * rate` directly inside `{...}` in native `href` or `k-attribute`; template interpolation binds existing values and does not execute arithmetic.

---

## Text Functions

## `trim`

- Remove leading and trailing whitespace.

## `lowercase`

- Convert to lowercase.

## `uppercase`

- Convert to uppercase.

## `truncate`

Purpose:

- Truncate text to a specified length.

Parameters:

- Pass `params='3'` or `params='"3"'` to specify the length directly.
- Object parameters support only:
    - `length`: maximum length.
    - `ellipsis`: optional suffix text.

Output:

- Return the original value when no length is provided.
- Return an empty string when `length <= 0`.
- When the length of `ellipsis` is greater than or equal to `length`, return the truncated `ellipsis`.

Example:

```html
<let from="keyword" as="normalizedKeyword" use="trim" />
<let from="status" as="statusCode" use="lowercase" />
<let from="title" as="titleUpper" use="uppercase" />
<let from="title" as="shortTitle" use="truncate" params='{"length":20,"ellipsis":"..."}' />
```
