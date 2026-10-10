# Review an existing App requirement change


Read the current Blueprint graph with `read_app_blueprint` using `{}` and select affected objects by their returned GUIDs; see [Blueprint](blueprint.md). Read the current server App Plan with `read_app_plan({})` and select affected objects by their returned IDs; see [App Plan](app-plan.md). Correct parameter errors in the affected call without rereading still-valid data.

`review_app_requirements` keeps the discussion identifier inside `review.discussionId`, not at the top level. The tool is deferred in AI Chat, so load its InputSchema with `use_function` before the first call in a conversation.

## Assess planning impact before choosing a workflow

For each new request, discover the target with `get_site_info`. Read the current Blueprint graph, then the affected App Plan objects and resources. Use persisted current definitions as the baseline. The system tracks successful reads automatically.

Analyze whether the request changes existing definitions or business behavior. This protocol applies to changes in an existing App. For a new App's initial creation, follow the first-creation SOP: show a `propose_blueprint` proposal and, after explicit user approval, call `save_blueprint`; do not call `review_app_requirements`. New capabilities and changes affecting Blueprint or App Plan in an existing App require `review_app_requirements` before planning or resource writes. Styling, wording and repairs restoring specified behavior may proceed as ordinary authorized maintenance. Adding a category icon field requires review; changing an existing icon's spacing normally does not.

Failed reads or unresolved impact permit read-only analysis and a draft review, not implementation. Do not mark a review ready until current planning is readable. An ordinary preference choice does not authorize implementation. Only the exact unchanged reviewed scope may reuse its authorization; independent requirements need their own review. Initial builds on sites with neither planning artifact retain the initial-build SOP.

## Discuss before writing

Read the current Blueprint graph, affected App Plan objects, relevant Topics and resources. Identify what the user explicitly requested, what remains uncertain and which existing behaviors must be preserved. A request such as “support product image uploads” opens requirement discussion; it does not confirm a single-image limit, gallery behavior, storage policy or unrelated page changes.

For an existing App, present a concise proposal with `review_app_requirements` before planning or resource writes. Keep material unanswered decisions empty and required. Store independent questions, suggestions and answers using PlanningTopic; Topic confirmation records discussion only. Do not label inferred choices as `user-stated` facts. For a planning-impacting change in an existing App, use the requirement review even when the initial request asks for implementation; only continuation of the exact previously reviewed and authorized scope avoids repeated confirmation.

## Editable review contract

The function accepts `question`, optional `description`, and `review`. Pass `question` as the user-facing title and `review` as:

```json
{
  "discussionId": "product-images",
  "analysisStatus": "draft",
  "impacts": [],
  "notes": "",
  "sections": [
    { "id": "requirements", "fields": [
      { "id": "image-purpose", "label": "Requested capability", "type": "textarea", "value": "Upload product images to help identify products.", "required": true },
      { "id": "image-count", "label": "Images per product", "type": "select", "value": "", "required": true, "options": [
        { "id": "single", "label": "One main image" },
        { "id": "gallery", "label": "Up to five images" }
      ] },
      { "id": "image-actions", "label": "Suggested image actions", "type": "multiselect", "value": ["replace", "remove"], "options": [
        { "id": "replace", "label": "Allow replacement" },
        { "id": "remove", "label": "Allow removal" }
      ] }
    ] },
    { "id": "pages", "fields": [
      { "id": "list-thumbnail", "label": "Show thumbnails in the product list", "type": "boolean", "value": true, "current": "The list displays product text only." }
    ] },
    { "id": "data", "fields": [
      { "id": "product-images", "label": "Product image information", "type": "textarea", "value": "The product will reference uploaded images. The number of images remains to be confirmed.", "readOnly": true, "current": "Products have no image information." }
    ] },
    { "id": "acceptance", "fields": [
      { "id": "image-persistence", "label": "Acceptance criterion", "type": "textarea", "value": "Saved images remain visible when reopening the product.", "required": true }
    ] }
  ]
}
```

Exactly four unique sections are required. Field IDs are globally unique within the review; option IDs are unique within each field. Supported types: `text`, `textarea`, `boolean`, `select`, `multiselect`. Value is a string, boolean or string array appropriate to the type. Select values reference option IDs; use `""` for an unanswered single choice. `current` is an optional before-state description. Set `readOnly=true` only for derived analysis; users can supply corrections through `notes`. Use the conversation language for labels and values. Do not display internal planning IDs, JSON, version details or tool names.

The user can edit text, scope selections and acceptance criteria, and add, rename or delete choice options. New choices are selected; deleting a selected single choice makes it unanswered. Every edit invalidates the analysis. The inline requirement message guides the user through clarification, impact review and scope confirmation. Collapsing it retains the draft; the chat composer can submit additional feedback together with the current draft for analysis. The frontend retains each submitted round and a read-only confirmation snapshot. Browser-tab storage retains unsent edits for the same call when available.

## Structured planning impacts

`discussionId` identifies one discussion across analysis rounds. Reuse it when answering `analyze`; after `confirm`, `plan` or `implement`, a further requirement change starts a new discussion ID and keeps the previous confirmation immutable. It is a correlation ID, not a planning version.

Use `analysisStatus="draft"` while material choices are unresolved. Set `ready` only after reading relevant current planning and deriving impacts from the completed choices. Filling a form does not itself make analysis ready. Unanswered required choices still block confirmation. Chat feedback returns `analyze` with the current draft and verbatim feedback; it never authorizes implementation. Preserve custom options and selections; ask a required question for ambiguous changes instead of guessing.

`impacts` is an array of `{id,target,path,label,change,before,after,reason,fieldIds,dependsOn}`. `target` is `blueprint` or `app-plan`; `change` is `add`, `modify`, or `remove`. `path` is an actual planning property/ID path from current reads (a proposed stable ID for additions). Reuse existing IDs; never use array indexes. Labels, before/after descriptions and reasons are concise user-facing text. `fieldIds` references the requirement fields that motivate the change; `dependsOn` references upstream impact IDs and describes proposal dependencies, not business-object relationships. Empty arrays are allowed; do not fabricate changes merely to fill each dimension.

For product images, Blueprint impacts may introduce a product-image relationship, a maximum count, and a main-image business rule. App Plan impacts may cover image storage/references, upload actions, the product form, and the list display. Choose a field, image array, or related object based on actual existing design and the user's choices; do not present alternatives as simultaneous changes. Inspect referenced screens before proposing image-plus-title on other lists, and ask whether to include them if that scope is undecided. Each item compares the read current state against one proposed outcome, with explicit links from business rules to affected delivery objects. Impact references are proposals, not executable patches or proof of writes.

## Interpret the user's action

The result is `{confirmed, value, feedback}`, where `value={action, review, implementationAuthorized}`:

- `action="analyze"`, `confirmed=false`: the user requests another analysis, not cancellation. Preserve all edited text, stable IDs, custom options, selections, notes and acceptance criteria from `value.review`. Re-read only relevant facts, apply the user's feedback to the editable choices, update structured impacts, reuse discussionId and return another `review_app_requirements`. Explain material conflicts with a new required question; do not silently rewrite a user answer. This action authorizes no planning or business-resource writes.
- `action="confirm"`, `confirmed=true`, `implementationAuthorized=false`: record the requirement answers in Topics and stop for implementation instructions. Do not write Blueprint, App Plan or resources.
- `action="plan"`, `confirmed=true`, `implementationAuthorized=false`: record answers, update only the reviewed Blueprint and App Plan through their respective local-update contracts, check their consistency, then stop. Do not create or modify code, resources or business data.
- `action="implement"`, `confirmed=true`, `implementationAuthorized=true`: record the answers, update only affected planning content and implement only the reviewed scope.

Do not treat the outer `confirmed` field alone as implementation authorization. Analysis must remain read-only apart from recording discussion in Topics. Do not test storage by writing probe resources while discussing a requirement.

## Local planning updates

Read the affected definitions and reuse their saved IDs. Before Blueprint writes, load the [object-specific save contracts](blueprint.md#save-boundaries). After planning-update or implementation authorization, use the matching `save_*` Function for each reviewed complete object. Preserve unchanged fields and saved IDs; save dependencies first. Use its typed ID parameters to add links. Omitted objects and links are retained. Updating business definitions does not require a new root proposal or implementation pass.

Object and relation deletion has no operation in this save contract. If a reviewed requirement needs it, record the unresolved deletion and resolve that operation before dependent writes; do not represent removal by omitting an object or clearing a collection. Do not claim deletion succeeded.

For affected App Plan content, follow [App Plan authoring](app-plan.md). Submit the complete changed object through its typed save Function, preserving unchanged fields and stable IDs from the current read. A local Action change requires saving its complete owning Screen; a field or operation change requires saving its complete DataContract or Service. Use `delete_app_plan_objects` for reviewed top-level deletion, repairing references first or deleting the dependent objects in the same batch.

Update Blueprint first, then affected App Plan definitions. Preserve the selected design and unaffected content. Read back both saved results and reconcile the reviewed scope before authorized implementation. Execution progress stays outside the Plan; no Milestone binding is required for its authoring or implementation. If a write fails, follow that artifact's recovery contract.

## Impact analysis scope

During discussion, use the current graph and affected Plan/resource reads with concise `impacts` describing current behavior, proposed changes and their reasons. Reference existing planning IDs where available. Prepare Blueprint object batches and App Plan updates only after planning-update or implementation authorization. Mark analysisStatus ready when material choices are resolved and the affected scope has been explained.
