# Site Observability - Resource Protection

## Page Purpose

- Admin pages include the resource-protection overview, protection rules, and recent resource logs.
- This module observes requests for site resources such as images, styles, scripts, and files and can protect later requests by path, extension, and source domain.
- Resource Logs and Website Protection are independent switches: the former controls statistics collection and the latter rule execution. Enabling logs does not enable blocking, and enabling protection does not mean complete statistics already exist.

## Resource Statistics

- Statistics summarize human and bot resource requests, bandwidth, file types, external sources, invalid resources, and protected requests for the selected dates. Requests while logs are disabled are not backfilled.
- An invalid resource was requested but not found, with observed source locations; it is not a static dependency scan and does not guarantee discovery of runtime, external, or unvisited references.
- External sites using your resources aggregates request Referer. Referer can be missing or forged, so it is a traffic clue, not proof of domain ownership or infringement.
- Resource relationships provide known references only when the system can identify the corresponding site route; they are not a complete dependency graph. Check dynamic URLs, script concatenation, and external references before changing or deleting resources.
- Recent resource logs come from an in-memory recent circular record and are paginated by the frontend; they are not complete historical details for selected dates or long-term audit logs.

## Rule Matching

- After saving, rules are checked by ascending Priority; the first rule matching path, extension, and source conditions executes and stops later matching.
- Start Path is a case-insensitive prefix match. Multiple prefixes can be comma-separated, and a trailing `*` means only that prefix; it is not a regular expression.
- Extension exactly matches the extension without the dot, case-insensitively. `*` means all extensions; a URL without an extension matches only an all-extensions rule.
- With an empty domain blacklist, a rule applies to all sources when path and extension match. With a blacklist, only an exact Referer host match executes it; a request without Referer does not match this blacklist condition.
- Domain matching does not automatically include all subdomains and does not treat the path as part of the domain. Test the main domain, subdomain, port, empty Referer, and forged Referer separately before saving.

## Protection Actions

- Forbidden returns 403 for a matching request.
- Redirect returns 302 for a complete HTTP/HTTPS address. For a site address, the system tries to resolve a site route and returns 404 if the target is not found. An invalid target or mutual redirects can create an unusable entry or loop.
- The current interface provides Watermark with text, opacity, font-size, and position, but current runtime does not actually generate watermark images. Do not interpret a successful save or rule hit as proof that watermarking is active.
- Protection depends on request information and cannot replace login authorization, private-storage signatures, or CDN security policy. Referer hotlink protection especially is not an unbypassable access control.

## Change Boundaries

- Confirming in the rule dialog changes only the pending list in the page; click Save at the bottom for rules to persist.
- Deleting a rule or disabling Website Protection affects later matching only. It does not delete historical logs, restore returned responses, or retract resources already cached by external sites.
- After changing rules, use real resource URLs to verify allowed sources, forbidden sources, empty Referer, different extensions, and priority conflicts separately; do not judge activation only from the rule list.
