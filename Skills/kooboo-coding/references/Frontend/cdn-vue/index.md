# Kooboo Client-Rendered Vue Development Guide

> Naming boundary: the directory name exists only for compatibility with existing technical routing. For ordinary users, consistently say “client-rendered”; do not expose the internal directory name, Vue version, import map, or Page/Style/Script object composition in replies, confirmation questions, milestone names, or next-step recommendations. Explain implementation details only when the user asks about them.

Build Vue 3 CSR interfaces that run directly in the browser from Kooboo Page, Style, and Script objects. Keep business code as standard HTML, CSS, and JavaScript ES Modules; do not depend on `.vue`, Vite, Webpack, Node build output, or a virtual file system.

This guide defines how client-rendered Vue is implemented technically in Kooboo, not a particular product workflow. It can be used for production business interfaces, admin panels, dashboards, internal tools, existing-page upgrades, or interaction prototypes.

Upstream artifacts such as AppBlueprint, Page Plan, visual designs, and API contracts may be inputs, but none is a prerequisite for entering this directory. Upstream business processes continue to decide “what to build”; this directory decides only “how to write Vue in Kooboo.”

## Use cases

- The page is primarily client-interactive and does not depend on search engines crawling complete business content;
- You want Vue reactivity, components, computed values, and lifecycle hooks;
- Code must be saved directly as Kooboo Page, Style, and Script objects;
- You do not need `.vue` SFCs, npm plugin compilation, TypeScript compilation, or bundler transformations.

Prefer another approach in these cases:

- Public content, SEO, and server-rendered first paint are priorities: use Kooboo SSR / View / KScript;
- Only a small amount of interaction is needed: use native JavaScript instead of introducing full Vue;
- SFCs, a complex npm ecosystem, compile-time plugins, or code splitting are required: use a formal build output and deploy it as static assets;
- The task is primarily about API, Database, Permission, or authentication: also read the relevant Backend / KScript / scenario guidance; this directory does not replace backend design.

## Kooboo object model

Use by default:

- One complete HTML Page without a Layout;
- One variable Style: `kooboo-var.css`, or the `/design-tokens.css` installed by `create_app_base`;
- One regular Style: `app.css`;
- One startup Script: `main.js`;
- One root-component Script: `app.js`;
- Store, service, and component Scripts split by responsibility.

For a new client-rendered App with an authorized delivery scope and a selected built-in design system, call `create_app_base` once instead of recreating the Page, token Style, base Style, Vue/Vue Router runtime, hash router, startup/root Scripts, `/_Admin/login` integration, current-user bootstrap, or minimal in-App notification infrastructure. With planning, use `settings.designSystem`; without planning, use direct configuration from the lightweight App path. Treat the returned base objects as the starting point and add only business-specific resources.

Kooboo objects are not disk files. Keep Script Names flat and route each Script at `/<Name>` so its Name and URL stay directly aligned:

| Type | Name | URL |
|---|---|---|
| Page | `index` | `/` |
| Style | `app.css` | `/app.css` |
| Script | `store.task.js` | `/store.task.js` |
| Script | `screen.task-list.js` | `/screen.task-list.js` |

For Scripts, derive the URL by prefixing the exact Name with `/`; do not translate dot groups into virtual folders. Page and API URLs remain semantic business routes. See [overview.md](overview.md) for the complete rules.

Before creating business Scripts, read [project-structure.md](project-structure.md) for the standard structure and naming.

## Default technical baseline

| Layer | Default choice |
|---|---|
| Vue | Vue 3 full build, loaded from the unpkg ESM URL recommended by the official documentation through an import map |
| Tailwind | Tailwind v3, using the Kooboo-hosted CDN |
| Components | Plain JavaScript objects with `setup()` + static `template` |
| Modules | Native `.js` ES Modules with site-root absolute URLs |
| State | Component `ref/reactive` or a module-level store |
| Data | Static data, localStorage, or an existing HTTP API |
| UI framework | Optional; Vue version, CDN/ESM loading, and installation must be verified |
| Icons | One verified icon library selected as needed |

In production, switch the Vue URL to `vue.esm-browser.prod.js` on the same official Vue 3 channel. Keep the Vue 3 full build, Kooboo Tailwind v3, and the loading form required by this guide; do not switch to a runtime-only build.

## Core hard constraints

- Page is a complete, independently runnable HTML document; do not write Layout, View, placeholder, `env="server"`, or other server-template directives.
- Style and Script use real Kooboo objects; do not generate project files such as `package.json`, `src/`, `.vue`, or build configuration that cannot be saved directly.
- Vue components directly export plain objects and do not use `defineComponent()`; templates are static inside the module.
- Before writing App-owned components, read the [component contract and preview source metadata](components.md#component-contract).
- Do not load templates remotely, concatenate templates at runtime, or use `v-html` to render untrusted content.
- Use root-absolute URLs for site CSS, Scripts, and imports, and retain the `.js` extension.
- Use the official CDN channel specified here for Vue; use the Kooboo-hosted v3 CDN for Tailwind. Decide whether to pin other third-party dependencies according to project compatibility. UI frameworks must reuse the same Vue and must not load a second Vue runtime.
- Check same-type Names and site-wide Routes before creating or updating; do not hide conflicts with random suffixes.
- Store only shared state in the store; keep component-local drafts, dialogs, and temporary inputs in the component.
- Create a service only when a real HTTP call exists; do not create an empty service or a placeholder module for possible future use.
- Treat the App shell as a singleton. Before adding navigation or branding, inspect the current root component and preserve its existing shell ownership. In one viewport, render the current App logo/name in only one identity region; do not nest another complete TopNav, Sidenav, brand block, or app shell inside the base shell. A second identity region is allowed only when it represents a genuinely different entity, such as the platform and a selected workspace, and the labels make that hierarchy explicit.
- Hiding a frontend button is not access control; security, authentication, and authorization must be implemented server-side.
- Read back key objects after writing, run the applicable Preflight, and verify network requests, the console, and interactions through the actual Page URL.

## Recommended implementation order

1. Confirm page responsibilities, existing objects, data sources, and runtime boundaries;
2. Design the Page / Style / Script object list and dependency direction;
3. Verify external dependencies such as Vue, the UI framework, icons, and fonts;
4. Create in the order “Style → store/service → leaf components → composite components → app.js → main.js → Page”;
5. When updating an existing app, modify in the order “leaf → direct consumers → root entry”;
6. Read back objects, run Preflight, and validate through the actual Page URL.

See [delivery.md](delivery.md) for detailed steps.

## Documentation navigation

| Document | Responsibility |
|---|---|
| [overview.md](overview.md) | Browser architecture, Name/URL, complete HTML entry, navigation, and import rules |
| [project-structure.md](project-structure.md) | Frontend structure, naming, responsibilities, and dependencies |
| [components.md](components.md) | Plain-object components, props/emits, lifecycle, and splitting rules |
| [ui-foundation.md](ui-foundation.md) | UI dependency compatibility, icons, and the Style technical contract |
| [delivery.md](delivery.md) | Discovery, creation, updates, readback, Preflight, and runtime acceptance |
| [quality.md](quality.md) | State, forms, accessibility, security, network, and final checks |

All App Base business HTTP calls must follow [request source binding](delivery.md): defineService defines the service and useServices binds it once in component setup; components must not call unbound methods.
