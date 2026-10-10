# loyalty

> 会员积分

## 概述

`k.commerce.loyalty` 提供会员积分相关操作，包括积分获取、兑换、会员等级管理等。

::: tip 会员创建
会员需要手动到 Kooboo 后台创建：**电商 → 会员（默认在隐藏目录中）→ 创建会员**
:::

## info()

获取顾客的会员积分信息。

```ts
k.api.get(() => {
    const customer = k.commerce.customer.getByEmail('test@example.com')
    return k.commerce.loyalty.info(customer.id)
})
```

**参数：**

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| customerId | `string` | 是 | 顾客ID |

**返回：** `LoyaltyInfo`

## changeMembership()

变更会员等级。

```ts
k.api.post(() => {
    const customer = k.commerce.customer.getByEmail('test@example.com')
    const memberships = k.commerce.membership.list()
    if (memberships && memberships.length > 0) {
        k.commerce.loyalty.changeMembership(customer.id, memberships[0].id)
    }
    return 'change success'
})
```

**参数：**

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| customerId | `string` | 是 | 顾客ID |
| membershipId | `string` | 是 | 会员ID |

**返回：** `void`

## earnPoints()

赚取积分。

```ts
k.api.post(() => {
    const customer = k.commerce.customer.getByEmail('test@example.com')
    k.commerce.loyalty.earnPoints(customer.id, 100, '购物奖励')
    return 'earn success'
})
```

**参数：**

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| customerId | `string` | 是 | 顾客ID |
| points | `number` | 是 | 赚取的积分数量 |
| description | `string` | 否 | 积分描述 |

**返回：** `void`

## redeemPoints()

消费积分。

```ts
k.api.post(() => {
    const customer = k.commerce.customer.getByEmail('test@example.com')
    k.commerce.loyalty.redeemPoints(customer.id, 50, '积分兑换')
    return 'redeem success'
})
```

**参数：**

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| customerId | `string` | 是 | 顾客ID |
| points | `number` | 是 | 消费的积分数量 |
| description | `string` | 否 | 积分描述 |

**返回：** `void`

## points()

获取顾客的积分记录。

```ts
k.api.get(() => {
    const customer = k.commerce.customer.getByEmail('test@example.com')
    return k.commerce.loyalty.points(customer.id, {
        pageIndex: 1,
        pageSize: 20
    })
})
```

**参数：**

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| customerId | `string` | 是 | 顾客ID |
| query | `object` | 否 | 查询参数 |
| query.pageIndex | `number` | 否 | 页码，默认 1 |
| query.pageSize | `number` | 否 | 每页条数，默认 10 |

**返回：**

```ts
{
    list: CustomerPointModel[];
    count: number;
    pageIndex: number;
    pageSize: number;
}
```

## memberships()

获取会员信息列表。

**参数**：无。

```ts
k.api.get(() => {
    const memberships = k.commerce.loyalty.memberships()
    return { count: memberships.length, items: memberships }
})
```

**返回：** `Membership[]`

## renewMembership()

续费会员。

```ts
k.api.post(() => {
    const customer = k.commerce.customer.getByEmail('test@example.com')
    k.commerce.loyalty.renewMembership(customer.id)
    return 'renew success'
})
```

**参数：**

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| customerId | `string` | 是 | 顾客ID |

**返回：** `void`

## 结构

### LoyaltyInfo 属性

| 属性 | 说明 | 类型 |
|------|------|------|
| Points | 会员积分 | `number` |
| MembershipStatus | 会员状态 | [`MembershipStatus`](#membershipstatus-属性) |

### MembershipStatus 属性

| 属性 | 说明 | 类型 |
|------|------|------|
| Membership | 会员信息 | [`Membership`](#membership-属性) |
| StartedAt | 会员开始时间 | `string` |
| EndAt | 会员结束时间 | `string` |
| Active | 会员是否有效 | `boolean` |

### Membership 属性

| 属性 | 说明 | 类型 |
|------|------|------|
| id | 会员ID | `string` |
| name | 会员名称 | `string` |
| description | 会员描述 | `string` |
| price | 会员价格 | `number` |
| duration | 会员时长 | `number` |
| durationUnit | 会员时长单位 | `"Year"` \| `"Month"` \| `"Day"` \| `"Week"` |
| priority | 会员优先级 | `number` |
| condition | 升级条件 | [`Condition[]`](#condition-属性) |
| allowAutoUpgrade | 是否允许自动升级 | `boolean` |
| allowPurchase | 是否允许购买 | `boolean` |
| customData | 自定义数据 | `{key: value}[] \| null` |
| createdAt | 创建时间 | `string` |
| updatedAt | 更新时间 | `string` |

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

### CustomerPointModel 属性

| 属性 | 说明 | 类型 |
|------|------|------|
| customerId | 客户ID | `string` |
| points | 积分 | `number` |
| createdAt | 创建时间 | `string` |
| type | 类型 | `string` |
| description | 描述 | `string` |
| key | 键 | `string` |
| disabled | 是否禁用 | `boolean` |

## 相关文档

- [k.commerce](./commerce.md) - 电商模块概述
- [customer](./customer.md) - 客户管理
- [membership](./membership.md) - 会员等级
