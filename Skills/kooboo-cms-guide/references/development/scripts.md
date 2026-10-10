# Development Resources - Scripts

## Page Purpose

- Admin list: `/development/scripts`; editor: `/development/script/edit`.
- This page manages scripts loaded by the browser, divided into external scripts, embedded scripts, and script groups.
- These scripts are frontend JavaScript resources, not the server-side KScript, APIs, or CodeBlocks on the Code page.

## External and Embedded Scripts

- An external script is an independent resource with a site URL and can be viewed through its preview address.
- A new external script uses the `.js` extension by default and establishes the corresponding site route. If the entered path is already bound to a script, saving may update the existing script rather than create a duplicate at the same path.
- The name of an existing external script cannot be changed directly in the current editor. To change its address, inspect references in all pages, layouts, views, and script groups as well.
- An embedded script belongs to another site object such as a page, layout, or view and is not an ordinary independent external script. Editing or deleting it affects the corresponding script fragment in its owner.
- Editing an embedded script does not show an independent-script name form because its identity and ownership come from the owner; saving updates script content in that owner.
- Saving a script does not automatically load it on any page. The browser loads it only when a page, layout, view, or other resource actually references it.
- Editor formatting changes JavaScript layout only; it does not execute the script or validate browser compatibility, dependency order, or business results.

## Script Groups

- A script group combines the routes of multiple external scripts in order into one accessible resource; it does not copy or permanently merge child scripts into new source code.
- Child-script order affects execution order. After changing it, verify pages that depend on earlier scripts.
- Removing a script from a group changes only the group's composition and does not delete the script resource; after a child script changes, pages referencing the group use its updated content.
- After a script group is created, the current interface does not provide direct renaming; existing pages do not automatically switch references because a group is created or copied.

## Upload, Preview, and Site Settings

- Upload imports a JavaScript file. When a same-named script exists at the target location, confirmation is required; do not assume the system always retains two copies.
- A successful preview proves only that the script or script-group resource address returns content; it does not prove that a page references it or that browser business logic runs correctly.
- JS/CSS Minification affects minification when a script is returned externally; already-minified files and some special tool files may skip another minification.
- JS/CSS Browser Cache affects static-resource caching. After changing cache or minification, verify the effect through a real page and browser network request.
- At runtime, the script is returned with a JavaScript content type and supports transfer compression, versioning, and conditional requests; these behaviors do not mean the source code itself is permanently rewritten.

## Deletion and Change Risks

- Deleting or changing a script URL can break references in pages, layouts, views, script groups, or custom code.
- Used by information helps view dependencies identified by Kooboo, but dynamic concatenation, runtime code, and external references are not guaranteed to be identified completely.
- After changing a shared script or script group, at minimum verify load order, browser-console errors, and related interactions on the main pages.
