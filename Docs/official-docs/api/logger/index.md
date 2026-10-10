# k.logger

> Logging and querying

## Overview

`k.logger` provides logging function and supports five log levels: debug, information, warning, error, and critical. Each log method supports two calling methods: logging messages directly, or logging messages after specifying a classification.

## logging

Record a log, supporting the following five levels:

::: tip tip
- The parameter type of `message` is `string`. If you need to record the object, please use `JSON.stringify()` to convert it to a string first.
- The written log can be viewed in the Kooboo background at the path: **Development → Code Log**
:::

```ts
// Debug level
k.logger.debug('Store.Order', 'Order xxx created')

// Information level
k.logger.information('Store.Order', 'Order xxx created')

// Warning level
k.logger.warning('Store.Order', 'Order xxx not set create date!')

// Error level
k.logger.error('Store.Order', 'Order xxx not found!')

// Critical level
k.logger.critical('Application', 'Application crash!')

// Convert objects to strings before logging them
k.logger.error('Store.Order', JSON.stringify({ id: 1, message: 'Order not found' }))
```

**parameter:**

| Parameter | Type | Required | Description |
|------|------|------|------|
| category | `string` | no | Log classification |
| message | `string` | yes | Log information (the object needs to be converted into a string) |

When only one parameter is passed, the parameter is `message` and the log category is empty.

**Return:** `void`

## query()

Query log.

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

**parameter:**

| Parameter | Type | Required | Description |
|------|------|------|------|
| category | `string` | yes | Log classification |
| level | `string` | yes | Log level (such as `Error`, `Warning`, etc.) |
| keyword | `string` | yes | Keywords |
| traceId | `string` | yes | Tracking ID |
| pageIndex | `number` | yes | Page number (starting from 1) |
| pageSize | `number` | yes | Number of items per page |
| startDate | `Date` | no | start time |
| endDate | `Date` | no | end time |

**Return:** `CodeLogResult`

```ts
{
    list: CodeLog[]   // Log entries
    total: number     // Total count
    pageIndex: number // Current page index
    pageSize: number  // Items per page
    pageCount: number // Total pages
}
```
