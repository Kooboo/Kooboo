# Development Resources - Styles

## Page Purpose

- Admin list: `/development/styles`; editor: `/development/style/edit`.
- This page manages external styles, embedded styles, inline style rules, and style groups.
- External styles are independent resources with site URLs; embedded styles and inline rules belong to other page, layout, or view objects.

## External, Embedded, and Inline Styles

- A new external style uses the `.css` extension by default and establishes the corresponding site route. If the entered path is already bound to a style, saving may update the existing style rather than create a duplicate at the same path.
- The name of an existing external style cannot be changed directly in the current editor. Check references in pages, layouts, views, and style groups before changing its address.
- An embedded style belongs to another site object and is not an ordinary independent external CSS file. Changing it changes the corresponding style fragment in its owner.
- Editing an embedded style does not show an independent-style name form because its identity and ownership come from the owner; saving updates style content in that owner.
- The inline-style list shows the rule's owner and object type. An inline rule belongs to the listed site object; editing or deleting it changes that object's CSS rather than managing an independent stylesheet.
- Saving or previewing a style does not automatically apply it to a page; a page, layout, view, or other resource must actually reference it.

## Extensions and Generated Results

- Besides CSS, the page can use style-engine extensions actually supported by the current site. On save, the corresponding engine generates CSS for browser use.
- For these styles, source content in the editor and externally returned CSS are different layers. An extension being accepted does not prove the corresponding engine is installed or generation succeeds.
- After changing non-CSS source content, inspect the save result and actual resource URL to confirm generated CSS meets expectations.
- Editor formatting changes source-code layout only; it does not repair selectors, cascade conflicts, responsive breakpoints, or style-engine errors.

## Style Groups and Cascade Order

- A style group combines multiple external styles in order into one accessible resource; it does not copy or permanently merge child-style source files.
- Order affects CSS cascade results. Changing order can change the final style for the same selector.
- Removing a style from a group changes only the group's composition and does not delete the child style; after a child style changes, pages referencing the group use updated content.
- After a style group is created, the current interface does not provide direct renaming; existing references do not automatically switch to a new group or copy.

## Minification, Atomic CSS, and Media Splitting

- JS/CSS Minification affects minification when a style is returned externally; JS/CSS Browser Cache affects browser caching. Neither automatically fixes CSS logic.
- After Atomic CSS is enabled, configuration content must be valid JSON. Enabling, disabling server-side processing, or resetting default styles changes generation and rendering behavior and should be verified on real pages.
- Split CSS by Media affects returned content according to mobile and desktop thresholds. It does not automatically rewrite existing CSS into a correct responsive design.
- After changing these site-level settings, verify actual results on desktop, mobile, server-side rendering, and browser cache together.

## Deletion and Change Risks

- Deleting or changing a shared style, style group, embedded style, or inline rule can affect multiple pages at once.
- Used by information covers identified references, but dynamic or custom references may still be missed.
- After changes, verify layout, cascade order, responsive breakpoints, and CSS actually loaded by the browser on the main pages.
