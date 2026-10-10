# Commerce Loyalty

## info()

获取顾客的会员积分信息。

```ts
k.api.get(() => {
    const customerId = k.commerce.customer.currentId || ''
    if (!customerId) return { authenticated: false, loyalty: null }
    return k.commerce.loyalty.info(customerId)
})
```
**参数：** `customerId: string` | **返回：** `LoyaltyInfo | null`

## earnPoints()

赚取积分。

```ts
k.api.post(() => {
    const customerId = k.commerce.customer.currentId || ''
    if (!customerId) return { success: false, error: 'login required' }
    k.commerce.loyalty.earnPoints(customerId, 100, '购物奖励')
    return { success: true }
})
```
**参数：**

| 参数 | 类型 | 说明 |
|------|------|------|
| customerId | `string` | 顾客 ID |
| points | `number` | 积分数量 |
| description | `string` | 描述（可选） |

## redeemPoints()

消费积分。

```ts
k.api.post(() => {
    const customerId = k.commerce.customer.currentId || ''
    if (!customerId) return { success: false, error: 'login required' }
    k.commerce.loyalty.redeemPoints(customerId, 50, '积分兑换')
    return { success: true }
})
```
**参数：**

| 参数 | 类型 | 说明 |
|------|------|------|
| customerId | `string` | 顾客 ID |
| points | `number` | 积分数量 |
| description | `string` | 描述（可选） |

## points()

获取顾客的积分记录。

```ts
k.api.get(() => {
    const customerId = k.commerce.customer.currentId || ''
    if (!customerId) return { authenticated: false, list: [], count: 0 }
    return k.commerce.loyalty.points(customerId, { pageIndex: 1, pageSize: 20 })
})
```
**参数：**

| 参数 | 类型 | 说明 |
|------|------|------|
| customerId | `string` | 顾客 ID |
| query.pageIndex | `number` | 页码（可选，默认 1） |
| query.pageSize | `number` | 每页条数（可选，默认 10） |

**返回：** `{ list: CustomerPointModel[], count: number, pageIndex: number, pageSize: number }`

## changeMembership()

变更会员等级。

```ts
k.api.post(() => {
    const customerId = k.commerce.customer.currentId || ''
    if (!customerId) return { success: false, error: 'login required' }
    const memberships = k.commerce.membership.list()
    if (memberships && memberships.length > 0) {
        k.commerce.loyalty.changeMembership(customerId, memberships[0].id)
    }
})
```

## renewMembership()

续费会员。

```ts
k.api.post(() => {
    const customerId = k.commerce.customer.currentId || ''
    if (!customerId) return { success: false, error: 'login required' }
    k.commerce.loyalty.renewMembership(customerId)
})
```

这些方法虽然接收 `customerId`，但不会替调用方执行授权校验。站点前台接口必须从 `k.commerce.customer.currentId` 得到它；后台接口必须执行相应的管理员授权。

## memberships()

获取所有会员等级列表。

```ts
k.api.get(() => {
    const memberships = k.commerce.loyalty.memberships()
    return { count: memberships.length }
})
```

## 类型提示

如果你不确定 LoyaltyInfo / Membership 的具体字段，用 [Loyalty Types](types/loyalty.md) 获取完整类型定义。

## 边界说明

- 会员等级列表（membership.list）→ [Membership Guide](membership.md)
- 本 guide 不展开会员等级管理细节
