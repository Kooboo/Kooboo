# Resource Guardian

> Menu: **Operations Center → Resource Guardian**  
> Deep link: `/_Admin/system/resource-guardian?SiteId={siteGuid}`

Monitor **static resource** requests and bandwidth for a site: bandwidth by file type, resource composition and access ranking, live request list, bot traffic, and **hotlink / unauthorized scraping** protection. **Enable** resource logging first; stat cards and charts load only after that. The **Site Protection** switch controls whether off-site referrers are blocked per protection rules.

::: tip Permission
Same menu permission as Visitor Logs: `visitorLog` (view). Enable **Operations Center → Resource Guardian** in Edit Menu.
:::

## How to Open

1. In [Edit Menu](../navigation.md#permissions-and-edit-menu), enable **Operations Center → Resource Guardian**.
2. Open **Operations Center → Resource Guardian** on the left.

After entry, the main screen flows top to bottom: header controls, protection entry, metric cards, bandwidth analysis, resource composition and ranking, live log, bot intelligence, external site list; sections below include screenshots.

<DocImage src="/cms/operations/resource-guardian-overview.png" alt="Resource Guardian main screen overview" width="1120" />

## Header

Path: `/_Admin/system/resource-guardian?SiteId=...`

| Control | Description |
|------|------|
| **Enable** | `enableResourceLog`; when off, stats clear and only protection config entry remains |
| **Date Range** | Global `userBehaviorDateRange` (shared with behavior analysis, etc.); changing it refreshes `ResourceLog/Info` |

<DocImage src="/cms/operations/resource-guardian-header.png" alt="Resource Guardian header: enable and date range" width="1120" />

## Protection Entry

| Control | Description |
|------|------|
| **Configure Protection Rules** | Go to [Protection Rules](#protection-rules-subpage) (`resource-guardian/protection-rules`) |
| **Site Protection** | `enableGuardian`; when on, off-site requests are handled per rules (block / redirect / watermark, etc.) |

<DocImage src="/cms/operations/resource-guardian-toolbar.png" alt="Configure protection rules and site protection switch" width="1120" />

## Overview Metrics (Four Cards)

Shown after resource logging is enabled and a date range is selected:

| Card | Description |
|------|------|
| **Total Resource Traffic** | Total bandwidth of resource requests in the interval |
| **Invalid Resources** | 404 etc. resource count + **referrer** page count; click card for detail dialog (Resource / Referrer tabs) |
| **Protection Blocks** | Requests blocked by protection rules; click card for blocked resource list |
| **Traffic Composition** | Bot vs human share (`machineRate` / `humanRate`) |

<DocImage src="/cms/operations/resource-guardian-stat-cards.png" alt="Total resource traffic, invalid resources, protection blocks, traffic composition" width="1120" />

### Invalid Resources Dialog

After clicking **Invalid Resources**, switch **Invalid Resources** / **Referrer** for Top lists: resource URL, usage location, request count, referrer count, etc.

<DocImage src="/cms/operations/resource-guardian-invalid-dialog.png" alt="Invalid resources detail dialog" width="1120" />

### Protection Blocks Dialog

After clicking **Protection Blocks**, the table lists blocked **resources**, **request count**, and main **referrer** domains.

<DocImage src="/cms/operations/resource-guardian-protected-dialog.png" alt="Blocked resources dialog" width="1120" />

## Bandwidth Analysis Block

Horizontal stacked bars and legend by **file type**: bandwidth share and actual size per type (`bandwidthByFileType`). Hidden when there is no data.

<DocImage src="/cms/operations/resource-guardian-bandwidth.png" alt="Bandwidth analysis: stacked bars by file type" width="1120" />

## Site Resource Composition and Resource Ranking

Two columns in one row:

| Block | Description |
|------|------|
| **Site Resource Composition** | Image, script, style, file, page counts and share bars |
| **Resource Ranking** | Tabs: **Resource** / **Referrer** / **System Resource**; bar chart sorted by request count; open resource links |

<DocImage src="/cms/operations/resource-guardian-composition-ranking.png" alt="Site resource composition and resource ranking" width="1120" />

## Live Resource List and Usage Tracking

Main table shows ~**5** live records: **Resource URL**, **Referrer**, **Request Location**, **Usage Analysis** (bandwidth, request count); expand for links to pages/views, etc. **View Live Data** → [Resource Log subpage](#resource-log-subpage).

<DocImage src="/cms/operations/resource-guardian-live-log.png" alt="Live resource list main table" width="1120" />

## Bot Intelligence Analysis

Same component as [Visitor Logs](./visitor-logs.md): `botInfo` summary for resource requests (~5 rows on main screen).

<DocImage src="/cms/operations/resource-guardian-bot-intelligence.png" alt="Bot intelligence analysis" width="1120" />

## External Sites Using Your Resources

Table of external domains: **Site Domain**, **Request Count**, **Bandwidth Usage**, **Status** (Blocked / Allowed). Use to detect hotlinking or abnormal off-site references.

<DocImage src="/cms/operations/resource-guardian-external-domains.png" alt="External sites using this site's resources" width="1120" />

## Subpages

### Protection Rules Subpage

Path: `/_Admin/system/resource-guardian/protection-rules?SiteId=...`

Breadcrumb: **Resource Guardian → Protection Rules**. Rule table fields: **Priority**, **Start Path**, **Protected Extensions**, **Block Type** (hard block 403 / redirect / watermark); row **Edit**, **Delete**. **Add Rule** opens dialog. Footer **Save** writes `ResourceLog/UpdateSetting`.

<DocImage src="/cms/operations/resource-guardian-protection-rules.png" alt="Protection rules list" width="1120" />

#### Add / Edit Rule Dialog

Configure: start path, block type, redirect URL (when redirecting), watermark text/opacity/color/font size/position, protected extensions, blacklist domains, etc.

<DocImage src="/cms/operations/resource-guardian-rule-dialog.png" alt="Add protection rule dialog" width="1120" />

### Resource Log Subpage

Path: `/_Admin/system/resource-guardian/log-list?SiteId=...`

Breadcrumb: **Resource Guardian → Live Resource List and Usage Tracking**. Full paginated list: **Status**, **Resource URL**, **Type**, **Bandwidth**, etc. (per UI columns).

<DocImage src="/cms/operations/resource-guardian-log-list.png" alt="Resource log full list subpage" width="1120" />

::: tip Resource logging disabled
When header **Enable** is off, main stats and charts are empty with a "resource logging not enabled" message; turn on to view request logs and traffic stats. Protection rules can be configured in advance; **Site Protection** must also be on to take effect.
:::

## Related

- [Operations Center overview](./index.md)  
- [Visitor Logs](./visitor-logs.md) — site-wide visitors and page visits  
- [Site Settings](../navigation.md#site-settings) — resource CDN, cache, etc. (dedicated topic pending)
