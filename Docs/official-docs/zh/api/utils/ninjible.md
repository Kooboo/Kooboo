# k.utils.ninjible

> 站点导入与 Lighthouse 对比任务（Ninjible）

## 概述

`k.utils.ninjible` 创建远程站点迁移/优化任务并查询进度。

## createTask()

创建完整任务（含优化流程）。

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `url` | `string` | 是 | 要迁移/优化的旧站点 URL |
| `rootDomain` | `string` | 是 | 新站点根域名 |
| `orgName` | `string` | 是 | 组织名称，用于确定任务归属 |

**返回：** `string`，任务 Id。

```ts
const taskId = k.utils.ninjible.createTask(oldUrl, rootDomain, orgName)
```

## createImportTask()

仅导入站点。

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `url` | `string` | 是 | 要导入的旧站点 URL |
| `rootDomain` | `string` | 是 | 新站点根域名 |

**返回：** `string`，任务 Id。

```ts
const taskId = k.utils.ninjible.createImportTask("https://old.example.com", "new.example.com")
```

## getStatus()

按 `taskId` 获取 `NinjibleProgress`：`isDone`、`percent`、`oldUrl`、`newUrl`、Lighthouse 分数等。

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `taskId` | `string` | 是 | `createTask()` 或 `createImportTask()` 返回的任务 Id |

**返回：** `NinjibleProgress \| null`。

```ts
const status = k.utils.ninjible.getStatus(taskId)
return {
    percent: status?.percent,
    done: status?.isDone,
    newUrl: status?.newUrl
}
```

## 相关文档

- [k.google](../google/)
