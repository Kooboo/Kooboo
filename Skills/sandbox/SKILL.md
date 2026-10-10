---
name: sandbox
description: >-
  Use a hidden Kooboo Sandbox site to copy selected site objects, make isolated
  changes, execute KData or KScript, render templates, and verify page output
  without changing the current site.
---

# Kooboo Sandbox

Use this skill when the user explicitly asks to implement and verify site changes in a Sandbox, test site, or isolated environment. A Sandbox is an independent Kooboo `WebSite` and `SiteDb` in the current organization; it is not a terminal, container, or general-purpose operating-system sandbox.

## 1. Current Implementation Boundaries

- `create_sandbox` creates a site named `__ai_sandbox__{siteName}` with status `Hidden` and `Published = false`.
- The current creation flow does not add a domain binding, so do not report a Sandbox domain or `baseUrl`. Verify execution with `sandbox_render_template`, `sandbox_request_url`, or another Function that supports the `sandbox` parameter.
- A Sandbox and the production site have separate databases. Pages, Views, Layouts, Styles, Scripts, Content, and settings in a Sandbox are not automatically merged or synchronized back to the production site.
- The Sandbox list is queried by current organization, not isolated by source site or user. Sandbox names must be unique within an organization.
- There is currently no automatic expiration or background cleanup. Call `delete_sandbox` only when the user explicitly agrees to deletion.
- A Sandbox isolates site data only. KScript, KData, page code, or APIs may still cause external network, email, payment, or other off-site side effects.

## 2. Standard Workflow

1. Call `list_sandbox` to confirm the Sandboxes and names already present in the current organization.
2. Choose a short `siteName` before creation, and select the smallest complete `contentTypes` set for the task.
3. Call `create_sandbox`. This call requires approval.
4. Read `success`, `siteId`, and `siteName` from the result, then call `list_sandbox` again to confirm that the same `siteId` exists.
5. Every subsequent Sandbox-specific execution call must explicitly pass the same `sandbox` name.
6. If resources must be copied after creation, call `copy_sandbox_objects`. This call requires approval.
7. After changes, use read Functions inside the Sandbox and, as appropriate, verify with `sandbox_eval_kdata`, `sandbox_eval_script`, `sandbox_render_template`, or `sandbox_request_url`.
8. Report the Sandbox name, site ID, copied scope, verification result, and unverified limitations. Do not describe a Sandbox result as deployed to the production site.
9. After the user explicitly requests cleanup, call `delete_sandbox`, then call `list_sandbox` again to confirm deletion.

## 3. Native Function Contract

| Function | Input | Return | Approval |
|---|---|---|---|
| `list_sandbox` | Empty object | `[{ id, siteName, creationDate }]` | No |
| `create_sandbox` | Required `siteName`; optional `contentTypes` | `{ success, siteId, siteName }` | Yes |
| `copy_sandbox_objects` | `siteName`, non-empty `objectTypes` | `true` | Yes |
| `sandbox_eval_kdata` | Required `sandbox`, `source` | Exported variable dictionary | No |
| `sandbox_eval_script` | Required `sandbox`, `code` | KScript return value | No |
| `sandbox_render_template` | Required `sandbox`, `templateHtml`; optional `models` | Rendered string | No |
| `sandbox_request_url` | Required `sandbox`, `relativeUrl`; optional `httpMethod`, `queryString` | `{ statusCode, body, executionTimeMs, headers }` | No |
| `delete_sandbox` | Required `siteName` | Fixed string `delete success` | Yes |

`sandbox_eval_kdata`, `sandbox_eval_script`, `sandbox_render_template`, and `sandbox_request_url` require the Sandbox input. Before calling, the implementation switches Context and verifies that the current Context belongs to the target Sandbox; an empty name or nonexistent target returns an error and does not continue on the production site.

Do not call `k.sandbox.enter()` or `k.sandbox.exit()` directly. The Native Function base enters the Sandbox before each call and restores the original site Context in `finally`. Context switching applies only to the current Function call; the next call must pass `sandbox` again.

## 4. Creation and Copying

`create_sandbox.contentTypes` and `copy_sandbox_objects.objectTypes` support the following values, matched case-insensitively:

`Layout`、`View`、`Style`、`Script`、`Image`、`File`、`Menu`、`HtmlBlock`、`Label`、`Form`、`ContentFolder`、`ContentType`、`ContentCategory`、`TextContent`、`Code`、`OpenApi`、`Page`。

Copying rules:

- Only explicitly selected object types are copied; dependencies are not added automatically.
- The source is the site Context at call time, and the target is specified by `siteName`; do not make the target Sandbox the current Context before calling the copy function.
- All objects in each selected repository are written to the target Sandbox through `AddOrUpdate`.
- Repeated copying updates objects with the same ID but does not delete additional objects already present in the target Sandbox.
- There is currently no explicit copying of Routes, Commerce data, site settings, permissions, or repositories not listed here.
- When modifying an existing page, usually consider `Page`, `Layout`, `View`, `Style`, `Script`, `Image`, `File`, `Label`, `HtmlBlock`, and related content types together.
- `TextContent` does not automatically include `ContentType`, `ContentFolder`, or `ContentCategory`; select them explicitly when needed.
- Passing an unsupported type raises an error.

`create_sandbox` creates the site before copying. If copy arguments are invalid or copying fails partway through, the created Sandbox is not rolled back automatically. Call `list_sandbox` first to confirm the actual state, then decide whether to call `delete_sandbox` after user approval.

Creation example:

```json
{
  "siteName": "preview",
  "contentTypes": ["Layout", "View", "Style", "Script", "Page"]
}
```

Additional copy example:

```json
{
  "siteName": "preview",
  "objectTypes": ["Image", "File", "HtmlBlock", "Label"]
}
```

## 5. Calling Other Functions in a Sandbox

Many Site, Commerce, read, and write Functions inherit an optional `SandboxInput`, so their schemas may expose an optional `sandbox` field:

```json
{
  "sandbox": "preview",
  "nameOrId": "home"
}
```

For these general-purpose Functions:

- Pass `sandbox` again on every call.
- Check the real schema through `function_discover` or `use_function` first; when the `sandbox` field is absent, do not assume the call is isolated.
- A general-purpose Function with an optional Sandbox only attempts to enter the target site. The current `KSandbox.Enter` does not throw when the name is missing, so an incorrect name may let the call continue in the production-site Context.
- Before a general-purpose write, confirm the name with `list_sandbox`, then use `get_site_info` with the same `sandbox` to verify the site ID. Stop writing if this cannot be confirmed.
- A Sandbox does not change the Function's existing approval requirements.

For KScript verification, prefer `sandbox_eval_script` because it requires and verifies the Sandbox. Use `run_kscript` or `run_skill_script` with `sandbox` only when an existing general-purpose script interface is required.

## 6. KData Verification

The `source` for `sandbox_eval_kdata` must contain a complete `<k-data>...</k-data>` element:

```json
{
  "sandbox": "preview",
  "source": "<k-data><query as=\"products\" source=\"content\" resource=\"Product\" action=\"list\" export></query></k-data>"
}
```

Only explicitly exported variables are returned:

- A `<query>` with `export` or `export="true"`.
- A `<let>` with `export` or `export="true"`.
- An existing variable exported through `<export var="name"></export>`.

When an empty dictionary is returned, first check the export declarations, query conditions, and whether the relevant data exists in the target Sandbox. Do not automatically retry on the production site.

## 7. KScript and Template Verification

KScript:

```json
{
  "sandbox": "preview",
  "code": "return 1 + 1;"
}
```

The execution timeout for `sandbox_eval_script` is three minutes. Its Native Function does not itself require approval, but the script may still write to the Sandbox or trigger off-site side effects; call it only when the task authorizes that execution.

Template:

```json
{
  "sandbox": "preview",
  "templateHtml": "<h1>Sandbox preview</h1>"
}
```

`sandbox_render_template` renders the input HTML directly in the Sandbox Context without saving a Page, View, or Layout.

## 8. URL Verification

`sandbox_request_url` locates and renders a page through the Sandbox `SiteDb.Pages.GetByUrl`:

```json
{
  "sandbox": "preview",
  "relativeUrl": "/products",
  "httpMethod": "GET",
  "queryString": {
    "category": "featured"
  }
}
```

- `httpMethod` defaults to `GET`.
- `queryString` is optional.
- When the page cannot be found, the result contains `statusCode: 404` and `body: null`.
- The current implementation does not simulate request headers, body, session, or user.
- `RequestUrlResult.Headers` is not currently populated and is usually `null`.
- A copied Page without a usable URL/Route may return 404; this does not mean the page object is absent. Read the Page and Route state together to confirm.

## 9. Deletion and Production-Site Handling

`delete_sandbox` returns the fixed string `delete success` whether or not the target exists. Always call `list_sandbox` again after deletion; do not judge success from the returned string alone.

The current Sandbox has no automatic publish, merge, or write-back mechanism. After the user confirms the Sandbox result, apply it to the production site only if needed:

1. List the objects and data changes to apply explicitly.
2. Obtain production-site write authorization again; do not reuse approvals from Sandbox creation, copying, or modification.
3. Omit `sandbox`, then implement and verify again on the production site.
4. Obtain the corresponding approvals again for payment, email, account, order, deletion, or other off-site side effects.

## 10. Failure Handling

- When a Sandbox-specific execution Function reports that the target does not exist, call `list_sandbox` to reconfirm `siteName`; do not remove `sandbox` and retry.
- When a general-purpose Function uses an optional `sandbox`, stop subsequent writes immediately if the Context switch cannot be confirmed.
- If copying fails after creation, the Sandbox may still exist; list and inspect it before deciding whether to keep it, complete the copy, or request deletion approval.
- When `copy_sandbox_objects` returns an error, check the target name, whether `objectTypes` is empty, and whether it contains unsupported types.
- When `sandbox_request_url` returns 404, check the Page URL, Route, and copy scope; do not immediately conclude that the rendering engine failed.
- If runtime verification cannot be completed, a static read-back result may be reported, but it must be clearly marked as runtime verification incomplete.
