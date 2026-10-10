# Multilingual Site Scenario Guide

Use this guide for scenarios that need frontend pages, multilingual labels, back-office configuration, AI-driven site-setting updates, server-side redirects, or language switching in a Kooboo Site.

## What This Capability Solves

Site multilingual support in Kooboo is not a single feature; it is collaboration across three surfaces:

- Frontend: static page copy, attribute copy, and the language switcher
- Backend / KScript: returning text in the current language and generating language-aware URLs and redirects
- Kooboo back office / AI base functions: enabling multilingual support, configuring languages, and maintaining Label content

If only `k-label` is written but multilingual support is not enabled in the back office, or a redirect still hard-codes `/cart`, the experience is still broken.

## Keep These Conclusions in Mind

- Use `k-label` for static page copy
- Use `k.label(...)` or `k.t(...)` for attributes, scripts, and API copy
- Before starting, call `get_site_info` to check whether multilingual support is enabled, along with the default language, language list, and URL mode
- When AI must directly change site settings, call `update_site_settings` to update `isMultiLingual`, `defaultCulture`, `cultures`, and `enableSitePath`
- `update_site_settings.cultures` replaces the complete collection rather than appending; return the full list when preserving existing languages
- The multilingual master switch is at `/_Admin/system/settings?SiteId=xxx`
- The Label management page is at `/_Admin/content/labels?SiteId=xxx`
- Prefer `k.site.multilingual.currentURLs()` for language switching
- Do not hand-write `/products?lang=en`, `/products?culture=en`, `/en/products`, or hard-coded in-site redirects; generate URLs consistently through `k.site.multilingual.*`

## Read the Current Site Configuration First

Before deciding whether to keep parameter mode or switch to router mode, or whether to add languages, call `get_site_info`; do not write multilingual logic from assumptions.

Read `siteSettings` in particular:

- `isMultiLingual`: whether multilingual support is currently enabled
- `defaultCulture`: the default language, for example `en`
- `cultures`: the languages supported by the current site
- `enableSitePath`: `false` means parameter mode (such as `/about?lang=en`), and `true` means router mode (such as `/en/about`)
- `baseUrl`: use only when a public frontend preview URL is needed for the user

Minimum example:

```json
{
  "siteSettings": {
    "name": "demo-site",
    "isMultiLingual": true,
    "defaultCulture": "en",
    "cultures": ["en", "zh"],
    "enableSitePath": false,
    "baseUrl": "https://example.com"
  }
}
```

## Updating Site Settings Directly with AI

When the user explicitly wants AI to enable or adjust the site's multilingual configuration rather than only provide a back-office path, call `update_site_settings`.

Fields suitable for direct updates:

- `isMultiLingual`: enable or disable multilingual support
- `defaultCulture`: set the default language
- `cultures`: set the language list
- `enableSitePath`: `false` for parameter mode and `true` for router mode

Usage rules:

- Call `get_site_info` first and update only after confirming the current state
- Once `cultures` is passed, it replaces the entire collection. If the current value is `["en", "zh"]` and `fr` should be added, pass `["en", "zh", "fr"]`, not only `["fr"]`
- `defaultCulture` should remain in sync with `Cultures` so the default language is not absent from the language list
- `update_site_settings` changes site settings only; it does not automatically create or complete Label content
- This is an approval-required operation and is appropriate when the user explicitly asks to change configuration directly

Minimum example:

```json
{
  "isMultiLingual": true,
  "defaultCulture": "en",
  "cultures": ["en", "zh"],
  "enableSitePath": false
}
```

If the user only needs to confirm or continue maintaining Labels in the back office, follow the back-office entry guidance below.

## Back-Office Entry Points

### 1. Enable Site Multilingual Support

Open:

```text
/_Admin/system/settings?SiteId=xxx
```

Multilingual settings are in the `Multilingual` group on the Basic page.

Key options:

- `Multilingual`: master switch
- `Default Language`: default language
- `Parameter Mode` / `Router Mode`: parameter or router mode
- `Default Language Prefix`: whether the default language also carries a prefix
- `Auto Detect Culture`: automatic language detection
- `HrefLang`: output multilingual SEO markers

### 2. Maintain the Label List

Open:

```text
/_Admin/content/labels?SiteId=xxx
```

This is the multilingual data source read by `k-label`, `k.label()`, and `k.t()`.

A label normally contains at least:

- `name`: the label key, such as `hero_title`
- `values.en`
- `values.zh`

## Defining Multilingual Content in the Frontend

### Text Nodes

```html
<h1 k-label="hero_title">Build faster with Kooboo</h1>
<p k-label="hero_desc">One site, multiple languages.</p>
```

### Attribute Values

```html
<input k-attribute="placeholder k.label('search_placeholder')">
```

### Label Parameters

The back-office label content can be:

```text
Welcome back, {name}
```

Read it as:

```html
<div k-content="k.label('welcome_user', { name: userName })"></div>
```

## Choosing among `k-label`, `k.label()`, and `k.t()`

- `k-label`: best for fixed text nodes
- `k.label()`: best for attributes, scripts, APIs, and response messages
- `k.t()`: reads the same Label system as `k.label()`, but is more suitable for quickly connecting existing copy to multilingual support; in the current implementation it automatically adds a label when one is missing

Recommended convention:

- Prefer `k-label` for fixed copy in page structure
- Prefer `k.label(...)` in logic and attribute layers
- Use `k.t(...)` only when quickly connecting existing copy to multilingual support

## Parameter Mode and Router Mode

### Parameter Mode

Parameter-mode URL shapes:

```text
/about?lang=en
/about?lang=zh
```

The current request language can come from the `lang` or `language` query parameter, but the system outputs `lang=...` by default when generating links.

`/about?lang=en` here is only a runtime output example, not a redirect string to hand-write. When AI writes a language switcher or redirect, it should still prefer `k.site.multilingual.currentURLs()`, `getLangUrl(...)`, or `getUrl(...)`.

Suitable for:

- Existing site URLs that are already public and should not change path structure
- Language switching that only needs to remain in the query string

### Router Mode

Router-mode URL shapes:

```text
/en/about
/zh/about
```

After it is enabled, Kooboo automatically adds the current language information to in-site links during rendering.

For example, the page may originally contain:

```html
<a href="/about">About</a>
```

In router mode, the rendered result changes according to the current language:

```html
<a href="/en/about">About</a>
```

or:

```html
<a href="/zh/about">About</a>
```

Again, `/en/about` and `/zh/about` are final rendered results, not path templates that AI should hand-write.

Whether the default language has a prefix depends on `Default Language Prefix`:

- Disabled: the default language normally remains `/about`
- Enabled: the default language also becomes `/en/about`

### An Important Constraint in the Current Implementation

The core matching and link-concatenation logic for router mode uses the key of `WebSite.Culture`, which is the language code itself.

Therefore, use these defaults when generating pages:

- `en` maps to `/en/...`
- `zh` maps to `/zh/...`

Do not assume that the current project supports mapping `en` to a custom path prefix such as `/english/...`.

The current settings page and backend implementation also favor two-letter language codes; write values such as `en` and `zh` by default.


## Recommended Language-Switching Pattern

### Recommended option: `currentURLs()`

This is the safest approach because it returns the URL for each language directly from the current site configuration.

::: warning Do not mutate or spread API return items
The elements returned by `currentURLs()` are **runtime object proxies**, not ordinary JavaScript objects. Do not assign `item.linkClass = ...` inside `forEach`, and do not use `{ ...item, linkClass: ... }` inside `map` (this causes camelCase fields such as `item.key` to fail in templates). Use the `k-attribute` class object below for styling, or explicitly construct a plain object as described in [KScript Return Objects and Template Binding](../K-Script/kscript-clr-objects.md).
:::

```html
<div>
    <script env="server">
        var links = k.site.multilingual.currentURLs()
    </script>

    <ul class="language-switcher">
        <li k-for="item in links">
            <a href="{item.URL}" k-attribute="class {'is-active': item.isActive}" k-content="item.name"></a>
        </li>
    </ul>
</div>
```

Available fields normally include:

- `item.key`: language code, such as `en` or `zh`
- `item.name`: language display name
- `item.URL`: address after switching to that language
- `item.isActive`: whether it is the current language

## Backend / KScript / API Cooperation

### Returning a Message in the Current Language from an API

```ts
k.api.post((body) => {
    return {
        success: true,
        message: k.label("order_created", {
            orderNo: body.orderNo
        })
    }
})
```

### Preserving the Current Language in a Redirect

Do not write:

```ts
k.response.redirect("/cart")
```

Use:

```ts
var redirectUrl = k.site.multilingual.getUrl("/cart")
k.response.redirect(redirectUrl)
```

This allows the redirect to follow the current language in either parameter or router mode.

### Redirects after Login / Registration / Ordering

For all of these actions:

- Return after successful login
- Go to the home page after registration
- Go to a thank-you page after form submission
- Go to `/cart` after adding to cart
- Go to `/checkout` or `/order-success` after ordering

Generate the destination URL through `k.site.multilingual.getUrl(...)` rather than concatenating a string manually.

## Recommended Workflow

### When AI Writes a Page

1. Mark all fixed copy with `k-label`.
2. Change `placeholder`, `title`, `aria-label`, and `alt` to `k.label(...)`.
3. If there is a language switcher, prefer `k.site.multilingual.currentURLs()`.
4. If the page contains redirect logic, consistently use `k.site.multilingual.getUrl(...)`.

### When AI Configures Multilingual Support

1. Call `get_site_info` first and confirm `isMultiLingual`, `defaultCulture`, `cultures`, and `enableSitePath`.
2. If the user explicitly asks AI to change configuration directly, call `update_site_settings`.
3. If the user must confirm the back-office configuration, open `/_Admin/system/settings?SiteId=xxx`.
4. Enable `Multilingual` on the Basic page and choose the default language and `Parameter Mode` / `Router Mode`.
5. Then open `/_Admin/content/labels?SiteId=xxx` and complete labels for each language.

### When AI Runs Preflight

For multilingual objects, pass `multilingual: true` explicitly when appropriate so Preflight also checks the rule against hand-written multilingual URLs / redirects.

```json
{
  "taskType": "dynamic-page",
  "layers": ["frontend", "kooboo-template", "kscript", "integration"],
  "artifacts": [
    {
      "name": "checkout",
      "objectType": "Page",
      "multilingual": true
    }
  ]
}
```

This check focuses on blocking:

- `href="/products?lang=en"`
- `href="/products?culture=en"`
- `href="/en/products"`
- `location.href = "/cart"`
- `k.response.redirect("/checkout")`

## Practical Example

Here is a minimal usable combination:

```html
<div>
    <script env="server">
        var links = k.site.multilingual.currentURLs()
        var searchPlaceholder = k.label("search_placeholder")
    </script>

    <header class="site-header">
        <a href="/" class="logo" k-label="site_name">Kooboo Store</a>

        <nav>
            <a href="/about" k-label="nav_about">About</a>
            <a href="/pricing" k-label="nav_pricing">Pricing</a>
            <a href="/cart" k-label="nav_cart">Cart</a>
        </nav>

        <input k-attribute="placeholder searchPlaceholder">

        <ul class="language-switcher">
            <li k-for="item in links">
                <a href="{item.URL}" k-attribute="class {'is-active': item.isActive}" k-content="item.name"></a>
            </li>
        </ul>
    </header>
</div>
```

## Common Mistakes

### Mistake 1: Using `k-label` on `<input>`

Wrong:

```html
<input k-label="search_placeholder">
```

Correct:

```html
<input k-attribute="placeholder k.label('search_placeholder')">
```

### Mistake 2: Hand-building `/en/...` in Router Mode

Wrong:

```html
<a href="/{culture}/about">About</a>
```

Correct approach:

- Write ordinary in-site links as `href="/about"` and let the system render them
- Use `k.site.multilingual.getUrl("/about")` for logic redirects
- Use `k.site.multilingual.currentURLs()` or `getLangUrl(...)` to switch languages

### Mistake 3: Hand-building `?lang=` or `?culture=` in Parameter Mode

Wrong:

```html
<a href="/products?lang=en">English</a>
<a href="/products?culture=en">English</a>
```

```js
location.href = "/cart?lang=en"
k.response.redirect("/checkout?culture=zh")
```

Correct approach:

- Use `k.site.multilingual.currentURLs()` first when switching the current page's language
- Use `k.site.multilingual.getLangUrl("/products", "en")` to switch to a specified page in a specified language
- Use `k.site.multilingual.getUrl("/cart")` for a logical redirect that preserves the current language
- Even when you know parameter mode will ultimately output `?lang=...`, do not concatenate it manually

### Mistake 4: Enabling Multilingual Support without Completing Labels

Enabling multilingual support only means that the site supports multiple languages; it does not automatically generate all business copy. What is ultimately displayed still depends on whether the Label list contains the corresponding key and values for each language.

### Mistake 5: Treating `cultures` as an Incremental Append

Incorrect understanding:

- The current value is `["en", "zh"]`.
- To add `fr`, pass only `["fr"]`.

This directly replaces the site's language list so that only `fr` remains.

Correct approach:

- Call `get_site_info` first.
- Build the complete target list from the current value.
- Call `update_site_settings` with the complete list, for example `["en", "zh", "fr"]`.

## Related References

- [Site Multilingual Label Development Guide](../Frontend/k-label.md)
- [Template binding](../Frontend/template-binding-syntax.md)
