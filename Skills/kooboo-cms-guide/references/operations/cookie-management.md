# Site Compliance - Cookie Management

## Page Purpose

- This page configures the frontend Cookie-consent interface, display regions, categories, controlled resources, explanatory copy, and reports.
- The frontend injects the consent-management script and handles resources matched in the configuration only after Cookie Management is enabled. Re-enabling does not retroactively block already-loaded resources or clear Cookies already in the browser.
- This feature controls consent for configured frontend resources; it is not a general compliance scanner that discovers and blocks every Cookie, localStorage item, dynamically inserted script, or server-response Cookie.

## Display Regions

- All displays for every origin; EU displays only for visits identified as from the European Union; Custom can include or exclude specified countries.
- Region detection depends on country information identified by the server, not browser language. Proxies, shared exits, missing location data, or an unidentifiable country can make behavior differ from the user's location.
- The close button's default behavior is reject in EU regions and accept elsewhere; when continent information cannot be identified, it treats the visit as reject. accept and reject are fixed to acceptance and rejection respectively.

## Category and Resource Control

- Required categories cannot be deselected in the interface, and default selection determines the initial display state of optional categories. At runtime, Reject All gives special bypass only to resources whose category identifier is `essential`; do not assume any custom category or the default `necessary` identifier receives the same exemption automatically.
- Resource matching handles only script, image, iframe, and stylesheet-link addresses in HTML. Query parameters and fragments are removed before matching against configured URL wildcard rules.
- Without permission, a matched resource's real address is stored in a controlled attribute; an iframe also displays configured replacement content. The address is restored and loaded only after the user permits it.
- Only static resources matched in the configuration are governed. Dynamically inserted tags, inline code, external redirects, browser storage, and Cookies set directly by the server are not automatically controlled by this.
- Cookie names, descriptions, and expiration times entered under a resource are mainly for the consent interface and do not automatically create, renew, or delete browser Cookies.

## Links Scan, Save, and Preview

- Links scans only external URLs in static HTML of layouts, pages, views, and HTML blocks to help fill controlled resources; it is not a complete dependency graph and cannot find runtime-concatenated or script-dynamically loaded addresses.
- Deleting a category or resource first changes only the current form; click Save at the bottom to persist it. Deleting a category does not automatically repair resource configuration still referencing it or delete historical statistics.
- Save and Preview saves the current configuration first and then opens the site home page. It is not an isolated draft preview; saved rules may immediately affect real visitors.

## Report Basis

- When a user submits a consent state, the system adds one record by UTC date; every report is counted, with no visitor deduplication.
- Mode statistics distinguish all accepted, all rejected, and custom choices; category statistics count only accepted categories.
- Report numbers are not unique visitors, strict consent rates, or complete audit logs. Repeated choices, cleared state, or multi-device visits by one user can create multiple records.
- Deleting a category does not clean existing statistic keys. When analyzing historical trends, include category-change times and do not treat data from same-named or deleted categories as a stable basis directly.
