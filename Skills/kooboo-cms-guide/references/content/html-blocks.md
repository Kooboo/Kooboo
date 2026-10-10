# Content Management - HTML Blocks

## Page Purpose

- Admin pages: `/content/htmlblocks`, `/content/htmlBlock/edit`
- An HTML block is reusable rich HTML content that can be referenced by multiple pages, layouts, or components.
- It is suitable for content fragments that need centralized updates, such as footer descriptions, campaign notices, or general introductions. It is not a page, layout, or ordinary plain-text Label.

## Names and Reuse

- The HTML-block name is the resource's stable identifier. After creation, the current editing interface allows only multilingual content to be changed and does not provide direct name editing.
- The same HTML block can be referenced by multiple resources; after it is changed, all references use the updated content on subsequent rendering.
- Used by information shows identified reference relationships, but dynamic references may still not be listed completely.

## Multilingual Content

- An HTML block can save separate content per site language; frontend rendering reads the value for the current access language.
- Enabling multilingual content does not automatically translate existing HTML or ensure that links, images, and structure are consistent across languages.
- Changing one language version does not automatically synchronize other languages.

## Rendering Behavior

- An HTML block participates in page rendering as HTML content; it is not automatically escaped into ordinary text.
- Its links, images, styles, and interactions still depend on the frontend page environment; a successful save does not mean the visual effect is correct on every referencing page.
- An HTML block does not automatically create a page route or replace the overall structure and data logic in a layout or view.

## Versions and Deletion Risks

- The list provides a version entry for viewing existing site-log records for the HTML block.
- Before deleting or making major changes, inspect Used by relationships and spot-check the main referencing pages.
- Do not assume deleting an HTML block also deletes page references or that all dynamic references can be repaired automatically.
