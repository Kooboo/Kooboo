# Frontend Code Style Guide

## Specialized guide entry

When the user explicitly requests client-rendered Vue, Vue 3 CDN, no-build Vue, or a browser-run Vue application built with Kooboo Page, Style, and Script, you must first read the [Kooboo client-rendered Vue development guide](cdn-vue/index.md).

Client-rendered Vue is a complete HTML Page without a Layout. Its Vue import map, Tailwind v3 CDN, ES Modules, Style and Script object rules take precedence over this document's Tailwind inclusion method for Layout/View site building and its static-stage restrictions. HTML semantics, naming, and accessibility rules not covered by the specialized guide still apply.

## Styling approach

- **Write Tailwind utility classes directly in HTML when possible** to reduce duplicated layout code in standalone CSS files.
- Before using Tailwind, **ensure the CDN is included in the Layout's `<head>`**: `<script src="https://www.kooboo.com/tailwindcss.js"></script>`.
- Define custom configuration through `window.tailwind.config` in a separate `<script>` after the Tailwind CDN `<script>`; this order is mandatory because configuration written before the CDN does not take effect.
- When hand-written CSS is needed (complex animations, custom gradients, special background textures, and more), add it in Style as needed.

### Tailwind compatibility

The loaded Tailwind CSS version is 3.4.17. Tailwind Preflight resets the browser-default appearance of buttons, form controls, headings, lists, tables, media, and other elements. Do not rely on native browser styling; explicitly provide the styles required by the design.

## HTML conventions

- Use four spaces for indentation.
- Prefer semantic tags (`<header>` `<main>` `<section>` `<nav>` `<footer>`) and avoid piles of plain `<div>` elements.
- Attribute order: `id` → `class` → `data-*` → other attributes → `aria-*`.
- Do not add `/` to void elements (HTML5 style).

## Naming convention

- HTML/CSS: kebab-case (`class="hero-section"`, `id="main-nav"`).
- KScript variables: camelCase (`var siteTitle = '...'`).
- View/Page ids: snake_case (`<view id="product_list">`).

## Display identifiers and detail URLs

- Use short, readable, stable business codes, short IDs or slugs for user-visible record identifiers and detail URLs, for example `POST-1024`, `/post/1024` or `/post/spring-guide`. Avoid displaying GUIDs/UUIDs or generating addresses such as `/post/bb2db063-12c2-9bf2-97f9-c9dfd4053a85`. This also applies to hash routes.
- Internal primary keys may remain GUIDs, with a separate persisted `publicId`, `code` or `slug`. Enforce uniqueness within the relevant site/business scope on the backend and handle collisions. Do not simply truncate GUIDs, use list positions, or regenerate identifiers during rendering. Title edits must not automatically change an existing slug.
- List links, route parameters and detail APIs must share the same public identifier contract, resolved to the actual record on the server. Shortening only the displayed text or changing links alone is insufficient. Readable identifiers do not authorize access; detail/update/delete APIs still check record permissions.
- When maintaining existing applications, preserve published links and external API contracts. If migration is needed, support old identifiers or redirects rather than rewriting existing primary keys in bulk.

## View structure

- Wrap each View's outermost layer in a semantic container with a `class` to avoid polluting the global namespace.
- **Static View**: HTML structure → `<script>` (frontend interaction); use a standalone `<script env="server">` only when server-side constants/configuration are genuinely needed. Static work may include JS/CSS interactions for mobile menus, FAQs, tabs, galleries, static filters, hover/focus/active states, and other interactions independent of the backend.
- **Dynamic View**: **`<k-data>` (fetch/query/shape data)** → direct HTML bindings with `k-xx` → `<script>` (frontend interaction); **do not** add `env="server"` to an outer element, and do not replace k-data with `<script env="server">` calls to `k.commerce.*` / `k.content.*` / `k.DB.*`.

## Script placement

- **Dynamic data fetching**: prefer `<k-data>`, placed at the top of the View before the HTML structure.
- KScript with `env="server"` + `type="module"`: place it at the top of the block only when a CodeBlock/Service must be imported or k-data cannot cover the need.
- Frontend interaction JS: place it at the bottom of the View or in an independent Script resource, and never inside an `env="server"` ancestor. During static site-building stages, JS may operate only on the current DOM, classes, ARIA state, and visibility; `fetch`, `k.*`, real submissions, persistence, and business API calls are forbidden.

## Images

- Use ImagePlaceholder URLs for placeholders in all `<img>` and CSS `background-image`; do not find external links yourself.

For website and App imagery and editable image/attachment fields, follow [Images and uploads](images-and-files.md): provide selection/preview/upload instead of an image-URL-only input, with reference tracking and cleanup for persisted files. Static site stages retain their existing prohibition on real submissions.
