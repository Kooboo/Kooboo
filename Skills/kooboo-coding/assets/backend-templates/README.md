# Backend source templates

This directory is the source catalog for optional application-owned backend capabilities. `create_app_base` and `install_backend_templates` share a copy installer. Templates are not runtime plugins and have no automatic upgrade, uninstall or dependency manager.

To add a template:

1. Add an ID, description and guide path to `index.json`.
2. Create `<id>/manifest.json` with a nonempty `files` list. Each entry has `objectType: "CodeBlock"` or `"API"`, a unique site code `name`, and `source: "files/<filename>.ts"`. API entries also declare `route`, such as `/api/app/notifications/{action}`.
3. Add the TypeScript sources and `<id>/README.md` describing exports, prerequisites and replacement points. A template can create several CodeBlocks and APIs; sources should not have hidden dependencies on other templates.
   If storage is needed, add `"database": "database.json"` to the manifest. The resource contains a nonempty `tables` array; each entry uses the `create_database_table` input shape (`name`, `columns`). Keep primary keys and business unique constraints explicit. It describes initial setup, not an automatic migration policy.
4. Verify selected installation, same-name conflicts and preservation of application edits. Removing the helper should require only local changes to its consumers.

Capabilities own their database.json definition and normally one Services.xxx containing business logic and data access. No runtime Models layer or request-time schema initialization is required. Installation returns DatabaseDefinitions resource pointers with status not-checked; it does not inspect or create tables. Mock preview needs no storage. Before real use, follow [database preparation](../../references/Backend/app-base-services.md#database-preparation-and-mock-transition): inspect existing schemas, initialize missing tables with SQLite Functions and preserve customized data/code. Configuration is centralized in one shared ENV sourced from ENV.ts; group module settings within it. App Base owns identity/user startup and explicit integration files. Never duplicate a capability implementation in App Base. Keep copying isolated in `BackendTemplateInstaller` so business code does not depend on this catalog.
