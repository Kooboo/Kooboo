# Site Structure - Menus

## Page Purpose

- Admin page: `/development/menus`
- A menu is a reusable hierarchical navigation resource made of a root menu and multi-level items, renderable in pages, layouts, or views.
- A menu does not automatically create pages or routes. It stores navigation text, target addresses, hierarchy order, and rendering template.

## Menu Items and Links

- Each menu item can maintain display text in different languages, one URL, and child items.
- The URL selector lists existing page routes but also allows a custom address, so an item does not necessarily point to a Kooboo page.
- An empty URL is saved as `#`, usually suitable only for a parent or interaction entry without an independent target; it does not infer child-page addresses automatically.
- When page routes later change, manually entered or dynamically formed menu URLs are not guaranteed to synchronize; verify navigation proactively.

## Hierarchy, Order, and Templates

- Dragging a menu item saves the display order among siblings. Moving or deleting a child changes the whole root-menu resource, not a separate small file.
- The menu template determines how link text, target address, child container, current-item style, and parent-child identifiers are output.
- Current-item status is determined during rendering from the relative address the visitor is visiting; an inaccurate target address can make current-item styling inaccurate.
- Template preview checks generated structure and does not mean it displays correctly in every page style, script, or responsive environment.

## Reuse and Risks

- The same menu can be referenced by multiple pages or layouts. After changing items, order, or template, every reference uses the updated result.
- Menu text supports multiple languages but is not translated automatically and does not create corresponding pages for different languages.
- Used by helps locate identified references, but dynamic rendering or script references may not be listed completely.
- Deleting a menu or item does not delete the target page or guarantee that all menu references in pages are repaired automatically.
