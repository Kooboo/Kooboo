# Code Search

> Menu: **Development → Code Search**  
> Deep link: `/_Admin/development/code-search?SiteId={siteGuid}`

**Full-text search** across code and template bodies in the site: pages, View, Layout, Script, Style, Code, HtmlBlock, forms, content, and more. Lists hit line numbers and snippets; **Edit** opens the corresponding editor in a new tab.

::: tip Permissions
Menu permission: `code·view`. Enable **Development → Code Search** in Edit Menu (`advanced`). Row **Edit** icon also needs `view` permission for that object type (e.g. `page·view` for pages).
:::

## How to Open

1. In [Edit Menu](../navigation.md#edit-menu), enable **Development → Code Search**.
2. Open **Development → Code Search** in the left sidebar.

<DocImage src="/cms/development/code-search-overview.png" alt="Code search page overview" width="1120" />

## Search box

| Control | Description |
|------|------|
| Keyword input | `codeStore.keywords`, placeholder "Search code"; debounced ~1s then requests `CodeSearch/Search` |
| **Aa** | Toggle **case-sensitive** (`ignoreCase` inverted; highlighted = case-sensitive) |
| **.\*** | Toggle **use regular expression** (`isRegex`); invalid regex does not trigger search |

Empty keyword clears results. Leaving this page when target route has no `id` clears the keyword (avoids carrying it to other pages).

Shortcuts (same as buttons beside input): **case-sensitive**, **use regular expression**.

<DocImage src="/cms/development/code-search-toolbar.png" alt="Code search box with Aa and regex toggles" width="1120" />

## Results list

No pagination; when there are matches, table shows:

| Column | Description |
|----|------|
| **Name** | Object display name |
| **Type** | e.g. `Page`, `View`, `Code`, `Layout`, `Script`, `Style`, etc. |
| **Line** | One row per hit: `Line N:` + snippet (`matched[].lineNumber` is 0-based; UI shows N+1) |
| **Edit** | Pencil icon opens that object's edit route in a **new tab** (pages jump to designer/rich text/layout editor by page type) |

<DocImage src="/cms/development/code-search-results.png" alt="Code search results list" width="1120" />

### Edit navigation (by type)

| Type | Typical route |
|------|----------|
| `Page` | By `params.type`: layout designer, rich text, layout page editor, etc. |
| `View` / `Layout` / `Script` / `Style` / `Code` | Corresponding `view-edit`, `layout-edit`, etc. |
| `HtmlBlock` | `htmlBlock-edit` |
| `Form` | `form-edit` |
| `TextContent` | Content edit |

## Related

| Doc | Description |
|------|------|
| [Development overview](./index.md) | Development menu index |
| [Code](./code.md) | Code file management |
| [Code log](./code-log.md) | Runtime logs |
