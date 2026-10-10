# Database - Key-Value Store

## Page Purpose

- This page manages the current site's lightweight key-value store. Each key is unique within the site, and the admin displays and edits values as strings; it is not a table-structure editor, browser local storage, or external Redis management interface.
- Creating an item requires a unique key name and value. The key name cannot be changed when editing an existing item; saving directly overwrites the key's current value rather than adding a version.
- Values have a UTF-8 size limit, currently 4096 bytes. Character count is not byte count, so multibyte content such as Chinese reaches the limit sooner.

## Usage and Deletion Boundaries

- KScript and admin pages access the same site-level key-value data. When code writes an object to this store, it is ultimately converted to a string, so it must not be treated as a document database that retains arbitrary object types and query capabilities.
- Deleting a key immediately makes the value unavailable to later reads. The system does not automatically modify pages, views, or code referencing the key and provides no recycle bin for deletion on this page.
- Key-value storage suits small configurations, status flags, and simple text, not large files, large JSON blocks, business records requiring indexed queries, or sensitive credentials. Deleting or overwriting does not undo external requests already made from the old value.
