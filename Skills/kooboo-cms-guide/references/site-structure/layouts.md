# Site Structure - Layouts

## Page Purpose

- Admin list: `/development/layouts`; editor: `/development/layout/edit`.
- A layout is a complete HTML shell reusable by multiple pages, usually carrying document structure, header, footer, shared style references, and assembly locations for page content.
- A layout is not itself a visitor page and does not receive an ordinary page URL just because it is created.

## Placeholder Regions

- A layout receives page or view content through named placeholder regions. A new layout contains one `Main` placeholder by default.
- Page content must be assembled into a corresponding placeholder present in the layout; name mismatch, deletion, or structural damage can keep content from appearing in the expected position.
- A layout can contain multiple placeholders for main content, sidebar, or other assembly locations.
- A placeholder defines only “where content goes”; it does not automatically produce page content, menus, or data queries.

## Names, Reuse, and Copying

- A layout name is the resource's stable identifier. The current editor does not provide direct renaming after creation.
- Multiple pages can share one layout. After a shared layout changes, every dependent page uses the new structure on later rendering.
- Used by in the list shows identified dependent pages and resources; inspect impact before changing.
- Copying a layout creates an independent layout, but existing pages continue to reference the original and do not switch to the copy automatically.

## Editing and Preview Boundaries

- The layout editor saves a reusable shell; a page's body and page-level configuration remain maintained in the page.
- Formatting in the editor adjusts HTML layout only and does not repair placeholder names, KScript, resource references, or page assembly logic.
- A layout name can be set at creation; an existing layout name cannot be changed directly in the current editor.
- Base-address information needed for editor preview is not retained long-term as part of layout body when saving, so do not rely on it to define the site's actual access root.
- A successful save proves only that the layout resource was updated. Preview changes to shared headers, scripts, styles, or placeholders on main pages.

## Deletion and Change Risks

- A layout referenced by a page cannot be deleted directly. Handle references first, then delete.
- Deleting placeholders, changing names, or substantially changing HTML structure can make existing page assembly lose its position or render incorrectly.
- Used by covers identified references, but dynamic or custom references still require site-preview verification.
