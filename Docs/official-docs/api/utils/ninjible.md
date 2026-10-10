# k.utils.ninjible

> Site import and Lighthouse comparison task (Ninjible)

## Overview

`k.utils.ninjible` Create a remote site migration/optimization task and query the progress.

## createTask()

Create a complete task (including optimization process).

| Parameter | Type | Required | Description |
|------|------|------|------|
| `url` | `string` | Yes | Old site URL to migrate/optimize |
| `rootDomain` | `string` | Yes | New site root domain |
| `orgName` | `string` | Yes | Organization name used to determine task ownership |

**Returns:** `string`, the task Id.

```ts
const taskId = k.utils.ninjible.createTask(oldUrl, rootDomain, orgName)
```

## createImportTask()

Import sites only.

| Parameter | Type | Required | Description |
|------|------|------|------|
| `url` | `string` | Yes | Old site URL to import |
| `rootDomain` | `string` | Yes | New site root domain |

**Returns:** `string`, the task Id.

```ts
const taskId = k.utils.ninjible.createImportTask("https://old.example.com", "new.example.com")
```

## getStatus()

Press `taskId` to get `NinjibleProgress`: `isDone`, `percent`, `oldUrl`, `newUrl`, Lighthouse scores, and more.

| Parameter | Type | Required | Description |
|------|------|------|------|
| `taskId` | `string` | Yes | Task Id returned by `createTask()` or `createImportTask()` |

**Returns:** `NinjibleProgress \| null`.

```ts
const status = k.utils.ninjible.getStatus(taskId)
return {
    percent: status?.percent,
    done: status?.isDone,
    newUrl: status?.newUrl
}
```

## Related Docs

- [k.google](../google/)
