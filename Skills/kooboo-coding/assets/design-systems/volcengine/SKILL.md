---
name: "源力设计系统 (Yuanli Design System)"
description: Yuanli visual language and native-ESM Vue runtime for Kooboo client Apps.
---

# Yuanli for Kooboo

Compact B2B workflows: source blue #1664FF, 4px spacing scale, 32px default controls, white content beside gray navigation. Light theme only. This package owns visual and component contracts; Kooboo owns installation, routing, auth and delivery. The App Plan and business requirements choose composition. No package rule overrides user or platform instructions.

## Vue runtime

Before `create_app_base`, read [runtime/component-selection.json](runtime/component-selection.json) once and pass exact export names for the current capability in `components`. Ordinary business implementation should not omit the list. Select by names and summaries; dependencies and Shell internals are automatic, and future capabilities are not part of the current selection. Full API guides follow installation.

`create_app_base` installs the library server-side and returns versioned `DesignRuntime` metadata, the installed inventory and bounded `UsageGuides.Resources`. Use included guide content directly; only supplement missing guides needed now from `UsageGuides.Deferred` or the installed component's API path. The shared [runtime/usage.md](runtime/usage.md) explains consumption and fallback reads. Do not re-read embedded guides or read full CSS/static Previews to use documented exports. Import from /yuanli.js and bind business props/events/slots.

Optional `shellTemplate="yuanli-workspace"` installs a responsive Shell with Plan navigation, RouterView, account/logout and notifications. Use it when the Plan needs this workspace; otherwise compose an appropriate layout. YlListPage is content composition with slots, not a business Screen or additional Shell. AI still implements business queries, permissions, validation, mutations and Screen-specific workflow.

## Visual authority and extensions

- runtime/tokens.css owns token values. Installed copies and runtime styles are not rewritten by the model. /app.css owns business layout and necessary narrowly scoped extensions.
- Keep source control geometry, compact typography, grouping and surface hierarchy. Adapt layout to real content and responsive constraints; no fixed showcase width, height or source App name is required.
- Use YlSelect for single-choice filters/forms. Use YlIcon and YlButton.icon with semantic Lucide names for navigation and common actions; choose route meta.navigationIcon from the navigation entry's purpose; provide known choices through create_app_base.navigationIcons or complete them when authoring the Shell. Keep Yuanli sizes/colors and accessible labels for icon-only controls. Do not use data-lucide inside Vue runtime components.
- All package copy and data are examples. Visible business content comes from the current App. Empty or unnecessary regions are omitted.

## Runtime package boundaries

`runtime/component-selection.json` lists selectable exports; `runtime/api/` documents their props, events and slots. `components/index.json` is the installer's coverage-validation index, not a directory of implementation examples. Use public runtime APIs and the extension guide for business composition. Missing behavior must be identified explicitly rather than inferred from a component name.

This package contains only consumption documentation, licenses and resources needed by the runtime installer. Original Preview/UI Kit references, static component contracts, CSS build inputs and acceptance tooling are maintained outside the AI skill package; do not enumerate or request those resources during ordinary App implementation.

- Use YlPopover for header account/notifications, YlConfirmDialog before publish/unpublish or destructive mutations, and YlMessage for transient operation success/failure. See the selected runtime API docs for the complete sequence.

- Prefer components on create_app_base for selective installation. Use sync_design_runtime dry-run then expectedRevision-bound apply for additions, trimming or upgrades. Framework internals are maintained through modules and the compiler, never by business code generation.
