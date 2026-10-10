# k.logger

> 日志记录与查询

## 概述

`k.logger` 提供日志记录功能，支持五种日志级别：调试（debug）、信息（information）、警告（warning）、错误（error）、严重（critical）。每个日志方法支持两种调用方式：直接记录消息，或指定分类后记录消息。

## 记录日志

记录一条日志，支持以下五种级别：

::: tip 提示
- `message` 参数类型为 `string`，如果需要记录对象，请先使用 `JSON.stringify()` 转换为字符串
- 写入的日志可在 Kooboo 后台查看，路径：**开发 → 代码日志**
:::

```ts
// 调试级别
k.logger.debug('Store.Order', 'Order xxx created')

// 信息级别
k.logger.information('Store.Order', 'Order xxx created')

// 警告级别
k.logger.warning('Store.Order', 'Order xxx not set create date!')

// 错误级别
k.logger.error('Store.Order', 'Order xxx not found!')

// 严重级别
k.logger.critical('Application', 'Application crash!')

// 记录对象时需要转换为字符串
k.logger.error('Store.Order', JSON.stringify({ id: 1, message: 'Order not found' }))
```

**参数：**

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| category | `string` | 否 | 日志分类 |
| message | `string` | 是 | 日志信息（对象需转换为字符串） |

当只传一个参数时，该参数为 `message`，日志分类为空。

**返回：** `void`

## query()

查询日志。

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

**参数：**

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| category | `string` | 是 | 日志分类 |
| level | `string` | 是 | 日志级别（如 `Error`、`Warning` 等） |
| keyword | `string` | 是 | 关键字 |
| traceId | `string` | 是 | 追踪 ID |
| pageIndex | `number` | 是 | 页码（从 1 开始） |
| pageSize | `number` | 是 | 每页条数 |
| startDate | `Date` | 否 | 开始时间 |
| endDate | `Date` | 否 | 结束时间 |

**返回：** `CodeLogResult`

```ts
{
    list: CodeLog[]   // 日志列表
    total: number     // 总条数
    pageIndex: number // 当前页码
    pageSize: number  // 每页条数
    pageCount: number // 总页数
}
```
