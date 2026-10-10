# Database - Site IndexedDB Tables

## Page Purpose

- Admin pages include the table list, column settings, data list, and data editing. They manage custom tables in the current site's built-in IndexedDB, not browser-side IndexedDB or SQLite, MySQL, or SQL Server tables.
- A newly created table first receives the system primary-key structure; business fields must then be added in column settings. Table names and new column names can use only allowed letters, digits, and other formats; existing column names cannot be changed directly in the current interface.
- Deleting a table deletes the entire table and its data, and does not automatically repair references to it in KScript, pages, views, or table relationships.

## Columns, Indexes, and Data

- A column's control type determines both the admin editor and the underlying data type. An existing column can switch controls only within compatible data types; for example, a text box, textarea, and rich text are all string data.
- A primary key is necessarily unique and indexed; a unique column must also be indexed. Auto-increment applies only to numeric columns; the system generates its value for new data, and the admin cannot manually edit an auto-increment field.
- Saving column settings actually changes the table structure. A new column is added; deleting a column removes it and corresponding data in existing records. Structural changes may rebuild the table, but the system retains only fields with the same names in the old and new structures; delete-and-recreate must not be treated as a safe rename.
- The data list is paginated by default and can be sorted. System fields are hidden when editing a record. Deleting a record deletes only the selected data, does not undo external business side effects, and does not automatically repair logical references in other tables or code.

## Import, Export, and Boundaries

- The current page can export table data as CSV. The export excludes the internal `_id` field and is not a table-structure, index, relationship-definition, or complete site backup.
- The current admin does not provide a CSV import button for site IndexedDB tables; do not apply the import capability of the SQLite, MySQL, or SQL Server pages here.
- Table relationships are maintained on a separate page. Creating an ordinary column, index, or same-named field does not automatically create a relationship, and deleting a table or field must not be assumed to clean up relationship metadata and every consumer.
