---
name: kooboo-coding
description: >-
  Kooboo Site / App / Scoped Coding Router. Routes discussion, full-site builds, ToB App Blueprint and App Plan,
  client-side rendering, server-side rendering, App maintenance, external page import, and scoped coding tasks. Enter a resource-writing
  path only when the user explicitly authorizes implementation and this turn has the minimum executable requirements.
---

# Kooboo Coding Router

This file selects the correct reference and defines shared execution priorities and boundaries for Kooboo coding. For public Site building, read the [Site Building entry](references/Scenario/site-building/index.md), then load the planning/static/dynamic references by stage; for ToB App planning or business implementation, read the [App Building entry](references/Scenario/app-building/index.md). The two routes are mutually exclusive: an App resource delivery that has not yet been implemented must not be downgraded to ordinary site building.

For requested Vue App internationalization, or changes to an App already registered with `service.i18n.js`, load `kooboo-app-i18n`. It uses Kooboo Labels with vue-i18n and installs runtime assets only when needed; a site multilingual setting alone does not require App installation.

## Execution priorities and stop conditions

- **Browser verification is opt-in.** Ordinary implementation and repair use source readback, structural checks, and required read-only Preview diagnostics. Do not initiate browser/iframe observation, element search/inspection, navigation, clicks, or device switching unless the user explicitly requests browser testing or operation. Missing browser observations do not block business implementation. Report untested visual/interaction behavior as unverified; never fabricate acceptance evidence. This boundary applies to verification guidance in all coding references.
- Applicable Skill requirements take precedence over speed, convenience, or an early completion summary. Fast mode changes planning defaults, not implementation quality or acceptance. Higher-priority instructions and explicit user authorization still govern; preserve existing authorization and applicable documented exceptions instead of asking again.
- Treat must, must not, prohibited, only, and before-completion requirements as binding within their stated scope and phase. Do not silently reinterpret them as suggestions. Resolve applicability and explicit exceptions from the current task and contract, not keyword matching alone; do not invent exemptions because implementation is difficult.
- If compliance is uncertain, pause the dependent write and inspect the relevant current source, documentation, or rendered behavior. Reuse evidence already read; load only what resolves the uncertainty. Do not guess, copy a familiar implementation from memory, or introduce a temporary substitute to bypass the requirement.
- When a violation or failed prerequisite is found, stop dependent feature expansion, repair the current unit, read it back, and repeat the affected acceptance checks before advancing. Revert only the defective change introduced by this work when necessary; preserve unrelated user changes. Pausing the affected phase is not ending an otherwise authorized task: continue once the issue is resolved. Request user input only when information or authorization is genuinely missing.
- A successful save, syntax check, or generic diagnostics pass is not functional completion. Only the applicable task/phase acceptance criteria and sufficient current evidence permit a completion claim. Missing evidence remains unverified; do not mark Todos or Milestones complete or present a future-work list as delivery.
- Do not use placeholders, demonstrations, or temporary behavior in place of a required finished implementation. A stage-defined placeholder or an already authorized mock strategy remains valid only within that scope; it does not establish later-stage completion. A new reduction in delivery scope requires explaining the difference and obtaining user authorization before implementing it.

Shared Site and App interaction rules: before planning page interactions or writing / changing frontend interaction code, read the [Frontend Interaction Guide](references/Frontend/interaction-guide.md), including scoped interaction work on existing pages. Do not reload an unchanged guide already read in full this session; style-only changes do not acquire extra planning steps or a broader edit scope.

Shared Site and App image/file rules: before imagery or file upload, replacement, deletion, import or generation work, read [Images and file management](references/Frontend/images-and-files.md). Use ImagePlaceholder for missing sample imagery, provide uploads instead of URL-only editable image fields, and implement ownership, references and temporary/unreferenced-file cleanup for persisted files. Apply within the current static/dynamic stage without adding planning steps.

## 1. Determine authorization and task type first

Determine separately:

1. Whether the user explicitly asks to create, build, generate, modify, fix, or continue implementation now.
2. Whether the current product goal, core business outcome, or a clearly identified scoped object can be determined.

An explicit request to update only Blueprint/App Plan permits those planning updates and consistency checks, then stops before code or resource writes. Otherwise, without implementation authorization, only discuss, explain, or perform necessary read-only checks; do not create or modify Kooboo resources.

For business changes to an existing App, discover the target and read current affected Blueprint, App Plan and resources, then analyze planning impact. Changes to Blueprint or App Plan require requirement-review; simple UI-only repairs follow the existing-object repair exception below without planning reads. Failed necessary planning reads remain read-only. See `references/Scenario/app-building/requirement-review.md`.

### Existing-object repairs take priority

A repair to an existing App or Site object follows this scoped path even when the conversation already contains a Blueprint, App Plan, or unfinished Milestone. This path takes precedence over the technical reference-reading and App planning gates below. Authorization and Sandbox boundaries still apply. It does not authorize implementing missing features or changing business scope.

- Use the latest user symptom or selected-element annotation as the target. Preserve previous repair results as context, but do not answer a new layout request by reporting an earlier runtime fix. If the user corrects the scope during authorized work, continue the corrected repair without asking them to authorize it again.
- **Simple UI adjustments do not return to planning**: changes limited to spacing, alignment, colors, typography, dimensions, responsive arrangement, or the display position of existing controls go directly through source inspection, editing, readback, and target UI verification when they preserve data structures, business meaning, action behavior, permissions, routing contracts, and feature scope. This includes an existing App's Shell/Header. Do not read, validate, create, or update Blueprint/App Plan, run Preflight that rereads App Plan, or update the Milestone checklist for this path. Classify by change semantics even when styles live in a Script or shared component. This exception takes precedence over the general Preflight requirements below. Preserve the existing Completion Ledger and pending artifacts; verification of the local UI does not establish App/Milestone acceptance. Read the relevant business contract only if source inspection reveals that the repair requires changing it.
- For spacing, alignment, size, color, or ordering, inspect the selected element, its parent/siblings, and the owning markup and loaded Style rules. Follow actual resource links; in a client-rendered Page the markup may be in a Script. Read the current source with `read_object_text` before patching. When a reliable source line is known, start with a window covering the target and its enclosing structure, then widen as needed. Follow [Object reading and updating](references/Functions/read-update.md) for window parameters and safe patch preparation; full-read requirements still require complete coverage. Console silence or a passing `uiAudit` does not verify the requested appearance.
- For runtime errors, read issues once, then immediately read the named component/Script and relevant dependency. Trace the failing expression through imports, declarations, returned template bindings, and data shape. Read Network only when a failed resource or data request is relevant. Do not poll unchanged errors instead of reading source.
- Reuse references and function schemas already in context. Load another technical reference only for a concrete unresolved contract; ordinary CSS and JavaScript edits do not require rereading the framework overview, component guide, delivery guide, or App planning documents. If source tools are missing, load `read_object_text` and `update_text_object` through `use_function`.
- Do not read or validate Blueprint/App Plan before a local repair unless its intended behavior depends on that contract. Preserve any active App binding. Except for simple UI adjustments above, after the coherent repair batch run focused Preflight for the affected objects under the adapter-selected profile; use [App Preflight](references/Preflight/app.md) for its supported context and verification boundary; do not invent an evidence binding. Do not create planning artifacts, run capability closeout, or mark unrelated Milestones complete for a local repair.
- If `update_text_object` cannot match `oldContent`, the batch was not saved: reread the same object and rebuild the patch from current source. After a failed runtime recheck, inspect the changed source and new error together before another patch. Repeating the same replacement, reloading documents, or adding speculative fallbacks is not a recovery strategy.
- After saving, read back the affected source and verify the original behavior on the same route and viewport. Use a fresh diagnostics baseline after a preview document change; discard old cursors. State any missing runtime or visual evidence and do not claim completion from Preflight alone.

Task routing (for other work):

| Task                                                                                                                          | Entry                                                                                        |
| ----------------------------------------------------------------------------------------------------------------------------- | -------------------------------------------------------------------------------------------- |
| Create or broadly refactor a public content site, marketing site, or storefront                                               | `references/Scenario/site-building/index.md`                                                 |
| Authorized ToB App / back-office business planning                                                                            | `references/Scenario/app-building/index.md`                                                  |
| Client-rendered application, or an explicitly requested Vue 3 CDN, no-build Vue, or browser-run Page/Style/Script application | `references/Frontend/cdn-vue/index.md` (internal implementation reference)                   |
| A single Page, View, API, KScript task, or a specific failure                                                                 | The corresponding Frontend / Backend / K-Script / Debug reference                            |
| Import an external page URL into the current site | `references/Functions/frontend-create.md`, then load `transfer_page` with `use_function` |
| Site implementation and validation in a Sandbox, test site, or isolated environment                                           | First call `load_skill("sandbox")`, then read this Skill's reference for the specific object |
| Discussion only                                                                                                               | Remain in discussion; do not write resources                                                 |

When the user explicitly describes Vue 3 CDN, no-build Vue, or “Vue 3 + CDN/import map + Kooboo Page/Style/Script”, you must first read `references/Frontend/cdn-vue/index.md`, then progressively load its child documents according to its navigation. This is an internal technical reference for client-side rendering, not a product name the user needs to learn or confirm; App Building must first satisfy its selected implementation path's scope validation.

When the user explicitly asks to implement or validate site changes in a Sandbox, test site, or isolated environment, you must load the independent `sandbox` Skill. The `sandbox` Skill owns Sandbox creation, copying, Context switching, validation, and cleanup boundaries; concrete Page, View, KData, KScript, API, and other implementations still follow this Skill's object references. Sandbox authorization is not authorization to write to the production site: do not skip target-site confirmation or describe Sandbox validation as a production launch.

Once the current conversation has generated or read a Blueprint as ToB App work, later expressions such as “continue”, “next step”, or “start implementation” remain in the same App Building context and must not be rerouted to public Site work. An explicit repair to an existing object follows the scoped repair path above, including objects belonging to that App. Other independent public Site tasks may also be rerouted.

## 2. ToB App Building boundaries

Before App planning or business implementation, read the [App Building entry](references/Scenario/app-building/index.md), discover existing planning and resources, and select the path from the user's requested starting point. Do not infer a mandatory planning path from a product category, domain label, number of modules, or a familiar example. A new conversation does not justify recreating planning; a failed planning read does not mean absence. Existing-object repair still takes the local path above first.

- Lightweight implementation requires both Blueprint and App Plan to be confirmed absent. Resume the current saved planning.
- Planning authoring does not authorize resource implementation. Planning-only requests stop after the requested planning changes; implementation requires user authorization and validation of the selected scope.
- App and Site Building are mutually exclusive; do not use Site Building to bypass planning or implementation gates.
- Before changing business definitions or delivery design in an existing App, read [requirement review](references/Scenario/app-building/requirement-review.md). Initial creation follows the Blueprint proposal and save flow. Blueprint semantic creation and updates use the object-specific save contracts in the [Blueprint reference](references/Scenario/app-building/blueprint.md). Ordinary local UI repairs do not trigger planning.

Load the current stage before acting: [Blueprint](references/Scenario/app-building/blueprint.md) before Blueprint writes, [App Plan](references/Scenario/app-building/app-plan.md) before Plan writes, and [client delivery](references/Frontend/cdn-vue/delivery.md) before planned implementation; unplanned implementation reads [lightweight implementation](references/Scenario/app-building/lightweight.md). App Plan uses typed server Functions; execution status is separate. These references own object protocols, failure recovery and delivery requirements. Do not preload unrelated stages.

Before writing client App resources, read the [client implementation entry](references/Frontend/cdn-vue/index.md); before selecting or installing a design system, read [design systems](references/Frontend/app-design-systems.md). Follow these entries to layout and delivery references. Installation alone is not business delivery.

## 3. Site and scoped coding routes

Follow `references/Scenario/site-building/index.md` for the full Site workflow. Do not force a scoped change to a public page to become App Building merely because an App Blueprint exists in the repository.

For new scoped implementations, read by object. Existing-object repairs use the priority path above and read only unresolved contracts:

| Type                                                  | Required documents                                                                                                                                                                                                                                                     |
| ----------------------------------------------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Client-rendered Vue Page                              | `references/Frontend/cdn-vue/index.md` + `references/Frontend/code-style.md`; if they conflict, the specialized reference's rules for complete HTML, dependencies, and module structure take precedence                                                                |
| Page / Layout / View / Style / Script                 | `references/Frontend/code-style.md` + the corresponding Frontend reference                                                                                                                                                                                             |
| `k.*` KScript                                         | `references/K-Script/index.md` + the corresponding module                                                                                                                                                                                                              |
| Backend API / CodeBlock                               | `references/Backend/index.md`                                                                                                                                                                                                                                          |
| Uncertain Function parameters                         | `references/function-guide.md`                                                                                                                                                                                                                                         |
| Multilingual                                          | `references/Scenario/multilingual-site-guide.md`                                                                                                                                                                                                                       |
| Commerce customer flow                                | `references/K-Script/commerce/customer.md` + `references/Scenario/commerce-customer-login-flow.md`                                                                                                                                                                     |
| Commerce customer address / Checkout shipping address | `references/Scenario/commerce-customer-address-flow.md` + `references/K-Script/commerce/address.md` + `references/K-Script/commerce/types/customer.md`                                                                                                                 |
| Commerce storefront dynamicization                    | `references/Scenario/site-building/commerce-dynamicization.md` + the corresponding Commerce / k-data references                                                                                                                                                        |
| Commerce checkout payment                             | `references/Scenario/commerce-checkout-payment-flow.md` + `references/Scenario/commerce-customer-address-flow.md` + `references/K-Script/payment/index.md` + the corresponding provider guide                                                                          |
| Failure / error                                       | The current object reference + `references/Preflight/index.md`; first establish a reproduction baseline, collect minimal evidence, and run focused Preflight. When the user explicitly selects `/debug`, the independent Debug skill owns the complete diagnostic loop |
| Pre-delivery checks                                   | `references/Preflight/index.md`                                                                                                                                                                                                                                        |

If a scoped request changes business definitions in an existing Blueprint, update the affected planning first; unplanned Apps use the lightweight scope and memory contract without creating a Blueprint solely for the change.

## 4. Pre-write checks

Before each tool execution, perform a minimal internal review. For writes, deletes, and code execution, establish all of the following; for reads, check that the read resolves a current need without crossing a phase boundary. Reuse unchanged context instead of rereading all references or adding planning artifacts:

1. The user has authorized implementation for this turn.
2. The applicable Skill, task path (Site, App planning/implementation, or scoped work), and current phase.
3. Which task entry and object reference have been read, or for an existing-object repair, which current source and evidence support the patch.
4. Which mandatory requirements, prohibitions, prerequisites, and documented exceptions apply to this operation.
5. Whether this tool call crosses the current phase boundary or expands the authorized scope.
6. Whether current evidence permits this operation, phase transition, or completion claim, and what remains to be inspected.

If any required answer is missing, pause the dependent operation and obtain the specific missing evidence. Do not guess, lower the standard, or turn this internal review into another checklist the user must supply.

## 5. General coding red lines

1. Progressive reading: load only references needed by the current subtask, and reuse content already in context.
2. Check the real documentation or source before using a `k.*` API; do not invent method names or parameters.
3. After creating or modifying an object, read back key content whenever the object supports readback.
4. Form a structured contract first when data structures, permissions, business flows, or cross-resource relationships are affected.
5. When a 4xx / 5xx, preview failure, Network failure, or explicit error occurs, first establish the original route, device, input, operation, expected result, and actual result; read the current affected source. Run focused Preflight in the first evidence pass for HTTP 500, SSR/template failures, or an unavailable Preview; for a localized client runtime error, inspect its Script first and run focused Preflight after the repair batch. If the user explicitly selects `/debug`, let that skill orchestrate Preview, Network, and known-error playbooks.
6. Unless the user explicitly requests it, page content and controls must not use Emoji by default.
7. Except for simple UI adjustments above, after resource implementation is complete, read `references/Preflight/index.md`, then run Preflight through `run_skill_script`; site-building conversations must not read the implementation source of `scripts/preflight-scan.js` or `references/Preflight/rules/*.yaml`.
8. `run_kscript` returns results through `return` or the final expression; do not replace the return value with `k.response.write/json`.
9. In server-rendered templates, author link URLs with the native `href` attribute and brace interpolation, such as `href="/detail/{item.id}"` or `href="{linkUrl}"`. Use `k-attribute` for dynamic non-`href` attributes; when a link also needs a dynamic class or another attribute, keep `href` native and bind the remaining attributes through `k-attribute`.
10. Before creating or modifying public UI, read [UI diagnostics](references/Preflight/index.md#public-ui-diagnostics) for read-only evidence checkpoints; browser operations still follow the authorization boundary above.

## 6. Navigation

- [Site Building entry](references/Scenario/site-building/index.md)
- [App Building](references/Scenario/app-building/index.md)
- [Scenario index](references/Scenario/index.md)
- [Frontend index](references/Frontend/index.md)
- [Frontend Code Style](references/Frontend/code-style.md)
- [Client-rendered internal implementation guide](references/Frontend/cdn-vue/index.md)
- [Bundled App design systems](references/Frontend/app-design-systems.md)
- [Backend](references/Backend/index.md)
- [App Base services](references/Backend/app-base-services.md): use the installed user/notification APIs as the working reference. Before real data, permission-sensitive APIs or delivery, remove every server mock branch and verify anonymous 401 responses; changing frontend configuration is insufficient.
- [KScript](references/K-Script/index.md)
- [Function Guide](references/function-guide.md)
- [Function References](references/Functions/index.md)
- [Preflight](references/Preflight/index.md)
