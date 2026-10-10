# Vendored runtime

- Package: vue-i18n 11.4.8, MIT.
- Upstream: https://github.com/intlify/vue-i18n
- Source: npm vue-i18n@11.4.8, `dist/vue-i18n.esm-browser.prod.js`.
- Copied unchanged from the workspace's installed Frontend dependency.
- SHA-256: `af72dff8c31b938306ef779e73d2604255e387796db30af3aa855d77a64af3b1`.
- License: `vue-i18n.LICENSE`; upstream copyright header retained in the asset.
- Full browser ESM build includes message compilation and imports `vue` externally.
- Updating the dependency requires rerunning runtime.spec.mjs against the App Base Vue and KScript installation tests; copying a new version is not an automatic upgrade of already customized sites.
