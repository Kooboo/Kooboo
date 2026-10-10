# wishlist

> wish list

## Overview

`k.commerce.wishlist` provides wish list related operations for managing customers' product collection lists.

## list()

Gets a list of the specified customer's wish lists.

```ts
k.api.get(() => {
    const customer = k.commerce.customer.getByEmail("test@kooboo.com")
    return k.commerce.wishlist.list(customer.id)
})
```

**parameter:**

| Parameter | Type | Required | Description |
|------|------|------|------|
| customerId | `string` | yes | Customer ID |

**Return:** [WishlistItem](#wishlistitem-property)[]

## add()

Add an item to a specified customer's wish list.

```ts
k.api.get(() => {
    const customer = k.commerce.customer.getByEmail("test@kooboo.com")
    const variantId = k.commerce.product.get("test-product").variants[0].id
    k.commerce.wishlist.add(customer.id, variantId)
    return "add success"
})
```

**parameter:**

| Parameter | Type | Required | Description |
|------|------|------|------|
| customerId | `string` | yes | Customer ID |
| variantId | `string` | yes | Product variant ID |

**Return:** `void`

## remove()

Removes an item from the specified customer's wish list.

```ts
k.api.get(() => {
    const customer = k.commerce.customer.getByEmail("test@kooboo.com")
    const variantId = k.commerce.product.get("test-product").variants[0].id
    k.commerce.wishlist.remove(customer.id, variantId)
    return "remove success"
})
```

**parameter:**

| Parameter | Type | Required | Description |
|------|------|------|------|
| customerId | `string` | yes | Customer ID |
| variantId | `string` | yes | Product variant ID |

**Return:** `void`

## structure

### WishlistItem Property

| Property | Description | Type |
|------|------|------|
| customerId | Customer ID | `string` |
| productId | Product ID | `string` |
| variantId | Variant ID | `string` |

## Related Docs

- [k.commerce](./commerce.md) - E-commerce module overview
- [cart](./cart.md) - Shopping Cart
- [order](./order.md) - Order Management
