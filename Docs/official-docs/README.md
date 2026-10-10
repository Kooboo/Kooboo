# Kooboo official documentation (markdown)

Markdown snapshot of the Kooboo product documentation, imported from the `kb-docs`
VitePress site so the pages are readable and reviewable in this repository.

| Locale | Location | Files |
| --- | --- | --- |
| English (primary) | this directory | 208 |
| Chinese | [`zh/`](zh) | 208 |

Both locales cover exactly the same sections and the same page paths:
`api/`, `cms/`, `templateEngine/`, `quick-start/`, plus `index.md`.
Start at [index.md](index.md) or [zh/index.md](zh/index.md).

## Source and scope

- Source: `kb-docs/docs/` — English lives under `docs/en/**`, Chinese under `docs/**`.
- Source revision: kb-docs `7237c72` (2026-10-09), snapshot taken 2026-10-10.
- Scope: only the sections that the documentation site actually publishes — `api/`,
  `cms/`, `templateEngine/`, `quick-start/`.

Deliberately **not** imported, with the upstream reason:

| Excluded | Why |
| --- | --- |
| `cli/`, `product/` | unpublished upstream (`PUBLISH_CLI` / `PUBLISH_PRODUCT_FEATURES` are `false` in `docs/.vitepress/config.mts`) |
| `guide/` | unpublished upstream (`PUBLISH_GUIDE = false`) |
| `superpowers/` | internal planning notes and specs, not product documentation |
| all Chinese sources upstream | `PUBLISH_ZH = false` — the Chinese pages are not live on docs.kooboo.com |
| `.vitepress/`, `public/`, `*.png`, `*.svg` | site scaffolding, theme assets, and images |

## Link rewriting

The source files are written for a VitePress site, where navigation links are
site-absolute routes (`/en/api/cache/`, `/cms/getting-started/create-site`). GitHub cannot
resolve those, so every route was mapped to the markdown file that backs it and rewritten
as a relative path, inside the same locale tree:

```
/en/api/cache/                       -> ../../api/cache/index.md
/api/cache/                          -> ../api/cache/index.md       (Chinese tree)
/cms/getting-started/create-site     -> ../getting-started/create-site.md
/cms/#section                        -> ../index.md#section          (anchors preserved)
```

| Result | Count |
| --- | --- |
| Links rewritten | 768 (374 English, 394 Chinese) across 196 files |
| Files whose prose changed | 0 — 663 changed lines, every one a link target |
| Changes inside ``` code blocks | 0 (example code is untouched) |
| Broken relative links remaining | 0 |

## Refresh

```bash
KB_DOCS=/Users/achen/Priv/Yardi/kb-docs/docs
REPO=/Users/achen/Priv/Yardi/kb-github-docs
SECTIONS="api cms templateEngine quick-start"

# English (source: docs/en)
cd "$KB_DOCS/en" && find $SECTIONS -name '*.md' -print0 | tar --null -T - -cf - \
  | (cd "$REPO/Docs/official-docs" && tar -xf -)
cp "$KB_DOCS/en/index.md" "$REPO/Docs/official-docs/index.md"

# Chinese (source: docs)
cd "$KB_DOCS" && find $SECTIONS -name '*.md' -print0 | tar --null -T - -cf - \
  | (cd "$REPO/Docs/official-docs/zh" && tar -xf -)
cp "$KB_DOCS/index.md" "$REPO/Docs/official-docs/zh/index.md"

# Then re-apply the link rewriting:
#   map each site route to its .md file in the SAME locale tree, make it relative
#   to the linking file, keep the #anchor, skip anything inside ``` / ~~~ fences,
#   and leave targets outside the imported scope untouched.
```

## Known limitations

- **14 references still point at `/product/*`**, which is out of the imported scope: 6
  markdown links and 8 HTML `href`s, all in the Chinese tree (`index.md`,
  `quick-start/index.md`, `quick-start/build-first-page.md`,
  `quick-start/create-first-site.md`, `quick-start/add-script-api.md`). They cannot resolve
  here. Import `product/` (and `cli/`, `guide/`) to close them.
- **No images.** The pages reference 9 English and 11 Chinese images through absolute
  `<img src="/...">` URLs; those files were not copied, so they only render on the
  published docs site. Rewriting them to `https://docs.kooboo.com/...` would make them
  render on GitHub.
- Example code keeps its own absolute paths: 4 `<link href="/assets/site.css">` tags and 2
  `<a href="/">` tags sit **inside ```html blocks** that demonstrate real Kooboo markup, so
  they were deliberately preserved. They are the only absolute paths left inside a fence.
- VitePress-only syntax (`::: tip` blocks, `<HomeRouteTabs>` on the home page) is
  preserved as-is, so the home page will not render as a designed landing page on GitHub.

Related content: [`Skills/`](../../Skills) documents Kooboo's AI skills, and
[`Tools/`](../../Tools) lists the AI chat's callable functions.
