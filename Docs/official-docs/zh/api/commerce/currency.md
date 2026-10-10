# currency

> 货币管理

## 概述

`k.commerce.currency` 提供货币相关操作，用于获取货币列表和汇率转换。

## default

获取默认货币代码。

```ts
k.api.get(() => {
    return k.commerce.currency.default
})
```

**返回：** `string` - 默认货币代码（如 "CNY"）

## list()

获取支持的货币列表。

**参数**：无。

```ts
k.api.get(() => {
    return k.commerce.currency.list()
})
```

**返回：** [`CurrencyItem[]`](#currencyitem-属性)

## convert()

货币汇率转换。

```ts
k.api.get(() => {
    const usdAmount = 2
    const cnyAmount = k.commerce.currency.convert("USD", "CNY", usdAmount)
    return { usdAmount, cnyAmount }
})
```

**参数：**

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| from | `string` | 是 | 源货币代码 |
| to | `string` | 是 | 目标货币代码 |
| value | `number` | 是 | 转换金额 |

**返回：** `number` - 转换后的金额

## 结构

### CurrencyItem 属性

| 属性 | 说明 | 类型 |
|------|------|------|
| name | 货币名称 | `string` |
| nameNative | 本地货币名称 | `string` |
| code | 货币代码 | `string` |
| symbol | 货币符号 | `string` |
| exchangeRate | 汇率 | `number` |

## 相关文档

- [k.commerce](./commerce.md) - 电商模块概述
- [settings](./settings.md) - 电商设置
