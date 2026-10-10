# ToB App Building entry

Route by the current change. Simple UI adjustments to an existing App follow the scoped repair path in `kooboo-coding`; do not enter planning discovery for spacing, alignment, colors, typography or display-only changes. Read business contracts when meaning, data, permissions, action behavior, routing or feature scope changes. Existing business changes use [requirement review](requirement-review.md).

App Building and public Site Building are distinct routes. Do not switch routes to bypass applicable authorization or verification.

## Discover and choose the path

For a new App Building conversation, call `get_site_info`, then choose the path from the user's requested starting point:

- **Business modeling:** read `read_app_blueprint({})`, reuse the semantic graph, and follow [Blueprint](blueprint.md). Do not load App Plan unless the user also authorizes implementation planning.
- **App delivery:** read `read_app_blueprint({})` and `read_app_plan({})`, then resume, update or establish the requested planning. New conversations do not justify recreating existing planning. Failed reads do not mean absence.
- If neither artifact exists but meaningful application resources do, continue existing-App maintenance by default; establish planning only when requested.

**P1 fast mode** directly implements the authorized current scope without creating Blueprint or App Plan; use [lightweight implementation](lightweight.md) when both are absent. **P2 detailed mode** forms a reviewable business proposal and implementation plan through the two planning stages below. Resume existing artifacts rather than recreating them or clearing them to enter fast mode.

Do not choose planning based on system category, domain labels, module count or familiar examples. If the user has already chosen fast/detailed delivery or requested planning, reuse that choice. Ask only when the preferred next action is unclear: “Build a usable version first, or discuss the business and implementation plan first?” Missing requirements trigger necessary clarification; complexity affects implementation scope and sequencing, not mandatory artifact creation.

For authorized implementation, follow the user's explicit scope. When unspecified, choose a usable core business workflow and its dependencies; an overall Plan does not automatically authorize all modules. Preserve later work. Planning-only requests stop after planning.

## Stage 1: Blueprint

Read [Blueprint authoring](blueprint.md) before semantic writes. Begin with business meaning: Blueprint → Actors/Goals/Modules → Scenarios → atomic Facts → derived Business Objects and rules. Modules are business units; Scenarios can belong to several Modules. Do not start from tables or duplicate scenarios to represent memberships.

Use the Native Functions and reviewed complete-object save boundaries defined there. Reuse stable IDs and confirmed authorization. A semantic-only request ends with the saved semantic scope. Enter App Plan when `read_app_blueprint` returns `canCreateAppPlan=true` and the user authorizes planning; readiness is not authorization or proof of complete business coverage.

## Stage 2: App Plan

Read [App Plan authoring](app-plan.md) before writing and [design-system selection](../../Frontend/app-design-systems.md) before selecting design. App Plan is the current server implementation design: Settings, SourceFiles, DataContracts, Services, Screens and DeliverySlices.

Load the relevant typed save Functions through `use_function`, save reviewed complete objects in dependency order, and read back with `read_app_plan({})`. The first successful save creates the root automatically. Do not persist planning through scripts or resource files. The App Plan reference owns schemas, batch semantics, deletion and recovery; do not duplicate its protocol here.

## Stage 3: Implementation

App Plan defines how business will be implemented. DeliverySlices group the selected business outcomes; execution status is separate. Planned App implementation does not require a Milestone binding. Use the selected slices, their Screens, Actions, DataContracts, Services and SourceFiles as the design baseline, and Todos for temporary execution steps. [SiteMilestones](site-milestones.md) remain a separate checklist where applicable, not a Plan prerequisite.

Read the [client implementation entry](../../Frontend/cdn-vue/index.md) and [delivery guide](../../Frontend/cdn-vue/delivery.md). Use `create_app_base` with saved Settings, then implement backend resources, frontend adapters and Screens in dependency order. App Base installation and route placeholders do not implement business capabilities. Read [API layering](../../Backend/api-layer-guide.md) before backend work. Data strategy is resolved for actual implementation; mock results never establish durable storage.

If the App display name is empty or temporary, call `update_site_settings` once immediately before `create_app_base` to set only DisplayName to the agreed name. Do not rename during planning or change Website.Name, domains or paths. Preserve meaningful names.

Read the current Plan when resuming, after planning changes, when code conflicts with design, or before reconciling the selected delivery scope. Existing business changes use [requirement review](requirement-review.md). Compare all selected capabilities with actual resources and evidence before completion. Follow [Preflight](../../Preflight/index.md) for applicable checks; planning save success is not acceptance evidence.

## Discussion and user-facing language

Call results a “business proposal” and an “implementation plan”; users need not know internal IDs or tool names. Technical implementation details are not additional confirmation questions. Preserve authorization for the unchanged reviewed scope, distinguish planning from implementation, and ask only for genuinely unresolved decisions.

Assumptions, open questions and unconfirmed answers belong to the site-level [planning topics](planning-topics.md), independently of Blueprint and App Plan. Read relevant topics before continuing discussion/planning/implementation, and record discussion using its current Functions. Topic confirmation alone does not authorize writes to planning or resources.
