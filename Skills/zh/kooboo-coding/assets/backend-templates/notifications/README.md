# Notifications

An optional backend capability. Its single source lives here; App Base selects and connects it rather than keeping another implementation.

## Owned objects

| Object | Responsibility |
|---|---|
| API `app.notifications` | `/api/app/notifications/{action}`: GET list; GET `unread-count`; PUT `mark-read`; PUT `mark-all-read` |
| `Services.notification` | Current-recipient resolution, recipient-scoped list/count/read markers and trusted notification creation |
| Package `database.json` | Initial AppNotifications schema; read during explicit database preparation, not installed as code |
| Shared `ENV` | Read pagination defaults and limits from ENV.notifications; preserve existing settings |

Install with `install_backend_templates({ templates: ["notifications"] })`. No App Base is required. One shared ENV CodeBlock is created from assets/backend-templates/ENV.ts if absent; an existing ENV is preserved. ENV is not notification-owned and must not be deleted when removing notifications. The API gets local response helpers unless `response` is explicitly selected alongside it. API route conflicts are checked before any template writes. Customized copies are never overwritten.

Installation returns `DatabaseDefinitions` with the path to [database.json](database.json) and status `not-checked`; it does not create or inspect tables. A mock-authentication preview needs no tables; under `kooboo` authentication the Shell calls this API on every signed-in page, so create the table before first sign-in. Before calling real notification APIs, including standalone installation, follow [database preparation](../../../references/Backend/app-base-services.md#database-preparation-and-mock-transition). Create missing tables using SQLite Functions, preserve existing schemas and data, and retain the primary key. Ordinary query indexes are omitted initially; add them when actual query performance requires it. Services never initialize schema during requests.

## Identity and ownership

On standalone sites, getNotificationUser inside Services.notification uses `k.account` and Kooboo account IDs as notification recipient IDs. When installing through App Base, or adding to a site with its managed user service, the installer selects the getNotificationUser implementation that calls `getCurrentAppUser`; recipient IDs are then App user IDs. Keep notification producers consistent with that choice. Changing current-recipient resolution on an existing populated site requires an explicit recipient-ID migration; reinstalling templates does not migrate data.

Notification services accept a trusted `NotificationUser` containing `id`. Only getNotificationUser connects to the selected identity system; other service functions do not read requests or emit HTTP responses. Customize that function to connect another user system. Standalone installation strips the App Base import. Clients cannot choose the current recipient. Writes constrain both notification ID and recipient ID. `createAppNotification` is for trusted server business code and persists real data; do not invoke it for mock previews.

Anonymous API calls return HTTP 401 and the flat UNAUTHENTICATED response. GET parameters are bound through the typed API callback, then converted and validated. `page` is 1–100000, `pageSize` is 1–100 by default; change limits in `ENV.notifications`. `unreadOnly` accepts only `true` or `false`. PUT `mark-read` accepts only `{ id: string }`. Unknown or other-recipient IDs do not reveal record existence.

## App Base composition and removal

First App Base creation defaults to `["response", "notifications"]`. `backendTemplates: []` or `["response"]` creates no notification backend. App Base owns the browser adapter at `app-base/Script/service.notification.js`, installed as `/service.notification.js`, and startup composition at `/service.bootstrap.js`.

`appConfig.notificationsEnabled` controls browser integration. When false, startup and adapter methods make no notification requests, and the provided workspace shells hide the notification control. This is explicit capability selection, not fallback after HTTP errors. When true, API and network errors still propagate.

Installing notifications later does not rewrite frontend code: enable `notificationsEnabled` explicitly after preparing storage when real data is required and verifying the new API. To remove notifications, disable that flag and remove the two owned backend objects plus the API route when appropriate. Existing notification data is not automatically deleted. User identity, user storage and startup remain independent.

## Mock cleanup

App Base mock composition passes its trusted mock user through getNotificationUser; marked branches return empty notifications or no-op read markers. Standalone installation without App Base has no mock branches. Before real data or delivery, remove marked blocks from `Services.notification` as well as App Base's identity/user/API mock blocks. Do not merely toggle configuration. Template installation never upgrades an existing mock implementation.

For the base path use k.api.get("/", ...). Only GET has a final notFound catch-all; POST/PUT/DELETE have no fallback callbacks. APIs use dot-separated names; Services hold business behavior and data access. The manifest references the package's initial database definition.
