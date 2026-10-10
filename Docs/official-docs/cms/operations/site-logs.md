# Site Logs

> Menu: **Operations Center -> Site Logs**  
> Deep link: `/_Admin/system/site-logs?SiteId={siteGuid}`

Site logs track content and resource changes across a site: who changed what, when it happened, and which version was affected. You can review versions, compare differences, restore past states, export changes, and sometimes replay recorded editing videos.

<DocImage src="/cms/operations/site-logs-overview.png" alt="Site logs overview" width="1120" />

## Filters

| Control | Description |
|---------|-------------|
| Object type | Store name such as page, view, or content |
| Action | Add, update, delete, and similar actions |
| User | Admin user who made the change |
| Time range | Start and end date-time |
| Log item | Search by object name |
| Reset | Clear filters |
| Clear logs | Start a cleanup task for historical logs |

<DocImage src="/cms/operations/site-logs-filters.png" alt="Site log filters" width="1120" />

## Log list

| Column | Description |
|--------|-------------|
| ID | Log record id |
| Log item | Object display name; click it to open versions |
| Object type | Store type |
| Action | Change type |
| User | Operator |
| Last modified | Last modification time |
| Actions | Replay video or open version history |

<DocImage src="/cms/operations/site-logs-table.png" alt="Site log table and actions" width="1120" />

### Batch actions

| Button | Description |
|--------|-------------|
| Undo Changes | Revert the selected changes |
| Restore | Restore one selected log point |
| Checkout | Create a new site from one selected version |
| Export Changes | Export the selected change package |

<DocImage src="/cms/operations/site-logs-checkout-dialog.png" alt="Checkout site dialog" width="1120" />

## Version history

Route: `/_Admin/system/site-logs/versions?...`

This page lists version history for one object and lets you revert, compare with current, compare two versions, or replay a video when available.

<DocImage src="/cms/operations/site-logs-versions.png" alt="Site log version history" width="1120" />

## Version compare

Route: `/_Admin/system/site-logs/version-compare?...`

Kooboo opens a Monaco diff view or image diff for the selected versions.

<DocImage src="/cms/operations/site-logs-version-compare.png" alt="Version comparison diff" width="1120" />

## Related

- [Operations Center](./index.md)
- [Labels](../content/labels.md)
- [HTML Blocks](../content/html-blocks.md)
