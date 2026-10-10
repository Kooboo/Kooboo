# Template Binding Syntax

Kooboo templates combine server-evaluated `k-xxx` directives with native `href` URL interpolation.

::: tip Division of responsibility with k-data
- **Dynamic data fetching** (querying commerce/content/sqlite, reading request parameters, pagination and filtering): prefer [`k-data`](k-data/basic.md); do not use this section's `<script env="server">` to query storage through `k.*` directly.
- **This section covers** native `href` URL interpolation, template-binding syntax such as `k-if` / `k-for` / `k-content` / `k-attribute`, and supplemental server-side scripts when k-data cannot cover the need.
:::

## env="server"

Use `env="server"` only on `<script>` elements that genuinely need to execute KScript, such as reading a `k.*` API without a k-data mapping or importing a CodeBlock with `type="module"`.

::: danger Do not put it on ordinary containers
`k-if`, `k-for`, `k-content`, `k-attribute`, and `<k-data>` do not need `env="server"` on a parent element. Do not add it to ordinary UI containers such as `<div>`, `<section>`, or the outer element of a Page/Layout/View.

`env="server"` is inherited by descendants. If an ordinary client-side `<script>` is also inside the container, it may be processed as a server-side script, breaking frontend interactions such as clicks, login, and cart behavior.
:::

### Example

```html
<script env="server">
    var msg = "hello";
    var imageUrl = "/images/logo.png";
    var list = [{ title: "apple" }, { title: "orange" }];
</script>
<div>
    <div k-content="msg"></div>
    <img k-attribute="src imageUrl">
    <div k-for="item in list" k-content="item.title"></div>
</div>
```

### Standalone script example

If a script needs to define page-level variables for multiple child elements, declare `env="server"` separately:

```html
<script env="server">
    var msg = "hello";
</script>
<div k-content="msg"></div>
```

## k-if

Determines whether to render an element.

### Example

```html
<script env="server">
    var isShow = true;
</script>
<div k-if="isShow">hello</div>
```

## k-for

Renders elements in a loop.

::: warning When the data source is a `k.*` API
Array items are often runtime object proxies. Do not bind them after `item.foo = ...` or `{...item}` in KScript. See [KScript return objects and template binding](../K-Script/kscript-clr-objects.md); for derived classes and similar needs, prefer the class-object syntax for `k-attribute` below.
:::

### Example

```html
<script env="server">
    var list = [{ label: "apple" }, { label: "orange" }];
</script>
<div k-for="item in list" k-content="item.label"></div>
```

## k-content

Bind text content.

### Example

```html
<script env="server">
    var blogTitle = "hello";
</script>
<h1 k-content="blogTitle"></h1>
```

## Link URL binding (`href`)

Author link URLs with the native `href` attribute. Kooboo evaluates `{...}` placeholders in `href` through its URL-rendering path, including route and multilingual handling.
Each placeholder must be an existing variable or field path; precompute arithmetic or conditional results before inserting them into `href`.

```html
<a href="{linkHref}">About Us</a>
<a href="/detail/{itemId}">Detail</a>
```

When a link also needs another dynamic attribute, keep `href` native and bind only the remaining attributes through `k-attribute`:

```html
<a href="{linkHref}" k-attribute="class {active: isActive}">About Us</a>
```

## Dynamic attribute binding (`k-attribute`)

Use `k-attribute` for dynamic attributes other than `href`; single and multiple attributes use the same syntax. Separate multiple attributes with `;`.

### Example

```html
<script env="server">
    var imageUrl = "/images/logo.png";
    var linkHref = "/about";
    var isActive = true;
    var isChecked = false;
    var itemId = 123;
    var classObj = { active: true, 'text-danger': false };
    var attrsObj = { id: 'container', class: 'wrapper' };
</script>
<div>

    <!-- Link URLs use native href -->
    <a href="{linkHref}">About Us</a>
    <a href="/detail/{itemId}">Detail</a>

    <!-- Other dynamic attributes use k-attribute -->
    <img k-attribute="src imageUrl">

    <!-- String literal (quoted) -->
    <img k-attribute="src imageUrl; alt 'Logo'">

    <!-- Class object syntax: add a class when its condition is truthy -->
    <div k-attribute="class {active: isActive; checked: isChecked}">Content</div>

    <!-- Class array syntax -->
    <div k-attribute="class ['static', 'active']">Content</div>

    <!-- Class from an object variable (automatically unpacked) -->
    <div k-attribute="class classObj">Content</div>

    <!-- Static and dynamic classes coexist and are merged during rendering -->
    <a class="text-xs font-medium px-2 py-1"
       k-attribute="class {'text-white bg-blue-600': isActive, 'text-gray-500': !isActive}">
      Button
    </a>

    <!-- Attribute spread: omit the attribute name to expand all object keys -->
    <div k-attribute="attrsObj"></div>
</div>
```

### Dynamic attribute names

Use `[expr]` as the attribute name for runtime resolution.

```html
<script env="server">
    var dynamicKey = 'data-target';
    var targetValue = 'details';
</script>
<button k-attribute="[dynamicKey] targetValue">Open</button>
```

::: tip Suitable scenarios

- Image src and background-image URLs
- Dynamic class and style
- Custom data attributes
- Bulk attribute expansion
- Dynamic attribute names
:::

::: warning JavaScript operator concatenation is not supported
The `k-attribute` parser **does not execute JavaScript expressions**; do not concatenate with `+`:
```html
<!-- ❌ Incorrect -->
<div k-attribute="onclick 'editCategory(\'' + cat.id + '\')'"></div>
```
Use `{var}` brace interpolation instead:
```html
<!-- ✅ Correct -->
<button k-attribute="data-url /detail/{itemId}">Detail</button>
```
Brace interpolation `{var}` also **does not evaluate expressions** (such as `{pageIndex - 1}` or `{price * qty}`); it accepts only variable names or field paths. Precompute values when needed, then bind them.
:::

## k-label

Bind a multilingual label and render text automatically in the current language.

### Example

```html
<h1 k-label="title">默认标题</h1>
```

For more usage (such as combining `k-content` or `k-attribute` with `k.label()` and parameter replacement), see the `k-label` reference.

## type="module"

Integrate with a server-side CodeBlock.

### Example

```html
<script type="module" env="server">
    import { getBlogDetail } from "./Services.Blog";
    import { formatDate } from './Utils';
    const blogDetail = getBlogDetail();
    export {
        blogDetail,
        formatDate
    }
</script>
<div>
    <div k-content="blogDetail.title"></div>
    <div k-content="formatDate(blogDetail.created)"></div>
</div>
```
