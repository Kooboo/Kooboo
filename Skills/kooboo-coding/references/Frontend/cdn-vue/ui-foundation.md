# UI Dependencies and Style Technical Guide

The client-rendered Vue guide does not define a required visual style. Colors, fonts, density, radii, shadows, layout, and motion follow user requirements, the existing Site design system, or upstream visual design.

This file constrains only how those designs become CDN, ESM, and Kooboo Style objects that the browser can load directly and safely.

## UI dependency compatibility

UI framework, icon, font, chart, and other dependencies must be recorded and verified before writing the Page:

```text
name
version
loadMode              # script | css | esm | import-map
cdnAssets[]
importSpecifier
installMethod
vueCompatibility
componentCoverage[]
themeAdapter
smokeTest.status      # pending | passed | failed
smokeTest.pageUrl
smokeTest.assertions[]
```

- Vue uses the official CDN channel specified by the main guide, and Tailwind uses the Kooboo-hosted v3 CDN; record a clear, verified version strategy for other dependencies;
- `importSpecifier` matches the actual import in the import map and `main.js`;
- `installMethod` uses the current version's official usage; do not guess export names;
- A Vue UI framework must reuse the Vue in the import map and must not load Vue again through its own bundle;
- `componentCoverage[]` records only components truly needed by the current page;
- `themeAdapter` explains how Kooboo CSS Variables map to the framework's public variables, or is `none` when no framework is used;
- Begin business-component implementation only after the minimum smoke test passes.

An approved visual design does not mean that a suggested npm package can run directly through CDN/ESM. When a dependency is incompatible, adjust the implementation or choose an equivalent dependency; do not temporarily mix another loading method in a component.

## Dependency decision principles

Dependency minimization is not an independent quality goal. Compare the total cost of a mature library with a custom implementation, including implementation time, interaction completeness, responsiveness, accessibility, edge states, maintenance, bundle size, and runtime reliability. When a compatible, clearly licensed, exact-version specialist library materially reduces complexity or improves professional quality, the AI should introduce it proactively instead of choosing an obviously weaker implementation merely to claim “pure CSS” or “zero dependencies.”

- Use a mature chart library by default when a visualization needs axes, Tooltips, legends, zooming, responsive behavior, dynamic data, or multiple series. Reserve pure CSS for progress indicators, decorative mini-bars, or simple graphics that do not carry precise data-reading requirements.
- Prefer evaluating mature specialist libraries for date/time pickers, rich-text or code editing, drag-and-drop sorting, virtual lists, complex tables, maps, file previews, and orchestrated motion rather than rebuilding common infrastructure.
- Continue using native HTML/CSS/JS for simple, static, tightly bounded behavior such as one accordion, basic Tabs, short transitions, and a few state changes.
- A design system's ban on Tailwind, Bootstrap, or another CSS framework does not imply a ban on charting, editing, date, or other capability libraries. Such libraries need only pass compatibility, licensing, version, and theme-adapter checks.
- The AI may make and record ordinary implementation-level dependency choices without pausing to ask whether a library is allowed. Ask the user only when the dependency materially increases weight, introduces paid or restrictive licensing, changes the technical route, sends data to a third-party service, or creates a long-term platform constraint.
- Choose the smallest capability surface that meets the need: prefer an existing dependency, then a modular specialist library, and do not stack multiple libraries with overlapping responsibilities.

Never cite “one fewer dependency” as sufficient reason to sacrifice readability, interaction quality, accessibility, or delivery efficiency. When hand-building a non-trivial capability already covered by mature libraries, state a concrete reason why the custom implementation is better; “avoiding a dependency” alone is not a reason.

## Choosing a UI framework

A UI framework is not mandatory. Decide in this order:

1. The Site already has a framework: reuse the same version to avoid duplicate CSS and component semantics;
2. The user or design guidance specifies one: use it after verifying CDN/ESM and Vue compatibility;
3. Native HTML + CSS is sufficient: record `framework = none`;
4. A new framework has clear value: choose only one and state why it is being introduced.

Do not introduce a large framework with a complex loading chain for only a few Buttons, Inputs, or Cards.

## Icon libraries

Select an icon library whenever the App has functional icon slots in navigation, compact actions, search/filter controls, disclosure, pagination, row actions, status, or feedback. Banning Emoji, Unicode symbols, and hand-drawn SVG constrains the source; it is not a reason to remove icons:

- Prefer an icon library already used by the Site;
- Reuse the UI framework's official icon package when it meets the need;
- Otherwise choose an open-source icon library with a clear license and an exact-version CDN/ESM loading path; record its name, version, loading method, and license;
- Keep one primary icon library per application and do not mix in a second one for a few icons;
- Do not replace action, navigation, or status icons with Emoji;
- Do not use Unicode text symbols such as `★`, `→`, `×`, or `⋮` as fake icons, and do not remove a package-required icon slot or degrade it to text only because those substitutes are forbidden;
- Before writing UI, list the current Screen's semantic icon inventory. If the library lacks a glyph, choose a semantic equivalent from the same library or replace the library;
- Provide `aria-label` for icon-only buttons and `aria-hidden="true"` for decorative icons.

Record the name, version, asset URL, loading method, usage method, and smoke-test result.

## Minimum smoke test

Verify at least:

1. The import map resolves Vue and other ESM;
2. The page contains only one Vue runtime;
3. `createApp`, plugin installation, and `mount` succeed;
4. The minimum Button, Input, Dialog, and icon actually needed can render and interact, with no functional icon slot required by design evidence left empty or replaced by text only;
5. CSS, fonts, modules, icon sprites, and other subresources have no CORS or 4xx/5xx failures;
6. The console has no version incompatibility, duplicate Vue, component-resolution, or hydration warnings.

When the smoke test fails, retain the reason and return to dependency selection; do not rewrite `failed` as usable.

## `kooboo-var.css`

Create `kooboo-var.css` when DesignPanel must manage shared design variables. The file contains only:

- One `:root { ... }` block;
- `--<name>: <value>;` CSS custom properties;
- `/* @group ... */` grouping comments;
- End-of-line display-name comments after variable declarations.

Do not include:

- Ordinary CSS properties or additional selectors;
- Reset, layout, and component styles;
- Theme selectors;
- At-rules such as `@import`, `@media`, `@layer`, `@font-face`, and `@keyframes`.

Follow the format in [Design Variable Extraction](../../Scenario/design-variable-extraction.md). When there is no need for manageable variables, do not create an empty file and remove the corresponding `<link>` from the Page.

## `app.css`

`app.css` contains ordinary CSS:

- Reset and base rules;
- UI framework public-variable mappings;
- Theme selectors;
- Page and component styles;
- Responsive rules;
- Fonts, animation, and print styles.

Technical requirements:

- Load through `<link rel="stylesheet">` and do not chain site Styles with CSS `@import`;
- Referenced CSS Variables must actually exist;
- Framework variable names come from the official API for the corresponding version;
- Page, component classes, and Style object names remain consistent;
- Include `[v-cloak] { display: none; }` or an equivalent rule;
- Include visible, component-aware focus styles. Do not use a universal selector, a bare global `:focus-visible`, or a broad list of element-type selectors such as `button:focus-visible, a:focus-visible, input:focus-visible, select:focus-visible, textarea:focus-visible` to apply one outline treatment indiscriminately. Rewriting a global rule as an element list does not make it component-aware;
- Standalone controls may draw their own `:focus-visible` indicator through component-specific classes or selectors whose visual boundary is the focused element itself. Composite controls such as icon + input search boxes, input groups, date fields, and custom select triggers must draw exactly one focus indicator on the actual visual wrapper through `:has(:focus-visible)` or `:focus-within`, matching the wrapper's radius and bounds. Do not render both a wrapper ring and an inner-control outline. Remove the inner control's outline only when that wrapper provides a visible replacement. When CSS may be scanned separately from its HTML, qualify the inner removal selector with the same wrapper identity, such as `.search-box .search-input:focus-visible`, so the compensation relationship is statically verifiable;
- A composite control fails focus styling when an outline cuts through a prefix icon, suffix action, divider, or grouped field; when nested focus rings are visible; or when the ring follows an internal input rectangle instead of the complete interactive surface. Before completion, verify keyboard focus on each composite control at the rendered component level, not only by searching the CSS for `:focus-visible`;
- Non-essential motion follows `prefers-reduced-motion`;
- Startup, loading, error, and empty states have the necessary base styles.

For example, a search box should treat the wrapper as the visual control:

```css
/* Avoid: a broad element list still cuts through composite controls. */
button:focus-visible,
a:focus-visible,
input:focus-visible,
select:focus-visible,
textarea:focus-visible {
  outline: 2px solid var(--primary-6);
  outline-offset: 2px;
}

/* Prefer: one focus ring follows the search box boundary. */
.search-box:has(.search-input:focus-visible) {
  border-color: var(--primary-6);
  box-shadow: 0 0 0 2px color-mix(in srgb, var(--primary-6) 24%, transparent);
}

.search-box .search-input:focus-visible {
  outline: none;
}
```

## Tailwind v3 CDN

Tailwind always uses the Kooboo-hosted v3 CDN:

```text
https://www.kooboo.com/tailwindcss.js
```

Configure custom themes with `window.tailwind.config` in a separate `<script>` after the Tailwind CDN `<script>`. This order is mandatory because configuration written before the CDN does not take effect. Complete class names in JavaScript conditional branches must appear statically in the source:

```javascript
const classesByState = {
  idle: 'control control--idle',
  active: 'control control--active'
}
```

Do not concatenate incomplete class names at runtime:

```javascript
const className = `${prefix}-${state}-${level}`
```

The current project decides the visual responsibilities of Tailwind, ordinary CSS, and the UI framework; this guide requires only that loading, versions, and class names be handled reliably by the browser.
