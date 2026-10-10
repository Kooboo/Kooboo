# Site Settings - Action Hooks

## Page Purpose

- Admin page: `/system/backend-events`; rule editor: `/system/backend-events/edit`
- Action Hooks execute KScript when supported creation, update, deletion, or business actions occur on site objects or some e-commerce objects.
- They are not a unified audit log for every admin click, database write, or system job. Only event types explicitly exposed by the current system and operations passing through the corresponding entry trigger them.

## Event Stages and Scope

- Objects such as pages, views, layouts, code, menus, HTML blocks, content, Labels, scripts, styles, images, products, and product categories provide before/after create, update, or delete events.
- Some e-commerce business operations also provide dedicated events such as product copy, order confirmation, shipment, and cancellation. The event list is the basis of availability; an object existing in CMS does not prove a Hook exists.
- Creating, Updating, and Deleting run before the original operation is persisted and can inspect or change the current event object. Created, Updated, and Deleted run afterward and are not a reliable transaction rollback mechanism.
- Action Hooks use site-admin execution context, not visitor-request hooks. They have no ordinary page-response path directly observable by visitors and should not handle frontend route-not-found.

## Rules and Code

- Each event type stores independent rules. All conditions in IF must be true for THEN; otherwise ELSE runs. A DO rule runs directly.
- Condition fields are determined by event type and usually concern object name, body, or event-specific fields. Recheck available fields when copying a rule across events.
- Top-level rules, nested branches, and code in one DO execute in order; earlier side effects on the event object or external system can affect later processing.
- Action-Hook code can read and write site data and call external services. A successful save does not mean the code is safe, idempotent, or downstream business succeeded.

## Enable, Save, and Delete

- The master switch controls whether Action Hooks participate in later supported operations. Disabling retains rules and does not replay or undo changes made while disabled.
- Deleting an event deletes all rules under that event type, but does not delete objects, historical site changes, or system support for the event type.
- Creating a rule and enabling the switch do not process existing objects retroactively; use an explicit migration or batch process for historical data.

## Error and Transaction Boundaries

- The current Action-Hook execution entry isolates unhandled exceptions; the original admin operation must not rely on a Hook exception to reliably block saving or deletion. Use object validation or explicit transaction logic for mandatory business constraints.
- A failed after-action Hook does not automatically roll back the completed original operation; sent email, HTTP requests, and other external side effects are not undone by disabling or deleting rules.
- After changes, verify before and after events, matching and non-matching conditions, and key create/update/delete actions separately. Check target data, external systems, and code logs rather than only the admin save notice.
