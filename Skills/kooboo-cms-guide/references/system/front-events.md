# Site Settings - Request Hooks

## Page Purpose

- Admin page: `/system/front-events`; rule editor: `/system/front-events/edit`
- Request Hooks run while a visitor request resolves a site route, page, or view and can execute KScript, change the resolution result, or change the response according to the request and found object.
- They take effect only when Request Hooks are enabled and the request enters through a real site domain or port binding. Direct admin calls, management requests carrying only a site identifier, or calls bypassing a binding are not equivalent validation.

## Event Stages

- Route events include before-found, found, and not-found; view events include before-found, found, and not-found; page events include before-found and found, with no current page not-found event.
- Before Found can intervene before default lookup; Found can inspect or replace the found object; Not Found can provide an alternative result. Exposed condition fields and mutable objects differ by event.
- Request Hooks are not admin content-change notifications or unified access logs. Use Action Hooks for page, content, and other admin create/update/delete events.

## Rules and Execution Order

- Each event type stores an independent rule set. All IF conditions must be true; THEN runs when true and ELSE otherwise; DO needs no condition.
- Available condition fields are determined by the selected event, and string comparison ignores case. Do not apply fields from one event directly to another.
- Top-level rules, nested branches, and multiple code blocks in one DO execute sequentially. Earlier changes to event object, request, or response may affect later code, so drag order is runtime semantics, not only display order.
- Non-empty text returned by KScript is appended to the current response; code can also change URL, route, page, view, status, or redirect. Rules have real request side effects and are not read-only filters.

## Enable, Save, and Delete

- The master switch controls whether saved rules participate in visitor requests. Disabling retains rules; they can execute again after re-enabling.
- Saving a rule proves only that configuration was persisted; it does not mean the event occurs or every condition branch and domain entry is correct.
- Deleting an event in the list deletes all rules under that event type but does not remove system support for the type or affect other event types.

## Risks and Verification

- Request Hooks run in the same request chain as normal rendering. Unhandled exceptions, wrong redirects, invalid replacement objects, or slow external calls can fail the request, create redirect loops, or slow responses.
- After changes, use real binding addresses to test matching and non-matching conditions, existing and missing routes, and different languages and devices, checking status code, response body, final URL, and server logs together.
- Disabling or deleting rules does not undo data written by code, external requests already sent, or results previously returned to visitors.
