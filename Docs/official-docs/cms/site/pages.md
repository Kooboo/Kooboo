# Pages

> Menu: top-left **Pages**  
> Deep link: `/_Admin/site/pages?SiteId={siteGuid}`

Manage all accessible page resources in the site: list maintenance, routing, code editing, and inline editing. For page syntax, see [Template Engine -> Page](../../templateEngine/page/index.md). For scripts, see [k.site.pages](../../api/site/page.md).

::: tip Prerequisite
It helps to understand [Media library](./media.md) and [Dashboard](./dashboard.md) first. If you want to create a layout page, prepare at least one layout under **Development -> Layouts** first.
:::

## Open the page

1. Enter a site admin.
2. Click **Pages** in the left toolbar.
3. The URL is `/_Admin/site/pages?SiteId=...`.

<DocImage src="/cms/site/pages-list.png" alt="Page list" width="1120" />

## List page

| Column | Description |
|--------|-------------|
| Name | Page resource name. The home icon marks the start page or home link. |
| Link | Whether the page is linked from other resources |
| Online | Whether the page is published |
| References | Which layouts, views, or content items use it |
| Last modified | Last save time |
| Preview | The front-end path; open it in a new window |

You can switch between **Pages** and **Page Tree**:

- **Pages** is a flat list, better for search, copy, and delete.
- **Page Tree** shows hierarchical structure and supports advanced filtering.

## Toolbar

| Action | Description |
|--------|-------------|
| **Create Page** | Choose a page type and start the creation flow |
| **Create Designer Page** | Create a visual design page based on a layout |
| **Import** | Import pages from a package or external source |
| **Route Settings** | Manage routing rules across multiple pages |

## Create page

The **Create Page** menu usually includes:

| Type | Description |
|------|-------------|
| **HTML Page** | Standalone HTML, optional layout |
| **Rich Text Page** | Content managed through the rich text editor |
| **{Layout name} Page** | Uses a specific layout and fills its placeholders |

For an **HTML Page**, enter a page name and URL first. For a **Layout Page**, the system also passes `layoutId`.

## Row actions

Each page usually has three entry points:

| Entry | Doc | Purpose |
|-------|-----|---------|
| **Settings** | [Page settings](./pages-setting.md) | URL, title, meta, page-level styles/scripts, cache |
| **Edit Code** / **Design** | [Edit code](./pages-edit-code.md) | Change the HTML body or open the visual designer |
| **Inline Edit** | [Inline edit](./pages-inline-edit.md) | Edit visible text, images, links, and local styles in the rendered page |

::: tip Which one should you use?
- **Settings** for route, SEO, publish state, and asset references.
- **Edit Code** for page structure, placeholders, and view insertion.
- **Inline Edit** for small visible content changes on a live page.
:::

## Typical workflow

1. Create an HTML page or a layout page.
2. Use [Edit code](./pages-edit-code.md) to add structure.
3. Use [Page settings](./pages-setting.md) to confirm URL and publish state.
4. Preview the page.
5. Use [Inline edit](./pages-inline-edit.md) for later content tweaks.

## Related

| Doc | Description |
|-----|-------------|
| [Dashboard](./dashboard.md) | Page totals in My Resources |
| [Media library](./media.md) | Images used in pages |
| [Content folders](../content/contents-folders-list.md) | Often paired with detail pages |
