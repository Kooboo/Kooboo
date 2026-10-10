# Site Structure - Page Editors

## Page Purpose

- This document applies to dedicated workspaces for page-code editing, page settings, visual design, layout-page editing, and rich-text page editing.
- These workspaces still edit the same kind of Page resource: the page name identifies it in the admin, URL serves visitor access, and content, online status, cache, and related resources together determine final behavior.
- Editors differ only in how they maintain the page and do not create a separate second page. After saving, return to the page list or real access address to confirm the object and route.

## HTML Page Code Editing

- The HTML editor directly maintains complete page HTML together with page name and URL. On creation, name and URL must be available; an existing page name cannot be changed directly in the current interface, but its URL can change.
- Formatting adjusts code layout only and does not automatically repair HTML, KScript, styles, scripts, or business logic.
- Preview opens the current page's preview address. Being able to open it does not mean the page is online, the site published, or every visitor domain accessible.
- Saving writes the current page and may update the route. After changing a URL, check menus, aliases, script-generated addresses, external links, and cache because dynamic references are not guaranteed to be fully repaired.

## Page Settings

- The settings page parses page HTML and maintains page URL, multilingual paths, online status, title and Meta, parameters, styles, scripts, and cache configuration.
- Adding, deleting, or reordering styles and scripts changes corresponding references and order in page HTML; saving is not merely changing an independent settings table.
- Embedded preview on the settings page is generated from the page currently being edited and may include unsaved changes. A correct preview does not prove public routes, permissions, APIs, or third-party resources work in the real access environment.
- Page cache can vary by version, language, device, and specified query parameters. Enabling cache does not judge suitability automatically; wrong dimensions can reuse an unsuitable result for different requests.
- Page-level styles, scripts, and Meta affect only references saved by the current page; changing a referenced shared resource can still affect other consumers.

## Layout-Page Editing and Assembly

- A layout page uses a shared layout and assembles its own content into named layout placeholders. Page content and layout source are two objects.
- The code editor maintains layout assembly saved by the page; it does not directly change the shared layout. Later layout-source changes affect every referencing page and may make existing placeholder assembly no longer match.
- The settings assembler reads layout placeholders and allows available component resources to be added, removed, or ordered at each location. Saving writes the current page's assembly relationships; deleting an assembly item in one page does not delete the shared View, Layout, or other resource.
- Assembly preview combines the current layout, page settings, and loaded resources. It is not a complete public request and cannot establish routes, permissions, dynamic data, or external APIs passed.

## Visual Design Pages

- The visual designer maintains generated page HTML and design configuration together. On save, it generates page content from the current design and saves structural data needed for further editing.
- A no-layout design page generates a complete page from an independent template; a layout design page writes visual content into placeholders of the selected layout. Neither automatically changes shared layout source.
- Availability of custom components, page elements, and shared resources depends on existing site resources, permissions, and runtime dependencies. Successful drag-in or designer validation does not prove component APIs, login, or business side effects were verified.
- After switching editing modes, manually changing generated HTML, or changing layout structure, the old design configuration may no longer represent final content accurately; reopen the designer and check the real page for critical pages.

## Rich-Text Pages

- The rich-text editor maintains page name, URL, title, body, online status, and cache options and suits pages whose body is mainly maintained in the editor.
- On save, the title becomes the page document title and the body enters the page body. It still creates an ordinary accessible page route, not TextContent in a content folder or an HTML Block.
- The rich-text editor does not automatically provide shared-layout assembly, complete SEO configuration, or a business data model. Choose appropriate page types and site resources when these capabilities are needed.

## Saving, Leaving, and Acceptance Boundaries

- Editors detect unsaved changes and prompt on exit, but the prompt is not a backup, version-publication, or concurrency-conflict solution.
- Save succeeded proves only that the current page object was persisted; it does not prove site publication, DNS, permissions, cache refresh, backend APIs, or external services are complete.
- Acceptance should cover the real page URL, main devices and languages, cache dimensions, layout placeholders, shared scripts and styles, form or API interaction, and links entered from other pages and menus at minimum.
