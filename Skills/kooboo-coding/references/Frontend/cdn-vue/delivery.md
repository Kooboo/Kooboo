# Kooboo Client-Rendered Vue Delivery Workflow

This guide defines how to create or update a client-rendered Vue interface as Kooboo Page, Style, and Script objects. It does not require the product to come from a Blueprint or restrict the application to mock data.

## Request source binding (App Base)

Business services use `defineService(import.meta.url, methods)`. Every requesting Screen, dialog, and layout must bind services once in its own setup with `useServices`. Do not import unbound named methods such as `listAssetRecords` into components or call the module-level `assetService.listAssetRecords()` directly: those calls only produce X-Kooboo-Service and lose screen, route, and caller resource. New business services export the service object only; do not copy the auth/notification named exports retained for legacy compatibility.

```js
import { useServices } from '/request-context.js'
import { assetService } from '/service.asset.js'

// Inside the requesting Screen/component setup():
const { asset } = useServices(import.meta.url, { asset: assetService })
// Event handlers, watches, pagination and onMounted use this bound instance:
const data = await asset.listAssetRecords({ page: 1, pageSize: 10 })
```

Each dialog that actually requests data binds its own import.meta.url; do not label it with the parent's Source. Persistent shells pass `useServices(import.meta.url, services, { currentRoute: true })`. Service methods use the injected request, without reading Vue components or writing headers. Outside components use bindServices with explicit source. See [project-structure.md](project-structure.md) for the full contract.

Business leaf routes need stable string names. Preserve meta.screenId for App Plans; without a Plan, Screen falls back to the route name. During code readback confirm the call chain actually uses bound instances. When runtime acceptance is authorized, inspect page-load, pagination, dialog, and Shell notification requests for X-Kooboo-Screen / Route / Source / Service. Bootstrap legitimately has only Source / Service. Service alone does not establish complete tracking.

## D0: Confirm runtime boundaries

Clarify before implementation:

- Whether this is a new app, an existing Page upgrade, or a single-component change;
- Page Name, access URL, language, and whether the existing Route must be retained;
- Whether SEO or server-rendered first-page content is required;
- Whether data comes from a mock API, static values, localStorage, or an existing HTTP API;
- Whether login, permissions, uploads, payments, or other server capabilities are involved;
- Browser compatibility range and whether external CDNs are allowed.

When the task needs server capabilities, this directory still owns the Vue frontend, but the corresponding backend guidance must also be used. Frontend state or hidden buttons cannot replace a server implementation.

## D1: Discover existing objects

Before writing:

1. List related Page, Style, and Script objects;
2. Check whether the target Name exists in the same type;
3. Check whether the target URL conflicts with any object Route;
4. Read existing content, URLs, references, and purposes before updating an object;
5. Decide whether to reuse, make a minimal update, add a business prefix, or stop for confirmation.

Do not bypass conflicts with random Names or URLs, and do not overwrite common objects such as `index`, `main.js`, `app.js`, or `app.css` without reading their existing content.

## D2: Design the object list

Build the object list according to [project-structure.md](project-structure.md), including at least:

- `objectType`；
- `name`；
- `url`；
- `operation`：create / update；
- `responsibility`；
- `imports[]`；
- `consumers[]`。

The object list is an implementation aid, not a product model that must be persisted. A small local change may retain only affected objects; a complete application must first close the dependency graph of the current delivery slice, not list or create every not-yet-implemented screen component in the first round.

Follow the dependency direction in [project-structure.md](project-structure.md).

## D3: Verify external dependencies

Confirm before writing business objects:

- Vue uses the Vue 3 full-build CDN channel recommended by the official documentation;
- Tailwind, UI framework, icon, and font URLs are accessible;
- ESM package export names, import specifiers, and installation methods work in reality;
- The UI framework reuses the Vue in the import map and does not load a second Vue;
- Font and icon subresources have no CORS or 404 failures;
- The smallest component mounts, renders, and responds to interaction in a complete HTML Page.

See [ui-foundation.md](ui-foundation.md) for dependency-check rules. When a check fails, change the loading method or dependency; do not write unverified guesses into business components.

## D4: First creation

For an App without Blueprint/App Plan, follow [lightweight App implementation](../../Scenario/app-building/lightweight.md): read its persisted SiteMilestone checklist, install the same App Base using direct configuration, then implement the current capability dependency-first. Add routes and navigation as Screens are saved and read back. No complete Screen inventory or Plan manifest is required. The App Plan-specific steps below apply only to planned Apps; shared resource and design-system conventions remain applicable.

For an independent frontend task, proceed from leaves to root according to dependencies. If the task comes from the user-authorized scope in a current App Plan, the following “home-first incremental delivery” overrides the full-creation order in this section.

### App Plan: Home-first incremental delivery

Before the first App UI write, read [App Base layout delivery](app-layout.md). Persist/read back CSS and Shell, then continue directly to the default Screen dependencies. Keep Route placeholders until their dependencies exist; do not insert browser layout acceptance before business implementation. Keep the supplied App setup and perform component-anatomy checks; reading CSS alone is not acceptance.

1. Read `read_app_plan({})`, select the full authorized DeliverySlice/object scope, and create concrete Todos for its business outcomes. Choose a routed entry Screen serving the task; preserve the existing entry unless reviewed requirements change it.
2. Use DataContracts and Service operations as the interface design, and implement the agreed data strategy. Mock API fixtures stay inside the HTTP API; their success-only writes do not establish durable storage. Real persistence requires actual read/write verification.
3. Install App Base and its runtime through [design-system consumption](../app-design-systems.md), and integrate the layout through [App layout](app-layout.md). Continue with business Screens; installation itself is not delivery.
4. After `/app.css` and `/layout.default.js` have been persisted and read back, build the default Screen as one dependency closure: API → service/store → leaves → a new Screen-specific Vue root-component Script. That root must organize its own `setup`, template, information hierarchy, and interaction composition around the Screen's App Plan Blocks, Actions, and primary task. Design-system primitives, leaf business components, services, stores, and composables may be shared. Multiple `screen.*.js` files must not merely pass titles, fields, datasets, or copy into one full-page factory that owns the header, metrics, content panels, table, and form. If two Routes differ only by configuration while retaining the same root DOM skeleton and primary workflow, the later Screen remains unimplemented. Persist and read back every imported local Script before replacing only that Screen's `PlannedScreen` reference in `/router.js`. Never rewrite or repurpose the shared managed `/component.planned-screen.js`; otherwise every unimplemented Route will display the same implemented page. The Shell must not import not-yet-created Screen modules.
5. If any dependency fails, keep the placeholder and do not update consumers. This applies to static and dynamic imports and prevents transient module errors; never write all components first and postpone Router/root integration to a final batch.
6. Read back the Screen and its dependency closure, confirm that the Route targets a Screen-specific root and that it actually covers the current Screen's Blocks, Actions, and loading/empty/error/success states rather than acting as a configuration wrapper around a generic full-page factory. Use the limited read-only Preview diagnostics for available runtime issues; do not initiate browser observation, element inspection, clicks, navigation, or device switching unless the user explicitly requests browser testing. Report untested visual and interaction behavior as unverified. Repeat steps 4–6 for one remaining Screen at a time. Do not add a per-Screen Preflight scan.
7. App Base is not a deliverable or stopping point. Keep scaffold/runtime status out of product UI and continue until the entire authorized scope is implemented or a concrete external blocker exists. Unselected Plan scope is not automatically authorized.
8. Before completion, read the current Plan and verify the selected scope's reachable Screens, Actions, Service operations, DataContracts and actual data behavior. Keep partial/unverified Todos open; do not store execution status in the Plan.

The full first-creation order for an independent frontend task is:

1. `create_style`: `kooboo-var.css` (when design variables are truly needed);
2. `create_style`：`app.css`；
3. `create_script`: service (only when a real HTTP call exists);
4. `create_script`：store；
5. `create_script`: leaf components;
6. `create_script`: composite components and screens;
7. `create_script`：`app.js`；
8. `create_script`：`main.js`；
9. `create_page`: complete HTML entry; omit `layoutName`;
10. Read back key objects one by one and verify URLs, imports, exports, component registration, and external assets.

Before creating the Page, the Style and Script Routes referenced directly by the entry must already exist. Do not create empty stores, services, or components merely for “structural completeness.”

## D5: Update an existing application

First determine the smallest impact surface:

1. Modify leaf objects;
2. Update direct consumers;
3. Update `app.js`, `main.js`, or the Page last;
4. Read back modified objects after each batch;
5. Refresh and check the actual Page URL. If login or unavailable diagnostics prevents verification, follow [Implementation and acceptance](../../Scenario/app-building/site-milestones.md#runtime-verification): retain pending verification and continue authorized implementation; repair actual failures before dependent work.

Special cases:

- Module URL changes: create and verify the new URL first, update all consumers, and handle the old object last;
- Export or component-name changes: update imports, registration names, and template tags in the same batch;
- Store-action changes: find every caller rather than changing only the definition;
- API-contract changes: update the service, store, loading/error state, and UI consumers together;
- Object deletion: confirm there are no consumers, then follow Kooboo deletion guidance.

## D6：Preflight

Use a focused Preflight for a completed local change, not for each object or Screen, and a capability Preflight only for complete page delivery or App delivery closeout. Example:

```json
{
  "taskType": "all",
  "layers": ["frontend", "design", "accessibility", "security", "performance"],
  "mode": "capability",
  "artifacts": [
    { "name": "index", "objectType": "Page" },
    { "name": "app.css", "objectType": "Style" },
    { "name": "main.js", "objectType": "Script" },
    { "name": "app.js", "objectType": "Script" }
  ]
}
```

Complete the actual written objects according to this change's object list. Do not pass `layoutName` for a Page without a Layout. Preflight PASS proves only the structure in the scan scope and does not replace browser runtime acceptance.

## D7: Accept through the actual Page URL

Verify at least:

- Page, CSS, ES Module, CDN, and API requests have no unexpected 4xx/5xx responses;
- Only one Vue loads and it mounts successfully;
- The console has no module-resolution, template-compilation, or runtime errors;
- The default interface, navigation, forms, and core actions work;
- Loading, empty, error, and success feedback match the actual data pattern;
- localStorage restore, corruption fallback, and reset work when used;
- HTTP timeout, failure, duplicate submission, and unauthorized states are handled when used;
- The interface is operable at 320px, tablet, and desktop widths;
- Keyboard, focus, Escape, labels, and error messages are verifiable.

Reading back source or receiving Preflight PASS cannot replace the actual Page URL. If the environment cannot access the actual URL, explicitly report that runtime acceptance is incomplete.

## Completion response

Report at the end:

- Created or updated Page, Style, and Script objects;
- Actual entry URL;
- Data sources and server dependencies;
- Main interactions that were verified;
- Delivery Slice status, naming every partial or unverified Slice;
- Unverified items, known limitations, or follow-up work.

Do not use “first version”, “complete”, or equivalent completion language merely because the default Screen renders or Preflight passes. For a planned App, completion language requires coverage of the entire authorized scope; otherwise report partial implementation and continue remaining authorized work.

When writing to a Site is authorized, actually create or update the objects instead of pasting all source code in the response by default. Output complete content by object Name and URL only when the user explicitly requests “output code only, do not write to the Site.”

For subsequent authorized App changes, preserve the installed Shell and entry Screen. Apply dependency and route replacement steps only to reviewed scope; do not recreate layout or implement unrelated capabilities.
