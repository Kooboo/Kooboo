# Request Hooks

> Menu: **Settings -> Request Hooks** (advanced menu)  
> List: `/_Admin/system/front-events?SiteId={siteGuid}`  
> Edit: `/_Admin/system/front-events/edit?SiteId={siteGuid}&name={eventName}&display={displayName}`

**Request Hooks** run KScript rules at fixed points in the visitor **HTTP request** lifecycle (before/after route, page, view resolution) for URL rewrite, conditional redirects, dynamic page/view replacement, etc. Maps to `BusinessRule` + `FrontEvent`; requires site **`enableFrontEvents`** on.

::: tip Permissions
| Action | Permission |
|--------|------------|
| View hook list and edit page | `frontEvents` |
| **New Hook**, edit rules, **Save**, list **Hook switch** | `frontEvents·edit` |
| Multi-select **Delete** all rules for an event | List delete (removes all rules for that event type) |
:::

::: info Master switch
Besides **Hook On** on this page, **Request Hooks** (`enableFrontEvents`) in [Basic Settings · Advanced](./basic/advanced.md#constraints-and-other-switches) must be on; either off disables all request hooks.
:::

::: warning Distinction from Action Hooks
| | **Request Hooks (this page)** | [Action Hooks](./action-hooks.md) |
|--|-------------------------------|-----------------------------------|
| When they run | Front-end **HTTP requests** (Route / Page / View) | Backend **save, delete**, etc. on resources |
| Path | `/system/front-events` | `/system/backend-events` |
| Master switch | `enableFrontEvents` | `enableBackendEvents` |
:::

## How to Open

1. In [Edit Menu](../navigation.md#permissions-and-edit-menu), enable **Settings -> Request Hooks**.  
2. Open **Settings -> Request Hooks** in the left sidebar.

<DocImage src="/cms/settings/settings-request-hooks-overview.png" alt="Request hooks list" width="1120" />

## Toolbar

| Control | Description |
|---------|-------------|
| **New Hook** | Opens [hook type picker](#new-hook) (configured event types hidden) |
| **Hook On / Off** | `enableFrontEvents`; toggling **saves site immediately** (`saveSite`) |
| Status text | On: “Hook On” (blue); off: gray “Hook Off” |

<DocImage src="/cms/settings/settings-request-hooks-toolbar.png" alt="Request hooks toolbar" width="1120" />

## Hook List

| Column | Description |
|--------|-------------|
| **Type** | Localized event `name` (e.g. RouteFinding; wording per login language) |
| **Rule count** | Saved rules for that event |
| **Edit** | Opens [rule edit page](#rule-edit-page) |

Multi-select **Delete** removes **all** rules for selected events (`BusinessRule/DeleteEvents`), not single rules.

<DocImage src="/cms/settings/settings-request-hooks-list.png" alt="Request hooks list" width="1120" />

## New Hook

Click **New Hook** for **Hook list** grouped by **category** (Route / View / Page); click a type to open edit page (row may appear only after save).

<DocImage src="/cms/settings/settings-request-hooks-event-picker.png" alt="Hook type picker" width="1120" />

### Available Event Types

| Category | Event name (`name`) | Typical timing |
|----------|---------------------|----------------|
| **Route** | `RouteFinding` | **Before** route resolution (can change `k.event.url`, etc.) |
| | `RouteFound` | Route matched |
| | `RouteNotFound` | No route match |
| **View** | `ViewFinding` | **Before** view lookup |
| | `ViewFound` | View found |
| | `ViewNotFound` | View not found |
| **Page** | `PageFinding` | **Before** page lookup |
| | `PageFound` | Page found |

Display names localized via `Hardcoded`; use `BusinessRule/EventList` as source of truth.

## Rule Edit Page

Breadcrumb: **Request Hooks -> {event display name}**. No sidebar item; deep link needs `name` (required) and `display` (breadcrumb).

<DocImage src="/cms/settings/settings-request-hooks-edit.png" alt="Rule edit page" width="1120" />

### New Rule

**New Rule** dropdown:

| Type | Value | Structure |
|------|-------|-----------|
| **IF-ELSE-THEN** | `IF-ELSE-THEN` | **IF** + **THEN** / **ELSE** (nested rules) |
| **DO** | `DO` | One or more **Code** blocks in sequence |

Default on empty edit page: one **IF-ELSE-THEN** rule.

### Rule Cards

- Drag handle on **title bar** for execution order within the event.  
- **Delete** on title bar removes the whole rule.  

#### IF-ELSE-THEN

| Area | Description |
|------|-------------|
| **IF** | Opens [condition editor](#edit-conditions); conditions shown with `&&` |
| **THEN** / **ELSE** | Nested rules; add IF-ELSE-THEN or DO |

#### DO

Code snippet summaries; **+** add, pencil edit. Monaco KScript (`language: typescript`, KScript mode).

<DocImage src="/cms/settings/settings-request-hooks-code-dialog.png" alt="Code edit dialog" width="1120" />

Example variables in editor comments (vary by event):

- Common: `k.event.url`, `k.event.userAgent`, `k.event.culture`  
- Route: `k.event.page`, `k.event.view`, `k.event.route`  

Example in **RouteFinding**: rewrite URL: `k.event.url = "/pageone"` (valid only in that event).

### Edit Conditions

<DocImage src="/cms/settings/settings-request-hooks-condition-dialog.png" alt="Edit conditions" width="1120" />

| Column | Description |
|--------|-------------|
| **Left value** | Fields for current event (`BusinessRule/ConditionOption`) |
| **Operator** | Varies (e.g. `==`, `contains`) |
| **Right value** | Constant or expression fragment |

**+** adds rows; confirm writes rule `if` array. Runtime: `ConditionManager.Evaluate`.

### Save and Leave

Bottom **KBottomBar**: **Back** to list; **Save** calls `BusinessRule/post` (`eventName` + `rules`). Unsaved changes prompt (`useSaveTip`).

## Execution Order (Concept)

1. Site `enableFrontEvents` is true.  
2. Request enters FrontEvent (e.g. `RouteFinding`).  
3. Rules run in list order; IF-ELSE-THEN then DO as described.  

Execution is handled by the site's front-end request hook engine, in the order configured on this page.

## Typical Workflow

1. Confirm [Advanced tab](./basic/advanced.md) and **Hook On** here.  
2. **New Hook** → e.g. **RouteFinding**.  
3. Add **IF-ELSE-THEN** or **DO**, conditions and Code.  
4. **Save**, verify on front end with target URL.  
5. Collaborative export: **BusinessRule** in site package.

## Related

| Doc | Description |
|-----|-------------|
| [Settings overview](./index.md) | Menu and sub-routes |
| [Basic Settings · Advanced](./basic/advanced.md) | `enableFrontEvents` |
| [Action Hooks](./action-hooks.md) | Backend operation events |
| [Development -> Code](../development/code.md) | KScript code resources |
