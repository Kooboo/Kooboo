# Element Attributes

> Menu: **Content → Element attributes** (route `text`; must be enabled in **Edit Menu**)  
> Deep link: `/_Admin/content/text?SiteId={siteGuid}`

**Element attributes** (engine **KConfig**) let you **centrally edit** editable HTML tag attributes in templates (e.g. `href`, `src`, inner HTML) without changing Page/View source each time. Entries come from **`k-config`** markers in templates; you **cannot create** a config from scratch on this page—there is no **New** button.

::: tip Niche but useful
Good for “footer link URL”, “image `src`”, “replaceable block HTML” reused across pages that operators should change in the admin. For short multilingual copy, use [labels](./labels.md); for a full reusable HTML block, use [HTML blocks](./html-blocks.md).
:::

::: warning Unrelated to “Labels” and content entries
| | **Element attributes** (this page) | [Labels](./labels.md) | [Content entries](./contents-entries.md) |
|--|-----------------------------------|------------------------|------------------------------------------|
| Route | `/content/text` | `/content/labels` | `/content/content` |
| Data | KConfig: HTML attribute key-values | Label: multilingual strings | TextContent: content folder fields |
| Template marker | `k-config="key"` | `k.label()` / `k.t()` | `k.content` / k-data |
:::

## How It Works

Add **`k-config="uniqueKey"`** (or `v-config`, same engine binding) on HTML in Page, Layout, or View. When the site **renders or saves** related pages, Kooboo registers or updates the KConfig record and extracts editable attributes into a **binding** dictionary (e.g. `href`, `src`, `innerHtml`).

On render, the `k-config` attribute itself is not in final HTML; binding values are written back to the corresponding attributes.

```html
<!-- Example: footer link, key footer-home -->
<a k-config="footer-home" href="/" class="footer-link">Home</a>

<!-- Example: image src editable here or via media library -->
<img k-config="logo-main" src="/images/logo.png" alt="Logo" />
```

::: tip Which attributes become bindings?
Generally **normal HTML attributes** (`href`, `src`, `class`, `id`, etc.) are included; **`style`**, **`on*` events**, and names with **`-` / `_`** usually stay static and are not edited here (per engine `IsIgnoreAttribute`). Non–self-closing tags also include **`innerHtml`** (inner HTML).
:::

## How to Open

1. In [Edit Menu](../navigation.md#permissions-and-edit-menu), check **Element attributes**.
2. Open **Content → Element attributes** in the left sidebar.
3. If the list is empty, add `k-config` in templates and save/visit related pages so the system generates entries.

<DocImage src="/cms/content/tag-attributes-list.png" alt="Element attributes list" width="1120" />

## List Page

| Column / action | Description |
|-----------------|-------------|
| Name | KConfig **key** (`k-config` value) or display name; subtitle shows first binding summary (e.g. `href: /`) |
| Preview | For **`img`** tags with **`src`** in binding, shows thumbnail |
| Tag name | Original HTML tag, e.g. `<a>`, `<img>`; hover for `tagHtml` snippet |
| References | Referenced by which Page / Layout / View, etc. |
| Last modified | Update time |
| **Edit** | Change bindings; for `img` **`src`**, opens [media library](../site/media.md) picker |
| **Version** | Version history (when permissions allow) |

Supports multi-select **batch delete** (permission `text` · `delete`).

## Edit Dialog

Title **Element attributes**. One row per **binding key** (multiline text), for example:

| Key | Meaning (common) |
|-----|------------------|
| `href` | Link URL |
| `src` | Image/resource URL |
| `innerHtml` | Inner HTML (non–self-closing tags) |
| `class` / `id`, etc. | Other editable attributes |

Save writes to site KConfig; subsequent requests render new values.

::: tip `img` and `src`
When tag name is **`img`**, editing **`src`** opens the **media library** picker directly (same as list row logic); selected image writes back to binding.
:::

<DocImage src="/cms/content/tag-attributes-edit.png" alt="Edit element attributes: href, innerHtml bindings" width="960" />

## With Inline Edit

In [inline edit](../site/pages-inline-edit.md), selecting an element with `k-config` can open the same edit dialog (prompt usually indicates immediate effect). Change links or images on the preview page without opening this list.

## Typical Flow

1. Developer adds `k-config="promo-banner"` and initial `href`, `innerHtml`, etc. on View or Page.
2. Save and preview/publish so KConfig appears in **Element attributes** list.
3. Operators change `href` / `innerHtml` here or in **inline edit**.
4. Use **References** to see which pages still use the entry; delete unused entries.

## Related

| Doc | Description |
|-----|-------------|
| [Content overview](./index.md) | Content group menu |
| [Edit Menu](../navigation.md#permissions-and-edit-menu) | Show this menu |
| [Inline edit](../site/pages-inline-edit.md) | Preview-page KConfig editing |
| [Template Engine · Page](../../templateEngine/page/index.md) | HTML with `k-config` |
| [Labels](./labels.md) | Multilingual short copy (not this feature) |
