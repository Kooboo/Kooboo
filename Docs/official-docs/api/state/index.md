# k.state

> View data (ViewData) in the current request rendering context

## Overview

`k.state` reads and writes key values ​​in the view data stack of the current HTTP request for use by page, layout, and view template binding (for example, `k-state-key` or reading data with the same name in the template). The data is released when the request ends and is not persisted across requests.

::: The difference between tip, k.cache and k.session
| API | Scope | Typical uses |
|-----|--------|----------|
| **`k.state`** | View rendering within a single request | Controller/CodeBlock passes `product`, `breadcrumb` to the template |
| **`k.cache`** | Site-level memory, expirable | Calculation results shared across requests |
| **`k.session`** | guest session | Login status, shopping cart temporary data |
:::

Must be used in the process involved in **HTML page rendering** (page CodeBlock, layout script, etc.). In requests where pure `k.api` returns JSON and does not render templates, `set`/`get` are usually only visible to the current script and will not appear in the response HTML.

## TypeScript Definition

```ts
interface KState {
  set(key: string, value: any): void;
  setCurrent(key: string, value: any): void;
  get(key: string): any;
}
```

## set()

**Push** a piece of named data to the view data stack (which can form a hierarchy with subsequent `set` and be parsed by the rendering engine).

```ts
const product = { id: "1", name: "Sample" }
k.state.set("product", product)
```

| Parameter | Type | Required | Description |
|------|------|------|------|
| `key` | `string` | yes | Key names used in templates |
| `value` | `any` | yes | any serializable object or value |

**Returns:** `void`.

## setCurrent()

Writes or overwrites the key on the top frame of the current stack; if the stack is empty, the behavior is the same as `set`.

```ts
k.state.set("items", list)
k.state.setCurrent("items", updatedList)
```

| Parameter | Type | Required | Description |
|------|------|------|------|
| `key` | `string` | yes | Key name |
| `value` | `any` | yes | new value |

**Returns:** `void`.

## get()

Press the key to read the view data (parsed from the current data context, including the outer stack).

```ts
const product = k.state.get("product")
```

| Parameter | Type | Required | Description |
|------|------|------|------|
| `key` | `string` | yes | Key name |

**Returns:** The value previously written to `set` / `setCurrent`; `undefined` or empty if not present (whichever is runtime).

## Example: List page passes data to template

```ts
// Page CodeBlock
const items = k.DB.sqlite.table("Product").all()
k.state.set("products", items)
k.page.setTitle("Product List")
```

```ts
// Read from another script
const products = k.state.get("products")
```

## Related Docs

- [k.page](../page/) — Current page `<title>` / `<meta>`
- [k.cache](../cache/) — site cache
- [k.session](../session/) — session
