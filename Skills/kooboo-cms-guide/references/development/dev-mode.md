# Development Mode - Multi-Resource Workspace

## Page Purpose

- Admin page: `/dev-mode`.
- Development Mode brings pages, views, layouts, code, styles, scripts, menus, forms, search, debugging, modules, and other current-site resources into one multi-tab workspace.
- It is not a new site-resource type. Each activity panel still reads and writes the original Page, View, Layout, Code, Style, Script, Menu, Form, or Module object and is subject to the corresponding permissions.
- `activity`, `moduleId`, and preview-file parameters in the route only select an activity area or open a resource; do not infer resource content, runtime results, or trusted knowledge from them.

## Activity Panels and Tabs

- The left activity area shows available resource categories according to the current user's permissions. A missing panel may be caused by permissions or current site state and does not mean the resource type does not exist.
- Clicking a resource opens a tab in the workspace. Multiple tabs can exist simultaneously; the active tab determines the editing area and available actions.
- A change marker on a tab means the current frontend model changed, not that it has been written to the site. Saving belongs to the specific resource panel; there is no unified transaction that automatically saves every tab.
- Refreshing a tab rereads the current resource and may lose unsaved local changes. Prompts when closing, closing in bulk, or leaving Development Mode are not backup or version-recovery mechanisms.
- Switching sites clears the current Development Mode tabs; resources and unsaved states from different sites must not be mixed.

## Resource Editing Boundaries

- Pages, views, layouts, code, styles, scripts, menus, and forms retain the saving, preview, cache, reference, and deletion boundaries of their own list pages and dedicated editors.
- Saving a shared View, Layout, Script, Style, Code, or Menu in Development Mode can still affect multiple pages or callers; the multi-tab interface does not reduce the scope of shared-resource impact.
- A page-designer type opens a dedicated visual editor and is not necessarily edited in an ordinary code tab.
- Embedded styles and scripts belong to their host object; external resources are independently referenceable files. Their presence in the resource tree does not make embedded resources independent routes.
- The module activity area edits files in the module's independent directory, not ordinary site Page/View/Layout objects; module publication, settings, import overwrite, and dynamic menus still follow the module's own boundaries.

## Code References and AI Chat

- Some open tabs can add current editor content to AI Chat as a code reference. The reference captures the editor content at that moment and may include unsaved changes.
- Add code reference provides context only to the current conversation; it does not save files, execute code, publish the site, or automatically approve a write operation.
- Code resources provide this entry only for APIs and CodeBlocks. Styles and scripts provide it only for independent external resources; embedded resources are not referenced as independent files.
- An AI answer should combine the current reference with its resource type and must not generalize one tab's code into the final runtime state of the whole site.

## Search, Preview, and Debugging

- Search in Development Mode is static code search, not a complete dependency graph, and cannot prove that code has executed.
- Preview in Page, View, Script, Style, Code, and other panels opens only the corresponding preview or resource address. Returning content does not mean references, permissions, cache, interactions, and downstream business are correct.
- The debug panel is controlled by code-debug permissions. Refreshing or opening a debug target does not mean an exception has been reproduced, and a debug tab must not be confused with ordinary resource saving.

## Completion and Acceptance Boundaries

- A successful save of one tab proves only that its resource was persisted; it does not prove other open tabs were saved or that the site was published.
- Development Mode has no cross-resource atomic commit. When multiple related resources are changed in sequence, some saves can succeed and others fail, leaving an intermediate state.
- After changes, confirm each tab has no unsaved marker, then verify with real pages, APIs, resource URLs, devices, languages, and external business results; a workspace without errors is not enough.
