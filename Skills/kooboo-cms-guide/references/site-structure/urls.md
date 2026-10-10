# Site Structure - URLs

## Page Purpose

- Admin page: `/development/urls`
- The URL page separately manages internal site routes, external resource addresses recorded by the site, and recent Not Found requests that still cannot match a route.
- Internal, External, and Not Found are not simple filters of one data set; they are resources and records with different purposes.

## Internal URLs

- Each item in the internal list is a site route and shows whether it points to a page, script, style, image, file, or other target type.
- A route shown without a target object has no normally bound target resource and usually needs orphan-route investigation; an item whose target type is route may be an alias to another route.
- Editing an internal URL changes route identity and attempts to update references identified by Kooboo. Addresses dynamically concatenated, script-generated, on external sites, or in plain text are not guaranteed to update.
- The primary route bound to an ordinary target resource cannot be casually deleted here as an independent empty link; aliases and routes without a normal target are the main deletable items.

## URL Aliases

- An alias adds a route pointing to an existing target and does not copy the target page or other resource.
- The original address and alias can both access the same target; when the target changes, both addresses see the same content.
- If the target route has parameters, the alias must still satisfy the parameter semantics; creating an alias does not fill business data automatically.
- Creating an alias from a Not Found record lets future requests for the path match the target route, but does not create content or retroactively delete historical logs.

## External URLs and Internalization

- The external list stores addresses of remote scripts, styles, images, and other resources referenced by pages, layouts, or views; they are not internal site routes.
- Internalize for a script, style, or image converts the remote resource to a local site resource and updates identifiable references; it is more than renaming a URL.
- After internalization, verify resource content, relative dependencies, cross-origin behavior, and frontend loading.
- Deleting an external URL record does not delete the file on the remote server or guarantee all custom references are cleaned up.

## Not Found and Cleanup Risks

- The Not Found list comes from recent 404 request logs and retains only addresses still unable to resolve to a site route.
- It discovers broken entries and creates aliases; it is not a complete, permanent dead-link scan report.
- Whether identified references are cleaned when a route is deleted depends on site settings. Even with cleanup enabled, do not promise every dynamic or external reference is repaired automatically.
- After changing, aliasing, or deleting a URL, verify main entries, menus, multilingual addresses, static resources, and external links.
