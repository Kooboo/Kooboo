# DB Guide

## Module navigation

Choose by data type:

| User goal | Use |
|----------|----------|
| Use the site's built-in SQLite to store/query business data | [SQLite Guide](sqlite.md) |

## Usage flow

1. Determine whether the data fits `k.DB.sqlite` (not `k.commerce` or `k.content`).
2. Design the table structure and initialize the table-creation SQL.
3. Encapsulate `query/execute/transaction` in Service/Utils.
4. Call the Service from the API layer; do not concatenate complex SQL directly.

## Boundary

- This entry only routes and navigates SQLite; it does not cover other database types.
