# ImagePlaceholder

Placeholders are display-only: do not download them as upload assets or include them in file cleanup. Imagery alone needs this guide, not an upload backend; actual image editing follows [Images and file management](../Frontend/images-and-files.md).

When a loadable placeholder image is needed, compose the in-site URL directly; do not call a Function:

```text
/_api/v2/AiSiteBuilder/ImagePlaceholder?keyword=<encoded>&orientation=<orientation>&size=<size>&page=<page>&color=<encoded>
```

| Parameter | Constraint |
|---|---|
| `keyword` | Required; a specific English scene description, URL-encoded |
| `orientation` | Optional: `landscape` / `portrait` / `squarish` |
| `size` | Optional: `thumb` / `small` / `regular` |
| `page` | Optional, 1–20; increment for similar-list images |
| `color` | Optional; a color name or encoded hexadecimal value, with `#` encoded as `%23` |

Use it for `<img src>`, CSS `url()`, product images, and content-cover fields. Keywords must match the current content and visual direction; do not repeat the same generic industry term across the entire site. Do not search online for images yourself or invent an external image host or random URL.
