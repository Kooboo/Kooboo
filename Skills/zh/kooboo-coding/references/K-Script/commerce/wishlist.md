# Commerce Wishlist

## list()

获取指定客户的愿望清单列表。

```ts
k.api.get(() => {
    const customerId = k.commerce.customer.currentId || ''
    if (!customerId) return { authenticated: false, items: [] }
    return k.commerce.wishlist.list(customerId)
})
```
**参数：** `customerId: string` | **返回：** `WishlistItem[]`

站点前台 API 应从 `k.commerce.customer.currentId` 得到 `customerId`，不要接受请求中的任意顾客 ID。

## add()

添加商品到愿望清单。

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
**参数：**

| 参数 | 类型 | 说明 |
|------|------|------|
| customerId | `string` | 客户 ID |
| variantId | `string` | 商品变体 ID |

## remove()

从愿望清单移除商品。

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

`add()` 和 `remove()` 只写入存储，不返回 wishlist 项；外层 API 可以自行返回成功结果。这些底层方法不会执行调用方授权校验，公开接口必须验证当前顾客归属。

## WishlistItem 类型定义

```ts
interface WishlistItem {
    customerId: string;
    productId: string;
    variantId: string;
}
```
