# Service Integrations - Database

## Page Purpose

- The Database group stores connection strings for external data services such as MongoDB, MySQL, SQL Server, and Redis; the SQLite item controls only foreign-key constraints for the current site's local SQLite.
- These configurations let site runtime code establish connections. They do not import an external database into Kooboo or copy, migrate, or synchronize data when saved.

## Connection and Runtime Conditions

- MySQL, SQL Server, MongoDB, and Redis each require a correctly formatted connection string and a Kooboo server that can access the target host and port.
- Saving a connection string does not immediately establish and validate a connection. A connection is usually created when KScript first uses the corresponding database entry; missing configuration, invalid format, unreachable network, TLS, or account-permission problems surface during the actual call.
- When a MongoDB connection string omits a database name, the system uses a default database name derived from the current site; do not assume this connects to any existing business database in the provider account.
- SQLite uses the current site's local database file rather than a remote connection string. Switching foreign-key constraints does not migrate table structure or automatically repair existing inconsistent data.

## Difference from Site Database Selection

- External connection configuration only makes the corresponding database entry available; it does not automatically switch the site's default relational database to that provider. The corresponding configuration is read only when code explicitly uses MySQL, SQL Server, MongoDB, or Redis.
- The SQL-log switch belongs to site advanced settings, not this page. Even with SQL logs enabled, all network, authentication, or server errors from an external provider are not guaranteed to form a complete audit record.

## Risks and Verification

- Connection strings usually contain accounts and passwords. Use least-privilege accounts and restrict network sources. Changing credentials affects later connections but cannot undo queries or writes already executed.
- Start verification with a minimal read-only query, then verify write permissions, transactions, character set, time zone, and timeout separately. A successful save, JSON shown in the list, or one successful query does not prove all business queries and high-concurrency scenarios work.
