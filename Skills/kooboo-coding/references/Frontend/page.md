# Site Page Development Guide

## Overview

This guide is a concise reference for site-page development. It introduces common structures, usage scenarios, and example code to help build functional pages quickly.

## Usage scenario

Use it when creating or modifying site pages, including standard pages and pages with a Layout.

## Page URL and dynamic routes

The `url` passed to `create_page` is the browser route and may be a static path or a dynamic path template with `{parameter}` segments. For independently addressable detail pages, use a path parameter as the canonical identity: prefer `/blog/{slug}` for an article and `/product/{seoName}` for a Commerce product. Use query strings for list state such as search, filters, sorting, and pagination.

Keep the same parameter name in the Page URL, list/detail links, and the detail page's `k-data`. A route `/blog/{slug}` exposes `slug` to the rendering context; `/product/{seoName}` exposes `seoName`. Preserve an existing public route when compatibility requires it, but do not introduce a query-string detail identity when a path route is available.

## Examples

### Standard page

```html
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>Document</title>
</head>
<body>
    <div>
        <h1>Site Page Demo</h1>
        <view id="hero"></view>
    </div>
</body>
</html>
```

### Page with a Layout

`<layout>` is an **outer container**, not a declaration tag. Every `<placeholder>` must be nested inside `<layout>`; sibling or empty-closed placeholders prevent content from being injected into the Layout.

```html
<layout id="main">
    <placeholder id="Main">
        <div>main content</div>
    </placeholder>
    <placeholder id="Sider">
        <div>sider content</div>
    </placeholder>
</layout>
```

### Common error (forbidden)

```html
<!-- 错误：layout 与 placeholder 并列，placeholder 内容不会进入 Layout -->
<layout id="main"></layout>
<placeholder id="Main">
    <div>main content</div>
</placeholder>
```

## Features

- `<layout id="[LayoutName]">`: **Container tag** that must wrap every `<placeholder>` on this page; `id` must match the `layoutName` used by `create_page` and the Layout object name.
- `<view id="[Custom Name]"></view>`: References a View component; see the `view` reference.
- `<placeholder id="[Name]">`: A Page with a Layout must use it to inject content into the Layout's `k-placeholder="[Name]"` area. The `id` is case-sensitive and must match the Layout's `k-placeholder` attribute value character-for-character. For example, when the Layout uses `k-placeholder="Main"`, the Page must use `<placeholder id="Main">`, not `main`.
- Ordinary Page containers must not declare `env="server"`; `k-data` / `k-xx` bindings can be used directly, while actual server-side logic belongs in a standalone `<script env="server">`.

## Component-splitting principles

Pages should not carry too much code. When a Page contains any of the following, split the independent block into a View:

- Multiple semantically independent regions (Header/Hero/Features/Footer, and so on)
- Blocks reusable on other pages (product cards, article lists, sidebars, and so on)
- More than 3-4 independent blocks in one Page file

After splitting, keep the Page concise and move concrete logic into each View and its corresponding Script. Each View should focus only on its own block for easier maintenance and reuse.

## Page skeleton and View assembly (Site Building S4)

This is the **single source for the inline Page skeleton** in Site Building S4 step ④. Other references (including the SOP) only refer to this section and do not repeat the code.

During site building, first obtain the current page `page-item`, then **`create_page`, followed by `create_view` one at a time**; do not finish all real Views first.

1. **Site level**: `create_layout` (with `<view>` references for chrome such as `site_header` / `site_footer`) → `create_style` → lightweight `create_view` (only cross-page blocks such as header/footer).
2. **Page level**: obtain `blockRefs + compositionGroups` from `page-item` → use ordinary HTML/CSS in `create_page` to compose the page and place each **page-level** block → open the page in Preview → call `create_view` per block and replace the skeleton with `<view id="..."></view>`.

**Chrome does not belong in the Page placeholder**: the Layout references header/footer; referencing the same View id in both Layout and Page renders it twice.

**A Page can contain normal HTML elements**: inside `<placeholder>`, you may use `main`, `section`, `div`, `aside`, grid/flex wrappers, page headings, and breadcrumbs. Views own independent content blocks, while the Page owns layout relationships among Views; do not mistakenly write a Page as a list that can only place consecutive `<view>` elements.

Inline skeleton example (page-level blocks only):

```html
<layout id="main">
  <placeholder id="Main">
    <section data-block="hero" class="min-h-[40vh] bg-gray-100 animate-pulse rounded-lg"></section>
    <section data-block="features" class="h-32 animate-pulse bg-gray-50 rounded-lg mt-8"></section>
  </placeholder>
</layout>
```

When `compositionGroups` are present, create the composition wrapper in the Page skeleton first. The following only illustrates information architecture; determine exact column widths, spacing, sticky behavior, and visual style together with the current Design System:

```html
<layout id="main">
  <placeholder id="Main">
    <main class="mx-auto max-w-7xl px-4 py-10">
      <section data-composition-group="product-detail-product-decision"
               class="grid gap-8 lg:grid-cols-[minmax(0,1.25fr)_minmax(20rem,0.75fr)] lg:items-start">
        <div data-block="product-detail-product-gallery"
             class="min-h-[32rem] animate-pulse rounded-lg bg-gray-100"></div>
        <aside data-block="product-detail-product-summary"
               class="min-h-80 animate-pulse rounded-lg bg-gray-50"></aside>
      </section>
      <section data-block="product-detail-related-products"
               class="mt-16 h-48 animate-pulse rounded-lg bg-gray-50"></section>
    </main>
  </placeholder>
</layout>
```

After the real View is complete, replace only the corresponding `data-block` position and keep the Page composition wrapper:

```html
<section data-composition-group="product-detail-product-decision"
         class="grid gap-8 lg:grid-cols-[minmax(0,1.25fr)_minmax(20rem,0.75fr)] lg:items-start">
  <view id="product_detail_product_gallery"></view>
  <aside><view id="product_detail_product_summary"></view></aside>
</section>
```

Default relationships for Commerce task pages:

- Product Detail: `product-gallery` main area + `product-summary` side area.
- Cart: `cart-items` main area + `checkout-summary` side area.
- Checkout: `checkout-form` main area + `checkout-summary` side area.
- Order Detail: `order-status` main area + `checkout-summary` side area.

On desktop, place them adjacent side by side by default; on mobile, stack them in semantic order with the main area first and side area second. AI may change proportions, whitespace, boundaries, sticky behavior, and visual treatment, but must not default these task areas to two consecutive full-width sections merely because the Views are independent.
