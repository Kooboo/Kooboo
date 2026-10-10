# Site Settings - Advanced

## Page Purpose

- Admin page: `/system/settings?name=advance`
- Configures the default relational database, SQL logs, code logs, automatic repair on save and deletion, frontend and admin events, continued download for cloned sites, and custom site settings.
- Most switches affect runtime, log volume, or other resource content; understand their purpose and verify in a test environment before changing.

## Default Database

- Determines whether generic SQL access without an explicit database type connects by default to SQLite, MySQL, or SQL Server.
- In automatic mode, MySQL is used when only MySQL is configured; SQL Server when only SQL Server is configured; and site SQLite when neither is configured.
- When both MySQL and SQL Server are configured, automatic mode may not determine one uniquely; explicitly select a database.
- This setting does not create an external database or validate connection configuration; configure and test external connections in Service Integrations.

## SQL Logs

- When enabled, Kooboo records SQL, parameters, and execution time from site database access for query and performance troubleshooting.
- It does not record every server-database activity and covers only operations entering Kooboo's site-database access path.
- Long-term enablement increases logs; decide whether to keep it after troubleshooting based on actual need.

## Code Logs

- The master switch decides whether site code logs are written.
- Log level is the minimum recorded level; for example, after selecting Warning, Debug and Information are no longer recorded.
- Retention days clean log files beyond the period and do not mean logs are deleted exactly at expiration.
- Code logs differ from visitor and SQL logs; choose the correct source when troubleshooting.

## Repair Constraints on Save

- When enabled, Kooboo runs checks registered as “auto-repair on save” while saving non-deletion site objects.
- Current repair scope includes some resource references, addresses, and style constraints; it is not general HTML beautification, complete syntax repair, or business-correctness checking.
- Automatic repair may change saved content. For sites with handwritten structures or much special syntax, compare before-and-after differences first.

## Clean References on Resource Deletion

- When enabled, deleting an object referenced by other pages or resources makes Kooboo try to remove the related component or address reference from callers.
- This is a high-impact operation that changes other resource content, not merely a relationship-index cleanup.
- Automatic cleanup depends on references identified by Kooboo and cannot guarantee discovery of references in arbitrary dynamic concatenation or custom logic.
- Export a backup before enabling for important sites and inspect affected page and resource differences after deletion.

## Request Hooks

- When enabled, frontend requests, page, view, and route processing can trigger configured frontend events or business rules.
- A Hook does not create business behavior by itself; it executes only when the site has corresponding event scripts or rules.
- Events can change lookup or rendering and add request cost; include frontend events when troubleshooting an abnormal page.

## Action Hooks

- When enabled, admin creation, change, or deletion of site objects such as pages and images can trigger configured before- or after-action events.
- It is not an ordinary HTTP Request Hook and mainly targets site-object changes.
- Events may block, change, or append admin-operation results; confirm existing rules are repeatable and failure handling is explicit before enabling.

## Continued Download

- This option appears only in qualifying cloned or migrated sites.
- When enabled and a visitor requests an address not yet local, Kooboo uses the original crawl-task record to try downloading content from the source site and then matches routes again.
- It depends on source reachability and valid historical task information and is subject to server download limits; it is not a permanent reverse proxy.
- If source content changes or is untrusted, continued download may bring new external content into the current site; use long-term enablement cautiously.

## Custom Settings

- Custom settings are site-level key-value configuration readable by site scripts or specific Kooboo features.
- Key names are agreed by actual consumers; a key with no consumer has no automatic effect.
- Some system features may also read conventional key names; confirm purpose before changing an unfamiliar existing key.
- Values are saved as ordinary site configuration and must not be assumed encrypted or equipped with dedicated key-management capability.
