---
name: element-plus-runtime
description: Consume the bundled native Element Plus Vue runtime for Kooboo AI Apps using exact exports, documented APIs and public theme tokens.
---

# Element Plus

Use design-system ID `element-plus`. create_app_base installs the complete official Element Plus 2.14.5 distribution with Kooboo adapter 0.3.0. `components` optionally requests API guides only. Discover guide names in `runtime/component-selection.json`; new component usage needs no installation or synchronization. Reuse returned usage/APIs before reading more resources.

Follow `runtime/usage.md` for registration, locales, services and layout. Preserve native El* APIs, interaction and theme-chalk variables. Use `runtime/extensions.md` for custom business components and `runtime/api/{slug}.md` for selected APIs. Do not rebuild upstream controls from static CSS, import another Vue, run npm in generated sites, or copy maintenance demos as business facts.

Layout comes from the current business. Optional `element-plus-workspace` is one sidebar preset, not the only arrangement. Its main content already has 24px padding (16px at widths up to 767px). Inspect `runtime/element-plus.adapters.css` for Shell, navigation skeleton and icon styles; these are separate from upstream `runtime/element-plus.css`. Keep charts as a separate integration themed through Element Plus tokens. Business owns content, requests, validation, state and authorization. Runtime installation does not establish business or browser acceptance.
