# Database - SQL Logs

## Page Purpose

- This page views SQL logs generated when the current site executes through Kooboo's SQLite, MySQL, and SQL Server database entries. It is not a site IndexedDB operation log, a complete audit log of an external database server, or all statements executed outside Kooboo's database entries.
- Each record contains database type, record time, execution duration, SQL text, and serialized parameters. Parameters may contain business data or sensitive values; assess access permissions and data-exposure risks before enabling it.

## Enabling and Writing

- The page's Enable switch directly saves the site-level SQL-log setting. After enabling, it records only later SQL executed through supported entries and does not backfill earlier queries; disabling does not delete saved logs.
- Logs first enter an in-memory queue and are then written asynchronously to log files divided by week. The list may briefly lack a record just after SQL executes; an abnormal process exit means queued logs cannot be considered persisted.
- A log record proves only that a database entry observed an SQL execution event; it does not prove the transaction committed, downstream business succeeded, or returned data was correct.

## Query and Retention Boundaries

- The page filters by available week, database type, and keyword, with 50 records per page and newer records first. Keywords match only SQL text, not parameter content.
- The week list comes from log files that still exist and contain records for this site. The current page has no delete, export, or fixed-retention-days setting. A week appearing only means that readable records still exist for it; it does not guarantee long-term, tamper-resistant audit retention.
- SQL logs are not a performance analyzer. Execution duration can help locate slow queries, but without query plans, lock waits, transaction traces, or database-server metrics, one duration record alone cannot establish the root cause.
