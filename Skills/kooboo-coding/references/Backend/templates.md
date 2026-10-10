# Optional backend templates

Templates copy ordinary application-owned CodeBlocks and routed APIs into the site. They provide defaults, not an enforced framework. Prefer selected helpers for new APIs; preserve existing and user-specified contracts. Replacing a helper should require only local changes at its explicit import/call sites.

## Discover and install

1. Read `assets/backend-templates/index.json` with `read_skill_resource`, `skillName="kooboo-coding"`.
2. Read the selected template's guide from the catalog for its exports, configuration, integration and removal. For lightweight file operations, select `files` and read [FileIO helpers](../../assets/backend-templates/files/README.md). It installs no tables or API; business permissions, references and cleanup scheduling remain with the caller.
3. Load `install_backend_templates` through `use_function` and call it with `{"templates":["response"]}`. No App Base or App Plan is required by the installer; follow the current task's implementation scope.
4. Check `Success`, `Conflicts` and `Objects`. Read `UsageGuides` if not already loaded. Existing customized copies must be read before use.
5. `DatabaseDefinitions` lists initial schema resources with status `not-checked`, not database readiness. A mock-authentication preview needs no tables; under `kooboo` authentication, create them before the first signed-in request. Before real API use, follow [database preparation](app-base-services.md#database-preparation-and-mock-transition), including standalone templates: inspect existing schemas and initialize missing tables with SQLite Functions.

Only selected templates are installed; `[]` writes nothing. A template may contain several CodeBlocks and APIs. All selected resources and same-name and route conflicts are checked before any template writes. Identical existing code is skipped. Different content or object type is a conflict, never an overwrite. A persistence failure can leave earlier creations; the result reports completed/failed objects for inspection before retrying.

## App Base defaults

`create_app_base.backendTemplates` controls selection:

| Value | Behavior |
|---|---|
| Omitted on first creation | Install `response` and `notifications` |
| Omitted when `app-config.js` or `app.js` already exists | No implicit template installation; preserve edits, deletions and prior opt-out |
| `[]` | Install no backend templates; disable notification integration |
| Explicit IDs | Install exactly that selection, with the same conflict checks |

The result's `BackendTemplates` contains the template-specific receipt. For later additions, use `install_backend_templates` without recreating App Base. Explicit reinstallation can restore a deleted copy. There is no automatic upgrade or dependency manager.

## Response convention

Read `assets/backend-templates/response/README.md` for the executable example and export contract. `successResponse(data = null, message = "", httpStatus = 200)` and `errorResponse(httpStatus, code, message, data = null)` return `k.api.httpCode(...)` with the flat `{ success, code, message, data }` body. API handlers should `return successResponse(...)` or `return errorResponse(...)` directly. The helpers set HTTP status and body only; authentication, logging and data access remain in the API/Service layers.

The template does not rewrite bootstrap/notification APIs or frontend request utilities. App services may adapt the response locally. Custom formats, downloads and redirects remain valid; neither Kooboo runtime nor Preflight requires this envelope.

App Base owns the startup and user skeleton; templates own optional capabilities. Notification storage must not initialize user tables, and user storage must not initialize notification tables. Standalone template installation never changes browser configuration; enable `appENV.notificationsEnabled` explicitly after adding notifications to an existing App.
