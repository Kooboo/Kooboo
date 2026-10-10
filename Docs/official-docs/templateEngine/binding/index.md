# Template Binding Syntax

> Server-side `k-*` directives for binding data and attributes inside Layout, Page, and View templates.

Kooboo template bindings such as `k-if`, `k-for`, `k-content`, and `k-attribute` are evaluated on the **server** automatically. Use **`env="server"`** only on a `<script>` that needs to execute KScript. These bindings are meant for HTML resources such as [Page](../page/) and [View](../view/), not for Vue or React running in the browser.

## `env="server"` {#env-server}

Executes a `<script>` as server-side KScript. Put the attribute directly on the script that needs it.

::: warning Do not put it on ordinary containers
`k-if`, `k-for`, `k-content`, and `k-attribute` do not need `env="server"` on a parent element. Although the server environment is inherited by descendants, putting it on a `<div>`, `<section>`, or other UI container can cause a nested browser script to be processed as KScript.
:::

```html
<script env="server">
    var msg = "hello";
    var imageUrl = "/images/logo.png";
    var list = [{ title: "apple" }, { title: "orange" }];
</script>
<div k-content="msg"></div>
<img k-attribute="src imageUrl">
<div k-for="item in list" k-content="item.title"></div>
```

Page-level variables can also live directly on a script tag:

```html
<script env="server">
    var msg = "hello";
</script>
<div k-content="msg"></div>
```

## `k-define`

Declares a value in the template for the current element and subsequent bindings. The syntax is `name value`. Quote strings and wrap JSON objects in backticks.

```html
<div k-define='profile `{"name":"Kooboo","role":"admin"}`'>
    <strong k-content="profile.name"></strong>
    <span k-content="profile.role"></span>
</div>
```

## `k-if`

Renders an element only when the condition is true.

```html
<script env="server">
    var isShow = true;
</script>
<div k-if="isShow">hello</div>
```

## `k-else-if`

Tests another condition when the preceding sibling `k-if` branch does not render. Keep all branches at the same DOM level and in branch order.

```html
<script env="server">
    var isAdmin = false;
    var isMember = true;
</script>

<p k-if="isAdmin">Administrator</p>
<p k-else-if="isMember">Member</p>
<p k-else>Guest</p>
```

## `k-else`

Renders when the preceding sibling `k-if` and `k-else-if` branches do not render. It does not need an attribute value.

```html
<p k-if="isLoggedIn">Welcome back</p>
<p k-else>Please sign in</p>
```

## `k-for`

Renders a loop.

```html
<script env="server">
    var list = [{ label: "apple" }, { label: "orange" }];
</script>
<div k-for="item in list" k-content="item.label"></div>
```

## `k-content`

Binds text content.

```html
<script env="server">
    var blogTitle = "hello";
</script>
<h1 k-content="blogTitle"></h1>
```

## `k-attribute`

The unified syntax for attribute binding. Separate multiple attributes with `;`.

```html
<script env="server">
    var imageUrl = "/images/logo.png";
    var linkHref = "/about";
    var isActive = true;
    var itemId = 123;
    var classObj = { active: true, 'text-danger': false };
    var attrsObj = { id: 'container', class: 'wrapper' };
</script>

<div>
    <a k-attribute="href linkHref">About Us</a>
    <img k-attribute="src imageUrl">
    <img k-attribute="src imageUrl; alt 'Logo'">
    <a k-attribute="href /detail?id={itemId}">Detail</a>
    <div k-attribute="class {active: isActive}">Content</div>
    <div k-attribute="class classObj">Content</div>
    <div k-attribute="attrsObj"></div>
</div>
```

Dynamic attribute names:

```html
<a k-attribute="[dynamicKey] linkUrl">About</a>
```

::: warning JavaScript expression concatenation is not supported
`k-attribute` does **not** execute operators such as `+`. For path building, use brace interpolation:

```html
<a k-attribute="href /detail?id={itemId}">Detail</a>
```

`{var}` only accepts a variable name or field path. Expressions such as `{pageIndex - 1}` are not supported. Calculate the value first, then bind it.
:::

## `k-label`

Renders a label in the current language.

```html
<h1 k-label="title">Default title</h1>
```

See also [k.label / k.t](../../api/label/index.md).

## `type="module"` {#type-module}

Server-side module scripts can `import` other CodeBlocks and `export` symbols back to the template:

```html
<script type="module" env="server">
    import { getBlogDetail } from "./Services.Blog";
    const blogDetail = getBlogDetail();
    export { blogDetail }
</script>
<div k-content="blogDetail.title"></div>
```

## Related

- [Page](../page/) and [View](../view/)
- [k-data](../k-data/) - declarative data flow that replaces `k-query`
