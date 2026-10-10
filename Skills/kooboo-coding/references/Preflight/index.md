# Preflight Core and Profile Routing

Preflight is the mandatory delivery gate for Kooboo coding tasks. One shared scanner core owns artifact loading, focused/capability scope, rule execution, structural receipts, and Completion Ledger integration. Delivery semantics are selected through an adapter-authoritative profile:

- [Site Preflight](site.md): Site Brief/Page Plan delivery, public-site stages, Commerce, Payment, multilingual behavior, and closeout checks.
- [App Preflight](app.md): planned App Plan bindings and the shared structural profile for lightweight Apps, with the applicable structural and delivery evidence.

For lightweight Apps, the adapter selects the shared `site` structural profile. This is not a switch to Site Building or permission to bypass existing planning.

The model does not choose the effective profile or manufacture evidence bindings. The adapter defaults to the site structural profile. A successful current server Plan read selects the App profile and its session-local evidence context. See [App Preflight](app.md) for planned and unplanned contexts, invalidation and recovery. Design reads do not authorize implementation.

Execution reads this file and the selected profile contract, then runs `scripts/preflight-scan.js` through `run_skill_script`. `scripts/preflight-scan.js` and `references/Preflight/rules/*.yaml` are execution implementations and are not loaded into conversation context through `read_skill_resource`; the server allows rules to be loaded internally only when the current trusted execution script is confirmed to be this scanner. Other scripts and direct tool calls still return an execution-only notice.

## Simple UI adjustment exception

Simple UI adjustments identified by `kooboo-coding` as unrelated to business do not require this delivery scan. Read back the changes and verify the target element's appearance and existing interaction on the original route and viewport. Do not restore an App binding, reread Blueprint/App Plan, or trigger capability closeout for a style repair. This exception takes precedence over the general requirement for any repair pass below. Retain pending artifacts in the existing Ledger; the overall delivery gate does not assess the local UI result and this exception cannot establish App completion.

## When to run it

In the following states, whenever code or a site object is created or modified, Preflight must run before the final reply to the user:

- S4 static site implementation
- S5 static iteration
- S9 dynamic implementation
- S10 dynamic iteration
- Any bug fix / repair pass that modifies code or a site object

If any `BLOCKER` exists, do not claim the task is complete. Fix it first. If it cannot be fixed in the current turn, explicitly report the blocker, affected object, and reason it cannot be fixed.

## Invocation cadence: focused scan vs capability scan

The default mode is `capability`, ensuring that callers without an authoritative dirty scope cannot silently omit objects. To reduce iteration cost, the caller must explicitly select `focused`:

- On each round (each user feedback or write batch), pass only **dirty objects + direct consumers**, and mark objects written in this round with the `dirty` parameter; do not pass every object in the capability slice on every round.
- Treat one coherent Slice or repair set as one write batch: finish its related object writes and readbacks, then run one focused Preflight. Do not run Preflight after every individual Page, Style, Script, API, or CodeBlock write inside the same batch.
- `mode: "focused"` (explicit): scan dirty objects and direct consumers. Only non-cross-object, non-Commerce artifacts may be reused when a trusted adapter explicitly passes `useCache: true` and a context-matching `cache`; otherwise read and scan them again.
- `mode: "capability"`: disable cache reuse and rescan the current delivery scope in full. The AI Chat adapter authoritatively merges every successfully persisted site object from this conversation in the Completion Ledger into `artifacts`; the model may add unwritten direct consumers or existing objects belonging to the same capability, but cannot shrink the tracked scope.
- A focused scan cannot be used to claim stage completion: it returns `completionGate.focused=true`, `claimAllowed=false`, and one `missingEvidence` requiring a capability scan first.

### Dirty set

After a write, the AI Chat Completion Ledger records the standard `site_write` receipt and the adapter authoritatively injects `dirty` into Preflight; the model does not submit or override this set. The adapter also automatically adds dirty objects to focused `artifacts`; the scanner retains `dirty_scope_incomplete` as a second-layer check. Example model invocation for a focused scan:

```json
{
  "skillName": "kooboo-coding",
  "scriptName": "scripts/preflight-scan.js",
  "arguments": {
    "taskType": "dynamic-page",
    "deliveryStage": "commerce-runtime",
    "layers": ["frontend", "kscript", "integration"],
    "mode": "focused",
    "artifacts": [
      { "name": "home", "objectType": "Page", "layoutName": "main" }
    ]
  }
}
```

During execution, the adapter merges the dirty `product_list` View back into `artifacts`. The output `directConsumers` lists objects in this batch that reference the dirty object; the next focused scan should pass those consumers too. The adapter can authoritatively complete written objects only and cannot infer every unmodified consumer, so the model still needs to include existing direct consumers for the current capability; focused is never a final check.

### Explicit cache safety boundaries

- Caching is disabled by default; the scanner does not read or write site resources through `k.file`. A trusted adapter manages the cache outside model context.
- Reuse is possible only when `mode: "focused"`, `useCache: true`, an explicit `cache`, matching rule versions, matching scan context, an object that is not dirty, and comparable equal Sizes are all present.
- Always reread Page / Layout / View / Style and every Commerce artifact because resource references, Layout/Page assembly, Products bundles, and runtime-evidence decisions require real source.
- Size is only a low-cost heuristic; same-length rewrites may still invalidate content. The stage must close with a full `mode: "capability"` scan.
- The cache is invalidated whenever the scanner rule version (`RULE_VERSION`) changes. **Increment `RULE_VERSION` whenever any rule YAML, custom check, or scan logic changes**; otherwise an old cache may reuse results from old rules.

### Added parameters and output

| Parameter | Default | Description |
| --- | --- | --- |
| `mode` | `capability` | `focused` / `capability` |
| `dirty` | `[]` | Adapter-injected list of objects written in this round (`nameOrId` + `objectType`); the model does not submit it |
| `cache` | none | Optional cache object or JSON explicitly injected by a trusted adapter; the scanner does not persist it |
| `useCache` | `false` | Whether an explicit cache may be used; effective only in focused mode |

New output includes `scanSummary` (mode, ruleVersion, `cacheEnabled`, per-object status and counts, `dirtyCount`/`artifactCount`; focused with empty dirty and artifacts≥2 includes `warnings: ["focused_without_dirty"]`), `cachePersisted: false`, `directConsumers` (objects in this batch that reference dirty objects), and `skippedChecks`, which appears only when structural-check dependencies are unavailable. `structuralReceipt.complete` indicates whether all structural checks ran. When a focused scan cannot claim completion, `nextActions.beforeClaimCompletion` instructs the caller to run capability first.

The complete scanner return is persisted so the Completion Ledger can validate the receipt, object hashes, and evidence requirements. When later model requests are sent, history compaction retains only actionable fields from the latest Preflight: conclusion and findings, skipped checks, receipt passed/complete/mode/ruleVersion/evidence requirements, aggregate gate, count-based scan summary, next actions, and direct consumers. Repeated `verifiedArtifacts` and normal per-object scan details are not reinserted into model context. Earlier Preflight results are superseded by later scans and retain only a superseded marker; scanner/rule implementation text already read in history is also replaced with an execution notice. This compaction does not change persisted raw evidence.

## Known-error routing

Scan results may include top-level `knownErrors` and a `knownError` in the corresponding finding:

```json
{
  "knownErrors": [
    {
      "id": "KOOB-CDATA-WRAPPER",
      "priority": "immediate",
      "playbook": {
        "skillName": "debug",
        "resourceName": "references/common-errors.md",
        "anchor": "3-koob-cdata-wrapper"
      },
      "ruleIds": ["PF-UNIVERSAL-004"],
      "artifacts": ["Page:home"]
    }
  ]
}
```

This means the scanner hit a common error with a deterministic handling path. When the user explicitly selects `/debug`, use `playbook.skillName` and `playbook.resourceName` to call `read_skill_resource`, then locate the corresponding section with `playbook.anchor`; generate other hypotheses only after handling this error. `priority="immediate"` means not to keep waiting for Preview or repeat Browser Control. See the Debug skill's [Kooboo common-error fast paths](../../../debug/references/common-errors.md) for currently integrated entries and maturity.

## Required script invocation

Before the final reply, invoke the scanner through the current skill:

```json
{
  "skillName": "kooboo-coding",
  "scriptName": "scripts/preflight-scan.js",
  "arguments": {
    "taskType": "site-page",
    "deliveryStage": "static-first",
    "layers": ["frontend", "design", "kooboo-template"],
    "mode": "capability",
    "artifacts": [
      {
        "name": "main",
        "objectType": "Layout"
      },
      {
        "name": "home",
        "objectType": "Page",
        "layoutName": "main"
      }
    ]
  }
}
```

By default, `artifacts` only needs object references, avoiding the token cost of putting complete code into Function parameters. The scanner calls `read_object_text` internally to obtain the latest content.

**Capability detection and cross-object checks**: the scanner hydrates content and reads platform metadata before loading applicable rule groups. App/Site constrains the scenario; Layout/Page/View assembly, assets, forms and KScript/API checks are shared. k-data and Commerce+k-data currently apply only to Site. `layers` and `taskType` are descriptive; omitting them cannot disable a detected capability. See [Capabilities and evidence boundaries](capabilities.md).

Persisted Page LayoutName and real Routes come from `list_site_object`; source `<layout>` tags provide detection and cross-checking. Unavailable binding metadata yields `skippedChecks`. Placeholder names must be non-empty and match; bound Layouts and referenced Views must exist. Repeated Views and empty data-blocks produce composition advice because reusable Views and client mount points can be intentional.

Asset checks resolve relative URLs, Page `<base href>`, CSS URLs, literal module imports and Page import maps included in the scan. Correctly resolved relative URLs are valid. Missing owning Page/URL/import map, computed dynamic imports or necessary inventories yield incomplete checks. External assets and dynamic template values need runtime evidence; static acceptance does not prove successful module loading.

`findings` contains structural issues; `advisories` contains intent, data-flow or runtime questions requiring review. Advice does not change structural PASS and is not evidence of success. Explicit private-key material remains a redacted BLOCKER; heuristic credential candidates are advice. Missing information needed for structural verification sets `structuralReceipt.complete=false` and keeps the completion gate closed.

Recommended fields:

| Field | Required | Description |
| --- | --- | --- |
| `name` | Yes | Object name, such as `home`, `main.css`, or `Services.product` |
| `nameOrId` | No | Pass this when using a GUID or an identifier different from the display name; takes precedence over `name` |
| `objectType` | Yes | `Page` / `Layout` / `View` / `Style` / `Script` / `Api` / `CodeBlock` |
| `content` | No | Only for unsaved temporary code fragments or test scenarios; do not pass it for ordinary Kooboo objects |
| `url` | No | The real Url of a routed object such as an Api. Saved objects are usually filled automatically through `list_site_object`; when scanning an unsaved named action API, pass it explicitly to validate the `{action}` route contract |
| `layoutName` | No | Read from the platform for saved Pages; optional expected binding for unsaved source. App and Site share applicable template assembly checks. |
| `multilingual` | No | Platform multilingual activation comes from get_site_info; model metadata cannot override it. |
| `commerceModel` | No | `catalog-contact` / `online-checkout`; may be passed at top level as the default for all artifacts or specified per artifact |
| `commerceCapability` | No | Explicit Commerce context such as auth/checkout/payment/orders. Ordinary App pages with similar names do not activate Commerce. |
| `commerceSurface` | No | Explicit Commerce page/endpoint responsibility. Actual Commerce API/query use or explicit Commerce context enables checks; names such as login/orders/account alone do not. |
| `deliveryStage` | Required for Commerce | `static-first` / `commerce-runtime` / `payment-runtime`; determines which rules belong to the current delivery stage |

When updating an existing object, the script reads the latest content saved in Kooboo. If an object is not yet saved but still needs scanning, pass `content` explicitly.

Commerce Preflight must receive an explicit `deliveryStage`:

- `static-first`: Run general static rules; defer Commerce dynamic rules so they do not block static delivery.
- `commerce-runtime`: Activate product, category, PDP, login, Header, Cart, Checkout, and order-ownership rules; payment-provider rules remain `deferred`.
- `payment-runtime`: Activate payment endpoints, provider callback / checkStatus, and real order payment-status rules.

The scanner selects rules by actual capability and then splits applicable findings into active/deferred by stage; `result` is determined only by active findings. `deferred` returns only the later stage and unique rule ID, without repeating the full finding.

Preflight Core must produce a valid `structuralReceipt`: `passed` for whether structural rules have no active `BLOCKER`, `complete` for whether all structural checks ran, the scan mode, rule version, verified objects and their content hashes, and `evidenceRequirements[]` with stable `id` values and `runtime | business` levels. `passed=true` with `complete=false` means structural checks were incomplete because a dependency was unavailable; it is not a complete PASS. AI Chat accepts only a complete receipt produced by `kooboo-coding`'s `scripts/preflight-scan.js`; missing fields, invalid object shapes, another same-named skill script, or a legacy return containing only a compatible `completionGate` all block completion.

The AI Chat Verification Ledger aggregates `site_write`, `structuralReceipt`, and later controlled runtime evidence; only the Gate Evaluator decides the final `claimAllowed`. After completing all steps for a runtime requirement, submit the `verificationRequirementId` returned by that receipt on the last successful Preview/browser assertion; the adapter verifies that it belongs to the latest `structuralReceipt`, comes from a registered browser tool, and has the runtime level before recording evidence. The model cannot submit `level` or mint evidence by returning same-shaped JSON from a script. The payment completion gate remains closed until the business producer is integrated. Any new write invalidates old runtime/business receipts.

The Ledger retains `trackedArtifacts` for objects successfully written in this conversation. A focused PASS only clears dirty markers and does not shrink this scope; the capability receipt must cover every tracked artifact with matching content hashes, or the adapter's aggregate gate remains false. Saving the SiteMilestone checklist is a progress record only: it does not clear or reset the tracked scope, Preflight receipts or runtime/business evidence. The scanner's compatible `completionGate` is for human-readable hints only and cannot replace `structuralReceipt`.

`completionGate` always retains `claimAllowed`, `maxCompletionLevel`, and `verifiedArtifacts[]`; `missingEvidence` and `unresolvedErrors` are returned only when non-empty. Consumers must treat missing arrays as `[]`.

If the current object belongs to a multilingual site, pass `multilingual: true` on the artifact when appropriate. The scanner then additionally blocks:

- Hand-written `href="/products?lang=en"` / `href="/products?culture=en"`
- Language-prefix links such as hand-written `href="/en/products"`
- Internal navigation such as `location.href = "/cart"` or `k.response.redirect("/checkout")` that was not generated through `k.site.multilingual.getUrl(...)`

## taskType values

Choose the type closest to the current task:

| Task Type | Applicable scenarios |
| --- | --- |
| `site-page` | Layout, Page, View, static blocks, landing pages |
| `style` | CSS / Tailwind changes only |
| `script` | Frontend JavaScript |
| `api` | API objects |
| `codeblock` | Service / Utils CodeBlock |
| `dynamic-page` | Pages involving frontend, KScript, k-data, APIs, or data sources |
| `form` | Forms, submission, login/registration, contact flows |
| `data` | Data sources, content, sqlite, commerce, or structural design |
| `fix` | Bug fixes or rework |
| `all` | Broad or mixed tasks |

## layers values

Pass every layer actually involved in this round:

- `universal`
- `frontend`
- `design`
- `kooboo-template`
- `kscript`
- `backend`
- `data`
- `forms`
- `security`
- `accessibility`
- `performance`
- `integration`

The scanner always includes `universal` rules. Pass the other layers explicitly according to this round's changes so rule selection is accurate enough.

## Result strategy

The scanner aggregates findings by rule ID; hits for the same rule across multiple objects go into `occurrences[]`, without repeating title, fix, pattern, or tags. Severity semantics:

- `BLOCKER`: Blocks delivery and must be fixed before completion.
- `HIGH`: High risk; fix when possible, and explain it in residual risk if retained.
- `MEDIUM`: Quality issue; fix when it is within the current scope.
- `LOW`: Optimization suggestion.

Final-reply rules:

- If `result` is `FAIL`: fix blockers first, then run Preflight again.
- If `result` is `PASS_WITH_WARNINGS`: structural checks are deliverable, but explain retained high-risk items and continue to follow `completionGate`.
- If `result` is `PASS`: this means only that currently enabled structural rules passed; claim stage completion only when `completionGate.claimAllowed=true`.

## Site-building closeout checks

This section applies to Site Building tasks. App completion follows [App Preflight](app.md): planned Apps bind the Plan; lightweight Apps use the shared Site structural rules, without adopting Site Building closeout stages.

After a site-building task passes Preflight, perform one non-script closeout check before the final reply:

1. If the current conversation still has incomplete todos, call `update_todos` first to set their accurate status.
2. When the user-visible stage is complete, record the finished checklist items with `update_site_milestones`. The checklist is a progress record: do not mark an item `completed=true` while its verification is still pending.
3. If there is a natural next step, call the default Function `show_next_recommended_action` directly; prioritize incomplete checklist items the user has asked to deliver next. Show 2-3 items only when multiple capabilities can start independently and each has Site Brief/Page Plan evidence.
4. If the current task only fixes a blocker, performs scoped rework, or has no natural next step for the user, do not force a recommendation.

Non-FAIL results return minimal `nextActions`: always include `always: [update_todos]` and `whenNaturalNextStep: [show_next_recommended_action]`; include `whenStageComplete: [update_site_milestones]` only when `completionGate.claimAllowed=true`. This is not a finding and does not change `PASS` / `PASS_WITH_WARNINGS` / `FAIL`.

`show_next_recommended_action` is not a skill script. Incorrect example; do not copy:

```text
错误：run_skill_script({
  "skillName": "kooboo-coding",
  "scriptName": "show_next_recommended_action",
  "arguments": { "actions": [] }
})
```

Call the default Function `show_next_recommended_action` directly with `{ "actions": [...] }`.

## Rule sources

Rules under `references/Preflight/rules/` are grouped by capability:

- `common.yaml`: completeness, links, content wrappers and content-policy advice.
- `frontend.yaml`, `forms.yaml`: presentation, accessibility and forms.
- `template.yaml`, `assembly.yaml`: shared template assembly and skeleton advice.
- `k-data.yaml`: Site k-data syntax and bindings.
- `kscript.yaml`, `multilingual.yaml`: platform APIs and enabled platform multilingual routes.
- `data-contract.yaml`, `security.yaml`: data-strategy review, credentials and code safety.
- `commerce.yaml`, `commerce-template.yaml`, `payment.yaml`: Commerce, Commerce+k-data and payment-provider capabilities.
- `assets.yaml`: resource references, image-source advice and import maps.

Every rule declares `profiles` and `capability`; combined capabilities declare `requires`. Ordinary and cross-object checks use the same filtering. `advisory: true` is separate from structural findings. `requiredStage` controls delivery-stage checks; dangerous payment-state writes already present in code are not hidden by the static stage. New rules need positive/negative fixtures and a bounded failure signal; unknown intent or missing evidence must not be presented as a proven defect.

## Public UI diagnostics

The root Skill owns browser authorization. This section defines checkpoints and evidence for existing read-only diagnostics; it does not grant browser-operation permission.

When creating or modifying public UI, static source and Preflight cannot replace final browser styles. After the first browsable page is formed, read `read_preview_diagnostics(mode="issues")` once; when the final capability Preflight returns `site.ui-runtime`, read it once more against the final rendered page with that `verificationRequirementId`. `uiAudit` checks for missing CSS and severe overflow, and when the design system or CSS variables provide a color baseline, checks whether large-area backgrounds drift and brand colors degrade to black and white. `colorAudit` returns the baseline source, actual primary background, drift ratio, and brand-color hit count. `passed=true` only means these limited rules found no blocking errors; it cannot prove that a requested visual, layout, spacing, or style fix is correct. For a UI issue reported by the user, independently check the target element and original expectation; never claim it is fixed based only on `uiAudit.passed`. When `uiAudit.passed=false`, a UI audit error, or Stylesheet/Link loading failure occurs, fix it first, do not expand the page, and do not claim completion.
