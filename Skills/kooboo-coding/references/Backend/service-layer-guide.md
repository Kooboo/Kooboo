# Service Layer Guide

## Role

The Service layer is the core of business logic and is responsible for:

- Data retrieval and processing
- Business-rule implementation
- Cross-module coordination
- Error handling and boundary checks

The API layer only obtains parameters and returns results; it does not contain business logic.

## Naming convention

Use a dotted folder system for CodeBlock Name:


| Prefix | Purpose | Examples |
| ----------- | ---- | ------------------------------------------------------ |
| `Services.` | Business services | `Services.auth`, `Services.product`, `Services.order` |
| `Utils.` | General utilities | `Utils.formatDate`, `Utils.validation`, `Utils.response` |


## CodeBlock organization

One functional module corresponds to one CodeBlock file:

```
Services.product    // 产品相关服务
├── listProducts()   // 导出函数
├── getProduct()     // 导出函数
└── searchProducts() // 导出函数
```

```ts
// Services.product (CodeBlock)

export function listProducts(query) {
    const products = k.commerce.product.list(query)
    return products
}

export function getProduct(id) {
    try {
        return k.commerce.product.get(id)
    } catch (e) {
        return null
    }
}
```

## Import pattern

The Service layer may import other Services or Utils:

```ts
// Services.order 引用 Services.product
import { getProduct } from "./Services.product"
import { formatDate } from "./Utils.formatDate"

export function createOrder(items) {
    const product = getProduct(items[0].productId)
    return {
        items,
        createdAt: formatDate(new Date())
    }
}
```

Use a relative path for imports, beginning with `.`:

- Reference: `./Services.product`

## Error handling

The Service layer catches and handles errors:

```ts
export function getProduct(id) {
    try {
        const product = k.commerce.product.get(id)
        if (!product) {
            return { error: "Product not found" }
        }
        return product
    } catch (e) {
        return { error: e.message }
    }
}
```

## Data transformation

Complete data transformation in the Service layer to keep the API layer concise:

```ts
// Services.product

export function listProducts(query) {
    const products = k.commerce.product.list(query)
    return products.map(p => ({
        id: p.id,
        name: p.name,
        price: p.price,
        image: p.images?.[0]?.url
    }))
}
```

## Composition pattern

Complex business logic may require multiple Services to collaborate:

```ts
// Services.checkout

import { getCart } from "./Services.cart"
import { calculateDiscount } from "./Services.discount"
import { createOrder } from "./Services.order"

export function checkout(cartId, customerId) {
    const cart = getCart(cartId)
    if (!cart || cart.items.length === 0) {
        return { error: "Cart is empty" }
    }

    const discount = calculateDiscount(cart, customerId)
    const order = createOrder({
        cartId,
        customerId,
        discount
    })

    return { order, discount }
}
```

## Notes

1. **Do not return HTTP responses directly from the Service layer**; return data only.
2. **Keep functions single-purpose**; each function should do one thing.
3. **Avoid circular dependencies**: when Service A references Service B, do not make Service B reference Service A as well.
4. **Handle errors consistently** with a returned `{ error }` object instead of throwing exceptions.
5. **Use `k.logger` for debug output**; do not use `console.log` / `console.error` (Kooboo server does not support console methods).
6. **Logic called by an API must use `export function`**; the API references it through `import { ... } from "./Services.xxx"`. **There is no** syntax such as `k.req.services` for reflecting CodeBlock calls from a request object.
