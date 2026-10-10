# discount

> Discounts

## Overview

`k.commerce.discount` provides discount coupon related operations for obtaining and managing coupon information.

## list()

Get a list of discounts.

**Parameters**: none.

```ts
k.api.get(() => {
    const discounts = k.commerce.discount.list()
    return { count: discounts.length, items: discounts.slice(0, 3) }
})
```

**Return:** `Discount[]`

## structure

### Discount property

| Property | Description | Type |
|------|------|------|
| ID | Discount ID | `string` |
| code | Offer discount code | `string` |
| title | Discount name | `string` |
| startDate | Discount start time | `string` |
| endDate | Discount end time | `string` |
| condition | Discount conditions | [DiscountCondition](#discountcondition-property) |
| Method | Discount methods | `"DiscountCode"` \| `"AutomaticDiscount"` |
| Type | Offer discount type | `"ProductAmountOff"` \| `"OrderAmountOff"` \| `"FreeShipping"` |
| value | Discount value | `number` |
| isPercent | Is it a percentage discount? | `boolean` |
| priority | Discount priority | `number` |
| isExclusion | Are they mutually exclusive? | `boolean` |
| codeUsageLimit | Code usage restrictions | `number\| null` |
| createdAt | creation time | `string` |
| updatedAt | Update time | `string` |

### DiscountCondition Property

| Property | Description | Type |
|------|------|------|
| isAny | Any one of the conditions need to be met, otherwise all conditions need to be met | `boolean` |
| items | List of conditional items | [ConditionItem](#conditionitem-property) |

### ConditionItem Property

| Property | Description | Type |
|------|------|------|
| option | Condition options | `string` |
| Method | conditional method | `string` |
| value | condition value | `string` |

## Related Docs

- [k.commerce](./commerce.md) - E-commerce module overview
- [cart](./cart.md) - Shopping Cart
- [order](./order.md) - Order Management
