# App Base user and notification services

Use the installed App Base code as the working reference for business APIs. Read only the relevant API and Service before extending it. These are live foundation features, not separate example copies.

## Ownership

App Base owns startup, identity, user data and capability integration. Optional backend templates own their APIs, business services and database definitions; server settings share one ENV. Keep one implementation per capability; App Base installs and connects it instead of keeping another copy.

- `assets/app-base/API` and `CodeBlock`: user/identity API and service; `assets/app-base/database.json` defines AppUsers.
- `assets/backend-templates/notifications/files`: notification API and one service (including current-recipient resolution); the template's `database.json` defines AppNotifications.
- `assets/app-base/Script/service.notification.js`: browser integration, controlled by `appENV.notificationsEnabled`.
- `/service.bootstrap.js`: startup composition; `/service.auth.js` only owns authentication/user operations.

First creation defaults to response and notifications. `backendTemplates: []` omits both; user APIs use local response helpers and startup makes no notification requests. Later installation does not modify frontend configuration. See [notification installation and removal](../../assets/backend-templates/notifications/README.md).

## Organization and routes

| Object | Route / responsibility |
|---|---|
| API `app.user` | `/api/app/user/{action}`: GET current user; PUT current user's display name at the base path; POST `logout` |
| API `app.notifications` | `/api/app/notifications/{action}`: GET list at base; GET `unread-count`; PUT `mark-read`; PUT `mark-all-read` |
| `ENV` | Single server configuration: auth mode, language and notifications settings; source: assets/backend-templates/ENV.ts |
| `Services.user` | Synchronize account identity, read/update App-owned user data |
| `Services.notification` | Optional notifications template: recipient-scoped operations |
| Package `database.json` | Initial table definitions, referenced by the package manifest; not installed as runtime code |

`assets/app-base/API` and `CodeBlock` are source directories; installed names are Kooboo code objects, not filesystem paths. Import CodeBlocks with `./Services.user`, `./ENV`, etc. Group operations by business domain. For a dynamic `{action}` route, register base reads/writes with `k.api.get("/", ...)` and `k.api.put("/", ...)`. Do not bind or inspect an `action` query parameter to determine the path. Named operations use their action names. Put `k.api.get(() => k.api.notFound())` last for unknown GET paths; do not add POST/PUT/DELETE catch-all handlers.

API object names and filenames use dots, for example `app.user` / `app.user.ts`. Prefer one `Services.xxx` per business module, including parameterized data access; split only when complexity justifies it. No mandatory Models layer or table initialization in request handlers. Keep one shared `ENV`, grouping settings by module.

API owns query/body parsing, validation and HTTP responses. Service receives ordinary validated arguments and trusted user context; it does not read request parameters or write HTTP responses. Current identity comes from the server, never a caller-supplied current-user ID. SQL values are parameterized. Updates constrain record ownership and select writable fields explicitly.

## Request and response

- User GET returns `successResponse(user)`; anonymous GET and PUT return HTTP 401 with `{ success: false, code: "UNAUTHENTICATED", message: "请先登录", data: null }`.
- User PUT uses the callback's `body` parameter, accepts only a trimmed `displayName` of 1–80 characters, and returns the updated user. Account IDs, account email and administrator flags are not editable here.
- Notification GET declares `page`, `pageSize`, and `unreadOnly` as `string | null` callback parameters for native binding. Convert pagination strings to `pageNumber` and `pageSizeNumber` before validation. Prefer callback parameters for ordinary inputs and `body: unknown` for JSON bodies; read `k.request` explicitly when a particular parameter source must be selected. Do not manually parse the URL or add a fallback parser. Defaults and maximums come from `ENV.notifications`; the installed defaults are page 1, pageSize 20, max page 100000 and max pageSize 100. `unreadOnly` accepts only `true`/`false`. Invalid values return HTTP 400 / `INVALID_ARGUMENT`.
- Validate body shape, writable fields, field types and value ranges in separate readable steps. Normalize strings once after checking their type. Do not combine unrelated validation rules into one long condition.
- Separate logical steps with a blank line: route checks, authentication, parameter validation, business calls and response returns. Keep closely related statements together; do not add a blank line after every statement.
- `mark-read` receives `{ id }` in JSON body. Both notification ID and authenticated recipient constrain the write. Repeating read operations is safe and does not reveal another user's notification existence.
- Keep business calls separate from response construction: assign the service result to a local variable, then `return successResponse(result)`. Do not nest service calls inside response helpers. Existing meaningful variables such as `user` can be returned directly. Errors use `return errorResponse(httpStatus, code, message, data?)`; the helper returns `k.api.httpCode(...)` with the agreed JSON body. Service does not depend on the response helper.
- The default response template is explicitly imported. If backend templates are opted out, App Base APIs receive local helpers with the same contract. No global request/runtime envelope is required.
- User synchronization uses an atomic upsert and preserves the App display name. `LastSeenAt` records observation of the account, not a login event; `UpdatedAt` records App profile edits.

When enabled, frontend services call real HTTP endpoints, including during mock preview. Disabling notifications explicitly suppresses notification requests; HTTP errors never disable the module automatically. User service treats only 401 / UNAUTHENTICATED as logged out; network errors and 500 remain failures. `loadAppBootstrap` in `/service.bootstrap.js` only composes the separate responses on the client for the Shell. General `requestJson` returns the payload unchanged; user/notification services adapt their own envelopes.

## Database preparation and mock transition

Default new App implementation to mock preview unless the user requests real persistence; preserve existing strategies. Explain that mock writes do not persist. Keep frontend HTTP contracts stable when replacing mock API internals.

`create_app_base` and `install_backend_templates` return `DatabaseDefinitions` entries with `Resource` and `Status: "not-checked"`. Installation neither checks nor creates tables, even with `authenticationMode="kooboo"`. `FoundationTables` lists expected names, not database readiness. With `authenticationMode="mock"`, previews need no tables; do not load definitions just to prepare for possible future persistence. With `authenticationMode="kooboo"`, the user endpoint upserts `AppUsers` (and installed notifications read `AppNotifications`) on every signed-in request, so create the missing FoundationTables before first sign-in even when business data stays mock; otherwise signed-in pages fail to load.

Before enabling real storage or calling authenticated real APIs:

1. Read relevant `DatabaseDefinitions[].Resource` with `read_skill_resource(skillName="kooboo-coding", resourceName=...)`. App Base uses [its manifest](../../assets/app-base/manifest.json) and [database definition](../../assets/app-base/database.json); notifications use [their manifest](../../assets/backend-templates/notifications/manifest.json) and [definition](../../assets/backend-templates/notifications/database.json). Reuse these paths for a later transition when installation receipts are unavailable.
2. Load SQLite Functions and inspect with `list_database_table` / `get_database_table`. Each `tables[]` entry is a `create_database_table` argument object (`name`, `columns`); create only absent tables. Preserve primary keys and unique constraints, especially AppUsers.KoobooUserId used by upsert. Notifications initially omit ordinary query indexes.
3. Existing schemas and application code are authoritative after customization. Compare before changing; use `update_database_table` only for required changes in authorized scope. Do not replay the template over existing tables, rebuild them, delete data, or assume a failed read means absence. Column type/constraint changes require an explicit migration; the current update Function does not support them.
4. Read back the schema, then remove affected mock branches and enable real use. Verify actual API writes and reads across requests, applicable transaction rollback and user isolation. KScript database checks alone do not verify HTTP authorization. Installation or structural checks do not prove database readiness.

Database definitions remain package resources, not CodeBlocks or application runtime dependencies. Normal Service requests perform data operations only; missing tables are setup failures, not a reason to silently create schema or fall back to mock. Reinstalling templates performs no migration or upgrade.

`create_app_base(authenticationMode="mock")` installs explicit `/* mock:start */` ... `/* mock:end */` blocks in server CodeBlocks. `Services.user` owns the fixed mock identity; user and notification services consistently branch on that trusted identity. Requests cannot enable mock mode. Mock profile writes return a preview result without persistent state; notifications are empty and read markers return success without touching SQLite. No real login session is created.

**Before introducing real data, permission-sensitive operations, or delivery, remove every marked mock block from Services.user, API app.user and, when installed, Services.notification.** Read back `ENV` and these objects (Services.notification only when installed), verify no mock fallback remains, and verify anonymous user/notification calls return the agreed 401 response. Also remove any additional mock paths introduced for business features. Do not merely change a browser configuration or fall back to mock on API errors. New `authenticationMode="kooboo"` installation already omits these blocks. Re-running create_app_base preserves existing resources and cannot perform this transition for you.

Keep user authentication and notification data concerns separate. Do not add a server API that bundles both merely for startup convenience. User model IDs and platform administrator status are identity facts, not an automatic authorization policy for future business APIs.

POST user/logout uses the same server identity mode: mock returns success without clearing a real session; real mode clears account cookies via Kooboo logout. Frontend logout always calls this API.

## TypeScript source

Server APIs and CodeBlocks are authored as `.ts`. Installation preserves the TypeScript body, sets `Extension` to `.ts`, and uses `ScriptType.Module`; Kooboo performs its existing TypeScript transpilation at execution time. Use `import type` for shared service contracts. Treat request bodies as `unknown`, validate at the API boundary, and pass typed inputs to Services. Types do not replace runtime validation.

For repository editing, `assets/tsconfig.json` resolves the API, CodeBlock and response-template directories with `rootDirs`. `assets/backend-env.d.ts` is an editor-only subset of the platform declarations; neither file is installed into the App. CLI projects use their generated `kooboo.d.ts` and local import-path conversion.
