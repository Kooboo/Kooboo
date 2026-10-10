# k.logger

> Logging and querying

## Overview

`k.logger` provides logging with five levels: debug, information, warning, error, and critical. Each method supports two calling forms: log a message directly, or specify a category before the message.

::: warning Important: do not use console
The Kooboo server does not support `console.log`, `console.error`, or other `console.*` methods. Use `k.logger` for debug and informational output.
:::

## Logging

Writes a log entry at one of the five supported levels:

```ts
// 调试级别
k.logger.debug('Store.Order', 'Order xxx created')

// 信息级别
k.logger.information('Store.Order', 'Order xxx created')

// 警告级别
k.logger.warning('Store.Order', 'Order not set create date!')

// 错误级别
k.logger.error('Store.Order', 'Order not found!')

// 严重级别
k.logger.critical('Application', 'Application crash!')

// 记录对象时需要转换为字符串
k.logger.error('Store.Order', JSON.stringify({ id: 1, message: 'Order not found' }))
```

**Parameters:**

| Parameter | Type | Required | Description |
| --- | --- | --- | --- |
| category | `string` | No | Log category |
| message | `string` | Yes | Log message (objects must be converted to strings) |

When only one argument is supplied, it is the `message` and the category is empty.

**Returns:** `void`

::: tip Note

- `message` must be a `string`; use `JSON.stringify()` first when logging an object.
- A log is ignored when site code logging is disabled or its level is below the configured minimum level.
- Logs can be viewed in the Kooboo admin UI at **Development → Code Logs**.
:::

## query()

Queries logs.

```ts
k.api.get("query", () => {
    return k.logger.query({
        startDate: '2021-10-27 12:00',
        endDate: '2021-10-27 17:25',
        category: 'user',
        level: 'Error',
        pageIndex: 1,
        pageSize: 100,
        keyword: 'exception',
        traceId: '0HMCP7T6K7BFS:00000002'
    })
})
```

**Parameters:**

| Parameter | Type | Required | Description |
| --- | --- | --- | --- |
| category | `string` | No | Exact log category filter |
| level | `string` | No | Exact log level filter (such as `Error` or `Warning`) |
| keyword | `string` | No | Case-sensitive substring matched against category or message |
| traceId | `string` | No | Exact trace ID filter |
| pageIndex | `number` | No | Page number, starting at 1; defaults to `1` |
| pageSize | `number` | No | Items per page; defaults to `20` |
| startDate | `Date` | No | Start time |
| endDate | `Date` | No | End time |

**Returns:** `CodeLogResult`

```ts
{
    list: CodeLog[]   // 日志列表
    totalCount: number // 总条数
    pageNr: number     // 当前页码
    pageSize: number  // 每页条数
    totalPages: number // 总页数
}
```

## Common mistakes

### ❌ Incorrect: use console.log

```ts
// ❌ Kooboo 服务端不支持 console
k.api.get(() => {
    console.log('debug info')  // 不生效
    return { success: true }
})
```

### ✅ Correct: use k.logger

```ts
// ✅ 正确
k.api.get(() => {
    k.logger.debug('Store.Order', 'debug info')
    return { success: true }
})
```

### ❌ Incorrect: log an object directly

```ts
// ❌ message 必须是字符串
k.logger.error('Store.Order', { id: 1, message: 'error' })  // 类型错误
```

### ✅ Correct: serialize before logging

```ts
// ✅
k.logger.error('Store.Order', JSON.stringify({ id: 1, message: 'error' }))
```

## Related documents

- [Security](security.md) — Security tools
- Kooboo admin UI: **Development → Code Logs** — View production logs
