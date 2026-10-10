# Content Management - Content

## Page Purpose

- Admin pages: `/content/contents`, `/content/textContentsByFolder`, `/content/content`
- Creates content-folder or single-content entries and manages their bound structure, display mode, sorting, related content, and editing interface.
- A content type defines “which fields exist”; a content folder decides “where the content goes and how it is organized and edited”; actual content items save the field values.

## Content Folders and Single Content

- A content folder stores multiple items of the same kind, such as news, product information, or team members, and provides list, pagination, sorting, and add-item entries.
- Single content is for cases that need only one record, such as a site introduction or contact information. Opening it edits the record directly and does not provide an ordinary multi-item list.
- Both require selecting a content type when created. After creation, the internal name and bound content type cannot be changed directly in the current interface; the display name can still be adjusted.
- Renaming, hiding, or grouping a folder does not change its bound content type.

## Folder Settings

- The display name is the readable name in the admin; the internal name stably identifies the resource. They are different concepts.
- Grouping collapses multiple content entries into one group in the admin list. It mainly organizes the management interface and does not automatically create a content hierarchy or frontend category.
- Hidden only keeps the entry out of the content-management list by default; it remains visible when Show All is opened. It is not content unpublishing, access control, or permission protection.
- A multi-item content folder can set the page size. Disable Pagination uses a very large single-page size and does not make the data volume or query cost disappear.
- Sorting can use ascending or descending field order or drag-and-drop order. Drag-and-drop has clear meaning only when the corresponding sort mode is selected.

## Related Folders and Field Order

- A related folder connects the current content to category or embedded content, allowing the editing form to provide a corresponding selector or nested editing area.
- A category relationship means that the current content selects items in another folder as categories or related items.
- An embedded relationship means that the current content maintains a set of related content internally; it is not a simple plain-text field.
- Field order controls only the display order in the current folder's editing form and does not change the field definition of the content type itself.

## Preview URL

- The preview URL generates a frontend address for Save and Preview and can use current content fields as placeholders in the address.
- Preview saves the content first, replaces placeholders with field values in the selected language, and opens the address in a mode that allows unpublished content to be viewed.
- Configuring a preview URL does not automatically create the corresponding page or route. Preview can still fail when the URL template, field values, or frontend route do not match.

## Publishing and Multilingual Content

- A content item can have an online status, availability time, and field values for each language. These are item-level publishing properties and are not replaced by the folder's Hidden switch.
- Multilingual fields are saved per language; fields not marked multilingual are shared across languages.
- Changing a content type or folder configuration does not automatically fill, translate, or repair existing content data.

## Deletion Risks

- The page shows which objects use a resource. Check references, related content, and frontend queries before deleting a folder.
- Do not assume deleting a folder safely migrates its content or that all dynamic references can be found or repaired automatically.
