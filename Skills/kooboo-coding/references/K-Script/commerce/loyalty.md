# Commerce Loyalty

## info()

Get a customer's loyalty-point information.

```ts
k.api.get(() => {
    const customerId = k.commerce.customer.currentId || ''
    if (!customerId) return { authenticated: false, loyalty: null }
    return k.commerce.loyalty.info(customerId)
})
```
**Parameter:** `customerId: string` | **Returns:** `LoyaltyInfo | null`

## earnPoints()

Earn points.

```ts
k.api.post(() => {
    const customerId = k.commerce.customer.currentId || ''
    if (!customerId) return { success: false, error: 'login required' }
    k.commerce.loyalty.earnPoints(customerId, 100, '购物奖励')
    return { success: true }
})
```
**Parameters:**

| Parameter | Type | Description |
|------|------|------|
| customerId | `string` | Customer ID |
| points | `number` | Point amount |
| description | `string` | Description (optional) |

## redeemPoints()

Redeem points.

```ts
k.api.post(() => {
    const customerId = k.commerce.customer.currentId || ''
    if (!customerId) return { success: false, error: 'login required' }
    k.commerce.loyalty.redeemPoints(customerId, 50, '积分兑换')
    return { success: true }
})
```
**Parameters:**

| Parameter | Type | Description |
|------|------|------|
| customerId | `string` | Customer ID |
| points | `number` | Point amount |
| description | `string` | Description (optional) |

## points()

Get a customer's point records.

```ts
k.api.get(() => {
    const customerId = k.commerce.customer.currentId || ''
    if (!customerId) return { authenticated: false, list: [], count: 0 }
    return k.commerce.loyalty.points(customerId, { pageIndex: 1, pageSize: 20 })
})
```
**Parameters:**

| Parameter | Type | Description |
|------|------|------|
| customerId | `string` | Customer ID |
| query.pageIndex | `number` | Page number (optional, default 1) |
| query.pageSize | `number` | Items per page (optional, default 10) |

**Returns:** `{ list: CustomerPointModel[], count: number, pageIndex: number, pageSize: number }`

## changeMembership()

Change membership tier.

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

Renew membership.

```ts
k.api.post(() => {
    const customerId = k.commerce.customer.currentId || ''
    if (!customerId) return { success: false, error: 'login required' }
    k.commerce.loyalty.renewMembership(customerId)
})
```

These methods accept a `customerId` but do not verify caller authorization. Storefront APIs must derive it from `k.commerce.customer.currentId`; administrative APIs must enforce the appropriate admin authorization.

## memberships()

Get the list of all membership tiers.

```ts
k.api.get(() => {
    const memberships = k.commerce.loyalty.memberships()
    return { count: memberships.length }
})
```

## Type hint

If you are unsure about the fields of LoyaltyInfo / Membership, use [Loyalty Types](types/loyalty.md) for the complete type definitions.

## Boundaries

- Membership-tier list (membership.list) → [Membership Guide](membership.md)
- This guide does not cover membership-tier management details
