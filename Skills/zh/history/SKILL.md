---
name: history
description: "Inspect Kooboo CMS history, list and filter change records, view object versions, compare revisions, undo selected changes, restore a website to a historical point, or check out a historical state as another website. Use for audit, recovery, rollback, revision comparison, and historical checkout tasks involving k.history or native History functions. / 用于查看、比较、撤销、恢复或检出 Kooboo CMS 历史记录。"
---

# Kooboo History

使用当前网站 Context 中的 CMS 历史记录完成审计、版本比较和恢复。KScript 入口是 `k.history`；Native Function 使用下表列出的 History 名称，不需要 `site` 前缀。

## 1. 安全工作流

1. 先通过 `function_discover` 和 `use_function` 选择下表中的 History Function，再用 `list_recent_history` 或 `list_history` 找到候选记录。不得在读取 skill 或 function schema 之前并行调用目标 Function。
2. 用 `get_history_item` 确认对象、操作类型、用户和时间。
3. 用 `list_history_versions` 或 `list_object_history_versions` 查看同一对象的全部版本。
4. 在任何撤销或恢复前调用比较 Function，确认实际差异。
5. 优先选择影响范围最小的动作：单条 undo 优于批量 undo，批量 undo 优于整站 restore。
6. `undo_*`、`restore_*` 和 `checkout_history` 都需要审批。审批前说明目标 ID、影响范围和预期结果。
7. 写入完成后重新调用 `list_recent_history`，验证新历史记录和最终状态。

如果用户只是想查看过去状态，优先使用读取、比较或 `checkout_history`，不要直接恢复当前网站。

## 2. Native Function 合同

| Function | 主要输入 | 返回 | 审批 |
|---|---|---|---|
| `list_recent_history` | `top`，默认 100，最大 200 | 历史分页结果 | 否 |
| `list_history` | 分页及可选过滤条件 | 历史分页结果 | 否 |
| `get_history_item` | `logId` | 单条历史记录 | 否 |
| `list_history_versions` | `logId` | 同一对象的版本数组 | 否 |
| `list_object_history_versions` | `nameOrId`，以及 `storeName` 或 `tableName` | 对象版本数组 | 否 |
| `compare_history_with_latest` | `logId` | 与最新版本的比较结果 | 否 |
| `compare_history_versions` | `firstLogId`、`secondLogId` | 两个版本的比较结果 | 否 |
| `undo_history_item` | `logId` | `true` | 是 |
| `undo_history_items` | 非空 `logIds`，最多 1000 条 | `true` | 是 |
| `restore_history_to_log` | `logId` | 实际恢复到的 log ID | 是 |
| `restore_history_to_time` | `time` | 实际恢复到的 log ID | 是 |
| `checkout_history` | `logId`、`siteName`、`fullDomain`、`published` | 新网站信息 | 是 |

所有 Function 都在调用时的单一网站 Context 中工作。输入 schema 还支持可选 `sandbox`；只有任务明确要求操作 Sandbox 时才传入，并在写入前先确认 Sandbox 名称。

## 3. 列出和过滤历史

快速读取最近记录：

```json
{
  "top": 100
}
```

`list_history` 支持：

- `pageNr`：从 1 开始。
- `pageSize`：默认 50，最大 200。
- `actionType`：CMS 操作类型，例如 `Add`、`Update` 或 `Delete`。
- `storeName`：CMS repository 名称；使用 `Table` 可选择所有动态表记录。
- `tableName`：动态表名。
- `name`：对象显示名称的包含匹配，不区分大小写。
- `userId`：执行变更的用户 ID。
- `startDate`、`endDate`：包含边界的日期时间过滤。

示例：

```json
{
  "pageNr": 1,
  "pageSize": 50,
  "storeName": "View",
  "actionType": "Update",
  "name": "home"
}
```

读取结果中的 `id` 是后续 get、versions、compare、undo、restore 和 checkout 使用的 `logId`。不要使用 `storeNameHash`、`tableNameHash` 或 `keyHash` 作为用户输入。

## 4. 查找对象版本

已知任意一条历史记录时，优先使用：

```json
{
  "logId": 123
}
```

已知对象名称或 ID 时使用 `list_object_history_versions`，并且必须二选一：

- CMS 对象：传 `nameOrId` 和 `storeName`。
- 动态表对象：传 `nameOrId` 和 `tableName`。

不要同时传 `storeName` 和 `tableName`，也不要两个都省略。

```json
{
  "nameOrId": "home",
  "storeName": "View"
}
```

对于已经删除且不是 NameAsId 的 CMS 对象，名称可能无法重新解析，此时必须使用对象 GUID。NameAsId 对象可以从名称稳定计算 ID。

## 5. 比较版本

用 `compare_history_with_latest` 检查一个历史版本与同一对象最新记录之间的差异：

```json
{
  "logId": 123
}
```

用 `compare_history_versions` 比较同一对象的两个历史版本：

```json
{
  "firstLogId": 120,
  "secondLogId": 123
}
```

两个 ID 必须属于同一 CMS 对象或同一动态表对象。字符串对象返回 `source1` 和 `source2`；图片比较返回版本 URL。比较结果为空内容时同时检查该版本是否是 Delete 操作。

## 6. Undo

`undo_history_item` 撤销一条记录所表示的变更，并创建新的历史记录：

```json
{
  "logId": 123
}
```

`undo_history_items` 在任何修改发生前验证全部 ID，最多接受 1000 条，重复 ID 会去重：

```json
{
  "logIds": [123, 122, 121]
}
```

批量撤销时明确说明 ID 顺序和对象范围。操作后重新列出最近历史，不要把原记录被撤销理解成原历史记录被删除。

## 7. Restore

`restore_history_to_log` 把当前网站恢复到指定 log ID 时的状态。实现会撤销该 ID 之后的全部记录；超过 99,999 条后续记录时拒绝执行。

`restore_history_to_time` 先查找指定时间点或之前的最后一条记录，再执行相同恢复。使用带时区的 ISO 8601 时间，避免本地时区歧义：

```json
{
  "time": "2026-08-17T10:00:00+08:00"
}
```

Restore 是整站范围操作。审批前必须先列出目标点之后的记录，说明会受影响的对象；如果只需查看或验证历史状态，改用 checkout。

## 8. Checkout

`checkout_history` 创建一个普通的新网站，并将其内容恢复到历史 log ID。它不是隐藏 Sandbox，也不会修改当前网站。

```json
{
  "logId": 123,
  "siteName": "history-preview",
  "fullDomain": "history-preview.local",
  "published": false
}
```

规则：

- `siteName` 在当前组织中必须可用。
- `fullDomain` 不能被其他 binding 使用。
- 当前用户的组织必须与来源网站一致。
- 超过 99,999 条后续记录时拒绝 checkout。
- 创建或历史恢复失败时，当前实现会删除本次创建的新网站。
- 默认优先使用 `published: false`；只有用户明确要求立即发布时才设为 `true`。

## 9. KScript

KScript 使用 `k.history`：

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

写入方法：

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

不要让用户提供 hash 字段。对象版本查询只接受 `nameOrId` 加 `storeName` 或 `tableName`。

## 10. 失败处理

- ID 不存在时重新 list/get，确认网站 Context 和 ID，不要改用猜测值。
- compare 报对象不一致时，分别读取两个 ID，确认 `storeName`/`tableName` 和对象标识。
- 名称查询失败时先检查 repository/table 名称；删除对象再尝试其 GUID。
- restore time 找不到记录时停止，不要自动改成最早记录。
- undo、restore 或 checkout 失败后重新读取历史和网站列表，报告实际状态，不要假设事务已经完成或完全回滚。
- 运行环境无法验证时，只报告静态结果，并明确标记未执行写入操作。
