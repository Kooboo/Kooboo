# Style

> Routed `.css` resources for global themes, patches, and Layout `<head>` references.

## What It Is

**Style** is a separately stored CSS file in a Kooboo site. Once registered, it is available through a **URL** such as `/assets/site.css`. Use it for site-wide themes, component-library patches, or styles that should not live inside each individual Page.

It is managed by **`k.site.styles`**. See [k.site.styles](../../api/site/css.md).

## Referencing It in Templates

Inside the `<head>` of a Layout, or in a Page when needed:

```html
<link rel="stylesheet" href="/assets/site.css" />
```

The `href` is the public path of the Style resource. On the server, `k.site.styles.getUrl(id)` returns the relative URL.

## Style vs Tailwind vs Inline CSS

| Approach | Notes |
|----------|-------|
| Tailwind CDN | Commonly loaded in a Layout with `https://www.kooboo.com/tailwindcss.js`, then utility classes are written directly in HTML |
| Style resource | Good for complex animation, global variables, and third-party component patches such as Element Plus plus Tailwind specificity fixes |
| `<style>` inside a Page or View | Best when the rule only affects one page or one isolated fragment |

For most layout-oriented styling, prefer Tailwind first. Use a Style resource for rules that repeat, need central maintenance, or are awkward to keep inline.

## Symmetry with Script

| | Style | Script |
|---|------|--------|
| Tag | `<link rel="stylesheet">` | `<script src="...">` |
| Common location | Layout `<head>` | Layout `<head>` or near the end of a Page |
| API | `k.site.styles` | `k.site.scripts` |

Both support routed URLs and version history. Layout and View resources do **not** have standalone URLs.

## Related

- [Layout](../layout/)
- [Script](../js/)
- [k.site.styles](../../api/site/css.md)
