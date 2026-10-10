# Content

> Core routes: `/_Admin/content/contentTypes?SiteId=...` and `/_Admin/content/contents?SiteId=...`

Structured site data such as articles, banners, product copy, and reusable text is managed in the **Content** section. Frontend templates and scripts then read that data through [k.content](../../api/content/index.md) or `k-data`.

## Two Core Concepts

| Concept | Admin menu | Purpose |
|---------|------------|---------|
| **Content Type** | Content -> **Content Types** | Defines the field model: name, control type, multilingual behavior, validation, and more |
| **Content Folder** | Content -> **Content** | Holds entries that use one content type |

Always create the **content type first**, then create the **content folder** that uses it.

```mermaid
flowchart LR
  A[Content Type] --> B[Content Folder]
  B --> C[Content Entry]
  C --> D[k.content / templates]
```

## Main Submenus

| Menu | Route | What it is for |
|------|-------|----------------|
| Content | `/content/contents` | Content folders and content entries |
| Content Types | `/content/contentTypes` | Field schema and editor behavior |
| Labels | `/content/labels` | Multilingual labels and short UI text |
| HTML Blocks | `/content/htmlblocks` | Reusable HTML fragments |
| Files | `/content/files` | Site files in the CMS file library |
| Tag Attributes | `/content/text` | Editable template attributes exposed through `k-config` |
| Param Config | `/content/useroptions` | Structured parameters used by `k.paramConfig` |

## Recommended Flow

1. [Content types](./content-types.md): define fields such as title, body, image, date, and status.
2. Create a content folder that uses the content type.
3. Add content entries under that folder.
4. Read the data from templates or scripts via [k.content](../../api/content/index.md).

## Related

| Doc | Description |
|-----|-------------|
| [Content types](./content-types.md) | Field modeling and editor behavior |
| [k.content](../../api/content/index.md) | CRUD and query examples |
| [Template Engine](../../templateEngine/index.md) | How templates consume structured content |
