# Write Your First KScript API

> Add a server-side endpoint to Kooboo so the frontend can retrieve JSON or execute business logic.

## Choose an Entry Point

| Entry | Recommended use | Documentation |
|-------|-----------------|---------------|
| API page | A standalone URL that returns JSON or handles a form | [CMS: Open APIs](../cms/development/openapis.md) · [k.api](../api/api/index.md) |
| CodeBlock | Reuse the same logic from several pages or APIs | [CMS: Code](../cms/development/code.md) · [k.site.codes](../api/site/code.md) |
| Scheduled job | Periodic synchronization, cleanup, or notifications | [CMS: Jobs](../cms/development/jobs.md) · [k.site.runJob](../api/site/job.md) |

For your first attempt, start with an API page: enter a URL, save the script, and call it from the browser or a frontend `fetch` request.

## What a Minimal API Must Consider

| Item | Details |
|------|---------|
| Route | Give the API a stable URL such as `/api/news` |
| Input | Read query parameters, body, headers, or files from [k.request](../api/request/index.md) |
| Business data | Retrieve data with [k.content](../api/content/index.md), [k.DB](../api/database/index.md), or an external service |
| Output | Return JSON, a redirect, or a status code with [k.response](../api/response/index.md) |
| Permissions | Read [k.session](../api/session/index.md) or account APIs when authentication is required |
| Logging | Write important failure paths to [k.logger](../api/logger/index.md) |

## Coordinate with Admin Configuration

| Goal | Configure first |
|------|-----------------|
| Read a content list | [Content Types](../cms/content/content-types.md) and [Content Folders](../cms/content/contents-folders-list.md) |
| Send email | [k.mail](../api/mail/index.md) |
| Start a payment | [Payment Integration](../cms/settings/integrations/payment.md) and [k.payment](../api/payment/index.md) |
| Call an external service | [Open APIs](../cms/development/openapis.md) or [k.net.httpClient](../api/net/httpclient.md) |
| Store business data | [Database](../cms/database/index.md) and [k.DB](../api/database/index.md) |

## Debugging Recommendations

1. Return a fixed value first to confirm that the route is accessible.
2. Connect the real data query next.
3. Return an explicit status code and JSON body for each error path.
4. Finally, call the API from the page and inspect the browser Network panel.
