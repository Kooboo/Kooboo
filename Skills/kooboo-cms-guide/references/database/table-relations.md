# Database - Table Relations

## Page Purpose

- This page declares relationship metadata for the current site's IndexedDB custom tables. It can select Table A, Field A, Table B, and Field B, and specify one-to-one, one-to-many, many-to-one, or many-to-many.
- A relationship name is available to runtime code when accessing the relationship; the system may also look it up by the related table name. Relationships apply only to the site IndexedDB tables listed here and do not manage real foreign keys in SQLite, MySQL, or SQL Server.

## Runtime Semantics

- A relationship definition does not copy data between two tables or automatically fill related fields. The actual values in both fields must still be maintained by data writes or business code.
- When runtime reads a relationship property of an IndexedDB record, the system queries the other table from the values of both fields. One-to-many and many-to-many return record collections; one-to-one and many-to-one return one matching record.
- Many-to-many here remains a matching query based on the two selected fields; it does not automatically create a junction table, cascade writes, or database-level constraints.
- A relationship declaration is not a foreign-key constraint: it does not prevent orphan values, verify that the two field types are compatible, or automatically enforce cascade updates or deletes.

## Deletion and Risks

- Deleting a relationship deletes only relationship metadata. It does not delete tables, fields, or existing data, and does not undo external side effects previously caused by business code.
- When deleting or changing tables and fields participating in a relationship, do not assume the relationship is repaired automatically; an invalid relationship may return an empty result or report that the table cannot be found at runtime. Check the relationship definition and code using its name before changing the structure.
