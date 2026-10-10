# shipping

> 配送方式

## 概述

`k.commerce.shipping` 提供配送方式相关操作，用于获取实体产品和数字产品的配送配置。

## list()

获取实体产品配送方式列表。

```ts
k.api.get(() => {
    const shippings = k.commerce.shipping.list()
    return { count: shippings.length, items: shippings }
})
```

**参数：**

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| options.country | `string` | 否 | 按国家筛选 |

**返回：** `ShippingItem[]`

## digitalList()

获取数字产品配送方式列表。

**参数**：无。

```ts
k.api.get(() => {
    const digitalShippings = k.commerce.shipping.digitalList()
    return { count: digitalShippings.length, items: digitalShippings }
})
```

**返回：** `DigitalShipping[]`

## 结构

### ShippingItem 属性

| 属性 | 说明 | 类型 |
|------|------|------|
| id | 配送方式ID | `string` |
| name | 配送方式名称 | `string` |
| description | 配送方式描述 | `string` |
| baseCost | 基础运费 | `number` |
| additionalCosts | 附加运费列表 | [`AdditionalCost[]`](#additionalcost-属性) |
| isDefault | 是否为默认配送方式 | `boolean` |
| countries | 支持的国家列表 | [`SupportCountry[]`](#supportcountry-属性) |
| currentCost | 当前运费 | `number` |
| code | 快递公司代码 | `string` |
| currency | 货币代码 | `string` |
| symbol | 货币符号 | `string` |

### DigitalShipping 属性

| 属性 | 说明 | 类型 |
|------|------|------|
| id | 配送方式ID | `string` |
| name | 配送方式名称 | `string` |
| description | 配送方式描述 | `string` |
| mailServerType | 邮件服务器类型 | `string` |
| koobooEmailAddress | Kooboo 邮箱地址 | `string` |
| customMailServer | 自定义邮件服务器 | [`MailServer`](./settings.md#mailserver-属性) |
| mailTemplate | 邮件模板 | `{ subject: string, body: string }` |
| isDefault | 是否为默认配送方式 | `boolean` |
| createdAt | 创建时间 | `Date` |
| updatedAt | 更新时间 | `Date` |

### AdditionalCost 属性

| 属性 | 说明 | 类型 |
|------|------|------|
| cost | 附加运费金额 | `number` |
| description | 附加运费描述 | `string` |
| condition | 附加运费条件 | [`Condition`](#condition-属性) |

### SupportCountry 属性

| 属性 | 说明 | 类型 |
|------|------|------|
| name | 国家代码 | `string` |
| display | 国家显示名称 | `string` |
| estimatedDaysOfArrival | 预计到达天数 | `number` |

### Condition 属性

| 属性 | 说明 | 类型 |
|------|------|------|
| isAny | 任一条件满足即可 | `boolean` |
| items | 条件项列表 | [`ConditionItem[]`](#conditionitem-属性) |

### ConditionItem 属性

| 属性 | 说明 | 类型 |
|------|------|------|
| option | 条件选项 | `string` |
| method | 条件方法 | `string` |
| value | 条件值 | `string` |

## 相关文档

- [k.commerce](./commerce.md) - 电商模块概述
- [cart](./cart.md) - 购物车
- [order](./order.md) - 订单管理
