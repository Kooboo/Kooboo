# Site Settings - Performance

## Page Purpose

- Admin page: `/system/settings?name=performance`
- Configures script and style minification, browser caching, image-cache policy, video cache, and conversion of uploaded images to WebP.
- After changes, verify first load, repeat access, and behavior after resource updates; one page opening is not enough.

## JavaScript and CSS Minification

- When enabled, Kooboo generates more compact content when returning site scripts and styles to reduce transfer size.
- Already-minified scripts and some unsuitable for processing again skip repeated minification.
- Minification here is content minimization, not server Gzip or Brotli transfer compression.
- If third-party or legacy scripts rely on unusual syntax, test key interactions after enabling and compare returned resources with the setting off if errors occur.

## JavaScript and CSS Browser Cache

- When enabled, Kooboo adds version information to site-script and style addresses and returns long-term cache information.
- When resource content changes, the version changes and the browser requests a new version; unchanged content can reuse local cache.
- This mainly affects site resources managed and normally rendered by Kooboo and does not control every external CDN file automatically.

## Video Browser Cache

- When enabled, Kooboo-managed video resources return long-term browser-cache information to reduce repeated downloads.
- After replacing a video, verify actual address and cache, especially when reusing the same path.

## Image Browser Cache

- Version-Controlled mode: Kooboo adds version information when rendering image addresses and uses long-term cache. After an image changes, the new address prompts a fresh browser fetch.
- Fixed-Days mode: the browser caches images for configured days and requests again after expiration; it does not depend on rewriting address versions.
- Images that update frequently while keeping the same address suit Version-Controlled mode better.
- Use Fixed-Days mode when the cache period must be explicit and predictable.
- This mainly targets Kooboo-managed site images; external images use cache policy from their source server.

## Automatic Image Conversion to WebP

- When enabled, Kooboo tries to convert supported images to WebP on later creation or update.
- It does not batch-rewrite every existing site image automatically.
- Unsupported formats, deletion, or conversion failure do not guarantee that every image becomes WebP.
- After enabling, check transparent and animated images, quality, and frontend references.

## Common Misconceptions

- Content minification is not network transfer compression.
- Browser cache does not reduce first-download size.
- Long cache requires reliable version changes or users may continue to see old resources.
- The WebP switch mainly affects later uploads or updates, not one-click migration of the historical media library.
