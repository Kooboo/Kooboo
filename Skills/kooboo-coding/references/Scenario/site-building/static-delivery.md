# Static Delivery (S4–S5)

Read this document only after the Page Plan, site milestones, and Design System are complete.

## S4 Current-Batch Implementation

Create only the current `resolvedBrief.pages`. Do not keep generating pages outside the current batch merely because the final goal includes transactions, accounts, or payment. Establish new todos for the next batch and reuse `resolvedBrief`, replacing only `pages`.

“Static” means only that backend data, authentication, persistence, and business APIs are not connected yet. Interactions that can be completed within the DOM—mobile menus, tabs, accordions, galleries, static filters, hover/focus states, reveals, and similar behavior—must be completed in the first version.

### Site-Level Preparation

1. Before creating the first site resource, call `get_site_info` to check the current site name. Only when `DisplayName` is empty or is clearly a system temporary name such as `ai_site_260813103`, `untitled`, a pure number, or a timestamp, try calling `update_site_settings` to set `DisplayName` to `resolvedBrief.projectName`; preserve an existing meaningful name and do not modify the internal `Website.Name`, domain, or path. If reading or updating the name fails, record the limitation and continue current S4 resource creation; do not block site delivery.
2. Read [Frontend Creation Functions](../../Functions/frontend-create.md), [Code Style](../../Frontend/code-style.md), [Layout](../../Frontend/layout.md), and [Design Variables](../design-variable-extraction.md) as needed.
3. `create_layout`: have the Layout reference shared Header/Footer and declare `k-placeholder`.
4. `create_style`, and `create_script` when necessary: create global variables, styles, and shared static interactions.
5. Call `view-batch` once for Page Plan `sharedBlocks[]`, generating, creating, and reading back each shared View.

Shared Views are referenced only by the Layout and must not be placed in a Page placeholder again.

### Single-Page Pipeline

Prefer incremental delivery in Page Plan order, starting with the home or core page. This is generation guidance, not a prerequisite for individual object writes:

1. `page-item` obtains ordered `blockRefs[]` and `compositionGroups[]`;
2. Set the Page URL before `create_page`: independently addressable detail pages use a path template such as `/blog/{slug}` or `/product/{seoName}`; reserve query strings for list state and other non-identity parameters. The placeholder name must match the value exposed to the detail Page's `k-data`.
3. When an early preview helps, `create_page` may write a lightweight Layout-wrapped skeleton. If the Views already exist, mount them directly. Page owns ordinary HTML/CSS containers and any cross-View composition wrapper;
4. Read `read_preview_diagnostics(mode="issues")` when the preview is available. It checks computed styles, overflow, clipping, overlap, and Design System palette drift through `uiAudit.colorAudit`. Diagnose and repair the responsible Page/Layout/View/Style, then recheck. Unavailable diagnostics are recorded as unverified; they do not block creating resources. `uiAudit.passed=true` is generic runtime evidence, not proof of visual quality or task completion;
5. Call `view-batch` once to obtain the authoritative prompts for this page;
6. Compose the page using UUPM component suggestions; when whole-page composition guidance helps, call `read_skill_resource` with `{"skillName":"ui-ux-pro-max-pp","resourceName":"references/page-composition.md"}`. Execute each item independently using `baseContract + current item.prompt`: generate → `create_view` → `read_object_text` → Prompt Conformance Gate → `update_text_object` when necessary. As soon as all current-Page Views exist, immediately replace every empty `data-block` skeleton with the matching `<view id="...">` reference in Page Plan order while preserving any required composition wrapper; then read the Page back and confirm that no empty `data-block` remains. Complete this assembly before milestone completion or delivery claims. Run Preflight and refresh the real Page route only after this assembly readback.
7. After assembly, read the Page and 1–3 key Views for focused refinement. When rendered visual review is available and authorized, inspect the real Page route on desktop and mobile for focal hierarchy, media crop, headline wrapping, section handoff, and clipping. Repair the smallest responsible View/Page/CSS and recheck the affected result. If visual Preview is unavailable, report visual acceptance as unverified and continue other pages and capability Preflight; do not infer a visual pass from source or diagnostics.

By default, do not finish Views for every page before creating Pages, and do not accept the user's page on a View URL. Only when the user explicitly requests a single-file Page may the final block be inlined directly; even then, first obtain the corresponding `view-batch` prompt.

Object creation does not depend on the latest `page-item`, a skeleton receipt, or a preview diagnostic result. Shared Header/Footer Views can be created independently of Pages. Reading several page plans does not constrain which Page may be created next. Check persisted Page assembly, referenced resources, and actual runtime results separately; fix real issues before claiming completion. Preserve the existing capability Preflight and completion evidence requirements.

### Page Composition Gate

`compositionGroups[]` defines the information architecture for cross-View tasks only; it does not constrain the concrete design. AI may adjust column widths, gaps, containers, sticky behavior, boundaries, and visual treatment, but the Page must own the composition relationship between sibling Views:

- `product-gallery + product-summary`: on desktop, the product media main area is adjacent to the purchase summary;
- `cart-items + checkout-summary`: on desktop, the cart-item main area is adjacent to the amount/checkout sidebar;
- `checkout-form + checkout-summary`: on desktop, the checkout-form main area is adjacent to the order-review sidebar;
- `order-status + checkout-summary`: on desktop, the order-status main area is adjacent to the amount-review sidebar.

On mobile, stack using `stack-primary-first`. Unless the user explicitly requests another structure or content/device constraints provide sufficient reason, members of a matched group must not become two consecutive full-width desktop sections. Primary/aside placement constraints in the View prompt must be implemented: a View adapts within its assigned column, does not create a cross-sibling wrapper, and does not duplicate the sibling's primary responsibility.

After both member Views are complete, read the Page and confirm that they are inside the same ordinary HTML wrapper; desktop has a real grid/flex main area plus aside; the main area comes first on mobile; and purchase, amount, or review actions are not pushed below unrelated long content. If the result is still full-width sections stacked vertically on desktop, treat it as **FAIL** and modify the Page HTML/CSS first.

### Prompt Conformance Gate

Check each View:

- whether the View implements its UUPM component role and fits the page composition;
- whether required content is complete;
- whether avoid patterns are absent;
- for cards spanning columns, rows, or stretched by same-row elements in a grid/bento, whether the inner visible wrapper also fills the span; when content is insufficient, reduce the span or exit stretch instead of leaving an empty card interior;
- whether images, data, and interactions match real capabilities;
- whether fabricated phone numbers, addresses, prices, reviews, certifications, or submission behavior exist.

When any core item fails, fix it and read back again; an ordinary Preflight PASS cannot replace prompt compliance.

### Static Delivery Evidence

- Read [ImagePlaceholder](../../Functions/image-placeholder.md) as needed for image URLs; do not invent image-host URLs;
- Before creating a Page/Layout/View/Style/Script, read only the reference needed by the current object;
- Before delivery, read [Preflight](../../Preflight/index.md) and scan this batch's objects with `deliveryStage: "static-first"`;
- Checklist progress order: run the final capability Preflight for this batch, then call `update_site_milestones` with the complete checklist, marking only the verified items `completed=true`;
- When a capability Preflight returns a `site.ui-runtime` evidence requirement, against the final rendered page call `read_preview_diagnostics(mode="issues", verificationRequirementId="site.ui-runtime")`. Only `uiAudit.passed=true` and `actionableErrorCount=0` count as generic UI runtime regression evidence; this cannot replace task-specific visual acceptance or independently prove that a user-reported styling issue is fixed. Continue fixing on failure and do not end the task;
- Report the scope and result of rendered visual review separately; when unavailable, identify visual quality as unverified. Source readback, Prompt Conformance, `uiAudit`, and Preflight cannot independently prove the intended visual quality;
- Active `BLOCKER`s must be fixed; deferred rules wait for the corresponding dynamic stage.

## S5 Static Iteration

After receiving feedback, prefer local View changes, then adjust Page block order, and change Layout or global Style last. When the AI Chat Completion Ledger is connected, each round may explicitly use `mode: "focused"`; the adapter fills in dirty objects and the model supplies existing direct consumers. When this static batch is complete, one scan with `mode: "capability"` is required; the adapter authoritatively fills all successfully written objects in the session, and the model adds existing consumers that belong to this batch's capability but were not modified.

When the user has not requested dynamicization, the task may end after the static confirmation version. When a natural next step exists, use `show_next_recommended_action`; the recommendation itself is not authorization.

Calls to `search.js` for planning and component suggestions are independent of skeleton and preview diagnostics readiness. They may proceed while diagnostics are pending; handle preview problems through page repair and rechecking.
