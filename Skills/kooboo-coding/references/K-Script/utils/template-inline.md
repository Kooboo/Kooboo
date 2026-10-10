# k.utils.template / k.utils.inlineHtml

> View rendering and inline page HTML

## k.utils.template

Render a template string. A simple empty `<view id="..."></view>` input takes the direct View-rendering path; other template strings use the normal template evaluator.

### render()

```ts
const html = k.utils.template.render("<view id='product-card'></view>");
```

The fast path requires a simple empty View tag with only an `id` attribute. It is not a general requirement for `render()` input.

Use it to assemble HTML fragments in an API or CodeBlock.

Other exposed methods:

| Method | Contract |
| --- | --- |
| `renderView(viewNameOrId)` | Renders a View and returns its HTML; returns `null` when the View is not found. |
| `executeView(viewNameOrId)` | Executes a View into the current response and returns `void`; a missing View produces no output. |
| `renderPage(url)` | Renders a matched Page with a cloned request context. An empty or unmatched URL returns an empty string; missing page data returns `null`. |

---

## k.utils.inlineHtml

Convert a Site **Page** into standalone inline HTML, commonly for previews or pre-screenshot processing. The implementation attempts to inline resolvable site-local images, files, scripts, styles, and CSS image URLs; external HTTP(S), protocol-relative URLs, and data URIs remain external.

### convertPageInline()

Find by the Page name or ID:

```ts
const html = k.utils.inlineHtml.convertPageInline("home");
```

### convertPageUrlInline()

Find by the Site's **relative URL**. The URL selects the Page; it is not used to preserve the full original request context during inlining:

```ts
const html = k.utils.inlineHtml.convertPageUrlInline("/about");
```

Returns `null` when the page is not found.

## Related modules

- Screenshots: `takePage` in [screenshot.md](screenshot.md) uses inline HTML internally
