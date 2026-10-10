# loyalty

> Member points

## Overview

`k.commerce.loyalty` provides member points related operations, including point acquisition, redemption, membership level management, etc.

::: tip member creation
Members need to be created manually in the Kooboo backend: **E-commerce → Member (default in hidden directory) → Create member**
:::

## info()

Get the customer's membership points information.

```ts
k.api.get(() => {
    const customer = k.commerce.customer.getByEmail('test@example.com')
    return k.commerce.loyalty.info(customer.id)
})
```

**parameter:**

| Parameter | Type | Required | Description |
|------|------|------|------|
| customerId | `string` | yes | Customer ID |

**Return:** `LoyaltyInfo`

## changeMembership()

Change membership level.

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

**parameter:**

| Parameter | Type | Required | Description |
|------|------|------|------|
| customerId | `string` | yes | Customer ID |
| membershipId | `string` | yes | Member ID |

**Return:** `void`

## earnPoints()

Earn points.

```ts
k.api.post(() => {
    const customer = k.commerce.customer.getByEmail('test@example.com')
    k.commerce.loyalty.earnPoints(customer.id, 100, 'Purchase reward')
    return 'earn success'
})
```

**parameter:**

| Parameter | Type | Required | Description |
|------|------|------|------|
| customerId | `string` | yes | Customer ID |
| points | `number` | yes | Number of points earned |
| description | `string` | no | Points description |

**Return:** `void`

## redeemPoints()

Spend points.

```ts
k.api.post(() => {
    const customer = k.commerce.customer.getByEmail('test@example.com')
    k.commerce.loyalty.redeemPoints(customer.id, 50, 'Points redemption')
    return 'redeem success'
})
```

**parameter:**

| Parameter | Type | Required | Description |
|------|------|------|------|
| customerId | `string` | yes | Customer ID |
| points | `number` | yes | Amount of points spent |
| description | `string` | no | Points description |

**Return:** `void`

## points()

Get the customer's points record.

```ts
k.api.get(() => {
    const customer = k.commerce.customer.getByEmail('test@example.com')
    return k.commerce.loyalty.points(customer.id, {
        pageIndex: 1,
        pageSize: 20
    })
})
```

**parameter:**

| Parameter | Type | Required | Description |
|------|------|------|------|
| customerId | `string` | yes | Customer ID |
| query | `object` | no | query parameters |
| query.pageIndex | `number` | no | Page number, default 1 |
| query.pageSize | `number` | no | Number of items per page, default 10 |

**return:**

```ts
{
    list: CustomerPointModel[];
    count: number;
    pageIndex: number;
    pageSize: number;
}
```

## memberships()

Get a list of member information.

**Parameters**: none.

```ts
k.api.get(() => {
    const memberships = k.commerce.loyalty.memberships()
    return { count: memberships.length, items: memberships }
})
```

**Return:** `Membership[]`

## renewMembership()

Renew membership.

```ts
k.api.post(() => {
    const customer = k.commerce.customer.getByEmail('test@example.com')
    k.commerce.loyalty.renewMembership(customer.id)
    return 'renew success'
})
```

**parameter:**

| Parameter | Type | Required | Description |
|------|------|------|------|
| customerId | `string` | yes | Customer ID |

**Return:** `void`

## structure

### LoyaltyInfo Property

| Property | Description | Type |
|------|------|------|
| Points | Member points | `number` |
| MembershipStatus | Member status | [MembershipStatus](#membershipstatus-property) |

### MembershipStatus Property

| Property | Description | Type |
|------|------|------|
| Membership | Member information | [Membership](#membership-attribute) |
| StartedAt | Membership start time | `string` |
| EndAt | Membership end time | `string` |
| Active | Is the member valid? | `boolean` |

### Membership attribute

| Property | Description | Type |
|------|------|------|
| ID | Member ID | `string` |
| name | Member name | `string` |
| description | Member description | `string` |
| price | Member price | `number` |
| duration | Membership length | `number` |
| durationUnit | Member time unit | `"Year"` \| `"Month"` \| `"Day"` \| `"Week"` |
| priority | Member priority | `number` |
| condition | Upgrade conditions | [Condition](#condition-property) |
| allowAutoUpgrade | Whether to allow automatic upgrades | `boolean` |
| allowPurchase | Whether to allow purchase | `boolean` |
| customData | custom data | `{key: value}[] \| null` |
| createdAt | creation time | `string` |
| updatedAt | Update time | `string` |

### Condition property

| Property | Description | Type |
|------|------|------|
| isAny | Any condition can be met | `boolean` |
| items | List of conditional items | [ConditionItem](#conditionitem-property) |

### ConditionItem Property

| Property | Description | Type |
|------|------|------|
| option | Condition options | `string` |
| Method | conditional method | `string` |
| value | condition value | `string` |

### CustomerPointModel Property

| Property | Description | Type |
|------|------|------|
| customerId | Customer ID | `string` |
| points | integral | `number` |
| createdAt | creation time | `string` |
| Type | type | `string` |
| description | Description | `string` |
| key | key | `string` |
| disabled | Whether to disable | `boolean` |

## Related Docs

- [k.commerce](./commerce.md) - E-commerce module overview
- [customer](./customer.md) - Customer Management
- [membership](./membership.md) - Membership level
