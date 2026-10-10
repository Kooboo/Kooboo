# Content Management - Tag Attributes

## Page Purpose

- Admin page: `/content/text`
- Tag attributes centrally maintain attributes or inner HTML already declared as configurable on HTML elements, such as image URLs, link URLs, prompt text, or tag content.
- They are not ordinary Labels, content fields, or arbitrary key-value storage. The current page has no create entry; configuration items are usually identified and created after a page, layout, view, or other HTML resource uses a configuration marker.

## How Configuration Items Are Formed

- The configuration name comes from the configuration marker on the HTML element. A same-named configuration uses one site-level record for its value, so multiple resources referencing the name share changes.
- The system adds ordinary HTML attributes to editable bindings; non-self-closing tags also add `innerHtml` to replace content inside the tag.
- Event attributes, `style`, and attributes whose names contain hyphens or underscores are not added to these editable bindings and remain as in the source HTML.
- An image tag provides configurable `src` even when it originally has no `src`. Therefore, image configuration opens the media picker in the current interface, and the selection is written to `src`.
- When new configurable attributes later appear in source HTML, re-identification can add missing attributes to the bindings; existing binding values are not automatically overwritten by source HTML as a result.

## Editing and Frontend Effects

- The current editing operation updates only binding values; it does not change the configuration name, tag name, or original tag HTML shown in the list.
- Ordinary configuration is edited attribute by attribute; image configuration preferably selects `src` through the media library.
- When the frontend renders the configuration marker, the saved binding value replaces the corresponding attribute or `innerHtml`; a part without a saved value continues to use the original value in source HTML.
- Tag attributes have no interface for separate per-language values. A same-named configuration is a site-level shared value and does not switch automatically with the current content language.
- A successful save proves only that the configuration record was updated; it does not prove that links, images, HTML structure, or interactions remain correct on every referencing page. Spot-check the main usage locations.

## Lists, Versions, and References

- The name in the list is the configuration identifier; the summary shows only the first binding item and does not mean the configuration has one attribute only.
- The tag name and original HTML help determine the configuration source. Image preview also applies only to an image's `src` binding.
- Used by shows HTML-resource relationships identified by the system and is not a complete list of all dynamically generated or runtime references.
- The version entry reads existing site-operation versions for the configuration and can inspect and restore historical values; it is not a unified version snapshot of the entire page or every referencing resource.

## Deletion Risks and Fallback

- Deleting a configuration record does not delete the element or configuration marker in source HTML and does not automatically rewrite pages, layouts, or views that reference it.
- If the configuration record cannot be found during rendering, the resource falls back to the original tag HTML retained at compilation time; it is not guaranteed to continue using the last configured value.
- If the source resource still retains the configuration marker, the system may recreate the configuration record from source HTML when it is saved or relationships are recalculated later. Deletion therefore does not mean configuration capability is permanently removed.
- Before deleting or changing it, inspect Used by relationships and resources sharing the same-named configuration to avoid affecting multiple locations with one change.
