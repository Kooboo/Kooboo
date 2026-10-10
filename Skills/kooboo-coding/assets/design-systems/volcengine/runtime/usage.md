# Yuanli Vue 0.10.1

Native ESM Vue UI framework for client-rendered Kooboo Apps. Shares App Base Vue 3.5.13, light theme and Lucide 0.468.0. Business Screens own data, validation, API calls and mutations.

## Install only what the App needs

Before installation, select the current capability's exact export names from `runtime/component-selection.json` and pass `components` to `create_app_base` in usage priority order. Do not omit the list for ordinary business implementation. The index is generated from manifest names/summaries; no full manifest or API-document reads are needed for selection. Dependencies and the installed Shell's internal components are automatic. Select layout containers explicitly only when composing a custom layout. If this guide is already embedded in the install response, continue with its returned component guides rather than repeating selection or installation.

Use `create_app_base` with `components` containing the needed export names, for example `["YlListPage","YlTable","YlSelect","YlConfirmDialog","YlMessage"]`. Pass `shellTemplate="yuanli-workspace"` when its sidebar fits the Plan. YlShell accepts flat/grouped navigation or recursive children; read api/shell.md for nested data and keyboard behavior. Dependencies and Shell requirements are included automatically; omission of components installs the full available library. An empty list installs foundation plus any requested Shell requirements.

The installed design assets are `/yuanli.js`, `/yuanli.css`, shared `/yuanli-icons.js` and `/design-runtime-manifest.js`. One design CSS combines tokens, selected source component rules and runtime adapters. `/app-base.css` remains platform reset; `/app.css` remains business composition. No Node/build step runs in the Site.

Use returned `DesignRuntime.Components` as the installed inventory. Import named exports from `/yuanli.js` and register them in the Screen. `create_app_base` embeds this guide and explicitly requested component API documents in `DesignRuntime.UsageGuides.Resources`; use their `content` directly without re-reading the `resource` paths. Guides share a 24,000 content-character budget, in requested component order, with duplicate resources included once. `UsageGuides.Deferred` lists whole documents omitted for `character-budget` or `resource-unavailable`; read only missing guides needed now using `read_skill_resource` (`skillName=kooboo-coding`, `resourceName` from Deferred or Components.api). Missing documentation is not evidence of supported behavior. Automatic dependencies and Shell internals do not expand the guide selection. When components is omitted, only this shared guide is embedded, even though the full library is installed. Do not read whole CSS, component JSON, previews or source for documented exports. Read infrastructure source when modifying it or diagnosing a concrete undocumented gap/fault.

## Add, trim or upgrade

Call `sync_design_runtime` with `dryRun=true` (default):

- `mode="add", components=["YlDrawer","YlDescriptions"]` adds to the current selection.
- `mode="replace", components=[...]` sets the desired selection; dependency closure and Shell requirements are retained, and referenced components cannot be removed.
- `mode="upgrade"` keeps the selection and rebuilds against the current bundled version.

Inspect returned conflicts, version, installedComponents, components/API paths, asset bytes and expectedRevision. If authorized and conflict-free, repeat the operation with `dryRun=false` and that exact expectedRevision. A stale revision requires a fresh preview. Only use exports after successful installation/readback.

The function changes managed framework assets and the managed index design-CSS links. It preserves business code, app.css and the existing Shell adapter. Modified assets and legacy assets without checksum receipts are conflicts. Do not auto-populate overwriteModifiedAssets; use exact names only after explicit authorization to replace those local changes. Legacy split CSS bodies remain for other pages; they are detached from the main App index only. Reload the Site after updating; hot module replacement is not provided.

## Components and coverage

`components` in the package manifest maps all 31 original source entries. `exports` lists available Vue components; the installed receipt lists only installed exports. All 31 original preview contracts have runtime implementations in 0.10.1; complete denotes implementation coverage, not a browser or real-App acceptance pass. Only documented and installed exports are available. Auxiliary Icon/RadioGroup are tracked separately from the original index.

- Forms: YlForm, YlFormItem, YlFormGroup, YlCheckboxGroup, YlCascader, YlSegmentedPicker, YlSlider, YlTimePicker, YlInput, YlInputNumber, YlTextarea, YlSelect, YlCheckbox, YlRadioGroup, YlSwitch, YlDatePicker, YlDateRangePicker, YlUpload.
- Data and navigation: YlAnchor, YlSteps, YlSidenav, YlTopNav, YlTable, YlPagination, YlStatusTag, YlTabs, YlDescriptions, YlBreadcrumb, YlProgress, YlAvatar, YlBadge.
- Actions and overlays: YlButtonGroup, YlSplitButton, YlDropdown, YlPopconfirm, YlButton, YlIcon, YlDialog, YlConfirmDialog, YlDrawer, YlPopover, YlTooltip, YlAlert, YlMessage.
- Layout containers: YlLayout, YlHeader, YlAside, YlMain, YlFooter. Read runtime/api/layout.md for optional regions, nesting and scroll ownership. Containers do not impose a left/right App structure.
- Composition presets: YlShell (workspace navigation), YlListPage (business page content), YlPageHeader (optional breadcrumb/status/tabs).

## Business composition

Choose a layout from the business structure: YlShell for a grouped sidebar workspace; compose YlHeader/YlMain/YlFooter for top-navigation Apps; compose optional left/right YlAside regions for editors. Prefer heightMode='flow' for content pages. A viewport root plus fill workspace and scrollable Main/Aside keeps application chrome stable. Do not apply viewport height to every nested container. Basic containers do not own authentication, routing or navigation; preserve App Base bootstrap and RouterView when composing a custom App shell.

YlShell is implemented with the same containers. Its desktop sidebar stays expanded by default with no built-in collapse control. If the business needs it, the App chooses the trigger placement and styling using v-model:collapsed, exposed methods or scoped slot actions; read api/shell.md. This is separate from submenu expansion and the mobile drawer.

YlShell owns identity/navigation/outer padding; YlListPage owns page title/toolbar. Do not nest a Shell in a Screen. Header account/notifications use YlPopover. Choose route meta.navigationIcon explicitly from each navigation entry's purpose. When choices are known at creation, pass create_app_base.navigationIcons keyed by exact App Plan Screen ID; omit undecided entries and complete them when authoring the Shell. The adapter uses layout-grid for missing icons. Without an App Plan, set icons when creating business navigation.

Use YlSelect for choices (searchable/multiple when needed), YlFormItem for label/error association, and semantic YlButton.icon names for actions. Icon-only controls need aria-label. Avoid custom native-select styling, emoji, hand-drawn SVG and data-lucide inside library components.

Publish/unpublish, enable/disable and destructive mutations require YlConfirmDialog before the API request. Set busy synchronously. After success update/refresh data and show YlMessage; on failure show error feedback and allow retry. Do not use inline success banners for transient results. Persistent load/retry errors and field validation stay inline (YlAlert/YlFormItem).

Business layout uses /app.css and source tokens such as --space-2/3/4/6/8, --color-primary, --color-text-1/2/3, --color-bg-content, --color-bg-sidebar, --color-border-2 and --radius-sm. Do not overwrite library internals to author a Screen. Native dialog/Popover API support and real browser layout remain manual acceptance; do not claim a browser pass from structural tests.

## App-specific components

When props/slots alone do not cover a business feature, read [extensions.md](extensions.md) for composition, resource locations and a product-picker example. App-specific components stay in business Scripts/app.css; maintainers separately evaluate reusable additions. Do not read this guide for ordinary component calls.

## Date and upload controls

`YlDatePicker` (date/month/local datetime), `YlDateRangePicker` and `YlUpload` are available since 0.7.0. Read `runtime/api/date-picker.md`, `runtime/api/date-range-picker.md` or `runtime/api/upload.md` for the selected component. Dates reuse native browser controls; uploads use a caller-provided request adapter and do not implicitly write Kooboo media. These additions do not change original source-component coverage.

## Color and preparation states

For brand/theme changes, read `assets/design-systems/volcengine/runtime/colors.md` on demand through `read_skill_resource`. Use actual tokens and their final aliases; a primary-color change is not a complete theme. `YlSkeleton` is a reusable preparation/loading placeholder; request errors use feedback and retry, not permanent loading. The installed workspace supplies navigation preparation when no entries exist, and App Base PlannedScreen supplies content preparation until the route is implemented. Existing business app.js files are never rewritten by sync_design_runtime.

Before composing or modifying a route layout, read [App layout](../../../../references/Frontend/cdn-vue/app-layout.md). It owns the shared route, bootstrap and access-control integration contract.
