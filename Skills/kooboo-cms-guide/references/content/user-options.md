# Content Management - Parameter Configurations

## Page Purpose

- Admin pages: `/content/useroptions`, `/content/useroptions/create`, `/content/useroptions/setting`, `/content/useroptions/edit`
- Parameter configurations define a set of site-level structured parameters and maintain their actual JSON data. KScript can read the data through `k.paramConfig.{名称}`.
- They are suitable for site parameters that business code needs to read but admin staff should maintain. They are not environment variables, a service-integration secret store, content types, or request-parameter validators.

## Definition and Data Are Separate Layers

- The definition contains the stable name, admin display name, and schema; data is the actual JSON value maintained according to that definition.
- The Settings permission creates configurations and changes the display name or schema; the Edit permission changes actual data. The two permissions are independent.
- After creation, the name cannot be changed on the current settings page and forms part of the KScript read path. The display name affects only admin identification and does not change the read name.
- Clicking a name in the list opens data editing; the settings button opens schema editing. Changing the schema and changing data are different operations.

## Schema Capabilities

- Fields support strings, numbers, booleans, objects, and arrays.
- Array elements can be strings, numbers, booleans, or objects. Objects and arrays of objects can continue to define child fields, allowing nested structures.
- The schema generates the admin editing form and provides structure and type hints for the KScript editing experience.
- The schema is not a runtime data-validation rule. Changing it does not automatically migrate, convert, delete, or fill existing JSON data, or check whether old values still match the new type.
- After a schema field is deleted, a same-named property may remain in the original JSON data. When a new field is opened in the form, its initial value is an empty string, `0`, `false`, empty object, or empty array according to its type.

## Data Editing and Raw JSON

- The ordinary editing interface generates inputs from the schema and is suitable for maintaining values in the expected structure.
- Raw JSON editing allows the entire data object to be changed directly. The current interface checks that JSON can be parsed, but does not validate that fields exist in the schema, that value types match, or that extra fields are forbidden.
- Saving data overwrites the configuration data with the current editing result as a whole; it does not merge field by field. Confirm before saving that raw JSON still retains required properties.
- When changing the schema, the interface sends an empty data object, but the definition-save API does not use it to overwrite existing data; existing data remains unchanged.
- A successful save proves only that JSON was persisted. How business code interprets fields, whether empty values are allowed, and whether external operations succeed are still determined by the KScript or page logic using these parameters.

## KScript Read Behavior

- KScript gets the saved JSON value by configuration name; a missing name returns an undefined value rather than automatically creating a configuration.
- Blank data also appears as an undefined value. Business code should handle missing configurations, missing fields, and types that do not match expectations.
- Parameter configurations are site-level shared data and do not automatically generate different copies by visitor, admin user, language, or request.
- The cache is invalidated after data changes, and later reads use the new value; this does not mean running external tasks or previously produced side effects are undone.

## Modification and Deletion Risks

- Changing field names, types, nesting levels, or array-element types can make existing data inconsistent with calling code. The current page does not automatically find or repair these consumers.
- Deleting a parameter configuration removes its schema and data together. KScript references to the name are not automatically rewritten, and later reads return an undefined value.
- Search code and pages for read locations before deletion. Do not treat list deletion confirmation as a complete dependency check or business rollback.
