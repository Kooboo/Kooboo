# membership

> Membership level

## Overview

`k.commerce.membership` provides membership level related operations and is used to obtain membership level information.

::: tip membership level creation
Membership levels need to be created manually in the Kooboo backend: **E-commerce → Member (default in hidden directory) → Create member**
:::

## list()

Get a list of membership levels.

**Parameters**: none.

```ts
k.api.get(() => {
    const memberships = k.commerce.membership.list()
    return { count: memberships.length, items: memberships }
})
```

**Return:** `Membership[]`

## get()

Get membership level information based on member ID.

```ts
k.api.get(() => {
    const memberships = k.commerce.membership.list()
    if (memberships && memberships.length > 0) {
        return k.commerce.membership.get(memberships[0].id)
    }
    return { error: 'no membership' }
})
```

**parameter:**

| Parameter | Type | Required | Description |
|------|------|------|------|
| ID | `string` | yes | Member ID |

**Return:** `Membership`

## structure

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

## Related Docs

- [k.commerce](./commerce.md) - E-commerce module overview
- [loyalty](./loyalty.md) - Member Points
- [customer](./customer.md) - Customer Management
