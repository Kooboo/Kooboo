# Commerce Wishlist

## list()

Get the wishlist for a specified customer.

```ts
k.api.get(() => {
    const customerId = k.commerce.customer.currentId || ''
    if (!customerId) return { authenticated: false, items: [] }
    return k.commerce.wishlist.list(customerId)
})
```
**Parameter:** `customerId: string` | **Returns:** `WishlistItem[]`

For storefront APIs, derive `customerId` from `k.commerce.customer.currentId`; do not accept an arbitrary customer ID from the request.

## add()

Add a product to the wishlist.

```ts
k.api.post(() => {
    const customerId = k.commerce.customer.currentId || ''
    if (!customerId) return { success: false, error: 'login required' }
    // 仅适用于确认只有一个默认变体的商品；多变体商品须先匹配用户选项。
    const variantId = k.commerce.product.get("test-product").variants[0].id
    k.commerce.wishlist.add(customerId, variantId)
    return { success: true }
})
```
**Parameters:**

| Parameter | Type | Description |
|------|------|------|
| customerId | `string` | Customer ID |
| variantId | `string` | Product variant ID |

## remove()

Remove a product from the wishlist.

```ts
k.api.post(() => {
    const customerId = k.commerce.customer.currentId || ''
    if (!customerId) return { success: false, error: 'login required' }
    // 仅适用于确认只有一个默认变体的商品；多变体商品须先匹配用户选项。
    const variantId = k.commerce.product.get("test-product").variants[0].id
    k.commerce.wishlist.remove(customerId, variantId)
    return { success: true }
})
```

`add()` and `remove()` update storage and return no wishlist item; the enclosing API can return its own success response. These low-level methods do not verify caller authorization, so public endpoints must validate the current customer's ownership.

## WishlistItem type definition

```ts
interface WishlistItem {
    customerId: string;
    productId: string;
    variantId: string;
}
```
