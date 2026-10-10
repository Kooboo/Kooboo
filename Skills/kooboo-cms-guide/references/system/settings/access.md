# Site Settings - Access Control

## Page Purpose

- Admin page: `/system/settings?name=access`
- Controls who can access the site, path exceptions, visitor country/region restrictions, request rate, request blocking, cross-origin access, and development-preview protection in certain hosting environments.

## Site Types

- Public Site: all visitors can access.
- Logged-in Users: any user logged in to Kooboo can access.
- Organization Members: only current organization members can access.
- Site Users: only users in the current site-user list can access.
- An unauthenticated user visiting a protected page is redirected to login; a logged-in user without the required qualification receives forbidden access.

## Path Rules

- Path rules apply only when the site is not public.
- Exclude Paths makes listed paths public exceptions while other paths remain protected by site type.
- Include Paths keeps only listed paths protected while other paths become public exceptions.
- Paths use full matching and support regular-expression semantics. Special characters can change scope; an invalid expression does not match.
- Include Paths is easy to misunderstand: if only a few paths are entered, other unlisted pages may become public. Verify each path in an unauthenticated window after saving.

## Single Sign-On

- When enabled, a protected page requiring login redirects to Kooboo's single-sign-on entry and returns to the original address after success.
- It changes only login redirection and does not automatically change site type, organization membership, or site-user permissions.

## Visitor Country or Region Restrictions

- Include allows only selected countries or regions; Exclude rejects selected countries or regions.
- Selecting no country or region produces no restriction.
- Restrictions mainly apply to normal frontend page requests, not admin system paths or nonstandard page channels.
- A restricted request returns 451. A site page can provide the message; missing configuration or page uses default error handling.
- Country or region judgment depends on access-IP geolocation and is not suitable as the sole identity or compliance-authentication method.

## Request Rate Limits

- Limit All Requests limits allowed counts within a specified window by visitor IP.
- With it off, individual limits apply only to an exact IP in the list or a request whose User-Agent contains specified text.
- An over-limit page request returns 429.
- This suppresses bursts and simple abuse; it is not a complete firewall, bot detector, or distributed-attack defense.

## Request Blocking

- IP blacklist uses exact client-IP matching.
- User-Agent blacklist uses contains matching for keywords; verify case and keyword scope with real requests.
- A page request matching a blocking rule returns 422.
- User-Agent can be forged by the client and is not a strong identity basis alone.

## CORS

- When enabled, Kooboo returns response headers for allowed origins, methods, headers, and credentials on cross-origin and preflight requests.
- The current setting is not a per-domain origin allowlist and must not be treated as a strict cross-origin security policy.
- CORS constrains whether browser scripts can read a response; it does not replace login, permission checks, or API authentication.

## Development Preview Password

- This area appears only in qualifying hosting environments and service tiers.
- It protects development-preview access and is not an ordinary site-member password or replacement for site type and user permissions.
