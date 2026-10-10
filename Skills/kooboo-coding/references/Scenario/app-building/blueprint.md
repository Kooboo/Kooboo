# Business Blueprint Modeling

The reviewed semantic graph lives in the site's `AIApplicationStore`. Save one reviewed object per specialized Function call; the initial structure uses `save_blueprint`.

## Modeling flow

Discuss participants, goals and business Scenarios. Use `propose_blueprint` for a non-persisted initial proposal, show it to the user, then save the approved root/Actors/Goals/Modules/Scenarios with `save_blueprint`. Continue reviewing complete FCO-IM Facts, derived Business Objects, required Business Rules and User Stories individually. Do not require every object type for every business.

## Current reads and model ownership

Call `read_app_blueprint` with `{}`. It returns `exists`, `blueprint`, `actors`, `goals`, `modules`, `scenarios`, `facts`, `businessObjects`, `businessRules`, `userStories`, `relations`, `counts` and `canCreateAppPlan`. Reuse a successful read while the graph is unchanged; reread after writes, known changes or failed/unknown reads. The Function has no view or pagination parameters.

The root contains `id`, `name`, `description`. Other objects are stored independently. Use returned GUIDs for dependencies; new object IDs may be omitted in single-object saves and are assigned by the server. Reuse saved IDs for updates. Facts use `kind` and `fcoim`; Business Objects preserve `fields`, `sourceFactIds`, `dbName`, `storage` and `stateDimensions`. Unresolved questions and assumptions belong in [Planning Topics](planning-topics.md).

`canCreateAppPlan` checks the presence of root, Modules, Scenarios, Facts and Business Objects. It does not prove requirement coverage or grant planning/implementation authorization. The root ID identifies the site singleton, not a content revision.

## Save boundaries

All save Functions require user approval. Read the relevant Function schema before submitting. Save dependencies before dependents; never resubmit the full graph for an individual object update.

| Function | Input and relationship ownership |
|---|---|
| `save_blueprint` | `proposal` contains only `blueprint`, `actors`, `goals`, `modules`, `scenarios`, `relations`. Initial creation requires the root and initial structure; use the IDs from `propose_blueprint`. For a root update, send only the complete root. Structure relations use `sourceType`, `sourceId`, `targetType`, `targetId`: `Goal -> Actor`, `Module -> Scenario` and root links. |
| `save_actor` | One complete `actor`; the server links it to the current root. |
| `save_goal` | One complete `goal`, plus stored `actorIds`; a new Goal needs at least one Actor link. |
| `save_module` | One complete `module`, optional stored `scenarioIds`; root linkage is automatic. |
| `save_scenario` | One complete `scenario`, stored `moduleIds`; a new Scenario needs at least one Module link. |
| `save_fact` | One complete `fact`, optional stored `scenarioId`. Include the reviewed reading, object types, roles, fields, constraints and examples. |
| `save_business_object` | One complete `businessObject`, derived from already saved reviewed Facts. |
| `save_business_rule` | One complete `businessRule`, optional stored `scenarioIds`; the server creates root and Scenario links. |
| `save_user_story` | One complete `userStory` with beneficiary `actorId`, need, business value and Given/When/Then `acceptanceCriteria`; `goalId` is required for creation, plus optional stored `scenarioIds`. The server creates its primary Goal and Scenario links. |

A Module is a business capability area, not a table. A Scenario may belong to multiple Modules without duplication. Submitted objects replace their saved definitions in full: preserve unchanged fields. Omitted objects/links are retained; empty collections do not delete them. Deletion or changing a story's existing primary Goal requires resolving the requested change before dependent saves; these Functions do not remove links.

## Business Rules

Create rules when validation, calculation, authorization, transition, derivation or policy behavior requires them. Review meaning and source first. A rule needs name, statement, `reviewStatus="confirmed"` and exactly one `specification` member matching `type`. Follow schema enum values exactly; for example `validationKind="unique"`. Referenced Facts, Actors, Business Objects and states must already be saved. Supply `scenarioIds` rather than constructing rule relations.

Input-shape example for a reviewed policy rule on an existing Blueprint (not an executed site operation):

```json
{"businessRule":{"name":"Decision reason","type":"policy","statement":"A decision records its reason.","reviewStatus":"confirmed","specification":{"policy":{"requirements":["Record the decision reason."]}},"source":{"type":"user-stated","description":"Reviewed requirement"}},"scenarioIds":[]}
```

## Save verification and recovery

After saving, read the graph and verify the saved object and links. For invalid arguments, correct the reported field using its expected enum/shape or GUID, and retry only that object's Function. Preserve the approved meaning and ID; do not regenerate unrelated objects or reload unchanged references. Missing dependencies must be saved first. Validation finishes before writes. After a storage/runtime failure, read back before retrying: there is no multi-store rollback guarantee. A failed read is not absence.

## Persistence boundary

Saving records business definitions only; it does not implement Pages, APIs, databases, access control or rule enforcement. Report persisted results. Business-modeling-only requests end after the authorized semantic scope. For planned delivery, continue within existing authorization after semantic readiness; saving itself grants no additional authorization. Follow [App Building](index.md) for the next authorized stage.
