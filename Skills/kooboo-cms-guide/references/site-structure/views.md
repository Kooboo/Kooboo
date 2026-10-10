# Site Structure - Views

## Page Purpose

- Admin list: `/development/views`; editor: `/development/view/edit`.
- A view is a reusable dynamic HTML fragment that can contain page markup, data reads, and KScript logic and be called by pages, layouts, or other assembly locations.
- A view usually owns a local feature or display area; it is not a complete page and does not own home-page or error-page responsibilities.

## Preview and Page Elements

- Each view can have an independent preview address for checking rendering; it is not a public page route and does not make the view an ordinary page.
- A view can be enabled as a page element with display name, cover, and configurable properties for design-page selection and configuration.
- Page-element properties provide call-time parameters and defaults to a view but do not automatically replace its internal data and rendering logic.
- A view name is a stable identifier. The current editor does not provide direct renaming after creation.
- The editor's Settings dialog maintains page-element information and cache options. Confirming in the dialog brings changes into the current edit model only; click Save at the page bottom to persist the view.
- Formatting changes HTML layout only; it does not validate KScript, page-element properties, cache dimensions, or actual data results.

## Cache Behavior

- A view can cache rendered results and set a cache duration.
- If output changes by device, language, or query parameters, enable the corresponding cache dimension. Missing a dimension can make different requests incorrectly reuse one cached result.
- Cache by Query Parameters should include only parameters that actually affect output. Unlisted changes do not enter the cache key automatically; too many parameters reduce reuse.
- Version-based caching lets related-resource changes invalidate cache. Its scope should match actual view dependencies and must not be understood as automatic discovery of every dynamic dependency.

## Reuse, Copying, and Deletion

- Changing a shared view affects every reference; spot-check main pages and different parameter scenarios after saving.
- Copying creates an independent new view; existing pages and layouts continue to reference the original and do not switch to the copy automatically.
- Used by helps identify references, but dynamic names, script calls, and indirect assembly are not guaranteed to appear completely.
- Check references and frontend behavior before deleting a view; do not assume deletion repairs every call location automatically.
