# Database - SQLite, MySQL, and SQL Server Tables

## Data-Source Differences

- SQLite uses the current site's local `sqlite.db` file. MySQL and SQL Server use external connection strings saved under Service Integrations > Database. They are different data sources from site IndexedDB, and tables and data are not synchronized automatically.
- When MySQL or SQL Server configuration is missing, the connection fails, or permissions are insufficient, the table page shows an error and can navigate to Database configuration. Saving a connection string alone does not prove the database is connected.
- The table list comes from the actual current database. Kooboo synchronizes a structural mapping for admin controls and editing, but does not copy the external database into Kooboo.

## Table Structure and Data

- Creating a table creates it in the target database with the system-used `_id` primary key; business columns must then be added. Deleting a table executes a drop-table operation on the target database and removes Kooboo's structural mapping at the same time.
- Existing column names cannot be changed directly in the current interface. Adding or deleting columns or changing string length performs real structural changes; deleting a column loses its data. When SQLite changes structure, it rebuilds the table and copies columns with the same names in the old and new structures; delete-and-recreate is not a safe rename.
- The admin maps database field types to editing controls such as text, number, boolean, or date. The mapping helps editing but does not fully preserve every provider-specific type semantic; complex types may be handled as text in the admin.
- Data editing locates records by the table primary key. Creating, editing, and deleting act directly on the target database; a successful page save does not mean business flows, caches, or external systems referencing the data are synchronized.

## CSV Import and Export

- SQLite, MySQL, and SQL Server data pages support CSV import, up to 5000 rows per operation. After upload, CSV fields must be mapped to database fields, and required fields and a unique field for duplicate detection can be marked.
- Rows missing required values are skipped. When an existing record matches the unique field, the row is skipped with overwrite disabled; the matching record is updated only when overwrite is enabled. Import adds or updates mapped records and does not delete database records absent from the CSV.
- Export generates a CSV of current table data without the internal `_id`, table structure, indexes, constraints, triggers, or relationship definitions; it cannot replace a database backup.

## Risks and Verification

- Structural changes and deletion directly affect the real database. Before operating, confirm provider backups, account permissions, field dependencies, and downtime impact; an admin success message cannot prove that triggers, stored procedures, application caches, and external consumers remain correct.
- SQLite foreign-key constraints are controlled by a separate switch in Database integration; the table page does not automatically repair existing inconsistent data. The site's Table Relations page is not a foreign-key manager for these three database types either.
