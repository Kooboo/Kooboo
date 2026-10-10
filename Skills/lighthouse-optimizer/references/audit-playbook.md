# Lighthouse Audit Mapping Playbook

Treat the Audit ID as stable evidence. Because audit names may change between Lighthouse versions, match both the complete ID and clearly meaningful fragments when mapping an issue.

## Contents

- Performance and Core Web Vitals
- Images and fonts
- JavaScript and CSS
- Accessibility
- SEO and best practices
- Source mapping rules
- Fix and verification rules

## Performance and Core Web Vitals

| Audit ID or fragment | Inspect first | Common fix direction | Verification |
|---|---|---|---|
| `largest-contentful-paint`, `lcp-*` | DOM node, Page, View, Layout, Image, Style | Prioritize the actual LCP resource, reduce render delay, avoid lazy-loading the LCP asset, and simplify overly expensive hero markup | LCP element remains the same, Preview remains visually consistent, new LCP measurement |
| `cumulative-layout-shift`, `cls-*` | DOM nodes, View, Page, Layout, Style | Reserve space for media or components, stabilize fonts and asynchronous content, and avoid delayed layout insertion | Same viewport and interaction path, new CLS measurement |
| `total-blocking-time`, `interaction-to-next-paint`, long tasks | Script, inline scripts in Views, third parties | Reduce synchronous work, defer noncritical initialization, and isolate page-specific behavior | Interactions work, no Console/Network errors, new metric |
| `server-response-time`, `document-latency-*` | Page, View, Layout, API, CodeBlock, data binding | Remove repeated work, reduce blocking remote or data calls, and simplify server rendering | Response content unchanged, no server error, new latency measurement |
| `render-blocking-resources`, `render-blocking-*`, critical request chains | Layout, Style, Script, shared View | Defer noncritical resources and reduce dependencies while preserving required critical styles | No FOUC, complete functionality, Network ordering, FCP/LCP |
| `network-dependency-tree-*`, redirects | Page route, Layout resource references, external integrations | Remove unnecessary redirects and dependency hops without changing canonical or authentication behavior | Final URL, authentication, canonical, Network chain |
| `dom-size`, `dom-size-*` | Page composition, duplicated Views, Layout | Reduce meaningless wrappers and excessive repeated DOM nodes | Visual hierarchy, bindings, interaction selectors |

## Images and Fonts

| Audit ID or fragment | Inspect first | Common fix direction | Verification |
|---|---|---|---|
| `unsized-images`, `image-aspect-ratio` | DOM snippet, View, Page, Layout | Add correct intrinsic dimensions or an aspect ratio while preserving responsive CSS | No image distortion, responsive layout, CLS |
| `modern-image-formats`, `uses-optimized-images`, `image-delivery-*` | Image object and every View/Page that references it | Confirm quality, transparency, animation, and sharing scope before resizing or converting formats | Visual comparison, file size, all affected pages |
| `offscreen-images` | Image markup in a View/Page | Use lazy-loading only for below-the-fold images; exclude LCP and interaction-critical images | LCP identity, scroll loading, galleries |
| `preload-lcp-image`, `lcp-discovery-*` | Layout/head, hero View, image markup | Make the LCP URL discoverable early; preload only a confirmed and stable LCP resource | No duplicate download, correct responsive candidate |
| `font-display`, `font-display-*` | Font declarations in Style and Layout | Use an appropriate display policy and preload only critical local fonts | Typography, language glyphs, CLS, Network |

## JavaScript and CSS

| Audit ID or fragment | Inspect first | Common fix direction | Verification |
|---|---|---|---|
| `unused-javascript`, `legacy-javascript-*` | Script, Layout, inline code in Page/View | Remove confirmed dead code, limit page-specific code to its proper scope, and defer optional modules | All dependent pages and interactions |
| `unminified-javascript` | Script delivery/configuration | Optimize delivery first; do not replace maintainable source with directly minified code | Source maintainability and actual delivery response |
| `unused-css-rules` | Style, Layout, shared Views | Never delete selectors based on one coverage run alone; verify templates, responsive behavior, hover, locale, and dynamic states | Multiple routes, breakpoints, and interaction states |
| `unminified-css` | Style delivery/configuration | Optimize delivery while keeping the source maintainable | CSS loads correctly and source remains editable |
| `third-party-*` | Layout, shared View, Script, business integration | Confirm ownership and business purpose before deferring or removing | Analytics, consent, ads, chat, payment, authentication |
| `cache-*`, `uses-long-cache-ttl` | Resource type and site/server settings | Recommend configuration changes separately; do not substitute a page source rewrite | Response headers and cache invalidation |

## Accessibility

| Audit ID or fragment | Inspect first | Common fix direction | Verification |
|---|---|---|---|
| `color-contrast` | DOM node, Style, View | Adjust a clearly owned design token or scoped selector; do not change unrelated global colors | States, themes, WCAG contrast, visual design |
| `image-alt` | Image context, View/Page | Write meaningful alt text for informative images; use empty alt text for decorative images | Screen-reader semantics and duplicated content |
| `link-name`, `button-name`, ARIA name audits | DOM node, View/Page/Layout | Add a visible name or accessible name that matches the action | Keyboard and assistive-technology behavior |
| `label`, form audits | Form markup in View/Page | Associate labels and descriptions with the actual control | Submission, validation, keyboard flow |
| `heading-order`, `document-title` | Page and composed Views | Correct the semantic hierarchy; do not choose headings for visual font size | Outline, styling, dynamic title |
| `aria-*`, `role-*` | Exact DOM node and interaction code | Prefer native semantics; modify ARIA only after understanding the widget | Keyboard interaction and state announcements |
| `tap-targets`, viewport audits | Style, Layout, Page | Preserve zoom capability and usable target sizes; inspect responsive breakpoints | Mobile viewport and touch behavior |

## SEO and Best Practices

| Audit ID or fragment | Inspect first | Common fix direction | Verification |
|---|---|---|---|
| `meta-description`, `document-title` | Page metadata and dynamic binding | Use page-specific text grounded in actual content | Rendered head and locale variants |
| `crawlable-anchors` | View/Page links | Use real target addresses; never invent routes | Route exists and navigation works |
| `canonical` | Page/head configuration | Preserve intentional cross-domain and localized canonical strategies | Rendered canonical URL |
| `robots-txt`, `is-crawlable` | Site settings and deployment policy | Treat as a configuration/policy issue and confirm the environment before changing it | Production indexing intent |
| `errors-in-console`, `deprecations` | Script, API/resource failures, third parties | Reproduce the problem and confirm source ownership before editing | Console and Network after repeating the same action |
| `uses-http2`, security headers, CSP | Server/deployment settings | Unless a specific site setting exists, report as an infrastructure/configuration issue | Response protocol and headers |

## Source Mapping Rules

1. Prefer the returned `selector`, `snippet`, resource `url`, or `sourceLocation`; do not guess from the audit title.
2. For DOM evidence, call `read_preview_element_context` when Preview context exists. Otherwise search the composed Page/View/Layout source for a distinctive snippet or resource URL.
3. For a resource URL, first list matching Script, Style, Image, or File objects, then search Page/View/Layout references.
4. Treat shared Layout, Style, Script, and View objects as high-blast-radius. List potentially affected routes before proposing changes.
5. When ownership cannot be confirmed, mark the issue as "requires human review"; never choose an object at random.

## Fix and Verification Rules

- Optimize the bottleneck that was actually measured, not merely the score label.
- Do not remove third-party code before confirming its business purpose.
- Do not delete CSS based on a single coverage run.
- Do not lazy-load a confirmed LCP image.
- Do not change accessibility semantics before checking keyboard and assistive behavior.
- Do not claim that a metric improved before a new measurement is returned. Preview and Preflight detect regressions; they do not prove that the Lighthouse score increased.
