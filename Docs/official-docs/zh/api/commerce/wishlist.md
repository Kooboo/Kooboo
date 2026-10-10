# wishlist

> 愿望清单

## 概述

`k.commerce.wishlist` 提供愿望清单相关操作，用于管理客户的商品收藏列表。

## list()

获取指定客户的愿望清单列表。

```ts
k.api.get(() => {
    const customer = k.commerce.customer.getByEmail("test@kooboo.com")
    return k.commerce.wishlist.list(customer.id)
})
```

**参数：**

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| customerId | `string` | 是 | 客户ID |

**返回：** [`WishlistItem[]`](#wishlistitem-属性)

## add()

添加商品到指定客户的愿望清单。

```ts
k.api.get(() => {
    const customer = k.commerce.customer.getByEmail("test@kooboo.com")
    const variantId = k.commerce.product.get("test-product").variants[0].id
    k.commerce.wishlist.add(customer.id, variantId)
    return "add success"
})
```

**参数：**

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| customerId | `string` | 是 | 客户ID |
| variantId | `string` | 是 | 商品变体ID |

**返回：** `void`

## remove()

从指定客户的愿望清单中移除商品。

```ts
k.api.get(() => {
    const customer = k.commerce.customer.getByEmail("test@kooboo.com")
    const variantId = k.commerce.product.get("test-product").variants[0].id
    k.commerce.wishlist.remove(customer.id, variantId)
    return "remove success"
})
```

**参数：**

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| customerId | `string` | 是 | 客户ID |
| variantId | `string` | 是 | 商品变体ID |

**返回：** `void`

## 结构

### WishlistItem 属性

| 属性 | 说明 | 类型 |
|------|------|------|
| customerId | 客户ID | `string` |
| productId | 商品ID | `string` |
| variantId | 变体ID | `string` |

## 相关文档

- [k.commerce](./commerce.md) - 电商模块概述
- [cart](./cart.md) - 购物车
- [order](./order.md) - 订单管理
