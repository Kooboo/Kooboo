# Content Management - Media Library

## Page Purpose

- Admin pages: `/site/media`, `/site/editImage`
- The media library manages site images and icons and provides directories, uploading, searching, pagination, sorting, previews, downloads, reference hints, and image editing.
- The media library is image-centered; documents, archives, and other general files should usually be managed on the file page.

## Directories, Search, and Storage Providers

- Images are organized by paths and folders in the current storage provider. Creating a folder, uploading, renaming, and deleting all act on the current provider.
- A storage provider is the actual storage backend, not an ordinary filter label. After switching providers, directories, object identifiers, pagination capability, error states, and available operations may differ.
- Search finds media in the current provider by keyword; the list can also be sorted by name, size, or modification time. Search results do not represent image references across the site.
- Images identified by the system as used by pages, views, layouts, content, styles, or HTML blocks can be viewed by object type, but this is not a complete dependency graph.

## Uploads, References, and Metadata

- A successful upload proves only that the image was written to the current storage; it does not automatically insert the image into pages, content, or styles.
- Reference counts in the list come from site-object relationships the system can identify. References in script-concatenated addresses, runtime-generated content, external pages, or external systems may not be listed completely.
- An image's Alt is media-object metadata. Changing it does not automatically rewrite independently saved `alt` attributes in all existing pages or HTML, and does not mean SEO optimization is complete.
- Information such as image source, author, dimensions, and size supports management and preview; it does not prove copyright, accessibility, or frontend display has been accepted.

## Image Editing

- The image-edit page can change the filename, Alt, and image content; saving updates the original media object rather than automatically creating a copy.
- When bitmap content is unchanged, saving can update only the name and Alt; a new editing result overwrites image content and recalculates dimensions.
- SVG uses a dedicated editor, and saving writes the current SVG content back to the media object.
- Renaming updates the image's own access route. The system may synchronize identified relationships, but do not assume that all static text, dynamic scripts, caches, external references, or third-party data are repaired automatically; verify key pages after renaming.
- Overwriting image content affects every location that continues to reference the media object, but does not automatically check crop ratios, layout adaptation, cache refresh, or visual quality.

## Deletion and Difference from the File Page

- Deleting an image or directory acts directly on the current storage provider. It does not automatically repair pages, content, styles, or scripts that reference them, and not every provider can be assumed to have a recycle bin.
- Before deletion, check reference hints and key pages together; dynamic references are not guaranteed to be discovered completely.
- The media library is suitable for images, icons, Alt, dimensions, and visual previews; the file page is suitable for general files. The two pages may use the same type of storage provider, but their object types and management behavior differ, so they must not be treated as two filtered views of one list.
