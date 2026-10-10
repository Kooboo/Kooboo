# Content Management - Content Types

## Page Purpose

- Admin pages: `/content/contentTypes`, `/content/contentType`
- A content type is the structural definition of content data. It specifies which fields a content item contains and how each field is edited, validated, displayed, and localized.
- A content type does not itself store articles or records. Content items save data according to the structure only after a content folder is bound to the type.

## Fields and Editing Controls

- A field name is the stable identifier in the structure; the display name is the text seen by editors.
- The control type determines how the admin editing interface enters a field, such as text, rich text, image, selection, or relationship data. It is not merely a visual setting and also affects data format and available configuration.
- A field can configure validation, prompts, options, whether multiple values are allowed, and other behavior. When content is saved, the editing interface generates the corresponding inputs from these definitions.
- Field order affects the content-editing form and the default field display order.

## Multilingual Fields

- After a field's multilingual property is enabled, it can save separate values for the different languages enabled on the site.
- A field without multilingual enabled shares one value across languages.
- Changing an existing field to or from multilingual does not mean historical data is automatically migrated, merged, or translated; inspect existing content afterward.

## Summary Fields

- Summary fields represent a content item in content lists and admin overviews, and are suitable for identifiable fields such as a title or name.
- Multiple summary fields can be configured.
- If no ordinary summary field is explicitly selected, Kooboo first selects an ordinary text field; if none is found, it selects the first field by field order as the list summary.
- Summary fields affect only list identification and display; they do not automatically become an SEO title or frontend page title.

## System Fields

- A new content type includes system fields such as publishing status, unique identifier, order, and availability time.
- System fields can be expanded in the interface, but they are not ordinary custom fields; Kooboo keeps some names, types, and behavior fixed.
- Reserved system names cannot be recreated as ordinary custom fields, and duplicate key system fields are not retained as multiple independent fields.
- User editable determines whether a field appears in the ordinary content-editing form; it does not mean that the field does not exist or will not be used at runtime.

## Modification and Deletion Risks

- Changing a control type, deleting a field, or changing multilingual and multiple-value properties can make existing content inconsistent with the new structure. Saving the content type does not automatically complete a full data migration.
- Before deleting a content type, check the content folders bound to it, existing content, and frontend queries. Do not assume related resources will automatically migrate to another type.
- For a content type already in use, prefer adding new fields and migrating data gradually before deleting old fields.
