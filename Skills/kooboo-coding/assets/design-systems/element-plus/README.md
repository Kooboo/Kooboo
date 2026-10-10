# Element Plus for Kooboo AI Apps

Official **Element Plus 2.14.5** (MIT), Kooboo adapter **0.3.0**, shared App Base **Vue 3.5.13**.

The complete official browser ESM `dist/index.full.min.mjs` is copied byte-for-byte into `runtime/element-plus.vendor.js`. It is the same file served by the pinned CDN URL recorded with its SHA-256 in `runtime/manifest.json`. Official `dist/index.css` is copied unchanged into `runtime/element-plus.css`. Kooboo Shell, navigation skeleton and icon styles live separately in the readable `runtime/element-plus.adapters.css`. Generated Apps use these local resources without fetching Element Plus from a CDN.

`runtime/element-plus.js` is a small standard esbuild bundle containing only the Kooboo Shell/icon adapters and two locale objects; it re-exports the official vendor module. Vue stays external and resolves through App Base's import map. Official components are neither split nor rebuilt. No custom module loader, dependency archive, CSS ownership map or per-site component selection is maintained.

The .NET installer copies the manifest's four fixed assets and links official CSS before adapter CSS, with business `/app.css` afterward. The optional `components` input selects documentation only. Component APIs and native exports remain available without additional installation. `sync_design_runtime` provides protected fixed-version upgrades; experimental chunk installations are not migrated. Existing fixed-distribution Apps must link `/element-plus.adapters.css` after `/element-plus.css` before upgrading from the combined CSS distribution; an absent link blocks the upgrade without changing resources.

This package contains runtime assets and consumption references. Licenses and notices remain in `runtime/licenses/` and the adapter entry. Business guidance lives in `runtime/usage.md`, `runtime/extensions.md` and `runtime/api/`.

Implementation/test evidence is separate from manual browser and real AI App acceptance.
