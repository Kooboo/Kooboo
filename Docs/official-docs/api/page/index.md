# k.page

> The `<title>` and `<meta>` bindings of the page rendered by the current HTTP request

## Overview

`k.page` writes the header binding in the rendering context of the current request and is output by the layout/page rendering phase to `<title>` and `<meta>` of HTML. Suitable for dynamically changing titles or SEO/sharing tags based on interface data and user status.

::: The difference between tip and k.site.pages, k.response.meta
| API | effect |
|-----|------|
| **`k.page`** | Current page `setTitle`, `setNameMeta`, `setPropertyMeta`, etc. (this page) |
| **`k.site.pages`** | For the addition, deletion, modification and query of Page resources within the site, see [site/page.md](../site/page.md) |
| **`k.response.meta`** | Another entry bound to the same set of page headers: `meta.title`, `meta.setMeta(name, content)`, see [k.response](../response/) |

`k.page.setNameMeta("description", "…")` and `k.response.meta.setMeta("description", "…")` are written into the same binding list; just choose one according to team habits.
:::

Must be called in a script that will participate in the HTML rendering of the page (such as the CodeBlock and layout script associated with the page). Pure API returns JSON and does not use page template requests, and the call usually does not affect the response body.

## TypeScript Definition

```ts
interface KPage {
  setTitle(value: string): void;
  setCharsetMeta(value: string): void;
  setNameMeta(name: string, content: string): void;
  setPropertyMeta(property: string, content: string): void;
  setHttpEquivMeta(equiv: string, content: string): void;
}
```

## setTitle()

Sets or overrides the `<title>` text of the current page.

```ts
k.page.setTitle("Product Details - My Site")
```

| Parameter | Type | Required | Description |
|------|------|------|------|
| `value` | `string` | yes | title text |

**Returns:** `void`.

## setCharsetMeta()

Set document character set meta (write `charset` binding).

```ts
k.page.setCharsetMeta("utf-8")
```

| Parameter | Type | Required | Description |
|------|------|------|------|
| `value` | `string` | yes | Character set, ignored when empty string |

**Returns:** `void`.

## setNameMeta()

Set `<meta name="…" content="…">`.

```ts
k.page.setNameMeta("description", "Site description")
k.page.setNameMeta("keywords", "kooboo, cms")
```

| Parameter | Type | Required | Description |
|------|------|------|------|
| `name` | `string` | yes | `name` attribute |
| `content` | `string` | yes | `content` attribute |

**Returns:** `void`.

If the `name` with the same name already exists, the content will be **updated**, otherwise the binding will be appended.

## setPropertyMeta()

Set `<meta property="…" content="…">` (Open Graph, etc.).

```ts
k.page.setPropertyMeta("og:title", "Shared title")
k.page.setPropertyMeta("og:image", "https://example.com/cover.jpg")
```

| Parameter | Type | Required | Description |
|------|------|------|------|
| `property` | `string` | yes | `property` attribute |
| `content` | `string` | yes | `content` attribute |

**Returns:** `void`.

## setHttpEquivMeta()

Set `<meta http-equiv="…" content="…">`.

```ts
k.page.setHttpEquivMeta("refresh", "30;url=/next")
k.page.setHttpEquivMeta("X-UA-Compatible", "IE=edge")
```

| Parameter | Type | Required | Description |
|------|------|------|------|
| `equiv` | `string` | yes | `http-equiv` attribute |
| `content` | `string` | yes | `content` attribute |

**Returns:** `void`.

## Example: Setting up SEO by routing parameters

```ts
// Page or layout CodeBlock
const productName = k.request.queryString.name || "Product"
k.page.setTitle(productName + " - Store")
k.page.setNameMeta("description", "Buy " + productName)
k.page.setPropertyMeta("og:title", productName)
```

## Related Docs

- [k.site.pages](../site/page.md) — Site page resource management
- [k.response](../response/) — `k.response.meta` Read and write page header binding
- [k.request](../request/) — Current request parameters
