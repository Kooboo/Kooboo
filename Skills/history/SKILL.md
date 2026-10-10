---
name: history
description: "Inspect Kooboo CMS history, list and filter change records, view object versions, compare revisions, undo selected changes, restore a website to a historical point, or check out a historical state as another website. Use for audit, recovery, rollback, revision comparison, and historical checkout tasks involving k.history or native History functions."
---

# Kooboo History

Use the CMS history in the current site Context for audits, revision comparison, and recovery. The KScript entry point is `k.history`; Native Functions use the History names listed below and do not require a `site` prefix.

## 1. Safe workflow

1. Use `function_discover` and `use_function` to select a History Function from the table below, then use `list_recent_history` or `list_history` to find candidate records. Do not call the target Function in parallel before reading the skill or function schema.
2. Use `get_history_item` to confirm the object, operation type, user, and time.
3. Use `list_history_versions` or `list_object_history_versions` to view all versions of the same object.
4. Call a comparison Function before any undo or restore to confirm the actual difference.
5. Prefer the action with the smallest impact: a single undo over a batch undo, and a batch undo over a site-wide restore.
6. `undo_*`, `restore_*`, and `checkout_history` all require approval. Before approval, explain the target ID, impact scope, and expected result.
7. After a write completes, call `list_recent_history` again to verify the new history record and final state.

If the user only wants to inspect a past state, prefer reading, comparison, or `checkout_history`; do not directly restore the current site.

## 2. Native Function contract

| Function | Main inputs | Return | Approval |
|---|---|---|---|
| `list_recent_history` | `top`, default 100, maximum 200 | Paginated history result | No |
| `list_history` | Pagination and optional filters | Paginated history result | No |
| `get_history_item` | `logId` | Single history record | No |
| `list_history_versions` | `logId` | Version array for the same object | No |
| `list_object_history_versions` | `nameOrId`, plus `storeName` or `tableName` | Object version array | No |
| `compare_history_with_latest` | `logId` | Comparison with the latest version | No |
| `compare_history_versions` | `firstLogId`, `secondLogId` | Comparison of two versions | No |
| `undo_history_item` | `logId` | `true` | Yes |
| `undo_history_items` | Non-empty `logIds`, maximum 1000 | `true` | Yes |
| `restore_history_to_log` | `logId` | The log ID actually restored to | Yes |
| `restore_history_to_time` | `time` | The log ID actually restored to | Yes |
| `checkout_history` | `logId`, `siteName`, `fullDomain`, `published` | New site information | Yes |

All Functions operate in the single site Context active at call time. The input schema also supports an optional `sandbox`; pass it only when the task explicitly requires operating on a Sandbox, and confirm the Sandbox name before writing.

## 3. List and filter history

Quickly read recent records:

```json
{
  "top": 100
}
```

`list_history` supports:

- `pageNr`: Starts at 1.
- `pageSize`: Defaults to 50, maximum 200.
- `actionType`: CMS operation type, such as `Add`, `Update`, or `Delete`.
- `storeName`: CMS repository name; use `Table` to select all dynamic-table records.
- `tableName`: Dynamic table name.
- `name`: Case-insensitive contains match on the object's display name.
- `userId`: ID of the user who made the change.
- `startDate`, `endDate`: Date-time filters including the boundaries.

Example:

```json
{
  "pageNr": 1,
  "pageSize": 50,
  "storeName": "View",
  "actionType": "Update",
  "name": "home"
}
```

The `id` in the result is the `logId` used by later get, versions, compare, undo, restore, and checkout calls. Do not use `storeNameHash`, `tableNameHash`, or `keyHash` as user input.

## 4. Find object versions

When any history record is known, prefer:

```json
{
  "logId": 123
}
```

When the object name or ID is known, use `list_object_history_versions` and choose exactly one:

- CMS object: pass `nameOrId` and `storeName`.
- Dynamic-table object: pass `nameOrId` and `tableName`.

Do not pass both `storeName` and `tableName`, and do not omit both.

```json
{
  "nameOrId": "home",
  "storeName": "View"
}
```

For a deleted CMS object that is not NameAsId, the name may no longer resolve; use the object GUID instead. A NameAsId object can have its ID calculated stably from its name.

## 5. Compare versions

Use `compare_history_with_latest` to inspect the difference between a historical version and the latest record for the same object:

```json
{
  "logId": 123
}
```

Use `compare_history_versions` to compare two historical versions of the same object:

```json
{
  "firstLogId": 120,
  "secondLogId": 123
}
```

The two IDs must belong to the same CMS object or the same dynamic-table object. String objects return `source1` and `source2`; image comparisons return the version URLs. When the comparison result has empty content, also check whether the version represents a Delete operation.

## 6. Undo

`undo_history_item` undoes the change represented by one record and creates a new history record:

```json
{
  "logId": 123
}
```

`undo_history_items` validates all IDs before any modification, accepts at most 1000, and removes duplicate IDs:

```json
{
  "logIds": [123, 122, 121]
}
```

For a batch undo, state the ID order and object scope explicitly. List recent history again afterward; do not interpret an undone record as the original history record being deleted.

## 7. Restore

`restore_history_to_log` restores the current site to the state at the specified log ID. The implementation undoes all records after that ID and rejects the operation when there are more than 99,999 later records.

`restore_history_to_time` first finds the last record at or before the specified time, then performs the same restore. Use a time-zone-aware ISO 8601 time to avoid local-time ambiguity:

```json
{
  "time": "2026-08-17T10:00:00+08:00"
}
```

Restore is site-wide. Before approval, list the records after the target point and explain the affected objects; if the goal is only to inspect or validate a historical state, use checkout instead.

## 8. Checkout

`checkout_history` creates a normal new site and restores its content to the historical log ID. It is not a hidden Sandbox and does not modify the current site.

```json
{
  "logId": 123,
  "siteName": "history-preview",
  "fullDomain": "history-preview.local",
  "published": false
}
```

Rules:

- `siteName` must be available in the current organization.
- `fullDomain` must not be used by another binding.
- The current user's organization must match the source site.
- Checkout is rejected when there are more than 99,999 later records.
- If creation or historical restore fails, the current implementation deletes the new site created for this operation.
- Prefer `published: false` by default; set it to `true` only when the user explicitly requests immediate publication.

## 9. KScript

KScript uses `k.history`:

```javascript
const recent = k.history.list(100);
const page = k.history.list({
  pageNr: 1,
  pageSize: 50,
  storeName: "View",
  actionType: "Update"
});
const item = k.history.get(123);
const versions = k.history.versions(123);
const namedVersions = k.history.versions("home", "View", null);
const latestDiff = k.history.compare(123);
const versionDiff = k.history.compare(120, 123);
```

Write methods:

```javascript
k.history.undo(123);
k.history.undoMany([123, 122]);
k.history.restoreTo(123);
k.history.restoreTo(new Date("2026-08-17T10:00:00+08:00"));
k.history.checkOut({
  logId: 123,
  siteName: "history-preview",
  fullDomain: "history-preview.local",
  published: false
});
```

Do not ask the user to provide hash fields. Object-version queries accept only `nameOrId` together with `storeName` or `tableName`.

## 10. Failure handling

- If an ID does not exist, list/get again to confirm the site Context and ID; do not substitute a guessed value.
- If compare reports inconsistent objects, read both IDs separately and confirm `storeName`/`tableName` and the object identifiers.
- If a name query fails, first check the repository/table name; after deletion, try the object's GUID.
- If restore time finds no record, stop; do not automatically switch to the earliest record.
- After undo, restore, or checkout fails, read history and the site list again and report the actual state; do not assume the transaction completed or fully rolled back.
- If the runtime cannot be verified, report only the static result and explicitly mark that no write operation was executed.
