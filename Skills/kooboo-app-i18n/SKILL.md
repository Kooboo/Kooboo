---
name: kooboo-app-i18n
description: "Add or maintain multilingual Vue AI Apps using Kooboo Labels and vue-i18n, including language switching, translated menus and titles, and component-library locales. Use for requested App internationalization or changes to an App already using this integration; single-language Apps need no installation."
---

# Kooboo App i18n

Load `kooboo-coding` first for site writes and follow its existing planning/authorization and verification rules. This skill adds an optional capability; loading it does not install anything. A site's multilingual switch alone does not mean its App uses vue-i18n.

1. Read current site settings and affected index/main/router/layout/services. Recognize an existing integration by its actual main registration and service, not by memory alone. For installation or migration, read [integration](references/integration.md).
2. Use Kooboo Label as the persistent application-copy source. Use stable application-prefixed keys, at most 64 ASCII letters/digits/dots/underscores/hyphens. Read existing values before `set_label`; preserve translations unless their modification is requested. Browser reads must never create Labels.
3. Keep App i18n settings in the shared server `ENV.i18n`; follow the integration guide to preserve existing ENV fields and review the installer's configuration receipt. Install independent assets with `scripts/install-runtime.js`; do not read/re-emit the vendored library. Installer success means assets exist, not that the App is translated. Read and patch existing import map/main/layout/router locally. Keep single-language defaults unchanged.
4. Translate interface text using Composition API `useI18n().t`. Menus, rules, and titles must react to locale changes. Runtime Label templates use vue-i18n syntax; do not pre-interpolate them with k.label or render them as HTML. Historical notifications and business content are separate data.
5. Read [acceptance](references/acceptance.md) when verifying delivery or diagnosing a language issue. Preserve existing routing/authentication. Follow `kooboo-coding`'s browser opt-in policy; report unverified interactions honestly.

The backend exposes only configured application prefixes. The runtime loads the requested and default languages, caches within this page session, and commits a language switch only after resources are ready. Do not use `/_spa/lang` (different storage), normal Label fallback, or a second JSON translation store.

Record the installed integration and language/key conventions in site memory after successful integration. Continue loading this skill for later affected App edits. Existing planning descriptions and acceptance criteria can record language scope; do not add undeclared App Plan fields.
