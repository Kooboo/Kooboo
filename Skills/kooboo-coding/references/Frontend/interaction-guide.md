# Frontend Interaction Guide

Applies to pages and components in public Sites, member areas, storefronts, and ToB Apps, regardless of framework, rendering mode, or design system. Read before planning page interactions or writing or changing frontend interaction code. No repeat read is needed within the session when the complete guide has been read and is unchanged.

This guide owns task entry, editing containers, feedback, and exit behavior. [Frontend code style](code-style.md) owns code conventions; [Frontend visual design](frontend-design.md) owns visual expression. The selected design system supplies component appearance without removing interaction boundaries. A scoped color or spacing change that preserves interaction does not require a workflow redesign.

## 1. Choose the editing container by task complexity

| Task | Presentation | Decision criteria |
|---|---|---|
| Create or change a few independent fields, such as name, note, or category | Modal dialog | One clear goal, short form, one submission, no complex relationships or child tables |
| Multiple form groups, related objects, child rows, complex validation, or multiple steps | Dedicated detail / edit page | Requires space and stable context; may involve extended work |
| Quick summary or supporting information | Read-only dialog or drawer | Resume the current task afterward; offer access to full details |
| Full details that need sharing or refresh recovery | Detail page with its own URL | Supports direct access, refresh, and browser back / forward |
| Deletion, publication, voiding, or another action requiring confirmation | In-app confirmation dialog | Identify the object, consequences, and final action |

- Simple edits must use dialogs; complex edits must use dedicated pages. Do not use drawers as a general editing container.
- Assess relationships, validation, and workload, not just field count. A BOM with few base fields is still complex when maintaining components, quantities, loss, and versions.
- Use consistent containers and field organization for creating and editing the same kind of object. Complex creation also needs a dedicated page.
- Detail and edit pages may share components, but require clear viewing / editing states, recoverable routes, and save / cancel boundaries.

## 2. Do not assemble editing flows with local visibility toggles

- Do not reveal editing forms beside, beneath, or inside a list row through `v-if`, `v-show`, CSS visibility, or equivalents after Create / Edit. Do not replace routing with booleans that swap a list, full details, and editing in one region.
- Do not reserve list space for an always-present “new object” side card. Before creation starts, show neither an unfinished form nor an editor without a target object.
- Lists support searching, comparing, selecting, and action entry. Read-only previews may use an explicit master-detail layout; editing still follows the previous section instead of turning the preview card into a form.
- Conditional rendering remains valid for mounting dialogs, loading states, permission visibility, conditional fields, and read-only regions. The restriction concerns the interaction pattern, not template syntax.
- A child table inside a complex editing page may support row additions, removals, and cell inputs within the same explicit editing session and shared save / cancel boundary. This is distinct from expanding an editor in a browsing list.

For a BOM list, “Create BOM” opens a dedicated creation page; “View details” opens the detail page; “Edit components” opens an editor with base information, component rows, and validation feedback. A note-only change uses a dialog. Do not progressively build a complex editor in a “New BOM” card beside the list.

## 3. Use in-app feedback components

Do not use browser-native `alert()`, `prompt()`, or `confirm()` for business interactions, including `window.*` forms or indirect wrapper calls. Use dialogs, messages, notifications, and form components consistent with the selected design system.

| Situation | Feedback |
|---|---|
| Ordinary successful save | Brief nonblocking message and updated content |
| Field validation failure | Specific reason and correction next to the field; focus the first error |
| Submission failure | Persistent error inside the form, preserved input, and correction or retry |
| Page loading failure | Explicit error state and retry, not an empty dataset |
| Long or batch operation | Running state, progress when available, and final outcome; distinguish partial from complete success |
| A user decision is required | In-app confirmation dialog with explicit choices |

Do not make a transient toast the only evidence of an important error. Do not require acknowledgment of ordinary saves through success dialogs. Use business language instead of exposing internal exception stacks.

## 4. Complete the edit, save, and exit lifecycle

- Edit a separate draft. Cancel or close must not mutate saved list or detail data. Reinitialize for the target object when reopening; do not retain values or errors from the previous session.
- Validate required fields, formats, and cross-field rules before submission. Frontend validation assists users; the server remains authoritative for validation and authorization.
- Show a busy state and prevent duplicate submission. Report success, exit editing, and update the interface according to the data contract only after a successful response. Preserve input on failure.
- With unsaved changes, close, cancel, backdrop, Esc, in-app navigation, and back actions must follow consistent discard handling. Exit directly when unchanged. Closing during submission must not leave an ambiguous result.
- Use in-app confirmation for in-app navigation. Only when needed to protect dirty input against browser refresh / close, use browser `beforeunload` protection while dirty; this is not a substitute for business dialogs and cannot rely on custom message text.
- Autosave requires an explicitly designed workflow with saving, saved, and failure states and appropriate recovery. Do not silently turn ordinary forms into immediate persistence.
- Honor mock / static delivery strategies: do not claim persistence or introduce real submission, APIs, or storage merely to fulfill an interaction example. Clearly state the available scope of actions that are not connected yet.

## 5. Preserve list, detail, and navigation context

- Preserve search, filters, sorting, and page when returning from details / editing; restore scroll where practical. Update the affected record after saving and return to a valid page after deleting its last record.
- Give details and complex editors stable URLs and object identifiers with direct access and refresh support. Distinguish missing objects, denied access, and loading errors, with a way back.
- Reset pagination appropriately when search or filters change. Prevent stale asynchronous responses from overwriting results for newer conditions. Show active filters and provide clearing.
- Distinguish initial empty data, no matches, loading, and failure, with suitable create, clear filters, waiting, or retry feedback.
- Row actions affect only their record and must not also trigger row navigation. Batch actions show selection count and scope; filtering or pagination must not silently change their targets.

## 6. Action hierarchy, destructive actions, and business states

- Emphasize one primary action per action region. Place page-level creation in the toolbar, record actions on the record, and batch actions with selection. Do not repeat the same primary button throughout the page.
- Name actions and objects, such as “Save changes”, “Delete order”, or “Void record”. Avoid a generic “OK” in confirmation dialogs. Give cancel a clear secondary role.
- Confirmations for deletion, voiding, or overwriting must identify the object, affected scope, and reversibility. Avoid redundant confirmation for ordinary saves. High-impact irreversible operations may require reconfirming the object according to risk.
- Separate “Save draft” from “Publish / Activate / Submit for approval”. Saving must not implicitly advance business state. Respect transition prerequisites instead of bypassing rules to enable a button.
- Explain unavailable actions. Hide or disable unauthorized actions according to product policy, but never substitute frontend visibility for server authorization. No button should silently do nothing.

## 7. Dialogs, forms, responsive behavior, and accessibility

- Give dialogs an accessible title, close control, and appropriate dialog semantics. Move focus inside on open, contain focus within a modal, and restore it to the trigger or a suitable next location on close.
- Support keyboard operation and visible focus. Esc follows the same dirty-state rules as other exits. Avoid stacked dialogs; move complex tasks to pages.
- Use persistent field labels, not placeholder-only labels. Explain units, required status, and formats. Associate errors with fields and never communicate state through color alone. Non-submit buttons explicitly use `type="button"`.
- Simple editing dialogs may become fullscreen modals on small screens; complex editing retains its dedicated page. Manage scrolling so titles, close, and save controls remain reachable and are not obscured by soft keyboards or fixed bars.
- Do not squeeze desktop split layouts into narrow screens. Row actions must not be hover-only; icon buttons need accessible names and tooltips cannot be the only explanation.

## 8. Apply the rules in planning and implementation

- Sites: describe entry, container, save / cancel, and success / failure behavior in existing Page Plan page / interaction descriptions. Mark static demonstration boundaries and preserve the interaction contract during dynamicization.
- Apps: use existing Screens / Actions. Simple editing uses `presentation.desktop: modal` with `modal` or `fullscreen` on mobile as appropriate. Complex editing uses `page` with a `targetScreenId`. Enum support for `drawer`, `split-pane`, and `inline` does not make them appropriate for editing.
- Select the interaction before coding new or changed flows. If an existing plan conflicts, correct the relevant interaction contract through its existing workflow before implementation. Do not silently replace a plan in code or rebuild planning for a style-only change.
- Do not create parallel planning artifacts or extend schemas to store these rules. Use existing descriptions, feedback, and acceptance fields.

Review the actual task: is entry clear, does the container fit complexity, is cancel free of side effects, does failure preserve input, does back preserve context, are destructive consequences explicit, and can keyboard and small-screen users complete it? Runtime validation still follows user authorization and the current delivery stage; this guide does not automatically expand tests or builds.
