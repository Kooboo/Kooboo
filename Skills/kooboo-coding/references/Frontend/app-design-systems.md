# Bundled App Runtime Design Systems

Use this reference to select and consume an executable Vue design system for ToB Apps. `assets/design-systems/catalog.json` is the selection authority. Every listed system must have a supported runtime manifest; static CSS/Preview packages are not installation options.

## Select one runtime

- Read available options from the catalog. Each system must provide a runtime, component API documentation and an installation contract.
- An explicit user choice takes precedence. Otherwise compare catalog `bestFor`, `signatureTraits` and `reuseDefaults` with business tasks, information density, interaction needs and responsive composition. Do not load every candidate package or default to one brand for all Apps.
- Without Blueprint/App Plan, pass the selected ID to `create_app_base` and save rationale/resource pointers in site memory. Do not create planning artifacts solely to select a design system.
- For planned Apps, reuse an explicit design choice; otherwise use `ask_user_input(kind="design-system")` with the recommended `defaultValue` and business rationale, and require `confirmed=true`. Save the exact catalog ID/name in `settings.designSystem` through `save_app_settings`; preserve the complete Settings when updating. Fast direct delivery follows the unplanned path above.
- Reuse the selected system across Screens. A saved retired ID must be explicitly reselected before new installation; never silently map it to another runtime or overwrite an existing site's library.
- Read [Frontend Design](frontend-design.md) for layout integrity, spatial rhythm, hierarchy and responsive containment. The runtime controls visual language; business requirements control page composition.

## Read progressively

During planning, use the catalog and current business evidence. Read only the chosen system's overview if necessary to resolve a concrete capability question. Do not read full manifests, CSS, component APIs or all examples just to rank systems. Keep planning free of HTML/CSS implementation content. App Plan SourceFiles may specify planned filenames and relative directories.

During implementation:

1. Read the selected package's `SKILL.md` and its lightweight `runtime/component-selection.json` when selecting component capabilities or discovering API names.
2. Set `create_app_base.components` according to the selected package guide; its effect on installed assets and API guides is package-specific. Do not assume all runtimes use the same strategy.
3. Use returned `DesignRuntime.UsageGuides.Resources` directly. Supplement only current, missing guides from `UsageGuides.Deferred` or `DesignRuntime.Components.api`, using `read_skill_resource`. A missing guide is not permission to invent props, events or slots.
4. Import exact named Vue exports from `DesignRuntime.ModuleUrl`. A service, directive or locale export is not a template component. Preserve each library's native API; do not transfer props between libraries or install another Vue runtime.
5. Read managed source only to diagnose a concrete fault or resolve an undocumented behavior. Full design CSS, static Previews and UIKit pages are not prerequisites for using documented runtime exports. Do not recursively enumerate maintenance resources.

## Shell and styles

- `create_app_base` installs infrastructure, selected runtime assets and route placeholders. Choose a preset from the selected package guide only when its workspace fits the business; otherwise compose a custom layout from installed components. Installation alone does not implement business Screens.
- Follow [App layout](cdn-vue/app-layout.md) for route ownership and infrastructure integration; read the actual layout component before modifying it.
- Load runtime and adapter styles in the order documented by the selected package, followed by business `/app.css`. Account for existing Shell padding and scroll containers rather than duplicating them in Screens.
- Runtime code owns component DOM, CSS, interaction and accessibility behavior. Use public props, events, slots and theme tokens. Do not reconstruct buttons, forms, tables, cards or navigation from compiled CSS, or turn all libraries into a uniform `.ds-*` API.
- `/app.css` owns business layout, spacing, width allocation, scrolling and narrowly scoped overrides for concrete contract gaps. Read existing business styles before editing. Never replace installer-managed JS/CSS or reset styles with model-generated implementations.
- Read the selected `runtime/extensions.md` for business composition and its package guides for themes and colors. Do not assume libraries share tokens or dark-theme support.
- Preview `sync_design_runtime` using a mode supported by the selected package, then apply with the preview’s exact `expectedRevision`. Respect modified-asset conflicts and preserve business code. Do not silently migrate retired static design systems.

## Business content and icons

A design package supplies visual and interaction contracts, never facts about the current App. Examples, names, metrics, identities, timestamps, sample rows and helper text are fixtures. Every visible label, title, aria-label, placeholder, tooltip, empty state and chart value must come from current business requirements, user input or API/service data. Mock fixtures must describe that business, not a library demo. Omit irrelevant regions and close the layout.

Choose navigation/action icons semantically. Use the package's documented icon adapter and the installed App Base Lucide runtime for business glyphs; preserve native upstream icons inside library controls. Do not substitute emoji, Unicode symbols, empty boxes or hand-drawn glyphs. Icon-only controls need accessible labels; decorative icons beside text must not duplicate the accessible name.

## Charts, motion and delivery

- Add charts only for a real business task. Prefer a verified mature engine for axes, legends, tooltips, multiple series, responsive updates and interaction. Apply the runtime's documented tokens/theme adapters; CSS bars are not a substitute for required chart capabilities.
- Reuse documented component motion and states. Without explicit guidance, keep added motion short and functional.
- Follow [client delivery](cdn-vue/delivery.md) for business resource implementation and delivery order.
- Check spacing, hierarchy, containment, state handling and working controls independently of token fidelity. Avoid duplicate Shells, inert account controls, unsupported overflow, decorative metrics and copied demo content.
- Source/structural checks, manual component acceptance and real business-App acceptance are separate evidence. Do not initiate browser operations without explicit user authorization. Untested visual/runtime behavior remains unverified.
- Preserve bundled licenses and attribution. Catalog inclusion is an implementation contract, not proof of browser acceptance.
