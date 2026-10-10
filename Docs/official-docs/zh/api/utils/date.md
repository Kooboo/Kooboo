# k.utils.date

> 日期时间与格式化

## 概述

`k.utils.date` 提供当前时间与 `DateTime` 运算、格式化。

## 属性

| 属性 | 说明 |
|------|------|
| `utcNow` | UTC 当前时间 |
| `now` | 本地当前时间 |

**类型：** `Date`。

## formate()

格式化日期（方法名为源码拼写 `formate`）。

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `dateTime` | `Date` | 是 | 要格式化的时间 |
| `formate` | `string` | 否 | 格式字符串；不传时使用 `yyyy-MM-dd HH:mm:ss` |

**返回：** `string`。

```ts
const s = k.utils.date.formate(k.utils.date.now, "yyyy-MM-dd")
const s2 = k.utils.date.formate(k.utils.date.now) // 默认格式
```

## addSeconds / addMinutes / addHours / addDays / addMonths / addYears

对给定 `DateTime` 做偏移，返回新时间。

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `dateTime` | `Date` | 是 | 基准时间 |
| `value` | `number` | 是 | 偏移量；`addMonths`、`addYears` 按整数处理 |

**返回：** `Date`，偏移后的新时间。

```ts
const tomorrow = k.utils.date.addDays(k.utils.date.now, 1)
const expiresAt = k.utils.date.addMinutes(k.utils.date.now, 10)
```

## 相关文档

- [k.utils 概述](./index.md)
