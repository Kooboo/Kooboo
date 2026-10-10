# Page

> An HTML page with a route: either a full standalone document or a Layout-based page with injected placeholders.

## What It Is

A **Page** is an HTML resource in Kooboo that has a URL. When a visitor opens `/about`, Kooboo loads the matching Page, optionally applies a Layout, resolves `<view>` references, and runs any server-side bindings inside the template.

Do not confuse this with [k.page](../../api/page/index.md): `k.page` only changes the title or metadata for the **current request**, while a **Page resource** is the stored site page managed by the site repository.

## Two Common Patterns

### 1. Standalone full page

Use a complete HTML document when the page does not need a shared layout:

```html
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>About us</title>
</head>
<body>
    <script env="server">
        var title = "About us";
    </script>
    <h1 k-content="title"></h1>
    <view id="hero"></view>
</body>
</html>
```

### 2. Page that uses a Layout

For multi-page sites, the recommended pattern is to keep the shared shell inside a Layout and only define the page-specific content in the Page body:

```html
<layout id="main">
    <placeholder id="Main">
        <div class="prose">
            <h1>Main content</h1>
            <p>Only the page-specific part lives here.</p>
        </div>
    </placeholder>
    <placeholder id="Sidebar">
        <aside>Sidebar content</aside>
    </placeholder>
</layout>
```

| Element | Description |
|---------|-------------|
| `<layout id="...">` | References the Layout resource by name |
| `<placeholder id="...">` | Must match a `k-placeholder` defined by the Layout |
| Placeholder body | Can contain normal HTML, `<view>`, and inline scripts including `<script env="server">` |

## Referencing Views

You can insert reusable fragments anywhere in a Page:

```html
<view id="product_card"></view>
```

The `id` must match the site View resource name.

## Scripts and Styles

- Put **global** scripts and styles in the Layout `<head>`.
- Put **page-specific** logic inside the Page or reference routed Script / Style assets.

## Split Large Pages Into Views

Consider moving markup into **View** resources when:

- a page contains several clearly independent sections,
- the same block is reused in multiple pages,
- or the page becomes hard to read and maintain as one file.

## Manage Pages With Code

Use `k.site.pages` when you need to create pages, update routes, or manage page resources from KScript.

## Related

- [Layout](../layout/)
- [View](../view/)
- [k.site.layouts](../../api/site/layout.md)
