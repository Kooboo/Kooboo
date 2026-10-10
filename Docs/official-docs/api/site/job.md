# k.site.runJob

> Corresponding backend [Develop -> Scheduled Jobs](../../cms/development/jobs.md)

## Overview

Site **Scheduled Task (SiteJob)** configures name, schedule and **Inline KScript** in the background. The runtime is executed on the server side by `JobService` without going through the frontend HTTP.

The script side only provides **trigger** and (some versions) **status query**; please complete the addition, deletion and modification of tasks in the background.

::: tip configure the background first
After **adding a task** on [Scheduled Jobs](../../cms/development/jobs.md) and saving it, you can call `k.site.runJob` by **name** in other codes.
:::

## k.site.runJob(name)

Execute the specified task asynchronously once (equivalent to list **Run**, the log description is `Manual trigger from kscript`).

| Parameter | Type | Required | Description |
|------|------|------|------|
| `name` | `string` | yes | Task **name** or task ID (GUID string) |

**Returns:** `void`.

```typescript
k.site.runJob("nightly_sync")
```

Throws `Job not found` if the task does not exist.

::: warning Relationship with the scheduler
`runJob` **does not** replace periodic scheduling; recurring tasks are still automatically executed according to the background **start time** and **interval**. Manual running can be used for debugging when the site is not published.
:::

## execution environment

- Uses the current site's server-side execution context.
- Code comes from `SiteJob.Code` field (backend Monaco save), executed with **Module** script type, **Long Task** option.
- You can use the same `k.*` API as other server-side codes (`k.DB`, `k.content`, `k.mail`, etc.).

The execution results are written to the center **JobLog** (background **Completed** / **Failed** tab).

## Difference from k.site.codes

| | **SiteJob (scheduled task)** | **k.site.codes** |
|--|--------------------------|------------------|
| Configuration | Development → Scheduled tasks | Development → Code |
| trigger | Scheduler / `runJob` | HTTP routing, events, page references |
| code storage | `SiteJob.Code` inline | `Code` Resources `body` |

If you want to reuse the same piece of logic, you can place the public function in Code and call it by convention in the task script (you need to add `import` yourself or inline it. Kooboo tasks do not automatically bind Code Id by default).

## Module tasks

The **task** and events of `module.config` in the module package are handled by `k.module` / module scheduling, not `k.site.runJob`. See [k.module](../module/index.md).

## Related

- [Scheduled Jobs（CMS）](../../cms/development/jobs.md)
- [k.site](./index.md)
- [k.site.codes](./code.md)
