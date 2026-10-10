# k.logger

> 日志记录与查询

## 概述

`k.logger` 提供日志记录功能，支持五种日志级别：调试（debug）、信息（information）、警告（warning）、错误（error）、严重（critical）。每个日志方法支持两种调用方式：直接记录消息，或指定分类后记录消息。

::: warning 重要：不要使用 console
Kooboo 服务端不支持 `console.log` / `console.error` 等 `console.*` 方法。调试和信息输出必须使用 `k.logger`。
:::

## 记录日志

记录一条日志，支持以下五种级别：

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

**参数：**


| 参数     | 类型     | 必填 | 说明                           |
| -------- | -------- | ---- | ------------------------------ |
| category | `string` | 否   | 日志分类                       |
| message  | `string` | 是   | 日志信息（对象需转换为字符串） |


当只传一个参数时，该参数为 `message`，日志分类为空。

**返回：** `void`

::: tip 提示

- `message` 参数类型为 `string`，如果需要记录对象，请先使用 `JSON.stringify()` 转换为字符串
- 当站点关闭代码日志，或日志级别低于站点配置的最低级别时，该日志会被忽略
- 写入的日志可在 Kooboo 后台查看，路径：**开发 → 代码日志**
:::

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


| 参数      | 类型     | 必填 | 说明                                 |
| --------- | -------- | ---- | ------------------------------------ |
| category  | `string` | 否   | 精确匹配的日志分类                   |
| level     | `string` | 否   | 精确匹配的日志级别（如 `Error`、`Warning`） |
| keyword   | `string` | 否   | 在分类或消息中进行区分大小写的包含匹配 |
| traceId   | `string` | 否   | 精确匹配的追踪 ID                    |
| pageIndex | `number` | 否   | 页码（从 1 开始），默认 `1`           |
| pageSize  | `number` | 否   | 每页条数，默认 `20`                   |
| startDate | `Date`   | 否   | 开始时间                             |
| endDate   | `Date`   | 否   | 结束时间                             |


**返回：** `CodeLogResult`

```ts
{
    list: CodeLog[]   // 日志列表
    totalCount: number // 总条数
    pageNr: number     // 当前页码
    pageSize: number  // 每页条数
    totalPages: number // 总页数
}
```

## 常见错误

### ❌ 错误：使用 console.log

```ts
// ❌ Kooboo 服务端不支持 console
k.api.get(() => {
    console.log('debug info')  // 不生效
    return { success: true }
})
```

### ✅ 正确：使用 k.logger

```ts
// ✅ 正确
k.api.get(() => {
    k.logger.debug('Store.Order', 'debug info')
    return { success: true }
})
```

### ❌ 错误：直接记录对象

```ts
// ❌ message 必须是字符串
k.logger.error('Store.Order', { id: 1, message: 'error' })  // 类型错误
```

### ✅ 正确：序列化后再记录

```ts
// ✅
k.logger.error('Store.Order', JSON.stringify({ id: 1, message: 'error' }))
```

## 相关文档

- [Security](security.md) — 安全工具
- Kooboo 后台：**开发 → 代码日志** — 查看线上日志
