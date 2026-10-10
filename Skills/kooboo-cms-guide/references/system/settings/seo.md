# Site Settings - SEO

## Page Purpose

- Admin page: `/system/settings?name=seo`
- Manages Lighthouse optimizations, sitemap, PWA, visitor logs, search-engine blocking, image Alt, and Canonical URL.
- These switches separately affect page output, crawler hints, logs, or offline caching; none is equivalent to “SEO is complete.”

## Lighthouse Optimization

- The page shows optimization items available in the current version and allows each to be enabled or configured.
- Visible items do not mean each has complete, verifiable page-rewrite capability.
- Do not promise “automatically passes Lighthouse after enabling” or a specific score. Use actual page output and repeated Lighthouse results.

## Sitemap

- When enabled, the site provides an XML sitemap at the configured address.
- Automatic mode generates address lists from Kooboo pages and suits simple structures wanting automatic page coverage.
- Manual mode lets developers customize the result for excluding pages, adding special addresses, or custom rules.
- A manual configuration error can make the sitemap return an error or invalid XML; visit the sitemap address and verify after saving.
- A sitemap helps crawlers discover addresses but does not guarantee indexing or ranking.

## PWA

- When enabled, Kooboo adds an app manifest and Service Worker registration to pages and provides corresponding PWA resource addresses.
- Name, icons, theme color, background color, start address, and display mode describe the installed-app experience.
- Service Worker rules can use a cache-first strategy by request method and address pattern and set cache validity.
- Overly broad rules may cache interfaces or old content that should not be cached; be especially careful with login state, orders, payments, and personalized responses.
- Opening PWA does not mean the site automatically meets all installation or offline conditions; check HTTPS, browser support, icons, and cache rules.

## Visitor Logs

- When enabled, Kooboo records new frontend visitor logs and qualifying page-not-found visits.
- Disabling stops collecting new logs and does not automatically delete saved history.
- Visitor logs support site analysis and are not code logs or SQL logs.

## Block SEO

- When enabled, Kooboo declares `noindex, nofollow` in responses, telling search engines not to index pages or continue following links.
- This is an instruction to crawlers, not access control. Visitors may still open pages; sensitive content needs real login and permission protection.
- Already indexed pages are not guaranteed to disappear immediately; wait for search engines to recrawl and process them.

## Use Image Alt Values

- When enabled, Kooboo tries to use the media-library Alt maintained for a site image when the page has no explicit Alt.
- An Alt already written on the page is not replaced.
- External images or images without Alt in the media library do not receive a high-quality description automatically.
- Whether a decorative image should have empty Alt remains a page-design and accessibility decision.

## Canonical URL

- When enabled, Kooboo declares a canonical address in page responses to help search engines combine multiple addresses for the same content.
- Force HTTPS makes the canonical address use HTTPS; Canonical Domain unifies the canonical primary domain, and when empty uses the current access domain.
- Common advertising-tracking parameters are removed from the canonical address; other valid query parameters may remain.
- In current runtime behavior, Normalize Home Page has no independently confirmed behavioral difference; do not promise that toggling it changes the home-page canonical address.
- Canonical is a search-engine hint, not a redirect, and does not automatically repair wrong domains, duplicate content, or route design.
