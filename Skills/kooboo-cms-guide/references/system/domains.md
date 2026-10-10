# Site Settings - Domain Management

## Page Purpose

- Admin page: `/system/domains`
- This page manages “how a domain or port binds to the current site.” Binding decides which site receives a request; it is not for buying domains, changing public DNS, or creating page routes.
- One site can have multiple bindings. Deleting or changing a binding changes an access entry but does not delete the site, pages, content, or domain account.

## Creating a Binding

- A domain binding selects an available root domain from the current organization and fills a subdomain. Online environments also verify domain ownership; an email-only domain cannot be a website binding.
- Local deployment can use a port binding. The port must be available; saving a binding does not free a port occupied by another service.
- A complete domain cannot be bound twice. Successful binding means only that Kooboo recorded a host-to-site mapping; public DNS, reverse proxy, firewall, and certificate must separately satisfy access conditions.
- A request entering through a binding uses it to locate the site. Do not treat domain binding as an internal URL alias; page paths remain determined by site routes.

## Redirects and Language

- Redirect To is a binding-level domain redirect. Visiting the binding returns a permanent redirect to the target domain and preserves the original path and query parameters.
- Target protocol depends on whether the target domain has a usable certificate. It is not page-level rewriting or content copying; incorrect configuration can create a redirect loop.
- After a language is selected for a binding, requests entering through it prefer that language. It does not translate content or replace site multilingual, Label, or content-language configuration.

## SSL and Forced SSL

- The SSL switch for one domain binding first verifies issuance conditions, then requests or enables a certificate. Only domains verifiable from the internet, using Kooboo DNS, or hosted by Kooboo may pass.
- Successful SSL means the host has certificate configuration; it does not mean every other binding has a certificate. On failure, inspect DNS, domain reachability, and hosting conditions from the list error.
- Force SSL is site-level: for an HTTP request received through a site binding, the system returns 308 and redirects to the corresponding HTTPS address. It differs from binding-level Redirect To, which returns 301; do not confuse the status codes. Force SSL does not repair DNS or replace certificate issuance.
- Before enabling Force SSL, confirm every domain still in use works over HTTPS, or visitors may be redirected to a certificate error or unreachable address.

## Deletion and Verification

- Deleting a binding removes only the domain or port entry to the current site; it does not delete the root domain, DNS record, SSL certificate, site resources, or bindings of other sites.
- After deleting the current primary binding, the old address may immediately stop locating this site; admin access through another binding, port, or management entry does not mean the old domain remains valid.
- After changes, verify real access with HTTP, HTTPS, original paths, paths with query parameters, and each language domain separately; do not treat a successful admin list as a release conclusion.
