# Site Settings - Basic Settings

## Page Purpose

- Admin page: `/system/settings?name=basic`
- Manages site display name, preview address, HTTPS redirects, SPA, multilingual settings, and site-package import/export.
- Describe actual Kooboo behavior first, then add general website recommendations.

## Display Name

- The display name is the human-readable name used to identify the site in Kooboo admin.
- Changing it does not change internal site name, site ID, existing routes, or domain bindings.

## Base URL

- This configures the base address Kooboo uses to generate preview and site-access addresses.
- It is not domain-binding management; add, delete, or switch production domains on the Domain Management page.
- A wrong address can send admin previews, redirect links, or features depending on the site root to the wrong location.

## Force SSL

- When enabled, ordinary HTTP requests bound to the site redirect to HTTPS.
- This switch does not request, install, or renew an SSL certificate; ensure HTTPS works for target domains first.
- With a reverse proxy or CDN, failure to pass the original protocol correctly can create repeated redirects; check proxy configuration too.

## SPA

- The SPA switch supports single-page apps using frontend History routing.
- When a request matches no Kooboo route and the site has a default home page, Kooboo renders that home page at the current request address for the frontend router to handle.
- This is not a redirect to the home page; the browser keeps the requested address.
- An ordinary multi-page site should not enable SPA merely because it uses JavaScript, Vue, or React.
- After enabling, a truly nonexistent address may return the app shell instead of a normal 404, affecting error monitoring and SEO judgment.
- For details, read `kooboo-cms-guide` `references/system/settings/spa-routing.md` as needed.

## Multilingual

- Multilingual settings manage supported languages, default language, language-address mode, automatic language detection, and Hreflang.
- They do not translate existing content; each language still requires corresponding content, Labels, or field values.
- For details, read `kooboo-cms-guide` `references/system/settings/multilingual.md` as needed.

## Import and Export

- Importing a site package applies compressed site content to the current site and may overwrite or update existing objects.
- Site export can be complete or custom by resource type; modes differ in scope and size.
- Successful download means only that the package was generated; external databases, payments, object storage, domains, certificates, and other environment dependencies are not guaranteed migrated.
- For details, read `kooboo-cms-guide` `references/system/settings/import-export.md` as needed.

## Common Misconceptions

- Base URL is not domain binding.
- Force SSL is not automatic certificate configuration.
- SPA fallback rendering is not a redirect rule.
- Enabling multilingual is not automatic translation.
