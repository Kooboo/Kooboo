# k.utils.template

> View and page rendering

## Overview

`k.utils.template` Renders Kooboo **View** or **Page** HTML on the server side.

## render()

Renders a view tag or view ID string, returning HTML.

| Parameter | Type | Required | Description |
|------|------|------|------|
| `view` | `string` | Yes | View tag, view name, or view Id |

**Returns:** `string`.

```ts
const html = k.utils.template.render("<view id=\"...\"></view>")
```

## executeView()

Execution view (side effects are written to the current response context, no strings are returned).

| Parameter | Type | Required | Description |
|------|------|------|------|
| `viewNameOrId` | `string` | Yes | View name or Id |

**Returns:** `void`.

```ts
k.utils.template.executeView("ProductCard")
```

## renderView()

Render HTML by view name or Id.

| Parameter | Type | Required | Description |
|------|------|------|------|
| `viewNameOrId` | `string` | Yes | View name or Id |

**Returns:** `string \| null`.

```ts
const html = k.utils.template.renderView("ProductCard")
```

## renderPage()

Renders a full page of HTML by page URL.

| Parameter | Type | Required | Description |
|------|------|------|------|
| `url` | `string` | Yes | Site page URL, optionally with query string |

**Returns:** `string`.

```ts
const pageHtml = k.utils.template.renderPage("/products")
```

## Related Docs

- [k.site.views](../site/view.md)
- [k.site.pages](../site/page.md)
