# App implementation without planning artifacts

Before writing business Screens, read [Images and uploads](../../Frontend/images-and-files.md) for content imagery, editable image uploads and persisted-file cleanup; this adds no planning step.

Use this path when current discovery confirms that both Blueprint and App Plan are absent and the user's current goal is clear enough to implement. A user request for P1 Fast Mode or direct delivery selects this path: do not create Blueprint or App Plan artifacts. It supports a new App and further capabilities in an existing unplanned App. If the user wants discussion and an overall proposal first, use the planned path instead. If planning exists or cannot be read, use the existing planning path and resolve its state.

## Scope and memory

Before selecting scope or writing resources, read the [SiteMilestone contract](site-milestones.md) for item structure and how progress is recorded.

- Read `get_site_info`, relevant Planning Topics and the site memory index, then only the notes and source needed now. Reuse established design, authentication, routing and data conventions.
- Describe each unplanned capability in concise business terms. Example: “Product management: list, search, create, edit and deactivate products. Excludes stock movements.” Do not add a structured acceptance plan or copy page/API designs into the milestone.
- Planning Topics own questions and answers; SiteMilestones own scope and progress; Todos contain temporary steps; source owns runtime configuration. Memory saves stable requirements, conventions, reasons and resource pointers that have no other authoritative home. Do not copy the checklist or implementation inventory into memory.

## Install and implement

Follow `references/Frontend/app-design-systems.md` for design-system selection and installation; use the selected package guide for component selection, Shell presets and updates.

1. Preserve a meaningful Site display name. If it is empty or temporary, wait until immediately before `create_app_base` to call `update_site_settings` and set only `DisplayName` to the agreed App name.
2. For a new client-rendered App, read the design-system catalog and select an exact built-in ID appropriate to current requirements, preserving an explicit user choice. Read the selected package and [App layout](../../Frontend/cdn-vue/app-layout.md). Determine `mock` or `kooboo` authentication from the current requirements; ask only for material unresolved choices. Explain a mock data strategy before using it and preserve any requirement for real persistence.
3. Follow [design-system consumption](../../Frontend/app-design-systems.md) and call `create_app_base` with direct configuration. It installs common infrastructure and a neutral home placeholder without a page inventory. Do not recreate an existing App Base.
4. Implement the current capability's actual API → service/store → components → Screen dependency closure. AI chooses the needed temporary steps and checks. Use the same client-rendered resource, interaction, authentication and design-system conventions as planned Apps. Save/read back dependencies before importing them. Add business routes and navigation as Screens become implemented; preserve existing routes. Replace the neutral home placeholder with the first implemented entry. Never modify the shared placeholder to impersonate multiple Screens.
5. Default new Apps to mock unless real persistence is requested; preserve existing strategies. A mock API keeps fixture reads in the API and returns success signals for writes without claiming persistence; do not create business tables for preview. With `kooboo` authentication, the FoundationTables from DatabaseDefinitions are still required before first sign-in. Before real use, follow [database preparation and mock transition](../../Backend/app-base-services.md#database-preparation-and-mock-transition), consume returned DatabaseDefinitions and initialize missing tables with SQLite Functions. Keep frontend HTTP contracts stable when replacing mock internals. Source configuration remains authoritative after installation; record durable decisions and pointers in memory.

## Verify and complete

Read back changed source and verify the intended behavior using suitable checks. Run focused Preflight after a coherent batch and capability Preflight before completion; see [App Preflight](../../Preflight/app.md). The adapter selects the shared `site` structural profile for this scan. Do not supply or invent a Plan version. This scan profile does not route the task to Site Building.

Resolve failed writes and actual check failures. Collect the runtime evidence requested by the scan, including the existing read-only Preview diagnostics where applicable. Browser operations remain opt-in. Structural checks and general diagnostics do not prove business completion: assess the implemented result against `content`, report what was checked and any untested behavior, and keep unmet scope incomplete. No structured business acceptance artifact is required.

Follow the [SiteMilestone contract](site-milestones.md) to save the checklist progress for the authorized batch; saving it is a progress record and does not gate closeout.
