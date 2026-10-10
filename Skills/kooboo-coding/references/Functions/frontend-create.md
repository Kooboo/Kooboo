# Frontend Object Creation

Call `list_site_object` before creating; when an object with the same name exists, use [Read and update](read-update.md) instead of creating a duplicate with a random name.

## Parameters

| Function | Current fields |
|---|---|
| `create_layout` | `name`、`url`、`content` |
| `create_page` | `name`、`url`、`content`、optional `layoutName` |
| `create_view` | `name`、`content` |
| `create_style` | `name`、`url`、`content`; the name includes the suffix |
| `create_script` | `name`、`url`、`content`; the name includes the suffix |

Use concise names made of letters, digits, underscores, hyphens, or dots, up to 64 characters; URLs begin with `/`.

For `create_page.url`, use a dynamic path template for independently addressable detail pages when possible, such as `/blog/{slug}` or `/product/{seoName}`. Keep the placeholder name aligned with the detail page's `k-data` binding and use query strings for list state (search, filters, sorting, and pagination).

## Import an External Page

For an explicitly authorized single-page import, load `transfer_page` through `use_function`. It uses the site's transfer engine directly, imports into the current site, and requires approval. It does not create a new site or crawl multiple pages.

- `pageUrl` (required): source HTTP or HTTPS URL.
- `name` (optional): unused destination page name/route; omitted or empty derives the name from the source URL. Check existing pages/routes first; occupied routes are rejected, not overwritten.
- `headless` (optional, default `false`): enable when the source needs JavaScript rendering.

The call waits for the transfer and returns `taskId`, `finish`, `success`, `messages`, and `model` containing the imported page's `id` and `name`. Do not equate `finish=true` with success: confirm `success=true` and a returned page model before claiming the page was imported. On failure, report the returned messages or error instead of blindly retrying. Protected and invalid URLs are rejected. Read back the imported page and verify it in Preview; successful import alone does not prove that remote application behavior was preserved.

## Page / Layout Structure

The Layout owns site-wide chrome Views and `k-placeholder`. A Page's `<layout>` must be the root container around `<placeholder>`, and must match `layoutName` and the real Layout name; placeholder IDs must match `k-placeholder` casing.

```html
<layout id="main">
  <placeholder id="Main">
    <view id="home_hero"></view>
  </placeholder>
</layout>
```

Reference shared Header/Footer once in the Layout only; do not put them in the Page placeholder. A View outputs one section only and does not include a Page, Layout, or complete document shell.

## Page Plan Assembly

For initial planning, pass only `mode: "page-plan"` and `brief`. Later use the Page Plan `resolvedBrief`:

1. `design-system` obtains site-wide visual variables;
2. `page-item` obtains ordered `blockRefs[]` for the current page;
3. `view-batch` obtains prompts for the current page in one call;
4. Generate a View independently for each item with `baseContract + item.prompt`, using `recommendedViewName`;
5. Obtain shared blocks in a separate batch and reference them from the Layout.

The script selects the default first batch of Page Plan pages by entry points, core paths, action loops, and a unified budget. Pass `pages` only when the user explicitly specifies the current batch; reuse `resolvedBrief` and replace `pages` for the next batch.

## Creation Order

Site level: Layout → Style/Script → shared Views.

Page level: `page-item` → visible lightweight Page skeleton → open the Page and read `read_preview_diagnostics` → `view-batch` → create and read back each View → replace the skeleton → Preflight → refresh the Page. Do not obtain View prompts or create page-level Views before diagnostics pass.

Do not wait until every View is created to create the home page, and do not write Views from only a Page Plan summary. Read back key content after successful creation; use `update_text_object` when corrections are needed.\n
