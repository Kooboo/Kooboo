# discount

> 折扣优惠

## 概述

`k.commerce.discount` 提供折扣优惠券相关操作，用于获取和管理优惠券信息。

## list()

获取优惠折扣列表。

**参数**：无。

```ts
k.api.get(() => {
    const discounts = k.commerce.discount.list()
    return { count: discounts.length, items: discounts.slice(0, 3) }
})
```

**返回：** `Discount[]`

## 结构

### Discount 属性

| 属性 | 说明 | 类型 |
|------|------|------|
| id | 优惠折扣ID | `string` |
| code | 优惠折扣代码 | `string` |
| title | 优惠折扣名称 | `string` |
| startDate | 优惠折扣开始时间 | `string` |
| endDate | 优惠折扣结束时间 | `string` |
| condition | 优惠折扣条件 | [`DiscountCondition`](#discountcondition-属性) |
| method | 优惠折扣方法 | `"DiscountCode"` \| `"AutomaticDiscount"` |
| type | 优惠折扣类型 | `"ProductAmountOff"` \| `"OrderAmountOff"` \| `"FreeShipping"` |
| value | 优惠折扣值 | `number` |
| isPercent | 是否为百分比折扣 | `boolean` |
| priority | 优惠折扣优先级 | `number` |
| isExclusion | 是否互斥 | `boolean` |
| codeUsageLimit | 代码使用限制 | `number \| null` |
| createdAt | 创建时间 | `string` |
| updatedAt | 更新时间 | `string` |

### DiscountCondition 属性

| 属性 | 说明 | 类型 |
|------|------|------|
| isAny | 任一条件满足即可，否则需满足所有条件 | `boolean` |
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
