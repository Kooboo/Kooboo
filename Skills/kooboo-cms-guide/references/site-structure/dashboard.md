# Site Overview - Dashboard

## Page Purpose

- Admin page: `/site`
- The dashboard gathers basic site information, major resource scale, recent visits, and recent editing summaries to quickly assess the site and enter the next management task.
- It is a read-only aggregation page, not a real-time monitoring center, complete audit system, SEO report, billing page, or order-management page. Opening or refreshing it does not modify underlying site resources.

## Recent Visits and Site Information

- Recent Visits comes from admin access records saved by the current browser for this site and shows at most common entries. It is not shared server access history for all administrators or frontend visitor behavior.
- Site information includes name, site type, creation time, bound domains, site members and roles, and a preview address generated from the site base address and default route.
- A preview address existing does not mean DNS, SSL, publication status, permissions, or external network access are correct; open it in the target environment to verify.
- Domain and member information is a current-state summary. Check binding, permission, and role details and risks on their corresponding management pages.

## Resource Statistics

- My Resources counts and sizes major site resources such as pages, views, layouts, code, content, images, styles, scripts, HTML blocks, and Labels.
- These numbers are not a complete ledger of site data and exclude every database table, log, cache, external storage, e-commerce data, and third-party service data.
- Count and size describe scale only; they do not prove resources are published, referenced, renderable, or free of redundancy.

## Traffic and Rankings

- Visitor trends, popular pages, and popular sources summarize site access statistics from the latest 30 days.
- Popular pages and sources are visit-count rankings, not page lists, reference relationships, SEO keyword rankings, or conversion analysis.
- An empty or low chart does not alone prove no site visits; also check access-log enablement, selected site, and usable data in the period.
- The dashboard shows only overview metrics. For bots, human visitors, countries, browsers, or individual requests, use Visitor Logs and Behavior Analysis.

## Recent Editing and External Information

- The Site Logs area shows at most the latest 10 site-object editing summaries, including object, action, user, and time.
- It is not a complete operation audit or login, HTTP-request, code-execution, or automatic-job log; use Site Operation Logs for full versions, comparison, and rollback.
- Documentation entries, banners, and news may come from Kooboo external sources and short-term local cache, so they may be missing or delayed by network, language, or remote content state.
- The current dashboard does not provide effective management for recent orders, account balance, or fee settlement; view order and sales data on corresponding e-commerce pages.
