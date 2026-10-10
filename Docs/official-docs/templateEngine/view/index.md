# View

> Reusable HTML fragments embedded into a Page, Layout, or another View with `<view id="...">`

## What It Is

**View** (sometimes called Component in the admin) is a reusable HTML template fragment inside a site. It **does not** have a standalone URL. When you write:

```html
<view id="hero"></view>
```

Kooboo replaces that node with the content of the View resource named `hero`, and then continues resolving `env="server"`, nested `<view>` tags, and other bindings inside that fragment.

## Referencing a View Inside `<head>`

You can no longer put `<view id="...">` directly inside `<head>` (for example, dropping a Tailwind fragment View into the head). That pattern is **no longer supported**.

**Incorrect:**

```html
<head>
    <meta charset="UTF-8">
    <view id="tailwind"></view>
</head>
```

**Compatible approach (current):** render the View from a server-side script inside `<head>`:

```html
<head>
    <meta charset="UTF-8">
    <script env="server">
        k.response.renderView("<view id='tailwind'></view>")
    </script>
</head>
```

`renderView` accepts a View tag string, which is equivalent to `<view id="...">` in the body. It renders the View in the head context and writes it to the output.

**Short form (recommended):** in supported environments, write the View name directly:

```html
<script env="server" view="tailwind"></script>
```

`view` is the site **View resource name**. It matches the `id` used by `<view id="tailwind">`. When there is no script body, only that View is rendered.

`<body>` and `<placeholder>` can still use `<view id="...">` directly without `renderView`.

See the server-side API: [k.response.renderView](../../api/response/index.md#renderview).

## Applicable Scenarios

| Good fit for a View | Better left in a Page |
|---------------------|-----------------------|
| Site-wide header / footer | Route-level `<layout>` and `<placeholder>` |
| Product cards, article list items | Large blocks of copy used only once on the page |
| Blocks with their own data preparation | Thin pages that only combine multiple Views |

## Structure Suggestions

Each View should use a semantic outer container with a `class` to avoid polluting the global scope:

```html
<section class="hero">
    <script env="server">
        var title = "Welcome";
    </script>
    <h1 k-content="title"></h1>
    <view id="hero_cta"></view>
</section>
```

Recommended order:

1. `<script env="server">` - prepare data for this block
2. HTML structure plus `k-content`, `k-for`, and similar bindings
3. Frontend interaction - bottom `<script>` or a [Script](../js/) reference

## Naming

The View `id` matches the resource **name**. Use **snake_case** when possible, for example `product_list` or `site_header`.

## Nesting

A View can reference other Views:

```html
<view id="other_view"></view>
```

Avoid circular references, such as A -> B -> A.

## Difference from Layout / Page

| Resource | How it is referenced | Has route? |
|----------|----------------------|------------|
| Layout | A Page uses `<layout id="...">` | No |
| Page | Direct site URL | Yes |
| View | `<view id="...">` | No |

## Manage Views with Scripts

See [k.site.views](../../api/site/view.md) for add, delete, update, and query APIs.

## Related Docs

- [Page](../page/)
- [Template Binding Syntax](../binding/)
- [Script](../js/)
