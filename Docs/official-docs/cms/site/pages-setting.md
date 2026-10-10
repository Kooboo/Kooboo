# Page Settings

> Open from a row in [Pages](./pages.md) with **Settings**  
> HTML page: `/_Admin/page/setting?SiteId={siteGuid}&id={pageId}`  
> Layout page: `/_Admin/page/layout-setting?SiteId=...&id=...&layoutId={layoutId}`

Page settings manage route, meta, page-level assets, and cache beside a live preview. This page does not edit the HTML body itself.

## Open the page

1. Open [Pages](./pages.md).
2. Find a page row and click **Settings**.
3. The route becomes `page/setting` or `page/layout-setting`.

<DocImage src="/cms/site/pages-setting.png" alt="Page settings with preview and settings sidebar" width="1120" />

## Layout

| Area | Description |
|------|-------------|
| Left panel | Live preview of the current page |
| Header | Page name |
| Right sidebar | Settings panels and language switching when multilingual is enabled |
| Footer | **Back**, **Save**, **Save and Back** |

## Setting groups

### Basic

| Field | Description |
|-------|-------------|
| URL | The default-language route, which must be unique |
| Multilingual URL | Per-language routes when multilingual is enabled |
| Content title | Page title values used by the page and metadata |
| Publish | Whether the page is live |

### HTML Meta

Manage `<head>` metadata such as description, keywords, or Open Graph tags.

### Parameters

Configure URL parameters and bindings used by the page.

### Styles

Attach site style resources to this page only. You can sort, insert, and remove them.

### Scripts

Attach site script resources to this page only.

### Cache

| Option | Description |
|--------|-------------|
| Enable cache | Whether this page output is cached |
| Device / language / version | Cache-key dimensions |
| Cache minutes | Expiration time |
| Query keys | Query parameters included in the cache key |

## Choose the right editor

| Need | Use |
|------|-----|
| URL, title, meta, publish, cache | **Page Settings** |
| HTML structure, layout placeholders, views | [Edit code](./pages-edit-code.md) |
| Visible text, images, links, simple style adjustments | [Inline edit](./pages-inline-edit.md) |

## Related

| Doc | Description |
|-----|-------------|
| [Pages](./pages.md) | Entry and list actions |
| [Edit code](./pages-edit-code.md) | Edit the page body in Monaco |
| [Template Engine -> Page](../../templateEngine/page/index.md) | Page and layout syntax |
