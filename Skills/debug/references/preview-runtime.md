# Preview Runtime Debugging

Runtime debugging and re-verification for the Site Preview on the right side of AI Chat. This file is the Preview-specific reference for the [Kooboo Debug SOP](triage.md); it covers only the combined use of Browser Control, Runtime Log, and Network.

---

## 1. Capabilities and boundaries

### Browser Control

Use it to open pages, set the device, observe the interactive semantic tree, find elements, perform allowed interactions, and wait for results:

- `browser_control`
- `navigate_iframe`
- `set_iframe_device`
- `observe_iframe`
- `observe_iframe_changes`
- `find_iframe_elements`
- `inspect_iframe_element`
- `act_on_iframe_element`
- `wait_for_iframe`
- `set_iframe_files`

It is for reproducing and validating the UI; it does not authorize backend writes to manufacture a successful result.

### Runtime Log

`read_preview_diagnostics` can read the following after applying limits and redaction:

- An automatic UI audit based on the final DOM and `getComputedStyle`, including unapplied stylesheets, browser default styles, obvious drift from the design background/brand-color baseline, horizontal overflow, text clipping, and severe sibling overlap; when a color baseline exists, read primary background samples and brand-color matches from `uiAudit.colorAudit`.
- `console.log / info / debug / warn / error`;
- `window.error`;
- `unhandledrejection`;
- Resource-loading errors for Script, Style, Image, and other resources.

Start with `mode="issues"` by default; use `mode="debug"` only when errors and warnings do not explain the problem.
When the result contains `uiAudit.passed=false`, handle the `source="ui-audit"` finding and Style/Link resource errors first; “there is no console error” does not override a failed UI audit.

### Network

`read_preview_network` can read:

- fetch / XHR;
- requests for Script, Stylesheet, Image, Font, Media, and other resources;
- pending, success, HTTP error, network error, timeout, aborted, and opaque states;
- URL, method, status code, duration, redirects, content type, response size, and the fetch / XHR initiator stack.

Safety boundary: request bodies, response bodies, Cookies, Authorization headers, and sensitive URL parameters are not returned. If the root cause depends on response JSON content, continue with server logs, endpoint source, or other authorized and redacted real data; do not pretend to have seen the response body.

---

## 2. Two standard reproduction paths

### 2.1 First-screen / blank-page / startup failure

Startup errors require runtime early capture and static quick triage together; do not wait for the iframe to recover before checking source:

1. Reload the current page with `navigate_iframe(reload=true)`.
2. At the same time, run Preflight for the current Page, bound Layout, and mounted View, and read [Kooboo Common Errors Quick Path](common-errors.md).
3. When the Preview Agent recovers within the bounded wait, use `observe_iframe` to obtain the page state for the new document.
4. Call `read_preview_diagnostics(mode="issues")`.
5. Call `read_preview_network(mode="issues")`.
6. Read the corresponding source based on Preflight `knownErrors`, stacks, failed resources, failed APIs, and initiators.

If reload still returns `frame_not_ready`, reports `agentReady=false`, or Log / Network is unavailable, stop Browser Control retries immediately and continue with Preflight and source inspection. Once a deterministic known error is hit, do not delay the repair decision while waiting for the iframe.

Do not save a cursor before reload and continue using it; after the document changes, both the cursor and nodeId belong to the old page.

### 2.2 Interaction failures such as clicking, filling, or saving

1. Open the target URL and set the target device when necessary.
2. Establish a DOM baseline with `observe_iframe`.
3. Read Log and Network once each and save their returned `nextCursor` values.
4. Find semantic elements with `find_iframe_elements`.
5. Use `inspect_iframe_element` when necessary to confirm state.
6. Replay the user action with `act_on_iframe_element`.
7. Use `wait_for_iframe` to wait for the expected UI, hidden state, or URL change.
8. Call `observe_iframe_changes` within the same document.
9. Use each `afterCursor` to read the new Log and Network entries produced by this action.
10. Correlate DOM, Log, Network, and source to confirm the root cause.

---

## 3. Cursor, document, and nodeId

### Log cursor

```json
{
  "documentId": "...",
  "seq": 12
}
```

### Network cursor

```json
{
  "documentId": "...",
  "revision": 18
}
```

Must follow:

- Log and Network cursors cannot be interchanged.
- When reload, navigation, or device switching changes the iframe document, read a complete new baseline.
- Discard old nodeIds after the document changes.
- `observe_iframe_changes` is only for incremental changes within the same document.
- When `cursorReset` or a full observation is returned, use the new document as authoritative; do not treat it as a tool failure.
- When a query result is truncated, first add targeted `search`, `states`, `types`, or `levels`; do not prioritize unbounded reads.

---

## 4. Browser Control rules

- Prefer semantic role, accessible name, text, and tag; do not guess CSS selectors.
- Use `observe_iframe` for the initial baseline; when the target is already clear, call `find_iframe_elements` directly.
- Prefer `observe_iframe_changes` after an action; do not reread the entire page.
- Use `wait_for_iframe` for loading, dialogs, delayed text, and URL navigation; do not click again immediately.
- Discard old nodeIds after an action returns `navigated=true`; a new Agent not being ready does not mean the click failed.
- When a nodeId is stale, observe/find again instead of reusing the old reference.
- Use the `hover` action for hover menus; do not guess with click.
- For responsive failures, restore the same device with `set_iframe_device` before reproducing.
- Before uploading a file, obtain a user-approved `fileId` through `ask_user_input(kind="upload", upload.target="iframe")`, then call `set_iframe_files`.

### Side-effect boundary

The following actions require confirmation or clearly identified test data before execution:

- Login submission or account registration.
- Order creation, cart submission, or payment.
- Deletion, publication, or sending email/SMS.
- Changing users, permissions, passwords, or real business state.

Do not bypass the UI through backend APIs, database writes, or `run_kscript` to make browser steps appear successful.

---

## 5. Correlating Log and Network

| Observation | Next step |
|---|---|
| Observation | Next step |
| JS stack points to a specific Script line | Read the corresponding Script/View and verify the call context |
| XHR/fetch returns 4xx | Check the URL, method, credential mode, route, parameter source, and permissions |
| XHR/fetch returns 5xx | Query the server Error Log by time and URL, then read the API/Service |
| Request remains pending | Check timeout, duration, and initiator; confirm whether it is triggered repeatedly or waiting indefinitely |
| Same URL requested repeatedly | Use all mode and the initiator stack to find duplicate effects/events/retries |
| Script/CSS/Image fails | Check the final URL, redirects, resource path, and Page/Layout references |
| DOM does not change and there is no error | Check event binding, whether the element is disabled, and whether a conditional branch returned early |
| DOM changed but the user cannot see it | Check visibility, stacking, styles, viewport, and scroll position |

When Network has no response body, do not invent the server response from the status code.

---

## 6. Temporary instrumentation

Add it only when the existing DOM, Log, Network, and source are still insufficient to distinguish hypotheses.

### Browser side

Use a stable, searchable prefix:

```js
console.debug("AI_DEBUG cart-submit", { cartId, itemCount });
```

Then use:

```text
read_preview_diagnostics(mode="debug", search="AI_DEBUG")
```

Record only the minimum fields needed to distinguish branches; do not record passwords, tokens, Cookies, payment information, or complete user data.

### Server-side KScript

Use `k.logger`, never `console.*`. Delete temporary logs after obtaining evidence and rerun the same-path verification.

---

## 7. Standard post-fix verification

1. Save the change.
2. Reload the original URL.
3. Restore the original device.
4. Establish a new document baseline with `observe_iframe`.
5. Find the elements again; do not reuse old nodeIds.
6. Replay the same inputs and actions.
7. Wait for the same result conditions.
8. Confirm that the expected DOM, URL, copy, and state appear.
9. Read Log issues for the new document.
10. Read Network issues for the new document.
11. Confirm that temporary instrumentation has been removed.

“No console error” does not mean it passed; the expected behavior must actually appear.

---

## 8. Minimal case: Loading continues after save

### Reproduce

1. Navigate to the form page and set the device on which the user saw the problem.
2. Observe the page and find the form, save button, and loading element.
3. Read the Log/Network baseline cursors.
4. Enter the minimum input matching the user's case.
5. Click save.
6. Wait for loading to become hidden or a success message to become visible.
7. Read DOM changes after the timeout.
8. Read the incremental Log/Network entries.

### Locate

- If Network has no request: check event binding, validation, and early returns.
- If the request is pending: check duplicate submission, an unfinished Promise, and server duration.
- If it is 4xx: check parameters, route, credentials, and permissions.
- If it is 5xx: locate the API/Service with the server Error Log.
- If the request succeeds but the UI is still loading: check response handling, error branches, and finally-state restoration.

### Verify

After the change, reload the same page and replay it with the same inputs. Confirm:

- loading ends normally.
- The success state actually appears.
- There is no related Log issue.
- There are no failed, timed-out, or duplicate Network requests.
