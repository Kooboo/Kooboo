# Layout

> Site-level HTML skeleton: shared `<head>`, global assets, and `k-placeholder` regions.

## What It Is

A **Layout** is a site resource that stores the overall HTML structure of a page. Multiple **Page** resources can reuse the same layout and only replace the placeholder content, which keeps navigation, footer, CDN setup, and global styles consistent.

A Layout **does not** have its own URL. Visitors always access a Page route, and Kooboo merges the Page content into the Layout during rendering.

## The Key Concept: `k-placeholder`

The Layout body must define at least one placeholder region that a Page can fill:

```html
<div k-placeholder="Main"></div>
```

| Item | Description |
|------|-------------|
| Attribute | `k-placeholder` |
| Value | A custom name such as `Main` or `Sidebar` |
| Matching rule | Must match the `id` used by `<placeholder id="...">` inside the Page body |

### Example: Two-column layout

```html
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>Site title</title>
    <link rel="stylesheet" href="/assets/site.css" />
</head>
<body>
    <header>Global navigation</header>
    <div class="flex">
        <main k-placeholder="Main"></main>
        <aside k-placeholder="Sidebar"></aside>
    </div>
    <footer>Global footer</footer>
</body>
</html>
```

## What Usually Lives In a Layout

- `<head>` metadata, import maps, global styles, and shared scripts
- Shared site shell such as header, footer, or sidebar chrome
- Placeholder regions reserved for Page-level content

::: warning Referencing a View inside `<head>`
Do not place `<view id="...">` directly inside `<head>`. Use `k.response.renderView("<view id='...'></view>")` or `<script env="server" view="view_name"></script>` instead.
:::

## How Layout and Page Work Together

Pages reference a layout with `<layout id="layoutName">` and fill its placeholders with `<placeholder id="...">`:

```html
<layout id="main">
    <placeholder id="Main">
        <h1>Page content</h1>
    </placeholder>
    <placeholder id="Sidebar">
        <nav>Sidebar links</nav>
    </placeholder>
</layout>
```

The `layout` `id` matches the Layout resource name. Each placeholder `id` must match a `k-placeholder` value in the Layout.

## When To Create a New Layout

- When a new site needs its first shared shell
- When you need a fundamentally different page skeleton, such as a marketing landing page versus a dashboard-style layout

Small changes such as adding one placeholder or changing the footer usually belong in the existing layout instead of creating a brand-new one.

## Manage Layouts With Code

Use [k.site.layouts](../../api/site/layout.md) when you need to create, read, update, or delete Layout resources from KScript.

## Related

- [Page](../page/)
- [View](../view/)
- [k.site.layouts](../../api/site/layout.md)
