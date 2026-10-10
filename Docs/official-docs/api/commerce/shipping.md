# shipping

> Delivery method

## Overview

`k.commerce.shipping` provides operations related to delivery methods, used to obtain delivery configurations of physical products and digital products.

## list()

Get a list of delivery methods for physical products.

```ts
k.api.get(() => {
    const shippings = k.commerce.shipping.list()
    return { count: shippings.length, items: shippings }
})
```

**parameter:**

| Parameter | Type | Required | Description |
|------|------|------|------|
| options.country | `string` | no | Filter by country |

**Return:** `ShippingItem[]`

## digitalList()

Get a list of shipping methods for digital products.

**Parameters**: none.

```ts
k.api.get(() => {
    const digitalShippings = k.commerce.shipping.digitalList()
    return { count: digitalShippings.length, items: digitalShippings }
})
```

**Return:** `DigitalShipping[]`

## structure

### ShippingItem Properties

| Property | Description | Type |
|------|------|------|
| ID | Shipping method ID | `string` |
| name | Shipping method name | `string` |
| description | Shipping method description | `string` |
| baseCost | Basic shipping fee | `number` |
| additionalCosts | Additional freight list | [AdditionalCost](#additionalcost-property) |
| isDefault | Is it the default shipping method? | `boolean` |
| countries | Supported country list | [SupportCountry](#supportcountry-property) |
| currentCost | Current shipping costs | `number` |
| code | Express company code | `string` |
| currency | currency code | `string` |
| symbol | currency symbol | `string` |

### DigitalShipping Property

| Property | Description | Type |
|------|------|------|
| ID | Shipping method ID | `string` |
| name | Shipping method name | `string` |
| description | Shipping method description | `string` |
| mailServerType | Mail server type | `string` |
| koobooEmailAddress | Kooboo email address | `string` |
| customMailServer | Custom mail server | [MailServer](./settings.md#mailserver-properties) |
| mailTemplate | Email template | `{ subject: string, body: string }` |
| isDefault | Is it the default shipping method? | `boolean` |
| createdAt | creation time | `Date` |
| updatedAt | Update time | `Date` |

### AdditionalCost Property

| Property | Description | Type |
|------|------|------|
| cost | Additional shipping amount | `number` |
| description | Additional shipping description | `string` |
| condition | Additional shipping conditions | [Condition](#condition-property) |

### SupportCountry Property

| Property | Description | Type |
|------|------|------|
| name | country code | `string` |
| display | Country display name | `string` |
| estimatedDaysOfArrival | Estimated days of arrival | `number` |

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
- [cart](./cart.md) - Shopping Cart
- [order](./order.md) - Order Management
