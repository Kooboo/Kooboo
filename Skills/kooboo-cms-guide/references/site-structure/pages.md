# Site Structure - Pages

## Page Purpose

- Admin page: `/site/pages`
- A page is a complete content resource visitors can access through a site address. It can be an independent HTML page, rich-text page, or content assembled into a layout.
- A page owns the concrete access entry and page-level content; a layout owns a reusable shell and a view owns reusable dynamic fragments. They are different resource types.

## Page Names and Access URLs

- Page name and access URL are different concepts. The name identifies the page in the admin; URL is the site path matched by visitor requests.
- Names and URLs must both remain unique when creating or copying a page. Changing a name is not naturally the same as changing a URL, and vice versa.
- A page URL corresponds to an internal site route. When it changes, Kooboo attempts to update identified references, but dynamic concatenation, script-generated addresses, and external links are not guaranteed to be repaired.
- A multilingual site can maintain different access paths per language; verify actual addresses per language after changes.

## Page Types and Assembly

- An HTML page saves complete page content and suits an independent page not relying on a shared layout.
- A rich-text page suits scenarios where the editor mainly maintains the body.
- A layout page uses a shared layout and puts page content in its placeholders. Changing the shared layout affects every page using it.
- A design page also starts from a layout but provides visual assembly; availability depends on current site page-element capability and permissions.

## Home, Error Pages, and Online Status

- Route settings can specify a home page, custom 404 page, and custom 500 error page. The home page remains an ordinary page resource marked as the site's default entry.
- The current interface does not allow the same home page to be both custom 404 and 500 page; recheck error-page selection when changing the home page.
- Page online status and site publication are different levels. A page being online does not mean an unpublished site is normally accessible to visitors.
- A custom error page decides only which page appears for the matching error; it does not repair the broken link or program logic automatically.

## Page Tree, Copying, and Import

- The page tree scans page structure and aggregates related nodes, with an option to include scripts. Converting, editing, or deleting tree resources are writes; do not treat scan results as a read-only report.
- Copying creates a new page and URL. Related resources identified in page structure can also be copied, so the result is not necessarily a shallow body copy.
- Shared resources not selected for copying remain referenced by both old and new pages; copied resources become new objects and later changes do not synchronize back to originals.
- Import can download a page from a URL or import a supported local file. Successful import means only that related page resources were established; external dependencies, APIs, login state, and dynamic behavior are not guaranteed migrated completely.

## Modification and Deletion Risks

- Used by relationships help understand associations with pages and other resources but cannot guarantee discovery of every dynamic reference.
- After changing page URL, layout, or online status, verify main entries, menu links, multilingual addresses, and external references.
- Deleting a page is not the same as deleting every menu item, alias, or custom link pointing to it, and those references must not be assumed to be repaired automatically.
