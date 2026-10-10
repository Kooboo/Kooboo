# Site Observability - Visitor Logs

## Page Purpose

- Admin pages include the visit overview, latest visitors, bot intelligence, and rankings for pages, images, sources, countries, and browsers.
- This module records frontend page requests entering the current site to observe visits, request results, sources, and client environment. It is not admin-operation auditing, complete network capture, or a real-name user profile.
- Enable determines only whether later new visits enter the recording flow; re-enabling does not backfill requests during disabled time or before enabling.

## PV, UV, and Date Basis

- Page Views is the count of human page requests recorded in selected dates; bot requests are counted separately and excluded from Page Views.
- Unique Visitors uses request features and a daily salt for same-day deduplication. It is not logged-user count and cannot stably identify the same person across days. For multiple days, the page adds each day's unique visitors, so one visitor across days may be counted repeatedly.
- A statistical day begins at 4:00 AM server time rather than midnight. The date picker queries a date range; consider this boundary across time zones or near dawn.
- Error rate is error-page count divided by human Page Views. Empty results may come from the switch being off, wrong dates, asynchronous log writing not finished, or no matching data and do not alone prove “no one visited” or “no errors ever occurred.”

## Individual Requests and Client Information

- The latest-visitors list can show IP, country, page, start time, status code, duration, Referer, User-Agent, response size, and some client-network hints.
- ECT, RTT, downlink speed, screen pixel density, viewport, and color preference depend on client hints actually provided by the browser. An empty field does not mean a network error or that every browser supports reporting.
- Duration details come from execution stages recorded by the current request and help locate timing clues inside that request; they are not a complete browser performance waterfall and do not cover external resources or user rendering time.
- IP and User-Agent in the list help troubleshooting and rule configuration, but proxies, shared exits, privacy mechanisms, or forgery can affect them; they are not reliable identity credentials.

## Rankings and Bot Classification

- Page, image, Referer, country, and browser rankings aggregate recorded requests for selected dates; they are not site-resource lists, reference relationships, or search rankings.
- An empty Referer may mean direct access, client privacy policy, or a request without a source and does not necessarily indicate malicious traffic.
- Bot names and Good, Bad, Unknown categories come from current identification rules. They help judge traffic composition but cannot identify every automated request, and Unknown must not be treated directly as malicious.
- Blocked means the statistics observed that the bot was blocked by an existing rule; it does not mean all its requests were permanently blocked.

## Configuring Access Limits from Logs

- A specific IP or User-Agent keyword can be added from one log to blocking or rate-limit settings. Saving changes site-level request-limit configuration and may enable the corresponding limit automatically; it is not merely tagging the historical record.
- IP blocking and User-Agent keyword blocking reject later matching requests; rate limiting applies to later requests by specified seconds and allowed count. They do not delete existing logs or retract completed requests.
- User-Agent keywords use contains matching; a short or broad keyword can affect many normal clients. Shared IPs, proxy exits, and dynamic IPs can also make IP rules affect unrelated visitors.
- After configuration, use controlled requests to verify actual status codes and allowed frequency; a changed list icon alone does not prove proxy, cache, or every entry point executes as expected.
